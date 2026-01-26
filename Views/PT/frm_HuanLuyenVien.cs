using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Models;
using HealthApp.Services;
using HealthApp.Services.Interfaces;
using HealthApp.Common.Helpers;
using HealthApp.Views.Dashboard;
using Guna.UI2.WinForms;
using System.Drawing.Imaging;

namespace HealthApp.Views.PT
{
    public partial class frm_HuanLuyenVien : Form
    {
        private readonly IPTDashboardService _ptDashboardService;
        private readonly WF_HealthTracker _context;
        private readonly HealthApp.Views.Dashboard.frmDashBoard1 _parentDashboard;
        private string _ptId;
        private Timer _refreshTimer;

        public frm_HuanLuyenVien(HealthApp.Views.Dashboard.frmDashBoard1 parentDashboard = null)
        {
            InitializeComponent();
            _context = new WF_HealthTracker();
            _ptDashboardService = new PTDashboardService(_context);
            _parentDashboard = parentDashboard;
            InitializeEventHandlers();
            LoadData();
            StartAutoRefresh();

            // Đảm bảo panel thao tác nhanh hiển thị trên cùng
            pnThaoTacNhanh.BringToFront();
        }

        private void InitializeEventHandlers()
        {
            btnBack.Click += BtnBack_Click;
            btnDongY.Click += BtnDongY_Click;
            btnXoa.Click += BtnXoa_Click;
            btnGiaoBT.Click += BtnGiaoBT_Click;
            btnLichPT.Click += BtnLichPT_Click;
        }

        private async void LoadData()
        {
            try
            {
                // Kiểm tra đăng nhập
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                // Lấy PTID
                _ptId = await _ptDashboardService.GetPTIDByUserIDAsync(CurrentUser.User.UserID);
                if (string.IsNullOrEmpty(_ptId))
                {
                    MessageBox.Show("Không tìm thấy thông tin PT!", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Load thống kê
                await LoadStatistics();

                // Load yêu cầu thuê PT
                await LoadPTRequests();

                // Load khách hàng đang tập
                await LoadActiveCustomers();

                // Load lịch trình hôm nay
                await LoadTodaySchedule();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadStatistics()
        {
            try
            {
                var totalCustomers = await _ptDashboardService.GetTotalCustomersAsync(_ptId);
                var todaySessions = await _ptDashboardService.GetTodaySessionsAsync(_ptId);
                var monthlyIncome = await _ptDashboardService.GetMonthlyIncomeAsync(_ptId);
                var avgRating = await _ptDashboardService.GetAverageRatingAsync(_ptId);

                lblKhachHang.Text = totalCustomers.ToString();
                lblBuoiTap.Text = todaySessions.ToString();
                lblThuNhap.Text = monthlyIncome.ToString("N0") + " VNĐ";
                lblDanhGia.Text = avgRating > 0 ? avgRating.ToString("F1") : "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thống kê: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPTRequests()
        {
            try
            {
                // Xóa các panel cũ (trừ panel mẫu)
                var panelsToRemove = pnlYeuCauThuePT.Controls
                    .OfType<Guna2ShadowPanel>()
                    .Where(p => p != guna2ShadowPanel1)
                    .ToList();
                var buttonsToRemove = pnlYeuCauThuePT.Controls
                    .OfType<Guna2CircleButton>()
                    .Where(b => b != btnDongY && b != btnXoa)
                    .ToList();

                foreach (var panel in panelsToRemove)
                {
                    pnlYeuCauThuePT.Controls.Remove(panel);
                    panel.Dispose();
                }
                foreach (var button in buttonsToRemove)
                {
                    pnlYeuCauThuePT.Controls.Remove(button);
                    button.Dispose();
                }

                // Ẩn panel mẫu
                guna2ShadowPanel1.Visible = false;
                btnDongY.Visible = false;
                btnXoa.Visible = false;

                var requests = await _ptDashboardService.GetPTRequestsAsync(_ptId);

                if (requests.Count == 0)
                {
                    // Không có yêu cầu
                    return;
                }

                int yOffset = 69; // Vị trí Y ban đầu
                int panelHeight = 76;
                int spacing = 10;

                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];

                    // Tạo panel mới
                    var panel = CreateRequestPanel(request, yOffset);
                    pnlYeuCauThuePT.Controls.Add(panel);

                    // Tạo nút đồng ý (vị trí Y = yOffset + 5 để căn giữa với panel)
                    var btnAccept = CreateAcceptButton(request.DatLichID, yOffset + 5);
                    pnlYeuCauThuePT.Controls.Add(btnAccept);

                    // Tạo nút xóa (vị trí Y = yOffset + 46 để căn giữa với panel)
                    var btnReject = CreateRejectButton(request.DatLichID, yOffset + 46);
                    pnlYeuCauThuePT.Controls.Add(btnReject);

                    yOffset += panelHeight + spacing;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load yêu cầu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Guna2ShadowPanel CreateRequestPanel(PTRequestViewModel request, int yPos)
        {
            var panel = new Guna2ShadowPanel
            {
                BackColor = Color.Transparent,
                FillColor = Color.Honeydew,
                Location = new Point(21, yPos),
                Name = $"pnlRequest_{request.DatLichID}",
                Radius = 10,
                ShadowColor = Color.FromArgb(0, 192, 0),
                ShadowShift = 1,
                Size = new Size(329, 76)
            };

            // Avatar
            var avatar = new Guna2CirclePictureBox
            {
                ImageRotate = 0F,
                Location = new Point(17, 12),
                Name = $"ptrAvatar_{request.DatLichID}",
                ShadowDecoration = { Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle },
                Size = new Size(60, 53),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            LoadAvatar(avatar, request.AnhDaiDien);
            panel.Controls.Add(avatar);

            // Tên khách hàng
            var lblTen = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 13.2F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(82, 12),
                Name = $"lblTen_{request.DatLichID}",
                Size = new Size(148, 25),
                Text = request.TenKhachHang
            };
            panel.Controls.Add(lblTen);

            // Mục tiêu
            var lblMucTieu = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(83, 46),
                Name = $"lblMucTieu_{request.DatLichID}",
                Size = new Size(80, 19),
                Text = request.MucTieu
            };
            panel.Controls.Add(lblMucTieu);

            // Thời gian
            var lblThoiGian = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 7.8F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(241, 20),
                Name = $"lblThoiGian_{request.DatLichID}",
                Size = new Size(81, 15),
                Text = request.ThoiGian
            };
            panel.Controls.Add(lblThoiGian);

            // Ngày tập
            var lblNgay = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(231, 46),
                Name = $"lblNgay_{request.DatLichID}",
                Size = new Size(91, 19),
                Text = request.NgayGioDat.ToString("dd/MM/yyyy")
            };
            panel.Controls.Add(lblNgay);

            return panel;
        }

        private Guna2CircleButton CreateAcceptButton(string datLichID, int yPos)
        {
            var button = new Guna2CircleButton
            {
                BackColor = Color.Transparent,
                DisabledState = {
                    BorderColor = Color.DarkGray,
                    CustomBorderColor = Color.DarkGray,
                    FillColor = Color.FromArgb(169, 169, 169),
                    ForeColor = Color.FromArgb(141, 141, 141)
                },
                FillColor = Color.Lime,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                Location = new Point(356, yPos),
                Name = $"btnAccept_{datLichID}",
                ShadowDecoration = { Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle },
                Size = new Size(41, 37),
                Tag = datLichID
            };
            
            // Copy Image từ nút mẫu nếu có
            if (btnDongY != null && btnDongY.Image != null)
            {
                button.Image = (Image)btnDongY.Image.Clone();
                button.ImageSize = btnDongY.ImageSize;
            }
            
            button.Click += BtnAcceptRequest_Click;
            return button;
        }

        private Guna2CircleButton CreateRejectButton(string datLichID, int yPos)
        {
            var button = new Guna2CircleButton
            {
                BackColor = Color.Transparent,
                DisabledState = {
                    BorderColor = Color.DarkGray,
                    CustomBorderColor = Color.DarkGray,
                    FillColor = Color.FromArgb(169, 169, 169),
                    ForeColor = Color.FromArgb(141, 141, 141)
                },
                FillColor = Color.FromArgb(192, 0, 0),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                Location = new Point(356, yPos),
                Name = $"btnReject_{datLichID}",
                ShadowDecoration = { Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle },
                Size = new Size(41, 37),
                Tag = datLichID
            };
            
            // Copy Image từ nút mẫu nếu có
            if (btnXoa != null && btnXoa.Image != null)
            {
                button.Image = (Image)btnXoa.Image.Clone();
                button.ImageSize = btnXoa.ImageSize;
            }
            
            button.Click += BtnRejectRequest_Click;
            return button;
        }

        private async Task LoadActiveCustomers()
        {
            try
            {
                // Xóa các panel cũ (trừ panel mẫu)
                var panelsToRemove = pnlDanhsach.Controls
                    .OfType<Guna2ShadowPanel>()
                    .Where(p => p != pnlDanhSach1)
                    .ToList();
                var buttonsToRemove = pnlDanhsach.Controls
                    .OfType<Guna2Button>()
                    .ToList();

                foreach (var panel in panelsToRemove)
                {
                    pnlDanhsach.Controls.Remove(panel);
                    panel.Dispose();
                }
                foreach (var button in buttonsToRemove)
                {
                    pnlDanhsach.Controls.Remove(button);
                    button.Dispose();
                }

                // Ẩn panel mẫu
                pnlDanhSach1.Visible = false;

                var customers = await _ptDashboardService.GetActiveCustomersAsync(_ptId);

                if (customers.Count == 0)
                {
                    return;
                }

                int yOffset = 14;
                int panelHeight = 88;
                int spacing = 10;

                for (int i = 0; i < customers.Count; i++)
                {
                    var customer = customers[i];

                    // Tạo panel mới
                    var panel = CreateCustomerPanel(customer, yOffset);
                    pnlDanhsach.Controls.Add(panel);

                    yOffset += panelHeight + spacing;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load khách hàng: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Guna2ShadowPanel CreateCustomerPanel(PTCustomerViewModel customer, int yPos)
        {
            var panel = new Guna2ShadowPanel
            {
                BackColor = Color.Transparent,
                FillColor = Color.Honeydew,
                Location = new Point(6, yPos),
                Name = $"pnlCustomer_{customer.DatLichID}",
                Radius = 10,
                ShadowColor = Color.FromArgb(0, 192, 0),
                ShadowShift = 1,
                Size = new Size(625, 88)
            };

            // Avatar
            var avatar = new Guna2CirclePictureBox
            {
                ImageRotate = 0F,
                Location = new Point(15, 12),
                Name = $"ptrAvatar_{customer.DatLichID}",
                ShadowDecoration = { Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle },
                Size = new Size(64, 64),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            LoadAvatar(avatar, customer.AnhDaiDien);
            panel.Controls.Add(avatar);

            // Tên khách hàng
            var lblTen = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 13.2F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(94, 23),
                Name = $"lblTen_{customer.DatLichID}",
                Size = new Size(147, 25),
                Text = customer.TenKhachHang
            };
            panel.Controls.Add(lblTen);

            // Ngày
            var lblNgay = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(95, 57),
                Name = $"lblNgay_{customer.DatLichID}",
                Size = new Size(91, 19),
                Text = customer.NgayGioDat.ToString("dd/MM/yyyy")
            };
            panel.Controls.Add(lblNgay);

            // Thời gian
            var lblThoiGian = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(217, 57),
                Name = $"lblThoiGian_{customer.DatLichID}",
                Size = new Size(107, 19),
                Text = customer.ThoiGian
            };
            panel.Controls.Add(lblThoiGian);

            return panel;
        }


        private async Task LoadTodaySchedule()
        {
            try
            {
                // Xóa các panel cũ (trừ panel mẫu)
                var panelsToRemove = pnlLichTrinh.Controls
                    .OfType<Guna2ShadowPanel>()
                    .Where(p => p != pnlLichTrinh1)
                    .ToList();

                foreach (var panel in panelsToRemove)
                {
                    pnlLichTrinh.Controls.Remove(panel);
                    panel.Dispose();
                }

                // Ẩn panel mẫu
                pnlLichTrinh1.Visible = false;

                var schedules = await _ptDashboardService.GetTodayScheduleAsync(_ptId);

                if (schedules.Count == 0)
                {
                    return;
                }

                int yOffset = 61;
                int panelHeight = 80;
                int spacing = 10;

                for (int i = 0; i < schedules.Count; i++)
                {
                    var schedule = schedules[i];

                    // Tạo panel mới
                    var panel = CreateSchedulePanel(schedule, yOffset);
                    pnlLichTrinh.Controls.Add(panel);

                    yOffset += panelHeight + spacing;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load lịch trình: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Guna2ShadowPanel CreateSchedulePanel(PTScheduleViewModel schedule, int yPos)
        {
            var panel = new Guna2ShadowPanel
            {
                BackColor = Color.Transparent,
                FillColor = Color.MintCream,
                Location = new Point(31, yPos),
                Name = $"pnlSchedule_{schedule.DatLichID}",
                Radius = 10,
                ShadowColor = Color.Silver,
                ShadowShift = 1,
                Size = new Size(790, 80)
            };

            // Icon
            var icon = new Guna2PictureBox
            {
                BackColor = Color.LightCyan,
                Image = ptrImageLichTrinh.Image,
                ImageRotate = 0F,
                Location = new Point(14, 17),
                Name = $"ptrIcon_{schedule.DatLichID}",
                Size = new Size(51, 45),
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            panel.Controls.Add(icon);

            // Thời gian bắt đầu
            var lblTime1 = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                Location = new Point(71, 17),
                Name = $"lblTime1_{schedule.DatLichID}",
                Size = new Size(42, 19),
                Text = schedule.ThoiGianBatDau.ToString("HH:mm")
            };
            panel.Controls.Add(lblTime1);

            // Dấu gạch ngang
            var lblTime = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                Location = new Point(108, 17),
                Name = $"lblTime_{schedule.DatLichID}",
                Size = new Size(15, 19),
                Text = "-"
            };
            panel.Controls.Add(lblTime);

            // Thời gian kết thúc
            var lblTime2 = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                Location = new Point(119, 17),
                Name = $"lblTime2_{schedule.DatLichID}",
                Size = new Size(42, 19),
                Text = schedule.ThoiGianKetThuc.ToString("HH:mm")
            };
            panel.Controls.Add(lblTime2);

            // Tên khách hàng và loại buổi tập
            var lblLich = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = Color.Silver,
                Location = new Point(71, 43),
                Name = $"lblLich_{schedule.DatLichID}",
                Size = new Size(175, 19),
                Text = $"{schedule.TenKhachHang} - {schedule.LoaiBuoiTap}"
            };
            panel.Controls.Add(lblLich);

            // Trạng thái
            var isOngoing = DateTime.Now >= schedule.ThoiGianBatDau && 
                           DateTime.Now <= schedule.ThoiGianKetThuc;
            var lblDienRa = new Label
            {
                BackColor = isOngoing ? Color.FromArgb(192, 255, 192) : Color.Transparent,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = isOngoing ? Color.FromArgb(0, 192, 0) : Color.Transparent,
                Location = new Point(645, 46),
                MaximumSize = new Size(300, 100),
                Name = $"lblDienRa_{schedule.DatLichID}",
                Size = new Size(130, 23),
                Text = isOngoing ? "Đang diễn ra" : "",
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblDienRa);

            return panel;
        }

        private void LoadAvatar(Guna2CirclePictureBox pictureBox, string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    pictureBox.Image = null;
                    return;
                }

                string fullPath = imagePath;

                // Nếu là đường dẫn relative (bắt đầu với PTDocuments hoặc Resources)
                if (!Path.IsPathRooted(imagePath))
                {
                    var appDirectory = Application.StartupPath;
                    // Thử nhiều đường dẫn có thể
                    var possiblePaths = new[]
                    {
                        Path.Combine(appDirectory, "Resources", imagePath),
                        Path.Combine(appDirectory, imagePath),
                        Path.Combine(appDirectory, "Resources", "PTDocuments", imagePath)
                    };

                    foreach (var path in possiblePaths)
                    {
                        if (File.Exists(path))
                        {
                            fullPath = path;
                            break;
                        }
                    }
                }

                if (File.Exists(fullPath))
                {
                    pictureBox.Image = Image.FromFile(fullPath);
                }
                else
                {
                    // Thử đường dẫn trực tiếp nếu không tìm thấy
                    if (File.Exists(imagePath))
                    {
                        pictureBox.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        pictureBox.Image = null;
                    }
                }
            }
            catch
            {
                pictureBox.Image = null;
            }
        }

        private void StartAutoRefresh()
        {
            _refreshTimer = new Timer
            {
                Interval = 60000 // Refresh mỗi 60 giây
            };
            _refreshTimer.Tick += async (s, e) =>
            {
                await LoadStatistics();
                await LoadTodaySchedule();
            };
            _refreshTimer.Start();
        }

        private async void BtnAcceptRequest_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Guna2CircleButton;
                if (button?.Tag == null) return;

                string datLichID = button.Tag.ToString();
                
                // Kiểm tra yêu cầu còn tồn tại và chưa được chấp nhận
                var datLich = await Task.Run(() => _context.DatLichPT.FirstOrDefault(d => d.DatLichID == datLichID));
                if (datLich == null || datLich.TrangThai != "Pending")
                {
                    MessageBox.Show("Yêu cầu này đã được xử lý!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await LoadPTRequests();
                    return;
                }

                // Kiểm tra trùng lịch với các lịch đã được xác nhận của PT
                var conflictingBooking = await Task.Run(() =>
                {
                    return _context.DatLichPT
                        .Where(d => d.PTID == _ptId &&
                                   d.DatLichID != datLichID && // Loại trừ chính yêu cầu này
                                   (d.TrangThai == "Confirmed" || d.TrangThai == "Completed" || d.TrangThai == "Pending") && // Kiểm tra cả Pending vì có thể đã được PT chấp nhận
                                   // Kiểm tra trùng thời gian: (start1 < end2) && (start2 < end1)
                                   datLich.ThoiGianBatDau < d.ThoiGianKetThuc &&
                                   d.ThoiGianBatDau < datLich.ThoiGianKetThuc)
                        .FirstOrDefault();
                });

                if (conflictingBooking != null)
                {
                    // Lấy thông tin khách hàng của lịch trùng
                    var conflictingCustomer = await Task.Run(() => 
                        _context.Users.FirstOrDefault(u => u.UserID == conflictingBooking.KhachHangID));
                    string customerName = conflictingCustomer?.HoTen ?? conflictingCustomer?.Username ?? "Khách hàng";
                    string conflictTime = $"{conflictingBooking.ThoiGianBatDau:HH:mm} - {conflictingBooking.ThoiGianKetThuc:HH:mm}";
                    string conflictDate = conflictingBooking.ThoiGianBatDau.ToString("dd/MM/yyyy");

                    MessageBox.Show(
                        $"Bạn đã có lịch trùng với thời gian này!\n\n" +
                        $"Lịch hiện có:\n" +
                        $"• Khách hàng: {customerName}\n" +
                        $"• Ngày: {conflictDate}\n" +
                        $"• Thời gian: {conflictTime}\n" +
                        $"• Trạng thái: {conflictingBooking.TrangThai}\n\n" +
                        $"Vui lòng từ chối yêu cầu này hoặc yêu cầu khách hàng chọn thời gian khác.",
                        "Lịch trùng",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Cập nhật PTID và giữ nguyên trạng thái "Pending" (chờ thanh toán)
                // Lưu ý: Trạng thái sẽ chuyển thành "Confirmed" sau khi thanh toán thành công
                datLich.PTID = _ptId;
                datLich.TrangThai = "Pending"; // Giữ nguyên "Pending" vì CHECK constraint không cho phép "PendingPayment"
                datLich.NgayCapNhat = DateTime.Now;
                
                await Task.Run(() => _context.SaveChanges());

                MessageBox.Show("Đã đồng ý yêu cầu! Khách hàng cần thanh toán để xác nhận.", "Thành công", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload data
                await LoadPTRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnRejectRequest_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Guna2CircleButton;
                if (button?.Tag == null) return;

                string datLichID = button.Tag.ToString();

                var result = MessageBox.Show("Bạn có chắc chắn muốn từ chối yêu cầu này?", 
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    var success = await _ptDashboardService.RejectRequestAsync(datLichID);
                    if (success)
                    {
                        MessageBox.Show("Đã từ chối yêu cầu!", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Reload data
                        await LoadPTRequests();
                    }
                    else
                    {
                        MessageBox.Show("Không thể từ chối yêu cầu!", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDongY_Click(object sender, EventArgs e)
        {
            // Nút mẫu, không dùng
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            // Nút mẫu, không dùng
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                if (_parentDashboard != null && !_parentDashboard.IsDisposed)
                {
                    _parentDashboard.ShowDashboard();
                }
                else
                {
                    // Nếu không có parent dashboard, tạo Dashboard mới
                    var newDashboard = new frmDashBoard1();
                    this.Close();
                    newDashboard.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGiaoBT_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new GiaoBTChoUser())
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

        private async void BtnLichPT_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new frm_LichPT())
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    await frm.EnsureCurrentWeekLoadedAsync();
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở Lịch PT: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            _context?.Dispose();
            base.OnFormClosing(e);
        }

        // Các event handlers cũ giữ lại để không lỗi
        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void lblKhachHang1_Click(object sender, EventArgs e) { }
        private void lblKhachHang_Click(object sender, EventArgs e) { }
        private void lblBuoiTap_Click(object sender, EventArgs e) { }
        private void lblThuNhap_Click(object sender, EventArgs e) { }
        private void lblDanhGia_Click(object sender, EventArgs e) { }
        private void lblKhachHangDangTap_Click(object sender, EventArgs e) { }
        private void pnlImageDachSach1_Click(object sender, EventArgs e) { }
        private void lblTenDanhSach1_Click(object sender, EventArgs e) { }
        private void lblThoiGianDanhSach1_Click(object sender, EventArgs e) { }
        private void ptrImageLichTrinh_Click(object sender, EventArgs e) { }
        private void pnlLichTrinh1_Paint(object sender, PaintEventArgs e) { }
    }
}
