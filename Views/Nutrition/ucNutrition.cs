extern alias ef6;

using HealthApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ef6::System.Data.Entity.Infrastructure;

namespace HealthApp.Views.Nutrition
{
    public partial class ucNutrition : UserControl
    {
        private WF_HealthTracker _dbContext;
        private Guna.UI2.WinForms.Guna2Panel _pnlScrollMonAnPhoBien;
        private Guna.UI2.WinForms.Guna2Panel _pnlScrollNhatKyBuaAn;
        private List<BuaAnChiTiet> _danhSachMonAnDaThem;

        public ucNutrition()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            _danhSachMonAnDaThem = new List<BuaAnChiTiet>();
            InitializeScrollPanels();
        }

        private void InitializeScrollPanels()
        {
            // Tạo scrollable panel cho món ăn phổ biến
            _pnlScrollMonAnPhoBien = new Guna.UI2.WinForms.Guna2Panel
            {
                AutoScroll = true,
                Location = new Point(20, 70),
                Size = new Size(488, 600),
                BackColor = Color.Transparent
            };
            pnlMonAnPhoBien.Controls.Add(_pnlScrollMonAnPhoBien);

            // Tạo scrollable panel cho nhật ký bữa ăn
            _pnlScrollNhatKyBuaAn = new Guna.UI2.WinForms.Guna2Panel
            {
                AutoScroll = true,
                Location = new Point(20, 70),
                Size = new Size(666, 500),
                BackColor = Color.Transparent
            };
            pnlDanhSachNhatKyBuaAn.Controls.Add(_pnlScrollNhatKyBuaAn);
        }

        private void Nutrition_Load(object sender, EventArgs e)
        {
            LoadMonAnPhoBien();
            LoadNhatKyBuaAn();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            btnThemMon.Click += BtnThemMon_Click;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Event handler for lblTieuDe click
        }

        private void guna2PictureBox6_Click(object sender, EventArgs e)
        {
            // Event handler for picFork click
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            // Event handler for picDumbbells click
        }

        private bool TestDatabaseConnection()
        {
            try
            {
                // Test connection bằng cách thực hiện một query đơn giản
                var connection = _dbContext.Database.Connection;
                string connectionString = connection.ConnectionString;
                
                // Kiểm tra database có tồn tại không
                if (!_dbContext.Database.Exists())
                {
                    MessageBox.Show($"Database không tồn tại!\n\n" +
                        $"Server: DESKTOP-QH7JC2G\\LOC1109\n" +
                        $"Database: HealthTracker\n\n" +
                        $"Vui lòng kiểm tra:\n" +
                        $"1. SQL Server đang chạy\n" +
                        $"2. Database 'HealthTracker' đã được tạo\n" +
                        $"3. Connection string trong App.config đúng",
                        "Lỗi Kết Nối Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Test connection bằng cách mở và đóng connection
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                connection.Close();
                
                return true;
            }
            catch (SqlException sqlEx)
            {
                string errorMsg = $"Lỗi SQL Server:\n\n";
                errorMsg += $"Mã lỗi: {sqlEx.Number}\n";
                errorMsg += $"Thông báo: {sqlEx.Message}\n\n";
                
                // Thông báo lỗi cụ thể dựa trên mã lỗi
                switch (sqlEx.Number)
                {
                    case -1: // Connection timeout
                        errorMsg += "Không thể kết nối đến SQL Server. Kiểm tra:\n";
                        errorMsg += "- SQL Server đang chạy\n";
                        errorMsg += "- Server name đúng: DESKTOP-QH7JC2G\\LOC1109\n";
                        errorMsg += "- Firewall không chặn port 1433\n";
                        break;
                    case 2: // SQL Server not found
                        errorMsg += "Không tìm thấy SQL Server. Kiểm tra server name.\n";
                        break;
                    case 4060: // Cannot open database
                        errorMsg += "Không thể mở database 'HealthTracker'. Kiểm tra:\n";
                        errorMsg += "- Database đã được tạo\n";
                        errorMsg += "- User có quyền truy cập database\n";
                        break;
                    case 18456: // Login failed
                        errorMsg += "Đăng nhập thất bại. Kiểm tra:\n";
                        errorMsg += "- Windows Authentication đúng\n";
                        errorMsg += "- User có quyền truy cập SQL Server\n";
                        break;
                }
                
                errorMsg += $"\nConnection String:\n{_dbContext.Database.Connection.ConnectionString}";
                
                MessageBox.Show(errorMsg, "Lỗi Kết Nối Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Lỗi kết nối database:\n\n";
                errorMsg += $"Thông báo: {ex.Message}\n\n";
                if (ex.InnerException != null)
                {
                    errorMsg += $"Chi tiết: {ex.InnerException.Message}\n\n";
                }
                errorMsg += $"Connection String:\n{_dbContext.Database.Connection.ConnectionString}";
                
                MessageBox.Show(errorMsg, "Lỗi Kết Nối Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadMonAnPhoBien()
        {
            try
            {
                // Test connection trước khi load data
                if (!TestDatabaseConnection())
                {
                    return;
                }

                _pnlScrollMonAnPhoBien.Controls.Clear();

                // Kiểm tra xem table có tồn tại và có data không
                try
                {
                    // Test query trước
                    var testCount = _dbContext.ThuVienMonAn.Count();
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Số lượng món ăn trong database: {testCount}");
                    
                    if (testCount == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("[DEBUG] Database không có dữ liệu món ăn");
                        return;
                    }

                    var danhSachMonAn = _dbContext.ThuVienMonAn
                        .OrderByDescending(m => m.Calories ?? 0)
                        .Take(20)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã load {danhSachMonAn.Count} món ăn");

                    int yPos = 10;
                    foreach (var monAn in danhSachMonAn)
                    {
                        var ucMonAn = new ucMonAnItem(monAn);
                        ucMonAn.Location = new Point(10, yPos);
                        ucMonAn.Width = 468;
                        ucMonAn.MonAnClicked += UcMonAn_MonAnClicked;
                        _pnlScrollMonAnPhoBien.Controls.Add(ucMonAn);
                        yPos += ucMonAn.Height + 10;
                    }
                }
                catch (Exception entityEx) when (entityEx.GetType().Name.Contains("Entity") || 
                                                  entityEx.GetType().Name.Contains("Db") ||
                                                  (entityEx.InnerException is SqlException))
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Entity Framework Error: {entityEx.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {entityEx.Message}");
                    if (entityEx.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception: {entityEx.InnerException.GetType().Name} - {entityEx.InnerException.Message}");
                    }
                    
                    string errorMsg = $"Lỗi Entity Framework khi load món ăn:\n\n";
                    errorMsg += $"Loại lỗi: {entityEx.GetType().Name}\n";
                    errorMsg += $"Thông báo: {entityEx.Message}\n";
                    if (entityEx.InnerException != null)
                    {
                        errorMsg += $"\nChi tiết: {entityEx.InnerException.Message}";
                    }
                    MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] SQL Error Number: {sqlEx.Number}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] SQL Error Message: {sqlEx.Message}");
                
                string errorMsg = $"Lỗi SQL khi load món ăn phổ biến:\n\n";
                errorMsg += $"Mã lỗi: {sqlEx.Number}\n";
                errorMsg += $"Thông báo: {sqlEx.Message}\n";
                if (sqlEx.InnerException != null)
                {
                    errorMsg += $"\nChi tiết: {sqlEx.InnerException.Message}";
                }
                MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] General Error: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception: {ex.InnerException.Message}");
                }
                
                string errorMsg = $"Lỗi khi load món ăn phổ biến:\n\n";
                errorMsg += $"Loại lỗi: {ex.GetType().Name}\n";
                errorMsg += $"Thông báo: {ex.Message}\n";
                if (ex.InnerException != null)
                {
                    errorMsg += $"\nChi tiết: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadNhatKyBuaAn()
        {
            try
            {
                _pnlScrollNhatKyBuaAn.Controls.Clear();
                lblThongTinDanhSachNhatKyBuaAn.Visible = false;

                // Kiểm tra xem table BuaAnChiTiet có tồn tại không
                // Nếu không có, sẽ dùng NhatKyDinhDuong thay thế
                try
                {
                    // Test xem có thể query được không
                    var testQuery = _dbContext.BuaAnChiTiet.Take(1).ToList();
                }
                catch
                {
                    // Table không tồn tại, hiển thị thông báo
                    lblThongTinDanhSachNhatKyBuaAn.Visible = true;
                    lblThongTinDanhSachNhatKyBuaAn.Text = "Chưa có món ăn nào được thêm vào hôm nay";
                    return;
                }

                // Load món ăn đã thêm hôm nay từ NhatKyDinhDuong
                var ngayHomNay = DateTime.Today;
                var ngayBatDau = ngayHomNay.Date;
                var ngayKetThuc = ngayHomNay.Date.AddDays(1).AddTicks(-1);
                
                // Load từ database (NhatKyDinhDuong)
                var danhSachNhatKy = _dbContext.BuaAnChiTiet
                    .Where(b => b.NgayAn >= ngayBatDau && 
                           b.NgayAn < ngayKetThuc)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Số lượng nhật ký dinh dưỡng hôm nay: {danhSachNhatKy.Count}");

                // Tính toán các giá trị dinh dưỡng từ DinhDuongMonAn
                _danhSachMonAnDaThem = new List<BuaAnChiTiet>();
                foreach (var nhatKy in danhSachNhatKy)
                {
                    // Load thông tin món ăn từ DinhDuongMonAn
                    var monAnGoc = _dbContext.ThuVienMonAn.FirstOrDefault(m => m.MonAnID == nhatKy.MonAnID);
                    if (monAnGoc == null) continue;

                    // Tính toán dinh dưỡng dựa trên LuongThucAn
                    double luongThucAn = nhatKy.KhoiLuongChuan ?? 0;
                    double tiLe = luongThucAn / 100.0;

                    // Parse LoaiBuaAn từ GhiChu (format: "LoaiBuaAn: Breakfast|GhiChu khác")
                    string loaiBuaAn = "";
                    string ghiChu = nhatKy.GhiChu ?? "";
                    if (ghiChu.StartsWith("LoaiBuaAn: "))
                    {
                        int index = ghiChu.IndexOf("|");
                        if (index > 0)
                        {
                            loaiBuaAn = ghiChu.Substring(11, index - 11);
                            ghiChu = ghiChu.Substring(index + 1);
                        }
                        else
                        {
                            loaiBuaAn = ghiChu.Substring(11);
                            ghiChu = "";
                        }
                    }

                    // Tạo BuaAnChiTiet với đầy đủ thông tin
                    var buaAn = new BuaAnChiTiet
                    {
                        BuaAnID = nhatKy.BuaAnID,
                        KeHoachAnID = nhatKy.KeHoachAnID,
                        MonAnID = nhatKy.MonAnID,
                        NgayAn = nhatKy.NgayAn,
                        KhoiLuongChuan = nhatKy.KhoiLuongChuan,
                        GhiChu = ghiChu,
                        // Các field tính toán
                        TenMonAn = monAnGoc.TenMonAn,
                        Donvi = monAnGoc.Donvi ?? "g",
                        LoaiBuaAn = loaiBuaAn,
                        Calories = (monAnGoc.Calories ?? 0) * tiLe,
                        Protein = (monAnGoc.Protein ?? 0) * tiLe,
                        Carbs = (monAnGoc.Carbs ?? 0) * tiLe,
                        Fat = (monAnGoc.Fat ?? 0) * tiLe,
                        Fiber = null
                    };

                    _danhSachMonAnDaThem.Add(buaAn);
                }

                // Sắp xếp theo LoaiBuaAn
                _danhSachMonAnDaThem = _danhSachMonAnDaThem
                    .OrderBy(b => b.LoaiBuaAn)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Số lượng món ăn đã thêm hôm nay: {_danhSachMonAnDaThem.Count}");

                if (_danhSachMonAnDaThem.Count == 0)
                {
                    lblThongTinDanhSachNhatKyBuaAn.Visible = true;
                    lblThongTinDanhSachNhatKyBuaAn.Text = "Chưa có món ăn nào được thêm vào hôm nay";
                    return;
                }

                int yPos = 10;
                foreach (var monAn in _danhSachMonAnDaThem)
                {
                    var ucMonAn = new ucMonAnDaThemItem(monAn);
                    ucMonAn.Location = new Point(10, yPos);
                    ucMonAn.Width = 646;
                    ucMonAn.XoaClicked += UcMonAn_XoaClicked;
                    _pnlScrollNhatKyBuaAn.Controls.Add(ucMonAn);
                    yPos += ucMonAn.Height + 10;
                }

                UpdateTongDinhDuong();
            }
            catch (Exception entityEx) when (entityEx.GetType().Name.Contains("Entity") || 
                                              entityEx.GetType().Name.Contains("Db") ||
                                              (entityEx.InnerException is SqlException))
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Entity Framework Error in LoadNhatKyBuaAn: {entityEx.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {entityEx.Message}");
                if (entityEx.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception: {entityEx.InnerException.GetType().Name} - {entityEx.InnerException.Message}");
                }
                
                string errorMsg = $"Lỗi Entity Framework khi load nhật ký:\n\n";
                errorMsg += $"Loại lỗi: {entityEx.GetType().Name}\n";
                errorMsg += $"Thông báo: {entityEx.Message}\n";
                if (entityEx.InnerException != null)
                {
                    errorMsg += $"\nChi tiết: {entityEx.InnerException.Message}";
                }
                MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] General Error in LoadNhatKyBuaAn: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception: {ex.InnerException.Message}");
                }
                
                string errorMsg = $"Lỗi khi load nhật ký bữa ăn:\n\n";
                errorMsg += $"Loại lỗi: {ex.GetType().Name}\n";
                errorMsg += $"Thông báo: {ex.Message}\n";
                if (ex.InnerException != null)
                {
                    errorMsg += $"\nChi tiết: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UcMonAn_MonAnClicked(object sender, ThuVienMonAn monAn)
        {
            OpenFormThemMonAn(monAn);
        }

        private void BtnThemMon_Click(object sender, EventArgs e)
        {
            OpenFormThemMonAn(null);
        }

        private void OpenFormThemMonAn(ThuVienMonAn monAn)
        {
            try
            {
                // Nếu không có món ăn được chọn, hiển thị danh sách để chọn
                if (monAn == null)
                {
                    // Lấy món ăn đầu tiên hoặc hiển thị form chọn món
                    var danhSachMonAn = _dbContext.ThuVienMonAn.Take(1).FirstOrDefault();
                    if (danhSachMonAn == null)
                    {
                        MessageBox.Show("Không có món ăn nào trong thư viện!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    monAn = danhSachMonAn;
                }

                using (var frm = new frmThemMonAn(monAn, _dbContext))
                {
                    if (frm.ShowDialog() == DialogResult.OK && frm.MonAnDaThem != null)
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== BẮT ĐẦU LƯU MÓN ĂN VÀO DATABASE =====");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] BuaAnID: {frm.MonAnDaThem.BuaAnID}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] KeHoachAnID (UserID): {frm.MonAnDaThem.KeHoachAnID}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] MonAnID: {frm.MonAnDaThem.MonAnID}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] NgayAn: {frm.MonAnDaThem.NgayAn}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] KhoiLuongChuan: {frm.MonAnDaThem.KhoiLuongChuan}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] GhiChu: {frm.MonAnDaThem.GhiChu}");

                            // Đảm bảo UserID tồn tại trong database
                            string userID = frm.MonAnDaThem.KeHoachAnID;
                            var userExists = _dbContext.Users.Any(u => u.UserID == userID);
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] UserID '{userID}' tồn tại: {userExists}");
                            
                            if (!userExists)
                            {
                                System.Diagnostics.Debug.WriteLine($"[DEBUG] Tạo user mặc định: {userID}");
                                // Tìm UserID đầu tiên có sẵn trong database
                                var firstUser = _dbContext.Users.FirstOrDefault();
                                if (firstUser != null)
                                {
                                    userID = firstUser.UserID;
                                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Sử dụng UserID có sẵn: {userID}");
                                }
                                else
                                {
                                    // Tạo user mặc định - chỉ dùng các field có trong database
                                    var defaultUser = new Users
                                    {
                                        UserID = userID,
                                        Username = "default_user",
                                        PasswordHash = "default_hash", // Required field
                                        // Chỉ set các field có trong database, không set SDT và các field khác
                                        Email = "default@example.com",
                                        HoTen = "Default User",
                                        CreatedDate = DateTime.Now
                                        // Không set: SDT, NgaySinh, GioiTinh, AnhDaiDien, Theme, NgonNgu, TimeZone, ResetToken, ResetTokenExpiry
                                    };
                                    try
                                    {
                                        _dbContext.Users.Add(defaultUser);
                                        _dbContext.SaveChanges();
                                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã tạo user mặc định thành công");
                                    }
                                    catch (Exception userEx)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Lỗi khi tạo user: {userEx.Message}");
                                        if (userEx.InnerException != null)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception: {userEx.InnerException.Message}");
                                        }
                                        // Nếu không tạo được, thử dùng UserID đầu tiên có sẵn
                                        var anyUser = _dbContext.Users.FirstOrDefault();
                                        if (anyUser != null)
                                        {
                                            userID = anyUser.UserID;
                                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Sử dụng UserID có sẵn: {userID}");
                                        }
                                        else
                                        {
                                            throw new Exception($"Không thể tạo user mặc định và không có user nào trong database. Vui lòng tạo user trước.", userEx);
                                        }
                                    }
                                }
                            }

                            // Đảm bảo MonAnID tồn tại
                            var monAnExists = _dbContext.ThuVienMonAn.Any(m => m.MonAnID == frm.MonAnDaThem.MonAnID);
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] MonAnID '{frm.MonAnDaThem.MonAnID}' tồn tại: {monAnExists}");
                            
                            if (!monAnExists)
                            {
                                throw new Exception($"MonAnID '{frm.MonAnDaThem.MonAnID}' không tồn tại trong database.");
                            }

                            // Lưu vào database
                            // Tạo một object mới chỉ chứa các field có trong database
                            var nhatKyDinhDuong = new BuaAnChiTiet
                            {
                                BuaAnID = frm.MonAnDaThem.BuaAnID,
                                KeHoachAnID = userID, // Dùng UserID đã đảm bảo tồn tại
                                MonAnID = frm.MonAnDaThem.MonAnID,
                                NgayAn = frm.MonAnDaThem.NgayAn, // Map vào NgayGhiLog
                                KhoiLuongChuan = frm.MonAnDaThem.KhoiLuongChuan, // Map vào LuongThucAn
                                GhiChu = frm.MonAnDaThem.GhiChu
                                // Không set các field [NotMapped] và navigation properties
                            };
                            
                            _dbContext.BuaAnChiTiet.Add(nhatKyDinhDuong);
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã thêm vào DbContext với UserID: {userID}");

                            _dbContext.SaveChanges();
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã lưu thành công vào database");

                            // Reload danh sách
                            LoadNhatKyBuaAn();
                        }
                        catch (DbUpdateException dbEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== DB UPDATE EXCEPTION KHI LƯU =====");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Exception Type: {dbEx.GetType().FullName}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {dbEx.Message}");
                            
                            // Extract inner exception (thường là SqlException)
                            Exception innerEx = dbEx.InnerException;
                            while (innerEx != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception Type: {innerEx.GetType().FullName}");
                                System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception Message: {innerEx.Message}");
                                
                                if (innerEx is SqlException sqlEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[DEBUG] SQL Error Number: {sqlEx.Number}");
                                    System.Diagnostics.Debug.WriteLine($"[DEBUG] SQL Error State: {sqlEx.State}");
                                    System.Diagnostics.Debug.WriteLine($"[DEBUG] SQL Error Severity: {sqlEx.Class}");
                                    
                                    if (sqlEx.Errors != null && sqlEx.Errors.Count > 0)
                                    {
                                        foreach (SqlError error in sqlEx.Errors)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"[DEBUG] SQL Error Detail - Number: {error.Number}, Message: {error.Message}, LineNumber: {error.LineNumber}, Procedure: {error.Procedure}");
                                        }
                                    }
                                }
                                
                                innerEx = innerEx.InnerException;
                            }

                            // Build error message
                            string errorMsg = $"Lỗi khi lưu món ăn:\n\n";
                            errorMsg += $"Loại lỗi: {dbEx.GetType().Name}\n";
                            errorMsg += $"Thông báo: {dbEx.Message}\n\n";
                            
                            // Get the most inner exception (thường là SqlException)
                            Exception deepestEx = dbEx;
                            while (deepestEx.InnerException != null)
                            {
                                deepestEx = deepestEx.InnerException;
                            }
                            
                            if (deepestEx is SqlException sqlException)
                            {
                                errorMsg += $"Chi tiết SQL:\n";
                                errorMsg += $"Mã lỗi: {sqlException.Number}\n";
                                errorMsg += $"Thông báo: {sqlException.Message}\n";
                                
                                // Thông báo lỗi cụ thể dựa trên mã lỗi
                                switch (sqlException.Number)
                                {
                                    case 547: // Foreign key constraint violation
                                        errorMsg += "\nLỗi ràng buộc khóa ngoại:\n";
                                        errorMsg += "- UserID không tồn tại trong bảng Users\n";
                                        errorMsg += "- Hoặc MonAnID không tồn tại trong bảng DinhDuongMonAn\n";
                                        break;
                                    case 2627: // Primary key violation
                                    case 2601: // Unique constraint violation
                                        errorMsg += "\nLỗi trùng khóa:\n";
                                        errorMsg += "- DinhDuongID đã tồn tại\n";
                                        break;
                                    case 515: // Cannot insert NULL
                                        errorMsg += "\nLỗi giá trị NULL:\n";
                                        errorMsg += "- Một trường bắt buộc (NOT NULL) chưa được điền\n";
                                        break;
                                }
                                
                                if (sqlException.Errors != null && sqlException.Errors.Count > 0)
                                {
                                    errorMsg += "\nChi tiết lỗi:\n";
                                    foreach (SqlError error in sqlException.Errors)
                                    {
                                        errorMsg += $"- {error.Message}\n";
                                    }
                                }
                            }
                            else
                            {
                                errorMsg += $"Chi tiết: {deepestEx.Message}";
                            }
                            
                            MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (SqlException sqlEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== SQL EXCEPTION KHI LƯU =====");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Number: {sqlEx.Number}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {sqlEx.Message}");
                            
                            if (sqlEx.Errors != null && sqlEx.Errors.Count > 0)
                            {
                                foreach (SqlError error in sqlEx.Errors)
                                {
                                    System.Diagnostics.Debug.WriteLine($"[DEBUG] SQL Error - Number: {error.Number}, Message: {error.Message}, LineNumber: {error.LineNumber}");
                                }
                            }

                            string errorMsg = $"Lỗi SQL khi lưu món ăn:\n\n";
                            errorMsg += $"Mã lỗi: {sqlEx.Number}\n";
                            errorMsg += $"Thông báo: {sqlEx.Message}\n";
                            if (sqlEx.InnerException != null)
                            {
                                errorMsg += $"\nChi tiết: {sqlEx.InnerException.Message}";
                            }
                            MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== EXCEPTION KHI LƯU =====");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Exception Type: {ex.GetType().FullName}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Stack Trace: {ex.StackTrace}");
                            
                            Exception innerEx = ex.InnerException;
                            int depth = 0;
                            while (innerEx != null && depth < 5)
                            {
                                System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception [{depth}] Type: {innerEx.GetType().FullName}");
                                System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception [{depth}] Message: {innerEx.Message}");
                                innerEx = innerEx.InnerException;
                                depth++;
                            }

                            string errorMsg = $"Lỗi khi lưu món ăn:\n\n";
                            errorMsg += $"Loại lỗi: {ex.GetType().Name}\n";
                            errorMsg += $"Thông báo: {ex.Message}\n";
                            
                            // Get deepest inner exception
                            Exception deepestEx = ex;
                            while (deepestEx.InnerException != null)
                            {
                                deepestEx = deepestEx.InnerException;
                            }
                            if (deepestEx != ex)
                            {
                                errorMsg += $"\nChi tiết: {deepestEx.Message}";
                            }
                            
                            MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm món ăn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UcMonAn_XoaClicked(object sender, BuaAnChiTiet monAn)
        {
            try
            {
                var item = _dbContext.BuaAnChiTiet.Find(monAn.BuaAnID);
                if (item != null)
                {
                    _dbContext.BuaAnChiTiet.Remove(item);
                    _dbContext.SaveChanges();
                    LoadNhatKyBuaAn();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa món ăn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTongDinhDuong()
        {
            try
            {
                double tongCalories = _danhSachMonAnDaThem.Sum(m => m.Calories ?? 0);
                double tongProtein = _danhSachMonAnDaThem.Sum(m => m.Protein ?? 0);
                double tongCarbs = _danhSachMonAnDaThem.Sum(m => m.Carbs ?? 0);
                double tongFat = _danhSachMonAnDaThem.Sum(m => m.Fat ?? 0);

                // Cập nhật các label tổng dinh dưỡng (nếu có)
                if (lblChisoCalo != null)
                    lblChisoCalo.Text = tongCalories.ToString("F0");
                if (lblChisoProtein != null)
                    lblChisoProtein.Text = tongProtein.ToString("F1");
                if (lblChisoCarbs != null)
                    lblChisoCarbs.Text = tongCarbs.ToString("F1");
                if (lblChisoChatbeo != null)
                    lblChisoChatbeo.Text = tongFat.ToString("F1");
            }
            catch (Exception ex)
            {
                // Silent fail
            }
        }

        // Cleanup resources when control is disposed
        private void CleanupResources()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }
        }
    }
}
