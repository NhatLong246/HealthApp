using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using HealthApp.Controllers;
using HealthApp.Models;
using HealthApp.Common.Helpers;
using HealthApp.Services;
using Guna.UI2.WinForms;

namespace HealthApp.Views.Nutrition
{
    public partial class ucCheDoAnUongDeXuat : UserControl
    {
        private NutritionController _nutritionController;
        private ChatGPTService _chatGPTService;

        // Lưu trữ món ăn đã load để tính toán mà không cần gọi lại database
        private List<ThuVienMonAn> _loadedFoodsSang = new List<ThuVienMonAn>();
        private List<ThuVienMonAn> _loadedFoodsTrua = new List<ThuVienMonAn>();
        private List<ThuVienMonAn> _loadedFoodsToi = new List<ThuVienMonAn>();
        private List<ThuVienMonAn> _loadedFoodsPhu = new List<ThuVienMonAn>();
        
        // Lưu trữ món ăn đã đề xuất để tránh trùng lặp giữa các bữa
        private List<string> _monAnDaDeXuatTrongNgay = new List<string>();
        
        // Lưu trữ số lượng đề xuất cho từng món ăn (để tính toán chính xác)
        private Dictionary<string, double> _khoiLuongDeXuat = new Dictionary<string, double>();
        
        private FlowLayoutPanelNoScrollbar _pnlScrollBuaSang;
        private FlowLayoutPanelNoScrollbar _pnlScrollBuaTrua;
        private FlowLayoutPanelNoScrollbar _pnlScrollBuaToi;
        private FlowLayoutPanelNoScrollbar _pnlScrollBuaPhu;

        private Guna2Panel _loadingOverlay;
        private Guna2WinProgressIndicator _loadingIndicator;
        private Label _loadingLabel;
        private int _loadingCounter;

        private const string LoadingDefaultText = "\u0110ang t\u1ea3i d\u1eef li\u1ec7u...";
        private const string LoadingMealsText = "\u0110ang c\u1eadp nh\u1eadt m\u00f3n \u0103n...";
        private const string LoadingPlanText = "\u0110ang t\u1ea3i ch\u1ebf \u0111\u1ed9 \u0103n...";
        private const string FoodInfoMissingText = "Kh\u00f4ng t\u00ecm th\u1ea5y th\u00f4ng tin m\u00f3n \u0103n.";
        private const string NoFoodsToAddText = "Ch\u01b0a c\u00f3 m\u00f3n \u0103n \u0111\u1ec3 th\u00eam.";
        private const string NoGoalFoodsText = "Ch\u01b0a c\u00f3 m\u00f3n \u0103n ph\u00f9 h\u1ee3p v\u1edbi m\u1ee5c ti\u00eau hi\u1ec7n t\u1ea1i.";
        private const string NoAvailableFoodsText = "Kh\u00f4ng c\u00f3 m\u00f3n n\u00e0o \u0111\u1ec3 th\u00eam.";
        private const string NoSuggestionTextFormat = "Ch\u01b0a c\u00f3 m\u00f3n \u0103n \u0111\u1ec1 xu\u1ea5t cho {0}";
        private const string PanelErrorTextFormat = "L\u1ed7i khi load m\u00f3n \u0103n cho {0}: {1}";
        private const string GenericErrorTextFormat = "L\u1ed7i: {0}";

        public ucCheDoAnUongDeXuat()
        {
            InitializeComponent();
            
            // LƯU Ý: Không set lại Location/Size của các controls ở phần header (lblTieuDe, lblInfor, 
            // guna2DateTimePicker1, lblHienThiMucTieu, pnlTieuDe) vì chúng được load từ resx file.
            // Nếu thay đổi layout trong Designer, cần rebuild project để resx được cập nhật.
            
            try
            {
                _nutritionController = new NutritionController();
                _chatGPTService = new ChatGPTService();
                InitializeScrollPanels();
                InitializeEventHandlers();
                InitializeLoadingOverlay();
                
                // Ẩn panel guna2Panel14 (thanh màu xanh) vì không còn button
                if (guna2Panel14 != null)
                {
                    guna2Panel14.Visible = false;
                    guna2Panel14.Enabled = false;
                }
                
                // Load dữ liệu async để không block UI
                LoadDataAsync();
                
                // Load facts về sức khỏe
                _ = LoadHealthFactsAsync();
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
            ShowLoading(LoadingPlanText);
            try
            {
                // Load mục tiêu trước (nhanh)
                LoadUserGoal();
                
                // Cập nhật ngay chất lượng dinh dưỡng đề xuất cho 1 ngày dựa trên mục tiêu
                UpdateNutritionSummary();
                
                // Load món ăn async (có thể mất thời gian nếu gọi AI)
                await LoadSuggestedFoodsAsync();
                
                // Đảm bảo label đánh giá có giá trị ban đầu
                if (lblDanhGia != null)
                {
                    if (string.IsNullOrWhiteSpace(lblDanhGia.Text))
                    {
                        lblDanhGia.Text = "Đang tính toán đánh giá dinh dưỡng...";
                    }
                    System.Diagnostics.Debug.WriteLine($"Sau LoadDataAsync - lblDanhGia.Text: '{lblDanhGia.Text}'");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblDanhGia is NULL sau LoadDataAsync!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong LoadDataAsync: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideLoading();
            }
        }

        /// <summary>
        /// Load mục tiêu đầu tiên của user và hiển thị vào lblHienThiMucTieu
        /// </summary>
        private void LoadUserGoal()
        {
            try
            {
                string userId = CurrentUser.UserID;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    if (lblHienThiMucTieu != null)
                    {
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() => lblHienThiMucTieu.Text = "Chưa có mục tiêu"));
                        }
                        else
                        {
                            lblHienThiMucTieu.Text = "Chưa có mục tiêu";
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("User chưa đăng nhập");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"=== LoadUserGoal START for user: {userId} ===");

                var goalController = new GoalController();
                try
                {
                var goals = goalController.GetGoalsByUser(userId, "Đang thực hiện");
                System.Diagnostics.Debug.WriteLine($"GetGoalsByUser trả về {goals?.Count ?? 0} mục tiêu");

                    // Cập nhật label với mục tiêu đầu tiên
                    if (lblHienThiMucTieu != null)
                    {
                        string goalText = "Chưa có mục tiêu";
                if (goals != null && goals.Count > 0)
                {
                    var firstGoal = goals.FirstOrDefault();
                    if (firstGoal != null)
                    {
                                // Hiển thị tên mục tiêu hoặc loại mục tiêu
                                if (!string.IsNullOrWhiteSpace(firstGoal.TenMucTieu))
                                {
                                    goalText = firstGoal.TenMucTieu;
                                }
                                else if (!string.IsNullOrWhiteSpace(firstGoal.LoaiMucTieu))
                                {
                                    goalText = firstGoal.LoaiMucTieu;
                                }
                            }
                        }

                        // Cập nhật UI trên UI thread
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() => lblHienThiMucTieu.Text = goalText));
                }
                else
                {
                            lblHienThiMucTieu.Text = goalText;
                        }
                    }
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
                if (lblHienThiMucTieu != null)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => lblHienThiMucTieu.Text = "Không thể tải mục tiêu"));
                    }
                    else
                {
                        lblHienThiMucTieu.Text = "Không thể tải mục tiêu";
                }
            }
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

            btnThemMon1.Click += BtnThemMon1_Click;
            btnThemMon2.Click += BtnThemMon2_Click;
            btnThemMon3.Click += BtnThemMon3_Click;

            // Cấu hình lblDanhGia và pnlChuaDanhGia
            if (lblDanhGia != null)
            {
                lblDanhGia.AutoSize = false;
                
                // Kiểm tra và cấu hình panel
                if (panel3 != null)
            {
                    System.Diagnostics.Debug.WriteLine($"pnlChuaDanhGia initialized - Size: {panel3.Size}, Location: {panel3.Location}, Visible: {panel3.Visible}");
                    
                    // Đảm bảo panel visible
                    panel3.Visible = true;
                    
                    // Đảm bảo panel có BackColor (không trong suốt)
                    if (panel3.BackColor == Color.Transparent)
                    {
                        panel3.BackColor = Color.White;
                    }
                    
                    // Cấu hình label theo kích thước panel
                    int labelWidth = panel3.Width - 20; // Trừ padding
                    if (labelWidth <= 0) labelWidth = panel3.Width;
                    if (labelWidth <= 0) labelWidth = 300; // Fallback
                    
                    int labelHeight = panel3.Height - 30; // Trừ padding
                    if (labelHeight <= 0) labelHeight = panel3.Height;
                    if (labelHeight <= 0) labelHeight = 200; // Fallback
                    
                    lblDanhGia.MaximumSize = new Size(labelWidth, 0); // Cho phép wrap theo width
                    lblDanhGia.Width = labelWidth;
                    lblDanhGia.Height = labelHeight; // Đảm bảo có chiều cao khi khởi tạo
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: pnlChuaDanhGia is NULL!");
                    lblDanhGia.MaximumSize = new Size(lblDanhGia.Width, 0);
                }
                
                // Đảm bảo label có giá trị mặc định và visible
                if (string.IsNullOrWhiteSpace(lblDanhGia.Text))
                {
                    lblDanhGia.Text = "Đang tải đánh giá dinh dưỡng...";
            }

                lblDanhGia.Visible = true;
                if (lblDanhGia.ForeColor == Color.Transparent || lblDanhGia.ForeColor == Color.White)
                {
                    lblDanhGia.ForeColor = Color.Black;
                }
                
                System.Diagnostics.Debug.WriteLine($"lblDanhGia initialized - Size: {lblDanhGia.Size}, Location: {lblDanhGia.Location}, Visible: {lblDanhGia.Visible}, ForeColor: {lblDanhGia.ForeColor}, Text='{lblDanhGia.Text}'");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: lblDanhGia is NULL trong InitializeEventHandlers!");
            }

            // Tạo panel để vẽ biểu đồ bên trong pnlChart
            if (pnlChart != null)
            {
                _chartPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(0)
                };
                
                _chartPanel.Paint += PnlChart_Paint;
                _chartPanel.Resize += (s, e) =>
                {
                    _chartPanel.Invalidate();
                };
                
                pnlChart.Controls.Add(_chartPanel);
                _chartPanel.BringToFront();
            }

            // Tạo panel để vẽ biểu đồ tuần và tháng
            InitializeWeekMonthCharts();
        }

        private void InitializeLoadingOverlay()
        {
            _loadingOverlay = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(180, 255, 255, 255),
                Visible = false,
                Enabled = false
            };

            _loadingIndicator = new Guna2WinProgressIndicator
            {
                Size = new Size(80, 80),
                Location = new Point((this.Width - 80) / 2, (this.Height - 80) / 2),
                ProgressColor = Color.FromArgb(19, 217, 195),
                AutoStart = true
            };

            _loadingLabel = new Label
            {
                Text = LoadingDefaultText,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Semibold", 11F),
                ForeColor = Color.FromArgb(32, 32, 32),
                Width = 300,
                Height = 30,
                Location = new Point((this.Width - 300) / 2, _loadingIndicator.Bottom + 10)
            };

            _loadingOverlay.Controls.Add(_loadingIndicator);
            _loadingOverlay.Controls.Add(_loadingLabel);

            this.Controls.Add(_loadingOverlay);
            _loadingOverlay.BringToFront();

            this.Resize += (s, e) =>
            {
                if (_loadingOverlay == null) return;
                _loadingIndicator.Location = new Point((this.Width - _loadingIndicator.Width) / 2, (this.Height - _loadingIndicator.Height) / 2 - 20);
                _loadingLabel.Location = new Point((this.Width - _loadingLabel.Width) / 2, _loadingIndicator.Bottom + 10);
            };
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
        /// Khởi tạo các panel scrollable cho mỗi bữa ăn (gắn vào các panel mới từ Designer)
        /// </summary>
        private void InitializeScrollPanels()
        {
            // Clear và tạo FlowLayoutPanel mới cho Bữa Sáng
            if (pnlLoadBuaSang != null)
            {
                pnlLoadBuaSang.Controls.Clear();
                _pnlScrollBuaSang = CreateMealFlowLayout();
                pnlLoadBuaSang.Controls.Add(_pnlScrollBuaSang);
                _pnlScrollBuaSang.Dock = DockStyle.Fill;
            }

            // Clear và tạo FlowLayoutPanel mới cho Bữa Trưa
            if (pnlLoadBuaTrua != null)
            {
                pnlLoadBuaTrua.Controls.Clear();
                _pnlScrollBuaTrua = CreateMealFlowLayout();
                pnlLoadBuaTrua.Controls.Add(_pnlScrollBuaTrua);
                _pnlScrollBuaTrua.Dock = DockStyle.Fill;
            }

            // Clear và tạo FlowLayoutPanel mới cho Bữa Tối
            if (pnlLoadBuaToi != null)
            {
                pnlLoadBuaToi.Controls.Clear();
                _pnlScrollBuaToi = CreateMealFlowLayout();
                pnlLoadBuaToi.Controls.Add(_pnlScrollBuaToi);
                _pnlScrollBuaToi.Dock = DockStyle.Fill;
            }

            // Clear và tạo FlowLayoutPanel mới cho Bữa Phụ
            if (pnlLoadBuaPhu != null)
            {
                pnlLoadBuaPhu.Controls.Clear();
                _pnlScrollBuaPhu = CreateMealFlowLayout();
                pnlLoadBuaPhu.Controls.Add(_pnlScrollBuaPhu);
                _pnlScrollBuaPhu.Dock = DockStyle.Fill;
            }
        }

        private FlowLayoutPanelNoScrollbar CreateMealFlowLayout()
        {
            return new FlowLayoutPanelNoScrollbar
            {
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(15, 10, 15, 10),
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill // Fill vào panel cha
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
                _loadedFoodsPhu.Clear();
                _monAnDaDeXuatTrongNgay.Clear(); // Clear danh sách món đã đề xuất khi load lại
                _khoiLuongDeXuat.Clear(); // Clear số lượng đề xuất
                
                // Load món ăn tuần tự để tránh trùng lặp (bữa sau biết bữa trước đã đề xuất gì)
                await LoadFoodsToPanelAsync("Sáng", _pnlScrollBuaSang, 3);
                await LoadFoodsToPanelAsync("Trưa", _pnlScrollBuaTrua, 3);
                await LoadFoodsToPanelAsync("Tối", _pnlScrollBuaToi, 3);
                await LoadFoodsToPanelAsync("Bữa phụ", _pnlScrollBuaPhu, 3);

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
        /// Cập nhật UI cho biểu đồ dinh dưỡng
        /// </summary>
        private void UpdateNutritionSummaryUI(double calories, double protein, double carbs, double fat)
        {
            try
            {
                // Vẽ biểu đồ vào pnlChart
                DrawNutritionChart(calories, protein, carbs, fat);
                
                System.Diagnostics.Debug.WriteLine($"Nutrition Summary - Calories: {calories:F0}, Protein: {protein:F1}g, Carbs: {carbs:F1}g, Fat: {fat:F1}g");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật UI thống kê dinh dưỡng: {ex.Message}");
            }
        }

        // Lưu giá trị dinh dưỡng hiện tại để vẽ lại khi resize
        private double _currentCalories = 0;
        private double _currentProtein = 0;
        private double _currentCarbs = 0;
        private double _currentFat = 0;
        
        // Panel để vẽ biểu đồ (bên trong pnlChart)
        private Panel _chartPanel;
        
        // Panel để vẽ biểu đồ tuần và tháng
        private Panel _weekChartPanel;
        private Panel _monthChartPanel;
        
        // Lưu dữ liệu tuần và tháng để vẽ lại
        private double _tongCaloTuan = 0;
        private double _trungBinhCaloNgay = 0;
        private double _tongCaloThang = 0;
        private double _trungBinhCaloThang = 0;

        /// <summary>
        /// Vẽ biểu đồ dinh dưỡng vào pnlChart
        /// </summary>
        private void DrawNutritionChart(double calories, double protein, double carbs, double fat)
        {
            // Lưu giá trị để vẽ lại khi resize
            _currentCalories = calories;
            _currentProtein = protein;
            _currentCarbs = carbs;
            _currentFat = fat;

            // Invalidate chart panel để trigger Paint event
            if (_chartPanel != null)
            {
                _chartPanel.Invalidate();
            }
        }

        /// <summary>
        /// Vẽ biểu đồ trong Paint event của chart panel
        /// </summary>
        private void PnlChart_Paint(object sender, PaintEventArgs e)
        {
            if (_chartPanel == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            // Đảm bảo panel có kích thước hợp lệ
            if (_chartPanel.Width <= 0 || _chartPanel.Height <= 0) return;

            // Tính toán giá trị tối đa để scale biểu đồ
            // Scale kcal xuống 10 lần để vừa với các giá trị khác (protein, carbs, fat thường < 500g)
            double scaledCalories = _currentCalories / 10.0;
            double maxValue = Math.Max(scaledCalories, Math.Max(_currentProtein, Math.Max(_currentCarbs, _currentFat)));
            if (maxValue <= 0) maxValue = 100; // Tránh chia cho 0

            // Màu sắc cho từng loại
            Color colorProtein = Color.FromArgb(19, 217, 195); // Teal
            Color colorCarbs = Color.FromArgb(255, 193, 7);    // Amber
            Color colorFat = Color.FromArgb(255, 87, 34);      // Deep Orange
            Color colorCalories = Color.FromArgb(255, 152, 0); // Orange

            // Kích thước và vị trí - tối ưu cho panel
            int padding = 30;
            int chartWidth = _chartPanel.Width - (padding * 2);
            int chartHeight = _chartPanel.Height - (padding * 2) - 30; // Trừ thêm 30 cho label phía dưới
            int barWidth = Math.Max(40, (chartWidth / 4) - 15);
            int spacing = 15;
            int startX = padding + 10;

            // Vẽ các thanh bar chart
            DrawBar(g, startX, padding, barWidth, chartHeight, _currentProtein, maxValue, colorProtein, "Protein");
            DrawBar(g, startX + barWidth + spacing, padding, barWidth, chartHeight, _currentCarbs, maxValue, colorCarbs, "Carbs");
            DrawBar(g, startX + (barWidth + spacing) * 2, padding, barWidth, chartHeight, _currentFat, maxValue, colorFat, "Fat");
            DrawBar(g, startX + (barWidth + spacing) * 3, padding, barWidth, chartHeight, scaledCalories, maxValue, colorCalories, "Kcal");
        }

        /// <summary>
        /// Vẽ một thanh bar trong biểu đồ
        /// </summary>
        private void DrawBar(Graphics g, int x, int y, int width, int maxHeight, double value, double maxValue, Color color, string label)
        {
            // Tính chiều cao thanh bar
            int barHeight = maxValue > 0 ? (int)((value / maxValue) * maxHeight) : 0;
            if (barHeight < 0) barHeight = 0;
            int barY = y + maxHeight - barHeight;

            // Vẽ thanh bar với bo góc
            if (barHeight > 0)
            {
                using (var brush = new SolidBrush(color))
                {
                    // Vẽ thanh bar với border radius
                    var rect = new Rectangle(x, barY, width, barHeight);
                    using (var path = GetRoundedRectangle(rect, 5))
                    {
                        g.FillPath(brush, path);
                        using (var pen = new Pen(Color.FromArgb(200, color), 1))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }

            // Vẽ giá trị trên thanh bar (nếu có giá trị)
            if (value > 0 && barHeight > 20)
            {
                string valueText = label == "Kcal" ? (value * 10).ToString("F0") : value.ToString("F1");
                var font = new Font("Segoe UI", 9F, FontStyle.Bold);
                var textSize = g.MeasureString(valueText, font);
                int textX = x + (width - (int)textSize.Width) / 2;
                int textY = barY - 20;
                
                if (textY < y)
                {
                    textY = barY + 5; // Hiển thị bên trong thanh nếu không đủ chỗ
                    g.DrawString(valueText, font, new SolidBrush(Color.White), textX, textY);
                }
                else
                {
                    g.DrawString(valueText, font, new SolidBrush(Color.Black), textX, textY);
                }
            }

            // Vẽ label phía dưới
            var labelFont = new Font("Segoe UI", 8F);
            var labelSize = g.MeasureString(label, labelFont);
            int labelX = x + (width - (int)labelSize.Width) / 2;
            g.DrawString(label, labelFont, 
                new SolidBrush(Color.FromArgb(100, 100, 100)), labelX, y + maxHeight + 5);
        }

        /// <summary>
        /// Tạo path cho hình chữ nhật bo góc
        /// </summary>
        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        /// <summary>
        /// Khởi tạo các panel để vẽ biểu đồ tuần và tháng
        /// </summary>
        private void InitializeWeekMonthCharts()
        {
            if (guna2Panel13 == null) return;

            // Tìm hoặc tạo panel cho biểu đồ tuần
            Control weekContainer = guna2Panel13.Controls.Find("pnlWeekChart", true).FirstOrDefault();
            if (weekContainer == null)
            {
                // Tạo panel mới bên trong guna2Panel13
                weekContainer = new Guna2Panel
                {
                    Name = "pnlWeekChart",
                    BackColor = Color.Transparent,
                    FillColor = Color.White,
                    BorderRadius = 15,
                    BorderThickness = 1,
                    BorderColor = Color.FromArgb(19, 217, 195),
                    Location = new Point(50, 100),
                    Size = new Size(450, 350)
                };
                guna2Panel13.Controls.Add(weekContainer);
            }

            if (weekContainer != null)
            {
                _weekChartPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(0)
                };
                
                _weekChartPanel.Paint += WeekChartPanel_Paint;
                _weekChartPanel.Resize += (s, e) => _weekChartPanel.Invalidate();
                
                weekContainer.Controls.Add(_weekChartPanel);
                _weekChartPanel.BringToFront();
            }

            // Tìm hoặc tạo panel cho biểu đồ tháng
            Control monthContainer = guna2Panel13.Controls.Find("pnlMonthChart", true).FirstOrDefault();
            if (monthContainer == null)
            {
                // Tạo panel mới bên trong guna2Panel13
                monthContainer = new Guna2Panel
                {
                    Name = "pnlMonthChart",
                    BackColor = Color.Transparent,
                    FillColor = Color.White,
                    BorderRadius = 15,
                    BorderThickness = 1,
                    BorderColor = Color.FromArgb(255, 152, 0),
                    Location = new Point(550, 100),
                    Size = new Size(450, 350)
                };
                guna2Panel13.Controls.Add(monthContainer);
            }

            if (monthContainer != null)
            {
                _monthChartPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(0)
                };
                
                _monthChartPanel.Paint += MonthChartPanel_Paint;
                _monthChartPanel.Resize += (s, e) => _monthChartPanel.Invalidate();
                
                monthContainer.Controls.Add(_monthChartPanel);
                _monthChartPanel.BringToFront();
            }
        }

        /// <summary>
        /// Vẽ biểu đồ tuần (Line Chart)
        /// </summary>
        private void DrawWeekChart(double tongCaloTuan, double trungBinhCaloNgay)
        {
            if (_weekChartPanel != null)
            {
                _weekChartPanel.Invalidate();
            }
        }

        /// <summary>
        /// Vẽ biểu đồ tháng (Line Chart)
        /// </summary>
        private void DrawMonthChart(double tongCaloThang, double trungBinhCaloThang)
        {
            if (_monthChartPanel != null)
            {
                _monthChartPanel.Invalidate();
            }
        }

        /// <summary>
        /// Vẽ biểu đồ tuần trong Paint event
        /// </summary>
        private void WeekChartPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_weekChartPanel == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            if (_weekChartPanel.Width <= 0 || _weekChartPanel.Height <= 0) return;

            // Vẽ line chart cho 7 ngày trong tuần
            DrawLineChart(g, _weekChartPanel.Width, _weekChartPanel.Height, 
                _tongCaloTuan, _trungBinhCaloNgay, "Tuần này", Color.FromArgb(19, 217, 195));
        }

        /// <summary>
        /// Vẽ biểu đồ tháng trong Paint event
        /// </summary>
        private void MonthChartPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_monthChartPanel == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            if (_monthChartPanel.Width <= 0 || _monthChartPanel.Height <= 0) return;

            // Vẽ line chart cho tháng (30 ngày)
            DrawLineChart(g, _monthChartPanel.Width, _monthChartPanel.Height, 
                _tongCaloThang, _trungBinhCaloThang, "Tháng này", Color.FromArgb(255, 152, 0));
        }

        /// <summary>
        /// Vẽ line chart chung cho tuần và tháng
        /// </summary>
        private void DrawLineChart(Graphics g, int width, int height, double totalValue, double avgValue, string title, Color lineColor)
        {
            int padding = 40;
            int chartWidth = width - (padding * 2);
            int chartHeight = height - (padding * 2) - 10; // Giảm 20px vì không cần chỗ cho tiêu đề

            // Tính số điểm trên biểu đồ (7 ngày cho tuần, 30 ngày cho tháng)
            int pointCount = title.Contains("Tuần") ? 7 : 30;
            
            // Tính giá trị tối đa để scale (dùng avgValue * 1.5 để có không gian phía trên)
            double maxValue = avgValue > 0 ? avgValue * 1.5 : 2000;
            if (maxValue <= 0) maxValue = 2000;

            // Vẽ grid lines
            using (var gridPen = new Pen(Color.FromArgb(230, 230, 230), 1))
            {
                for (int i = 0; i <= 5; i++)
                {
                    int y = padding + (chartHeight * i / 5);
                    g.DrawLine(gridPen, padding, y, padding + chartWidth, y);
                    
                    // Vẽ giá trị trên grid
                    double gridValue = maxValue - (maxValue * i / 5.0);
                    string gridText = gridValue.ToString("F0");
                    var gridFont = new Font("Segoe UI", 8F);
                    var gridTextSize = g.MeasureString(gridText, gridFont);
                    g.DrawString(gridText, gridFont, new SolidBrush(Color.FromArgb(150, 150, 150)), 
                        padding - (int)gridTextSize.Width - 5, y - (int)gridTextSize.Height / 2);
                }
            }

            // Vẽ trục X và Y
            using (var axisPen = new Pen(Color.FromArgb(150, 150, 150), 2))
            {
                // Trục Y
                g.DrawLine(axisPen, padding, padding, padding, padding + chartHeight);
                // Trục X
                g.DrawLine(axisPen, padding, padding + chartHeight, padding + chartWidth, padding + chartHeight);
            }

            // Tạo dữ liệu với biến thiên nhẹ để biểu đồ có dạng sóng tự nhiên
            PointF[] points = new PointF[pointCount];
            Random rand = new Random(title.GetHashCode()); // Seed cố định để đồng bộ
            
            for (int i = 0; i < pointCount; i++)
            {
                float x = padding + (chartWidth * i / (float)(pointCount - 1));
                
                // Tạo biến thiên ±10% quanh giá trị trung bình
                double variation = (rand.NextDouble() - 0.5) * 0.2; // -10% đến +10%
                double dailyValue = avgValue * (1 + variation);
                
                float y = padding + chartHeight - (float)((dailyValue / maxValue) * chartHeight);
                points[i] = new PointF(x, y);
            }

            // Vẽ đường line với gradient fill phía dưới
            if (points.Length > 1)
            {
                // Vẽ area fill phía dưới đường line
                PointF[] areaPoints = new PointF[points.Length + 2];
                areaPoints[0] = new PointF(points[0].X, padding + chartHeight);
                for (int i = 0; i < points.Length; i++)
                {
                    areaPoints[i + 1] = points[i];
                }
                areaPoints[areaPoints.Length - 1] = new PointF(points[points.Length - 1].X, padding + chartHeight);
                
                using (var fillBrush = new SolidBrush(Color.FromArgb(50, lineColor)))
                {
                    g.FillPolygon(fillBrush, areaPoints);
                }

                // Vẽ đường line
                using (var linePen = new Pen(lineColor, 3))
                {
                    linePen.LineJoin = LineJoin.Round;
                    g.DrawLines(linePen, points);
                }

                // Vẽ các điểm
                using (var brush = new SolidBrush(lineColor))
                {
                    foreach (var point in points)
                    {
                        g.FillEllipse(brush, point.X - 5, point.Y - 5, 10, 10);
                        g.DrawEllipse(new Pen(Color.White, 2), point.X - 5, point.Y - 5, 10, 10);
                    }
                }
            }

            // Vẽ giá trị trung bình và tổng
            string avgText = $"TB: {avgValue:F0} kcal";
            string totalText = title.Contains("Tuần") ? $"Tổng: {totalValue:F0} kcal" : $"Tổng: {totalValue:F0} kcal";
            var textFont = new Font("Segoe UI", 9F);
            g.DrawString(avgText, textFont, new SolidBrush(Color.FromArgb(100, 100, 100)), 
                padding + 10, padding - 20);
            g.DrawString(totalText, textFont, new SolidBrush(Color.FromArgb(100, 100, 100)), 
                padding + chartWidth - 150, padding - 20);
        }

        /// <summary>
        /// Đánh giá mức dinh dưỡng bằng AI dựa trên calo trung bình
        /// </summary>
        private async Task<string> EvaluateNutritionAsync(double trungBinhCaloNgay, double trungBinhCaloThang)
        {
            try
            {
                // Lấy mục tiêu calo của user
                var userGoal = GetUserGoalInfo();
                double targetCalories = 2000; // Mặc định
                string mucTieu = null;
                double? protein = null;
                double? carbs = null;
                double? fat = null;

                if (userGoal != null)
                {
                    try
                    {
                        var nutritionTarget = CalculateNutritionTargetForGoal(userGoal);
                        targetCalories = nutritionTarget.TargetCalories;
                        protein = nutritionTarget.TargetProtein;
                        carbs = nutritionTarget.TargetCarbs;
                        fat = nutritionTarget.TargetFat;
                        
                        if (!string.IsNullOrWhiteSpace(userGoal.TenMucTieu))
                            mucTieu = userGoal.TenMucTieu;
                        else if (!string.IsNullOrWhiteSpace(userGoal.LoaiMucTieu))
                            mucTieu = userGoal.LoaiMucTieu;
                    }
                    catch { }
                }

                // Gọi AI để đánh giá
                if (_chatGPTService != null)
                {
                    string aiEvaluation = await _chatGPTService.EvaluateNutritionAsync(
                        trungBinhCaloNgay,
                        trungBinhCaloThang,
                        targetCalories,
                        mucTieu,
                        protein,
                        carbs,
                        fat
                    );
                    
                    if (!string.IsNullOrWhiteSpace(aiEvaluation))
                    {
                        return aiEvaluation;
                    }
                }

                // Fallback nếu AI không hoạt động
                return GetFallbackEvaluation(trungBinhCaloNgay, targetCalories);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi đánh giá dinh dưỡng bằng AI: {ex.Message}");
                
                // Fallback
                var userGoal = GetUserGoalInfo();
                double targetCalories = 2000;
                if (userGoal != null)
                {
                    try
                    {
                        var nutritionTarget = CalculateNutritionTargetForGoal(userGoal);
                        targetCalories = nutritionTarget.TargetCalories;
                    }
                    catch { }
                }
                
                return GetFallbackEvaluation(trungBinhCaloNgay, targetCalories);
            }
        }

        /// <summary>
        /// Đánh giá fallback khi không có AI
        /// </summary>
        private string GetFallbackEvaluation(double trungBinhCaloNgay, double targetCalories)
        {
            double percent = trungBinhCaloNgay > 0 ? (trungBinhCaloNgay / targetCalories) * 100 : 0;

            if (percent < 70)
            {
                return "Mức dinh dưỡng của bạn đang thấp hơn mục tiêu. Hãy tăng cường bổ sung các bữa ăn đầy đủ chất dinh dưỡng để đạt được mục tiêu sức khỏe.";
            }
            else if (percent >= 70 && percent < 90)
            {
                return "Mức dinh dưỡng của bạn đang ở mức khá tốt nhưng vẫn còn thiếu một chút. Hãy cố gắng duy trì và cải thiện thêm để đạt mục tiêu.";
            }
            else if (percent >= 90 && percent <= 110)
            {
                return "Mức dinh dưỡng của bạn đang rất tốt và phù hợp với mục tiêu. Hãy tiếp tục duy trì chế độ ăn uống lành mạnh này.";
            }
            else if (percent > 110 && percent <= 130)
            {
                return "Mức dinh dưỡng của bạn đang vượt quá mục tiêu một chút. Hãy điều chỉnh lại để phù hợp hơn với kế hoạch của bạn.";
            }
            else
            {
                return "Mức dinh dưỡng của bạn đang vượt quá mục tiêu nhiều. Hãy xem xét lại chế độ ăn uống và điều chỉnh để đạt được mục tiêu sức khỏe tốt hơn.";
            }
        }

        /// <summary>
        /// Cập nhật label đánh giá
        /// </summary>
        private void UpdateAssessmentLabel(string danhGia)
        {
            try
            {
                if (lblDanhGia == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblDanhGia is NULL trong UpdateAssessmentLabel!");
                    return;
                }

                // Khai báo biến labelWidth và labelHeight ở ngoài để có thể sử dụng ở nhiều nơi
                int labelWidth = 300; // Fallback mặc định
                int labelHeight = 200; // Fallback mặc định
                
                // Kiểm tra panel chứa label
                if (panel3 != null)
                {
                    System.Diagnostics.Debug.WriteLine($"pnlChuaDanhGia - Size: {panel3.Size}, Location: {panel3.Location}, Visible: {panel3.Visible}, BackColor: {panel3.BackColor}");
                    
                    // Đảm bảo panel visible và có kích thước hợp lệ
                    if (!panel3.Visible)
                    {
                        panel3.Visible = true;
                        System.Diagnostics.Debug.WriteLine("Đã set pnlChuaDanhGia.Visible = true");
                    }
                    
                    // Tìm panel cha (guna2Panel13) để giới hạn kích thước
                    Control parentPanel = panel3.Parent;
                    int maxParentWidth = 1128; // Kích thước mặc định từ resx
                    int maxParentHeight = 529;
                    
                    if (parentPanel != null)
                    {
                        maxParentWidth = parentPanel.Width;
                        maxParentHeight = parentPanel.Height;
                        System.Diagnostics.Debug.WriteLine($"Parent panel ({parentPanel.Name}) - Size: {parentPanel.Size}");
                    }
                    
                    // Giới hạn pnlChuaDanhGia theo panel cha (trừ border và padding)
                    // Border của guna2Panel13 là 2px mỗi bên, padding khoảng 20px mỗi bên
                    int maxPanelWidth = maxParentWidth - 50; // Trừ border và padding
                    
                    // Tính chiều cao tối đa dựa trên vị trí của panel trong parent
                    int panelTop = panel3.Top;
                    // Trừ border (2px) và padding dưới (20px) = 22px
                    int maxPanelHeight = maxParentHeight - panelTop - 22; // Đảm bảo không che border dưới
                    
                    // Đảm bảo panel có kích thước hợp lý nhưng không vượt quá panel cha
                    if (panel3.Width > maxPanelWidth)
                    {
                        System.Diagnostics.Debug.WriteLine($"pnlChuaDanhGia width ({panel3.Width}) > max ({maxPanelWidth}). Đang giới hạn...");
                        panel3.Width = maxPanelWidth;
                    }
                    
                    if (panel3.Height > maxPanelHeight)
                    {
                        System.Diagnostics.Debug.WriteLine($"pnlChuaDanhGia height ({panel3.Height}) > max ({maxPanelHeight}). Đang giới hạn để không che border (panel top: {panelTop}, parent height: {maxParentHeight})");
                        panel3.Height = maxPanelHeight;
                    }
                    
                    // Kiểm tra lại: đảm bảo panel bottom không vượt quá parent bottom
                    int panelBottom = panelTop + panel3.Height;
                    if (panelBottom > maxParentHeight - 12) // Trừ border và padding
                    {
                        int newHeight = maxParentHeight - 12 - panelTop;
                        if (newHeight > 0)
                        {
                            panel3.Height = newHeight;
                            System.Diagnostics.Debug.WriteLine($"Điều chỉnh pnlChuaDanhGia height: {newHeight}px để không che border");
                        }
                    }
                    
                    // Nếu panel quá nhỏ, set lại nhưng không vượt quá panel cha
                    if (panel3.Width < 300 || panel3.Height < 300)
                    {
                        int newWidth = Math.Min(334, maxPanelWidth);
                        int newHeight = Math.Min(600, maxPanelHeight);
                        System.Diagnostics.Debug.WriteLine($"pnlChuaDanhGia có kích thước nhỏ: {panel3.Size}. Đang set lại: {newWidth}x{newHeight}");
                        panel3.Size = new Size(newWidth, newHeight);
                    }
                    
                    // Đảm bảo label fill toàn bộ panel (trừ padding nhỏ) và không vượt quá panel cha
                    labelWidth = panel3.Width - 10; // Padding nhỏ (5px mỗi bên)
                    if (labelWidth <= 0) labelWidth = panel3.Width;
                    if (labelWidth <= 0) labelWidth = 300; // Fallback
                    
                    // Đảm bảo label không vượt quá panel cha
                    int maxLabelWidth = maxPanelWidth - 20; // Trừ thêm padding của pnlChuaDanhGia
                    labelWidth = Math.Min(labelWidth, maxLabelWidth);
                    
                    // Label height sẽ được tính sau dựa trên text, nhưng không vượt quá panel cha
                    labelHeight = panel3.Height - 10; // Padding nhỏ (5px trên dưới)
                    if (labelHeight <= 0) labelHeight = panel3.Height;
                    if (labelHeight <= 0) labelHeight = 500; // Fallback
                    
                    // Giới hạn label height theo panel cha
                    int maxLabelHeight = maxPanelHeight - 20;
                    labelHeight = Math.Min(labelHeight, maxLabelHeight);
                    
                    lblDanhGia.MaximumSize = new Size(labelWidth, 0); // Cho phép wrap theo width, KHÔNG giới hạn height (nhưng sẽ giới hạn sau)
                    lblDanhGia.Width = labelWidth;
                    // KHÔNG set height ở đây, sẽ tính sau dựa trên text
                    // Đảm bảo label có vị trí đúng (padding 5px)
                    lblDanhGia.Location = new Point(5, 5);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: pnlChuaDanhGia is NULL!");
                    // Fallback: dùng width hiện tại của label
                    if (lblDanhGia.Width > 0)
                    {
                        labelWidth = lblDanhGia.Width;
                    }
                    lblDanhGia.MaximumSize = new Size(labelWidth, 0);
                }

                // Đảm bảo label có thể xuống dòng tự động dựa trên kích thước
                lblDanhGia.AutoSize = false;
                
                // Đảm bảo label visible và có màu chữ
                lblDanhGia.Visible = true;
                if (lblDanhGia.ForeColor == Color.Transparent || lblDanhGia.ForeColor == Color.White)
                {
                    lblDanhGia.ForeColor = Color.Black;
                }
                
                // Set text trước để có thể đo chính xác
                if (string.IsNullOrWhiteSpace(danhGia))
                {
                    lblDanhGia.Text = "Chưa có đánh giá dinh dưỡng.";
                    System.Diagnostics.Debug.WriteLine("WARNING: Đánh giá rỗng, hiển thị mặc định");
                }
                else
                {
                    lblDanhGia.Text = danhGia;
                    System.Diagnostics.Debug.WriteLine($"Đã cập nhật lblDanhGia: {danhGia.Substring(0, Math.Min(50, danhGia.Length))}...");
                }
                
                // Tính lại chiều cao dựa trên text thực tế (SAU KHI SET TEXT)
                string textToMeasure = lblDanhGia.Text ?? "Đang tải...";
                int calculatedHeight = labelHeight; // Mặc định
                
                try
                {
                    using (Graphics g = lblDanhGia.CreateGraphics())
                    {
                        SizeF textSize = g.MeasureString(
                            textToMeasure, 
                            lblDanhGia.Font, 
                            labelWidth
                        );
                        calculatedHeight = (int)Math.Ceiling(textSize.Height) + 20; // Thêm padding
                        
                        // Giới hạn chiều cao theo panel cha để KHÔNG CHE BORDER
                        if (panel3 != null)
                        {
                            // Tìm panel cha để lấy kích thước tối đa
                            Control parentPanel = panel3.Parent;
                            int maxParentHeight = 529; // Mặc định từ resx
                            
                            if (parentPanel != null)
                            {
                                maxParentHeight = parentPanel.Height;
                                System.Diagnostics.Debug.WriteLine($"Parent panel height: {maxParentHeight}");
                            }
                            
                            // Tính toán vị trí của pnlChuaDanhGia trong panel cha
                            int panelTop = panel3.Top;
                            int panelBottom = panelTop + panel3.Height;
                            
                            // Chiều cao tối đa = khoảng cách từ top của panel đến bottom của panel cha - border - padding
                            // Trừ border (2px mỗi bên = 4px) và padding (20px mỗi bên = 40px)
                            int availableSpaceFromPanelTop = maxParentHeight - panelTop - 30; // Trừ border và padding dưới
                            
                            // Cho phép label sử dụng toàn bộ panel height (trừ padding nhỏ)
                            int panelAvailableHeight = panel3.Height - 10; // Trừ padding
                            
                            // Giới hạn theo cả panel hiện tại và không gian còn lại trong panel cha
                            int finalMaxHeight = Math.Min(panelAvailableHeight, availableSpaceFromPanelTop);
                            
                            // Đảm bảo không vượt quá để không che border
                            if (calculatedHeight > finalMaxHeight)
                            {
                                calculatedHeight = finalMaxHeight;
                                System.Diagnostics.Debug.WriteLine($"Text cần {calculatedHeight}px nhưng giới hạn để không che border: {finalMaxHeight}px (panel top: {panelTop}, parent height: {maxParentHeight})");
                            }
                            
                            // Kiểm tra lại: đảm bảo label + location không vượt quá panel cha
                            int labelBottom = panel3.Top + lblDanhGia.Top + calculatedHeight;
                            if (labelBottom > maxParentHeight - 10) // Trừ border và padding
                            {
                                calculatedHeight = maxParentHeight - 10 - panel3.Top - lblDanhGia.Top;
                                System.Diagnostics.Debug.WriteLine($"Điều chỉnh lại height để không che border: {calculatedHeight}px");
                            }
                        }
                        else
                        {
                            // Không có panel, giới hạn 500px
                            calculatedHeight = Math.Min(calculatedHeight, 500);
                        }
                    }
                }
                catch
                {
                    // Nếu không tính được, dùng labelHeight
                    calculatedHeight = labelHeight;
                }
                
                // Đảm bảo chiều cao tối thiểu
                if (calculatedHeight <= 0)
                {
                    calculatedHeight = Math.Max(50, labelHeight); // Tối thiểu 50px
                }
                
                // Set chiều cao cho label - ĐẢM BẢO KHÔNG CHE BORDER
                if (panel3 != null)
                {
                    // Kiểm tra lại lần cuối: đảm bảo label không che border dưới của panel cha
                    Control parentPanel = panel3.Parent;
                    if (parentPanel != null)
                    {
                        // Tính vị trí bottom của label trong panel cha
                        int labelTopInParent = panel3.Top + lblDanhGia.Top;
                        int labelBottomInParent = labelTopInParent + calculatedHeight;
                        int parentBottom = parentPanel.Height;
                        
                        // Trừ border (2px) và padding (10px) = 12px
                        int safeBottom = parentBottom - 12;
                        
                        // Nếu label vượt quá, giới hạn lại
                        if (labelBottomInParent > safeBottom)
                        {
                            calculatedHeight = safeBottom - labelTopInParent;
                            if (calculatedHeight < 50) calculatedHeight = 50; // Tối thiểu 50px
                            System.Diagnostics.Debug.WriteLine($"Điều chỉnh height để không che border: {calculatedHeight}px (labelBottom: {labelBottomInParent}, safeBottom: {safeBottom})");
                        }
                    }
                }
                
                lblDanhGia.Height = calculatedHeight;
                
                // Log thông tin
                if (panel3 != null)
                {
                    Control parentPanel = panel3.Parent;
                    if (parentPanel != null)
                    {
                        int labelBottom = panel3.Top + lblDanhGia.Top + calculatedHeight;
                        System.Diagnostics.Debug.WriteLine($"Đã set lblDanhGia.Height = {calculatedHeight}px. Label bottom trong parent: {labelBottom}px, Parent height: {parentPanel.Height}px");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Đã set lblDanhGia.Height = {calculatedHeight}px (panel height = {panel3.Height}, text length = {textToMeasure.Length})");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Đã set lblDanhGia.Height = {calculatedHeight}px (text length = {textToMeasure.Length})");
                }
                
                // Log thông tin label sau khi cập nhật
                System.Diagnostics.Debug.WriteLine($"lblDanhGia - Size: {lblDanhGia.Size}, Location: {lblDanhGia.Location}, Visible: {lblDanhGia.Visible}, ForeColor: {lblDanhGia.ForeColor}, Text length: {lblDanhGia.Text?.Length ?? 0}");
                
                // Force refresh
                lblDanhGia.Invalidate();
                lblDanhGia.Refresh();
                if (panel3 != null)
                {
                    panel3.Invalidate();
                    panel3.Refresh();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong UpdateAssessmentLabel: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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
        /// Cập nhật UI cho thống kê tuần và tháng - vẽ biểu đồ và đánh giá
        /// </summary>
        private void UpdateWeeklyMonthlyStatsUI(double tongCaloTuan, double trungBinhCaloNgay, double tongCaloThang, double trungBinhCaloThang)
        {
            try
            {
                // Lưu giá trị để vẽ lại khi resize
                _tongCaloTuan = tongCaloTuan;
                _trungBinhCaloNgay = trungBinhCaloNgay;
                _tongCaloThang = tongCaloThang;
                _trungBinhCaloThang = trungBinhCaloThang;

                // Vẽ biểu đồ tuần và tháng
                DrawWeekChart(tongCaloTuan, trungBinhCaloNgay);
                DrawMonthChart(tongCaloThang, trungBinhCaloThang);
                
                System.Diagnostics.Debug.WriteLine($"Weekly/Monthly Stats - Tuần: {tongCaloTuan:F0} Kcal, TB ngày: {trungBinhCaloNgay:F0}, Tháng: {tongCaloThang:F0} Kcal, TB tháng: {trungBinhCaloThang:F0}");
                System.Diagnostics.Debug.WriteLine($"lblDanhGia is null: {lblDanhGia == null}");
                
                // Đảm bảo có giá trị để đánh giá
                if (trungBinhCaloNgay > 0 || trungBinhCaloThang > 0)
                {
                    // Đánh giá dinh dưỡng bằng AI (async)
                    System.Diagnostics.Debug.WriteLine("Gọi EvaluateAndUpdateAssessmentAsync...");
                    _ = EvaluateAndUpdateAssessmentAsync(trungBinhCaloNgay, trungBinhCaloThang);
                }
                else
                {
                    // Nếu không có dữ liệu, hiển thị thông báo
                    System.Diagnostics.Debug.WriteLine("Không có dữ liệu calo, hiển thị thông báo mặc định");
                    if (lblDanhGia != null)
                    {
                        UpdateAssessmentLabel("Chưa có dữ liệu dinh dưỡng để đánh giá. Hãy thêm món ăn vào kế hoạch.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: lblDanhGia is NULL!");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật UI thống kê: {ex.Message}");
            }
        }

        /// <summary>
        /// Đánh giá dinh dưỡng bằng AI và cập nhật label
        /// </summary>
        private async Task EvaluateAndUpdateAssessmentAsync(double trungBinhCaloNgay, double trungBinhCaloThang)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== EvaluateAndUpdateAssessmentAsync START: {trungBinhCaloNgay:F0} kcal/ngày, {trungBinhCaloThang:F0} kcal/tháng ===");
                
                // Kiểm tra label có tồn tại không
                if (lblDanhGia == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblDanhGia is NULL!");
                    return;
                }

                // Hiển thị "Đang phân tích..." trong khi chờ AI
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => 
                    {
                        if (lblDanhGia != null)
                            lblDanhGia.Text = "Đang phân tích dinh dưỡng bằng AI...";
                    }));
                }
                else
                {
                    if (lblDanhGia != null)
                        lblDanhGia.Text = "Đang phân tích dinh dưỡng bằng AI...";
                }

                System.Diagnostics.Debug.WriteLine("Đang gọi AI để đánh giá...");
                
                // Gọi AI để đánh giá
                string danhGia = await EvaluateNutritionAsync(trungBinhCaloNgay, trungBinhCaloThang);
                
                System.Diagnostics.Debug.WriteLine($"AI trả về đánh giá: {danhGia?.Substring(0, Math.Min(100, danhGia?.Length ?? 0))}...");
                
                if (string.IsNullOrWhiteSpace(danhGia))
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: AI trả về đánh giá rỗng, dùng fallback");
                    // Fallback
                    var userGoal = GetUserGoalInfo();
                    double targetCalories = 2000;
                    if (userGoal != null)
                    {
                        try
                        {
                            var nutritionTarget = CalculateNutritionTargetForGoal(userGoal);
                            targetCalories = nutritionTarget.TargetCalories;
                        }
                        catch { }
                    }
                    danhGia = GetFallbackEvaluation(trungBinhCaloNgay, targetCalories);
                }
                
                // Cập nhật UI trên UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => 
                    {
                        if (lblDanhGia != null)
                            UpdateAssessmentLabel(danhGia);
                        else
                            System.Diagnostics.Debug.WriteLine("ERROR: lblDanhGia is NULL khi cập nhật!");
                    }));
                }
                else
                {
                    if (lblDanhGia != null)
                        UpdateAssessmentLabel(danhGia);
                    else
                        System.Diagnostics.Debug.WriteLine("ERROR: lblDanhGia is NULL khi cập nhật!");
                }
                
                System.Diagnostics.Debug.WriteLine($"=== EvaluateAndUpdateAssessmentAsync END ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi đánh giá dinh dưỡng: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Fallback
                try
                {
                    var userGoal = GetUserGoalInfo();
                    double targetCalories = 2000;
                    if (userGoal != null)
                    {
                        try
                        {
                            var nutritionTarget = CalculateNutritionTargetForGoal(userGoal);
                            targetCalories = nutritionTarget.TargetCalories;
                        }
                        catch { }
                    }
                    
                    string fallback = GetFallbackEvaluation(trungBinhCaloNgay, targetCalories);
                    
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => 
                        {
                            if (lblDanhGia != null)
                                UpdateAssessmentLabel(fallback);
                        }));
                    }
                    else
                    {
                        if (lblDanhGia != null)
                            UpdateAssessmentLabel(fallback);
                    }
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật fallback: {fallbackEx.Message}");
                }
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
        private async Task LoadFoodsToPanelAsync(string loaiBuaAn, FlowLayoutPanelNoScrollbar panel, int maxItems)
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
                var suggestedFoods = await _nutritionController.GetSuggestedFoodsAsync(loaiBuaAn, maxItems, selectedDate, monAnDaDeXuat).ConfigureAwait(false);
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
                if (loaiBuaAn.Equals("Sáng", StringComparison.OrdinalIgnoreCase))
                    _loadedFoodsSang = suggestedFoods ?? new List<ThuVienMonAn>();
                else if (loaiBuaAn.Equals("Trưa", StringComparison.OrdinalIgnoreCase))
                    _loadedFoodsTrua = suggestedFoods ?? new List<ThuVienMonAn>();
                else if (loaiBuaAn.Equals("Tối", StringComparison.OrdinalIgnoreCase))
                    _loadedFoodsToi = suggestedFoods ?? new List<ThuVienMonAn>();
                else if (loaiBuaAn.Equals("Bữa phụ", StringComparison.OrdinalIgnoreCase))
                    _loadedFoodsPhu = suggestedFoods ?? new List<ThuVienMonAn>();

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
                    ShowErrorInPanel(panel, string.Format(PanelErrorTextFormat, loaiBuaAn, ex.Message));
                    }));
                }
                else
                {
                ShowErrorInPanel(panel, string.Format(PanelErrorTextFormat, loaiBuaAn, ex.Message));
                }
            }
        }

        /// <summary>
        /// Update UI cho panel (chạy trên UI thread) - Tối ưu để không block
        /// </summary>
        private void UpdatePanelUI(string loaiBuaAn, FlowLayoutPanelNoScrollbar panel, List<ThuVienMonAn> suggestedFoods)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== UpdatePanelUI START: {loaiBuaAn}, {suggestedFoods?.Count ?? 0} món ăn ===");
                
                // Xóa các control cũ (trên UI thread) - cải thiện để tránh chồng chéo
                panel.SuspendLayout();
                try
                {
                    var controlsToRemove = panel.Controls.Cast<Control>().ToList();
                foreach (var ctrl in controlsToRemove)
                {
                    panel.Controls.Remove(ctrl);
                    if (ctrl is IDisposable disposable)
                        {
                            try
                    {
                        disposable.Dispose();
                            }
                            catch { } // Bỏ qua lỗi dispose
                    }
                }
                panel.Controls.Clear();
                }
                finally
                {
                    panel.ResumeLayout(false);
                }

                if (suggestedFoods == null || suggestedFoods.Count == 0)
                {
                    var lblEmpty = new Label
                    {
                    Text = string.Format(NoSuggestionTextFormat, loaiBuaAn),
                        AutoSize = false,
                        Width = panel.Width - 10,
                        Font = new Font("Segoe UI", 10F),
                        ForeColor = Color.Gray,
                        Height = 40,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    panel.Controls.Add(lblEmpty);
                    System.Diagnostics.Debug.WriteLine($"=== UpdatePanelUI END: Không có món ăn ===");
                    return;
                }

                // Tạo và thêm các item món ăn đơn giản vào panel (thay vì ucMonAnDeXuat)
                System.Diagnostics.Debug.WriteLine($"Bắt đầu tạo {suggestedFoods.Count} item món ăn cho {loaiBuaAn}...");

                // Suspend layout để tăng tốc độ
                panel.SuspendLayout();
                
                try
                {
                    // Tính toán số lượng đa dạng dựa trên mục tiêu và loại món ăn
                    var userGoal = GetUserGoal();
                    bool isGiamCan = userGoal != null && (userGoal.Contains("giảm cân") || userGoal.Contains("Giảm cân"));
                    
                    // Lọc trùng lặp và chỉ lấy tối đa 3 món
                    var uniqueFoods = new List<ThuVienMonAn>();
                    var addedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    
                    foreach (var monAn in suggestedFoods)
                    {
                        if (monAn != null && !string.IsNullOrWhiteSpace(monAn.TenMonAn))
                        {
                            // Tránh trùng lặp trong cùng một bữa
                            if (!addedNames.Contains(monAn.TenMonAn))
                            {
                                uniqueFoods.Add(monAn);
                                addedNames.Add(monAn.TenMonAn);
                                
                                // Chỉ lấy tối đa 3 món
                                if (uniqueFoods.Count >= 3)
                                    break;
                            }
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Sau khi lọc trùng lặp, còn {uniqueFoods.Count} món cho {loaiBuaAn}");
                    
                    int index = 0;
                    foreach (var monAn in uniqueFoods)
                    {
                        try
                        {
                            // Tính số lượng đa dạng dựa trên mục tiêu và loại món ăn
                            double? khoiLuong = CalculateSuggestedQuantity(monAn, loaiBuaAn, isGiamCan, index, uniqueFoods.Count);
                            
                            // Lưu số lượng đề xuất để tính toán thống kê
                            if (khoiLuong.HasValue)
                            {
                                _khoiLuongDeXuat[monAn.TenMonAn] = khoiLuong.Value;
                            }
                            
                            var item = new ucMonAnItem(monAn);

                            // Để item nằm giữa, trừ đi Padding của FlowLayoutPanel
                            var clientWidth = panel.ClientSize.Width - panel.Padding.Horizontal;
                            if (clientWidth > 0)
                            {
                                item.Width = clientWidth;
                            }
                            item.Margin = new Padding(0, 3, 0, 8);

                            item.MonAnClicked += (s, food) => OpenAddFoodForm(food);
                            panel.Controls.Add(item);
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
                ShowErrorInPanel(panel, string.Format(GenericErrorTextFormat, ex.Message));
            }
        }

        /// <summary>
        /// Hiển thị lỗi trong panel
        /// </summary>
        private void ShowErrorInPanel(FlowLayoutPanel panel, string errorMessage)
        {
            panel.Controls.Clear();
            var lblError = new Label
            {
                Text = errorMessage,
                AutoSize = false,
                Width = panel.Width - 20,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Red,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblError);
        }

        private void OpenAddFoodForm(ThuVienMonAn monAn)
        {
            if (monAn == null)
            {
                MessageBox.Show(FoodInfoMissingText, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var db = new WF_HealthTracker())
            using (var form = new frmThemMonAn(monAn, db))
            {
                form.ShowDialog();
            }
        }

        private void BtnThemMon1_Click(object sender, EventArgs e)
        {
            ShowMealSelectionMenu(sender as Control, new Dictionary<string, List<ThuVienMonAn>>
            {
                { "Bữa sáng", _loadedFoodsSang },
                { "Bữa trưa", _loadedFoodsTrua }
            });
        }

        private void BtnThemMon2_Click(object sender, EventArgs e)
        {
            ShowMealSelectionMenu(sender as Control, new Dictionary<string, List<ThuVienMonAn>>
            {
                { "Bữa tối", _loadedFoodsToi }
            });
        }

        private void BtnThemMon3_Click(object sender, EventArgs e)
        {
            ShowMealSelectionMenu(sender as Control, new Dictionary<string, List<ThuVienMonAn>>
            {
                { "Bữa phụ", _loadedFoodsPhu }
            });
        }

        private void ShowMealSelectionMenu(Control anchorControl, Dictionary<string, List<ThuVienMonAn>> mealOptions)
        {
            if (mealOptions == null || mealOptions.Count == 0)
            {
                MessageBox.Show(NoFoodsToAddText, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var menu = new ContextMenuStrip();
            var availableMeals = mealOptions.Where(o => o.Value != null && o.Value.Count > 0).ToList();

            if (availableMeals.Count == 0)
        {
                MessageBox.Show(NoGoalFoodsText, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool hasMultiple = availableMeals.Count > 1;
            foreach (var option in availableMeals)
            {
                if (hasMultiple)
                {
                    var mealItem = new ToolStripMenuItem(option.Key);
                    AddFoodMenuItems(mealItem.DropDownItems, option.Value);
                    if (mealItem.DropDownItems.Count > 0)
                    {
                        menu.Items.Add(mealItem);
                    }
                }
                else
                {
                    AddFoodMenuItems(menu.Items, option.Value);
                }
            }

            if (menu.Items.Count == 0)
            {
                MessageBox.Show(NoAvailableFoodsText, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                menu.Dispose();
                return;
            }

            var control = anchorControl ?? this;
            var screenPoint = control.PointToScreen(new Point(0, control.Height));
            menu.Show(screenPoint);
        }

        private void AddFoodMenuItems(ToolStripItemCollection items, List<ThuVienMonAn> foods)
        {
            foreach (var food in foods.Take(5))
            {
                var item = new ToolStripMenuItem(food.TenMonAn)
                {
                    Tag = food
                };
                item.Click += (s, e) =>
                {
                    if (item.Tag is ThuVienMonAn selectedFood)
                    {
                        OpenAddFoodForm(selectedFood);
                    }
                };
                items.Add(item);
            }
        }

        private void ShowLoading(string message = LoadingDefaultText)
        {
            if (_loadingOverlay == null) return;

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowLoading(message)));
                return;
            }

            _loadingCounter++;
            _loadingLabel.Text = message;
            _loadingOverlay.Visible = true;
            _loadingOverlay.Enabled = true;
            _loadingOverlay.BringToFront();
        }

        private void HideLoading()
        {
            if (_loadingOverlay == null) return;

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(HideLoading));
                return;
            }

            _loadingCounter = Math.Max(0, _loadingCounter - 1);
            if (_loadingCounter == 0)
            {
                _loadingOverlay.Visible = false;
                _loadingOverlay.Enabled = false;
            }
        }

        /// <summary>
        /// Reload món ăn đề xuất (có thể gọi khi thay đổi ngày hoặc mục tiêu)
        /// </summary>
        public void ReloadSuggestedFoods()
        {
            _ = ReloadSuggestedFoodsInternalAsync();
        }

        private async Task ReloadSuggestedFoodsInternalAsync()
        {
            ShowLoading(LoadingMealsText);
            try
        {
            LoadUserGoal(); // Reload mục tiêu khi thay đổi ngày/tuần
                await LoadSuggestedFoodsAsync(); // Load async để không block UI
            }
            finally
            {
                HideLoading();
            }
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
                // Dispose ChatGPT service
                _chatGPTService?.Dispose();
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

        /// <summary>
        /// Load 3 facts về sức khỏe và hiển thị trong phần "Bạn có biết"
        /// </summary>
        private async Task LoadHealthFactsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LoadHealthFactsAsync START ===");
                
                // Lấy 3 facts về sức khỏe (có thể dùng ChatGPT hoặc danh sách có sẵn)
                List<string> facts = await GetHealthFactsAsync();
                
                // Hiển thị facts trên UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => DisplayHealthFacts(facts)));
                }
                else
                {
                    DisplayHealthFacts(facts);
                }
                
                System.Diagnostics.Debug.WriteLine("=== LoadHealthFactsAsync END ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load health facts: {ex.Message}");
                // Fallback: dùng facts mặc định
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => DisplayHealthFacts(GetDefaultHealthFacts())));
                }
                else
                {
                    DisplayHealthFacts(GetDefaultHealthFacts());
                }
            }
        }

        /// <summary>
        /// Lấy 3 facts về sức khỏe (có thể dùng ChatGPT hoặc danh sách có sẵn)
        /// </summary>
        private async Task<List<string>> GetHealthFactsAsync()
        {
            try
            {
                // Thử dùng ChatGPT để lấy facts ngẫu nhiên
                if (_chatGPTService != null)
        {
                    string prompt = "Hãy đưa ra 3 facts ngắn gọn về sức khỏe và dinh dưỡng (mỗi fact 1-2 câu, tối đa 50 từ). Format: mỗi fact trên một dòng, không đánh số.";
                    string response = await _chatGPTService.GetSimpleResponseAsync(prompt);
                    
                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        // Parse response thành list
                        var facts = response.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(f => !string.IsNullOrWhiteSpace(f))
                            .Select(f => f.Trim())
                            .Where(f => f.Length > 10) // Bỏ qua dòng quá ngắn
                            .Take(3)
                            .ToList();
                        
                        if (facts.Count >= 3)
                        {
                            System.Diagnostics.Debug.WriteLine("Đã lấy facts từ ChatGPT");
                            return facts;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi lấy facts từ ChatGPT: {ex.Message}");
            }
            
            // Fallback: dùng facts mặc định
            return GetDefaultHealthFacts();
        }

        /// <summary>
        /// Danh sách facts mặc định về sức khỏe
        /// </summary>
        private List<string> GetDefaultHealthFacts()
        {
            var defaultFacts = new List<string>
            {
                "Ăn uống cân đối với đủ chất dinh dưỡng giúp duy trì sức khỏe tốt.",
                "Vận động thường xuyên giúp cải thiện sức khỏe tim mạch và hệ tiêu hóa.",
                "Uống đủ nước hàng ngày giúp duy trì cân nặng và làm cho da sáng hơn."
            };
            
            // Xáo trộn để hiển thị ngẫu nhiên
            var random = new Random();
            var shuffled = defaultFacts.OrderBy(x => random.Next()).Take(3).ToList();
            
            // Đảm bảo tất cả facts đều có giá trị
            for (int i = 0; i < shuffled.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(shuffled[i]))
                {
                    shuffled[i] = defaultFacts[i % defaultFacts.Count];
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"GetDefaultHealthFacts: Trả về {shuffled.Count} facts");
            foreach (var fact in shuffled)
            {
                System.Diagnostics.Debug.WriteLine($"  - '{fact}'");
            }
            
            return shuffled;
        }

        /// <summary>
        /// Hiển thị 3 facts trong guna2Panel15
        /// </summary>
        private void DisplayHealthFacts(List<string> facts)
        {
            try
            {
                if (guna2Panel15 == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: guna2Panel15 is NULL!");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"=== DisplayHealthFacts START ===");
                System.Diagnostics.Debug.WriteLine($"guna2Panel15 - Size: {guna2Panel15.Size}, Location: {guna2Panel15.Location}, Visible: {guna2Panel15.Visible}");
                System.Diagnostics.Debug.WriteLine($"label20 - Location: {label20?.Location}, Bottom: {label20?.Bottom}, Text: {label20?.Text}");
                System.Diagnostics.Debug.WriteLine($"label21 - Location: {label21?.Location}, Bottom: {label21?.Bottom}, Text: {label21?.Text}");

                // Xóa các controls cũ (trừ label20 và label21)
                var controlsToRemove = guna2Panel15.Controls.Cast<Control>()
                    .Where(c => c != label20 && c != label21 && c.Name != "label20" && c.Name != "label21")
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine($"Đang xóa {controlsToRemove.Count} controls cũ");
                foreach (var ctrl in controlsToRemove)
                {
                    guna2Panel15.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }

                if (facts == null || facts.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Không có facts để hiển thị");
                    return;
                }

                // Đảm bảo có đủ 3 facts
                while (facts.Count < 3)
                {
                    facts.Add("Thông tin đang được cập nhật...");
                }

                // Tính toán kích thước và vị trí
                int panelWidth = guna2Panel15.Width;
                int panelHeight = guna2Panel15.Height;
                int padding = 20;
                int cardSpacing = 15;
                
                // Vị trí bắt đầu (dưới label20 và label21)
                int startY = 80; // Bắt đầu từ 80px để tránh label
                if (label20 != null && label21 != null)
                {
                    startY = Math.Max(label20.Bottom, label21.Bottom) + 25; // Lấy bottom lớn nhất + 25px
                }
                else if (label20 != null)
                {
                    startY = label20.Bottom + 25;
                }
                else if (label21 != null)
                {
                    startY = label21.Bottom + 25;
                }
                
                int availableHeight = panelHeight - startY - 20; // Trừ padding dưới
                
                // Kích thước mỗi card
                int cardWidth = Math.Max(200, panelWidth - (padding * 2));
                int cardHeight = Math.Max(60, (availableHeight - (cardSpacing * 2)) / 3); // Chia đều cho 3 cards, tối thiểu 60px

                System.Diagnostics.Debug.WriteLine($"Panel: {panelWidth}x{panelHeight}, StartY: {startY}, AvailableHeight: {availableHeight}, CardSize: {cardWidth}x{cardHeight}");

                // Tạo 3 cards để hiển thị facts
                for (int i = 0; i < 3 && i < facts.Count; i++)
                {
                    var factCard = CreateFactCard(facts[i], cardWidth, cardHeight);
                    int cardY = startY + (cardHeight + cardSpacing) * i;
                    factCard.Location = new Point(padding, cardY);
                    
                    System.Diagnostics.Debug.WriteLine($"Tạo card {i + 1}: Location=({padding}, {cardY}), Size=({cardWidth}, {cardHeight}), Text='{facts[i].Substring(0, Math.Min(30, facts[i].Length))}...'");
                    
                    guna2Panel15.Controls.Add(factCard);
                    
                    // Đảm bảo card và controls bên trong visible
                    factCard.Visible = true;
                    factCard.BringToFront();
                    
                    // Force refresh
                    factCard.Invalidate();
                    factCard.Refresh();
                    
                    // Kiểm tra label bên trong card
                    var labelInCard = factCard.Controls.OfType<Label>().FirstOrDefault();
                    if (labelInCard != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Label trong card {i + 1}: Text='{labelInCard.Text}', Visible={labelInCard.Visible}, ForeColor={labelInCard.ForeColor}, BackColor={labelInCard.BackColor}, Size={labelInCard.Size}, TextLength={labelInCard.Text?.Length ?? 0}");
                        labelInCard.Visible = true;
                        labelInCard.BringToFront();
                        labelInCard.Invalidate();
                        labelInCard.Refresh();
                        labelInCard.Update();
                        
                        // Force update text
                        string currentText = labelInCard.Text;
                        if (!string.IsNullOrWhiteSpace(currentText))
                        {
                            labelInCard.Text = ""; // Clear
                            Application.DoEvents();
                            labelInCard.Text = currentText; // Set lại
                            labelInCard.Update();
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"WARNING: Không tìm thấy Label trong card {i + 1}!");
                        // Kiểm tra có controls nào không
                        System.Diagnostics.Debug.WriteLine($"Controls trong card: {factCard.Controls.Count}");
                        foreach (Control ctrl in factCard.Controls)
        {
                            System.Diagnostics.Debug.WriteLine($"  - {ctrl.GetType().Name}: {ctrl.Name}, Visible={ctrl.Visible}, Text='{ctrl.Text}'");
                        }
                    }
                }

                // Đảm bảo panel visible
                guna2Panel15.Visible = true;
                guna2Panel15.Invalidate();
                guna2Panel15.Refresh();

                System.Diagnostics.Debug.WriteLine($"Đã hiển thị {Math.Min(3, facts.Count)} facts trong guna2Panel15");
                System.Diagnostics.Debug.WriteLine($"=== DisplayHealthFacts END ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi hiển thị health facts: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Tạo một card để hiển thị fact
        /// </summary>
        private Guna2Panel CreateFactCard(string fact, int width, int height)
        {
            // Đảm bảo fact không null
            string factText = fact ?? "Thông tin đang được cập nhật...";
            if (string.IsNullOrWhiteSpace(factText))
            {
                factText = "Thông tin đang được cập nhật...";
            }

            var card = new Guna2Panel
            {
                Size = new Size(width, height),
                FillColor = Color.White, // Màu trắng rõ ràng
                BorderRadius = 15,
                BorderThickness = 2,
                BorderColor = Color.FromArgb(19, 217, 195), // Border teal để dễ thấy
                Padding = new Padding(15, 12, 15, 12),
                Name = $"FactCard_{Guid.NewGuid()}" // Tên unique để debug
            };
            
            // Label để hiển thị fact - dùng Label thông thường với cấu hình rõ ràng
            // Đảm bảo factText không null và có giá trị
            if (string.IsNullOrWhiteSpace(factText))
            {
                factText = "Thông tin đang được cập nhật...";
            }
            
            // Thêm bullet point "-" ở đầu nếu chưa có
            string displayText = factText;
            if (!displayText.TrimStart().StartsWith("-"))
            {
                displayText = "- " + displayText.TrimStart();
            }
            
            // Lưu displayText vào Tag để vẽ trong Paint event
            card.Tag = displayText;
            
            var lblFact = new Label
            {
                Text = displayText,
                AutoSize = false,
                Size = new Size(Math.Max(100, width - 30), Math.Max(40, height - 24)),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.Black, // Màu đen rõ ràng
                Location = new Point(15, 12),
                BackColor = Color.White, // Nền trắng để dễ thấy
                Visible = true,
                Enabled = true,
                UseCompatibleTextRendering = true,
                BorderStyle = BorderStyle.None // Không có border
            };
            
            // Debug ngay sau khi tạo label
            System.Diagnostics.Debug.WriteLine($"Tạo label với text: '{lblFact.Text}' (length={lblFact.Text?.Length ?? 0})");
            
            // Cho phép text wrap
            lblFact.MaximumSize = new Size(width - 30, 0);
            lblFact.AutoSize = false;
            
            // Tính toán chiều cao dựa trên text
            try
            {
                using (Graphics g = card.CreateGraphics())
                {
                    SizeF textSize = g.MeasureString(displayText, lblFact.Font, width - 30);
                    int calculatedHeight = (int)Math.Ceiling(textSize.Height) + 10;
                    if (calculatedHeight > height - 24)
                        calculatedHeight = height - 24;
                    lblFact.Height = Math.Max(40, calculatedHeight);
                }
            }
            catch
            {
                lblFact.Height = Math.Max(40, height - 24);
            }
            
            // Thêm label vào card TRƯỚC khi set các thuộc tính khác
            card.Controls.Add(lblFact);
            
            // Đảm bảo label hiển thị đúng - set lại sau khi add vào card
            lblFact.BringToFront();
            lblFact.Visible = true;
            lblFact.Show();
            lblFact.Invalidate();
            lblFact.Refresh();
            lblFact.Update();
            
            // Force text update
            string tempText = lblFact.Text;
            lblFact.Text = "";
            Application.DoEvents();
            lblFact.Text = tempText;
            lblFact.Update();
            
            // Vẽ text trực tiếp lên card bằng Paint event (backup nếu label không hiển thị)
            card.Paint += (s, e) =>
            {
                try
                {
                    Graphics g = e.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    
                    string text = card.Tag as string;
                    if (string.IsNullOrWhiteSpace(text))
                        return;
                    
                    // Đảm bảo có bullet point (nếu chưa có)
                    if (!text.TrimStart().StartsWith("-"))
                    {
                        text = "- " + text.TrimStart();
                    }
                    
                    // Vùng vẽ text (trừ padding)
                    Rectangle textRect = new Rectangle(15, 12, width - 30, height - 24);
                    
                    // Font và brush
                    using (var font = new Font("Segoe UI", 9.5F, FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        // Vẽ text với wrap
                        TextRenderer.DrawText(g, text, font, textRect, Color.Black, 
                            TextFormatFlags.WordBreak | TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.NoPrefix);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi vẽ text trong Paint event: {ex.Message}");
                }
            };
            
            // Debug
            System.Diagnostics.Debug.WriteLine($"Label fact: Text='{lblFact.Text}', Visible={lblFact.Visible}, ForeColor={lblFact.ForeColor}, BackColor={lblFact.BackColor}, Size={lblFact.Size}, Location={lblFact.Location}, Parent={lblFact.Parent?.Name}, TextLength={lblFact.Text?.Length ?? 0}");
            
            // Đảm bảo card visible
            card.Visible = true;
            card.BringToFront();
            
            // Force trigger Paint event để vẽ text
            card.Invalidate();
            card.Update();
            card.Refresh();
            
            // Thêm hover effect
            card.MouseEnter += (s, e) =>
            {
                card.FillColor = Color.FromArgb(240, 250, 248);
                card.BorderColor = Color.FromArgb(19, 217, 195);
                card.BorderThickness = 3;
                card.Invalidate(); // Vẽ lại khi hover
            };
            
            card.MouseLeave += (s, e) =>
            {
                card.FillColor = Color.White;
                card.BorderColor = Color.FromArgb(19, 217, 195);
                card.BorderThickness = 2;
                card.Invalidate(); // Vẽ lại khi leave
            };

            System.Diagnostics.Debug.WriteLine($"Đã tạo card: Size=({card.Size}), FillColor={card.FillColor}, BorderColor={card.BorderColor}, Text='{displayText.Substring(0, Math.Min(50, displayText.Length))}...'");
            System.Diagnostics.Debug.WriteLine($"Label: Size=({lblFact.Size}), Location=({lblFact.Location}), Text='{lblFact.Text}', Visible={lblFact.Visible}");

            return card;
        }
    }
}
