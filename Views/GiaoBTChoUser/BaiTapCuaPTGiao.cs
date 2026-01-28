using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using HealthApp.Common.Helpers;
using HealthApp.Controllers;
using HealthApp.Models;
using Newtonsoft.Json;

namespace HealthApp.Views.GiaoBTChoUser
{
    public partial class BaiTapCuaPTGiao : UserControl
    {
        private readonly PTController _ptController;
        private readonly Guna2ShadowPanel _assignmentTemplate;
        private LichLuyenTapUser _parentLichForm;

        public BaiTapCuaPTGiao(DateTime? selectedDate = null, LichLuyenTapUser parentLichForm = null)
        {
            InitializeComponent();
            _ptController = new PTController();
            _parentLichForm = parentLichForm;
            _assignmentTemplate = pnLichDat;
            _assignmentTemplate.Visible = false;

            if (flpBookings.Controls.Contains(pnLichDat))
            {
                flpBookings.Controls.Remove(pnLichDat);
            }

            // Nếu có ngày được chọn, set vào dtpTime
            if (selectedDate.HasValue)
            {
                dtpTime.Value = selectedDate.Value;
            }

            // Gắn event handlers
            this.Load += BaiTapCuaPTGiao_Load;
            dtpTime.ValueChanged += DtpTime_ValueChanged;
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
        }

        /// <summary>
        /// Set ngày được chọn (được gọi từ LichLuyenTapUser)
        /// </summary>
        public void SetSelectedDate(DateTime date)
        {
            if (dtpTime != null)
            {
                dtpTime.Value = date;
                _ = LoadAssignmentsForDateAsync(date);
            }
        }

        private void BaiTapCuaPTGiao_Load(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi xem bài tập đã giao!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Nếu dtpTime đã được set từ constructor, load dữ liệu cho ngày đó
                // Nếu không, mặc định chọn hôm nay
                if (dtpTime.Value == DateTime.MinValue || dtpTime.Value.Date == new DateTime(2025, 11, 26).Date)
                {
                    dtpTime.Value = DateTime.Today;
                }
                _ = LoadAssignmentsForDateAsync(dtpTime.Value.Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DtpTime_ValueChanged(object sender, EventArgs e)
        {
            _ = LoadAssignmentsForDateAsync(dtpTime.Value.Date);
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            var newDate = dtpTime.Value.Date.AddDays(-1);
            dtpTime.Value = newDate;
            _ = LoadAssignmentsForDateAsync(newDate);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            var newDate = dtpTime.Value.Date.AddDays(1);
            dtpTime.Value = newDate;
            _ = LoadAssignmentsForDateAsync(newDate);
        }

        private async Task LoadAssignmentsForDateAsync(DateTime date)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    flpBookings.Controls.Clear();
                    return;
                }

                // Lấy assignments theo UserID (user hiện tại)
                var assignments = await _ptController.GetAssignmentsByUserAndDateAsync(date);

                if (assignments == null || assignments.Count == 0)
                {
                    flpBookings.Controls.Clear();
                    return;
                }

                // Group assignments theo buổi (DatLichID)
                RenderAssignmentsBySession(assignments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải bài tập đã giao: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                flpBookings.Controls.Clear();
            }
        }

        private void RenderAssignmentsBySession(IList<GiaoBaiTapChoUser> assignments)
        {
            flpBookings.SuspendLayout();
            flpBookings.Controls.Clear();

            // Group theo DatLichID (buổi tập)
            var groupedBySession = assignments
                .Where(a => !string.IsNullOrWhiteSpace(a.DatLichID))
                .GroupBy(a => a.DatLichID)
                .ToList();

            // Hiển thị assignments không có DatLichID trước
            var noSessionAssignments = assignments
                .Where(a => string.IsNullOrWhiteSpace(a.DatLichID))
                .ToList();

            foreach (var assignment in noSessionAssignments)
            {
                var card = CreateAssignmentCard(assignment, null);
                flpBookings.Controls.Add(card);
            }

            // Hiển thị từng buổi với các bài tập trong buổi đó
            foreach (var sessionGroup in groupedBySession)
            {
                var sessionAssignments = sessionGroup.ToList();
                var firstAssignment = sessionAssignments.First();
                var booking = firstAssignment.DatLichPT;

                // Tạo card cho buổi tập
                var sessionCard = CreateSessionCard(booking, sessionAssignments);
                flpBookings.Controls.Add(sessionCard);
            }

            flpBookings.ResumeLayout();
        }

        private Control CreateAssignmentCard(GiaoBaiTapChoUser assignment, DatLichPT _)
        {
            var panel = new Guna2ShadowPanel
            {
                Width = _assignmentTemplate.Width,
                Height = 100, // Chiều cao nhỏ hơn cho bài tập đơn lẻ
                FillColor = _assignmentTemplate.FillColor,
                Radius = _assignmentTemplate.Radius,
                ShadowColor = _assignmentTemplate.ShadowColor,
                ShadowShift = _assignmentTemplate.ShadowShift,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Parse custom details từ GhiChuPT
            var customDetail = ParseAssignmentDetail(assignment.GhiChuPT);

            // Tên bài tập
            var lblTenBT = new Label
            {
                Text = assignment.TieuDe ?? "Bài tập",
                Font = new Font("Times New Roman", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(17, 12),
                AutoSize = true
            };

            // Thông tin chi tiết
            var detailText = BuildDetailText(assignment, customDetail);
            var lblDetail = new Label
            {
                Text = detailText,
                Font = new Font("Times New Roman", 9F, FontStyle.Regular),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(17, 35),
                Size = new Size(500, 50),
                AutoSize = false
            };

            // Trạng thái
            var statusText = GetStatusText(assignment.TrangThai);
            var lblStatus = new Label
            {
                Text = statusText,
                Font = new Font("Times New Roman", 10F, FontStyle.Bold),
                ForeColor = GetStatusColor(assignment.TrangThai),
                Location = new Point(650, 12),
                AutoSize = true
            };

            panel.Controls.Add(lblTenBT);
            panel.Controls.Add(lblDetail);
            panel.Controls.Add(lblStatus);

            return panel;
        }

        private Control CreateSessionCard(DatLichPT booking, IList<GiaoBaiTapChoUser> assignments)
        {
            var panel = new Guna2ShadowPanel
            {
                Width = _assignmentTemplate.Width,
                Height = 150 + (assignments.Count * 110), // Chiều cao động theo số bài tập
                FillColor = _assignmentTemplate.FillColor,
                Radius = _assignmentTemplate.Radius,
                ShadowColor = _assignmentTemplate.ShadowColor,
                ShadowShift = _assignmentTemplate.ShadowShift,
                Margin = new Padding(0, 0, 0, 15)
            };

            // Thông tin buổi tập
            var ptName = booking?.HuanLuyenVien?.Users?.HoTen ?? "PT";
            var lblSessionTitle = new Label
            {
                Text = $"Buổi tập với {ptName}",
                Font = new Font("Times New Roman", 13.2F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(17, 12),
                AutoSize = true
            };

            // Thời gian buổi tập
            string timeText = "Chưa xác định";
            if (booking != null)
            {
                timeText = $"{booking.ThoiGianBatDau:HH:mm} - {booking.ThoiGianKetThuc:HH:mm}";
            }
            var lblTime = new Label
            {
                Text = $"Thời gian: {timeText}",
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(17, 40),
                AutoSize = true
            };

            // Mục tiêu buổi tập
            var mucTieu = booking?.MucTieuLuyenTap ?? assignments.FirstOrDefault()?.MucTieuBuoiTap ?? "Không xác định";
            var lblMucTieu = new Label
            {
                Text = $"Mục tiêu: {mucTieu}",
                Font = new Font("Times New Roman", 9F, FontStyle.Regular),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(17, 60),
                AutoSize = true
            };

            // Danh sách bài tập
            var lblBaiTap = new Label
            {
                Text = $"Bài tập đã giao ({assignments.Count}):",
                Font = new Font("Times New Roman", 10F, FontStyle.Bold),
                ForeColor = Color.Orange,
                Location = new Point(17, 85),
                AutoSize = true
            };

            panel.Controls.Add(lblSessionTitle);
            panel.Controls.Add(lblTime);
            panel.Controls.Add(lblMucTieu);
            panel.Controls.Add(lblBaiTap);

            // Thêm các bài tập vào panel
            int yOffset = 110;
            foreach (var assignment in assignments)
            {
                var exerciseCard = CreateExerciseItemCard(assignment, yOffset);
                panel.Controls.Add(exerciseCard);
                yOffset += 100;
            }

            return panel;
        }

        private Control CreateExerciseItemCard(GiaoBaiTapChoUser assignment, int yOffset)
        {
            var panel = new Panel
            {
                BackColor = Color.FromArgb(40, 55, 74),
                Location = new Point(20, yOffset),
                Size = new Size(860, 95),
                BorderStyle = BorderStyle.None
            };

            // Parse custom details
            var customDetail = ParseAssignmentDetail(assignment.GhiChuPT);

            // Tên bài tập
            var lblTenBT = new Label
            {
                Text = $"• {assignment.TieuDe ?? "Bài tập"}",
                Font = new Font("Times New Roman", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 8),
                AutoSize = true
            };

            // Chi tiết
            var detailText = BuildDetailText(assignment, customDetail);
            var lblDetail = new Label
            {
                Text = detailText,
                Font = new Font("Times New Roman", 9F, FontStyle.Regular),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(10, 30),
                Size = new Size(600, 50),
                AutoSize = false
            };

            // Trạng thái
            var statusText = GetStatusText(assignment.TrangThai);
            var lblStatus = new Label
            {
                Text = statusText,
                Font = new Font("Times New Roman", 9F, FontStyle.Bold),
                ForeColor = GetStatusColor(assignment.TrangThai),
                Location = new Point(750, 8),
                AutoSize = true
            };

            panel.Controls.Add(lblTenBT);
            panel.Controls.Add(lblDetail);
            panel.Controls.Add(lblStatus);

            return panel;
        }

        private string BuildDetailText(GiaoBaiTapChoUser assignment, AssignmentCustomDetail customDetail)
        {
            var parts = new List<string>();

            if (customDetail != null)
            {
                if (!string.IsNullOrWhiteSpace(customDetail.Equipment))
                    parts.Add($"Dụng cụ: {customDetail.Equipment}");
                if (!string.IsNullOrWhiteSpace(customDetail.Sets))
                    parts.Add($"Set: {customDetail.Sets}");
                if (!string.IsNullOrWhiteSpace(customDetail.Reps))
                    parts.Add($"Rep: {customDetail.Reps}");
                if (customDetail.RestSeconds.HasValue)
                    parts.Add($"Nghỉ: {customDetail.RestSeconds}s");
            }

            if (!string.IsNullOrWhiteSpace(assignment.MoTa))
                parts.Add($"Mô tả: {assignment.MoTa}");

            return parts.Count > 0 ? string.Join(" | ", parts) : "Không có thông tin chi tiết";
        }

        private string GetStatusText(string status)
        {
            switch (status?.ToLower())
            {
                case "assigned":
                    return "Đã giao";
                case "inprogress":
                    return "Đang thực hiện";
                case "completed":
                    return "Hoàn thành";
                case "overdue":
                    return "Quá hạn";
                default:
                    return status ?? "Chưa xác định";
            }
        }

        private Color GetStatusColor(string status)
        {
            switch (status?.ToLower())
            {
                case "assigned":
                    return Color.Orange;
                case "inprogress":
                    return Color.Blue;
                case "completed":
                    return Color.Green;
                case "overdue":
                    return Color.Red;
                default:
                    return SystemColors.ActiveBorder;
            }
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ptController?.Dispose();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
