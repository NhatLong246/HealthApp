using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        private DatLichPT _currentSessionBooking; // Lưu buổi tập hiện tại (session đầu tiên có DatLichID)
        private List<GiaoBaiTapChoUser> _currentSessionAssignments; // Lưu danh sách bài tập của session hiện tại
        private List<GiaoBaiTapChoUser> _allAssignments; // Lưu tất cả assignments (để dùng khi không có session)

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

            // Nếu có ngày được chọn từ lịch, sử dụng đúng ngày đó (chỉ lấy phần Date)
            // Nếu không, mặc định là hôm nay
            dtpTime.Value = selectedDate?.Date ?? DateTime.Today;

            // Gắn event handlers
            this.Load += BaiTapCuaPTGiao_Load;
            dtpTime.ValueChanged += DtpTime_ValueChanged;
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnDanhGia.Click += BtnDanhGia_Click;
        }

        /// <summary>
        /// Set ngày được chọn (được gọi từ LichLuyenTapUser)
        /// </summary>
        public void SetSelectedDate(DateTime date)
        {
            if (dtpTime != null)
            {
                var onlyDate = date.Date;
                dtpTime.Value = onlyDate;
                _ = LoadAssignmentsForDateAsync(onlyDate);
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

                // dtpTime đã được set trong constructor (ngày click hoặc hôm nay)
                var selectedDate = dtpTime.Value.Date;
                _ = LoadAssignmentsForDateAsync(selectedDate);
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
                    btnDanhGia.Enabled = false;
                    return;
                }

                // Lấy assignments theo UserID (user hiện tại)
                var assignments = await _ptController.GetAssignmentsByUserAndDateAsync(date);

                // Lưu tất cả assignments để dùng cho đánh giá
                _allAssignments = assignments?.ToList() ?? new List<GiaoBaiTapChoUser>();

                if (assignments == null || assignments.Count == 0)
                {
                    flpBookings.Controls.Clear();
                    _currentSessionBooking = null;
                    _currentSessionAssignments = null;
                    btnDanhGia.Enabled = false;
                    return;
                }

                // Group assignments theo buổi (DatLichID)
                RenderAssignmentsBySession(assignments);

                // Cập nhật trạng thái nút đánh giá theo rule: lịch trình chỉ đánh giá 1 lần / buổi lẻ chỉ đánh giá 1 lần
                UpdateDanhGiaButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải bài tập đã giao: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                flpBookings.Controls.Clear();
                btnDanhGia.Enabled = false;
            }
        }

        private void UpdateDanhGiaButtonState()
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    btnDanhGia.Enabled = false;
                    return;
                }

                // Ưu tiên session đầu tiên
                var booking = _currentSessionBooking;
                var assignments = _currentSessionAssignments ?? _allAssignments;

                if (booking == null && (assignments == null || assignments.Count == 0))
                {
                    btnDanhGia.Enabled = false;
                    return;
                }

                // Resolve PTID
                string ptId = booking?.PTID;
                if (string.IsNullOrWhiteSpace(ptId))
                {
                    ptId = assignments?.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.PTID))?.PTID;
                }

                // Resolve DatLichID
                string datLichId = booking?.DatLichID;
                if (string.IsNullOrWhiteSpace(datLichId))
                {
                    datLichId = assignments?.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.DatLichID))?.DatLichID;
                }

                // Resolve LichTrinhID (nếu có)
                string lichTrinhId = booking?.LichTrinhID;
                if (string.IsNullOrWhiteSpace(lichTrinhId) && assignments != null)
                {
                    lichTrinhId = assignments.Select(a => a.DatLichPT?.LichTrinhID).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                }

                if (string.IsNullOrWhiteSpace(ptId) || string.IsNullOrWhiteSpace(datLichId))
                {
                    // Không đủ dữ liệu để khóa, nhưng vẫn cho phép mở form (frmDanhGiaPT sẽ tự chặn nếu cần)
                    btnDanhGia.Enabled = true;
                    btnDanhGia.Text = "Đánh giá";
                    return;
                }

                bool alreadyRated;
                using (var context = new WF_HealthTracker())
                {
                    if (string.IsNullOrWhiteSpace(lichTrinhId))
                    {
                        alreadyRated = context.DanhGiaPT.Any(d =>
                            d.KhachHangID == CurrentUser.UserID &&
                            d.PTID == ptId &&
                            d.DatLichID == datLichId);
                    }
                    else
                    {
                        alreadyRated = (from dg in context.DanhGiaPT
                                        join dl in context.DatLichPT on dg.DatLichID equals dl.DatLichID
                                        where dg.KhachHangID == CurrentUser.UserID
                                              && dg.PTID == ptId
                                              && dl.LichTrinhID == lichTrinhId
                                        select dg).Any();
                    }
                }

                btnDanhGia.Enabled = !alreadyRated;
                btnDanhGia.Text = alreadyRated ? "Đã đánh giá" : "Đánh giá";
            }
            catch
            {
                // fallback: vẫn cho phép click, frmDanhGiaPT sẽ chặn nếu đã đánh giá
                btnDanhGia.Enabled = true;
                btnDanhGia.Text = "Đánh giá";
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
            bool isFirstSession = true;
            foreach (var sessionGroup in groupedBySession)
            {
                var sessionAssignments = sessionGroup.ToList();
                var firstAssignment = sessionAssignments.First();
                var booking = firstAssignment.DatLichPT;

                // Đảm bảo booking có đầy đủ thông tin PT (nếu chưa có)
                if (booking != null && (booking.HuanLuyenVien == null || booking.HuanLuyenVien.Users == null))
                {
                    System.Diagnostics.Debug.WriteLine($"[RenderAssignmentsBySession] Booking thiếu thông tin PT, đang load...");
                    
                    // Nếu booking không có PT hoặc PT không có Users, thử lấy từ assignment
                    if (firstAssignment.HuanLuyenVien != null && firstAssignment.HuanLuyenVien.Users != null)
                    {
                        booking.HuanLuyenVien = firstAssignment.HuanLuyenVien;
                        System.Diagnostics.Debug.WriteLine($"[RenderAssignmentsBySession] Lấy PT từ assignment: {booking.HuanLuyenVien.PTID}, User: {booking.HuanLuyenVien.Users?.HoTen ?? "NULL"}");
                    }
                    else if (!string.IsNullOrWhiteSpace(booking.PTID))
                    {
                        // Load PT từ database nếu cần
                        System.Diagnostics.Debug.WriteLine($"[RenderAssignmentsBySession] Load PT từ DB: {booking.PTID}");
                        using (var context = new WF_HealthTracker())
                        {
                            var pt = context.HuanLuyenVien
                                .Include("Users")
                                .FirstOrDefault(h => h.PTID == booking.PTID);
                            if (pt != null)
                            {
                                booking.HuanLuyenVien = pt;
                                System.Diagnostics.Debug.WriteLine($"[RenderAssignmentsBySession] Load thành công: {pt.PTID}, User: {pt.Users?.HoTen ?? "NULL"}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"[RenderAssignmentsBySession] Load thất bại!");
                            }
                        }
                    }
                }

                // Lưu session đầu tiên để dùng cho đánh giá
                if (isFirstSession && booking != null)
                {
                    _currentSessionBooking = booking;
                    _currentSessionAssignments = sessionAssignments;
                    isFirstSession = false;
                }

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

        /// <summary>
        /// Event handler cho nút Đánh giá - mở form đánh giá PT
        /// </summary>
        private void BtnDanhGia_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi đánh giá PT!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DatLichPT booking = null;
                List<GiaoBaiTapChoUser> assignments = null;

                // Ưu tiên dùng session nếu có
                if (_currentSessionBooking != null && _currentSessionAssignments != null && _currentSessionAssignments.Count > 0)
                {
                    booking = _currentSessionBooking;
                    assignments = _currentSessionAssignments;
                    System.Diagnostics.Debug.WriteLine($"[BtnDanhGia] Dùng session: Booking.PTID={booking.PTID}, Booking.HuanLuyenVien={booking.HuanLuyenVien != null}, Assignments count={assignments.Count}");
                }
                // Nếu không có session, dùng assignments không có DatLichID hoặc tất cả assignments
                else if (_allAssignments != null && _allAssignments.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[BtnDanhGia] Không có session, dùng assignments. Count: {_allAssignments.Count}");
                    
                    // Lấy assignment đầu tiên để lấy thông tin PT
                    var firstAssignment = _allAssignments.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.PTID));
                    
                    if (firstAssignment != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[BtnDanhGia] First assignment PTID: {firstAssignment.PTID}, HuanLuyenVien={firstAssignment.HuanLuyenVien != null}");
                        
                        // Tạo một DatLichPT giả từ thông tin assignment
                        booking = CreateDummyBookingFromAssignment(firstAssignment);
                        System.Diagnostics.Debug.WriteLine($"[BtnDanhGia] Dummy booking created: PTID={booking.PTID}, HuanLuyenVien={booking.HuanLuyenVien != null}");
                        
                        // Dùng tất cả assignments hoặc chỉ những assignment cùng PT
                        assignments = _allAssignments
                            .Where(a => a.PTID == firstAssignment.PTID)
                            .ToList();
                        
                        // Nếu không có assignment nào cùng PT, dùng tất cả
                        if (assignments.Count == 0)
                        {
                            assignments = _allAssignments;
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"[BtnDanhGia] Final assignments count: {assignments.Count}");
                    }
                }

                // Nếu vẫn không có thông tin, vẫn cho phép mở form nhưng với thông tin tối thiểu
                if (booking == null || assignments == null || assignments.Count == 0)
                {
                    // Tạo booking và assignments rỗng để vẫn có thể đánh giá
                    booking = CreateEmptyBooking();
                    assignments = new List<GiaoBaiTapChoUser>();
                }

                // Đảm bảo assignments có đầy đủ thông tin PT (nếu chưa có)
                EnsureAssignmentsHavePTInfo(assignments);

                // Chặn mở form nếu đã đánh giá (lịch trình/buổi lẻ chỉ 1 lần)
                try
                {
                    string ptId = booking?.PTID ?? assignments.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.PTID))?.PTID;
                    string datLichId = booking?.DatLichID ?? assignments.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.DatLichID))?.DatLichID;
                    string lichTrinhId = booking?.LichTrinhID ?? assignments.Select(a => a.DatLichPT?.LichTrinhID).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                    if (!string.IsNullOrWhiteSpace(ptId))
                    {
                        using (var context = new WF_HealthTracker())
                        {
                            bool alreadyRated;
                            if (!string.IsNullOrWhiteSpace(lichTrinhId))
                            {
                                alreadyRated = (from dg in context.DanhGiaPT
                                                join dl in context.DatLichPT on dg.DatLichID equals dl.DatLichID
                                                where dg.KhachHangID == CurrentUser.UserID
                                                      && dg.PTID == ptId
                                                      && dl.LichTrinhID == lichTrinhId
                                                select dg).Any();
                            }
                            else if (!string.IsNullOrWhiteSpace(datLichId))
                            {
                                alreadyRated = context.DanhGiaPT.Any(d =>
                                    d.KhachHangID == CurrentUser.UserID &&
                                    d.PTID == ptId &&
                                    d.DatLichID == datLichId);
                            }
                            else
                            {
                                alreadyRated = false;
                            }

                            if (alreadyRated)
                            {
                                MessageBox.Show("Bạn đã đánh giá buổi/lịch trình này rồi. Không thể đánh giá thêm lần nữa!",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                UpdateDanhGiaButtonState();
                                return;
                            }
                        }
                    }
                }
                catch { }

                System.Diagnostics.Debug.WriteLine($"[BtnDanhGia] Mở form với: Booking.PTID={booking.PTID}, Booking.HuanLuyenVien={booking.HuanLuyenVien != null}, Assignments count={assignments.Count}");
                
                // Mở form đánh giá PT
                var frmDanhGia = new HealthApp.Views.PT.frmDanhGiaPT(booking, assignments);
                frmDanhGia.ShowDialog();

                // Sau khi đóng form đánh giá, refresh trạng thái nút
                UpdateDanhGiaButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form đánh giá: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tạo DatLichPT giả từ thông tin assignment (khi không có session)
        /// </summary>
        private DatLichPT CreateDummyBookingFromAssignment(GiaoBaiTapChoUser assignment)
        {
            // Ưu tiên lấy PT từ assignment nếu đã được load
            HuanLuyenVien pt = null;
            
            if (assignment.HuanLuyenVien != null && assignment.HuanLuyenVien.Users != null)
            {
                // PT đã được eager load trong assignment và có Users
                pt = assignment.HuanLuyenVien;
                System.Diagnostics.Debug.WriteLine($"[CreateDummyBooking] Lấy PT từ assignment.HuanLuyenVien: {pt.PTID}, User: {pt.Users?.HoTen ?? "NULL"}");
            }
            else if (!string.IsNullOrWhiteSpace(assignment.PTID))
            {
                // Load PT từ database nếu chưa có hoặc không có Users
                System.Diagnostics.Debug.WriteLine($"[CreateDummyBooking] Load PT từ DB: {assignment.PTID}");
                using (var context = new WF_HealthTracker())
                {
                    pt = context.HuanLuyenVien
                        .Include("Users")
                        .FirstOrDefault(h => h.PTID == assignment.PTID);
                    
                    if (pt != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CreateDummyBooking] Load thành công: {pt.PTID}, User: {pt.Users?.HoTen ?? "NULL"}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[CreateDummyBooking] Load thất bại!");
                    }
                }
            }

            if (pt == null)
            {
                System.Diagnostics.Debug.WriteLine("[CreateDummyBooking] PT null, tạo empty booking");
                return CreateEmptyBooking();
            }

            // Đảm bảo PT có Users (nếu chưa có, load lại)
            if (pt.Users == null && !string.IsNullOrWhiteSpace(pt.UserID))
            {
                System.Diagnostics.Debug.WriteLine($"[CreateDummyBooking] PT không có Users, load lại UserID: {pt.UserID}");
                using (var context = new WF_HealthTracker())
                {
                    pt.Users = context.Users.FirstOrDefault(u => u.UserID == pt.UserID);
                }
            }

            // Tạo DatLichPT giả với thông tin tối thiểu
            var dummyBooking = new DatLichPT
            {
                DatLichID = assignment.DatLichID ?? "DUMMY_" + Guid.NewGuid().ToString().Substring(0, 8),
                PTID = assignment.PTID,
                KhachHangID = assignment.UserID ?? CurrentUser.UserID,
                ThoiGianBatDau = dtpTime.Value.Date.AddHours(9), // Mặc định 9h sáng
                ThoiGianKetThuc = dtpTime.Value.Date.AddHours(10), // Mặc định 10h sáng
                TrangThai = "Completed", // Đánh dấu là đã hoàn thành để có thể đánh giá
                HuanLuyenVien = pt // Gán PT đã load (có Users)
            };
            
            System.Diagnostics.Debug.WriteLine($"[CreateDummyBooking] Tạo booking thành công: PTID={dummyBooking.PTID}, HuanLuyenVien={dummyBooking.HuanLuyenVien != null}, Users={dummyBooking.HuanLuyenVien?.Users != null}");
            return dummyBooking;
        }

        /// <summary>
        /// Đảm bảo assignments có đầy đủ thông tin PT
        /// </summary>
        private void EnsureAssignmentsHavePTInfo(List<GiaoBaiTapChoUser> assignments)
        {
            if (assignments == null || assignments.Count == 0) return;

            foreach (var assignment in assignments)
            {
                if (string.IsNullOrWhiteSpace(assignment.PTID)) continue;

                // Nếu assignment không có HuanLuyenVien hoặc HuanLuyenVien không có Users
                if (assignment.HuanLuyenVien == null || assignment.HuanLuyenVien.Users == null)
                {
                    using (var context = new WF_HealthTracker())
                    {
                        var pt = context.HuanLuyenVien
                            .Include("Users")
                            .FirstOrDefault(h => h.PTID == assignment.PTID);
                        
                        if (pt != null)
                        {
                            assignment.HuanLuyenVien = pt;
                            System.Diagnostics.Debug.WriteLine($"[EnsureAssignmentsHavePTInfo] Load PT cho assignment: {assignment.PTID}, User: {pt.Users?.HoTen ?? "NULL"}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tạo DatLichPT rỗng khi không có thông tin gì
        /// </summary>
        private DatLichPT CreateEmptyBooking()
        {
            return new DatLichPT
            {
                DatLichID = "EMPTY_" + Guid.NewGuid().ToString().Substring(0, 8),
                PTID = null,
                KhachHangID = CurrentUser.UserID,
                ThoiGianBatDau = dtpTime.Value.Date.AddHours(9),
                ThoiGianKetThuc = dtpTime.Value.Date.AddHours(10),
                TrangThai = "Completed"
            };
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
