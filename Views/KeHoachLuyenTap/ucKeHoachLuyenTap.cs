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
        private Guna.UI2.WinForms.Guna2Button _selectedDayButton; // Button ngày đang được chọn
        private DateTime? _lastSelectedDate; // Lưu ngày đang chọn để restore sau khi reload
        
        // Static variable để lưu ngày cần restore khi quay về từ ucTrienKhaiBaiTap
        private static DateTime? _restoreDateAfterWorkout;

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
            // Đăng ký event handler cho btnDoiLichTap
            if (btnDoiLichTap != null)
            {
                btnDoiLichTap.Click += BtnDoiLichTap_Click;
            }
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
            
            // Restore ngày đã lưu nếu có (khi quay về từ ucTrienKhaiBaiTap)
            if (_restoreDateAfterWorkout.HasValue)
            {
                _lastSelectedDate = _restoreDateAfterWorkout.Value;
                _restoreDateAfterWorkout = null; // Clear sau khi dùng
                
                // Cập nhật tuần hiện tại nếu ngày restore không nằm trong tuần hiện tại
                DateTime weekStartOfRestoreDate = GetStartOfWeek(_lastSelectedDate.Value);
                if (weekStartOfRestoreDate != _currentWeekStart)
                {
                    _currentWeekStart = weekStartOfRestoreDate;
                }
            }
            
            LoadWeekCalendar();
            
            // Luôn hiển thị panel danh sách bài tập
            pnlDanhSachMucTieu.Visible = true;
            
            // Cập nhật trạng thái thông báo
            UpdateThongBaoVisibility();
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
                    // Reset thông tin
                    ClearDetailInfo();
                    // Hiển thị thông báo vì chưa có kế hoạch
                    UpdateThongBaoVisibility();
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
                
                // Cập nhật trạng thái thông báo sau khi load kế hoạch
                UpdateThongBaoVisibility();
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

                // Reset button đang chọn khi chuyển tuần
                _selectedDayButton = null;

                // Cập nhật các nút ngày
                for (int i = 0; i < 7; i++)
                {
                    DateTime currentDate = _currentWeekStart.AddDays(i);
                    _dayButtons[i].Text = currentDate.Day.ToString();
                    _dayButtons[i].Tag = currentDate;

                    // Mặc định màu bình thường
                    _dayButtons[i].FillColor = Color.White;
                    _dayButtons[i].ForeColor = Color.FromArgb(64, 64, 64);

                    // Kiểm tra trạng thái buổi tập trong ngày
                    bool hasWorkout = HasWorkoutOnDate(currentDate);
                    bool hasCompletedWorkout = HasCompletedWorkoutOnDate(currentDate);

                    if (hasCompletedWorkout)
                    {
                        // Ngày có buổi tập đã hoàn thành → tô xám
                        _dayButtons[i].FillColor = Color.FromArgb(210, 210, 210);
                        _dayButtons[i].ForeColor = Color.DimGray;
                    }
                    else if (hasWorkout)
                    {
                        // Ngày có buổi tập chưa hoàn thành → tô xanh nhạt
                        _dayButtons[i].FillColor = Color.FromArgb(233, 252, 255);

                        _dayButtons[i].ForeColor = Color.Teal;
                    }
                }

                // Restore lại ngày đang chọn (ưu tiên _lastSelectedDate)
                DateTime? dateToRestore = _lastSelectedDate;
                if (dateToRestore.HasValue)
                {
                    // Tìm lại button tương ứng với ngày đã chọn
                    for (int i = 0; i < 7; i++)
                    {
                        if (_dayButtons[i].Tag != null && ((DateTime)_dayButtons[i].Tag).Date == dateToRestore.Value.Date)
                        {
                            // Reset button trước đó
                            if (_selectedDayButton != null && _selectedDayButton != _dayButtons[i])
                            {
                                ResetDayButton(_selectedDayButton);
                            }
                            
                            HighlightSelectedDay(_dayButtons[i]);
                            _selectedDayButton = _dayButtons[i];
                            
                            // Tìm lại buổi tập tương ứng
                            _selectedBuoiTap = _allBuoiTap?.FirstOrDefault(b => 
                                b.ThoiGianBatDau.HasValue && 
                                b.ThoiGianBatDau.Value.Date == dateToRestore.Value.Date);
                            
                            if (_selectedBuoiTap != null)
                            {
                                UpdateDetailInfo(_selectedBuoiTap);
                            }
                            break;
                        }
                    }
                }
                else if (_selectedDayButton != null && _selectedDayButton.Tag != null)
                {
                    // Fallback: sử dụng _selectedDayButton nếu không có _lastSelectedDate
                    DateTime selectedDate = (DateTime)_selectedDayButton.Tag;
                    DateTime dateOnly = selectedDate.Date;
                    
                    // Tìm lại button tương ứng với ngày đã chọn
                    for (int i = 0; i < 7; i++)
                    {
                        if (_dayButtons[i].Tag != null && ((DateTime)_dayButtons[i].Tag).Date == dateOnly)
                        {
                            HighlightSelectedDay(_dayButtons[i]);
                            _selectedDayButton = _dayButtons[i];
                            break;
                        }
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
                
                // Cập nhật thông báo
                UpdateThongBaoVisibility();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadWeekCalendar error: {ex.Message}");
            }
        }

        /// <summary>
        /// Highlight nhẹ button ngày được chọn
        /// </summary>
        private void HighlightSelectedDay(Guna.UI2.WinForms.Guna2Button button)
        {
            if (button == null) return;

            // Màu highlight nhẹ (màu tím nhạt)
            button.FillColor = Color.FromArgb(200, 190, 255); // Màu tím nhạt hơn
            button.ForeColor = Color.FromArgb(100, 88, 255); // Màu tím đậm hơn cho text
        }

        /// <summary>
        /// Reset button về trạng thái ban đầu (có buổi tập hoặc bình thường)
        /// </summary>
        private void ResetDayButton(Guna.UI2.WinForms.Guna2Button button)
        {
            if (button == null || button.Tag == null) return;

            DateTime date = (DateTime)button.Tag;
            bool hasWorkout = HasWorkoutOnDate(date);
            bool hasCompletedWorkout = HasCompletedWorkoutOnDate(date);

            if (hasCompletedWorkout)
            {
                // Ngày có buổi tập đã hoàn thành → tô xám
                button.FillColor = Color.FromArgb(210, 210, 210);
                button.ForeColor = Color.DimGray;
            }
            else if (hasWorkout)
            {
                // Ngày có buổi tập chưa hoàn thành → tô xanh nhạt
                button.FillColor = Color.FromArgb(233, 252, 255);
                button.ForeColor = Color.Teal;
            }
            else
            {
                // Màu bình thường
                button.FillColor = Color.White;
                button.ForeColor = Color.FromArgb(64, 64, 64);
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
        /// Kiểm tra xem ngày có buổi tập đã hoàn thành không
        /// </summary>
        private bool HasCompletedWorkoutOnDate(DateTime date)
        {
            if (_allBuoiTap == null || _allBuoiTap.Count == 0)
            {
                return false;
            }

            DateTime dateOnly = date.Date;
            return _allBuoiTap.Any(b =>
                b.ThoiGianBatDau.HasValue &&
                b.ThoiGianBatDau.Value.Date == dateOnly &&
                b.TrangThai == "Hoàn thành");
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
                    // Reset button ngày trước đó về trạng thái ban đầu
                    if (_selectedDayButton != null && _selectedDayButton != btn)
                    {
                        ResetDayButton(_selectedDayButton);
                    }

                    // Highlight button ngày được chọn
                    HighlightSelectedDay(btn);
                    _selectedDayButton = btn;

                    DateTime selectedDate = (DateTime)btn.Tag;
                    DateTime dateOnly = selectedDate.Date;
                    
                    // Lưu ngày đang chọn
                    _lastSelectedDate = dateOnly;

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
                    
                    // Cập nhật thông báo
                    UpdateThongBaoVisibility();
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
                // Tính số buổi tập (tổng số buổi tập trong kế hoạch)
                int soBuoiTap = _allBuoiTap?.Count ?? 0;
                lblSoBuoiTap.Text = soBuoiTap.ToString();

                // Tính số lượng bài tập (số bài tập trong buổi tập được chọn)
                int soLuongBaiTap = buoiTap.BaiTapChiTiet?.Count ?? 0;
                lblSoLuongBaiTap.Text = soLuongBaiTap.ToString();

                // Tính số ngày còn tập
                int soNgayConTap = CalculateRemainingDays();
                lblSoNgayConTap.Text = soNgayConTap.ToString();

                // Kiểm tra trạng thái và cập nhật nút "Dời lịch tập"
                bool isCompleted = buoiTap.TrangThai == "Hoàn thành";
                if (btnDoiLichTap != null)
                {
                    btnDoiLichTap.Enabled = !isCompleted;
                    if (isCompleted)
                    {
                        btnDoiLichTap.Text = "Đã hoàn thành";
                    }
                    else
                    {
                        btnDoiLichTap.Text = "Dời lịch tập";
                    }
                }

                // Load ThuVienBaiTap cho mỗi BaiTapChiTiet nếu chưa có
                using (var dbContext = new WF_HealthTracker())
                {
                    foreach (var baiTapChiTiet in buoiTap.BaiTapChiTiet)
                    {
                        if (baiTapChiTiet.ThuVienBaiTap == null && !string.IsNullOrEmpty(baiTapChiTiet.BaiTapID))
                        {
                            dbContext.Entry(baiTapChiTiet)
                                .Reference(bt => bt.ThuVienBaiTap)
                                .Load();
                        }
                    }
                }

                // Load danh sách bài tập vào DataGridView
                LoadBaiTapToDataGridView(buoiTap);

                // Panel luôn hiển thị, chỉ cập nhật thông báo
                pnlDanhSachMucTieu.Visible = true;
                UpdateThongBaoVisibility();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateDetailInfo error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load danh sách bài tập vào DataGridView
        /// </summary>
        private void LoadBaiTapToDataGridView(Models.BuoiTap buoiTap)
        {
            try
            {
                if (dgvDanhSachBaiTap == null)
                    return;

                // Clear DataGridView
                dgvDanhSachBaiTap.DataSource = null;
                dgvDanhSachBaiTap.Rows.Clear();
                dgvDanhSachBaiTap.Columns.Clear();

                if (buoiTap.BaiTapChiTiet == null || buoiTap.BaiTapChiTiet.Count == 0)
                {
                    return;
                }

                // Xác định buổi (Sáng, Chiều, Tối) từ ThoiGianBatDau
                string buoiText = GetBuoiText(buoiTap.ThoiGianBatDau);

                // Kiểm tra trạng thái buổi tập
                bool isBuoiTapCompleted = buoiTap.TrangThai == "Hoàn thành";

                // Tạo DataTable với các cột mới
                DataTable dt = new DataTable();
                dt.Columns.Add("BaiTapChiTietID", typeof(string)); // Ẩn, dùng để lưu ID
                dt.Columns.Add("Tên bài tập", typeof(string));
                dt.Columns.Add("Số Set", typeof(string));
                dt.Columns.Add("Số Rep", typeof(string));
                dt.Columns.Add("Cấp độ", typeof(string));
                dt.Columns.Add("Dụng cụ", typeof(string));
                dt.Columns.Add("Buổi", typeof(string));
                dt.Columns.Add("Tập luyện", typeof(string)); // Text column, sẽ thay bằng button sau

                // Điền dữ liệu
                foreach (var baiTapChiTiet in buoiTap.BaiTapChiTiet.OrderBy(b => b.ThuTuThucHien))
                {
                    DataRow row = dt.NewRow();
                    row["BaiTapChiTietID"] = baiTapChiTiet.BaiTapChiTietID ?? "";
                    row["Tên bài tập"] = baiTapChiTiet.ThuVienBaiTap?.TenBaiTap ?? "N/A";
                    row["Số Set"] = baiTapChiTiet.SoSet?.ToString() ?? (baiTapChiTiet.ThuVienBaiTap?.SoSet ?? "N/A");
                    row["Số Rep"] = baiTapChiTiet.SoRep?.ToString() ?? (baiTapChiTiet.ThuVienBaiTap?.SoRep ?? "N/A");
                    
                    // Cấp độ
                    string capDo = baiTapChiTiet.ThuVienBaiTap?.CapDo ?? "";
                    row["Cấp độ"] = MapCapDoToVietnamese(capDo);
                    
                    // Dụng cụ
                    row["Dụng cụ"] = baiTapChiTiet.ThuVienBaiTap?.DungCu ?? "Không cần";
                    
                    // Buổi
                    row["Buổi"] = buoiText;
                    
                    // Tập luyện
                    row["Tập luyện"] = isBuoiTapCompleted ? "Đã tập" : "Bắt đầu";
                    
                    dt.Rows.Add(row);
                }

                // Bind vào DataGridView
                dgvDanhSachBaiTap.DataSource = dt;

                // Cấu hình columns và tăng kích thước font, row height
                if (dgvDanhSachBaiTap.Columns.Count > 0)
                {
                    // Ẩn cột BaiTapChiTietID
                    if (dgvDanhSachBaiTap.Columns["BaiTapChiTietID"] != null)
                    {
                        dgvDanhSachBaiTap.Columns["BaiTapChiTietID"].Visible = false;
                    }

                    // Tăng kích thước font cho header
                    dgvDanhSachBaiTap.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                    dgvDanhSachBaiTap.ColumnHeadersHeight = 45; // Tăng chiều cao header
                    
                    // Tăng kích thước font cho các dòng nội dung
                    dgvDanhSachBaiTap.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
                    
                    // Set row height SAU KHI bind để đảm bảo tất cả row đều có height đúng
                    dgvDanhSachBaiTap.RowTemplate.Height = 50;
                    foreach (DataGridViewRow row in dgvDanhSachBaiTap.Rows)
                    {
                        row.Height = 50;
                    }
                    
                    // Cấu hình độ rộng cột
                    if (dgvDanhSachBaiTap.Columns["Tên bài tập"] != null)
                    {
                        dgvDanhSachBaiTap.Columns["Tên bài tập"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    if (dgvDanhSachBaiTap.Columns["Số Set"] != null)
                    {
                        dgvDanhSachBaiTap.Columns["Số Set"].Width = 80;
                    }
                    if (dgvDanhSachBaiTap.Columns["Số Rep"] != null)
                    {
                        dgvDanhSachBaiTap.Columns["Số Rep"].Width = 80;
                    }
                    if (dgvDanhSachBaiTap.Columns["Cấp độ"] != null)
                    {
                        dgvDanhSachBaiTap.Columns["Cấp độ"].Width = 100;
                    }
                    if (dgvDanhSachBaiTap.Columns["Dụng cụ"] != null)
                    {
                        dgvDanhSachBaiTap.Columns["Dụng cụ"].Width = 120;
                    }
                    if (dgvDanhSachBaiTap.Columns["Buổi"] != null)
                    {
                        dgvDanhSachBaiTap.Columns["Buổi"].Width = 80;
                    }
                    
                    // Thay thế cột text "Tập luyện" bằng button column
                    if (dgvDanhSachBaiTap.Columns["Tập luyện"] != null)
                    {
                        int columnIndex = dgvDanhSachBaiTap.Columns["Tập luyện"].Index;
                        dgvDanhSachBaiTap.Columns.Remove("Tập luyện");
                        
                        var buttonColumn = new DataGridViewButtonColumn();
                        buttonColumn.Name = "Tập luyện";
                        buttonColumn.HeaderText = "Tập luyện";
                        // Sử dụng Value của từng ô để hiển thị text, tránh lệch / sai text
                        buttonColumn.UseColumnTextForButtonValue = false;
                        // Bind với cột "Tập luyện" trong DataTable để tự nhận giá trị "Bắt đầu"/"Đã tập"
                        buttonColumn.DataPropertyName = "Tập luyện";
                        buttonColumn.Width = 120;
                        buttonColumn.FlatStyle = FlatStyle.Flat;
                        // Căn giữa nội dung nút cho đẹp
                        buttonColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        
                        // Set style đơn giản
                        if (isBuoiTapCompleted)
                        {
                            buttonColumn.DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
                            buttonColumn.DefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100);
                        }
                        else
                        {
                            buttonColumn.DefaultCellStyle.BackColor = Color.FromArgb(100, 88, 255);
                            buttonColumn.DefaultCellStyle.ForeColor = Color.White;
                        }
                        buttonColumn.DefaultCellStyle.SelectionBackColor = buttonColumn.DefaultCellStyle.BackColor;
                        buttonColumn.DefaultCellStyle.SelectionForeColor = buttonColumn.DefaultCellStyle.ForeColor;
                        
                        dgvDanhSachBaiTap.Columns.Insert(columnIndex, buttonColumn);
                        
                        // Không cần tự set lại Value vì đã bind qua DataPropertyName.
                        // Nếu buổi tập đã hoàn thành thì chặn sửa trên cả cột.
                        if (isBuoiTapCompleted)
                        {
                            buttonColumn.ReadOnly = true;
                        }
                    }
                    
                    // Đăng ký event handler cho button click (chỉ khi chưa hoàn thành)
                    dgvDanhSachBaiTap.CellContentClick -= DgvDanhSachBaiTap_CellContentClick;
                    if (buoiTap.TrangThai != "Hoàn thành")
                    {
                        dgvDanhSachBaiTap.CellContentClick += DgvDanhSachBaiTap_CellContentClick;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadBaiTapToDataGridView error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xác định buổi (Sáng, Chiều, Tối) từ ThoiGianBatDau
        /// </summary>
        private string GetBuoiText(DateTime? thoiGianBatDau)
        {
            if (!thoiGianBatDau.HasValue)
                return "N/A";

            int hour = thoiGianBatDau.Value.Hour;
            
            if (hour >= 5 && hour < 11)
                return "Sáng";
            else if (hour >= 11 && hour < 17)
                return "Chiều";
            else if (hour >= 17 && hour < 22)
                return "Tối";
            else
                return "N/A";
        }

        /// <summary>
        /// Map cấp độ từ tiếng Anh sang tiếng Việt
        /// </summary>
        private string MapCapDoToVietnamese(string capDo)
        {
            if (string.IsNullOrWhiteSpace(capDo))
                return "N/A";

            switch (capDo.ToLower())
            {
                case "beginner":
                    return "Người mới";
                case "intermediate":
                    return "Trung cấp";
                case "advanced":
                    return "Nâng cao";
                case "all levels":
                    return "Tất cả";
                default:
                    return capDo;
            }
        }

        /// <summary>
        /// Event handler khi click button "Bắt đầu" trong DataGridView
        /// </summary>
        private void DgvDanhSachBaiTap_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var dgv = sender as DataGridView;
                if (dgv == null || e.RowIndex < 0) return;

                // Chỉ xử lý khi click vào cột button "Tập luyện"
                if (dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn && 
                    dgv.Columns[e.ColumnIndex].Name == "Tập luyện")
                {
                    // Kiểm tra xem button có phải "Đã tập" không (đã hoàn thành)
                    var buttonCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (buttonCell != null && buttonCell.Value != null && 
                        buttonCell.Value.ToString() == "Đã tập")
                    {
                        // Button "Đã tập" không thể click
                        return;
                    }
                    
                    // Lấy BaiTapChiTietID từ row
                    string baiTapChiTietID = null;
                    if (dgv.Rows[e.RowIndex].Cells["BaiTapChiTietID"] != null && 
                        dgv.Rows[e.RowIndex].Cells["BaiTapChiTietID"].Value != null)
                    {
                        baiTapChiTietID = dgv.Rows[e.RowIndex].Cells["BaiTapChiTietID"].Value.ToString();
                    }

                    if (string.IsNullOrWhiteSpace(baiTapChiTietID) || _selectedBuoiTap == null)
                    {
                        MessageBox.Show("Không tìm thấy thông tin bài tập!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    // Kiểm tra trạng thái buổi tập
                    if (_selectedBuoiTap.TrangThai == "Hoàn thành")
                    {
                        MessageBox.Show("Buổi tập này đã hoàn thành!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Lưu ngày đang chọn trước khi điều hướng
                    if (_selectedBuoiTap?.ThoiGianBatDau.HasValue == true)
                    {
                        _restoreDateAfterWorkout = _selectedBuoiTap.ThoiGianBatDau.Value.Date;
                    }
                    
                    // Tìm frmDashBoard để load ucTrienKhaiBaiTap
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi bắt đầu bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"DgvDanhSachBaiTap_CellContentClick error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa thông tin chi tiết
        /// </summary>
        private void ClearDetailInfo()
        {
            lblSoBuoiTap.Text = "00";
            lblSoLuongBaiTap.Text = "00";
            lblSoNgayConTap.Text = "00";
            
            // Panel luôn hiển thị
            pnlDanhSachMucTieu.Visible = true;
            
            // Clear DataGridView
            if (dgvDanhSachBaiTap != null)
            {
                dgvDanhSachBaiTap.DataSource = null;
                dgvDanhSachBaiTap.Rows.Clear();
            }
            
            // Reset button
            if (btnDoiLichTap != null)
            {
                btnDoiLichTap.Enabled = false;
                btnDoiLichTap.Text = "Dời lịch tập";
            }
            
            // Reset button ngày được chọn về trạng thái ban đầu
            if (_selectedDayButton != null)
            {
                ResetDayButton(_selectedDayButton);
                _selectedDayButton = null;
            }
            
            // Cập nhật thông báo
            UpdateThongBaoVisibility();
        }

        /// <summary>
        /// Cập nhật hiển thị/ẩn thông báo dựa trên việc có kế hoạch hay không
        /// </summary>
        private void UpdateThongBaoVisibility()
        {
            try
            {
                if (lblThongBao == null)
                    return;

                // Nếu chưa có kế hoạch hoặc không có buổi tập được chọn → hiển thị thông báo
                if (_currentWorkoutPlan == null || _selectedBuoiTap == null)
                {
                    lblThongBao.Visible = true;
                }
                else
                {
                    // Có kế hoạch và có buổi tập được chọn → ẩn thông báo
                    lblThongBao.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateThongBaoVisibility error: {ex.Message}");
            }
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
        /// Xử lý khi click nút "Dời lịch tập"
        /// </summary>
        private async void BtnDoiLichTap_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedBuoiTap == null)
                {
                    MessageBox.Show("Vui lòng chọn một buổi tập để dời lịch!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Kiểm tra xem buổi tập đã hoàn thành chưa
                if (_selectedBuoiTap.TrangThai == "Hoàn thành")
                {
                    MessageBox.Show("Buổi tập này đã hoàn thành, không thể dời lịch!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lưu BuoiTapID để tìm lại sau khi dời lịch
                string buoiTapIdToFind = _selectedBuoiTap?.BuoiTapID;

                // Mở form dời lịch tập
                using (var frmDoiLich = new frmDoiLichTap())
                {
                    frmDoiLich.SetBuoiTap(_selectedBuoiTap, _allBuoiTap, _currentGoal);
                    if (frmDoiLich.ShowDialog() == DialogResult.OK)
                    {
                        // Reload data sau khi dời lịch thành công
                        await LoadWorkoutPlanAsync();
                        
                        // Tìm lại buổi tập đã được dời (dựa vào BuoiTapID gốc)
                        if (!string.IsNullOrEmpty(buoiTapIdToFind))
                        {
                            // Reload lại từ database để có thông tin mới nhất
                            using (var dbContext = new WF_HealthTracker())
                            {
                                var updatedBuoiTap = dbContext.BuoiTap
                                    .FirstOrDefault(b => b.BuoiTapID == buoiTapIdToFind);
                                
                                if (updatedBuoiTap != null)
                                {
                                    // Load BaiTapChiTiet navigation property
                                    dbContext.Entry(updatedBuoiTap)
                                        .Collection(bt => bt.BaiTapChiTiet)
                                        .Load();
                                    
                                    // Load ThuVienBaiTap cho mỗi BaiTapChiTiet
                                    foreach (var btc in updatedBuoiTap.BaiTapChiTiet)
                                    {
                                        if (btc.ThuVienBaiTap == null && !string.IsNullOrEmpty(btc.BaiTapID))
                                        {
                                            dbContext.Entry(btc)
                                                .Reference(bt => bt.ThuVienBaiTap)
                                                .Load();
                                        }
                                    }
                                    
                                    // Cập nhật lại trong _allBuoiTap
                                    var index = _allBuoiTap?.FindIndex(b => b.BuoiTapID == buoiTapIdToFind);
                                    if (index.HasValue && index.Value >= 0 && _allBuoiTap != null)
                                    {
                                        _allBuoiTap[index.Value] = updatedBuoiTap;
                                    }
                                    
                                    _selectedBuoiTap = updatedBuoiTap;
                                }
                            }
                            
                            // Cập nhật lại calendar để highlight đúng ngày mới
                            if (_selectedBuoiTap != null && _selectedBuoiTap.ThoiGianBatDau.HasValue)
                            {
                                DateTime newDate = _selectedBuoiTap.ThoiGianBatDau.Value.Date;
                                // Cập nhật tuần hiện tại nếu ngày mới không nằm trong tuần hiện tại
                                DateTime weekStartOfNewDate = GetStartOfWeek(newDate);
                                if (weekStartOfNewDate != _currentWeekStart)
                                {
                                    _currentWeekStart = weekStartOfNewDate;
                                }
                            }
                            
                            // Reload calendar và cập nhật thông tin chi tiết
                            LoadWeekCalendar();
                            
                            if (_selectedBuoiTap != null)
                            {
                                UpdateDetailInfo(_selectedBuoiTap);
                                
                                // Highlight button ngày mới
                                if (_selectedBuoiTap.ThoiGianBatDau.HasValue)
                                {
                                    DateTime newDate = _selectedBuoiTap.ThoiGianBatDau.Value.Date;
                                    for (int i = 0; i < 7; i++)
                                    {
                                        if (_dayButtons[i].Tag != null && 
                                            ((DateTime)_dayButtons[i].Tag).Date == newDate.Date)
                                        {
                                            // Reset button trước đó
                                            if (_selectedDayButton != null && _selectedDayButton != _dayButtons[i])
                                            {
                                                ResetDayButton(_selectedDayButton);
                                            }
                                            
                                            // Highlight button ngày mới
                                            HighlightSelectedDay(_dayButtons[i]);
                                            _selectedDayButton = _dayButtons[i];
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Nếu không tìm thấy, chỉ reload calendar
                            LoadWeekCalendar();
                        }
                        
                        // Cập nhật thông báo
                        UpdateThongBaoVisibility();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi dời lịch tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"BtnDoiLichTap_Click error: {ex.Message}");
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
                    // Lưu ngày đang chọn và BuoiTapID trước khi cập nhật
                    DateTime? currentSelectedDate = _selectedBuoiTap?.ThoiGianBatDau?.Date;
                    string buoiTapIdToUpdate = _selectedBuoiTap?.BuoiTapID;
                    
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
                    
                    // Restore lại ngày đang chọn
                    if (currentSelectedDate.HasValue)
                    {
                        _lastSelectedDate = currentSelectedDate.Value;
                    }
                    
                    LoadWeekCalendar();
                    
                    // Cập nhật lại thông tin chi tiết với buổi tập đã hoàn thành
                    if (!string.IsNullOrEmpty(buoiTapIdToUpdate))
                    {
                        // Reload buổi tập từ database để có trạng thái mới nhất
                        var updatedBuoiTap = _allBuoiTap?.FirstOrDefault(b => b.BuoiTapID == buoiTapIdToUpdate);
                        if (updatedBuoiTap != null)
                        {
                            // Load navigation properties
                            using (var dbContext = new WF_HealthTracker())
                            {
                                dbContext.Entry(updatedBuoiTap)
                                    .Collection(bt => bt.BaiTapChiTiet)
                                    .Load();
                                
                                foreach (var btc in updatedBuoiTap.BaiTapChiTiet)
                                {
                                    if (btc.ThuVienBaiTap == null && !string.IsNullOrEmpty(btc.BaiTapID))
                                    {
                                        dbContext.Entry(btc)
                                            .Reference(bt => bt.ThuVienBaiTap)
                                            .Load();
                                    }
                                }
                            }
                            
                            _selectedBuoiTap = updatedBuoiTap;
                            UpdateDetailInfo(_selectedBuoiTap);
                        }
                    }
                    
                    // Cập nhật thông báo
                    UpdateThongBaoVisibility();
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
