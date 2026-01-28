using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using HealthApp.Controllers;
using HealthApp.Common.Helpers;
using HealthApp.Models;
using HealthApp.Services;

namespace HealthApp.Views.Nutrition
{
    public partial class ucKeHoachAnUong : UserControl
    {
        private sealed class SuggestedFoodTag
        {
            public ThuVienMonAn MonAn { get; }
            public string LoaiBuaAn { get; }
            public SuggestedFoodTag(ThuVienMonAn monAn, string loaiBuaAn) { MonAn = monAn; LoaiBuaAn = loaiBuaAn; }
        }
        private NutritionController _nutritionController;
        private GoalController _goalController;
        private ChatGPTService _chatGPTService;

        private FlowLayoutPanel _pnlScrollBuaSang;
        private FlowLayoutPanel _pnlScrollBuaTrua;
        private FlowLayoutPanel _pnlScrollBuaToi;
        private FlowLayoutPanel _pnlScrollBuaPhu;

        private DateTime _selectedDate;

        // Mục tiêu dinh dưỡng từ KeHoachAnUong (4 chỉ số)
        private double? _targetCalories;
        private double? _targetProtein;
        private double? _targetCarbs;
        private double? _targetFat;

        private string _keHoachAnID;
        private readonly List<string> _monAnDaDeXuatTrongNgay = new List<string>();

        private Panel _chartPanel;
        private double _chartCalories;
        private double _chartProtein;
        private double _chartCarbs;
        private double _chartFat;

        private double _lastTotalCal;
        private double _lastTotalPro;
        private double _lastTotalCarb;
        private double _lastTotalFat;
        private readonly List<(ThuVienMonAn mon, string loai)> _suggestedToSave = new List<(ThuVienMonAn, string)>();
        private readonly HashSet<DateTime> _savedDatesLoadFromDbOnly = new HashSet<DateTime>();

        public ucKeHoachAnUong()
        {
            InitializeComponent();

            _nutritionController = new NutritionController();
            _goalController = new GoalController();
            _chatGPTService = new ChatGPTService();

            _selectedDate = DateTime.Today;
            if (guna2DateTimePicker1 != null)
                guna2DateTimePicker1.Value = DateTime.Today;

            InitializeScrollPanels();
            InitializeChartPanel();
            InitializeDanhGiaLabel();
            RegisterEventHandlers();

            _ = LoadDataAsync();
        }

        private void InitializeDanhGiaLabel()
        {
            if (lblDanhGia == null) return;
            lblDanhGia.Visible = true;
            lblDanhGia.AutoSize = false;
            lblDanhGia.ForeColor = Color.FromArgb(50, 50, 50);
            var parent = lblDanhGia.Parent;
            if (parent != null)
            {
                int w = Math.Max(200, parent.Width - 18);
                int h = Math.Max(80, parent.Height - 30);
                lblDanhGia.Size = new Size(w, h);
                lblDanhGia.MaximumSize = new Size(w, 0);
                lblDanhGia.Location = new Point(9, 15);
            }
            else
            {
                lblDanhGia.Size = new Size(300, 180);
                lblDanhGia.MaximumSize = new Size(300, 0);
            }
            if (panel3 != null)
            {
                panel3.Visible = true;
                panel3.BringToFront();
            }
        }

        private void ApplyDanhGiaSize()
        {
            if (lblDanhGia == null) return;
            var p = lblDanhGia.Parent;
            int w = p != null ? Math.Max(200, p.Width - 18) : 300;
            lblDanhGia.MaximumSize = new Size(w, 0);
            if (p != null && lblDanhGia.Height < 60)
            {
                int h = Math.Max(80, p.Height - 30);
                lblDanhGia.Size = new Size(w, h);
            }
        }

        private void InitializeChartPanel()
        {
            if (pnlChart == null) return;
            _chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0)
            };
            _chartPanel.Paint += PnlChart_Paint;
            _chartPanel.Resize += (s, e) => _chartPanel?.Invalidate();
            pnlChart.Controls.Add(_chartPanel);
            _chartPanel.BringToFront();
        }

        private void InitializeScrollPanels()
        {
            _pnlScrollBuaSang = CreateMealFlowPanel();
            _pnlScrollBuaTrua = CreateMealFlowPanel();
            _pnlScrollBuaToi = CreateMealFlowPanel();
            _pnlScrollBuaPhu = CreateMealFlowPanel();

            if (pnlBuaSang != null)
            {
                pnlBuaSang.Controls.Clear();
                pnlBuaSang.Controls.Add(_pnlScrollBuaSang);
            }
            if (pnlBuaTrua != null)
            {
                pnlBuaTrua.Controls.Clear();
                pnlBuaTrua.Controls.Add(_pnlScrollBuaTrua);
            }
            if (pnlBuaToi != null)
            {
                pnlBuaToi.Controls.Clear();
                pnlBuaToi.Controls.Add(_pnlScrollBuaToi);
            }
            if (pnlBuaPhu != null)
            {
                pnlBuaPhu.Controls.Clear();
                pnlBuaPhu.Controls.Add(_pnlScrollBuaPhu);
            }
        }

        private FlowLayoutPanel CreateMealFlowPanel()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(8),
                BackColor = Color.Transparent
            };
        }

        private void RegisterEventHandlers()
        {
            if (guna2DateTimePicker1 != null)
            {
                guna2DateTimePicker1.ValueChanged += (s, e) =>
                {
                    _selectedDate = guna2DateTimePicker1.Value.Date;
                    _ = LoadDataAsync();
                };
            }

            if (btnThemMonSang != null) btnThemMonSang.Click += (s, e) => OpenThemMonAnForMeal("Sáng");
            if (btnThemMonTrua != null) btnThemMonTrua.Click += (s, e) => OpenThemMonAnForMeal("Trưa");
            if (btnThemMonToi != null) btnThemMonToi.Click += (s, e) => OpenThemMonAnForMeal("Tối");
            if (btnThemMonPhu != null) btnThemMonPhu.Click += (s, e) => OpenThemMonAnForMeal("Bữa phụ");
            if (btnLuuMonAn != null) btnLuuMonAn.Click += (s, e) => BtnLuuMonAn_Click();
        }

        private void OpenThemMonAnForMeal(string loaiBuaAn)
        {
            using (var frmDs = new frmDanhSachMonAn())
            {
                if (frmDs.ShowDialog() != DialogResult.OK || frmDs.SelectedFood == null)
                    return;
                var monAn = frmDs.SelectedFood;
                using (var db = new WF_HealthTracker())
                {
                    var frm = new frmThemMonAn(monAn, db, _keHoachAnID, loaiBuaAn, _selectedDate);
                    if (frm.ShowDialog() == DialogResult.OK)
                        _ = LoadDataAsync();
                }
            }
        }

        private async void BtnLuuMonAn_Click()
        {
            if (!CurrentUser.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập để lưu món ăn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool hasTarget = lblMucTieu != null && lblMucTieu.Text != "Chưa có mục tiêu";
            if (!hasTarget)
            {
                MessageBox.Show("Chưa có mục tiêu. Không thể kiểm tra và lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_targetCalories.HasValue || _targetCalories.Value <= 0)
            {
                MessageBox.Show("Chưa có mục tiêu dinh dưỡng. Hãy thiết lập KeHoachAnUong trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            double cal = _lastTotalCal, pro = _lastTotalPro, carb = _lastTotalCarb, fat = _lastTotalFat;
            double tCal = _targetCalories.Value, tPro = _targetProtein ?? 0, tCarb = _targetCarbs ?? 0, tFat = _targetFat ?? 0;

            var fails = new List<string>();
            if (tCal > 0)
            {
                double r = cal / tCal;
                if (r < 0.9 || r > 1.1) fails.Add("Calo");
            }
            if (tPro > 0)
            {
                double r = pro / tPro;
                if (r < 0.9 || r > 1.1) fails.Add("Protein");
            }
            if (tCarb > 0)
            {
                double r = carb / tCarb;
                if (r < 0.9 || r > 1.1) fails.Add("Carbs");
            }
            if (tFat > 0)
            {
                double r = fat / tFat;
                if (r < 0.9 || r > 1.1) fails.Add("Fat");
            }

            if (fails.Count > 0)
            {
                string msg = "Các chỉ số sau chưa đáp ứng biên độ ±10% so với mục tiêu:\n\n• " + string.Join("\n• ", fails) +
                    "\n\nVui lòng điều chỉnh bữa ăn rồi thử lại.";
                MessageBox.Show(msg, "Cảnh báo – Chưa đủ điều kiện lưu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var (saved, savedKeys) = await GetSavedMealsAndKeysForDateAsync();
                string keHoachAnId = _keHoachAnID;
                if (string.IsNullOrWhiteSpace(keHoachAnId))
                {
                    using (var db = new WF_HealthTracker())
                    {
                        keHoachAnId = GetOrCreateKeHoachAnUong(db);
                    }
                }
                if (string.IsNullOrWhiteSpace(keHoachAnId))
                {
                    MessageBox.Show("Không thể xác định kế hoạch ăn uống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var addedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var db = new WF_HealthTracker())
                {
                    var ngay = _selectedDate.Date;
                    int nextIndex = GetNextBuaAnIndex(db);
                    int idx = 0;
                    foreach (var (mon, loai) in _suggestedToSave)
                    {
                        var loaiDb = loai.Equals("Bữa phụ", StringComparison.OrdinalIgnoreCase) ? "Phụ" : loai;
                        var key = $"{mon.MonAnID}|{(loaiDb == "Phụ" ? "Bữa phụ" : loaiDb)}";
                        if (savedKeys.Contains(key) || addedKeys.Contains(key)) continue;

                        double khoiLuong = mon.KhoiLuongChuan ?? 100;
                        double tiLe = khoiLuong / 100.0;
                        var entity = new BuaAnChiTiet
                        {
                            BuaAnID = $"meal_{nextIndex + idx:D4}",
                            KeHoachAnID = keHoachAnId,
                            MonAnID = mon.MonAnID,
                            LoaiBuaAn = loaiDb,
                            NgayAn = ngay,
                            TenMonAn = mon.TenMonAn ?? "N/A",
                            Donvi = mon.Donvi ?? "g",
                            KhoiLuongChuan = khoiLuong,
                            Calories = (mon.Calories ?? 0) * tiLe,
                            Protein = (mon.Protein ?? 0) * tiLe,
                            Carbs = (mon.Carbs ?? 0) * tiLe,
                            Fat = (mon.Fat ?? 0) * tiLe,
                            Fiber = (mon.Fiber ?? 0) * tiLe,
                            GhiChu = "LoaiBuaAn: " + loai,
                            NgayCapNhat = DateTime.Now
                        };
                        db.BuaAnChiTiet.Add(entity);
                        addedKeys.Add(key);
                        idx++;
                    }
                    db.SaveChanges();
                }

                _savedDatesLoadFromDbOnly.Add(_selectedDate.Date);
                _ = LoadDataAsync();
                MessageBox.Show("Đã lưu món ăn vào kế hoạch. Dữ liệu hiển thị từ database.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu món ăn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                ClearAllMealPanels();
                _monAnDaDeXuatTrongNgay.Clear();
                _suggestedToSave.Clear();

                LoadUserGoalToLabel();

                if (lblMucTieu != null && lblMucTieu.Text == "Chưa có mục tiêu")
                {
                    var (saved, _) = await GetSavedMealsAndKeysForDateAsync();
                    AddSavedMealsToPanels(saved);
                    var (sc, sp, sb, sf) = SumSavedNutrition(saved);
                    UpdateNutritionSummary(sc, sp, sb, sf, hasTarget: false);
                    return;
                }

                await LoadKeHoachAnUongTargetsAsync();
                var (savedList, savedKeys) = await GetSavedMealsAndKeysForDateAsync();

                bool loadOnlyFromDb = _savedDatesLoadFromDbOnly.Contains(_selectedDate.Date);
                if (loadOnlyFromDb)
                {
                    AddSavedMealsToPanels(savedList);
                    var (sC, sP, sB, sF) = SumSavedNutrition(savedList);
                    UpdateNutritionSummary(sC, sP, sB, sF, hasTarget: true);
                    return;
                }

                var (sugCal, sugPro, sugCarb, sugFat) = await LoadSuggestedMealsAsync(savedKeys);
                AddSavedMealsToPanels(savedList);
                var (savedCal, savedPro, savedCarb, savedFat) = SumSavedNutrition(savedList);
                UpdateNutritionSummary(sugCal + savedCal, sugPro + savedPro, sugCarb + savedCarb, sugFat + savedFat, hasTarget: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải kế hoạch ăn uống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserGoalToLabel()
        {
            try
            {
                if (lblMucTieu == null)
                    return;

                if (!CurrentUser.IsLoggedIn)
                {
                    lblMucTieu.Text = "Chưa có mục tiêu";
                    return;
                }

                var goals = _goalController.GetGoalsByUser(CurrentUser.UserID, "Đang thực hiện");
                if (goals != null && goals.Count > 0)
                {
                    var firstGoal = goals.FirstOrDefault();
                    lblMucTieu.Text = !string.IsNullOrWhiteSpace(firstGoal?.LoaiMucTieu)
                        ? firstGoal.LoaiMucTieu
                        : "Chưa có mục tiêu";
                }
                else
                {
                    lblMucTieu.Text = "Chưa có mục tiêu";
                }
            }
            catch
            {
                if (lblMucTieu != null) lblMucTieu.Text = "Chưa có mục tiêu";
            }
        }

        private async Task LoadKeHoachAnUongTargetsAsync()
        {
            await Task.Yield();

            if (!CurrentUser.IsLoggedIn)
                return;

            using (var dbContext = new WF_HealthTracker())
            {
                var goals = _goalController.GetGoalsByUser(CurrentUser.UserID, "Đang thực hiện");
                var activeGoal = goals?.FirstOrDefault();
                if (activeGoal == null)
                    return;

                var keHoachAn = dbContext.KeHoachAnUong
                    .FirstOrDefault(k => k.MucTieuID == activeGoal.MucTieuID &&
                                         (k.TrangThai == "Đang thực hiện" || k.TrangThai == "Active" || k.TrangThai == "Đang hoạt động" || k.TrangThai == null));

                if (keHoachAn == null)
                    return;

                _keHoachAnID = keHoachAn.KeHoachAnID;
                _targetCalories = keHoachAn.TongCalories;
                _targetProtein = keHoachAn.TongProtein;
                _targetCarbs = keHoachAn.TongCarbs;
                _targetFat = keHoachAn.TongFat;
            }
        }

        private string GetOrCreateKeHoachAnUong(WF_HealthTracker dbContext)
        {
            try
            {
                var keHoachAn = dbContext.KeHoachAnUong
                    .Where(k => k.TrangThai == "Đang hoạt động" ||
                           k.TrangThai == "N'Đang hoạt động'" ||
                           k.TrangThai == null ||
                           string.IsNullOrEmpty(k.TrangThai))
                    .FirstOrDefault();

                if (keHoachAn == null)
                {
                    keHoachAn = new KeHoachAnUong
                    {
                        KeHoachAnID = $"meal_{DateTime.Now:yyyyMMddHHmmss}",
                        TrangThai = "Đang hoạt động",
                        MoTa = "Kế hoạch ăn uống tự do"
                    };
                    dbContext.KeHoachAnUong.Add(keHoachAn);
                    dbContext.SaveChanges();
                }

                return keHoachAn.KeHoachAnID;
            }
            catch
            {
                return null;
            }
        }

        private async Task<(double cal, double pro, double carb, double fat)> LoadSuggestedMealsAsync(HashSet<string> savedKeys)
        {
            if (!CurrentUser.IsLoggedIn || _chatGPTService == null)
                return (0, 0, 0, 0);

            // Nếu không có target thì bỏ qua
            if (!_targetCalories.HasValue && !_targetProtein.HasValue && !_targetCarbs.HasValue && !_targetFat.HasValue)
                return (0, 0, 0, 0);

            var mealRatios = new Dictionary<string, double>
            {
                { "Sáng", 0.25 },
                { "Trưa", 0.35 },
                { "Tối", 0.30 },
                { "Bữa phụ", 0.10 }
            };

            double sumCal = 0, sumPro = 0, sumCarb = 0, sumFat = 0;
            var t1 = await LoadSuggestedToPanelAsync("Sáng", _pnlScrollBuaSang, mealRatios["Sáng"], savedKeys);
            var t2 = await LoadSuggestedToPanelAsync("Trưa", _pnlScrollBuaTrua, mealRatios["Trưa"], savedKeys);
            var t3 = await LoadSuggestedToPanelAsync("Tối", _pnlScrollBuaToi, mealRatios["Tối"], savedKeys);
            var t4 = await LoadSuggestedToPanelAsync("Bữa phụ", _pnlScrollBuaPhu, mealRatios["Bữa phụ"], savedKeys);
            sumCal = t1.cal + t2.cal + t3.cal + t4.cal;
            sumPro = t1.pro + t2.pro + t3.pro + t4.pro;
            sumCarb = t1.carb + t2.carb + t3.carb + t4.carb;
            sumFat = t1.fat + t2.fat + t3.fat + t4.fat;
            return (sumCal, sumPro, sumCarb, sumFat);
        }

        private async Task<(double cal, double pro, double carb, double fat)> LoadSuggestedToPanelAsync(string loaiBuaAn, FlowLayoutPanel panel, double ratio, HashSet<string> savedKeys)
        {
            double sumCal = 0, sumPro = 0, sumCarb = 0, sumFat = 0;
            if (panel == null) return (0, 0, 0, 0);

            double targetCalories = (_targetCalories ?? 0) * ratio;
            double targetProtein = (_targetProtein ?? 0) * ratio;
            double targetCarbs = (_targetCarbs ?? 0) * ratio;
            double targetFat = (_targetFat ?? 0) * ratio;

            var foods = await GetSuggestedFoodsWithNutritionTarget(loaiBuaAn, targetCalories, targetProtein, targetCarbs, targetFat);

            panel.SuspendLayout();
            try
            {
                foreach (var f in foods)
                {
                    var key = $"{f.MonAnID}|{loaiBuaAn}";
                    if (savedKeys != null && savedKeys.Contains(key))
                        continue;
                    double khoiLuong = f.KhoiLuongChuan ?? 100;
                    double r = khoiLuong / 100.0;
                    double cal = (f.Calories ?? 0) * r;
                    double pro = (f.Protein ?? 0) * r;
                    double carb = (f.Carbs ?? 0) * r;
                    double fat = (f.Fat ?? 0) * r;
                    sumCal += cal; sumPro += pro; sumCarb += carb; sumFat += fat;
                    panel.Controls.Add(CreateFoodCard(f, loaiBuaAn));
                    _suggestedToSave.Add((f, loaiBuaAn));
                    if (!string.IsNullOrWhiteSpace(f.TenMonAn) && !_monAnDaDeXuatTrongNgay.Contains(f.TenMonAn))
                        _monAnDaDeXuatTrongNgay.Add(f.TenMonAn);
                }
            }
            finally
            {
                panel.ResumeLayout();
            }
            return (sumCal, sumPro, sumCarb, sumFat);
        }

        private async Task<List<ThuVienMonAn>> GetSuggestedFoodsWithNutritionTarget(string loaiBuaAn, double targetCalories, double targetProtein, double targetCarbs, double targetFat)
        {
            try
            {
                using (var dbContext = new WF_HealthTracker())
                {
                    var allFoods = dbContext.ThuVienMonAn.ToList();
                    if (allFoods == null || allFoods.Count == 0)
                        return new List<ThuVienMonAn>();

                    var danhSachTenMonAn = allFoods.Select(f => f.TenMonAn).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

                    string mucTieu = (lblMucTieu != null && lblMucTieu.Text != "Chưa có mục tiêu") ? lblMucTieu.Text : null;
                    var suggestedNames = await _chatGPTService.SuggestFoodsAsync(loaiBuaAn, mucTieu, danhSachTenMonAn, _selectedDate, _monAnDaDeXuatTrongNgay);

                    if (suggestedNames == null || suggestedNames.Count == 0)
                        suggestedNames = danhSachTenMonAn.Take(8).ToList();

                    var result = new List<ThuVienMonAn>();
                    double sumCal = 0, sumPro = 0, sumCarb = 0, sumFat = 0;

                    foreach (var name in suggestedNames)
                    {
                        var monAn = allFoods.FirstOrDefault(f => f.TenMonAn != null && f.TenMonAn.Equals(name, StringComparison.OrdinalIgnoreCase))
                                 ?? allFoods.FirstOrDefault(f => f.TenMonAn != null && f.TenMonAn.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);

                        if (monAn == null) continue;

                        double khoiLuong = monAn.KhoiLuongChuan ?? 100;
                        double r = khoiLuong / 100.0;
                        double cal = (monAn.Calories ?? 0) * r;
                        double pro = (monAn.Protein ?? 0) * r;
                        double carb = (monAn.Carbs ?? 0) * r;
                        double fat = (monAn.Fat ?? 0) * r;

                        double newCal = sumCal + cal;
                        double newPro = sumPro + pro;
                        double newCarb = sumCarb + carb;
                        double newFat = sumFat + fat;

                        bool ok = true;
                        if (targetCalories > 0 && newCal > targetCalories * 1.1) ok = false;
                        if (targetProtein > 0 && newPro > targetProtein * 1.1) ok = false;
                        if (targetCarbs > 0 && newCarb > targetCarbs * 1.1) ok = false;
                        if (targetFat > 0 && newFat > targetFat * 1.1) ok = false;

                        if (!ok) break;

                        result.Add(monAn);
                        sumCal = newCal; sumPro = newPro; sumCarb = newCarb; sumFat = newFat;

                        if (result.Count >= 7) break;
                    }

                    return result;
                }
            }
            catch
            {
                return new List<ThuVienMonAn>();
            }
        }

        private void ClearAllMealPanels()
        {
            _pnlScrollBuaSang?.Controls.Clear();
            _pnlScrollBuaTrua?.Controls.Clear();
            _pnlScrollBuaToi?.Controls.Clear();
            _pnlScrollBuaPhu?.Controls.Clear();
        }

        private Control CreateFoodCard(ThuVienMonAn monAn, string loaiBuaAn)
        {
            double khoiLuong = monAn.KhoiLuongChuan ?? 100;
            double r = khoiLuong / 100.0;
            double cal = (monAn.Calories ?? 0) * r;
            double pro = (monAn.Protein ?? 0) * r;
            double carb = (monAn.Carbs ?? 0) * r;
            double fat = (monAn.Fat ?? 0) * r;

            var card = new Guna2Panel
            {
                Width = 460,
                Height = 90,
                Margin = new Padding(0, 0, 0, 10),
                BorderRadius = 12,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(19, 217, 195),
                FillColor = Color.White,
                Padding = new Padding(12),
                Cursor = Cursors.Hand
            };
            card.Tag = new SuggestedFoodTag(monAn, loaiBuaAn);
            card.Click += OnSuggestedFoodCardClick;

            var lblName = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 24,
                Font = new Font("Times New Roman", 11, FontStyle.Bold),
                Text = $"{monAn.TenMonAn} - {khoiLuong:0}{monAn.Donvi ?? "g"}",
                Cursor = Cursors.Hand
            };
            lblName.Click += OnSuggestedFoodCardClick;

            var lblLine2 = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.Gray,
                Font = new Font("Times New Roman", 10),
                Text = $"P: {pro:0.0}g   C: {carb:0.0}g   F: {fat:0.0}g",
                Cursor = Cursors.Hand
            };
            lblLine2.Click += OnSuggestedFoodCardClick;

            var lblCalo = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.FromArgb(255, 128, 0),
                Font = new Font("Times New Roman", 10, FontStyle.Bold),
                Text = $"{cal:0} kcal",
                Cursor = Cursors.Hand
            };
            lblCalo.Click += OnSuggestedFoodCardClick;

            card.Controls.Add(lblCalo);
            card.Controls.Add(lblLine2);
            card.Controls.Add(lblName);

            return card;
        }

        private void OnSuggestedFoodCardClick(object sender, EventArgs e)
        {
            var c = sender as Control;
            var card = c as Guna2Panel ?? c?.Parent as Guna2Panel;
            var tag = card?.Tag as SuggestedFoodTag;
            if (tag == null) return;

            if (!CurrentUser.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập để sử dụng tính năng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var db = new WF_HealthTracker())
                {
                    string keHoachAnId = _keHoachAnID;
                    if (string.IsNullOrWhiteSpace(keHoachAnId))
                        keHoachAnId = GetOrCreateKeHoachAnUong(db);
                    if (string.IsNullOrWhiteSpace(keHoachAnId)) return;

                    var ngayBatDau = _selectedDate.Date;
                    var ngayKetThuc = _selectedDate.Date.AddDays(1).AddTicks(-1);
                    var monAn = tag.MonAn;
                    var loaiBuaAn = tag.LoaiBuaAn ?? "Sáng";
                    var loaiDb = loaiBuaAn.Equals("Bữa phụ", StringComparison.OrdinalIgnoreCase) ? "Phụ" : loaiBuaAn;

                    var buaAnChiTiet = db.BuaAnChiTiet
                        .FirstOrDefault(b => b.KeHoachAnID == keHoachAnId &&
                            b.MonAnID == monAn.MonAnID &&
                            b.NgayAn >= ngayBatDau && b.NgayAn <= ngayKetThuc &&
                            b.LoaiBuaAn == loaiDb);

                    if (buaAnChiTiet == null)
                    {
                        double khoiLuong = monAn.KhoiLuongChuan ?? 100;
                        double tiLe = khoiLuong / 100.0;
                        buaAnChiTiet = new BuaAnChiTiet
                        {
                            BuaAnID = GenerateBuaAnID(db),
                            KeHoachAnID = keHoachAnId,
                            MonAnID = monAn.MonAnID,
                            LoaiBuaAn = loaiDb,
                            NgayAn = _selectedDate.Date,
                            TenMonAn = monAn.TenMonAn ?? "N/A",
                            Donvi = monAn.Donvi ?? "g",
                            KhoiLuongChuan = khoiLuong,
                            Calories = (monAn.Calories ?? 0) * tiLe,
                            Protein = (monAn.Protein ?? 0) * tiLe,
                            Carbs = (monAn.Carbs ?? 0) * tiLe,
                            Fat = (monAn.Fat ?? 0) * tiLe,
                            Fiber = (monAn.Fiber ?? 0) * tiLe,
                            GhiChu = "LoaiBuaAn: " + loaiBuaAn,
                            NgayCapNhat = DateTime.Now
                        };
                        db.BuaAnChiTiet.Add(buaAnChiTiet);
                        db.SaveChanges();
                    }

                    using (var frm = new frmChinhSuaMonAn(buaAnChiTiet, db))
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                            _ = LoadDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form chỉnh sửa món ăn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GenerateBuaAnID(WF_HealthTracker db)
        {
            int next = GetNextBuaAnIndex(db);
            return $"meal_{next:D4}";
        }

        private static int GetNextBuaAnIndex(WF_HealthTracker db)
        {
            var last = db.BuaAnChiTiet.OrderByDescending(m => m.BuaAnID).FirstOrDefault();
            if (last == null || !last.BuaAnID.StartsWith("meal_"))
                return 1;
            if (int.TryParse(last.BuaAnID.Substring(5), out int n))
                return n + 1;
            return db.BuaAnChiTiet.Count() + 1;
        }

        /// <summary>
        /// Load món đã lưu theo ngày, trả về danh sách và tập key "MonAnID|LoaiBuaAn" để tránh hiển thị trùng với suggested.
        /// </summary>
        private async Task<(List<BuaAnChiTiet> saved, HashSet<string> keys)> GetSavedMealsAndKeysForDateAsync()
        {
            await Task.Yield();

            var saved = new List<BuaAnChiTiet>();
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!CurrentUser.IsLoggedIn)
                return (saved, keys);

            string keHoachAnId = _keHoachAnID;
            using (var dbContext = new WF_HealthTracker())
            {
                if (string.IsNullOrWhiteSpace(keHoachAnId))
                    keHoachAnId = GetOrCreateKeHoachAnUong(dbContext);

                if (string.IsNullOrWhiteSpace(keHoachAnId))
                    return (saved, keys);

                _keHoachAnID = keHoachAnId;

                var ngayBatDau = _selectedDate.Date;
                var ngayKetThuc = _selectedDate.Date.AddDays(1).AddTicks(-1);

                saved = dbContext.BuaAnChiTiet
                    .Where(b => b.KeHoachAnID == keHoachAnId &&
                                b.NgayAn >= ngayBatDau &&
                                b.NgayAn <= ngayKetThuc)
                    .OrderBy(b => b.LoaiBuaAn)
                    .ToList();

                foreach (var item in saved)
                {
                    if (string.IsNullOrEmpty(item.MonAnID) || string.IsNullOrEmpty(item.LoaiBuaAn)) continue;
                    var loai = item.LoaiBuaAn.Equals("Phụ", StringComparison.OrdinalIgnoreCase) ? "Bữa phụ" : item.LoaiBuaAn;
                    keys.Add($"{item.MonAnID}|{loai}");
                }
            }

            return (saved, keys);
        }

        private void AddSavedMealsToPanels(List<BuaAnChiTiet> saved)
        {
            if (saved == null) return;
            foreach (var item in saved)
            {
                var targetPanel = MapLoaiBuaAnToPanel(item.LoaiBuaAn);
                if (targetPanel != null)
                    targetPanel.Controls.Add(CreateSavedMealCard(item));
            }
        }

        private (double cal, double pro, double carb, double fat) SumSavedNutrition(List<BuaAnChiTiet> saved)
        {
            double cal = 0, pro = 0, carb = 0, fat = 0;
            if (saved == null) return (0, 0, 0, 0);
            foreach (var m in saved)
            {
                cal += m.Calories ?? 0;
                pro += m.Protein ?? 0;
                carb += m.Carbs ?? 0;
                fat += m.Fat ?? 0;
            }
            return (cal, pro, carb, fat);
        }

        private void UpdateNutritionSummary(double totalCal, double totalPro, double totalCarb, double totalFat, bool hasTarget)
        {
            void DoUpdate()
            {
                if (lblCalo != null) lblCalo.Text = $"{totalCal:0} kcal";
                if (lblProtein != null) lblProtein.Text = $"{totalPro:0.1} g";
                if (lblCarb != null) lblCarb.Text = $"{totalCarb:0.1} g";
                if (lblFat != null) lblFat.Text = $"{totalFat:0.1} g";

                var red = Color.FromArgb(192, 0, 0);
                var green = Color.FromArgb(0, 192, 0);

                void UpdateTyLe(Label lbl, double actual, double? target, string _)
                {
                    if (lbl == null) return;
                    if (!hasTarget || !target.HasValue || target.Value <= 0)
                    {
                        lbl.Text = "—";
                        lbl.ForeColor = Color.Gray;
                        return;
                    }
                    double pct = (actual - target.Value) / target.Value * 100.0;
                    if (pct > 0)
                        lbl.Text = $"vượt quá {Math.Abs(pct):0.#}%";
                    else if (pct < 0)
                        lbl.Text = $"thấp hơn {Math.Abs(pct):0.#}%";
                    else
                        lbl.Text = "đạt mục tiêu";
                    lbl.ForeColor = Math.Abs(pct) > 10 ? red : green;
                }

                UpdateTyLe(lblTyLeCalo, totalCal, _targetCalories, "Calo");
                UpdateTyLe(lblTyLeProtein, totalPro, _targetProtein, "Protein");
                UpdateTyLe(lblTyLeCảb, totalCarb, _targetCarbs, "Carbs");
                UpdateTyLe(lblTyLeFat, totalFat, _targetFat, "Fat");

                if (lblCaloCan != null)
                {
                    lblCaloCan.Visible = hasTarget;
                    lblCaloCan.Text = hasTarget && _targetCalories.HasValue && _targetCalories.Value > 0
                        ? $"{_targetCalories.Value:0} kcal" : "—";
                }
                if (lblProteinCan != null)
                {
                    lblProteinCan.Visible = hasTarget;
                    lblProteinCan.Text = hasTarget && _targetProtein.HasValue && _targetProtein.Value > 0
                        ? $"{_targetProtein.Value:0.1} g" : "—";
                }
                if (lblCarbCan != null)
                {
                    lblCarbCan.Visible = hasTarget;
                    lblCarbCan.Text = hasTarget && _targetCarbs.HasValue && _targetCarbs.Value > 0
                        ? $"{_targetCarbs.Value:0.1} g" : "—";
                }
                if (lblFatCan != null)
                {
                    lblFatCan.Visible = hasTarget;
                    lblFatCan.Text = hasTarget && _targetFat.HasValue && _targetFat.Value > 0
                        ? $"{_targetFat.Value:0.1} g" : "—";
                }

                if (lblDanhGia != null)
                {
                    lblDanhGia.Visible = true;
                    lblDanhGia.Text = "Đang đánh giá...";
                    lblDanhGia.AutoSize = false;
                    ApplyDanhGiaSize();
                }

                _chartCalories = totalCal;
                _chartProtein = totalPro;
                _chartCarbs = totalCarb;
                _chartFat = totalFat;
                _chartPanel?.Invalidate();

                _lastTotalCal = totalCal;
                _lastTotalPro = totalPro;
                _lastTotalCarb = totalCarb;
                _lastTotalFat = totalFat;
            }

            if (InvokeRequired)
                BeginInvoke(new Action(DoUpdate));
            else
                DoUpdate();

            _ = FetchAndUpdateDanhGiaAsync(totalCal, totalPro, totalCarb, totalFat, hasTarget);
        }

        private async Task FetchAndUpdateDanhGiaAsync(double totalCal, double totalPro, double totalCarb, double totalFat, bool hasTarget)
        {
            string text = null;
            try
            {
                if (_chatGPTService == null)
                {
                    text = GetFallbackDanhGia(totalCal, totalPro, totalCarb, totalFat, hasTarget);
                    goto Update;
                }

                if (hasTarget && _targetCalories.HasValue && _targetCalories.Value > 0)
                {
                    string mucTieu = lblMucTieu != null ? lblMucTieu.Text : null;
                    if (string.IsNullOrWhiteSpace(mucTieu) || mucTieu == "Chưa có mục tiêu")
                        mucTieu = null;
                    text = await _chatGPTService.EvaluateNutritionAsync(
                        totalCal,
                        totalCal,
                        _targetCalories.Value,
                        mucTieu,
                        totalPro,
                        totalCarb,
                        totalFat);
                }
                else
                {
                    var prompt = "Bạn là chuyên gia dinh dưỡng. Đánh giá ngắn gọn (1-2 câu, dưới 50 từ) mức dinh dưỡng bữa ăn hôm nay dựa trên: " +
                        $"Calo {totalCal:F0} kcal, Protein {totalPro:F1} g, Carbs {totalCarb:F1} g, Fat {totalFat:F1} g. " +
                        "Chưa có mục tiêu. Viết bằng tiếng Việt, thân thiện, động viên.";
                    text = await _chatGPTService.GetSimpleResponseAsync(prompt);
                }

                if (string.IsNullOrWhiteSpace(text))
                    text = GetFallbackDanhGia(totalCal, totalPro, totalCarb, totalFat, hasTarget);
            }
            catch
            {
                text = GetFallbackDanhGia(totalCal, totalPro, totalCarb, totalFat, hasTarget);
            }

        Update:
            var msg = text ?? "Không thể tải đánh giá.";
            void Set()
            {
                if (lblDanhGia != null)
                {
                    lblDanhGia.Visible = true;
                    lblDanhGia.Text = msg;
                    lblDanhGia.AutoSize = false;
                    ApplyDanhGiaSize();
                }
            }
            if (InvokeRequired)
                BeginInvoke(new Action(Set));
            else
                Set();
        }

        private string GetFallbackDanhGia(double cal, double pro, double carb, double fat, bool hasTarget)
        {
            if (!hasTarget)
                return "Chưa có mục tiêu.\r\nChưa thể đánh giá so với kế hoạch.";
            if (!_targetCalories.HasValue || _targetCalories.Value <= 0)
                return "Dựa trên bữa ăn hiện tại: Calo, Protein, Carbs, Fat đã được ghi nhận. Hãy thiết lập mục tiêu để nhận gợi ý chi tiết.";
            double pct = (cal / _targetCalories.Value) * 100;
            if (pct < 70)
                return "Mức dinh dưỡng đang thấp hơn mục tiêu. Hãy bổ sung thêm bữa ăn để đạt mục tiêu.";
            if (pct < 90)
                return "Mức dinh dưỡng khá tốt nhưng còn thiếu một chút. Hãy cố gắng cải thiện thêm.";
            if (pct <= 110)
                return "Mức dinh dưỡng rất tốt và phù hợp với mục tiêu. Hãy tiếp tục duy trì!";
            if (pct <= 130)
                return "Mức dinh dưỡng đang vượt mục tiêu một chút. Hãy điều chỉnh lại cho phù hợp.";
            return "Mức dinh dưỡng đang vượt mục tiêu nhiều. Hãy xem xét lại chế độ ăn uống.";
        }

        private void PnlChart_Paint(object sender, PaintEventArgs e)
        {
            if (_chartPanel == null) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            if (_chartPanel.Width <= 0 || _chartPanel.Height <= 0) return;

            double scaledCal = _chartCalories / 10.0;
            double maxVal = Math.Max(scaledCal, Math.Max(_chartProtein, Math.Max(_chartCarbs, _chartFat)));
            if (maxVal <= 0) maxVal = 100;

            int pad = 30;
            int cw = _chartPanel.Width - pad * 2;
            int ch = _chartPanel.Height - pad * 2 - 30;
            int bw = Math.Max(40, (cw / 4) - 15);
            int sp = 15;
            int sx = pad + 10;

            var clrP = Color.FromArgb(19, 217, 195);
            var clrC = Color.FromArgb(255, 193, 7);
            var clrF = Color.FromArgb(255, 87, 34);
            var clrK = Color.FromArgb(255, 152, 0);

            DrawChartBar(g, sx, pad, bw, ch, _chartProtein, maxVal, clrP, "Protein");
            DrawChartBar(g, sx + bw + sp, pad, bw, ch, _chartCarbs, maxVal, clrC, "Carbs");
            DrawChartBar(g, sx + (bw + sp) * 2, pad, bw, ch, _chartFat, maxVal, clrF, "Fat");
            DrawChartBar(g, sx + (bw + sp) * 3, pad, bw, ch, scaledCal, maxVal, clrK, "Kcal");
        }

        private void DrawChartBar(Graphics g, int x, int y, int w, int maxH, double val, double maxVal, Color clr, string label)
        {
            int bh = maxVal > 0 ? (int)((val / maxVal) * maxH) : 0;
            if (bh < 0) bh = 0;
            int by = y + maxH - bh;

            if (bh > 0)
            {
                using (var br = new SolidBrush(clr))
                using (var path = GetRoundedRect(new Rectangle(x, by, w, bh), 5))
                {
                    g.FillPath(br, path);
                    using (var pen = new Pen(Color.FromArgb(200, clr), 1))
                        g.DrawPath(pen, path);
                }
            }

            if (val > 0 && bh > 20)
            {
                string txt = label == "Kcal" ? (val * 10).ToString("F0") : val.ToString("F1");
                using (var f = new Font("Segoe UI", 9F, FontStyle.Bold))
                {
                    var sz = g.MeasureString(txt, f);
                    int tx = x + (w - (int)sz.Width) / 2;
                    int ty = by - 20;
                    if (ty < y) { ty = by + 5; g.DrawString(txt, f, Brushes.White, tx, ty); }
                    else g.DrawString(txt, f, Brushes.Black, tx, ty);
                }
            }

            using (var lf = new Font("Segoe UI", 8F))
            {
                var ls = g.MeasureString(label, lf);
                g.DrawString(label, lf, new SolidBrush(Color.FromArgb(100, 100, 100)), x + (w - (int)ls.Width) / 2, y + maxH + 5);
            }
        }

        private static GraphicsPath GetRoundedRect(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            p.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            p.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            p.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            p.CloseAllFigures();
            return p;
        }

        private async Task LoadSavedMealsAsync()
        {
            var (saved, _) = await GetSavedMealsAndKeysForDateAsync();
            AddSavedMealsToPanels(saved);
        }

        private FlowLayoutPanel MapLoaiBuaAnToPanel(string loaiBuaAn)
        {
            if (string.IsNullOrWhiteSpace(loaiBuaAn)) return null;

            if (loaiBuaAn.Equals("Sáng", StringComparison.OrdinalIgnoreCase)) return _pnlScrollBuaSang;
            if (loaiBuaAn.Equals("Trưa", StringComparison.OrdinalIgnoreCase)) return _pnlScrollBuaTrua;
            if (loaiBuaAn.Equals("Tối", StringComparison.OrdinalIgnoreCase)) return _pnlScrollBuaToi;
            if (loaiBuaAn.Equals("Bữa phụ", StringComparison.OrdinalIgnoreCase) || loaiBuaAn.Equals("Phụ", StringComparison.OrdinalIgnoreCase)) return _pnlScrollBuaPhu;

            return null;
        }

        private Control CreateSavedMealCard(BuaAnChiTiet meal)
        {
            var card = new Guna2Panel
            {
                Width = 460,
                Height = 90,
                Margin = new Padding(0, 0, 0, 10),
                BorderRadius = 12,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(19, 217, 195),
                FillColor = Color.White,
                Padding = new Padding(12),
                Cursor = Cursors.Hand
            };
            card.Tag = meal;
            card.Click += OnSavedMealCardClick;

            var lblName = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 24,
                Font = new Font("Times New Roman", 11, FontStyle.Bold),
                Text = $"{meal.TenMonAn} - {meal.KhoiLuongChuan:0}{meal.Donvi ?? "g"}",
                Cursor = Cursors.Hand
            };
            lblName.Click += OnSavedMealCardClick;

            var lblLine2 = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.Gray,
                Font = new Font("Times New Roman", 10),
                Text = $"P: {(meal.Protein ?? 0):0.0}g   C: {(meal.Carbs ?? 0):0.0}g   F: {(meal.Fat ?? 0):0.0}g",
                Cursor = Cursors.Hand
            };
            lblLine2.Click += OnSavedMealCardClick;

            var lblCalo = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.FromArgb(255, 128, 0),
                Font = new Font("Times New Roman", 10, FontStyle.Bold),
                Text = $"{(meal.Calories ?? 0):0} kcal",
                Cursor = Cursors.Hand
            };
            lblCalo.Click += OnSavedMealCardClick;

            card.Controls.Add(lblCalo);
            card.Controls.Add(lblLine2);
            card.Controls.Add(lblName);

            return card;
        }

        private void OnSavedMealCardClick(object sender, EventArgs e)
        {
            var c = sender as Control;
            var card = c as Guna2Panel ?? c?.Parent as Guna2Panel;
            if (card?.Tag == null || !(card.Tag is BuaAnChiTiet meal)) return;

            using (var db = new WF_HealthTracker())
            {
                using (var frm = new frmChinhSuaMonAn(meal, db))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                        _ = LoadDataAsync();
                }
            }
        }

        // Các phần chart/weekly-monthly stats sẽ được gắn sau; hiện ưu tiên fix compile & load món ăn.

        // Dispose(bool) đã được định nghĩa trong Designer.cs
    }
}
