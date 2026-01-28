using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using HealthApp.Common.Helpers;
using HealthApp.Models;
using HealthApp.Views.Dashboard;
using HealthApp.Views.PT;

namespace HealthApp.Views.GiaoBTChoUser
{
    /// <summary>
    /// Form hiển thị lịch luyện tập của User với PT trong 1 tuần
    /// </summary>
    public partial class LichLuyenTapUser : Form
    {
        private readonly CultureInfo _culture = new CultureInfo("vi-VN");
        private readonly string[] _dayHeaders = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ Nhật" };
        private DateTime _currentWeekStart;
        private Guna2Button[,] _slotButtons;
        private bool _hasInitialized;
        private Form _parentForm;

        private enum TimeSlot
        {
            Morning = 0,
            Afternoon = 1,
            Evening = 2
        }

        private sealed class SlotInfo
        {
            public DateTime Date { get; set; }
            public TimeSlot Slot { get; set; }
            public List<DatLichPT> Bookings { get; set; } = new List<DatLichPT>();
        }

        public LichLuyenTapUser(Form parentForm = null)
        {
            InitializeComponent();
            _parentForm = parentForm;
            _currentWeekStart = GetStartOfWeek(DateTime.Today);
            InitializeSlotMatrix();
            RegisterEvents();
        }

        private void InitializeSlotMatrix()
        {
            _slotButtons = new Guna2Button[3, 7]
            {
                { btnLichPT1, btnLichPT2, btnLichPT3, btnLichPT4, btnLichPT5, btnLichPT6, btnLichPT7 },
                { btnLichPT8, btnLichPT9, btnLichPT10, btnLichPT11, btnLichPT12, btnLichPT13, btnLichPT14 },
                { btnLichPT15, btnLichPT16, btnLichPT17, btnLichPT18, btnLichPT19, btnLichPT20, btnLichPT21 }
            };
        }

        private void RegisterEvents()
        {
            Load += LichLuyenTapUser_Load;
            FormClosed += LichLuyenTapUser_FormClosed;
            btnNext.Click += async (s, e) => await ChangeWeekAsync(1);
            btnPrevious.Click += async (s, e) => await ChangeWeekAsync(-1);
            btnBack.Click += BtnBack_Click;

            foreach (var button in _slotButtons)
            {
                button.Click += SlotButton_Click;
                button.TextAlign = HorizontalAlignment.Left;
            }
        }

        private async void LichLuyenTapUser_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
            {
                MessageBox.Show("Vui lòng đăng nhập để xem lịch luyện tập!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            // Đảm bảo tính lại tuần hiện tại từ ngày hôm nay
            _currentWeekStart = GetStartOfWeek(DateTime.Today);
            await EnsureCurrentWeekLoadedAsync();
        }

        private void LichLuyenTapUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Không dispose gì ở đây vì không dùng controller
        }

        private async Task ChangeWeekAsync(int weekOffset)
        {
            _currentWeekStart = _currentWeekStart.AddDays(7 * weekOffset);
            await LoadWeekBookingsAsync();
        }

        public async Task EnsureCurrentWeekLoadedAsync()
        {
            _currentWeekStart = GetStartOfWeek(DateTime.Today);
            await LoadWeekBookingsAsync();
            _hasInitialized = true;
        }

        private async Task LoadWeekBookingsAsync()
        {
            try
            {
                ToggleNavigation(false);
                UpdateWeekHeader();

                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    ClearCalendar();
                    ToggleNavigation(true);
                    return;
                }

                var userId = CurrentUser.UserID;
                var weeklyData = new List<DatLichPT>[7];

                // Load dữ liệu cho 7 ngày trong tuần
                await Task.Run(() =>
                {
                    using (var context = new WF_HealthTracker())
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            var date = _currentWeekStart.AddDays(i);
                            var start = date.Date;
                            var end = start.AddDays(1);

                            weeklyData[i] = context.DatLichPT
                                .Include("Users")
                                .Include("HuanLuyenVien")
                                .Where(d =>
                                    d.KhachHangID == userId &&
                                    d.ThoiGianBatDau >= start &&
                                    d.ThoiGianBatDau < end &&
                                    (d.TrangThai == "Confirmed" || d.TrangThai == "Completed"))
                                .OrderBy(d => d.ThoiGianBatDau)
                                .ToList();
                        }
                    }
                });

                UpdateDayHeaders();
                UpdateCalendarSlots(weeklyData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch luyện tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearCalendar();
            }
            finally
            {
                ToggleNavigation(true);
            }
        }

        private void UpdateWeekHeader()
        {
            var end = _currentWeekStart.AddDays(6);
            lblNgayTrongTuan.Text =
                $"{_currentWeekStart.ToString("dd/MM/yyyy", _culture)} - {end.ToString("dd/MM/yyyy", _culture)}";
        }

        private void UpdateDayHeaders()
        {
            var dayButtons = new[]
            {
                btnThu2, btnThu3, btnThu4, btnThu5, btnThu6, btnThu7, btnChuNhat
            };

            for (int i = 0; i < dayButtons.Length; i++)
            {
                var date = _currentWeekStart.AddDays(i);
                dayButtons[i].Text = $"{_dayHeaders[i]}\n{date.ToString("dd/MM", _culture)}";
            }
        }

        private void UpdateCalendarSlots(IList<DatLichPT>[] weeklyData)
        {
            for (int dayIndex = 0; dayIndex < 7; dayIndex++)
            {
                var date = _currentWeekStart.AddDays(dayIndex).Date;
                var bookings = weeklyData[dayIndex] ?? new List<DatLichPT>();

                foreach (TimeSlot slot in Enum.GetValues(typeof(TimeSlot)))
                {
                    var button = _slotButtons[(int)slot, dayIndex];
                    var slotBookings = bookings
                        .Where(b => DetermineSlot(b) == slot)
                        .OrderBy(b => b.ThoiGianBatDau)
                        .ToList();

                    button.Tag = new SlotInfo
                    {
                        Date = date,
                        Slot = slot,
                        Bookings = slotBookings
                    };

                    BindSlotButton(button, slotBookings);
                }
            }
        }

        private void BindSlotButton(Guna2Button button, IList<DatLichPT> bookings)
        {
            if (bookings == null || bookings.Count == 0)
            {
                button.Text = "Trống";
                button.FillColor = Color.White;
                button.ForeColor = Color.Black;
                return;
            }

            // Hiển thị giờ và tên PT
            var displayLines = bookings
                .Take(2)
                .Select(b =>
                {
                    var timeRange = $"{b.ThoiGianBatDau:HH:mm}-{b.ThoiGianKetThuc:HH:mm}";
                    var ptName = b.HuanLuyenVien?.Users?.HoTen ?? b.PTID ?? "PT";
                    return $"{timeRange}\n{ptName}";
                })
                .ToList();

            if (bookings.Count > 2)
            {
                displayLines.Add($"+{bookings.Count - 2} buổi khác");
            }

            button.Text = string.Join("\n----------------\n", displayLines);
            button.FillColor = Color.FromArgb(215, 233, 255);
            button.ForeColor = Color.Black;
        }

        private void SlotButton_Click(object sender, EventArgs e)
        {
            if (!(sender is Guna2Button button))
                return;

            var info = button.Tag as SlotInfo;
            if (info == null)
                return;

            if (info.Bookings == null || info.Bookings.Count == 0)
            {
                MessageBox.Show(
                    $"Chưa có lịch trong khung giờ {GetSlotName(info.Slot)} ngày {info.Date.ToString("dd/MM/yyyy", _culture)}.",
                    "Thông tin lịch",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Mở BaiTapCuaPTGiao với ngày tương ứng
            OpenBaiTapCuaPTGiao(info.Date);
        }

        private void OpenBaiTapCuaPTGiao(DateTime selectedDate)
        {
            try
            {
                this.Hide();
                // Tạo form container để hiển thị UserControl BaiTapCuaPTGiao
                var containerForm = new Form
                {
                    Text = "Bài Tập PT Đã Giao",
                    StartPosition = FormStartPosition.CenterScreen,
                    Size = new Size(1400, 800),
                    AutoScroll = false
                };

                // Tạo panel chứa UserControl
                var mainPanel = new Panel
                {
                    Dock = DockStyle.Fill
                };

                var baiTapCuaPTGiao = new BaiTapCuaPTGiao(selectedDate, this);
                baiTapCuaPTGiao.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(baiTapCuaPTGiao);

                // Tạo nút quay lại
                var btnBack = new Guna2CircleButton
                {
                    Text = "←",
                    Size = new Size(50, 50),
                    Location = new Point(10, 10),
                    FillColor = Color.White,
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
                };
                btnBack.Click += (s, e) =>
                {
                    containerForm.Close();
                };

                containerForm.Controls.Add(mainPanel);
                containerForm.Controls.Add(btnBack);
                containerForm.Controls.SetChildIndex(btnBack, 0); // Đặt nút quay lại lên trên
                
                // Khi form đóng, quay lại LichLuyenTapUser
                containerForm.FormClosed += (s, args) => this.Show();
                
                containerForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở bài tập PT đã giao: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        private static TimeSlot DetermineSlot(DatLichPT booking)
        {
            if (booking == null)
                return TimeSlot.Morning;

            if (!string.IsNullOrWhiteSpace(booking.LoaiBuoiTap))
            {
                var normalized = booking.LoaiBuoiTap.ToLowerInvariant();
                if (normalized.Contains("sáng")) return TimeSlot.Morning;
                if (normalized.Contains("chiều")) return TimeSlot.Afternoon;
                if (normalized.Contains("tối")) return TimeSlot.Evening;
            }

            var hour = booking.ThoiGianBatDau.Hour;
            if (hour < 12) return TimeSlot.Morning;
            if (hour < 18) return TimeSlot.Afternoon;
            return TimeSlot.Evening;
        }

        private static DateTime GetStartOfWeek(DateTime date)
        {
            // Tính ngày bắt đầu tuần (Thứ 2)
            // Nếu là Thứ 2 (Monday = 1), diff = 0
            // Nếu là Thứ 3 (Tuesday = 2), diff = 1
            // ...
            // Nếu là Chủ nhật (Sunday = 0), diff = 6
            var dayOfWeek = (int)date.DayOfWeek;
            // Chuyển đổi: Sunday = 0 -> 7, Monday = 1 -> 1, ..., Saturday = 6 -> 6
            // Để tính từ Thứ 2 (Monday = 1) là ngày đầu tuần
            var diff = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            return date.Date.AddDays(-diff);
        }

        private static string GetSlotName(TimeSlot slot)
        {
            switch (slot)
            {
                case TimeSlot.Morning:
                    return "buổi sáng";
                case TimeSlot.Afternoon:
                    return "buổi chiều";
                case TimeSlot.Evening:
                    return "buổi tối";
                default:
                    return "khung giờ";
            }
        }

        private void ToggleNavigation(bool enabled)
        {
            btnPrevious.Enabled = enabled;
            btnNext.Enabled = enabled;
            Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }

        private void ClearCalendar()
        {
            foreach (var button in _slotButtons)
            {
                button.Text = "Trống";
                button.FillColor = Color.White;
                button.Tag = new SlotInfo { Bookings = new List<DatLichPT>() };
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                // Nếu có parent form, quay về parent
                if (_parentForm != null && !_parentForm.IsDisposed)
                {
                    this.Hide();
                    if (_parentForm is frmDashBoard1 dashboard)
                    {
                        dashboard.ShowDashboard();
                    }
                    else
                    {
                        _parentForm.Show();
                        _parentForm.BringToFront();
                    }
                }
                else
                {
                    // Tìm frmDashBoard1 trong Application.OpenForms
                    frmDashBoard1 dashboard = null;
                    foreach (Form openForm in Application.OpenForms)
                    {
                        if (openForm is frmDashBoard1)
                        {
                            dashboard = openForm as frmDashBoard1;
                            break;
                        }
                    }

                    if (dashboard != null)
                    {
                        this.Hide();
                        dashboard.ShowDashboard();
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho Paint event của guna2Panel1
        /// </summary>
        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
            // Có thể thêm custom painting logic ở đây nếu cần
            // Hiện tại để trống vì không cần custom painting
        }
    }
}
