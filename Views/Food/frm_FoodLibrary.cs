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

        public frm_FoodLibrary()
        {
            InitializeComponent();
            _danhSachMonAn = new List<ThuVienMonAn>();
            _danhSachMonAnDayDu = new List<ThuVienMonAn>();
            _listUserControlFood = new List<UserControlFood>();
            
            // Đảm bảo panel có background màu trắng
            if (pnlDanhSachMonAn != null)
            {
                pnlDanhSachMonAn.FillColor = Color.White;
                pnlDanhSachMonAn.FillColor2 = Color.White;
                pnlDanhSachMonAn.AutoScroll = true;
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
        /// Hiển thị danh sách món ăn trong pnlDanhSachMonAn với layout 2x2
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

            System.Diagnostics.Debug.WriteLine($"Panel trước khi xóa: Controls count = {pnlDanhSachMonAn.Controls.Count}");

            // Xóa các UserControl cũ
            foreach (var uc in _listUserControlFood)
            {
                if (uc != null && !uc.IsDisposed)
                {
                    pnlDanhSachMonAn.Controls.Remove(uc);
                    uc.Dispose();
                }
            }
            _listUserControlFood.Clear();

            if (_danhSachMonAn == null || _danhSachMonAn.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Không có dữ liệu món ăn để hiển thị");
                System.Diagnostics.Debug.WriteLine($"_danhSachMonAn == null: {_danhSachMonAn == null}");
                System.Diagnostics.Debug.WriteLine($"_danhSachMonAn.Count: {(_danhSachMonAn?.Count ?? 0)}");
                MessageBox.Show("Không có dữ liệu món ăn trong database.\n\nVui lòng chạy file SampleData_ThuVienMonAn.sql để thêm dữ liệu mẫu.", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Bắt đầu hiển thị {_danhSachMonAn.Count} món ăn");

            // Kích thước và khoảng cách
            int itemWidth = 527;  // Width của UserControlFood
            int itemHeight = 256; // Height của UserControlFood
            int padding = 20;     // Khoảng cách giữa các items
            int itemsPerRow = 2;  // Số items mỗi hàng (2x2)
            int startY = 70;      // Bắt đầu từ dưới label "Danh sách món ăn" (tăng lên để tránh bị che)

            // Đảm bảo panel có thể scroll và có background
            pnlDanhSachMonAn.AutoScroll = true;
            pnlDanhSachMonAn.FillColor = Color.White;
            pnlDanhSachMonAn.FillColor2 = Color.White;

            System.Diagnostics.Debug.WriteLine($"Panel settings: AutoScroll={pnlDanhSachMonAn.AutoScroll}, Size={pnlDanhSachMonAn.Size}");

            // Tạo và đặt vị trí các UserControlFood
            for (int i = 0; i < _danhSachMonAn.Count; i++)
            {
                var monAn = _danhSachMonAn[i];
                System.Diagnostics.Debug.WriteLine($"Tạo UserControlFood {i + 1} cho: {monAn.TenMonAn}");
                
                var ucFood = new UserControlFood();
                
                // Gán dữ liệu
                ucFood.SetData(monAn);

                // Tính toán vị trí (layout 2x2)
                int row = i / itemsPerRow;
                int col = i % itemsPerRow;
                
                int x = padding + col * (itemWidth + padding);
                int y = startY + row * (itemHeight + padding);

                ucFood.Location = new Point(x, y);
                ucFood.Size = new Size(itemWidth, itemHeight);
                ucFood.Visible = true;
                ucFood.BackColor = Color.Transparent;

                // Thêm vào panel
                System.Diagnostics.Debug.WriteLine($"Thêm UserControlFood vào panel tại ({x}, {y})");
                pnlDanhSachMonAn.Controls.Add(ucFood);
                _listUserControlFood.Add(ucFood);
                
                // Set Z-order: Đưa UserControlFood lên trên cùng, sau đó đưa label và picture xuống dưới
                ucFood.BringToFront();
                
                System.Diagnostics.Debug.WriteLine($"Đã thêm UserControlFood {i + 1}: {monAn.TenMonAn} tại vị trí ({x}, {y})");
                System.Diagnostics.Debug.WriteLine($"  - UserControlFood Visible: {ucFood.Visible}, Size: {ucFood.Size}");
            }

            // Đảm bảo label và picture ở dưới cùng (không che UserControlFood)
            if (lblDanhSachMonAn != null)
            {
                lblDanhSachMonAn.SendToBack();
            }
            if (ptrDanhSachMonAn != null)
            {
                ptrDanhSachMonAn.SendToBack();
            }

            // Refresh panel để đảm bảo tất cả controls được render
            pnlDanhSachMonAn.Refresh();
            this.Refresh();
            
            System.Diagnostics.Debug.WriteLine($"=== KẾT QUẢ ===");
            System.Diagnostics.Debug.WriteLine($"Đã hiển thị {_listUserControlFood.Count} UserControlFood trong panel");
            System.Diagnostics.Debug.WriteLine($"Panel Controls count: {pnlDanhSachMonAn.Controls.Count}");
            System.Diagnostics.Debug.WriteLine($"Panel size: {pnlDanhSachMonAn.Size}, location: {pnlDanhSachMonAn.Location}");
            System.Diagnostics.Debug.WriteLine($"Panel visible: {pnlDanhSachMonAn.Visible}, enabled: {pnlDanhSachMonAn.Enabled}");
            System.Diagnostics.Debug.WriteLine("=== DisplayFoods KẾT THÚC ===");
        }

        /// <summary>
        /// Lọc danh sách món ăn theo loại và từ khóa tìm kiếm
        /// </summary>
        private void FilterFoods(string loai = null, string searchTerm = null)
        {
            _loaiHienTai = loai;
            _tuKhoaTimKiem = searchTerm ?? "";

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
            string searchTerm = txtTimKiem.Text?.Trim() ?? "";
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
            // Nếu text box trống, hiển thị lại tất cả theo loại đang chọn
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                FilterFoods(_loaiHienTai, null);
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
