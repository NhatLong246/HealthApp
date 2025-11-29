using Guna.UI2.WinForms;
using HealthApp.Common.Helpers;
using HealthApp.Controllers;
using HealthApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HealthApp.Views.PT
{
    public partial class frm_LichPT : Form
    {
        private readonly PTController _ptController;
        private readonly CultureInfo _culture = new CultureInfo("vi-VN");
        private readonly string[] _dayHeaders = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ Nhật" };
        private DateTime _currentWeekStart;
        private Guna2Button[,] _slotButtons;
        private bool _hasInitialized;

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

        public frm_LichPT()
        {
            InitializeComponent();

            _ptController = new PTController();
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
            Load += Frm_LichPT_Load;
            FormClosed += Frm_LichPT_FormClosed;
            btnNext.Click += async (s, e) => await ChangeWeekAsync(1);
            btnPrevious.Click += async (s, e) => await ChangeWeekAsync(-1);
            btnBack.Click += (s, e) => Close();

            foreach (var button in _slotButtons)
            {
                button.Click += SlotButton_Click;
                button.TextAlign = HorizontalAlignment.Left;
            }
        }

        private async void Frm_LichPT_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
            {
                MessageBox.Show("Vui lòng đăng nhập với tài khoản huấn luyện viên để xem lịch.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            await EnsureCurrentWeekLoadedAsync();
            if (!_hasInitialized)
            {
                await EnsureCurrentWeekLoadedAsync();
            }
        }

        private void Frm_LichPT_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ptController?.Dispose();
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

                var tasks = Enumerable.Range(0, 7)
                    .Select(offset => _ptController.GetBookingsForCurrentPTOnDateAsync(_currentWeekStart.AddDays(offset)))
                    .ToArray();

                var weeklyData = await Task.WhenAll(tasks);
                UpdateDayHeaders();
                UpdateCalendarSlots(weeklyData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch của bạn: {ex.Message}", "Lỗi",
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

            var displayLines = bookings
                .Take(2)
                .Select(b =>
                {
                    var customer = b.Users?.HoTen ?? b.KhachHangID ?? "Khách hàng";
                    var timeRange = $"{b.ThoiGianBatDau:HH:mm}-{b.ThoiGianKetThuc:HH:mm}";
                    return $"{timeRange}\n{customer}";
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

            ShowSlotDetailModal(info);
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
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
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

        private void ShowSlotDetailModal(SlotInfo info)
        {
            using (var detailForm = new Form())
            {
                detailForm.Text = $"Lịch {GetSlotName(info.Slot)} - {info.Date.ToString("dddd, dd/MM/yyyy", _culture)}";
                detailForm.StartPosition = FormStartPosition.CenterParent;
                detailForm.Size = new Size(520, 600);
                detailForm.MinimizeBox = false;
                detailForm.MaximizeBox = false;
                detailForm.FormBorderStyle = FormBorderStyle.FixedDialog;

                var flow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    Padding = new Padding(10),
                    BackColor = Color.FromArgb(245, 249, 255)
                };

                foreach (var booking in info.Bookings)
                {
                    var card = BuildBookingDetailCard(booking);
                    flow.Controls.Add(card);
                }

                    detailForm.Controls.Add(flow);

                detailForm.ShowDialog(this);
            }
        }

        private Control BuildBookingDetailCard(DatLichPT booking)
        {
            var panel = new Panel
            {
                Width = 460,
                Height = 140,
                Margin = new Padding(0, 0, 0, 12),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblCustomer = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 12F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, 10),
                Text = booking.Users?.HoTen ?? booking.KhachHangID ?? "Khách hàng"
            };

            var lblTime = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(10, 40),
                Text = $"Thời gian: {booking.ThoiGianBatDau:HH:mm} - {booking.ThoiGianKetThuc:HH:mm}"
            };

            var lblGoal = new Label
            {
                AutoSize = false,
                Font = new Font("Times New Roman", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(10, 65),
                Size = new Size(430, 40),
                Text = $"Mục tiêu: {booking.MucTieuLuyenTap ?? booking.LoaiBuoiTap ?? "Không rõ"}"
            };

            var lblStatus = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10F, FontStyle.Bold),
                ForeColor = booking.TrangThai == "Confirmed"
                    ? Color.FromArgb(0, 150, 0)
                    : Color.FromArgb(200, 120, 0),
                Location = new Point(10, 110),
                Text = $"Trạng thái: {booking.TrangThai}"
            };

            panel.Controls.Add(lblCustomer);
            panel.Controls.Add(lblTime);
            panel.Controls.Add(lblGoal);
            panel.Controls.Add(lblStatus);

            return panel;
        }
    }
}
