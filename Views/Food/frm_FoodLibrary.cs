using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Models;

namespace HealthApp.Views.Food
{
    public partial class frm_FoodLibrary : Form
    {
        private List<ThuVienMonAn> _danhSachMonAn; // Danh sách đã filter
        private List<ThuVienMonAn> _danhSachMonAnDayDu; // Danh sách đầy đủ ban đầu
        private List<UserControlFood> _listUserControlFood;
        private string _loaiHienTai = null; // Loại đang được chọn
        private string _tuKhoaTimKiem = ""; // Từ khóa tìm kiếm hiện tại
        
        // Phân trang
        private int _trangHienTai = 0; // Trang hiện tại (bắt đầu từ 0)
        private const int _soLuongMoiTrang = 4; // Số món ăn mỗi trang (2x2 = 4)
        private bool _daKhoiTaoUserControls = false; // Đánh dấu đã khởi tạo UserControls chưa
        
        // Danh sách món ăn đã chọn (chỉ lưu trong memory, hiển thị qua controls có sẵn)
        private List<MonAnDaChonInfo> _danhSachMonAnDaChon;
        
        // Danh sách các sets controls (Set 1 là controls có sẵn, Set 2+ là controls động)
        private List<SetControlsMonAn> _danhSachSetsControls;
        
        /// <summary>
        /// Class để lưu thông tin món ăn đã chọn
        /// </summary>
        private class MonAnDaChonInfo
        {
            public ThuVienMonAn MonAn { get; set; }
            public double TrongLuong { get; set; }
        }
        
        /// <summary>
        /// Class để lưu một set controls cho mỗi món ăn
        /// </summary>
        private class SetControlsMonAn
        {
            public Label LblTen { get; set; }
            public Label LblDonVi { get; set; }
            public Guna.UI2.WinForms.Guna2TextBox TxtTrongLuong { get; set; }
            public Guna.UI2.WinForms.Guna2CircleButton BtnXoa { get; set; }
            public int Index { get; set; } // Index của set (1, 2, 3, ...)
            public bool IsDynamicCreated { get; set; } // Đánh dấu là controls được tạo động hay có sẵn
        }

        public frm_FoodLibrary()
        {
            InitializeComponent();
            _danhSachMonAn = new List<ThuVienMonAn>();
            _danhSachMonAnDayDu = new List<ThuVienMonAn>();
            _listUserControlFood = new List<UserControlFood>();
            _danhSachMonAnDaChon = new List<MonAnDaChonInfo>();
            _danhSachSetsControls = new List<SetControlsMonAn>();
            
            // Khởi tạo Set 1 từ controls có sẵn
            KhoiTaoSetControlsDauTien();
            
            // Kết nối events cho các controls có sẵn
            ConnectEvents();
            
            // Đảm bảo panel có background màu trắng
            if (pnlDanhSachMonAn != null)
            {
                pnlDanhSachMonAn.FillColor = Color.White;
                pnlDanhSachMonAn.FillColor2 = Color.White;
                pnlDanhSachMonAn.AutoScroll = true;
            }
            
            // Điều chỉnh vị trí của guna2Panl1 để có thể cuộn được lên trên
            if (guna2Panl1 != null)
            {
                guna2Panl1.AutoScroll = true;
            }
        }

        private async void FoodLibrary_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== FoodLibrary_Load BẮT ĐẦU ===");
            System.Diagnostics.Debug.WriteLine($"Panel pnlDanhSachMonAn: {(pnlDanhSachMonAn != null ? "NOT NULL" : "NULL")}");
            if (pnlDanhSachMonAn != null)
            {
                System.Diagnostics.Debug.WriteLine($"Panel Size: {pnlDanhSachMonAn.Size}, Location: {pnlDanhSachMonAn.Location}");
                System.Diagnostics.Debug.WriteLine($"Panel Visible: {pnlDanhSachMonAn.Visible}, Enabled: {pnlDanhSachMonAn.Enabled}");
                System.Diagnostics.Debug.WriteLine($"Panel Controls Count: {pnlDanhSachMonAn.Controls.Count}");
            }

            await LoadFoodsFromDatabase();
            
            // Khởi tạo 4 UserControls một lần (tái sử dụng)
            InitializeUserControls();
            
            // Thiết lập button "Tất cả" được chọn ban đầu
            SetButtonSelected(btnTatCa);
            
            DisplayFoods();
            System.Diagnostics.Debug.WriteLine("=== FoodLibrary_Load KẾT THÚC ===");
        }

        /// <summary>
        /// Load dữ liệu bằng raw SQL connection (fallback khi EF fail)
        /// </summary>
        private List<ThuVienMonAn> LoadFoodsUsingRawSQL(string connectionString)
        {
            var list = new List<ThuVienMonAn>();
            
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Raw SQL connection đã mở");
                    
                    // Thử query không có schema
                    string sql = "SELECT MonAnID, imageURL, TenMonAn, Loai, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber, NgayTao FROM ThuVienMonAn";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var monAn = new ThuVienMonAn
                                {
                                    MonAnID = reader["MonAnID"]?.ToString(),
                                    imageURL = reader["imageURL"]?.ToString(),
                                    TenMonAn = reader["TenMonAn"]?.ToString(),
                                    Loai = reader["Loai"]?.ToString(),
                                    Donvi = reader["Donvi"]?.ToString(),
                                    KhoiLuongChuan = reader["KhoiLuongChuan"] as double?,
                                    Calories = reader["Calories"] as double?,
                                    Protein = reader["Protein"] as double?,
                                    Carbs = reader["Carbs"] as double?,
                                    Fat = reader["Fat"] as double?,
                                    Fiber = reader["Fiber"] as double?,
                                    NgayTao = reader["NgayTao"] as DateTime?
                                };
                                list.Add(monAn);
                            }
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Đã load {list.Count} món ăn bằng raw SQL");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load bằng raw SQL: {ex.Message}");
                throw;
            }
            
            return list;
        }

        /// <summary>
        /// Load dữ liệu món ăn từ database
        /// </summary>
        private async Task LoadFoodsFromDatabase()
        {
            System.Diagnostics.Debug.WriteLine("=== LoadFoodsFromDatabase BẮT ĐẦU ===");
            try
            {
                // Load dữ liệu trên background thread để không block UI
                _danhSachMonAn = await Task.Run(() =>
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Đang kết nối database...");
                        using (var db = new WF_HealthTracker())
                        {
                            System.Diagnostics.Debug.WriteLine("Database context đã tạo");
                            
                            // Test connection trước
                            if (!db.Database.Exists())
                            {
                                throw new Exception("Database không tồn tại!");
                            }
                            
                            System.Diagnostics.Debug.WriteLine("Database tồn tại, đang query...");
                            
                            // Thử load bằng Entity Framework trước
                            List<ThuVienMonAn> list = null;
                            try
                            {
                                list = db.ThuVienMonAn.ToList();
                                System.Diagnostics.Debug.WriteLine($"Đã load {list.Count} món ăn từ database bằng EF");
                            }
                            catch (Exception efEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Lỗi load bằng EF: {efEx.Message}");
                                
                                // Log inner exception chi tiết
                                Exception inner = efEx;
                                int depth = 0;
                                while (inner.InnerException != null && depth < 5)
                                {
                                    inner = inner.InnerException;
                                    System.Diagnostics.Debug.WriteLine($"  Inner exception (depth {depth}): {inner.Message}");
                                    depth++;
                                }
                                
                                // Nếu EF fail, thử dùng raw SQL query trực tiếp
                                System.Diagnostics.Debug.WriteLine("Thử load bằng raw SQL query...");
                                try
                                {
                                    // Kiểm tra table có tồn tại không
                                    try
                                    {
                                        var tableCheck = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ThuVienMonAn'").FirstOrDefault();
                                        System.Diagnostics.Debug.WriteLine($"Table check result: {tableCheck}");
                                        
                                        if (tableCheck == 0)
                                        {
                                            throw new Exception("Table 'ThuVienMonAn' không tồn tại trong database!");
                                        }
                                    }
                                    catch (Exception checkEx)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Lỗi kiểm tra table: {checkEx.Message}");
                                    }
                                    
                                    // Thử query trực tiếp bằng SQL - thử nhiều cách
                                    string[] sqlQueries = new string[]
                                    {
                                        "SELECT MonAnID, imageURL, TenMonAn, Loai, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber, NgayTao FROM ThuVienMonAn",
                                        "SELECT MonAnID, imageURL, TenMonAn, Loai, Donvi, KhoiLuongChuan, Calories, Protein, Carbs, Fat, Fiber, NgayTao FROM dbo.ThuVienMonAn",
                                        "SELECT * FROM ThuVienMonAn"
                                    };
                                    
                                    bool success = false;
                                    foreach (var sql in sqlQueries)
                                    {
                                        try
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Thử query: {sql}");
                                            list = db.Database.SqlQuery<ThuVienMonAn>(sql).ToList();
                                            System.Diagnostics.Debug.WriteLine($"Đã load {list.Count} món ăn bằng raw SQL query");
                                            success = true;
                                            break;
                                        }
                                        catch (Exception sqlEx)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"  Query fail: {sqlEx.Message}");
                                            continue;
                                        }
                                    }
                                    
                                    if (!success)
                                    {
                                        throw new Exception("Tất cả các cách query đều fail!");
                                    }
                                }
                                catch (Exception sqlEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Lỗi load bằng raw SQL: {sqlEx.Message}");
                                    // Re-throw với thông tin đầy đủ
                                    throw new Exception($"Không thể load dữ liệu. EF Error: {efEx.Message}, SQL Error: {sqlEx.Message}", efEx);
                                }
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"Tổng cộng: {list.Count} món ăn");
                            
                            // Log chi tiết từng món ăn
                            foreach (var monAn in list)
                            {
                                System.Diagnostics.Debug.WriteLine($"  - {monAn.MonAnID}: {monAn.TenMonAn} ({monAn.Loai})");
                            }
                            
                            return list;
                        }
                    }
                    catch (Exception dbEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi trong Task.Run: {dbEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {dbEx.StackTrace}");
                        throw;
                    }
                });
                
                // Lưu danh sách đầy đủ
                _danhSachMonAnDayDu = new List<ThuVienMonAn>(_danhSachMonAn);
                
                System.Diagnostics.Debug.WriteLine($"Sau khi load: _danhSachMonAn có {(_danhSachMonAn?.Count ?? 0)} items");
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                string innerMessage = "";
                
                // Kiểm tra InnerException
                Exception currentEx = ex;
                int depth = 0;
                while (currentEx.InnerException != null && depth < 5)
                {
                    currentEx = currentEx.InnerException;
                    innerMessage = currentEx.Message;
                    System.Diagnostics.Debug.WriteLine($"Inner exception (depth {depth}): {innerMessage}");
                    depth++;
                    
                    // Nếu là SqlException, lấy message chi tiết hơn
                    if (currentEx is System.Data.SqlClient.SqlException sqlEx)
                    {
                        errorMessage = $"SQL Error: {sqlEx.Message}";
                        System.Diagnostics.Debug.WriteLine($"SQL Error Number: {sqlEx.Number}");
                        
                        // Kiểm tra lỗi login
                        if (sqlEx.Message.Contains("Login failed") || sqlEx.Number == 18456)
                        {
                            errorMessage = "Lỗi đăng nhập SQL Server!\n\n" +
                                "Nguyên nhân có thể:\n" +
                                "1. Password sai\n" +
                                "2. User 'sa' bị disable\n" +
                                "3. SQL Server không cho phép SQL Authentication\n\n" +
                                "Giải pháp:\n" +
                                "- Kiểm tra password trong App.config\n" +
                                "- Hoặc đổi sang Windows Authentication trong connection string";
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Lỗi load database: {errorMessage}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Hiển thị lỗi trên UI thread - KHÔNG block UI
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show($"Lỗi khi load dữ liệu từ database:\n\n{errorMessage}\n\n{(string.IsNullOrEmpty(innerMessage) ? "" : $"Chi tiết: {innerMessage}")}\n\nVui lòng kiểm tra:\n1. Database có tồn tại không\n2. Bảng ThuVienMonAn có tồn tại không\n3. Connection string có đúng không",
                            "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    MessageBox.Show($"Lỗi khi load dữ liệu từ database:\n\n{errorMessage}\n\n{(string.IsNullOrEmpty(innerMessage) ? "" : $"Chi tiết: {innerMessage}")}\n\nVui lòng kiểm tra:\n1. Database có tồn tại không\n2. Bảng ThuVienMonAn có tồn tại không\n3. Connection string có đúng không",
                        "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                // Set danh sách rỗng để không crash
                _danhSachMonAn = new List<ThuVienMonAn>();
            }
            System.Diagnostics.Debug.WriteLine("=== LoadFoodsFromDatabase KẾT THÚC ===");
        }

        /// <summary>
        /// Khởi tạo 4 UserControls một lần để tái sử dụng (không phải tạo mới mỗi lần chuyển trang)
        /// </summary>
        private void InitializeUserControls()
        {
            if (_daKhoiTaoUserControls) return; // Đã khởi tạo rồi, không cần làm lại

            System.Diagnostics.Debug.WriteLine("=== InitializeUserControls BẮT ĐẦU ===");

            // Kiểm tra panel
            if (pnlDanhSachMonAn == null)
            {
                System.Diagnostics.Debug.WriteLine("LỖI: pnlDanhSachMonAn là NULL!");
                return;
            }

            // Kích thước và khoảng cách
            int itemWidth = 527;  // Width của UserControlFood
            int itemHeight = 256; // Height của UserControlFood
            int padding = 20;     // Khoảng cách giữa các items
            int itemsPerRow = 2;  // Số items mỗi hàng (2x2)
            int startY = 70;      // Bắt đầu từ dưới label "Danh sách món ăn"

            // Tạo 4 UserControls một lần
            for (int i = 0; i < _soLuongMoiTrang; i++)
            {
                var ucFood = new UserControlFood();
                
                // Tính toán vị trí (layout 2x2)
                int row = i / itemsPerRow;
                int col = i % itemsPerRow;
                
                int x = padding + col * (itemWidth + padding);
                int y = startY + row * (itemHeight + padding);

                ucFood.Location = new Point(x, y);
                ucFood.Size = new Size(itemWidth, itemHeight);
                ucFood.Visible = false; // Ẩn ban đầu, sẽ hiện khi có dữ liệu
                ucFood.BackColor = Color.Transparent;

                // Thêm vào panel và list
                pnlDanhSachMonAn.Controls.Add(ucFood);
                _listUserControlFood.Add(ucFood);
                ucFood.BringToFront();
                
                System.Diagnostics.Debug.WriteLine($"Đã tạo UserControlFood {i + 1} tại vị trí ({x}, {y})");
            }

            _daKhoiTaoUserControls = true;
            System.Diagnostics.Debug.WriteLine("=== InitializeUserControls KẾT THÚC ===");
        }

        /// <summary>
        /// Hiển thị danh sách món ăn trong pnlDanhSachMonAn với layout 2x2 (Tái sử dụng UserControls đã có)
        /// </summary>
        private void DisplayFoods()
        {
            System.Diagnostics.Debug.WriteLine("=== DisplayFoods BẮT ĐẦU ===");
            
            // Đảm bảo chạy trên UI thread
            if (this.InvokeRequired)
            {
                System.Diagnostics.Debug.WriteLine("DisplayFoods: InvokeRequired = true, gọi Invoke");
                this.Invoke(new Action(DisplayFoods));
                return;
            }

            System.Diagnostics.Debug.WriteLine("DisplayFoods: Đang chạy trên UI thread");

            // Kiểm tra panel
            if (pnlDanhSachMonAn == null)
            {
                System.Diagnostics.Debug.WriteLine("LỖI: pnlDanhSachMonAn là NULL!");
                MessageBox.Show("Panel pnlDanhSachMonAn không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Đảm bảo UserControls đã được khởi tạo
            if (!_daKhoiTaoUserControls)
            {
                InitializeUserControls();
            }

            if (_danhSachMonAn == null || _danhSachMonAn.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Không có dữ liệu món ăn để hiển thị");
                System.Diagnostics.Debug.WriteLine($"_danhSachMonAn == null: {_danhSachMonAn == null}");
                System.Diagnostics.Debug.WriteLine($"_danhSachMonAn.Count: {(_danhSachMonAn?.Count ?? 0)}");
                
                // Ẩn tất cả UserControls nếu không có dữ liệu
            foreach (var uc in _listUserControlFood)
            {
                if (uc != null && !uc.IsDisposed)
                {
                        uc.Visible = false;
                }
            }

                if (_danhSachMonAn != null && _danhSachMonAn.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu món ăn trong database.\n\nVui lòng chạy file SampleData_ThuVienMonAn.sql để thêm dữ liệu mẫu.", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                UpdatePaginationUI(0, 0);
                return;
            }

            // Tính toán phân trang
            int tongSoMonAn = _danhSachMonAn.Count;
            int tongSoTrang = (int)Math.Ceiling((double)tongSoMonAn / _soLuongMoiTrang);
            
            // Đảm bảo _trangHienTai hợp lệ
            if (_trangHienTai >= tongSoTrang && tongSoTrang > 0)
            {
                _trangHienTai = tongSoTrang - 1;
            }
            if (_trangHienTai < 0)
            {
                _trangHienTai = 0;
            }

            // Tính toán vị trí bắt đầu và kết thúc của trang hiện tại
            int startIndex = _trangHienTai * _soLuongMoiTrang;
            int endIndex = Math.Min(startIndex + _soLuongMoiTrang, tongSoMonAn);
            int soMonAnTrongTrang = endIndex - startIndex;

            System.Diagnostics.Debug.WriteLine($"=== PHÂN TRANG ===");
            System.Diagnostics.Debug.WriteLine($"Tổng số món ăn: {tongSoMonAn}");
            System.Diagnostics.Debug.WriteLine($"Tổng số trang: {tongSoTrang}");
            System.Diagnostics.Debug.WriteLine($"Trang hiện tại: {_trangHienTai + 1}/{tongSoTrang}");
            System.Diagnostics.Debug.WriteLine($"Hiển thị món ăn từ {startIndex + 1} đến {endIndex}");
            System.Diagnostics.Debug.WriteLine($"Bắt đầu hiển thị {soMonAnTrongTrang} món ăn trên trang {_trangHienTai + 1}");

            // Kích thước và khoảng cách
            int itemWidth = 527;  // Width của UserControlFood
            int itemHeight = 256; // Height của UserControlFood
            int padding = 20;     // Khoảng cách giữa các items
            int itemsPerRow = 2;  // Số items mỗi hàng (2x2)
            int startY = 70;      // Bắt đầu từ dưới label "Danh sách món ăn" (tăng lên để tránh bị che)

            // Đảm bảo panel có thể scroll và có background
            pnlDanhSachMonAn.AutoScroll = false; // Tắt AutoScroll vì chỉ hiển thị 4 món
            pnlDanhSachMonAn.FillColor = Color.White;
            pnlDanhSachMonAn.FillColor2 = Color.White;

            System.Diagnostics.Debug.WriteLine($"Panel settings: AutoScroll={pnlDanhSachMonAn.AutoScroll}, Size={pnlDanhSachMonAn.Size}");

            // Cập nhật dữ liệu cho các UserControls đã có (KHÔNG tạo mới)
            int indexTrongTrang = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (indexTrongTrang >= _listUserControlFood.Count)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi: Không đủ UserControls! Cần {indexTrongTrang + 1} nhưng chỉ có {_listUserControlFood.Count}");
                    break;
                }

                var monAn = _danhSachMonAn[i];
                var ucFood = _listUserControlFood[indexTrongTrang];
                
                System.Diagnostics.Debug.WriteLine($"Cập nhật UserControlFood {indexTrongTrang + 1} cho: {monAn.TenMonAn}");
                
                // Chỉ cập nhật dữ liệu (KHÔNG tạo mới) - Đây là điểm tối ưu chính!
                ucFood.SetData(monAn);

                // Kết nối event handler cho nút "Thêm" trong UserControlFood
                ConnectThemButtonEvent(ucFood, monAn);
                
                // Hiển thị control
                ucFood.Visible = true;
                
                // Set Z-order: Đưa UserControlFood lên trên cùng
                ucFood.BringToFront();
                
                indexTrongTrang++;
                
                System.Diagnostics.Debug.WriteLine($"Đã cập nhật UserControlFood {indexTrongTrang}: {monAn.TenMonAn}");
            }

            // Ẩn các UserControls không được sử dụng (nếu trang có ít hơn 4 món)
            for (int i = indexTrongTrang; i < _listUserControlFood.Count; i++)
            {
                if (_listUserControlFood[i] != null && !_listUserControlFood[i].IsDisposed)
                {
                    _listUserControlFood[i].Visible = false;
                    System.Diagnostics.Debug.WriteLine($"Ẩn UserControlFood {i + 1} (không được sử dụng)");
                }
            }

            // Cập nhật thông tin trang và trạng thái nút
            UpdatePaginationUI(tongSoMonAn, tongSoTrang);

            // Đảm bảo label và picture ở dưới cùng (không che UserControlFood)
            if (lblDanhSachMonAn != null)
            {
                lblDanhSachMonAn.SendToBack();
            }
            if (ptrDanhSachMonAn != null)
            {
                ptrDanhSachMonAn.SendToBack();
            }

            // Tối ưu: Chỉ refresh panel một lần thay vì cả form
            // Sử dụng SuspendLayout/ResumeLayout để tránh flickering
            pnlDanhSachMonAn.SuspendLayout();
            try
            {
            pnlDanhSachMonAn.Refresh();
            }
            finally
            {
                pnlDanhSachMonAn.ResumeLayout(false);
            }
            
            System.Diagnostics.Debug.WriteLine($"=== KẾT QUẢ ===");
            System.Diagnostics.Debug.WriteLine($"Đã hiển thị {soMonAnTrongTrang} món ăn trên trang {_trangHienTai + 1}");
            System.Diagnostics.Debug.WriteLine($"Sử dụng {indexTrongTrang} UserControls từ {_listUserControlFood.Count} controls đã tạo");
            System.Diagnostics.Debug.WriteLine($"Panel Controls count: {pnlDanhSachMonAn.Controls.Count}");
            System.Diagnostics.Debug.WriteLine("=== DisplayFoods KẾT THÚC ===");
        }

        /// <summary>
        /// Cập nhật UI phân trang (nút Previous/Next và label thông tin trang)
        /// </summary>
        private void UpdatePaginationUI(int tongSoMonAn, int tongSoTrang)
        {
            if (tongSoTrang <= 0)
            {
                // Không có món ăn
                if (btnPrevious != null) btnPrevious.Enabled = false;
                if (btnNext != null) btnNext.Enabled = false;
                if (lblThongTinTrang != null) lblThongTinTrang.Text = "Không có món ăn";
                return;
            }

            // Cập nhật trạng thái nút Previous
            if (btnPrevious != null)
            {
                btnPrevious.Enabled = _trangHienTai > 0;
            }

            // Cập nhật trạng thái nút Next
            if (btnNext != null)
            {
                btnNext.Enabled = _trangHienTai < tongSoTrang - 1;
            }

            // Cập nhật label thông tin trang
            if (lblThongTinTrang != null)
            {
                int startIndex = _trangHienTai * _soLuongMoiTrang + 1;
                int endIndex = Math.Min((_trangHienTai + 1) * _soLuongMoiTrang, tongSoMonAn);
                
                if (tongSoMonAn == 0)
                {
                    lblThongTinTrang.Text = "Không có món ăn";
                }
                else if (tongSoTrang == 1)
                {
                    lblThongTinTrang.Text = $"Hiển thị tất cả {tongSoMonAn} món ăn";
                }
                else
                {
                    lblThongTinTrang.Text = $"Trang {_trangHienTai + 1}/{tongSoTrang} - Hiển thị {startIndex}-{endIndex} trong tổng số {tongSoMonAn} món ăn";
                }
            }
        }

        /// <summary>
        /// Lọc danh sách món ăn theo loại và từ khóa tìm kiếm
        /// </summary>
        private void FilterFoods(string loai = null, string searchTerm = null)
        {
            _loaiHienTai = loai;
            _tuKhoaTimKiem = searchTerm ?? "";

            // Reset về trang đầu khi filter/search
            _trangHienTai = 0;

            // Bắt đầu từ danh sách đầy đủ
            var filtered = _danhSachMonAnDayDu.AsEnumerable();

            // Lọc theo loại
            if (!string.IsNullOrEmpty(loai))
            {
                filtered = filtered.Where(f => 
                    !string.IsNullOrEmpty(f.Loai) && 
                    f.Loai.Equals(loai, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                filtered = filtered.Where(f =>
                    (!string.IsNullOrEmpty(f.TenMonAn) && f.TenMonAn.ToLower().Contains(term)) ||
                    (!string.IsNullOrEmpty(f.Loai) && f.Loai.ToLower().Contains(term)));
            }

            _danhSachMonAn = filtered.ToList();
            DisplayFoods();
        }

        /// <summary>
        /// Reset trạng thái button (màu nền)
        /// </summary>
        private void ResetButtonStates()
        {
            btnTatCa.FillColor = Color.White;
            btnTatCa.FillColor2 = Color.White;
            btnTatCa.ForeColor = Color.Black;
            
            btnThit.FillColor = Color.White;
            btnThit.FillColor2 = Color.White;
            btnThit.ForeColor = Color.Black;
            
            btnRauCu.FillColor = Color.White;
            btnRauCu.FillColor2 = Color.White;
            btnRauCu.ForeColor = Color.Black;
            
            btnTraiCay.FillColor = Color.White;
            btnTraiCay.FillColor2 = Color.White;
            btnTraiCay.ForeColor = Color.Black;
            
            btnHaiSan.FillColor = Color.White;
            btnHaiSan.FillColor2 = Color.White;
            btnHaiSan.ForeColor = Color.Black;
        }

        /// <summary>
        /// Đánh dấu button được chọn
        /// </summary>
        private void SetButtonSelected(Guna.UI2.WinForms.Guna2GradientButton button)
        {
            ResetButtonStates();
            button.FillColor = Color.DeepSkyBlue;
            button.FillColor2 = Color.DeepSkyBlue;
            button.ForeColor = Color.White;
        }

        private void btnTatCa_Click(object sender, EventArgs e)
        {
            SetButtonSelected(btnTatCa);
            FilterFoods(null, _tuKhoaTimKiem);
        }

        private void btnThit_Click(object sender, EventArgs e)
        {
            SetButtonSelected(btnThit);
            FilterFoods("Thịt", _tuKhoaTimKiem);
        }

        private void btnRauCu_Click(object sender, EventArgs e)
        {
            SetButtonSelected(btnRauCu);
            FilterFoods("Rau củ", _tuKhoaTimKiem);
        }

        private void btnTraiCay_Click(object sender, EventArgs e)
        {
            SetButtonSelected(btnTraiCay);
            FilterFoods("Trái cây", _tuKhoaTimKiem);
        }

        private void btnHaiSan_Click(object sender, EventArgs e)
        {
            SetButtonSelected(btnHaiSan);
            FilterFoods("Hải sản", _tuKhoaTimKiem);
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Lấy text và kiểm tra xem có phải placeholder text không
            string text = txtTimKiem.Text?.Trim() ?? "";
            string placeholder = txtTimKiem.PlaceholderText ?? "";
            
            // Nếu text giống với placeholder hoặc rỗng, tìm kiếm với null
            string searchTerm = (string.IsNullOrWhiteSpace(text) || text == placeholder) ? null : text;
            
            FilterFoods(_loaiHienTai, searchTerm);
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTimKiem_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            // Lấy text và kiểm tra xem có phải placeholder text không
            string text = txtTimKiem.Text?.Trim() ?? "";
            string placeholder = txtTimKiem.PlaceholderText ?? "";
            
            // Nếu text box trống hoặc chỉ có placeholder, hiển thị lại tất cả theo loại đang chọn
            if (string.IsNullOrWhiteSpace(text) || text == placeholder)
            {
                FilterFoods(_loaiHienTai, null);
            }
        }

        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            // Khi focus vào, nếu text là placeholder thì clear
            if (txtTimKiem.Text == txtTimKiem.PlaceholderText)
            {
                txtTimKiem.Text = "";
            }
        }

        private void txtTimKiem_Leave(object sender, EventArgs e)
        {
            // Khi mất focus, nếu text rỗng thì có thể set lại placeholder (tùy chọn)
            // Guna2TextBox tự động xử lý placeholder nên không cần làm gì
        }

        /// <summary>
        /// Kết nối events cho các controls
        /// </summary>
        private void ConnectEvents()
        {
            // Kết nối event cho nút xóa món ăn có sẵn
            if (btnXoa != null)
            {
                btnXoa.Click += BtnXoa_Click;
            }

            // Kết nối event cho textbox trọng lượng có sẵn
            if (txtTrongLuong != null)
            {
                txtTrongLuong.TextChanged += TxtTrongLuong_TextChanged;
            }

            // Kết nối event cho nút xác nhận
            if (btnXacNhan != null)
            {
                btnXacNhan.Click += BtnXacNhan_Click;
            }

            // Kết nối event cho nút đặt lại
            if (btnDatLai != null)
            {
                btnDatLai.Click += BtnDatLai_Click;
            }
        }

        /// <summary>
        /// Kết nối event cho nút "Thêm" trong UserControlFood (được gọi khi cập nhật UserControl)
        /// </summary>
        private void ConnectThemButtonEvent(UserControlFood ucFood, ThuVienMonAn monAn)
        {
            if (ucFood == null || monAn == null) return;

            // Tìm nút "Thêm" trong UserControlFood bằng cách duyệt controls
            var btnThem = FindControl<Guna.UI2.WinForms.Guna2Button>(ucFood, "btnThem1");
            if (btnThem == null)
            {
                // Nếu không tìm thấy bằng tên, tìm bằng text "Thêm"
                btnThem = ucFood.Controls.OfType<Guna.UI2.WinForms.Guna2Button>()
                    .FirstOrDefault(btn => btn.Text == "Thêm");
            }

            if (btnThem != null)
            {
                // Unsubscribe trước để tránh duplicate
                btnThem.Click -= BtnThem_Click;
                // Subscribe lại
                btnThem.Click += BtnThem_Click;
                // Lưu thông tin món ăn vào Tag để xử lý khi click
                btnThem.Tag = monAn;
            }
        }

        /// <summary>
        /// Helper method để tìm control theo tên (recursive)
        /// </summary>
        private T FindControl<T>(Control parent, string name) where T : Control
        {
            if (parent == null) return null;
            
            // Kiểm tra chính control này
            if (parent is T && parent.Name == name)
                return parent as T;
            
            // Duyệt các controls con
            foreach (Control child in parent.Controls)
            {
                var found = FindControl<T>(child, name);
                if (found != null)
                    return found;
            }
            
            return null;
        }

        /// <summary>
        /// Event handler khi click nút "Thêm" trong UserControlFood
        /// </summary>
        private void BtnThem_Click(object sender, EventArgs e)
        {
            var btn = sender as Guna.UI2.WinForms.Guna2Button;
            if (btn == null) return;

            ThuVienMonAn monAn = btn.Tag as ThuVienMonAn;
            if (monAn == null) return;

            // Gọi method xử lý thêm món ăn
            ThemMonAnVaoDanhSach(monAn);
        }

        /// <summary>
        /// Xử lý thêm món ăn vào danh sách
        /// </summary>
        private void ThemMonAnVaoDanhSach(ThuVienMonAn monAn)
        {
            if (monAn == null) return;

            // Kiểm tra xem món ăn đã được thêm chưa
            var daCo = _danhSachMonAnDaChon.FirstOrDefault(m => m.MonAn.MonAnID == monAn.MonAnID);
            if (daCo != null)
            {
                MessageBox.Show($"Món ăn \"{monAn.TenMonAn}\" đã có trong danh sách!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Thêm món ăn vào danh sách với trọng lượng mặc định = khối lượng chuẩn
            double khoiLuongChuan = monAn.KhoiLuongChuan ?? 100;
            var monAnDaChon = new MonAnDaChonInfo
            {
                MonAn = monAn,
                TrongLuong = khoiLuongChuan
            };

            _danhSachMonAnDaChon.Add(monAnDaChon);
            
            // Hiển thị tất cả món ăn đã chọn (sẽ tự động tạo controls động nếu cần)
            HienThiTatCaMonAnDaChon();
            
            MessageBox.Show($"Đã thêm \"{monAn.TenMonAn}\" vào danh sách!", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Khởi tạo Set 1 từ controls có sẵn trong Designer
        /// </summary>
        private void KhoiTaoSetControlsDauTien()
        {
            if (pnlDanhSachMonAnDuocChon == null) return;

            var setControls = new SetControlsMonAn
            {
                LblTen = lblTenMonAnDuocChon1,
                LblDonVi = lblDonviMonAnDuocChon,
                TxtTrongLuong = txtTrongLuong,
                BtnXoa = btnXoa,
                Index = 1,
                IsDynamicCreated = false // Đánh dấu là controls có sẵn
            };

            // Sử dụng vị trí chính xác từ Designer của các controls gốc
            // lblTenMonAnDuocChon1: X = 58, Y = 18
            // lblDonviMonAnDuocChon: X = 457, Y = 18
            // txtTrongLuong: X = 659, Y = 18
            // btnXoa: X = 969, Y = 10
            int startY = 18; // Vị trí Y theo controls gốc trong Designer
            
            if (setControls.LblTen != null)
            {
                setControls.LblTen.Location = new Point(58, startY); // Vị trí chính xác từ Designer
                setControls.LblTen.Visible = false;
            }
            if (setControls.LblDonVi != null)
            {
                setControls.LblDonVi.Location = new Point(457, startY); // Vị trí chính xác từ Designer
                setControls.LblDonVi.Visible = false;
            }
            if (setControls.TxtTrongLuong != null)
            {
                setControls.TxtTrongLuong.Location = new Point(659, startY); // Vị trí chính xác từ Designer
                setControls.TxtTrongLuong.Visible = false;
            }
            if (setControls.BtnXoa != null)
            {
                setControls.BtnXoa.Location = new Point(969, 10); // Vị trí chính xác từ Designer (Y = 10)
                setControls.BtnXoa.Visible = false;
            }

            _danhSachSetsControls.Add(setControls);
        }

        /// <summary>
        /// Tạo một set controls mới động
        /// </summary>
        private SetControlsMonAn TaoSetControlsDong(int index)
        {
            if (pnlDanhSachMonAnDuocChon == null) return null;

            // Sử dụng vị trí chính xác từ Designer của các controls gốc
            // lblTenMonAnDuocChon1: X = 58, Y = 18
            // lblDonviMonAnDuocChon: X = 457, Y = 18
            // txtTrongLuong: X = 659, Y = 18
            // btnXoa: X = 969, Y = 10
            int startY = 18; // Vị trí Y theo controls gốc trong Designer
            int rowHeight = 60; // Khoảng cách giữa mỗi dòng
            int y = startY + (index - 1) * rowHeight;
            int yBtnXoa = 10 + (index - 1) * rowHeight; // btnXoa có Y = 10 trong Designer

            // Kiểm tra xem controls đã tồn tại chưa (tránh trùng lặp)
            string tenLblTen = $"lblTenMonAnDuocChon{index}";
            string tenLblDonVi = $"lblDonviMonAnDuocChon{index}";
            string tenTxtTrongLuong = $"txtTrongLuong{index}";
            string tenBtnXoa = $"btnXoa{index}";

            // Xóa controls cũ nếu đã tồn tại
            var controlsCu = pnlDanhSachMonAnDuocChon.Controls.Find(tenLblTen, false);
            foreach (var ctrl in controlsCu) { pnlDanhSachMonAnDuocChon.Controls.Remove(ctrl); ctrl.Dispose(); }
            controlsCu = pnlDanhSachMonAnDuocChon.Controls.Find(tenLblDonVi, false);
            foreach (var ctrl in controlsCu) { pnlDanhSachMonAnDuocChon.Controls.Remove(ctrl); ctrl.Dispose(); }
            controlsCu = pnlDanhSachMonAnDuocChon.Controls.Find(tenTxtTrongLuong, false);
            foreach (var ctrl in controlsCu) { pnlDanhSachMonAnDuocChon.Controls.Remove(ctrl); ctrl.Dispose(); }
            controlsCu = pnlDanhSachMonAnDuocChon.Controls.Find(tenBtnXoa, false);
            foreach (var ctrl in controlsCu) { pnlDanhSachMonAnDuocChon.Controls.Remove(ctrl); ctrl.Dispose(); }

            // Tạo Label tên món ăn
            var lblTen = new Label
            {
                Name = tenLblTen,
                Text = "",
                Font = new Font("Times New Roman", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(58, y), // Vị trí chính xác từ Designer
                Padding = new Padding(10),
                BackColor = Color.White,
                Visible = false
            };
            pnlDanhSachMonAnDuocChon.Controls.Add(lblTen);

            // Tạo Label đơn vị
            var lblDonVi = new Label
            {
                Name = tenLblDonVi,
                Text = "g",
                Font = new Font("Times New Roman", 12F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(457, y), // Vị trí chính xác từ Designer
                Padding = new Padding(10),
                BackColor = Color.White,
                Visible = false
            };
            pnlDanhSachMonAnDuocChon.Controls.Add(lblDonVi);

            // Tạo TextBox trọng lượng
            var txtTrongLuongMoi = new Guna.UI2.WinForms.Guna2TextBox
            {
                Name = tenTxtTrongLuong,
                Text = "",
                BorderColor = Color.Silver,
                BorderRadius = 20,
                Font = new Font("Times New Roman", 9F),
                Location = new Point(659, y), // Vị trí chính xác từ Designer
                Size = new Size(229, 37),
                TextAlign = HorizontalAlignment.Center,
                Visible = false
            };
            txtTrongLuongMoi.TextChanged += TxtTrongLuong_TextChanged_Generic;
            pnlDanhSachMonAnDuocChon.Controls.Add(txtTrongLuongMoi);

            // Tạo Button xóa - Copy hình ảnh từ button mẫu nếu có
            Image btnXoaImage = null;
            if (btnXoa != null && btnXoa.Image != null)
            {
                btnXoaImage = btnXoa.Image;
            }

            var btnXoaMoi = new Guna.UI2.WinForms.Guna2CircleButton
            {
                Name = tenBtnXoa,
                FillColor = Color.White,
                Image = btnXoaImage,
                ImageSize = new Size(40, 40),
                Location = new Point(969, yBtnXoa), // Vị trí chính xác từ Designer (Y = 10)
                Size = new Size(53, 51),
                Visible = false
            };
            btnXoaMoi.Click += BtnXoa_Click_Generic;
            pnlDanhSachMonAnDuocChon.Controls.Add(btnXoaMoi);

            return new SetControlsMonAn
            {
                LblTen = lblTen,
                LblDonVi = lblDonVi,
                TxtTrongLuong = txtTrongLuongMoi,
                BtnXoa = btnXoaMoi,
                Index = index,
                IsDynamicCreated = true
            };
        }

        /// <summary>
        /// Đảm bảo có đủ sets controls để hiển thị tất cả món ăn
        /// </summary>
        private void DamBaoDuSetsControls(int soLuongMonAn)
        {
            if (soLuongMonAn <= 0) return;

            // Tạo thêm sets controls nếu cần
            while (_danhSachSetsControls.Count < soLuongMonAn)
            {
                int indexMoi = _danhSachSetsControls.Count + 1;
                var setMoi = TaoSetControlsDong(indexMoi);
                if (setMoi != null)
                {
                    _danhSachSetsControls.Add(setMoi);
                }
                else
                {
                    break; // Không thể tạo thêm
                }
            }
        }

        /// <summary>
        /// Hiển thị tất cả món ăn đã chọn vào các sets controls (tự động tạo thêm nếu cần)
        /// </summary>
        private void HienThiTatCaMonAnDaChon()
        {
            if (pnlDanhSachMonAnDuocChon == null) return;

            int soLuongMonAn = _danhSachMonAnDaChon.Count;

            // Đảm bảo có đủ sets controls
            DamBaoDuSetsControls(soLuongMonAn);

            // Ẩn tất cả sets controls trước
            foreach (var set in _danhSachSetsControls)
            {
                if (set.LblTen != null) set.LblTen.Visible = false;
                if (set.LblDonVi != null) set.LblDonVi.Visible = false;
                if (set.TxtTrongLuong != null)
                {
                    set.TxtTrongLuong.Visible = false;
                    set.TxtTrongLuong.Tag = null;
                }
                if (set.BtnXoa != null) set.BtnXoa.Visible = false;
            }

            // Sử dụng vị trí chính xác từ Designer của các controls gốc
            // lblTenMonAnDuocChon1: X = 58, Y = 18
            // lblDonviMonAnDuocChon: X = 457, Y = 18
            // txtTrongLuong: X = 659, Y = 18
            // btnXoa: X = 969, Y = 10
            int startY = 18; // Vị trí Y theo controls gốc trong Designer
            int rowHeight = 60; // Khoảng cách giữa mỗi dòng

            for (int i = 0; i < Math.Min(soLuongMonAn, _danhSachSetsControls.Count); i++)
            {
                var monAnDaChon = _danhSachMonAnDaChon[i];
                var setControls = _danhSachSetsControls[i];

                if (monAnDaChon == null || monAnDaChon.MonAn == null) continue;

                int y = startY + i * rowHeight;
                int yBtnXoa = 10 + i * rowHeight; // btnXoa có Y = 10 trong Designer

                // Cập nhật vị trí cho tất cả controls trong set, sử dụng vị trí chính xác từ Designer
                if (setControls.LblTen != null)
                {
                    setControls.LblTen.Location = new Point(58, y); // Vị trí chính xác từ Designer
                    setControls.LblTen.Text = monAnDaChon.MonAn.TenMonAn ?? "";
                    setControls.LblTen.Visible = true;
                    setControls.LblTen.Tag = monAnDaChon;
                }

                if (setControls.LblDonVi != null)
                {
                    setControls.LblDonVi.Location = new Point(457, y); // Vị trí chính xác từ Designer
                    setControls.LblDonVi.Text = monAnDaChon.MonAn.Donvi ?? "g";
                    setControls.LblDonVi.Visible = true;
                }

                if (setControls.TxtTrongLuong != null)
                {
                    setControls.TxtTrongLuong.Location = new Point(659, y); // Vị trí chính xác từ Designer
                    setControls.TxtTrongLuong.Text = monAnDaChon.TrongLuong.ToString("F1");
                    setControls.TxtTrongLuong.Visible = true;
                    setControls.TxtTrongLuong.Tag = monAnDaChon;
                }

                if (setControls.BtnXoa != null)
                {
                    setControls.BtnXoa.Location = new Point(969, yBtnXoa); // Vị trí chính xác từ Designer (Y = 10)
                    setControls.BtnXoa.Visible = true;
                    setControls.BtnXoa.Tag = monAnDaChon;
                }
            }

            pnlDanhSachMonAnDuocChon.Refresh();
        }

        /// <summary>
        /// Event handler khi thay đổi trọng lượng (cho control mẫu ban đầu)
        /// </summary>
        private void TxtTrongLuong_TextChanged(object sender, EventArgs e)
        {
            TxtTrongLuong_TextChanged_Generic(sender, e);
        }

        /// <summary>
        /// Event handler generic cho textbox trọng lượng (dùng cho tất cả sets controls)
        /// </summary>
        private void TxtTrongLuong_TextChanged_Generic(object sender, EventArgs e)
        {
            var txtBox = sender as Guna.UI2.WinForms.Guna2TextBox;
            if (txtBox?.Tag is MonAnDaChonInfo monAnDaChon)
            {
                if (double.TryParse(txtBox.Text, out double trongLuong) && trongLuong > 0)
                {
                    monAnDaChon.TrongLuong = trongLuong;
                }
            }
        }

        /// <summary>
        /// Event handler khi click nút xóa món ăn (cho control mẫu ban đầu)
        /// </summary>
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            BtnXoa_Click_Generic(sender, e);
        }

        /// <summary>
        /// Event handler generic cho nút xóa (dùng cho tất cả sets controls)
        /// </summary>
        private void BtnXoa_Click_Generic(object sender, EventArgs e)
        {
            var btn = sender as Guna.UI2.WinForms.Guna2CircleButton;
            if (btn?.Tag is MonAnDaChonInfo monAnDaChon)
            {
                string tenMonAn = monAnDaChon.MonAn.TenMonAn ?? "món ăn này";

                // Hỏi xác nhận trước khi xóa
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa \"{tenMonAn}\" khỏi danh sách?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Xóa món ăn khỏi danh sách
                    _danhSachMonAnDaChon.Remove(monAnDaChon);

                    // Hiển thị lại tất cả món ăn (sẽ tự động ẩn controls thừa)
                    HienThiTatCaMonAnDaChon();

                    // Xóa các sets controls thừa (không cần thiết nữa)
                    XoaSetsControlsThua();

                    // Reset tổng dinh dưỡng
                    ResetTongDinhDuong();

                    MessageBox.Show("Đã xóa món ăn khỏi danh sách!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Xóa các sets controls thừa (không còn món ăn nào cần hiển thị)
        /// </summary>
        private void XoaSetsControlsThua()
        {
            if (pnlDanhSachMonAnDuocChon == null) return;

            int soLuongMonAn = _danhSachMonAnDaChon.Count;

            // Xóa từ cuối lên (trừ Set 1 - có sẵn)
            for (int i = _danhSachSetsControls.Count - 1; i >= soLuongMonAn && i >= 1; i--)
            {
                var set = _danhSachSetsControls[i];
                if (set.IsDynamicCreated) // Chỉ xóa các controls được tạo động
                {
                    if (set.LblTen != null && !set.LblTen.IsDisposed)
                    {
                        pnlDanhSachMonAnDuocChon.Controls.Remove(set.LblTen);
                        set.LblTen.Dispose();
                    }
                    if (set.LblDonVi != null && !set.LblDonVi.IsDisposed)
                    {
                        pnlDanhSachMonAnDuocChon.Controls.Remove(set.LblDonVi);
                        set.LblDonVi.Dispose();
                    }
                    if (set.TxtTrongLuong != null && !set.TxtTrongLuong.IsDisposed)
                    {
                        pnlDanhSachMonAnDuocChon.Controls.Remove(set.TxtTrongLuong);
                        set.TxtTrongLuong.Dispose();
                    }
                    if (set.BtnXoa != null && !set.BtnXoa.IsDisposed)
                    {
                        pnlDanhSachMonAnDuocChon.Controls.Remove(set.BtnXoa);
                        set.BtnXoa.Dispose();
                    }
                    _danhSachSetsControls.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Tính toán và hiển thị tổng dinh dưỡng dựa trên tất cả món ăn đã chọn
        /// </summary>
        private void TinhToanTongDinhDuong()
        {
            if (_danhSachMonAnDaChon == null || _danhSachMonAnDaChon.Count == 0)
            {
                ResetTongDinhDuong();
                return;
            }

            double tongCalories = 0;
            double tongProtein = 0;
            double tongCarbs = 0;
            double tongFat = 0;
            double tongFiber = 0;

            foreach (var monAnDaChon in _danhSachMonAnDaChon)
            {
                var monAn = monAnDaChon.MonAn;
                double trongLuong = monAnDaChon.TrongLuong;
                double khoiLuongChuan = monAn.KhoiLuongChuan ?? 100;

                if (trongLuong <= 0 || khoiLuongChuan <= 0)
                    continue;

                // Tính tỷ lệ: trọng lượng nhập / khối lượng chuẩn
                double tyLe = trongLuong / khoiLuongChuan;

                // Tính dinh dưỡng thực tế
                if (monAn.Calories.HasValue)
                    tongCalories += monAn.Calories.Value * tyLe;
                if (monAn.Protein.HasValue)
                    tongProtein += monAn.Protein.Value * tyLe;
                if (monAn.Carbs.HasValue)
                    tongCarbs += monAn.Carbs.Value * tyLe;
                if (monAn.Fat.HasValue)
                    tongFat += monAn.Fat.Value * tyLe;
                if (monAn.Fiber.HasValue)
                    tongFiber += monAn.Fiber.Value * tyLe;
            }

            // Hiển thị kết quả
            if (lblTongSoCalories != null)
                lblTongSoCalories.Text = tongCalories.ToString("F1");
            if (lblTongSoProtein != null)
                lblTongSoProtein.Text = tongProtein.ToString("F1");
            if (lblTongSoCarbs != null)
                lblTongSoCarbs.Text = tongCarbs.ToString("F1");
            if (lblSoTongFat != null)
                lblSoTongFat.Text = tongFat.ToString("F1");
            if (lblTongSoChatSo != null)
                lblTongSoChatSo.Text = tongFiber.ToString("F1");
        }

        /// <summary>
        /// Reset tổng dinh dưỡng về 0
        /// </summary>
        private void ResetTongDinhDuong()
        {
            if (lblTongSoCalories != null) lblTongSoCalories.Text = "0";
            if (lblTongSoProtein != null) lblTongSoProtein.Text = "0";
            if (lblTongSoCarbs != null) lblTongSoCarbs.Text = "0";
            if (lblSoTongFat != null) lblSoTongFat.Text = "0";
            if (lblTongSoChatSo != null) lblTongSoChatSo.Text = "0";
        }

        /// <summary>
        /// Event handler khi click nút Xác nhận
        /// </summary>
        private void BtnXacNhan_Click(object sender, EventArgs e)
        {
            if (_danhSachMonAnDaChon == null || _danhSachMonAnDaChon.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một món ăn vào danh sách!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra trọng lượng hợp lệ
            var chuaNhapTrongLuong = _danhSachMonAnDaChon.Where(m => m.TrongLuong <= 0).ToList();
            if (chuaNhapTrongLuong.Count > 0)
            {
                string danhSach = string.Join("\n", chuaNhapTrongLuong.Select(m => $"  - {m.MonAn.TenMonAn}"));
                MessageBox.Show($"Vui lòng nhập trọng lượng hợp lệ (> 0) cho:\n{danhSach}", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tính toán và hiển thị tổng dinh dưỡng
            TinhToanTongDinhDuong();

            MessageBox.Show($"Đã tính toán dinh dưỡng cho {_danhSachMonAnDaChon.Count} món ăn!", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Event handler khi click nút Đặt lại
        /// </summary>
        private void BtnDatLai_Click(object sender, EventArgs e)
        {
            if (_danhSachMonAnDaChon == null || _danhSachMonAnDaChon.Count == 0)
            {
                MessageBox.Show("Danh sách món ăn đã trống!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Hỏi xác nhận
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đặt lại danh sách món ăn đã chọn?\n\nTất cả dữ liệu sẽ bị xóa!",
                "Xác nhận đặt lại",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Xóa tất cả
                _danhSachMonAnDaChon.Clear();

                // Xóa tất cả sets controls được tạo động (giữ lại Set 1 có sẵn)
                XoaTatCaSetsControlsDong();

                // Ẩn Set 1 (controls có sẵn)
                if (_danhSachSetsControls.Count > 0)
                {
                    var setDauTien = _danhSachSetsControls[0];
                    if (setDauTien.LblTen != null) setDauTien.LblTen.Visible = false;
                    if (setDauTien.LblDonVi != null) setDauTien.LblDonVi.Visible = false;
                    if (setDauTien.TxtTrongLuong != null)
                    {
                        setDauTien.TxtTrongLuong.Visible = false;
                        setDauTien.TxtTrongLuong.Tag = null;
                    }
                    if (setDauTien.BtnXoa != null)
                    {
                        setDauTien.BtnXoa.Visible = false;
                        setDauTien.BtnXoa.Tag = null;
                    }
                }

                // Reset tổng dinh dưỡng
                ResetTongDinhDuong();

                MessageBox.Show("Đã đặt lại danh sách món ăn thành công!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Xóa tất cả sets controls được tạo động (giữ lại Set 1 có sẵn)
        /// </summary>
        private void XoaTatCaSetsControlsDong()
        {
            if (pnlDanhSachMonAnDuocChon == null) return;

            // Xóa từ cuối lên (trừ Set 1 - có sẵn)
            for (int i = _danhSachSetsControls.Count - 1; i >= 1; i--)
            {
                var set = _danhSachSetsControls[i];
                if (set.IsDynamicCreated) // Chỉ xóa các controls được tạo động
                {
                    if (set.LblTen != null && !set.LblTen.IsDisposed)
                    {
                        pnlDanhSachMonAnDuocChon.Controls.Remove(set.LblTen);
                        set.LblTen.Dispose();
                    }
                    if (set.LblDonVi != null && !set.LblDonVi.IsDisposed)
                    {
                        pnlDanhSachMonAnDuocChon.Controls.Remove(set.LblDonVi);
                        set.LblDonVi.Dispose();
                    }
                    if (set.TxtTrongLuong != null && !set.TxtTrongLuong.IsDisposed)
                    {
                        pnlDanhSachMonAnDuocChon.Controls.Remove(set.TxtTrongLuong);
                        set.TxtTrongLuong.Dispose();
                    }
                    if (set.BtnXoa != null && !set.BtnXoa.IsDisposed)
                    {
                        pnlDanhSachMonAnDuocChon.Controls.Remove(set.BtnXoa);
                        set.BtnXoa.Dispose();
                    }
                    _danhSachSetsControls.RemoveAt(i);
                }
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_trangHienTai > 0)
            {
                _trangHienTai--;
                DisplayFoods();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int tongSoTrang = (int)Math.Ceiling((double)_danhSachMonAn.Count / _soLuongMoiTrang);
            if (_trangHienTai < tongSoTrang - 1)
            {
                _trangHienTai++;
                DisplayFoods();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void lblLoaiMonAn_Click(object sender, EventArgs e)
        {

        }

        private void pnlLoaiMonAn_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblLoaiMonAn_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox31_Click(object sender, EventArgs e)
        {

        }
    }
}
