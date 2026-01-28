using HealthApp.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace HealthApp.Views.Nutrition
{
    public partial class frmThemMonAn : Form
    {
        public BuaAnChiTiet MonAnDaThem { get; private set; }
        private ThuVienMonAn _monAnGoc;
        private WF_HealthTracker _dbContext;
        private string _keHoachAnID;
        private string _loaiBuaAnInitial;
        private DateTime? _ngayAnInitial;

        public frmThemMonAn(ThuVienMonAn monAn, WF_HealthTracker dbContext, string keHoachAnID = null, string loaiBuaAn = null, DateTime? ngayAn = null)
        {
            InitializeComponent();
            _monAnGoc = monAn;
            _dbContext = dbContext;
            _keHoachAnID = keHoachAnID;
            _loaiBuaAnInitial = loaiBuaAn;
            _ngayAnInitial = ngayAn;
            InitializeData();
        }

        private void InitializeData()
        {
            // Hiển thị thông tin món ăn
            if (_monAnGoc != null)
            {
                lblTenMonAn.Text = _monAnGoc.TenMonAn;

                // Set đơn vị
                if (!string.IsNullOrEmpty(_monAnGoc.Donvi))
                {
                    lblDonVi.Text = $"Đơn vị: {_monAnGoc.Donvi}";
                }

                // Set giá trị mặc định dựa trên KhoiLuongChuan nếu có, nếu không thì dùng 100g
                double khoiLuongMacDinh = _monAnGoc.KhoiLuongChuan ?? 100.0;
                txtSoLuong.Text = khoiLuongMacDinh.ToString("F0");
            }

            // Load loại bữa ăn
            cboLoaiBuaAn.Items.AddRange(new string[] { "Sáng", "Trưa", "Tối", "Bữa phụ" });
            var idx = 0;
            if (!string.IsNullOrWhiteSpace(_loaiBuaAnInitial))
            {
                var i = cboLoaiBuaAn.Items.IndexOf(_loaiBuaAnInitial);
                if (i >= 0) idx = i;
            }
            cboLoaiBuaAn.SelectedIndex = idx;

            // Set ngày: ưu tiên _ngayAnInitial, mặc định hôm nay
            dtpNgayAn.Value = _ngayAnInitial ?? DateTime.Today;

            // Tính toán lại khi số lượng thay đổi
            txtSoLuong.TextChanged += TxtSoLuong_TextChanged;
            
            // Tính toán và hiển thị giá trị ban đầu
            CalculateNutrition();
        }

        private void TxtSoLuong_TextChanged(object sender, EventArgs e)
        {
            CalculateNutrition();
        }

        private void CalculateNutrition()
        {
            if (_monAnGoc == null) return;

            if (double.TryParse(txtSoLuong.Text, out double soLuong) && soLuong > 0)
            {
                // Tính toán dựa trên KhoiLuongChuan nếu có, nếu không thì dùng 100g làm chuẩn
                double khoiLuongChuan = _monAnGoc.KhoiLuongChuan ?? 100.0;
                double tiLe = soLuong / khoiLuongChuan;

                // Cho phép các giá trị null - chỉ tính toán nếu có giá trị
                double? calories = _monAnGoc.Calories.HasValue ? _monAnGoc.Calories.Value * tiLe : (double?)null;
                double? protein = _monAnGoc.Protein.HasValue ? _monAnGoc.Protein.Value * tiLe : (double?)null;
                double? carbs = _monAnGoc.Carbs.HasValue ? _monAnGoc.Carbs.Value * tiLe : (double?)null;
                double? fat = _monAnGoc.Fat.HasValue ? _monAnGoc.Fat.Value * tiLe : (double?)null;
                double? fiber = _monAnGoc.Fiber.HasValue ? _monAnGoc.Fiber.Value * tiLe : (double?)null;

                lblCalories.Text = calories.HasValue ? $"Calories: {calories.Value:F0} kcal" : "Calories: —";
                lblProtein.Text = protein.HasValue ? $"Protein: {protein.Value:F1}g" : "Protein: —";
                lblCarbs.Text = carbs.HasValue ? $"Carbs: {carbs.Value:F1}g" : "Carbs: —";
                lblFat.Text = fat.HasValue ? $"Fat: {fat.Value:F1}g" : "Fat: —";
            }
            else
            {
                // Reset về giá trị ban đầu nếu số lượng không hợp lệ
                lblCalories.Text = $"Calories: {_monAnGoc.Calories ?? 0} kcal";
                lblProtein.Text = $"Protein: {_monAnGoc.Protein ?? 0}g";
                lblCarbs.Text = $"Carbs: {_monAnGoc.Carbs ?? 0}g";
                lblFat.Text = $"Fat: {_monAnGoc.Fat ?? 0}g";
            }
        }

        private string GetOrCreateDefaultKeHoachAn()
        {
            // Tìm KeHoachAnUong mặc định hoặc tạo mới
            var keHoachAn = _dbContext.KeHoachAnUong
                .Where(k => k.TrangThai == "Đang hoạt động" || k.TrangThai == null)

                .FirstOrDefault();

            if (keHoachAn == null)
            {
                // Tạo mới KeHoachAnUong mặc định
                keHoachAn = new KeHoachAnUong
                {
                    KeHoachAnID = $"meal_{DateTime.Now:yyyyMMddHHmmss}",
                    TrangThai = "Đang hoạt động",
                    MoTa = "Kế hoạch ăn uống mặc định"
                };
                _dbContext.KeHoachAnUong.Add(keHoachAn);
                _dbContext.SaveChanges();
            }

            return keHoachAn.KeHoachAnID;
        }

        private string GenerateBuaAnID()
        {
            // Dùng timestamp + random để đảm bảo unique, tránh trùng khi nhiều request cùng lúc
            // Format: meal_YYMMDDHHmmssRRR (tối đa 20 ký tự)
            // meal_ = 5, YYMMDDHHmmss = 12, RRR = 3 → tổng = 20
            var timestamp = DateTime.Now.ToString("yyMMddHHmmss");
            var random = new Random();
            var randomPart = random.Next(100, 999).ToString("D3");
            var newID = $"meal_{timestamp}{randomPart}";
            
            // Kiểm tra xem ID có tồn tại chưa (rất hiếm nhưng vẫn kiểm tra)
            int attempt = 0;
            while (_dbContext.BuaAnChiTiet.Any(b => b.BuaAnID == newID) && attempt < 5)
            {
                randomPart = random.Next(100, 999).ToString("D3");
                newID = $"meal_{timestamp}{randomPart}";
                attempt++;
            }
            
            return newID;
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

                // Tính toán dinh dưỡng dựa trên số lượng nhập vào
                // Sử dụng KhoiLuongChuan nếu có, nếu không thì dùng 100g làm chuẩn
                double khoiLuongChuan = _monAnGoc.KhoiLuongChuan ?? 100.0;
                double tiLe = soLuong / khoiLuongChuan;

                // Cho phép các giá trị null - chỉ tính toán nếu có giá trị gốc
                double? calories = _monAnGoc.Calories.HasValue ? _monAnGoc.Calories.Value * tiLe : (double?)null;
                double? protein = _monAnGoc.Protein.HasValue ? _monAnGoc.Protein.Value * tiLe : (double?)null;
                double? carbs = _monAnGoc.Carbs.HasValue ? _monAnGoc.Carbs.Value * tiLe : (double?)null;
                double? fat = _monAnGoc.Fat.HasValue ? _monAnGoc.Fat.Value * tiLe : (double?)null;
                double? fiber = _monAnGoc.Fiber.HasValue ? _monAnGoc.Fiber.Value * tiLe : (double?)null;

                // Đảm bảo các giá trị số không phải NaN hoặc Infinity (chỉ kiểm tra nếu không null)
                if (calories.HasValue && (double.IsNaN(calories.Value) || double.IsInfinity(calories.Value)))
                    calories = null;
                if (protein.HasValue && (double.IsNaN(protein.Value) || double.IsInfinity(protein.Value)))
                    protein = null;
                if (carbs.HasValue && (double.IsNaN(carbs.Value) || double.IsInfinity(carbs.Value)))
                    carbs = null;
                if (fat.HasValue && (double.IsNaN(fat.Value) || double.IsInfinity(fat.Value)))
                    fat = null;
                if (fiber.HasValue && (double.IsNaN(fiber.Value) || double.IsInfinity(fiber.Value)))
                    fiber = null;
                
                if (double.IsNaN(soLuong) || double.IsInfinity(soLuong) || soLuong <= 0)
                    throw new Exception("Số lượng không hợp lệ.");

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Tính toán dinh dưỡng:");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] KhoiLuongChuan gốc: {khoiLuongChuan}g");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Số lượng nhập: {soLuong}g");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Tỷ lệ: {tiLe:F4}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Calories: {calories?.ToString("F2") ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Protein: {protein?.ToString("F2") ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Carbs: {carbs?.ToString("F2") ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Fat: {fat?.ToString("F2") ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Fiber: {fiber?.ToString("F2") ?? "null"}");

                // Lấy UserID từ CurrentUser
                string userID = HealthApp.Common.Helpers.CurrentUser.UserID;
                if (string.IsNullOrEmpty(userID))
                {
                    MessageBox.Show("Vui lòng đăng nhập để thêm món ăn!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // LoaiBuaAn: DB CHECK chỉ cho phép 'Sáng','Trưa','Tối','Phụ' — dùng "Phụ" thay "Bữa phụ"
                string loaiBuaAn = cboLoaiBuaAn.SelectedItem.ToString();
                string loaiDb = loaiBuaAn.Equals("Bữa phụ", StringComparison.OrdinalIgnoreCase) ? "Phụ" : loaiBuaAn;
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

                // Tạo BuaAnID theo format meal_YYMMDDHHmmssRRR (đảm bảo unique)
                string buaAnID = GenerateBuaAnID();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] BuaAnID: {buaAnID}");
                
                // Đảm bảo BuaAnID không null/empty (là key nên bắt buộc phải có)
                if (string.IsNullOrWhiteSpace(buaAnID))
                    throw new Exception("Không thể tạo BuaAnID. Vui lòng thử lại.");

                // Sử dụng KeHoachAnID nếu có, nếu không thì tạo mới
                string keHoachAnID = _keHoachAnID;
                if (string.IsNullOrEmpty(keHoachAnID))
                {
                    keHoachAnID = GetOrCreateDefaultKeHoachAn();
                }

                // Kiểm tra KeHoachAnID có tồn tại trong database không
                var keHoachAnExists = _dbContext.KeHoachAnUong.Any(k => k.KeHoachAnID == keHoachAnID);
                if (!keHoachAnExists)
                {
                    throw new Exception($"KeHoachAnID '{keHoachAnID}' không tồn tại trong database. Vui lòng thử lại.");
                }

                // Kiểm tra MonAnID có tồn tại trong database không
                if (!monAnExists)
                {
                    throw new Exception($"MonAnID '{_monAnGoc.MonAnID}' không tồn tại trong database.");
                }

                // Đảm bảo các trường required được set đúng và không vượt quá giới hạn
                // TenMonAn là REQUIRED - phải có giá trị
                string tenMonAn = (_monAnGoc.TenMonAn ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(tenMonAn))
                {
                    // Nếu TenMonAn null/empty, dùng tên từ MonAnID hoặc giá trị mặc định
                    tenMonAn = !string.IsNullOrWhiteSpace(_monAnGoc.MonAnID) 
                        ? $"Món ăn {_monAnGoc.MonAnID}" 
                        : "Món ăn không tên";
                }
                if (tenMonAn.Length > 200)
                    tenMonAn = tenMonAn.Substring(0, 200);
                
                // Donvi không required nhưng nên có giá trị mặc định
                string donVi = (_monAnGoc.Donvi ?? "g").Trim();
                if (string.IsNullOrWhiteSpace(donVi))
                    donVi = "g";
                if (donVi.Length > 10)
                    donVi = donVi.Substring(0, 10);
                
                // Đảm bảo TenMonAn không null/empty sau khi xử lý
                if (string.IsNullOrWhiteSpace(tenMonAn))
                    throw new Exception("Tên món ăn không được để trống.");
                
                string ghiChuFinal = (ghiChu ?? string.Empty).Trim();
                if (ghiChuFinal.Length > 500)
                    ghiChuFinal = ghiChuFinal.Substring(0, 500);

                // Đảm bảo BuaAnID không vượt quá 20 ký tự (format mới đã đảm bảo ≤ 20)
                if (buaAnID.Length > 20)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] WARNING: BuaAnID quá dài ({buaAnID.Length}): {buaAnID}");
                    buaAnID = buaAnID.Substring(0, 20);
                }

                // Đảm bảo KeHoachAnID và MonAnID không vượt quá 20 ký tự và không null/empty
                if (string.IsNullOrWhiteSpace(keHoachAnID))
                    throw new Exception("KeHoachAnID không được để trống.");
                if (keHoachAnID.Length > 20)
                    throw new Exception($"KeHoachAnID quá dài (tối đa 20 ký tự): {keHoachAnID}");
                
                if (string.IsNullOrWhiteSpace(_monAnGoc.MonAnID))
                    throw new Exception("MonAnID không được để trống.");
                if (_monAnGoc.MonAnID.Length > 20)
                    throw new Exception($"MonAnID quá dài (tối đa 20 ký tự): {_monAnGoc.MonAnID}");

                // Đảm bảo LoaiBuaAn không vượt quá 50 ký tự và đúng format
                if (string.IsNullOrWhiteSpace(loaiDb))
                    throw new Exception("LoaiBuaAn không được để trống.");
                if (loaiDb.Length > 50)
                    loaiDb = loaiDb.Substring(0, 50);

                // Validate tất cả các trường required trước khi tạo entity
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== VALIDATION TRƯỚC KHI TẠO ENTITY =====");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] BuaAnID: '{buaAnID}' (Length: {buaAnID?.Length ?? 0}, IsNullOrEmpty: {string.IsNullOrEmpty(buaAnID)})");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] KeHoachAnID: '{keHoachAnID}' (Length: {keHoachAnID?.Length ?? 0}, IsNullOrEmpty: {string.IsNullOrEmpty(keHoachAnID)})");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] MonAnID: '{_monAnGoc.MonAnID}' (Length: {_monAnGoc.MonAnID?.Length ?? 0}, IsNullOrEmpty: {string.IsNullOrEmpty(_monAnGoc.MonAnID)})");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] LoaiBuaAn: '{loaiDb}' (Length: {loaiDb?.Length ?? 0}, IsNullOrEmpty: {string.IsNullOrEmpty(loaiDb)})");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] TenMonAn: '{tenMonAn}' (Length: {tenMonAn?.Length ?? 0}, IsNullOrEmpty: {string.IsNullOrEmpty(tenMonAn)})");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Donvi: '{donVi}' (Length: {donVi?.Length ?? 0}, IsNullOrEmpty: {string.IsNullOrEmpty(donVi)})");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] KhoiLuongChuan: {soLuong}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Calories: {calories?.ToString() ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Protein: {protein?.ToString() ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Carbs: {carbs?.ToString() ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Fat: {fat?.ToString() ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Fiber: {fiber?.ToString() ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] NgayAn: {dtpNgayAn.Value.Date}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] NgayCapNhat: {DateTime.Now}");

                // Tạo BuaAnChiTiet - cho phép các giá trị null cho calories, protein, carbs, fat, fiber
                MonAnDaThem = new BuaAnChiTiet
                {
                    BuaAnID = buaAnID,
                    KeHoachAnID = keHoachAnID,
                    MonAnID = _monAnGoc.MonAnID,
                    LoaiBuaAn = loaiDb,
                    NgayAn = dtpNgayAn.Value.Date,
                    TenMonAn = tenMonAn,
                    Donvi = donVi,
                    KhoiLuongChuan = soLuong,
                    Calories = calories, // Cho phép null
                    Protein = protein, // Cho phép null
                    Carbs = carbs, // Cho phép null
                    Fat = fat, // Cho phép null
                    Fiber = fiber, // Cho phép null
                    GhiChu = string.IsNullOrWhiteSpace(ghiChuFinal) ? null : ghiChuFinal, // Cho phép null nếu empty
                    NgayCapNhat = DateTime.Now
                };

                // Validate entity trước khi add vào context
                var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(MonAnDaThem);
                bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(MonAnDaThem, validationContext, validationResults, true);
                
                if (!isValid)
                {
                    var errorDetails = new StringBuilder();
                    errorDetails.AppendLine("Lỗi validation trước khi lưu:");
                    foreach (var result in validationResults)
                    {
                        errorDetails.AppendLine($"  - {string.Join(", ", result.MemberNames)}: {result.ErrorMessage}");
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Validation Error: {string.Join(", ", result.MemberNames)} - {result.ErrorMessage}");
                    }
                    throw new Exception(errorDetails.ToString());
                }

                _dbContext.BuaAnChiTiet.Add(MonAnDaThem);
                _dbContext.SaveChanges();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception dbValEx) when (dbValEx.GetType().Name == "DbEntityValidationException" || 
                                             dbValEx.Message.Contains("Validation failed") ||
                                             (dbValEx.InnerException != null && dbValEx.InnerException.GetType().Name == "DbEntityValidationException"))
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ===== DB ENTITY VALIDATION EXCEPTION =====");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Exception Type: {dbValEx.GetType().FullName}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Exception Message: {dbValEx.Message}");
                
                var errorMsg = new StringBuilder();
                errorMsg.AppendLine("Lỗi validation khi thêm món ăn:");
                errorMsg.AppendLine();
                errorMsg.AppendLine("Chi tiết các lỗi:");
                errorMsg.AppendLine();

                // Sử dụng reflection để truy cập EntityValidationErrors nếu có
                try
                {
                    var entityValidationErrorsProperty = dbValEx.GetType().GetProperty("EntityValidationErrors");
                    if (entityValidationErrorsProperty != null)
                    {
                        var entityValidationErrors = entityValidationErrorsProperty.GetValue(dbValEx) as System.Collections.IEnumerable;
                        if (entityValidationErrors != null)
                        {
                            foreach (var validationError in entityValidationErrors)
                            {
                                var entryProperty = validationError.GetType().GetProperty("Entry");
                                var errorsProperty = validationError.GetType().GetProperty("ValidationErrors");
                                
                                if (entryProperty != null)
                                {
                                    var entry = entryProperty.GetValue(validationError);
                                    var entityProperty = entry?.GetType().GetProperty("Entity");
                                    var stateProperty = entry?.GetType().GetProperty("State");
                                    
                                    if (entityProperty != null)
                                    {
                                        var entity = entityProperty.GetValue(entry);
                                        errorMsg.AppendLine($"Entity: {entity?.GetType().Name ?? "Unknown"}");
                                        
                                        if (stateProperty != null)
                                        {
                                            errorMsg.AppendLine($"State: {stateProperty.GetValue(entry)}");
                                        }
                                        
                                        // Hiển thị giá trị hiện tại của entity
                                        if (entity is BuaAnChiTiet entityBuaAn)
                                        {
                                            errorMsg.AppendLine($"Giá trị hiện tại:");
                                            errorMsg.AppendLine($"  - BuaAnID: '{entityBuaAn.BuaAnID ?? "null"}'");
                                            errorMsg.AppendLine($"  - KeHoachAnID: '{entityBuaAn.KeHoachAnID ?? "null"}'");
                                            errorMsg.AppendLine($"  - MonAnID: '{entityBuaAn.MonAnID ?? "null"}'");
                                            errorMsg.AppendLine($"  - LoaiBuaAn: '{entityBuaAn.LoaiBuaAn ?? "null"}'");
                                            errorMsg.AppendLine($"  - TenMonAn: '{entityBuaAn.TenMonAn ?? "null"}'");
                                            errorMsg.AppendLine($"  - Donvi: '{entityBuaAn.Donvi ?? "null"}'");
                                        }
                                    }
                                }
                                
                                errorMsg.AppendLine();
                                errorMsg.AppendLine("Các lỗi validation:");
                                
                                if (errorsProperty != null)
                                {
                                    var errors = errorsProperty.GetValue(validationError) as System.Collections.IEnumerable;
                                    if (errors != null)
                                    {
                                        foreach (var error in errors)
                                        {
                                            var propertyNameProperty = error.GetType().GetProperty("PropertyName");
                                            var errorMessageProperty = error.GetType().GetProperty("ErrorMessage");
                                            
                                            string propName = propertyNameProperty?.GetValue(error)?.ToString() ?? "Unknown";
                                            string errMsg = errorMessageProperty?.GetValue(error)?.ToString() ?? "Unknown error";
                                            
                                            errorMsg.AppendLine($"  ✗ Property: '{propName}'");
                                            errorMsg.AppendLine($"    Lỗi: {errMsg}");
                                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Property: {propName}, Error: {errMsg}");
                                        }
                                    }
                                }
                                errorMsg.AppendLine();
                            }
                        }
                    }
                }
                catch (Exception reflectionEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Error accessing EntityValidationErrors via reflection: {reflectionEx.Message}");
                    errorMsg.AppendLine("Không thể truy cập chi tiết validation errors.");
                    errorMsg.AppendLine($"Lỗi: {dbValEx.Message}");
                }

                // Thêm thông tin về InnerException nếu có
                if (dbValEx.InnerException != null)
                {
                    errorMsg.AppendLine($"Chi tiết kỹ thuật:");
                    errorMsg.AppendLine(dbValEx.InnerException.Message);
                }

                MessageBox.Show(errorMsg.ToString(), "Lỗi Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

