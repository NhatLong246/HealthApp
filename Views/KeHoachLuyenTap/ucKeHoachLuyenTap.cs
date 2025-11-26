using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Models;
using HealthApp.Controllers;
using HealthApp.Common.Helpers;
using HealthApp.Views.Dashboard;
using Models = HealthApp.Models;

namespace HealthApp.Views.KeHoachLuyenTap
{
    public partial class ucKeHoachLuyenTap : UserControl
    {
        private readonly GoalController _goalController;
        private readonly WF_HealthTracker _dbContext;
        private Models.KeHoachLuyenTap _currentWorkoutPlan;
        private Models.MucTieu _currentGoal;
        private List<Models.BuoiTap> _allBuoiTap;
        private DateTime _currentWeekStart; // Ngày bắt đầu tuần hiện tại
        private Models.BuoiTap _selectedBuoiTap; // Buổi tập được chọn

        // Mapping thứ trong tuần
        private readonly Dictionary<string, int> _thuMapping = new Dictionary<string, int>
        {
            { "Thứ 2", 1 },
            { "Thứ 3", 2 },
            { "Thứ 4", 3 },
            { "Thứ 5", 4 },
            { "Thứ 6", 5 },
            { "Thứ 7", 6 },
            { "Chủ nhật", 0 }
        };

        // Buttons cho các ngày trong tuần
        private Guna.UI2.WinForms.Guna2Button[] _dayButtons;
        private Guna.UI2.WinForms.Guna2Button[] _thuButtons;

        public ucKeHoachLuyenTap()
        {
            InitializeComponent();
            _goalController = new GoalController();
            _dbContext = new WF_HealthTracker();
            _currentWeekStart = GetStartOfWeek(DateTime.Today);
            InitializeArrays();
            InitializeEventHandlers();
        }

        private void InitializeArrays()
        {
            _dayButtons = new Guna.UI2.WinForms.Guna2Button[]
            {
                btnNgay1, btnNgay2, btnNgay3, btnNgay4, btnNgay5, btnNgay6, btnNgay7
            };

            _thuButtons = new Guna.UI2.WinForms.Guna2Button[]
            {
                btnThu2, btnThu3, btnThu4, btnThu5, btnThu6, btnThu7, btnChuNhat
            };
        }

        private void InitializeEventHandlers()
        {
            this.Load += UcKeHoachLuyenTap_Load;
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnTroVe.Click += BtnTroVe_Click;
            btnBatDauBaiTap.Click += BtnBatDauBaiTap_Click;
            btnHoanThanh.Click += BtnHoanThanh_Click;
            btnHuyKeHoach.Click += BtnHuyKeHoach_Click;

            // Event handlers cho các nút ngày
            foreach (var btn in _dayButtons)
            {
                btn.Click += DayButton_Click;
            }
        }

        private async void UcKeHoachLuyenTap_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập để xem kế hoạch luyện tập!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await LoadWorkoutPlanAsync();
            LoadWeekCalendar();
        }

        /// <summary>
        /// Load kế hoạch luyện tập đang hoạt động của user
        /// </summary>
        private async Task LoadWorkoutPlanAsync()
        {
            try
            {
                string userId = CurrentUser.UserID;
                if (string.IsNullOrEmpty(userId))
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy kế hoạch luyện tập đang hoạt động
                _currentWorkoutPlan = _dbContext.KeHoachLuyenTap
                    .Where(k => k.UserID == userId && k.TrangThai == "Đang hoạt động")
                    .OrderByDescending(k => k.NgayCapNhat)
                    .FirstOrDefault();

                if (_currentWorkoutPlan == null)
                {
                    MessageBox.Show("Bạn chưa có kế hoạch luyện tập nào đang hoạt động!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lấy mục tiêu liên quan
                if (!string.IsNullOrEmpty(_currentWorkoutPlan.MucTieuID))
                {
                    _currentGoal = _dbContext.MucTieu
                        .FirstOrDefault(m => m.MucTieuID == _currentWorkoutPlan.MucTieuID);
                }

                // Load tất cả buổi tập
                _allBuoiTap = await _goalController.GetBuoiTapByKeHoachTapIdAsync(_currentWorkoutPlan.KeHoachTapID);

                System.Diagnostics.Debug.WriteLine($"=== LoadWorkoutPlanAsync ===");
                System.Diagnostics.Debug.WriteLine($"KeHoachTapID: {_currentWorkoutPlan.KeHoachTapID}");
                System.Diagnostics.Debug.WriteLine($"Loaded {_allBuoiTap?.Count ?? 0} buổi tập");
                
                if (_allBuoiTap != null && _allBuoiTap.Count > 0)
                {
                    foreach (var bt in _allBuoiTap)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - BuoiTapID: {bt.BuoiTapID}, ThuNgay: '{bt.ThuNgay}', GhiChu: '{bt.GhiChu}'");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("  - Không có buổi tập nào!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải kế hoạch luyện tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"LoadWorkoutPlanAsync error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load lịch tuần và highlight các ngày có buổi tập
        /// </summary>
        private void LoadWeekCalendar()
        {
            try
            {
                // Cập nhật label hiển thị tuần
                DateTime weekEnd = _currentWeekStart.AddDays(6);
                lblThangNam.Text = $"{_currentWeekStart:dd/MM/yyyy} - {weekEnd:dd/MM/yyyy}";

                // Cập nhật các nút ngày
                for (int i = 0; i < 7; i++)
                {
                    DateTime currentDate = _currentWeekStart.AddDays(i);
                    _dayButtons[i].Text = currentDate.Day.ToString();
                    _dayButtons[i].Tag = currentDate;

                    // Reset màu
                    _dayButtons[i].FillColor = Color.White;
                    _dayButtons[i].ForeColor = Color.FromArgb(64, 64, 64);

                    // Kiểm tra xem ngày này có buổi tập không
                    bool hasWorkout = HasWorkoutOnDate(currentDate);
                    if (hasWorkout)
                    {
                        // Highlight ngày có buổi tập
                        _dayButtons[i].FillColor = Color.FromArgb(233, 252, 255); // Màu xanh nhạt
                        _dayButtons[i].ForeColor = Color.Teal;
                    }
                }

                // Cập nhật thông tin chi tiết nếu có buổi tập được chọn
                if (_selectedBuoiTap != null)
                {
                    UpdateDetailInfo(_selectedBuoiTap);
                }
                else
                {
                    ClearDetailInfo();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadWeekCalendar error: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra xem ngày có buổi tập không (so sánh theo ngày thực tế)
        /// </summary>
        private bool HasWorkoutOnDate(DateTime date)
        {
            if (_allBuoiTap == null || _allBuoiTap.Count == 0)
            {
                return false;
            }

            // So sánh theo ngày thực tế (chỉ so sánh phần ngày, bỏ qua giờ)
            DateTime dateOnly = date.Date;
            bool hasWorkout = _allBuoiTap.Any(b => 
                b.ThoiGianBatDau.HasValue && 
                b.ThoiGianBatDau.Value.Date == dateOnly);
            
            return hasWorkout;
        }

        /// <summary>
        /// Chuyển DayOfWeek sang "Thứ X" hoặc "Chủ nhật"
        /// </summary>
        private string GetThuNgayFromDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: return "Thứ 2";
                case DayOfWeek.Tuesday: return "Thứ 3";
                case DayOfWeek.Wednesday: return "Thứ 4";
                case DayOfWeek.Thursday: return "Thứ 5";
                case DayOfWeek.Friday: return "Thứ 6";
                case DayOfWeek.Saturday: return "Thứ 7";
                case DayOfWeek.Sunday: return "Chủ nhật";
                default: return "";
            }
        }

        /// <summary>
        /// Lấy ngày bắt đầu tuần (Thứ 2)
        /// </summary>
        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Xử lý khi click nút ngày
        /// </summary>
        private void DayButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is Guna.UI2.WinForms.Guna2Button btn && btn.Tag != null)
                {
                    DateTime selectedDate = (DateTime)btn.Tag;
                    DateTime dateOnly = selectedDate.Date;

                    // Tìm buổi tập tương ứng theo ngày thực tế (ThoiGianBatDau)
                    _selectedBuoiTap = _allBuoiTap?.FirstOrDefault(b => 
                        b.ThoiGianBatDau.HasValue && 
                        b.ThoiGianBatDau.Value.Date == dateOnly);

                    if (_selectedBuoiTap != null)
                    {
                        UpdateDetailInfo(_selectedBuoiTap);
                    }
                    else
                    {
                        ClearDetailInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn ngày: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết
        /// </summary>
        private void UpdateDetailInfo(Models.BuoiTap buoiTap)
        {
            try
            {
                // Parse giờ bắt đầu và kết thúc từ GhiChu hoặc từ ThoiGianBatDau/ThoiGianKetThuc
                string gioBatDau = "hh:mm";
                string gioKetThuc = "hh:mm";

                if (buoiTap.ThoiGianBatDau.HasValue)
                {
                    gioBatDau = buoiTap.ThoiGianBatDau.Value.ToString("HH:mm");
                }
                else if (!string.IsNullOrEmpty(buoiTap.GhiChu))
                {
                    // Parse từ GhiChu: "Giờ: 07:00 - 11:00"
                    var timeMatch = System.Text.RegularExpressions.Regex.Match(
                        buoiTap.GhiChu, @"Giờ:\s*(\d{1,2}:\d{2})\s*-\s*(\d{1,2}:\d{2})");
                    if (timeMatch.Success)
                    {
                        gioBatDau = timeMatch.Groups[1].Value;
                        gioKetThuc = timeMatch.Groups[2].Value;
                    }
                }

                if (buoiTap.ThoiGianKetThuc.HasValue)
                {
                    gioKetThuc = buoiTap.ThoiGianKetThuc.Value.ToString("HH:mm");
                }

                lblGioBatDau.Text = gioBatDau;
                lblGioKetThuc.Text = gioKetThuc;

                // Tính số ngày còn tập
                int soNgayConTap = CalculateRemainingDays();
                lblSoNgayConTap.Text = soNgayConTap.ToString();

                // Kiểm tra trạng thái và cập nhật nút "Bắt đầu bài tập"
                bool isCompleted = buoiTap.TrangThai == "Hoàn thành";
                btnBatDauBaiTap.Enabled = !isCompleted;
                if (isCompleted)
                {
                    btnBatDauBaiTap.Text = "Đã hoàn thành";
                }
                else
                {
                    btnBatDauBaiTap.Text = "Bắt đầu bài tập";
                }

                // Hiển thị chi tiết bài tập nếu có
                if (buoiTap.BaiTapChiTiet != null && buoiTap.BaiTapChiTiet.Count > 0)
                {
                    var firstBaiTap = buoiTap.BaiTapChiTiet.FirstOrDefault();
                    if (firstBaiTap?.ThuVienBaiTap != null)
                    {
                        var baiTap = firstBaiTap.ThuVienBaiTap;
                        lblTenBaiTap.Text = baiTap.TenBaiTap;
                        
                        // Load ảnh minh họa nếu có
                        if (!string.IsNullOrEmpty(baiTap.AnhMinhHoa))
                        {
                            try
                            {
                                // Có thể là URL hoặc đường dẫn file
                                if (baiTap.AnhMinhHoa.StartsWith("http://") || baiTap.AnhMinhHoa.StartsWith("https://"))
                                {
                                    // Load từ URL (cần thêm logic download image)
                                    // Tạm thời bỏ qua
                                }
                                else
                                {
                                    // Load từ file path
                                    if (System.IO.File.Exists(baiTap.AnhMinhHoa))
                                    {
                                        picAnhMinhHoa.Image = System.Drawing.Image.FromFile(baiTap.AnhMinhHoa);
                                    }
                                }
                            }
                            catch
                            {
                                // Nếu không load được ảnh, giữ nguyên ảnh mặc định
                            }
                        }

                        // Hiển thị panel bài tập
                        pnlDanhSachMucTieu.Visible = true;
                    }
                }
                else
                {
                    // Không có bài tập, ẩn panel
                    pnlDanhSachMucTieu.Visible = false;
                    lblTenBaiTap.Text = "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateDetailInfo error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa thông tin chi tiết
        /// </summary>
        private void ClearDetailInfo()
        {
            lblGioBatDau.Text = "hh:mm";
            lblGioKetThuc.Text = "hh:mm";
            lblSoNgayConTap.Text = "00";
            lblTenBaiTap.Text = "";
            pnlDanhSachMucTieu.Visible = false;
            btnBatDauBaiTap.Enabled = false;
            btnBatDauBaiTap.Text = "Bắt đầu bài tập";
        }

        /// <summary>
        /// Tính số ngày còn tập
        /// </summary>
        private int CalculateRemainingDays()
        {
            if (_currentGoal == null)
            {
                return 0;
            }

            DateTime today = DateTime.Today;
            DateTime endDate = _currentGoal.NgayKetThucDuKien;

            if (endDate < today)
            {
                return 0;
            }

            return (endDate - today).Days;
        }

        /// <summary>
        /// Xử lý khi click nút tuần trước
        /// </summary>
        private void BtnPrev_Click(object sender, EventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            LoadWeekCalendar();
        }

        /// <summary>
        /// Xử lý khi click nút tuần sau
        /// </summary>
        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            LoadWeekCalendar();
        }

        /// <summary>
        /// Xử lý khi click nút trở về
        /// </summary>
        private void BtnTroVe_Click(object sender, EventArgs e)
        {
            // Tìm frmDashBoard và load lại dashboard
            Form form = this.FindForm();
            if (form is frmDashBoard dashboard)
            {
                var ucDashBoard = new Dashboard.ucDashBoard(dashboard);
                dashboard.LoadUserControl(ucDashBoard);
            }
        }

        /// <summary>
        /// Xử lý khi click nút "Bắt đầu bài tập"
        /// </summary>
        private void BtnBatDauBaiTap_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedBuoiTap == null)
                {
                    MessageBox.Show("Vui lòng chọn một buổi tập để bắt đầu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Kiểm tra xem buổi tập đã hoàn thành chưa
                if (_selectedBuoiTap.TrangThai == "Hoàn thành")
                {
                    MessageBox.Show("Buổi tập này đã hoàn thành, không thể tập lại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_selectedBuoiTap.BaiTapChiTiet == null || _selectedBuoiTap.BaiTapChiTiet.Count == 0)
                {
                    MessageBox.Show("Buổi tập này chưa có bài tập nào!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tìm frmDashBoard để load ucTrienKhaiBaiTap vào
                Form form = this.FindForm();
                if (form is frmDashBoard dashboard)
                {
                    var ucTrienKhai = new ucTrienKhaiBaiTap();
                    ucTrienKhai.SetBuoiTap(_selectedBuoiTap);
                    dashboard.LoadUserControl(ucTrienKhai);
                }
                else
                {
                    // Fallback: mở form mới
                    using (var newForm = new Form())
                    {
                        newForm.Text = $"Bắt đầu tập luyện - {_selectedBuoiTap.ThuNgay}";
                        newForm.StartPosition = FormStartPosition.CenterScreen;
                        newForm.Size = new System.Drawing.Size(1200, 800);
                        newForm.WindowState = FormWindowState.Normal;

                        var ucTrienKhai = new ucTrienKhaiBaiTap();
                        ucTrienKhai.Dock = DockStyle.Fill;
                        ucTrienKhai.SetBuoiTap(_selectedBuoiTap);
                        newForm.Controls.Add(ucTrienKhai);

                        newForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi bắt đầu bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"BtnBatDauBaiTap_Click error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý khi click nút "Hoàn thành"
        /// </summary>
        private async void BtnHoanThanh_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedBuoiTap == null)
                {
                    MessageBox.Show("Vui lòng chọn một buổi tập!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn đánh dấu buổi tập này là hoàn thành?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Cập nhật trạng thái buổi tập
                    _selectedBuoiTap.TrangThai = "Hoàn thành";
                    
                    // Set NgayThucHien = ngày của ThoiGianBatDau (ngày dự kiến tương ứng với thứ)
                    if (_selectedBuoiTap.ThoiGianBatDau.HasValue)
                    {
                        _selectedBuoiTap.NgayThucHien = _selectedBuoiTap.ThoiGianBatDau.Value;
                    }
                    else
                    {
                        // Nếu không có ThoiGianBatDau, dùng ngày hiện tại
                        _selectedBuoiTap.NgayThucHien = DateTime.Now;
                    }
                    
                    _selectedBuoiTap.NgayCapNhat = DateTime.Now;

                    _dbContext.SaveChanges();

                    MessageBox.Show("Đã đánh dấu buổi tập hoàn thành!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload data để cập nhật trạng thái
                    await LoadWorkoutPlanAsync();
                    LoadWeekCalendar();
                    
                    // Cập nhật lại thông tin chi tiết nếu vẫn đang chọn buổi tập này
                    if (_selectedBuoiTap != null)
                    {
                        // Reload buổi tập từ database để có trạng thái mới nhất
                        var updatedBuoiTap = _allBuoiTap?.FirstOrDefault(b => b.BuoiTapID == _selectedBuoiTap.BuoiTapID);
                        if (updatedBuoiTap != null)
                        {
                            _selectedBuoiTap = updatedBuoiTap;
                            UpdateDetailInfo(_selectedBuoiTap);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật trạng thái: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"BtnHoanThanh_Click error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý khi click nút "Hủy kế hoạch"
        /// </summary>
        private void BtnHuyKeHoach_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentWorkoutPlan == null)
                {
                    MessageBox.Show("Không tìm thấy kế hoạch luyện tập!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn hủy kế hoạch luyện tập này?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Cập nhật trạng thái kế hoạch
                    _currentWorkoutPlan.TrangThai = "Tạm dừng";
                    _currentWorkoutPlan.NgayCapNhat = DateTime.Now;

                    _dbContext.SaveChanges();

                    MessageBox.Show("Đã hủy kế hoạch luyện tập!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Quay về dashboard
                    BtnTroVe_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hủy kế hoạch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"BtnHuyKeHoach_Click error: {ex.Message}");
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        private void DisposeControllers()
        {
            _goalController?.Dispose();
            _dbContext?.Dispose();
        }
    }
}
