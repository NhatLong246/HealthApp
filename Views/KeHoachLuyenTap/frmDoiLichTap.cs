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
using Models = HealthApp.Models;

namespace HealthApp.Views.KeHoachLuyenTap
{
    public partial class frmDoiLichTap : Form
    {
        private Models.BuoiTap _buoiTapToReschedule;
        private List<Models.BuoiTap> _allBuoiTap;
        private Models.MucTieu _currentGoal;
        private DateTime _currentMonth;
        private DateTime? _selectedDate;
        private WF_HealthTracker _dbContext;
        private List<Guna.UI2.WinForms.Guna2Button> _dateButtons;

        public DateTime? NewSelectedDate { get; private set; }

        public frmDoiLichTap()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            _currentMonth = DateTime.Now;
            _dateButtons = new List<Guna.UI2.WinForms.Guna2Button>();
            InitializeDateButtons();
            InitializeEventHandlers();
        }

        /// <summary>
        /// Set thông tin buổi tập cần dời lịch
        /// </summary>
        public void SetBuoiTap(Models.BuoiTap buoiTap, List<Models.BuoiTap> allBuoiTap, Models.MucTieu goal)
        {
            _buoiTapToReschedule = buoiTap;
            _allBuoiTap = allBuoiTap;
            _currentGoal = goal;
            LoadCalendar();
        }

        /// <summary>
        /// Khởi tạo danh sách các button ngày
        /// </summary>
        private void InitializeDateButtons()
        {
            _dateButtons.Clear();
            var buttons = flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Button>()
                .Where(b => b.Name.StartsWith("btnLich"))
                .OrderBy(b => int.Parse(b.Name.Replace("btnLich", "")))
                .ToList();
            _dateButtons.AddRange(buttons);
        }

        /// <summary>
        /// Khởi tạo event handlers
        /// </summary>
        private void InitializeEventHandlers()
        {
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnXacNhan.Click += btnXacNhan_Click;
            btnHuy.Click += btnHuy_Click;
        }

        /// <summary>
        /// Load calendar và highlight các ngày có buổi tập
        /// </summary>
        private void LoadCalendar()
        {
            try
            {
                UpdateCalendarDisplay();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadCalendar error: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật hiển thị calendar
        /// </summary>
        private void UpdateCalendarDisplay()
        {
            lblThangNam.Text = $"Tháng {_currentMonth:MM}, {_currentMonth:yyyy}";

            // Lấy ngày đầu tiên và cuối cùng của tháng
            DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Tính ngày đầu tiên trong tuần (có thể là ngày cuối tháng trước)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            if (firstDayOfWeek == 0) firstDayOfWeek = 7; // Chủ nhật = 7
            firstDayOfWeek -= 1; // Chuyển về 0-based (Thứ 2 = 0, Chủ nhật = 6)

            // Lấy ngày bắt đầu và kết thúc của kế hoạch
            DateTime? planStartDate = null;
            DateTime? planEndDate = null;
            if (_currentGoal != null)
            {
                planStartDate = _currentGoal.NgayBatDau;
                planEndDate = _currentGoal.NgayKetThucDuKien;
            }

            // Bắt đầu từ ngày đầu tiên hiển thị
            DateTime currentDate = firstDayOfMonth.AddDays(-firstDayOfWeek);

            // Điền các ngày vào calendar
            for (int i = 0; i < _dateButtons.Count; i++)
            {
                var btn = _dateButtons[i];
                btn.Text = currentDate.Day.ToString();
                btn.Tag = currentDate;
                btn.Enabled = true;

                // Xác định màu sắc dựa trên việc ngày có thuộc tháng hiện tại không
                bool isCurrentMonth = currentDate.Month == _currentMonth.Month && currentDate.Year == _currentMonth.Year;

                // Kiểm tra ngày có nằm trong khoảng từ ngày bắt đầu đến ngày kết thúc không
                bool isInPlanRange = planStartDate.HasValue && planEndDate.HasValue &&
                    currentDate.Date >= planStartDate.Value.Date && 
                    currentDate.Date <= planEndDate.Value.Date;
                
                bool isFuture = currentDate.Date > DateTime.Today;
                bool isSelected = _selectedDate.HasValue && currentDate.Date == _selectedDate.Value.Date;

                if (isCurrentMonth)
                {
                    if (isSelected)
                    {
                        // Ngày được chọn - highlight màu khác (màu tím)
                        btn.FillColor = Color.FromArgb(200, 190, 255); // Màu tím nhạt
                        btn.ForeColor = Color.FromArgb(100, 88, 255); // Màu tím đậm
                    }
                    else if (isInPlanRange && isFuture)
                    {
                        // Ngày trong kế hoạch và ở tương lai - highlight nhẹ (có thể chọn)
                        btn.FillColor = Color.FromArgb(233, 252, 255); // Màu xanh nhạt
                        btn.ForeColor = Color.Teal;
                    }
                    else if (isInPlanRange)
                    {
                        // Ngày trong kế hoạch nhưng không phải tương lai - màu xám (không thể chọn)
                        btn.FillColor = Color.FromArgb(240, 240, 240);
                        btn.ForeColor = Color.FromArgb(150, 150, 150);
                        btn.Enabled = false;
                    }
                    else
                    {
                        // Ngày không có trong kế hoạch - màu trắng bình thường (không thể chọn)
                        btn.FillColor = Color.White;
                        btn.ForeColor = Color.FromArgb(64, 64, 64);
                        btn.Enabled = false;
                    }
                }
                else
                {
                    // Ngày thuộc tháng trước/sau - màu xám
                    btn.FillColor = Color.FromArgb(240, 240, 240);
                    btn.ForeColor = Color.FromArgb(150, 150, 150);
                    btn.Enabled = false;
                }

                // Đăng ký event handler
                btn.Click -= DateButton_Click;
                btn.Click += DateButton_Click;

                currentDate = currentDate.AddDays(1);
            }
        }

        /// <summary>
        /// Kiểm tra ngày có nằm trong khoảng từ ngày bắt đầu đến ngày kết thúc của kế hoạch không
        /// </summary>
        private bool IsDateInPlanRange(DateTime date)
        {
            if (_currentGoal == null)
                return false;

            DateTime dateOnly = date.Date;
            DateTime? planStartDate = _currentGoal.NgayBatDau;
            DateTime? planEndDate = _currentGoal.NgayKetThucDuKien;

            if (!planStartDate.HasValue || !planEndDate.HasValue)
                return false;

            return dateOnly >= planStartDate.Value.Date && dateOnly <= planEndDate.Value.Date;
        }

        /// <summary>
        /// Xử lý khi click vào button ngày
        /// </summary>
        private void DateButton_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Guna.UI2.WinForms.Guna2Button;
                if (button == null || button.Tag == null || !button.Enabled)
                    return;

                DateTime selectedDate = (DateTime)button.Tag;

                // Chỉ cho phép chọn ngày trong tương lai và có trong khoảng kế hoạch
                if (selectedDate.Date <= DateTime.Today)
                {
                    MessageBox.Show("Chỉ có thể chọn ngày trong tương lai!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!IsDateInPlanRange(selectedDate))
                {
                    MessageBox.Show("Ngày này không nằm trong khoảng thời gian của kế hoạch!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Reset button trước đó
                if (_selectedDate.HasValue)
                {
                    UpdateButtonColor(_selectedDate.Value);
                }

                // Set ngày mới được chọn
                _selectedDate = selectedDate;
                UpdateCalendarDisplay();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DateButton_Click error: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật màu button theo trạng thái
        /// </summary>
        private void UpdateButtonColor(DateTime date)
        {
            var button = _dateButtons.FirstOrDefault(b => b.Tag != null && ((DateTime)b.Tag).Date == date.Date);
            if (button == null) return;

            bool isCurrentMonth = date.Month == _currentMonth.Month && date.Year == _currentMonth.Year;
            
            // Lấy ngày bắt đầu và kết thúc của kế hoạch
            DateTime? planStartDate = null;
            DateTime? planEndDate = null;
            if (_currentGoal != null)
            {
                planStartDate = _currentGoal.NgayBatDau;
                planEndDate = _currentGoal.NgayKetThucDuKien;
            }

            bool isInPlanRange = planStartDate.HasValue && planEndDate.HasValue &&
                date.Date >= planStartDate.Value.Date && 
                date.Date <= planEndDate.Value.Date;
            bool isFuture = date.Date > DateTime.Today;

            if (isCurrentMonth)
            {
                if (isInPlanRange && isFuture)
                {
                    button.FillColor = Color.FromArgb(233, 252, 255); // Màu xanh nhạt
                    button.ForeColor = Color.Teal;
                }
                else if (isInPlanRange)
                {
                    button.FillColor = Color.FromArgb(240, 240, 240);
                    button.ForeColor = Color.FromArgb(150, 150, 150);
                }
                else
                {
                    button.FillColor = Color.White;
                    button.ForeColor = Color.FromArgb(64, 64, 64);
                }
            }
        }

        /// <summary>
        /// Xử lý khi click nút Previous
        /// </summary>
        private void BtnPrev_Click(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            LoadCalendar();
        }

        /// <summary>
        /// Xử lý khi click nút Next
        /// </summary>
        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            LoadCalendar();
        }

        /// <summary>
        /// Xử lý khi click nút Xác nhận
        /// </summary>
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_selectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày muốn dời đến!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_buoiTapToReschedule == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin buổi tập!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra ngày đã chọn có trong kế hoạch và ở tương lai không
                if (_selectedDate.Value.Date <= DateTime.Today)
                {
                    MessageBox.Show("Chỉ có thể dời đến ngày trong tương lai!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!IsDateInPlanRange(_selectedDate.Value))
                {
                    MessageBox.Show("Ngày này không nằm trong khoảng thời gian của kế hoạch!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật ThoiGianBatDau và ThoiGianKetThuc - chỉ thay đổi ngày, giữ nguyên giờ và các thông tin khác
                using (var dbContext = new WF_HealthTracker())
                {
                    var buoiTap = dbContext.BuoiTap.FirstOrDefault(b => b.BuoiTapID == _buoiTapToReschedule.BuoiTapID);
                    if (buoiTap != null)
                    {
                        // Lấy giờ từ buổi tập hiện tại (giữ nguyên)
                        TimeSpan? gioBatDau = _buoiTapToReschedule.ThoiGianBatDau?.TimeOfDay;
                        TimeSpan? gioKetThuc = _buoiTapToReschedule.ThoiGianKetThuc?.TimeOfDay;

                        // Cập nhật ngày mới với giờ giữ nguyên
                        if (gioBatDau.HasValue)
                        {
                            buoiTap.ThoiGianBatDau = _selectedDate.Value.Date.Add(gioBatDau.Value);
                        }
                        else
                        {
                            // Nếu không có giờ, chỉ set ngày (mặc định 00:00:00)
                            buoiTap.ThoiGianBatDau = _selectedDate.Value.Date;
                        }

                        if (gioKetThuc.HasValue)
                        {
                            buoiTap.ThoiGianKetThuc = _selectedDate.Value.Date.Add(gioKetThuc.Value);
                        }
                        else
                        {
                            // Nếu không có giờ kết thúc, tính từ giờ bắt đầu + 1 giờ (hoặc giữ null)
                            if (gioBatDau.HasValue)
                            {
                                buoiTap.ThoiGianKetThuc = _selectedDate.Value.Date.Add(gioBatDau.Value).AddHours(1);
                            }
                            else
                            {
                                buoiTap.ThoiGianKetThuc = _selectedDate.Value.Date.AddHours(1);
                            }
                        }

                        // Cập nhật ngày cập nhật
                        buoiTap.NgayCapNhat = DateTime.Now;

                        // Các thông tin khác giữ nguyên (ThuNgay, TrangThai, Calories, GhiChu, etc.)
                        // Không cần thay đổi gì

                        dbContext.SaveChanges();
                    }
                }

                NewSelectedDate = _selectedDate.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi dời lịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"btnXacNhan_Click error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý khi click nút Hủy
        /// </summary>
        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _dbContext?.Dispose();
        }
    }
}
