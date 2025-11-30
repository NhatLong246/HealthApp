extern alias ef6;

using HealthApp.Models;
using HealthApp.Common.Helpers;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ef6::System.Data.Entity.Validation;

namespace HealthApp.Views.Nutrition
{
    public partial class ucMonAnItem : UserControl
    {
        public ThuVienMonAn MonAn { get; private set; }
        public event EventHandler<ThuVienMonAn> MonAnClicked;
        public event EventHandler<BuaAnChiTiet> MonAnDeleted; // Event khi món ăn bị xóa
        public event EventHandler<BuaAnChiTiet> MonAnUpdated; // Event khi món ăn được cập nhật
        private BuaAnChiTiet _buaAnChiTiet;
        
        /// <summary>
        /// Nếu true, sẽ tự động mở frmChinhSuaMonAn khi click. Nếu false, chỉ raise event MonAnClicked.
        /// </summary>
        public bool AutoOpenEditForm { get; set; } = true;

        // Lưu giá trị dinh dưỡng hiện tại để tính toán biểu đồ
        private double _currentCalories = 0;
        private double _currentProtein = 0;
        private double _currentCarbs = 0;
        private double _currentFat = 0;

        /// <summary>
        /// Lấy giá trị dinh dưỡng hiện tại của món ăn (đã tính theo số lượng)
        /// </summary>
        public void GetCurrentNutrition(out double calories, out double protein, out double carbs, out double fat)
        {
            calories = _currentCalories;
            protein = _currentProtein;
            carbs = _currentCarbs;
            fat = _currentFat;
        }

        public ucMonAnItem(ThuVienMonAn monAn)
        {
            InitializeComponent();
            pnlMonAn.Dock = DockStyle.Fill;
            MonAn = monAn;

            // Điều chỉnh layout khi control thay đổi kích thước để vừa với mọi panel chứa
            this.Resize += UcMonAnItem_Resize;

            LoadData();
            UcMonAnItem_Resize(this, EventArgs.Empty);
        }

        public void LoadData()
        {
            if (MonAn == null) return;

            try
            {
                // Tìm BuaAnChiTiet đã có cho món ăn này hôm nay (nếu user đã đăng nhập)
                if (CurrentUser.IsLoggedIn)
                {
                    using (var dbContext = new WF_HealthTracker())
                    {
                        string keHoachAnID = GetOrCreateKeHoachAnUong(dbContext);
                        if (!string.IsNullOrEmpty(keHoachAnID))
                        {
                            var ngayHomNay = DateTime.Today;
                            var ngayBatDau = ngayHomNay.Date;
                            var ngayKetThuc = ngayHomNay.Date.AddDays(1).AddTicks(-1);

                            _buaAnChiTiet = dbContext.BuaAnChiTiet
                                .Where(b => b.KeHoachAnID == keHoachAnID &&
                                       b.MonAnID == MonAn.MonAnID &&
                                       b.NgayAn >= ngayBatDau && 
                                       b.NgayAn < ngayKetThuc)
                                .FirstOrDefault();
                        }
                    }
                }

                // Xác định số lượng và đơn vị
                double soLuong;
                string donVi;

                if (_buaAnChiTiet != null)
                {
                    // Dùng số lượng đã chỉnh sửa từ BuaAnChiTiet
                    soLuong = _buaAnChiTiet.KhoiLuongChuan ?? MonAn.KhoiLuongChuan ?? 100;
                    donVi = _buaAnChiTiet.Donvi ?? MonAn.Donvi ?? "g";
                }
                else
                {
                    // Dùng số lượng mặc định từ ThuVienMonAn
                    soLuong = MonAn.KhoiLuongChuan ?? 100;
                    donVi = MonAn.Donvi ?? "g";
                }

                // Hiển thị: "Tên món ăn - số lượng + đơn vị"
                lblTenMonAn.Text = $"{MonAn.TenMonAn} - {soLuong}{donVi}";

                // Tính toán dinh dưỡng dựa trên số lượng thực tế
                // ThuVienMonAn lưu giá trị dinh dưỡng cho KhoiLuongChuan
                double khoiLuongChuan = MonAn.KhoiLuongChuan ?? 100;
                if (khoiLuongChuan <= 0) khoiLuongChuan = 100; // Tránh chia cho 0
                double tiLe = soLuong / khoiLuongChuan;

                double calories = (MonAn.Calories ?? 0) * tiLe;
                double protein = (MonAn.Protein ?? 0) * tiLe;
                double carbs = (MonAn.Carbs ?? 0) * tiLe;
                double fat = (MonAn.Fat ?? 0) * tiLe;

                // Lưu giá trị dinh dưỡng hiện tại
                _currentCalories = calories;
                _currentProtein = protein;
                _currentCarbs = carbs;
                _currentFat = fat;

                lblCalories.Text = $"{calories:F0} kcal";
                lblProtein.Text = $"P: {protein:F1}g";
                lblCarbs.Text = $"C: {carbs:F1}g";
                lblFat.Text = $"F: {fat:F1}g";

                UcMonAnItem_Resize(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, hiển thị giá trị mặc định
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Lỗi khi load data: {ex.Message}");
                lblTenMonAn.Text = MonAn.TenMonAn;
                lblCalories.Text = $"{MonAn.Calories ?? 0} kcal";
                lblProtein.Text = $"P: {MonAn.Protein ?? 0}g";
                lblCarbs.Text = $"C: {MonAn.Carbs ?? 0}g";
                lblFat.Text = $"F: {MonAn.Fat ?? 0}g";
                UcMonAnItem_Resize(this, EventArgs.Empty);
            }
        }

        private void UcMonAnItem_Resize(object sender, EventArgs e)
        {
            // Tăng padding bên trái để cân bằng, giảm khoảng trống bên trái
            int horizontalPadding = 25;
            int metricsSpacing = 15;

            // Vị trí hàng macro (dưới cùng)
            int metricsTop = Math.Max(lblCalories.Top, pnlMonAn.Height - 35);
            lblCalories.Location = new Point(horizontalPadding, metricsTop);

            lblProtein.Location = new Point(lblCalories.Right + metricsSpacing, metricsTop);
            lblCarbs.Location = new Point(lblProtein.Right + metricsSpacing, metricsTop);
            lblFat.Location = new Point(lblCarbs.Right + metricsSpacing, metricsTop);

            // Canh phải nếu tổng width lớn hơn panel
            int rightEdge = pnlMonAn.Width - horizontalPadding;
            if (lblFat.Right > rightEdge)
            {
                lblFat.Left = rightEdge - lblFat.Width;
                lblCarbs.Left = lblFat.Left - metricsSpacing - lblCarbs.Width;
                lblProtein.Left = lblCarbs.Left - metricsSpacing - lblProtein.Width;
            }

            // Đảm bảo tên món căng ngang panel và căn trái với padding
            lblTenMonAn.Location = new Point(horizontalPadding, 15);
            lblTenMonAn.MaximumSize = new Size(pnlMonAn.Width - horizontalPadding * 2, 0);
        }

        private void pnlMonAn_Click(object sender, EventArgs e)
        {
            // Nếu AutoOpenEditForm = true, mở form chỉnh sửa
            // Nếu false, chỉ raise event để parent xử lý (ví dụ: mở frmThemMonAn)
            if (AutoOpenEditForm)
            {
                OpenChinhSuaMonAnForm();
            }
            
            // Luôn raise event để parent có thể xử lý
            MonAnClicked?.Invoke(this, MonAn);
        }

        private void lblTenMonAn_Click(object sender, EventArgs e)
        {
            pnlMonAn_Click(sender, e);
        }

        private void OpenChinhSuaMonAnForm()
        {
            try
            {
                if (MonAn == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin món ăn!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra đăng nhập
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập để sử dụng tính năng này!", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var dbContext = new WF_HealthTracker())
                {
                    // Tìm hoặc tạo KeHoachAnUong
                    string keHoachAnID = GetOrCreateKeHoachAnUong(dbContext);
                    if (string.IsNullOrEmpty(keHoachAnID))
                    {
                        // Lỗi đã được hiển thị trong GetOrCreateKeHoachAnUong
                        return;
                    }

                    // Tìm BuaAnChiTiet đã có cho món ăn này hôm nay
                    var ngayHomNay = DateTime.Today;
                    var ngayBatDau = ngayHomNay.Date;
                    var ngayKetThuc = ngayHomNay.Date.AddDays(1).AddTicks(-1);

                    var buaAnChiTiet = dbContext.BuaAnChiTiet
                        .Where(b => b.KeHoachAnID == keHoachAnID &&
                               b.MonAnID == MonAn.MonAnID &&
                               b.NgayAn >= ngayBatDau && 
                               b.NgayAn < ngayKetThuc)
                        .FirstOrDefault();

                    // Nếu chưa có, tạo mới BuaAnChiTiet với giá trị mặc định từ ThuVienMonAn
                    if (buaAnChiTiet == null)
                    {
                        // Tạo BuaAnID theo format meal_xxxx (giống frmThemMonAn)
                        string buaAnID = GenerateBuaAnID(dbContext);
                        
                        // Dùng KhoiLuongChuan và Donvi từ ThuVienMonAn
                        double khoiLuongChuan = MonAn.KhoiLuongChuan ?? 100;
                        string donVi = MonAn.Donvi ?? "g";
                        
                        // Tính toán dinh dưỡng dựa trên KhoiLuongChuan từ ThuVienMonAn
                        // ThuVienMonAn lưu giá trị dinh dưỡng cho KhoiLuongChuan
                        // Vì dùng chính KhoiLuongChuan làm số lượng ban đầu, tỷ lệ = 1.0
                        double tiLe = 1.0;
                        
                        buaAnChiTiet = new BuaAnChiTiet
                        {
                            BuaAnID = buaAnID,
                            KeHoachAnID = keHoachAnID,
                            MonAnID = MonAn.MonAnID ?? throw new Exception("MonAnID không được null"),
                            TenMonAn = MonAn.TenMonAn ?? throw new Exception("TenMonAn không được null"),
                            Donvi = donVi,
                            KhoiLuongChuan = khoiLuongChuan,
                            LoaiBuaAn = "Sáng", // Mặc định bữa sáng (phải khớp với CHECK constraint)
                            NgayAn = DateTime.Today,
                            Calories = (MonAn.Calories ?? 0) * tiLe,
                            Protein = (MonAn.Protein ?? 0) * tiLe,
                            Carbs = (MonAn.Carbs ?? 0) * tiLe,
                            Fat = (MonAn.Fat ?? 0) * tiLe,
                            Fiber = (MonAn.Fiber ?? 0) * tiLe,
                            GhiChu = "LoaiBuaAn: Sáng",
                            NgayCapNhat = DateTime.Now
                        };

                        // Validate trước khi lưu
                        try
                        {
                            // Lưu vào database trước khi mở form chỉnh sửa
                            dbContext.BuaAnChiTiet.Add(buaAnChiTiet);
                            dbContext.SaveChanges();
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã tạo BuaAnChiTiet: {buaAnID}");
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            // Hiển thị chi tiết lỗi validation
                            string errorMsg = "Lỗi validation khi tạo món ăn:\n\n";
                            foreach (var validationError in dbEx.EntityValidationErrors)
                            {
                                foreach (var error in validationError.ValidationErrors)
                                {
                                    errorMsg += $"- {error.PropertyName}: {error.ErrorMessage}\n";
                                }
                            }
                            MessageBox.Show(errorMsg, "Lỗi Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            throw;
                        }
                    }

                    // Mở form chỉnh sửa
                    using (var frm = new frmChinhSuaMonAn(buaAnChiTiet, dbContext))
                    {
                        // Lưu thông tin trước khi mở form để có thể thông báo sau khi xóa
                        string originalMonAnID = buaAnChiTiet?.MonAnID ?? MonAn?.MonAnID;
                        string originalBuaAnID = buaAnChiTiet?.BuaAnID;
                        
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            // Nếu đã xóa, refresh UI và thông báo cho parent
                            if (frm.IsDeleted)
                            {
                                System.Diagnostics.Debug.WriteLine($"[ucMonAnItem] Món ăn đã bị xóa. MonAnID: {originalMonAnID}, BuaAnID: {originalBuaAnID}");
                                
                                // Set null để LoadData() sẽ load lại từ ThuVienMonAn
                                _buaAnChiTiet = null;
                                LoadData(); // Reload để hiển thị giá trị mặc định
                                
                                // Thông báo cho parent control biết món ăn đã bị xóa
                                if (!string.IsNullOrEmpty(originalMonAnID))
                                {
                                    // Tạo một BuaAnChiTiet tạm với MonAnID để parent có thể xác định món nào bị xóa
                                    var deletedItem = new BuaAnChiTiet 
                                    { 
                                        BuaAnID = originalBuaAnID,
                                        MonAnID = originalMonAnID 
                                    };
                                    System.Diagnostics.Debug.WriteLine($"[ucMonAnItem] Đang raise event MonAnDeleted với MonAnID: {originalMonAnID}");
                                    MonAnDeleted?.Invoke(this, deletedItem);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"[ucMonAnItem] WARNING: originalMonAnID rỗng, không thể raise event MonAnDeleted");
                                }
                            }
                            // Nếu đã cập nhật, refresh UI với số lượng mới và thông báo cho parent
                            else if (frm.MonAnDaCapNhat != null)
                            {
                                _buaAnChiTiet = frm.MonAnDaCapNhat;
                                LoadData(); // Reload để hiển thị số lượng và dinh dưỡng mới
                                
                                // Thông báo cho parent control biết món ăn đã được cập nhật
                                MonAnUpdated?.Invoke(this, frm.MonAnDaCapNhat);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa món ăn: {ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetOrCreateKeHoachAnUong(WF_HealthTracker dbContext)
        {
            try
            {
                string userId = CurrentUser.UserID;
                if (string.IsNullOrEmpty(userId))
                {
                    System.Diagnostics.Debug.WriteLine("[DEBUG] CurrentUser.UserID is null or empty");
                    return null;
                }

                // Tìm KeHoachAnUong đang hoạt động
                // Thử nhiều giá trị TrangThai có thể có
                var keHoachAn = dbContext.KeHoachAnUong
                    .Where(k => k.TrangThai == "Đang hoạt động" || 
                           k.TrangThai == "N'Đang hoạt động'" ||
                           k.TrangThai == null ||
                           string.IsNullOrEmpty(k.TrangThai))
                    .FirstOrDefault();

                if (keHoachAn == null)
                {
                    // Tạo mới KeHoachAnUong cho kế hoạch tự do
                    try
                    {
                        keHoachAn = new KeHoachAnUong
                        {
                            KeHoachAnID = $"meal_{DateTime.Now:yyyyMMddHHmmss}",
                            TrangThai = "Đang hoạt động", // Dùng giá trị đơn giản, không có N prefix
                            MoTa = "Kế hoạch ăn uống tự do"
                        };
                        dbContext.KeHoachAnUong.Add(keHoachAn);
                        dbContext.SaveChanges();
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã tạo mới KeHoachAnUong: {keHoachAn.KeHoachAnID}");
                    }
                    catch (Exception createEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Lỗi khi tạo KeHoachAnUong: {createEx.Message}");
                        if (createEx.InnerException != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception: {createEx.InnerException.Message}");
                        }
                        throw;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Tìm thấy KeHoachAnUong: {keHoachAn.KeHoachAnID}, TrangThai: {keHoachAn.TrangThai}");
                }

                return keHoachAn.KeHoachAnID;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Lỗi khi tạo/lấy KeHoachAnUong: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Inner Exception: {ex.InnerException.Message}");
                }
                MessageBox.Show($"Lỗi khi khởi tạo kế hoạch ăn uống:\n\n{ex.Message}", 
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Tạo BuaAnID tự động theo format meal_xxxx (giống frmThemMonAn)
        /// </summary>
        private string GenerateBuaAnID(WF_HealthTracker dbContext)
        {
            try
            {
                var lastMeal = dbContext.BuaAnChiTiet
                    .OrderByDescending(m => m.BuaAnID)
                    .FirstOrDefault();

                if (lastMeal == null || !lastMeal.BuaAnID.StartsWith("meal_"))
                {
                    return "meal_0001";
                }

                string numberPart = lastMeal.BuaAnID.Substring(5);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    int newNumber = lastNumber + 1;
                    return $"meal_{newNumber:D4}";
                }

                int mealCount = dbContext.BuaAnChiTiet.Count();
                return $"meal_{(mealCount + 1):D4}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Lỗi khi tạo BuaAnID: {ex.Message}");
                // Fallback: dùng timestamp nhưng giới hạn 20 ký tự
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                return $"meal_{timestamp}".Substring(0, Math.Min(20, $"meal_{timestamp}".Length));
            }
        }
    }
}

