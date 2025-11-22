using HealthApp.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace HealthApp.Views.Nutrition
{
    public partial class frmThemMonAn : Form
    {
        public BuaAnChiTiet MonAnDaThem { get; private set; }
        private ThuVienMonAn _monAnGoc;
        private WF_HealthTracker _dbContext;

        public frmThemMonAn(ThuVienMonAn monAn, WF_HealthTracker dbContext)
        {
            InitializeComponent();
            _monAnGoc = monAn;
            _dbContext = dbContext;
            InitializeData();
        }

        private void InitializeData()
        {
            // Hiển thị thông tin món ăn
            if (_monAnGoc != null)
            {
                lblTenMonAn.Text = _monAnGoc.TenMonAn;
                lblCalories.Text = $"Calories: {_monAnGoc.Calories ?? 0} kcal";
                lblProtein.Text = $"Protein: {_monAnGoc.Protein ?? 0}g";
                lblCarbs.Text = $"Carbs: {_monAnGoc.Carbs ?? 0}g";
                lblFat.Text = $"Fat: {_monAnGoc.Fat ?? 0}g";

                // Set đơn vị
                if (!string.IsNullOrEmpty(_monAnGoc.Donvi))
                {
                    lblDonVi.Text = $"Đơn vị: {_monAnGoc.Donvi}";
                }

                // Set giá trị mặc định (database không có KhoiLuongChuan, dùng 100g mặc định)
                txtSoLuong.Text = "100"; // Mặc định 100g
            }

            // Load loại bữa ăn
            cboLoaiBuaAn.Items.AddRange(new string[] { "Sáng", "Trưa", "Tối", "Bữa phụ" });
            cboLoaiBuaAn.SelectedIndex = 0;

            // Set ngày mặc định là hôm nay
            dtpNgayAn.Value = DateTime.Today;

            // Tính toán lại khi số lượng thay đổi
            txtSoLuong.TextChanged += TxtSoLuong_TextChanged;
        }

        private void TxtSoLuong_TextChanged(object sender, EventArgs e)
        {
            CalculateNutrition();
        }

        private void CalculateNutrition()
        {
            if (_monAnGoc == null) return;

            if (double.TryParse(txtSoLuong.Text, out double soLuong))
            {
                // Database không có KhoiLuongChuan, luôn tính theo 100g
                double tiLe = soLuong / 100.0;

                double calories = (_monAnGoc.Calories ?? 0) * tiLe;
                double protein = (_monAnGoc.Protein ?? 0) * tiLe;
                double carbs = (_monAnGoc.Carbs ?? 0) * tiLe;
                double fat = (_monAnGoc.Fat ?? 0) * tiLe;

                lblCalories.Text = $"Calories: {calories:F0} kcal";
                lblProtein.Text = $"Protein: {protein:F1}g";
                lblCarbs.Text = $"Carbs: {carbs:F1}g";
                lblFat.Text = $"Fat: {fat:F1}g";
            }
        }

        private string GetOrCreateDefaultKeHoachAn()
        {
            // Tìm KeHoachAnUong mặc định hoặc tạo mới
            var keHoachAn = _dbContext.KeHoachAnUong
                .Where(k => k.TrangThai == "Active" || k.TrangThai == null)
                .FirstOrDefault();

            if (keHoachAn == null)
            {
                // Tạo mới KeHoachAnUong mặc định
                keHoachAn = new KeHoachAnUong
                {
                    KeHoachAnID = Guid.NewGuid().ToString().Substring(0, 20),
                    TrangThai = "Active",
                    MoTa = "Kế hoạch ăn uống mặc định"
                };
                _dbContext.KeHoachAnUong.Add(keHoachAn);
                _dbContext.SaveChanges();
            }

            return keHoachAn.KeHoachAnID;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(txtSoLuong.Text))
                {
                    MessageBox.Show("Vui lòng nhập số lượng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtSoLuong.Text, out double soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Số lượng phải là số dương!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboLoaiBuaAn.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn loại bữa ăn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tính toán dinh dưỡng (để hiển thị, không lưu vào database)
                double tiLe = soLuong / 100.0;
                double calories = (_monAnGoc.Calories ?? 0) * tiLe;
                double protein = (_monAnGoc.Protein ?? 0) * tiLe;
                double carbs = (_monAnGoc.Carbs ?? 0) * tiLe;
                double fat = (_monAnGoc.Fat ?? 0) * tiLe;

                // UserID mặc định (tạm thời dùng "default_user", sau này sẽ lấy từ session)
                string userID = "default_user";

                // Lưu LoaiBuaAn vào GhiChu (format: "LoaiBuaAn: Breakfast|GhiChu khác")
                string loaiBuaAn = cboLoaiBuaAn.SelectedItem.ToString();
                string ghiChu = $"LoaiBuaAn: {loaiBuaAn}";
                if (!string.IsNullOrWhiteSpace(txtGhiChu.Text))
                {
                    ghiChu += $"|{txtGhiChu.Text}";
                }

                // Debug: Log thông tin trước khi tạo
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== BẮT ĐẦU THÊM MÓN ĂN =====");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] UserID: {userID}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] MonAnID: {_monAnGoc.MonAnID}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] NgayAn: {dtpNgayAn.Value.Date}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] SoLuong: {soLuong}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] GhiChu: {ghiChu}");

                // Kiểm tra UserID có tồn tại trong database không
                var userExists = _dbContext.Users.Any(u => u.UserID == userID);
                System.Diagnostics.Debug.WriteLine($"[DEBUG] UserID '{userID}' tồn tại trong database: {userExists}");
                
                if (!userExists)
                {
                    // Tạo user mặc định nếu chưa có
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Tạo user mặc định: {userID}");
                    var defaultUser = new Users
                    {
                        UserID = userID,
                        Username = "default_user",
                        PasswordHash = "default_hash", // Required field
                        Email = "default@example.com",
                        HoTen = "Default User",
                        CreatedDate = DateTime.Now
                    };
                    try
                    {
                        _dbContext.Users.Add(defaultUser);
                        _dbContext.SaveChanges();
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã tạo user mặc định thành công");
                    }
                    catch (Exception userEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Lỗi khi tạo user mặc định: {userEx.Message}");
                        if (userEx.InnerException != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception: {userEx.InnerException.Message}");
                        }
                        // Tiếp tục thử thêm món ăn
                    }
                }

                // Kiểm tra MonAnID có tồn tại không
                var monAnExists = _dbContext.ThuVienMonAn.Any(m => m.MonAnID == _monAnGoc.MonAnID);
                System.Diagnostics.Debug.WriteLine($"[DEBUG] MonAnID '{_monAnGoc.MonAnID}' tồn tại: {monAnExists}");

                // Tạo DinhDuongID
                string dinhDuongID = $"nut_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 15)}";
                System.Diagnostics.Debug.WriteLine($"[DEBUG] DinhDuongID: {dinhDuongID}");

                // Tạo BuaAnChiTiet (map vào NhatKyDinhDuong)
                MonAnDaThem = new BuaAnChiTiet
                {
                    BuaAnID = dinhDuongID,
                    KeHoachAnID = userID, // Map vào UserID
                    MonAnID = _monAnGoc.MonAnID,
                    NgayAn = dtpNgayAn.Value.Date, // Map vào NgayGhiLog
                    KhoiLuongChuan = soLuong, // Map vào LuongThucAn
                    GhiChu = ghiChu, // Lưu LoaiBuaAn và ghi chú
                    // Các field NotMapped (tính toán khi load)
                    TenMonAn = _monAnGoc.TenMonAn,
                    Donvi = _monAnGoc.Donvi ?? "g",
                    LoaiBuaAn = loaiBuaAn,
                    Calories = calories,
                    Protein = protein,
                    Carbs = carbs,
                    Fat = fat,
                    Fiber = null
                };

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã tạo BuaAnChiTiet object");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] BuaAnID: {MonAnDaThem.BuaAnID}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] KeHoachAnID (UserID): {MonAnDaThem.KeHoachAnID}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] MonAnID: {MonAnDaThem.MonAnID}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] NgayAn: {MonAnDaThem.NgayAn}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] KhoiLuongChuan: {MonAnDaThem.KhoiLuongChuan}");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== SQL EXCEPTION =====");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Number: {sqlEx.Number}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {sqlEx.Message}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Source: {sqlEx.Source}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error State: {sqlEx.State}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Severity: {sqlEx.Class}");
                
                if (sqlEx.Errors != null && sqlEx.Errors.Count > 0)
                {
                    foreach (SqlError error in sqlEx.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] SQL Error Detail - Number: {error.Number}, Message: {error.Message}, LineNumber: {error.LineNumber}");
                    }
                }

                string errorMsg = $"Lỗi SQL khi thêm món ăn:\n\n";
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
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== GENERAL EXCEPTION =====");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Exception Type: {ex.GetType().FullName}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Error Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception Type: {ex.InnerException.GetType().FullName}");
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception Message: {ex.InnerException.Message}");
                    if (ex.InnerException.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Inner Exception: {ex.InnerException.InnerException.Message}");
                    }
                }

                string errorMsg = $"Lỗi khi thêm món ăn:\n\n";
                errorMsg += $"Loại lỗi: {ex.GetType().Name}\n";
                errorMsg += $"Thông báo: {ex.Message}\n";
                if (ex.InnerException != null)
                {
                    errorMsg += $"\nChi tiết: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMsg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

