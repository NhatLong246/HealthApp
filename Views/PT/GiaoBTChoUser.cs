using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Controllers;
using HealthApp.Models;

namespace HealthApp.Views.PT
{
    public partial class GiaoBTChoUser : Form
    {
        private readonly PTController _ptController;
        private DatLichPT _currentBooking;

        public GiaoBTChoUser()
        {
            InitializeComponent();
            _ptController = new PTController();

            // Gắn event handlers
            this.Load += GiaoBTChoUser_Load;
            dtpTime.ValueChanged += DtpTime_ValueChanged;
            btnHomNay.Click += BtnHomNay_Click;
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnGiaoBT.Click += BtnGiaoBT_Click;
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
                    lblTenNguoiYeuCauThue.Text = "Bạn chưa đăng ký PT";
                    lblMucTieuNguoiYeuCauThue.Text = string.Empty;
                    lblThoiGian.Text = string.Empty;
                    label1.Text = "Chưa có buổi tập nào trong ngày này";
                    btnGiaoBT.Enabled = false;
                    _currentBooking = null;
                    return;
                }

                // Tạm thời hiển thị lịch đầu tiên (có thể mở rộng thành danh sách sau)
                var firstBooking = bookings.First();
                _currentBooking = firstBooking;

                // Lấy thông tin user đặt lịch (chỉ dùng dữ liệu đã có trong Booking nếu thiếu repository riêng)
                lblTenNguoiYeuCauThue.Text = firstBooking.KhachHangID;

                // Mục tiêu: tạm thời lấy từ GhiChu nếu có
                if (!string.IsNullOrWhiteSpace(firstBooking.GhiChu))
                {
                    lblMucTieuNguoiYeuCauThue.Text = firstBooking.GhiChu;
                }
                else
                {
                    lblMucTieuNguoiYeuCauThue.Text = "Chưa ghi mục tiêu cụ thể";
                }

                // Thời gian buổi tập
                var batDau = firstBooking.ThoiGianBatDau;
                var ketThuc = firstBooking.ThoiGianKetThuc;
                lblThoiGian.Text = $"{batDau:HH:mm} - {ketThuc:HH:mm}";

                // Trạng thái giao bài tập
                label1.Text = "Chưa giao bài tập";

                btnGiaoBT.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch đặt: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGiaoBT.Enabled = false;
                _currentBooking = null;
            }
        }

        /// <summary>
        /// Mở form GiaoBaiTap để PT giao bài tập cho user theo lịch đang chọn.
        /// </summary>
        private void BtnGiaoBT_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentBooking == null)
                {
                    MessageBox.Show("Không tìm thấy lịch đặt để giao bài tập!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var frm = new GiaoBaiTap(_currentBooking))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở màn hình giao bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
