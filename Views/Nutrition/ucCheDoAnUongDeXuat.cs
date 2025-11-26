using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Controllers;
using HealthApp.Models;
using HealthApp.Common.Helpers;
using Guna.UI2.WinForms;

namespace HealthApp.Views.Nutrition
{
    public partial class ucCheDoAnUongDeXuat : UserControl
    {
        private NutritionController _nutritionController;
        
        // Lưu trữ món ăn đã load để tính toán mà không cần gọi lại database
        private List<ThuVienMonAn> _loadedFoodsSang = new List<ThuVienMonAn>();
        private List<ThuVienMonAn> _loadedFoodsTrua = new List<ThuVienMonAn>();
        private List<ThuVienMonAn> _loadedFoodsToi = new List<ThuVienMonAn>();
        
        // Lưu trữ món ăn đã đề xuất để tránh trùng lặp giữa các bữa
        private List<string> _monAnDaDeXuatTrongNgay = new List<string>();
        
        // Lưu trữ số lượng đề xuất cho từng món ăn (để tính toán chính xác)
        private Dictionary<string, double> _khoiLuongDeXuat = new Dictionary<string, double>();
        
        private Guna2Panel _pnlScrollBuaSang;
        private Guna2Panel _pnlScrollBuaTrua;
        private Guna2Panel _pnlScrollBuaToi;

        public ucCheDoAnUongDeXuat()
        {
            InitializeComponent();
            try
            {
                _nutritionController = new NutritionController();
                InitializeScrollPanels();
                InitializeEventHandlers();
                
                // Load dữ liệu async để không block UI
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không crash ứng dụng
                System.Diagnostics.Debug.WriteLine($"Lỗi khi khởi tạo ucCheDoAnUongDeXuat: {ex.Message}");
                // Hiển thị thông báo lỗi cho user
                MessageBox.Show($"Không thể tải dữ liệu món ăn: {ex.Message}\n\nVui lòng kiểm tra kết nối database.", 
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Load dữ liệu async để không block UI thread
        /// </summary>
        private async void LoadDataAsync()
        {
            try
            {
                // Load mục tiêu trước (nhanh)
                LoadUserGoal();
                
                // Cập nhật ngay chất lượng dinh dưỡng đề xuất cho 1 ngày dựa trên mục tiêu
                UpdateNutritionSummary();
                
                // Load món ăn async (có thể mất thời gian nếu gọi AI)
                await LoadSuggestedFoodsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong LoadDataAsync: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load mục tiêu đầu tiên của user (không hiển thị UI vì control không tồn tại)
        /// </summary>
        private void LoadUserGoal()
        {
            // Control lblHienThiMucTieu không tồn tại, chỉ log để debug
            try
            {
                string userId = CurrentUser.UserID;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    System.Diagnostics.Debug.WriteLine("User chưa đăng nhập");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"=== LoadUserGoal START for user: {userId} ===");

                var goalController = new GoalController();
                try
                {
                    var goals = goalController.GetGoalsByUser(userId, "Đang thực hiện");
                    System.Diagnostics.Debug.WriteLine($"GetGoalsByUser trả về {goals?.Count ?? 0} mục tiêu");
                }
                finally
                {
                    goalController?.Dispose();
                }

                System.Diagnostics.Debug.WriteLine($"=== LoadUserGoal END ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load mục tiêu: {ex.Message}");
            }
        }

        /// <summary>
        /// Khởi tạo các event handlers
        /// </summary>
        private void InitializeEventHandlers()
        {
            // Event khi thay đổi ngày
            if (guna2DateTimePicker1 != null)
            {
                guna2DateTimePicker1.ValueChanged += Guna2DateTimePicker1_ValueChanged;
            }

            // Các button chuyển tuần không tồn tại, đã bỏ
        }

        /// <summary>
        /// Lấy mục tiêu của user (để sử dụng trong tính toán)
        /// </summary>
        private string GetUserGoal()
        {
            try
            {
                string userId = CurrentUser.UserID;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return null;
                }

                var goalController = new GoalController();
                try
                {
                    var goals = goalController.GetGoalsByUser(userId, "Đang thực hiện");
                    if (goals != null && goals.Count > 0)
                    {
                        var firstGoal = goals.FirstOrDefault();
                        if (firstGoal != null)
                        {
                            return $"{firstGoal.LoaiMucTieu}: {firstGoal.TenMucTieu}";
                        }
                    }
                }
                finally
                {
                    goalController.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi lấy mục tiêu: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Tính toán số lượng đề xuất dựa trên mục tiêu và loại món ăn (cải thiện logic cho giảm cân)
        /// </summary>
        private double? CalculateSuggestedQuantity(ThuVienMonAn monAn, string loaiBuaAn, bool isGiamCan, int index, int totalCount)
        {
            if (monAn == null || !monAn.KhoiLuongChuan.HasValue)
                return null;

            double baseQuantity = monAn.KhoiLuongChuan.Value;
            double caloriesPer100g = monAn.Calories ?? 0;
            double suggestedQuantity = baseQuantity;

            // Tính toán số lượng dựa trên mục tiêu calo cho từng bữa ăn (giảm cân)
            if (isGiamCan)
            {
                // Mục tiêu calo cho từng bữa (giảm cân: tổng 1200-1500 kcal/ngày)
                double targetCalories = 0;
                switch (loaiBuaAn)
                {
                    case "Sáng":
                        targetCalories = 400; // Bữa sáng: 400 kcal
                        break;
                    case "Trưa":
                        targetCalories = 500; // Bữa trưa: 500 kcal
                        break;
                    case "Tối":
                        targetCalories = 350; // Bữa tối: 350 kcal (ít hơn để giảm cân)
                        break;
                }

                // Tính số lượng dựa trên mục tiêu calo và calo/100g của món ăn
                if (caloriesPer100g > 0)
                {
                    // Phân bổ calo: món đầu tiên chiếm 40%, các món sau chia đều phần còn lại
                    double allocatedCalories = index == 0 
                        ? targetCalories * 0.4 
                        : (targetCalories * 0.6) / Math.Max(1, totalCount - 1);
                    
                    suggestedQuantity = (allocatedCalories / caloriesPer100g) * 100;
                }
                else
                {
                    // Món không có calo (nước, rau xanh): tăng số lượng
                    suggestedQuantity = baseQuantity * 1.5;
                }

                // Điều chỉnh dựa trên loại món ăn
                if (caloriesPer100g > 200) // Món nhiều calo (thịt mỡ, đồ chiên)
                {
                    suggestedQuantity *= 0.6; // Giảm 40%
                }
                else if (caloriesPer100g > 100) // Món trung bình
                {
                    suggestedQuantity *= 0.85; // Giảm 15%
                }
                else if (caloriesPer100g < 50) // Rau xanh, ít calo
                {
                    suggestedQuantity *= 1.3; // Tăng 30% để no lâu hơn
                }
            }
            else
            {
                // Không phải giảm cân: điều chỉnh theo loại bữa ăn
                switch (loaiBuaAn)
                {
                    case "Sáng":
                        suggestedQuantity *= 1.0;
                        break;
                    case "Trưa":
                        suggestedQuantity *= 1.15;
                        break;
                    case "Tối":
                        suggestedQuantity *= 1.0;
                        break;
                }
            }

            // Đa dạng hóa số lượng giữa các món ăn (±5-15%)
            double variation = 0.05 + (index % 3) * 0.05; // 5%, 10%, 15%
            if (index % 2 == 0)
                suggestedQuantity *= (1 + variation);
            else
                suggestedQuantity *= (1 - variation);

            // Làm tròn đến 5g gần nhất
            suggestedQuantity = Math.Round(suggestedQuantity / 5) * 5;

            // Đảm bảo giới hạn hợp lý
            if (isGiamCan)
            {
                // Giảm cân: giới hạn chặt chẽ hơn
                suggestedQuantity = Math.Max(30, Math.Min(200, suggestedQuantity));
            }
            else
            {
                suggestedQuantity = Math.Max(50, Math.Min(300, suggestedQuantity));
            }

            System.Diagnostics.Debug.WriteLine($"Đề xuất số lượng cho {monAn.TenMonAn} ({loaiBuaAn}): {baseQuantity}g -> {suggestedQuantity}g ({caloriesPer100g} kcal/100g, mục tiêu: {(isGiamCan ? "giảm cân" : "khác")})");
            
            return suggestedQuantity;
        }

        private void Guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Reload món ăn và thống kê khi thay đổi ngày (async để không block UI)
            ReloadSuggestedFoods();
            _ = UpdateWeeklyMonthlyStatsAsync();
        }

        // Các hàm BtnChuyenTuanTruoc_Click và BtnChuyenTuanSau_Click đã bỏ vì control không tồn tại

        /// <summary>
        /// Khởi tạo các panel scrollable cho mỗi bữa ăn
        /// </summary>
        private void InitializeScrollPanels()
        {
            // Các panel pnlBuaSang, pnBuaTrua, pnBuaToi không tồn tại
            // Tạo các panel scroll tạm thời, sẽ được gán vào panel thực tế khi có
            _pnlScrollBuaSang = new Guna2Panel
            {
                AutoScroll = true,
                Padding = new Padding(10, 10, 10, 10),
                BackColor = Color.Transparent,
                BorderRadius = 20
            };

            _pnlScrollBuaTrua = new Guna2Panel
            {
                AutoScroll = true,
                Padding = new Padding(10, 10, 10, 10),
                BackColor = Color.Transparent,
                BorderRadius = 20
            };

            _pnlScrollBuaToi = new Guna2Panel
            {
                AutoScroll = true,
                Padding = new Padding(10, 10, 10, 10),
                BackColor = Color.Transparent,
                BorderRadius = 20
            };
        }

        /// <summary>
        /// Load món ăn đề xuất từ database vào các panel (async)
        /// </summary>
        private async Task LoadSuggestedFoodsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LoadSuggestedFoodsAsync START ===");
                
                // Clear dữ liệu cũ trước khi load mới
                _loadedFoodsSang.Clear();
                _loadedFoodsTrua.Clear();
                _loadedFoodsToi.Clear();
                _monAnDaDeXuatTrongNgay.Clear(); // Clear danh sách món đã đề xuất khi load lại
                _khoiLuongDeXuat.Clear(); // Clear số lượng đề xuất
                
                // Load món ăn tuần tự để tránh trùng lặp (bữa sau biết bữa trước đã đề xuất gì)
                await LoadFoodsToPanelAsync("Sáng", _pnlScrollBuaSang);
                await LoadFoodsToPanelAsync("Trưa", _pnlScrollBuaTrua);
                await LoadFoodsToPanelAsync("Tối", _pnlScrollBuaToi);

                // Đã load tuần tự ở trên, không cần Task.WhenAll nữa

                // Cập nhật UI trên UI thread (chỉ update summary, stats sẽ update async)
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        UpdateNutritionSummary(); // Tính từ dữ liệu đã load, không block
                        _ = UpdateWeeklyMonthlyStatsAsync(); // Fire and forget - không block
                    }));
                }
                else
                {
                    UpdateNutritionSummary(); // Tính từ dữ liệu đã load, không block
                    _ = UpdateWeeklyMonthlyStatsAsync(); // Fire and forget - không block
                }

                System.Diagnostics.Debug.WriteLine("=== LoadSuggestedFoodsAsync END ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong LoadSuggestedFoodsAsync: {ex.Message}");
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"Lỗi khi load món ăn đề xuất: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    MessageBox.Show($"Lỗi khi load món ăn đề xuất: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Load món ăn đề xuất từ database vào các panel (synchronous - để tương thích)
        /// </summary>
        private void LoadSuggestedFoods()
        {
            // Gọi async version
            _ = LoadSuggestedFoodsAsync();
        }

        /// <summary>
        /// Cập nhật CHẤT LƯỢNG DINH DƯỠNG ĐỀ XUẤT CHO 1 NGÀY dựa trên mục tiêu của user
        /// (Thay vì tổng từ các món ăn đã đề xuất)
        /// </summary>
        private void UpdateNutritionSummary()
        {
            try
            {
                // Lấy mục tiêu của user
                var userGoal = GetUserGoalInfo();
                if (userGoal == null)
                {
                    // Nếu không có mục tiêu, hiển thị 0
                    UpdateNutritionSummaryUI(0, 0, 0, 0);
                    System.Diagnostics.Debug.WriteLine("Không có mục tiêu, hiển thị 0 cho chất lượng dinh dưỡng đề xuất");
                    return;
                }

                // Tính toán lượng dinh dưỡng đề xuất cho 1 ngày dựa trên mục tiêu
                var nutritionTarget = CalculateNutritionTargetForGoal(userGoal);
                
                // Cập nhật UI với chất lượng dinh dưỡng đề xuất cho 1 ngày
                UpdateNutritionSummaryUI(
                    nutritionTarget.TargetCalories,
                    nutritionTarget.TargetProtein,
                    nutritionTarget.TargetCarbs,
                    nutritionTarget.TargetFat
                );
                    
                System.Diagnostics.Debug.WriteLine($"Đã cập nhật CHẤT LƯỢNG DINH DƯỠNG ĐỀ XUẤT CHO 1 NGÀY: {nutritionTarget.TargetCalories} kcal, P:{nutritionTarget.TargetProtein}g, C:{nutritionTarget.TargetCarbs}g, F:{nutritionTarget.TargetFat}g");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật chất lượng dinh dưỡng đề xuất: {ex.Message}");
                UpdateNutritionSummaryUI(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Lấy thông tin mục tiêu đầy đủ của user
        /// </summary>
        private Models.MucTieu GetUserGoalInfo()
        {
            try
            {
                string userId = CurrentUser.UserID;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return null;
                }

                var goalController = new GoalController();
                try
                {
                    var goals = goalController.GetGoalsByUser(userId, "Đang thực hiện");
                    if (goals != null && goals.Count > 0)
                    {
                        return goals.FirstOrDefault();
                    }
                }
                finally
                {
                    goalController.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi lấy thông tin mục tiêu: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Tính toán CHẤT LƯỢNG DINH DƯỠNG ĐỀ XUẤT CHO 1 NGÀY dựa trên mục tiêu của user
        /// </summary>
        private NutritionTarget CalculateNutritionTargetForGoal(Models.MucTieu goal)
        {
            try
            {
                // Lấy thông tin user để tính BMR/TDEE
                double canNang = 70; // Mặc định 70kg
                double chieuCao = 170; // Mặc định 170cm
                int tuoi = 30; // Mặc định 30 tuổi
                string gioiTinh = "Nam"; // Mặc định Nam
                string mucDoHoatDong = "Vừa phải"; // Mặc định

                // Lấy thông tin user từ CurrentUser
                if (CurrentUser.User != null)
                {
                    // Lấy giới tính
                    if (!string.IsNullOrWhiteSpace(CurrentUser.User.GioiTinh))
                    {
                        gioiTinh = CurrentUser.User.GioiTinh;
                    }

                    // Tính tuổi từ ngày sinh
                    if (CurrentUser.User.NgaySinh.HasValue)
                    {
                        tuoi = DateTime.Now.Year - CurrentUser.User.NgaySinh.Value.Year;
                        if (DateTime.Now.DayOfYear < CurrentUser.User.NgaySinh.Value.DayOfYear)
                            tuoi--;
                    }
                }

                // Lấy cân nặng và chiều cao từ TinhTrangTongQuan (bản ghi mới nhất)
                try
                {
                    using (var db = new WF_HealthTracker())
                    {
                        string userId = CurrentUser.UserID;
                        if (!string.IsNullOrWhiteSpace(userId))
                        {
                            var tinhTrang = db.TinhTrangTongQuan
                                .Where(t => t.UserID == userId)
                                .OrderByDescending(t => t.NgayGhiNhan)
                                .FirstOrDefault();

                            if (tinhTrang != null)
                            {
                                if (tinhTrang.CanNang.HasValue && tinhTrang.CanNang.Value > 0)
                                    canNang = tinhTrang.CanNang.Value;
                                if (tinhTrang.ChieuCao.HasValue && tinhTrang.ChieuCao.Value > 0)
                                    chieuCao = tinhTrang.ChieuCao.Value;
                                
                                // Lấy mức độ hoạt động từ TrinhDoCaNhan nếu có
                                if (!string.IsNullOrWhiteSpace(tinhTrang.TrinhDoCaNhan))
                                {
                                    mucDoHoatDong = tinhTrang.TrinhDoCaNhan;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi lấy thông tin cân nặng/chiều cao: {ex.Message}");
                    // Dùng giá trị mặc định
                }

                // Sử dụng DashboardController để tính BMR/TDEE chính xác hơn
                var dashboardController = new DashboardController();
                var bmrResult = dashboardController.CalculateBMR(canNang, chieuCao, tuoi, gioiTinh);
                var tdeeResult = dashboardController.CalculateTDEE(canNang, chieuCao, tuoi, gioiTinh, mucDoHoatDong);
                
                // Lấy giá trị BMR và TDEE từ kết quả
                double bmr = bmrResult.Success ? bmrResult.BMR : ((10 * canNang) + (6.25 * chieuCao) - (5 * tuoi) + (gioiTinh.ToLower().Contains("nam") ? 5 : -161));
                double tdee = tdeeResult.Success ? tdeeResult.TDEE : (bmr * GetActivityFactor(mucDoHoatDong));

                // Tính toán lượng dinh dưỡng dựa trên loại mục tiêu
                string loaiMucTieu = goal.LoaiMucTieu ?? "";
                string tenMucTieu = goal.TenMucTieu ?? "";
                double giaTriMucTieu = goal.GiaTriMucTieu ?? 0;

                double targetCalories = tdee;
                double targetProtein = canNang * 1.5; // Mặc định 1.5g/kg
                double targetCarbs = 0;
                double targetFat = 0;

                // Điều chỉnh dựa trên loại mục tiêu
                if (loaiMucTieu.Contains("Giảm cân") || tenMucTieu.Contains("giảm cân") || tenMucTieu.Contains("Giảm cân"))
                {
                    // Giảm cân: Calo thâm hụt 500-750 kcal/ngày
                    targetCalories = tdee - 500; // Thâm hụt 500 kcal
                    targetProtein = canNang * 2.0; // Protein cao: 2g/kg để giữ cơ
                    targetCarbs = targetCalories * 0.35 / 4; // 35% từ carbs (1g carbs = 4 kcal)
                    targetFat = targetCalories * 0.25 / 9; // 25% từ fat (1g fat = 9 kcal)
                }
                else if (loaiMucTieu.Contains("Tăng cân") || tenMucTieu.Contains("tăng cân") || tenMucTieu.Contains("Tăng cân"))
                {
                    // Tăng cân: Calo dư thừa 300-500 kcal/ngày
                    targetCalories = tdee + 400; // Dư thừa 400 kcal
                    targetProtein = canNang * 1.8; // Protein cao: 1.8g/kg
                    targetCarbs = targetCalories * 0.45 / 4; // 45% từ carbs
                    targetFat = targetCalories * 0.25 / 9; // 25% từ fat
                }
                else if (loaiMucTieu.Contains("Tăng cơ") || tenMucTieu.Contains("tăng cơ") || tenMucTieu.Contains("Tăng cơ"))
                {
                    // Tăng cơ: Calo dư thừa vừa phải, Protein rất cao
                    targetCalories = tdee + 300; // Dư thừa 300 kcal
                    targetProtein = canNang * 2.2; // Protein rất cao: 2.2g/kg
                    targetCarbs = targetCalories * 0.40 / 4; // 40% từ carbs
                    targetFat = targetCalories * 0.25 / 9; // 25% từ fat
                }
                else
                {
                    // Duy trì: Calo = TDEE
                    targetCalories = tdee;
                    targetProtein = canNang * 1.5; // 1.5g/kg
                    targetCarbs = targetCalories * 0.40 / 4; // 40% từ carbs
                    targetFat = targetCalories * 0.25 / 9; // 25% từ fat
                }

                // Đảm bảo tổng calo từ macros = targetCalories (làm tròn)
                double totalCalFromMacros = (targetProtein * 4) + (targetCarbs * 4) + (targetFat * 9);
                if (Math.Abs(totalCalFromMacros - targetCalories) > 50)
                {
                    // Điều chỉnh để tổng calo từ macros gần với targetCalories
                    double ratio = targetCalories / totalCalFromMacros;
                    targetCarbs *= ratio;
                    targetFat *= ratio;
                }

                return new NutritionTarget
                {
                    TargetCalories = Math.Max(1200, targetCalories), // Tối thiểu 1200 kcal
                    TargetProtein = Math.Max(50, targetProtein),
                    TargetCarbs = Math.Max(50, targetCarbs),
                    TargetFat = Math.Max(30, targetFat)
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tính toán lượng dinh dưỡng dự tính: {ex.Message}");
                // Trả về giá trị mặc định
                return new NutritionTarget
                {
                    TargetCalories = 2000,
                    TargetProtein = 100,
                    TargetCarbs = 200,
                    TargetFat = 65
                };
            }
        }

        /// <summary>
        /// Lấy Activity Factor dựa trên mức độ hoạt động
        /// </summary>
        private double GetActivityFactor(string mucDoHoatDong)
        {
            if (string.IsNullOrWhiteSpace(mucDoHoatDong))
                return 1.375; // Vừa phải

            string hoatDong = mucDoHoatDong.ToLower();
            if (hoatDong.Contains("ít") || hoatDong.Contains("sedentary"))
                return 1.2; // Ít vận động
            else if (hoatDong.Contains("nhẹ") || hoatDong.Contains("light"))
                return 1.375; // Vận động nhẹ
            else if (hoatDong.Contains("vừa") || hoatDong.Contains("moderate") || hoatDong.Contains("trung bình"))
                return 1.55; // Vận động vừa phải
            else if (hoatDong.Contains("nhiều") || hoatDong.Contains("active") || hoatDong.Contains("tích cực"))
                return 1.725; // Vận động nhiều
            else if (hoatDong.Contains("rất") || hoatDong.Contains("very") || hoatDong.Contains("extreme"))
                return 1.9; // Vận động rất nhiều
            else
                return 1.375; // Mặc định: vừa phải
        }

        /// <summary>
        /// Cập nhật UI cho 4 ô thống kê dinh dưỡng (các control không tồn tại, chỉ log)
        /// </summary>
        private void UpdateNutritionSummaryUI(double calories, double protein, double carbs, double fat)
        {
            try
            {
                // Các label không tồn tại, chỉ log để debug
                System.Diagnostics.Debug.WriteLine($"Nutrition Summary - Calories: {calories:F0}, Protein: {protein:F1}g, Carbs: {carbs:F1}g, Fat: {fat:F1}g");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật UI thống kê dinh dưỡng: {ex.Message}");
            }
        }

        /// <summary>
        /// Class để lưu trữ lượng dinh dưỡng dự tính
        /// </summary>
        private class NutritionTarget
        {
            public double TargetCalories { get; set; }
            public double TargetProtein { get; set; }
            public double TargetCarbs { get; set; }
            public double TargetFat { get; set; }
        }

        /// <summary>
        /// Cập nhật thống kê tuần và tháng DỰ KIẾN dựa trên món ăn đã đề xuất
        /// </summary>
        private async Task UpdateWeeklyMonthlyStatsAsync()
        {
            try
            {
                DateTime selectedDate = guna2DateTimePicker1.Value.Date;

                // Tính calo dự kiến cho 1 ngày từ món ăn đã đề xuất (có tính số lượng đề xuất)
                double caloDuKienMotNgay = await Task.Run(() =>
                {
                    try
                    {
                        // Tính tổng calo từ các món ăn đã đề xuất (dùng số lượng đề xuất đã lưu)
                        double totalCalories = 0;
                        
                        // Tính từ bữa sáng
                        foreach (var monAn in _loadedFoodsSang)
                        {
                            double caloriesPer100g = monAn.Calories ?? 0;
                            // Lấy số lượng đề xuất nếu có, nếu không dùng khối lượng chuẩn
                            double khoiLuong = _khoiLuongDeXuat.ContainsKey(monAn.TenMonAn) 
                                ? _khoiLuongDeXuat[monAn.TenMonAn] 
                                : (monAn.KhoiLuongChuan ?? 100);
                            double heSo = khoiLuong / 100.0;
                            totalCalories += caloriesPer100g * heSo;
                        }
                        
                        // Tính từ bữa trưa
                        foreach (var monAn in _loadedFoodsTrua)
                        {
                            double caloriesPer100g = monAn.Calories ?? 0;
                            double khoiLuong = _khoiLuongDeXuat.ContainsKey(monAn.TenMonAn) 
                                ? _khoiLuongDeXuat[monAn.TenMonAn] 
                                : (monAn.KhoiLuongChuan ?? 100);
                            double heSo = khoiLuong / 100.0;
                            totalCalories += caloriesPer100g * heSo;
                        }
                        
                        // Tính từ bữa tối
                        foreach (var monAn in _loadedFoodsToi)
                        {
                            double caloriesPer100g = monAn.Calories ?? 0;
                            double khoiLuong = _khoiLuongDeXuat.ContainsKey(monAn.TenMonAn) 
                                ? _khoiLuongDeXuat[monAn.TenMonAn] 
                                : (monAn.KhoiLuongChuan ?? 100);
                            double heSo = khoiLuong / 100.0;
                            totalCalories += caloriesPer100g * heSo;
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Calo dự kiến 1 ngày (từ {_loadedFoodsSang.Count + _loadedFoodsTrua.Count + _loadedFoodsToi.Count} món): {totalCalories} kcal");
                        return totalCalories;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi khi tính calo dự kiến: {ex.Message}");
                        return 0;
                    }
                });

                // Tính tuần hiện tại (từ thứ 2 đến chủ nhật)
                int daysUntilMonday = ((int)selectedDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                DateTime startOfWeek = selectedDate.AddDays(-daysUntilMonday);
                DateTime endOfWeek = startOfWeek.AddDays(7);
                
                // Tính số ngày trong tuần (từ đầu tuần đến ngày được chọn, hoặc đến cuối tuần)
                int soNgayTrongTuan = Math.Min(7, (int)(selectedDate - startOfWeek).TotalDays + 1);
                
                // Tính tổng calo dự kiến cho tuần
                double tongCaloTuan = caloDuKienMotNgay * soNgayTrongTuan;

                // Tính tháng hiện tại
                DateTime startOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1);
                
                // Tính số ngày trong tháng (từ đầu tháng đến ngày được chọn, hoặc đến cuối tháng)
                int soNgayTrongThang = Math.Min((int)(endOfMonth - startOfMonth).TotalDays, (int)(selectedDate - startOfMonth).TotalDays + 1);
                
                // Tính tổng calo dự kiến cho tháng
                double tongCaloThang = caloDuKienMotNgay * soNgayTrongThang;

                // Tính trung bình
                double trungBinhCaloNgay = caloDuKienMotNgay; // Trung bình = calo 1 ngày
                double trungBinhCaloThang = soNgayTrongThang > 0 ? tongCaloThang / soNgayTrongThang : 0;

                System.Diagnostics.Debug.WriteLine($"Thống kê dự kiến - Tuần: {tongCaloTuan} kcal ({soNgayTrongTuan} ngày), Tháng: {tongCaloThang} kcal ({soNgayTrongThang} ngày)");

                // Cập nhật UI trên UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        UpdateWeeklyMonthlyStatsUI(tongCaloTuan, trungBinhCaloNgay, tongCaloThang, trungBinhCaloThang);
                    }));
                }
                else
                {
                    UpdateWeeklyMonthlyStatsUI(tongCaloTuan, trungBinhCaloNgay, tongCaloThang, trungBinhCaloThang);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật thống kê tuần/tháng: {ex.Message}");
                // Hiển thị 0 nếu có lỗi
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        UpdateWeeklyMonthlyStatsUI(0, 0, 0, 0);
                    }));
                }
                else
                {
                    UpdateWeeklyMonthlyStatsUI(0, 0, 0, 0);
                }
            }
        }

        /// <summary>
        /// Cập nhật UI cho thống kê tuần và tháng (các control không tồn tại, chỉ log)
        /// </summary>
        private void UpdateWeeklyMonthlyStatsUI(double tongCaloTuan, double trungBinhCaloNgay, double tongCaloThang, double trungBinhCaloThang)
        {
            try
            {
                // Các label không tồn tại, chỉ log để debug
                System.Diagnostics.Debug.WriteLine($"Weekly/Monthly Stats - Tuần: {tongCaloTuan:F0} Kcal, TB ngày: {trungBinhCaloNgay:F0}, Tháng: {tongCaloThang:F0} Kcal, TB tháng: {trungBinhCaloThang:F0}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật UI thống kê: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thống kê tuần và tháng (synchronous - để tương thích)
        /// </summary>
        private void UpdateWeeklyMonthlyStats()
        {
            _ = UpdateWeeklyMonthlyStatsAsync(); // Fire and forget - không block
        }

        /// <summary>
        /// Load món ăn vào panel cụ thể (async)
        /// </summary>
        private async Task LoadFoodsToPanelAsync(string loaiBuaAn, Guna2Panel panel)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadFoodsToPanelAsync START: {loaiBuaAn} ===");
                
                if (_nutritionController == null)
                {
                    throw new Exception("NutritionController chưa được khởi tạo!");
                }

                // Lấy ngày được chọn để đề xuất khác nhau cho từng ngày
                DateTime selectedDate = guna2DateTimePicker1?.Value.Date ?? DateTime.Today;
                
                // Lấy danh sách món đã đề xuất ở các bữa trước để tránh trùng lặp
                var monAnDaDeXuat = new List<string>(_monAnDaDeXuatTrongNgay);
                System.Diagnostics.Debug.WriteLine($"Đề xuất cho {loaiBuaAn}: Đã có {monAnDaDeXuat.Count} món đã đề xuất ở các bữa trước: {string.Join(", ", monAnDaDeXuat)}");
                
                // Gọi async method trực tiếp (không cần Task.Run vì đã là async)
                // Truyền danh sách món đã đề xuất để tránh trùng lặp
                var suggestedFoods = await _nutritionController.GetSuggestedFoodsAsync(loaiBuaAn, 10, selectedDate, monAnDaDeXuat).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine($"GetSuggestedFoodsAsync trả về {suggestedFoods?.Count ?? 0} món ăn cho {loaiBuaAn} ngày {selectedDate:dd/MM/yyyy} (theo đề xuất AI, đã tránh trùng lặp)");
                
                // Lưu lại món ăn đã đề xuất để bữa sau tránh
                if (suggestedFoods != null)
                {
                    foreach (var monAn in suggestedFoods)
                    {
                        if (!string.IsNullOrWhiteSpace(monAn.TenMonAn) && !_monAnDaDeXuatTrongNgay.Contains(monAn.TenMonAn))
                        {
                            _monAnDaDeXuatTrongNgay.Add(monAn.TenMonAn);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"Đã lưu {suggestedFoods.Count} món vào danh sách tránh trùng lặp. Tổng: {_monAnDaDeXuatTrongNgay.Count} món");
                }

                // Lưu lại món ăn đã load để tính toán sau này
                if (loaiBuaAn == "Sáng")
                    _loadedFoodsSang = suggestedFoods ?? new List<ThuVienMonAn>();
                else if (loaiBuaAn == "Trưa")
                    _loadedFoodsTrua = suggestedFoods ?? new List<ThuVienMonAn>();
                else if (loaiBuaAn == "Tối")
                    _loadedFoodsToi = suggestedFoods ?? new List<ThuVienMonAn>();

                // Update UI trên UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => UpdatePanelUI(loaiBuaAn, panel, suggestedFoods)));
                }
                else
                {
                    UpdatePanelUI(loaiBuaAn, panel, suggestedFoods);
                }

                System.Diagnostics.Debug.WriteLine($"=== LoadFoodsToPanelAsync END: {loaiBuaAn} ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong LoadFoodsToPanelAsync cho {loaiBuaAn}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        ShowErrorInPanel(panel, $"Lỗi khi load món ăn cho {loaiBuaAn}: {ex.Message}");
                    }));
                }
                else
                {
                    ShowErrorInPanel(panel, $"Lỗi khi load món ăn cho {loaiBuaAn}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Update UI cho panel (chạy trên UI thread) - Tối ưu để không block
        /// </summary>
        private void UpdatePanelUI(string loaiBuaAn, Guna2Panel panel, List<ThuVienMonAn> suggestedFoods)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== UpdatePanelUI START: {loaiBuaAn}, {suggestedFoods?.Count ?? 0} món ăn ===");
                
                // Xóa các control cũ (trên UI thread)
                var controlsToRemove = new List<Control>();
                foreach (Control ctrl in panel.Controls)
                {
                    controlsToRemove.Add(ctrl);
                }
                foreach (var ctrl in controlsToRemove)
                {
                    panel.Controls.Remove(ctrl);
                    if (ctrl is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                panel.Controls.Clear();

                if (suggestedFoods == null || suggestedFoods.Count == 0)
                {
                    var lblEmpty = new Label
                    {
                        Text = $"Chưa có món ăn đề xuất cho {loaiBuaAn}",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10F),
                        ForeColor = Color.Gray,
                        Location = new Point(20, 20)
                    };
                    panel.Controls.Add(lblEmpty);
                    System.Diagnostics.Debug.WriteLine($"=== UpdatePanelUI END: Không có món ăn ===");
                    return;
                }

                // Tạo và thêm các item món ăn đơn giản vào panel (thay vì ucMonAnDeXuat)
                int yPosition = 10;

                System.Diagnostics.Debug.WriteLine($"Bắt đầu tạo {suggestedFoods.Count} item món ăn cho {loaiBuaAn}...");

                // Suspend layout để tăng tốc độ
                panel.SuspendLayout();
                
                try
                {
                    // Tính toán số lượng đa dạng dựa trên mục tiêu và loại món ăn
                    var userGoal = GetUserGoal();
                    bool isGiamCan = userGoal != null && (userGoal.Contains("giảm cân") || userGoal.Contains("Giảm cân"));
                    
                    int index = 0;
                    foreach (var monAn in suggestedFoods)
                    {
                        try
                        {
                            // Tính số lượng đa dạng dựa trên mục tiêu và loại món ăn
                            double? khoiLuong = CalculateSuggestedQuantity(monAn, loaiBuaAn, isGiamCan, index, suggestedFoods.Count);
                            
                            // Lưu số lượng đề xuất để tính toán thống kê
                            if (khoiLuong.HasValue)
                            {
                                _khoiLuongDeXuat[monAn.TenMonAn] = khoiLuong.Value;
                            }
                            
                            // Thay vì dùng ucMonAnDeXuat, tạo Label đơn giản
                            var lblMonAn = new Label
                            {
                                Text = $"{monAn.TenMonAn} - {(monAn.Calories ?? 0):F0} kcal/100g - Gợi ý: {(khoiLuong ?? monAn.KhoiLuongChuan ?? 100):F0}g",
                                AutoSize = false,
                                Width = panel.Width - 20,
                                Height = 40,
                                Location = new Point(10, yPosition),
                                Font = new Font("Segoe UI", 9F),
                                ForeColor = Color.FromArgb(40, 40, 40),
                                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                            };

                            panel.Controls.Add(lblMonAn);
                            yPosition += lblMonAn.Height + 10;
                            index++;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Lỗi khi tạo item món ăn cho {monAn?.TenMonAn}: {ex.Message}");
                        }
                    }
                }
                finally
                {
                    // Resume layout sau khi thêm xong tất cả
                    panel.ResumeLayout(true);
                    panel.PerformLayout();
                }

                System.Diagnostics.Debug.WriteLine($"=== UpdatePanelUI END: {loaiBuaAn} - Đã thêm {panel.Controls.Count} controls ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong UpdatePanelUI: {ex.Message}");
                ShowErrorInPanel(panel, $"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Hiển thị lỗi trong panel
        /// </summary>
        private void ShowErrorInPanel(Guna2Panel panel, string errorMessage)
        {
            panel.Controls.Clear();
            var lblError = new Label
            {
                Text = errorMessage,
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Red,
                Location = new Point(20, 20)
            };
            panel.Controls.Add(lblError);
        }

        /// <summary>
        /// Load món ăn vào panel cụ thể (synchronous - để tương thích)
        /// </summary>
        private void LoadFoodsToPanel(string loaiBuaAn, Guna2Panel panel)
        {
            // Gọi async version
            _ = LoadFoodsToPanelAsync(loaiBuaAn, panel);
        }

        /// <summary>
        /// Reload món ăn đề xuất (có thể gọi khi thay đổi ngày hoặc mục tiêu)
        /// </summary>
        public void ReloadSuggestedFoods()
        {
            LoadUserGoal(); // Reload mục tiêu khi thay đổi ngày/tuần
            _ = LoadSuggestedFoodsAsync(); // Load async để không block UI
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose components
                components?.Dispose();
                // Dispose nutrition controller
                _nutritionController?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }
    }
}
