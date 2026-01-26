using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using HealthApp.Common.Helpers;
using HealthApp.Controllers;
using HealthApp.Models;
using HealthApp.Views.Dashboard;
using Newtonsoft.Json;

namespace HealthApp.Views.PT
{
    public partial class GiaoBTChoUser : Form
    {
        private readonly PTController _ptController;
        private DatLichPT _currentBooking;
        private Dictionary<string, List<GiaoBaiTapChoUser>> _assignmentsByBooking = new Dictionary<string, List<GiaoBaiTapChoUser>>();
        private readonly Guna2ShadowPanel _bookingTemplate;

        public GiaoBTChoUser()
        {
            InitializeComponent();
            _ptController = new PTController();
            _bookingTemplate = pnLichDat;
            _bookingTemplate.Visible = false;

            if (flpBookings.Controls.Contains(pnLichDat))
            {
                flpBookings.Controls.Remove(pnLichDat);
            }

            // Gắn event handlers
            this.Load += GiaoBTChoUser_Load;
            dtpTime.ValueChanged += DtpTime_ValueChanged;
            btnHomNay.Click += BtnHomNay_Click;
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnGiaoBT.Click += BtnGiaoBT_Click;
            
            // Kết nối event cho nút quay lại
            if (btnTroVe != null)
            {
                btnTroVe.Click += BtnTroVe_Click;
            }
        }

        private void GiaoBTChoUser_Load(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi xem lịch đặt!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                // Mặc định chọn hôm nay
                dtpTime.Value = DateTime.Today;
                _ = LoadBookingsForDateAsync(DateTime.Today);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DtpTime_ValueChanged(object sender, EventArgs e)
        {
            _ = LoadBookingsForDateAsync(dtpTime.Value.Date);
        }

        private void BtnHomNay_Click(object sender, EventArgs e)
        {
            dtpTime.Value = DateTime.Today;
            _ = LoadBookingsForDateAsync(DateTime.Today);
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            var newDate = dtpTime.Value.Date.AddDays(-1);
            dtpTime.Value = newDate;
            _ = LoadBookingsForDateAsync(newDate);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            var newDate = dtpTime.Value.Date.AddDays(1);
            dtpTime.Value = newDate;
            _ = LoadBookingsForDateAsync(newDate);
        }

        /// <summary>
        /// Load các lịch đặt (DatLichPT) của PT trong ngày được chọn
        /// (qua PTController + PTService) và hiển thị lên panel thông tin.
        /// </summary>
        private async Task LoadBookingsForDateAsync(DateTime date)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    lblTenNguoiYeuCauThue.Text = "Chưa đăng nhập";
                    lblMucTieuNguoiYeuCauThue.Text = string.Empty;
                    lblThoiGian.Text = string.Empty;
                    label1.Text = "Không có dữ liệu";
                    btnGiaoBT.Enabled = false;
                    _currentBooking = null;
                    return;
                }

                // Lấy danh sách lịch đặt của PT hiện tại thông qua Controller (MVC)
                var bookings = await _ptController.GetBookingsForCurrentPTOnDateAsync(date);

                if (bookings == null || bookings.Count == 0)
                {
                    lblTenNguoiYeuCauThue.Text = "Bạn chưa có buổi tập nào trong ngày này";
                    lblMucTieuNguoiYeuCauThue.Text = string.Empty;
                    lblThoiGian.Text = string.Empty;
                    label1.Text = "Chưa có buổi tập nào trong ngày này";
                    btnGiaoBT.Enabled = false;
                    _currentBooking = null;

                    // Xóa hết card cũ và assignments khi không có lịch trong ngày
                    _assignmentsByBooking.Clear();
                    flpBookings.Controls.Clear();
                    return;
                }

                var assignments = await _ptController.GetAssignmentsForBookingsAsync(bookings);
                _assignmentsByBooking = assignments?
                    .Where(a => !string.IsNullOrWhiteSpace(a.DatLichID))
                    .GroupBy(a => a.DatLichID)
                    .ToDictionary(g => g.Key, g => g.ToList()) ?? new Dictionary<string, List<GiaoBaiTapChoUser>>();

                RenderBookings(bookings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch đặt: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGiaoBT.Enabled = false;
                _currentBooking = null;
                flpBookings.Controls.Clear();
            }
        }

        /// <summary>
        /// Mở form GiaoBaiTap để PT giao bài tập cho user theo lịch đang chọn.
        /// </summary>
        private void BtnGiaoBT_Click(object sender, EventArgs e)
        {
            if (_currentBooking == null)
            {
                MessageBox.Show("Không tìm thấy lịch đặt để giao bài tập!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            OpenAssignmentForm(_currentBooking);
        }

        private void RenderBookings(IList<DatLichPT> bookings)
        {
            flpBookings.SuspendLayout();
            flpBookings.Controls.Clear();

            foreach (var booking in bookings)
            {
                var card = CreateBookingCard(booking, GetAssignmentsForBooking(booking.DatLichID));
                flpBookings.Controls.Add(card);
            }

            flpBookings.ResumeLayout();

            _currentBooking = bookings.First();
            btnGiaoBT.Enabled = true;
        }

        private Control CreateBookingCard(DatLichPT booking, IList<GiaoBaiTapChoUser> assignments)
        {
            var panel = new Guna2ShadowPanel
            {
                Width = _bookingTemplate.Width,
                Height = _bookingTemplate.Height,
                FillColor = _bookingTemplate.FillColor,
                Radius = _bookingTemplate.Radius,
                ShadowColor = _bookingTemplate.ShadowColor,
                ShadowShift = _bookingTemplate.ShadowShift,
                Margin = new Padding(0, 0, 0, 15)
            };

            var avatar = new Guna2CirclePictureBox
            {
                Size = ptrAvatarNguoiYeuCauach1.Size,
                Location = new Point(17, 12),
                ImageRotate = 0,
                SizeMode = PictureBoxSizeMode.StretchImage,
                FillColor = ptrAvatarNguoiYeuCauach1.FillColor
            };

            var lblName = new Label
            {
                Text = booking.Users?.HoTen ?? booking.KhachHangID,
                Font = lblTenNguoiYeuCauThue.Font,
                ForeColor = lblTenNguoiYeuCauThue.ForeColor,
                Location = new Point(82, 12),
                AutoSize = true
            };

            var lblGoal = new Label
            {
                Text = DetermineGoal(booking),
                Font = lblMucTieuNguoiYeuCauThue.Font,
                ForeColor = lblMucTieuNguoiYeuCauThue.ForeColor,
                Location = new Point(83, 46),
                AutoSize = true
            };

            var lblTime = new Label
            {
                Text = $"{booking.ThoiGianBatDau:HH:mm} - {booking.ThoiGianKetThuc:HH:mm}",
                Font = lblThoiGian.Font,
                ForeColor = lblThoiGian.ForeColor,
                Location = new Point(260, 18),
                AutoSize = true
            };

            var lblStatus = new Label
            {
                Font = label1.Font,
                ForeColor = label1.ForeColor,
                Location = new Point(84, 78),
                AutoSize = true
            };
            lblStatus.Text = BuildAssignmentLabel(assignments);

            var btnAssign = new Guna2Button
            {
                Text = "Giao Bài Tập",
                Font = btnGiaoBT.Font,
                ForeColor = btnGiaoBT.ForeColor,
                FillColor = btnGiaoBT.FillColor,
                Size = btnGiaoBT.Size,
                BorderRadius = btnGiaoBT.BorderRadius,
                Location = new Point(panel.Width - btnGiaoBT.Width - 20, 31),
                Tag = booking
            };
            btnAssign.Click += BookingAssign_Click;

            panel.Controls.Add(avatar);
            panel.Controls.Add(lblName);
            panel.Controls.Add(lblGoal);
            panel.Controls.Add(lblTime);
            panel.Controls.Add(lblStatus);
            panel.Controls.Add(btnAssign);

            return panel;
        }

        private void BookingAssign_Click(object sender, EventArgs e)
        {
            if (sender is Guna2Button button && button.Tag is DatLichPT booking)
            {
                _currentBooking = booking;
                OpenAssignmentForm(booking);
            }
        }

        private void OpenAssignmentForm(DatLichPT booking)
        {
            try
            {
                using (var frm = new GiaoBaiTap(booking))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    var result = frm.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        _ = LoadBookingsForDateAsync(dtpTime.Value.Date);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở màn hình giao bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string DetermineGoal(DatLichPT booking)
        {
            if (!string.IsNullOrWhiteSpace(booking.MucTieuLuyenTap))
                return booking.MucTieuLuyenTap.Trim();

            if (!string.IsNullOrWhiteSpace(booking.GhiChu))
                return booking.GhiChu.Trim();

            if (!string.IsNullOrWhiteSpace(booking.LoaiBuoiTap))
                return booking.LoaiBuoiTap.Trim();

            return "Chưa ghi mục tiêu cụ thể";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _ptController?.Dispose();
        }

        /// <summary>
        /// Event handler cho nút quay lại - quay về Dashboard
        /// </summary>
        private void BtnTroVe_Click(object sender, EventArgs e)
        {
            try
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
                    // Ẩn form hiện tại và hiển thị lại dashboard
                    this.Hide();
                    dashboard.ShowDashboard();
                }
                else
                {
                    // Nếu không tìm thấy dashboard, chỉ đóng form hiện tại
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IList<GiaoBaiTapChoUser> GetAssignmentsForBooking(string datLichId)
        {
            if (string.IsNullOrWhiteSpace(datLichId))
                return new List<GiaoBaiTapChoUser>();

            return _assignmentsByBooking.TryGetValue(datLichId, out var list)
                ? (IList<GiaoBaiTapChoUser>)list
                : new List<GiaoBaiTapChoUser>();
        }

        private string BuildAssignmentLabel(IList<GiaoBaiTapChoUser> assignments)
        {
            if (assignments == null || assignments.Count == 0)
                return "Chưa giao bài tập";

            var names = assignments
                .Select(a => a.TieuDe)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct();

            return string.Join(", ", names);
        }

        private AssignmentCustomDetail ParseAssignmentDetail(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
                return null;

            try
            {
                return JsonConvert.DeserializeObject<AssignmentCustomDetail>(payload);
            }
            catch
            {
                return null;
            }
        }

        private class AssignmentCustomDetail
        {
            public string Equipment { get; set; }
            public string Sets { get; set; }
            public string Reps { get; set; }
            public int? RestSeconds { get; set; }
        }
    }
}
