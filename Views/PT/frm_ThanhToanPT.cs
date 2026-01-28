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
using HealthApp.Views.Dashboard;
using HealthApp.Models;
using HealthApp.Common.Helpers;
using HealthApp.Services;
using Guna.UI2.WinForms;

namespace HealthApp.Views.PT
{
    public partial class frm_ThanhToanPT : Form
    {
        private readonly frmDashBoard1 _parentDashboard;
        private readonly string _datLichID;
        private readonly WF_HealthTracker _context;
        private DatLichPT _datLich;
        private HuanLuyenVien _pt;
        private Users _khachHang;
        private string _selectedPaymentMethod = ""; // "MoMo" hoặc "ZaloPay"
        private List<Guna2CustomGradientPanel> _paymentPanels = new List<Guna2CustomGradientPanel>();
        private List<DatLichPT> _allDatLichInSchedule; // Danh sách tất cả các buổi tập trong lịch trình (nếu có)

        public frm_ThanhToanPT(frmDashBoard1 parentDashboard = null, string datLichID = null)
        {
            InitializeComponent();
            _parentDashboard = parentDashboard;
            _datLichID = datLichID;
            _context = new WF_HealthTracker();
            InitializeEventHandlers();
            LoadData();
        }

        private void InitializeEventHandlers()
        {
            btnThanhToan.Click += BtnThanhToan_Click;
            btnThemThanhToan.Click += BtnThemThanhToan_Click;

            // Event handlers cho chọn phương thức thanh toán
            pnlMomo.Click += PnlMoMo_Click;
            pnlZalopay.Click += PnlZaloPay_Click;
            
            // Kết nối event cho nút quay lại
            if (btnTroVe != null)
            {
                btnTroVe.Click += (s, e) => NavigateBackToDashboard();
            }

            // Lịch sử thanh toán
            if (btnLichSuThanhToan != null)
            {
                btnLichSuThanhToan.Click += BtnLichSuThanhToan_Click;
            }
        }

        private async void LoadData()
        {
            try
            {
                if (string.IsNullOrEmpty(_datLichID))
                {
                    // Không có đơn để thanh toán: vẫn hiển thị full UI, chỉ reset thông tin và disable thanh toán.
                    InitializeEmptyPaymentView();
                    return;
                }

                // Load DatLichPT
                _datLich = await Task.Run(() => _context.DatLichPT.FirstOrDefault(d => d.DatLichID == _datLichID));
                if (_datLich == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin đặt lịch!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Load PT (từ PTID trong DatLichPT)
                if (!string.IsNullOrEmpty(_datLich.PTID))
                {
                    _pt = await Task.Run(() => _context.HuanLuyenVien.FirstOrDefault(p => p.PTID == _datLich.PTID));
                }
                else
                {
                    MessageBox.Show("Yêu cầu này chưa được PT đồng ý!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                // Load khách hàng
                _khachHang = await Task.Run(() => _context.Users.FirstOrDefault(u => u.UserID == _datLich.KhachHangID));

                // Nếu có LichTrinhID, load tất cả các buổi tập trong lịch trình
                _allDatLichInSchedule = new List<DatLichPT>();
                if (!string.IsNullOrEmpty(_datLich.LichTrinhID))
                {
                    _allDatLichInSchedule = await Task.Run(() => _context.DatLichPT
                        .Where(d => d.LichTrinhID == _datLich.LichTrinhID 
                                 && d.KhachHangID == _datLich.KhachHangID 
                                 && d.PTID == _datLich.PTID
                                 && d.TrangThai == "Pending")
                        .OrderBy(d => d.ThoiGianBatDau)
                        .ToList());
                    
                    // Nếu không tìm thấy, thêm buổi tập hiện tại vào danh sách
                    if (_allDatLichInSchedule.Count == 0)
                    {
                        _allDatLichInSchedule.Add(_datLich);
                    }
                }
                else
                {
                    // Buổi tập đơn lẻ, chỉ có một buổi
                    _allDatLichInSchedule.Add(_datLich);
                }

                // Hiển thị dữ liệu
                DisplayData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Trạng thái khi user không có đơn Pending để thanh toán: vẫn show full UI nhưng không cho thao tác thanh toán.
        /// </summary>
        private void InitializeEmptyPaymentView()
        {
            try
            {
                // Thông tin tổng tiền
                lblTongTienThanhToan.Text = "0đ";
                lblTienThanhToan.Text = "0đ";

                // Reset card hiển thị đặt lịch mẫu
                lblTenNguoiYeuCauThue.Text = CurrentUser.User?.HoTen ?? CurrentUser.User?.Username ?? "User";
                lblMucTieuNguoiYeuCauThue.Text = "Chưa có đơn cần thanh toán";
                lblTenPTThanhToan.Text = "PT";
                lblThoiGian.Text = "";
                lblThu.Text = "";
                lblNgayTap.Text = "";

                // Ảnh có thể để trống
                try { ptrAvatarPT.Image = null; } catch { }
                try { ptrAvatarNguoiYeuCauach1.Image = null; } catch { }

                // Disable thao tác thanh toán
                btnThanhToan.Enabled = false;
                btnThanhToan.Text = "Không có đơn để thanh toán";
                btnThemThanhToan.Enabled = false;
                pnlMomo.Enabled = false;
                pnlZalopay.Enabled = false;

                _selectedPaymentMethod = "";
                pnlMomo.BorderColor = Color.Silver;
                pnlMomo.BorderThickness = 1;
                pnlZalopay.BorderColor = Color.Silver;
                pnlZalopay.BorderThickness = 1;

                // Giữ lại layout: panel đặt lịch vẫn hiện, nhưng chỉ có 1 card mẫu
                pnlTongTinDatLich.AutoScroll = true;
                pnlDanhSachThanhToan.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeEmptyPaymentView error: {ex.Message}");
            }
        }

        /// <summary>
        /// Hiển thị lịch sử thanh toán: số tiền, thời gian thanh toán, tên PT thụ hưởng.
        /// </summary>
        private void BtnLichSuThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null || string.IsNullOrWhiteSpace(CurrentUser.UserID))
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem lịch sử thanh toán!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var form = BuildPaymentHistoryDialog())
                {
                    form.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở lịch sử thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Form BuildPaymentHistoryDialog()
        {
            var form = new Form();
            form.Text = "Lịch sử thanh toán";
            form.StartPosition = FormStartPosition.CenterParent;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.BackColor = Color.FromArgb(245, 247, 250);
            form.Size = new Size(980, 600);

            var pnlMain = new Guna2ShadowPanel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                Radius = 15,
                ShadowColor = Color.Black,
                ShadowDepth = 10,
                Padding = new Padding(22)
            };
            form.Controls.Add(pnlMain);

            // Header
            var lblTitle = new Label
            {
                Text = "Lịch sử thanh toán",
                Font = new Font("Times New Roman", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                AutoSize = false,
                Location = new Point(22, 18),
                Size = new Size(820, 40)
            };
            pnlMain.Controls.Add(lblTitle);

            var lblSubTitle = new Label
            {
                Text = "Danh sách các giao dịch đã thanh toán thành công",
                Font = new Font("Times New Roman", 11.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(110, 110, 110),
                AutoSize = false,
                Location = new Point(24, 58),
                Size = new Size(880, 24)
            };
            pnlMain.Controls.Add(lblSubTitle);

            // Quick stats "chips" (custom) - dùng Panel + Label để tránh nút "x"
            var chipCountPanel = new Guna2Panel
            {
                Location = new Point(24, 90),
                Size = new Size(150, 32),
                FillColor = Color.FromArgb(239, 246, 255),
                BorderRadius = 10,
                BorderColor = Color.FromArgb(191, 219, 254),
                BorderThickness = 1
            };
            var chipCountLabel = new Label
            {
                Text = "0 giao dịch",
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Times New Roman", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235)
            };
            chipCountPanel.Controls.Add(chipCountLabel);
            pnlMain.Controls.Add(chipCountPanel);

            var chipTotalPanel = new Guna2Panel
            {
                Location = new Point(180, 90),
                Size = new Size(220, 32),
                FillColor = Color.FromArgb(240, 253, 244),
                BorderRadius = 10,
                BorderColor = Color.FromArgb(187, 247, 208),
                BorderThickness = 1
            };
            var chipTotalLabel = new Label
            {
                Text = "Tổng: 0đ",
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Times New Roman", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74)
            };
            chipTotalPanel.Controls.Add(chipTotalLabel);
            pnlMain.Controls.Add(chipTotalPanel);

            var container = new Guna2Panel
            {
                Location = new Point(22, 135),
                Size = new Size(916, 360),
                FillColor = Color.White,
                BorderRadius = 12,
                BorderColor = Color.FromArgb(229, 231, 235),
                BorderThickness = 1
            };
            pnlMain.Controls.Add(container);

            var dgv = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(896, 340),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(243, 244, 246),
                EnableHeadersVisualStyles = false
            };
            container.Controls.Add(dgv);

            // DGV styling
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(59, 130, 246);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 11F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgv.DefaultCellStyle.Font = new Font("Times New Roman", 10.5F, FontStyle.Regular);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(31, 41, 55);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 64, 175);
            dgv.DefaultCellStyle.Padding = new Padding(6);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);
            dgv.RowTemplate.Height = 38;

            var lblEmpty = new Label
            {
                Text = "",
                Font = new Font("Times New Roman", 12F, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(24, 515),
                Visible = false
            };
            pnlMain.Controls.Add(lblEmpty);

            var btnClose = new Guna2Button
            {
                Text = "Đóng",
                Size = new Size(140, 42),
                BorderRadius = 10,
                FillColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(798, 510),
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => form.Close();
            pnlMain.Controls.Add(btnClose);

            // Load data
            try
            {
                var userId = CurrentUser.UserID;
                var rows = (from gd in _context.GiaoDich
                            join pt in _context.HuanLuyenVien on gd.PTID equals pt.PTID
                            join ptUser in _context.Users on pt.UserID equals ptUser.UserID
                            where gd.KhachHangID == userId && gd.TrangThaiThanhToan == "Completed"
                            orderby gd.NgayGiaoDich descending
                            select new
                            {
                                TenPT = ptUser.HoTen ?? ptUser.Username ?? pt.PTID,
                                SoTien = gd.SoTien,
                                NgayGiaoDich = gd.NgayGiaoDich
                            }).ToList();

                var dt = new DataTable();
                dt.Columns.Add("Tên PT", typeof(string));
                dt.Columns.Add("Số tiền", typeof(string));
                dt.Columns.Add("Thời gian thanh toán", typeof(string));

                double total = 0;
                foreach (var r in rows)
                {
                    var dr = dt.NewRow();
                    dr["Tên PT"] = r.TenPT;
                    dr["Số tiền"] = r.SoTien.ToString("N0") + "đ";
                    dr["Thời gian thanh toán"] = r.NgayGiaoDich?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                    dt.Rows.Add(dr);
                    total += r.SoTien;
                }

                dgv.DataSource = dt;

                // update chips
                chipCountLabel.Text = $"{rows.Count} giao dịch";
                chipTotalLabel.Text = $"Tổng: {total:N0}đ";

                if (rows.Count == 0)
                {
                    lblEmpty.Text = "Bạn chưa có lịch sử thanh toán nào.";
                    lblEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblEmpty.Text = $"Không thể tải lịch sử thanh toán: {ex.Message}";
                lblEmpty.Visible = true;
            }

            form.AcceptButton = btnClose;
            form.CancelButton = btnClose;
            return form;
        }

        private void DisplayData()
        {
            try
            {
                // Hiển thị thông tin khách hàng
                if (_khachHang != null)
                {
                    lblTenNguoiYeuCauThue.Text = _khachHang.HoTen ?? _khachHang.Username;
                    LoadAvatar(ptrAvatarNguoiYeuCauach1, _khachHang.AnhDaiDien);
                }

                // Hiển thị thông tin PT
                if (_pt != null)
                {
                    var ptUser = _context.Users.FirstOrDefault(u => u.UserID == _pt.UserID);
                    if (ptUser != null)
                    {
                        lblTenPTThanhToan.Text = ptUser.HoTen ?? ptUser.Username;
                        LoadAvatar(ptrAvatarPT, _pt.AnhDaiDien);
                    }
                }

                // Hiển thị thời gian và ngày
                if (_datLich != null)
                {
                    // Kiểm tra xem có phải lịch trình nhiều ngày không
                    if (_allDatLichInSchedule != null && _allDatLichInSchedule.Count > 1)
                    {
                        // Lịch trình nhiều ngày
                        var firstSession = _allDatLichInSchedule.OrderBy(d => d.ThoiGianBatDau).First();
                        var lastSession = _allDatLichInSchedule.OrderByDescending(d => d.ThoiGianKetThuc).First();
                        
                        // Tính số tuần
                        var soNgay = (lastSession.ThoiGianKetThuc.Date - firstSession.ThoiGianBatDau.Date).TotalDays + 1;
                        var soTuan = (int)Math.Ceiling(soNgay / 7.0);
                        
                        lblThoiGian.Text = $"{soTuan} tuần";
                        lblNgayTap.Text = $"{firstSession.ThoiGianBatDau:dd/MM/yyyy} - {lastSession.ThoiGianBatDau:dd/MM/yyyy}";
                        lblThu.Text = ""; // Để trống cho lịch trình nhiều ngày
                    }
                    else
                    {
                        // Buổi tập đơn lẻ
                        lblThoiGian.Text = $"{_datLich.ThoiGianBatDau:HH:mm} - {_datLich.ThoiGianKetThuc:HH:mm}";
                        lblNgayTap.Text = _datLich.ThoiGianBatDau.ToString("dd/MM/yyyy");

                        // Tính thứ trong tuần
                        string thu = GetDayOfWeekVietnamese(_datLich.ThoiGianBatDau.DayOfWeek);
                        lblThu.Text = thu;
                    }

                    // Hiển thị mục tiêu từ MucTieuLuyenTap hoặc GhiChu
                    if (!string.IsNullOrEmpty(_datLich.MucTieuLuyenTap))
                    {
                        lblMucTieuNguoiYeuCauThue.Text = _datLich.MucTieuLuyenTap;
                    }
                    else if (!string.IsNullOrEmpty(_datLich.GhiChu))
                    {
                        lblMucTieuNguoiYeuCauThue.Text = _datLich.GhiChu;
                    }
                }

                // Tính toán và hiển thị số tiền
                CalculateAndDisplayPrice();

                // Hiển thị pnlDanhSachThanhToan trong pnlTongTinDatLich
                DisplayPaymentItem();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayPaymentItem()
        {
            try
            {
                // Ẩn panel mẫu pnlDanhSachThanhToan
                pnlDanhSachThanhToan.Visible = false;

                // Tạo panel thanh toán động trong pnlTongTinDatLich
                var paymentPanel = CreatePaymentItemPanel();
                pnlTongTinDatLich.Controls.Add(paymentPanel);
                _paymentPanels.Add(paymentPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị mục thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Guna2CustomGradientPanel CreatePaymentItemPanel()
        {
            var panel = new Guna2CustomGradientPanel
            {
                BackColor = Color.White,
                BorderColor = Color.Silver,
                BorderRadius = 20,
                BorderThickness = 1,
                Location = new Point(24, 53),
                Name = $"pnlPaymentItem_{_datLichID}",
                Size = new Size(958, 200)
            };

            // Copy các controls từ pnlDanhSachThanhToan mẫu
            // Thông tin khách hàng
            var pnlNguoiDungCopy = new Guna2ShadowPanel
            {
                BackColor = Color.Transparent,
                FillColor = Color.Honeydew,
                Location = new Point(49, 21),
                Name = $"pnlNguoiDung_{_datLichID}",
                Radius = 10,
                ShadowColor = Color.FromArgb(0, 192, 0),
                ShadowShift = 1,
                Size = new Size(356, 87)
            };

            var ptrAvatarCopy = new Guna2CirclePictureBox
            {
                ImageRotate = 0F,
                Location = new Point(17, 12),
                Name = $"ptrAvatar_{_datLichID}",
                ShadowDecoration = { Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle },
                Size = new Size(60, 53),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            if (_khachHang != null)
            {
                LoadAvatar(ptrAvatarCopy, _khachHang.AnhDaiDien);
            }
            pnlNguoiDungCopy.Controls.Add(ptrAvatarCopy);

            var lblTenCopy = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 13.2F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(82, 12),
                Name = $"lblTen_{_datLichID}",
                Text = _khachHang?.HoTen ?? _khachHang?.Username ?? ""
            };
            pnlNguoiDungCopy.Controls.Add(lblTenCopy);

            var lblMucTieuCopy = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(83, 46),
                Name = $"lblMucTieu_{_datLichID}",
                Text = _datLich?.GhiChu ?? ""
            };
            pnlNguoiDungCopy.Controls.Add(lblMucTieuCopy);
            panel.Controls.Add(pnlNguoiDungCopy);

            // Thông tin PT
            var pnlPTCopy = new Guna2ShadowPanel
            {
                BackColor = Color.Transparent,
                FillColor = Color.Honeydew,
                Location = new Point(575, 21),
                Name = $"pnlPT_{_datLichID}",
                Radius = 10,
                ShadowColor = Color.FromArgb(0, 192, 0),
                ShadowShift = 1,
                Size = new Size(356, 87)
            };

            var ptrPTAvatarCopy = new Guna2CirclePictureBox
            {
                ImageRotate = 0F,
                Location = new Point(17, 12),
                Name = $"ptrPTAvatar_{_datLichID}",
                ShadowDecoration = { Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle },
                Size = new Size(60, 53),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            if (_pt != null)
            {
                LoadAvatar(ptrPTAvatarCopy, _pt.AnhDaiDien);
            }
            pnlPTCopy.Controls.Add(ptrPTAvatarCopy);

            var lblPTTenCopy = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 13.2F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(82, 12),
                Name = $"lblPTTen_{_datLichID}",
                Text = ""
            };
            if (_pt != null)
            {
                var ptUser = _context.Users.FirstOrDefault(u => u.UserID == _pt.UserID);
                if (ptUser != null)
                {
                    lblPTTenCopy.Text = ptUser.HoTen ?? ptUser.Username;
                }
            }
            pnlPTCopy.Controls.Add(lblPTTenCopy);
            panel.Controls.Add(pnlPTCopy);

            // Icon và thông tin ngày giờ
            var ptrIconCopy = new Guna2PictureBox
            {
                BackColor = Color.White,
                Image = ptrIcon.Image,
                ImageRotate = 0F,
                Location = new Point(52, 122),
                Name = $"ptrIcon_{_datLichID}",
                Size = new Size(28, 26),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            panel.Controls.Add(ptrIconCopy);

            var lblChonNgayCopy = new Guna2HtmlLabel
            {
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 12F, FontStyle.Bold),
                Location = new Point(86, 123),
                Name = $"lblChonNgay_{_datLichID}",
                Text = "Ngày giờ"
            };
            panel.Controls.Add(lblChonNgayCopy);

            // Xác định nội dung hiển thị cho thời gian, thứ, ngày tập
            string thoiGianText = "";
            string thuText = "";
            string ngayTapText = "";
            
            if (_datLich != null)
            {
                if (_allDatLichInSchedule != null && _allDatLichInSchedule.Count > 1)
                {
                    // Lịch trình nhiều ngày
                    var firstSession = _allDatLichInSchedule.OrderBy(d => d.ThoiGianBatDau).First();
                    var lastSession = _allDatLichInSchedule.OrderByDescending(d => d.ThoiGianKetThuc).First();
                    var soNgay = (lastSession.ThoiGianKetThuc.Date - firstSession.ThoiGianBatDau.Date).TotalDays + 1;
                    var soTuan = (int)Math.Ceiling(soNgay / 7.0);
                    
                    thoiGianText = $"{soTuan} tuần";
                    thuText = ""; // Để trống
                    ngayTapText = $"{firstSession.ThoiGianBatDau:dd/MM/yyyy} - {lastSession.ThoiGianBatDau:dd/MM/yyyy}";
                }
                else
                {
                    // Buổi tập đơn lẻ
                    thoiGianText = $"{_datLich.ThoiGianBatDau:HH:mm} - {_datLich.ThoiGianKetThuc:HH:mm}";
                    thuText = GetDayOfWeekVietnamese(_datLich.ThoiGianBatDau.DayOfWeek);
                    ngayTapText = _datLich.ThoiGianBatDau.ToString("dd/MM/yyyy");
                }
            }
            
            var lblThoiGianCopy = new Label
            {
                AutoSize = true,
                BackColor = Color.White,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(57, 161),
                Name = $"lblThoiGian_{_datLichID}",
                Text = thoiGianText
            };
            panel.Controls.Add(lblThoiGianCopy);

            var lblThuCopy = new Label
            {
                AutoSize = true,
                BackColor = Color.White,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(170, 161),
                Name = $"lblThu_{_datLichID}",
                Text = thuText
            };
            panel.Controls.Add(lblThuCopy);

            var lblNgayTapCopy = new Label
            {
                AutoSize = true,
                BackColor = Color.White,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(227, 161),
                Name = $"lblNgayTap_{_datLichID}",
                Text = ngayTapText
            };
            panel.Controls.Add(lblNgayTapCopy);

            var lblDenCopy = new Label
            {
                AutoSize = true,
                BackColor = Color.White,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(460, 53),
                Name = $"lblDen_{_datLichID}",
                Text = "Đến"
            };
            panel.Controls.Add(lblDenCopy);

            // Tiền thanh toán
            var lblTienCopy = new Guna2HtmlLabel
            {
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15F, FontStyle.Bold),
                Location = new Point(492, 149),
                Name = $"lblTien_{_datLichID}",
                Text = "Tiền:"
            };
            panel.Controls.Add(lblTienCopy);

            var lblTienThanhToanCopy = new Guna2HtmlLabel
            {
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 15F, FontStyle.Bold),
                ForeColor = Color.Blue,
                Location = new Point(575, 149),
                Name = $"lblTienThanhToan_{_datLichID}",
                Text = CalculatePrice().ToString("N0") + "đ"
            };
            panel.Controls.Add(lblTienThanhToanCopy);

            return panel;
        }

        private void CalculateAndDisplayPrice()
        {
            try
            {
                double totalPrice = CalculatePrice();
                lblTienThanhToan.Text = totalPrice.ToString("N0") + "đ";
                lblTongTienThanhToan.Text = totalPrice.ToString("N0") + "đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính toán giá: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double CalculatePrice()
        {
            if (_datLich == null || _pt == null || _pt.GiaTheoGio == null)
            {
                System.Diagnostics.Debug.WriteLine("CalculatePrice: _datLich, _pt, hoặc _pt.GiaTheoGio là null");
                return 0;
            }

            // Kiểm tra xem có phải lịch trình nhiều ngày không
            if (_allDatLichInSchedule != null && _allDatLichInSchedule.Count > 1)
            {
                // Lịch trình nhiều ngày: tính tổng giá cho tất cả các buổi tập
                double totalPrice = 0;
                
                foreach (var session in _allDatLichInSchedule)
                {
                    if (session.ThoiGianBatDau != null && session.ThoiGianKetThuc != null)
                    {
                        TimeSpan duration = session.ThoiGianKetThuc - session.ThoiGianBatDau;
                        double hours = duration.TotalHours;
                        double sessionPrice = hours * _pt.GiaTheoGio.Value;
                        totalPrice += sessionPrice;
                        
                        System.Diagnostics.Debug.WriteLine($"CalculatePrice: Session {session.DatLichID} - {hours} giờ x {_pt.GiaTheoGio.Value} = {sessionPrice:N0}đ");
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"CalculatePrice: Tổng giá lịch trình ({_allDatLichInSchedule.Count} buổi) = {totalPrice:N0}đ");
                return totalPrice;
            }
            else
            {
                // Buổi tập đơn lẻ: tính giá cho một buổi
                TimeSpan duration = _datLich.ThoiGianKetThuc - _datLich.ThoiGianBatDau;
                double hours = duration.TotalHours;
                double price = hours * _pt.GiaTheoGio.Value;
                
                System.Diagnostics.Debug.WriteLine($"CalculatePrice: Buổi đơn lẻ - {hours} giờ x {_pt.GiaTheoGio.Value} = {price:N0}đ");
                return price;
            }
        }

        private string GetDayOfWeekVietnamese(DayOfWeek dayOfWeek)
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

                if (!Path.IsPathRooted(imagePath))
                {
                    var appDirectory = Application.StartupPath;
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
                else if (File.Exists(imagePath))
                {
                    pictureBox.Image = Image.FromFile(imagePath);
                }
                else
                {
                    pictureBox.Image = null;
                }
            }
            catch
            {
                pictureBox.Image = null;
            }
        }

        private void PnlMoMo_Click(object sender, EventArgs e)
        {
            _selectedPaymentMethod = "MoMo";
            UpdatePaymentMethodSelection();
        }

        private void PnlZaloPay_Click(object sender, EventArgs e)
        {
            _selectedPaymentMethod = "ZaloPay";
            UpdatePaymentMethodSelection();
        }

        private void UpdatePaymentMethodSelection()
        {
            // Highlight phương thức đã chọn
            if (_selectedPaymentMethod == "MoMo")
            {
                pnlMomo.BorderColor = Color.Blue;
                pnlMomo.BorderThickness = 2;
                pnlZalopay.BorderColor = Color.Silver;
                pnlZalopay.BorderThickness = 1;
            }
            else if (_selectedPaymentMethod == "ZaloPay")
            {
                pnlZalopay.BorderColor = Color.Blue;
                pnlZalopay.BorderThickness = 2;
                pnlMomo.BorderColor = Color.Silver;
                pnlMomo.BorderThickness = 1;
            }
        }

        private async void BtnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã chọn phương thức thanh toán
                if (string.IsNullOrEmpty(_selectedPaymentMethod))
                {
                    MessageBox.Show("Vui lòng chọn phương thức thanh toán!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra dữ liệu
                if (_datLich == null || _pt == null)
                {
                    MessageBox.Show("Thông tin đặt lịch không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Xác nhận thanh toán
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn thanh toán {CalculatePrice():N0}đ bằng {_selectedPaymentMethod}?",
                    "Xác nhận thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                // Giả lập thanh toán (trong thực tế sẽ gọi API thanh toán)
                bool paymentSuccess = await ProcessPayment();

                if (paymentSuccess)
                {
                    // Cập nhật trạng thái DatLichPT thành "Confirmed"
                    _datLich.TrangThai = "Confirmed";
                    _datLich.NgayCapNhat = DateTime.Now;

                    // Tạo giao dịch
                    var giaoDich = new GiaoDich
                    {
                        GiaoDichID = GenerateGiaoDichID(),
                        DatLichID = _datLich.DatLichID,
                        KhachHangID = _datLich.KhachHangID,
                        PTID = _pt.PTID,
                        SoTien = CalculatePrice(),
                        // Áp dụng hoa hồng app 15%
                        HoaHongApp = 15,
                        SoTienHoaHong = Math.Round(CalculatePrice() * 0.15, 0),
                        SoTienPTNhan = Math.Round(CalculatePrice() - (CalculatePrice() * 0.15), 0),
                        PhuongThucThanhToan = _selectedPaymentMethod,
                        TrangThaiThanhToan = "Completed",
                        NgayGiaoDich = DateTime.Now
                    };

                    _context.GiaoDich.Add(giaoDich);
                    await Task.Run(() => _context.SaveChanges());

                    MessageBox.Show("Thanh toán thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Đóng form và quay lại Dashboard
                    NavigateBackToDashboard();
                }
                else
                {
                    MessageBox.Show("Thanh toán thất bại! Vui lòng thử lại.", "Thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không hiển thị dialog
                System.Diagnostics.Debug.WriteLine($"Lỗi khi thanh toán: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private async Task<bool> ProcessPayment()
        {
            try
            {
                var paymentService = new PaymentService();
                var orderId = GenerateGiaoDichID();
                var amount = (long)CalculatePrice();

                // Lấy tên PT từ Users
                string ptName = "PT";
                if (_pt != null)
                {
                    var ptUser = _context.Users.FirstOrDefault(u => u.UserID == _pt.UserID);
                    if (ptUser != null)
                    {
                        ptName = ptUser.HoTen ?? ptUser.Username ?? "PT";
                    }
                }

                // Lấy ngày tập từ ThoiGianBatDau
                string ngayTap = _datLich?.ThoiGianBatDau.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy");

                var orderInfo = $"Thanh toán PT - {ptName} - {ngayTap}";

                // URL callback (có thể cấu hình trong App.config)
                var returnUrl = "https://your-domain.com/payment/return";
                var notifyUrl = "https://your-domain.com/payment/notify";
                var callbackUrl = "https://your-domain.com/payment/zalopay-callback";

                PaymentResult result;

                if (_selectedPaymentMethod == "MoMo")
                {
                    result = await paymentService.CreateMoMoPaymentAsync(
                        orderId,
                        amount,
                        orderInfo,
                        returnUrl,
                        notifyUrl
                    );
                }
                else if (_selectedPaymentMethod == "ZaloPay")
                {
                    result = await paymentService.CreateZaloPayPaymentAsync(
                        orderId,
                        amount,
                        orderInfo,
                        callbackUrl
                    );

                    // Log kết quả
                    System.Diagnostics.Debug.WriteLine($"=== ZaloPay Payment Result ===");
                    System.Diagnostics.Debug.WriteLine($"Success: {result.Success}");
                    System.Diagnostics.Debug.WriteLine($"Message: {result.Message}");
                    System.Diagnostics.Debug.WriteLine($"PaymentUrl: {result.PaymentUrl}");
                    System.Diagnostics.Debug.WriteLine($"OrderId: {result.OrderId}");
                }
                else
                {
                    MessageBox.Show("Phương thức thanh toán không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Kiểm tra kết quả và hiển thị lỗi nếu có
                if (!result.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"Payment failed: {result.Message}");
                    MessageBox.Show(
                        $"Không thể tạo yêu cầu thanh toán {_selectedPaymentMethod}!\n\n" +
                        $"Lỗi: {result.Message}\n\n" +
                        "Vui lòng kiểm tra:\n" +
                        "1. Kết nối internet\n" +
                        "2. Cấu hình thanh toán trong App.config\n" +
                        "3. Thử lại sau vài phút",
                        "Lỗi thanh toán",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }

                if (result.Success && !string.IsNullOrEmpty(result.PaymentUrl))
                {
                    // Kiểm tra xem đã có GiaoDich cho DatLichID này chưa (do constraint UNIQUE)
                    // Nếu là lịch trình, kiểm tra xem có GiaoDich cho bất kỳ DatLichID nào trong lịch trình chưa
                    GiaoDich existingGiaoDich = null;
                    if (_allDatLichInSchedule != null && _allDatLichInSchedule.Count > 1)
                    {
                        // Lịch trình: kiểm tra xem có GiaoDich cho bất kỳ buổi tập nào trong lịch trình chưa
                        var datLichIDs = _allDatLichInSchedule.Select(d => d.DatLichID).ToList();
                        existingGiaoDich = _context.GiaoDich
                            .FirstOrDefault(g => datLichIDs.Contains(g.DatLichID));
                    }
                    else
                    {
                        // Buổi tập đơn lẻ
                        existingGiaoDich = _context.GiaoDich
                            .FirstOrDefault(g => g.DatLichID == _datLich.DatLichID);
                    }

                    GiaoDich giaoDich;

                    if (existingGiaoDich != null)
                    {
                        // Đã có giao dịch, kiểm tra trạng thái
                        if (existingGiaoDich.TrangThaiThanhToan == "Completed")
                        {
                            MessageBox.Show(
                                "Booking này đã được thanh toán thành công rồi!\n\n" +
                                $"Mã giao dịch: {existingGiaoDich.GiaoDichID}\n" +
                                $"Ngày thanh toán: {existingGiaoDich.NgayGiaoDich:dd/MM/yyyy HH:mm}",
                                "Đã thanh toán",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            return false;
                        }
                        else if (existingGiaoDich.TrangThaiThanhToan == "Pending")
                        {
                            // Cập nhật giao dịch đang pending (giữ nguyên GiaoDichID để tránh conflict)
                            System.Diagnostics.Debug.WriteLine($"Cập nhật giao dịch đang pending: {existingGiaoDich.GiaoDichID}");
                            giaoDich = existingGiaoDich;
                            // Giữ nguyên GiaoDichID cũ, chỉ cập nhật thông tin thanh toán
                            var totalPrice = CalculatePrice();
                            giaoDich.SoTien = totalPrice;
                            // Áp dụng hoa hồng app 15%
                            giaoDich.HoaHongApp = 15;
                            giaoDich.SoTienHoaHong = Math.Round(totalPrice * 0.15, 0);
                            giaoDich.SoTienPTNhan = Math.Round(totalPrice - (totalPrice * 0.15), 0);
                            giaoDich.PhuongThucThanhToan = _selectedPaymentMethod;
                            giaoDich.TrangThaiThanhToan = "Pending";
                            giaoDich.MaGiaoDich = result.TransactionId;
                            giaoDich.NgayGiaoDich = DateTime.Now;

                            // Cập nhật orderId để dùng trong form QR code
                            orderId = existingGiaoDich.GiaoDichID;
                        }
                        else
                        {
                            // Trạng thái khác (Refunded), cho phép tạo mới bằng cách xóa cũ
                            System.Diagnostics.Debug.WriteLine($"Xóa giao dịch cũ với trạng thái: {existingGiaoDich.TrangThaiThanhToan}");
                            _context.GiaoDich.Remove(existingGiaoDich);
                            await Task.Run(() => _context.SaveChanges());

                            // Tạo mới
                            var totalPriceRefund = CalculatePrice();
                            giaoDich = new GiaoDich
                            {
                                GiaoDichID = orderId,
                                DatLichID = _datLich.DatLichID,
                                KhachHangID = _datLich.KhachHangID,
                                PTID = _pt.PTID,
                                SoTien = totalPriceRefund,
                                // Áp dụng hoa hồng app 15%
                                HoaHongApp = 15,
                                SoTienHoaHong = Math.Round(totalPriceRefund * 0.15, 0),
                                SoTienPTNhan = Math.Round(totalPriceRefund - (totalPriceRefund * 0.15), 0),
                                PhuongThucThanhToan = _selectedPaymentMethod,
                                TrangThaiThanhToan = "Pending",
                                MaGiaoDich = result.TransactionId,
                                NgayGiaoDich = DateTime.Now
                            };
                            _context.GiaoDich.Add(giaoDich);
                        }
                    }
                    else
                    {
                        // Chưa có giao dịch, tạo mới
                        // Với lịch trình nhiều ngày, dùng DatLichID của buổi tập đầu tiên làm đại diện
                        string representativeDatLichID = _datLich.DatLichID;
                        if (_allDatLichInSchedule != null && _allDatLichInSchedule.Count > 1)
                        {
                            representativeDatLichID = _allDatLichInSchedule.OrderBy(d => d.ThoiGianBatDau).First().DatLichID;
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Tạo giao dịch mới cho DatLichID: {representativeDatLichID} (lịch trình có {_allDatLichInSchedule?.Count ?? 1} buổi)");
                        var totalPriceNew = CalculatePrice();
                        giaoDich = new GiaoDich
                        {
                            GiaoDichID = orderId,
                            DatLichID = representativeDatLichID,
                            KhachHangID = _datLich.KhachHangID,
                            PTID = _pt.PTID,
                            SoTien = totalPriceNew,
                            // Áp dụng hoa hồng app 15%
                            HoaHongApp = 15,
                            SoTienHoaHong = Math.Round(totalPriceNew * 0.15, 0),
                            SoTienPTNhan = Math.Round(totalPriceNew - (totalPriceNew * 0.15), 0),
                            PhuongThucThanhToan = _selectedPaymentMethod,
                            TrangThaiThanhToan = "Pending",
                            MaGiaoDich = result.TransactionId,
                            NgayGiaoDich = DateTime.Now
                        };
                        _context.GiaoDich.Add(giaoDich);
                    }

                    // Lưu thay đổi
                    await Task.Run(() => _context.SaveChanges());
                    System.Diagnostics.Debug.WriteLine($"Đã lưu giao dịch: {giaoDich.GiaoDichID}");

                    // Mở form hiển thị QR code và WebView
                    using (var paymentForm = new frm_PaymentQRCode(
                        result.PaymentUrl,
                        result.QrCodeUrl,
                        orderId,
                        _selectedPaymentMethod,
                        CalculatePrice(),
                        _context))
                    {
                        var paymentResult = paymentForm.ShowDialog(this);

                        if (paymentResult == DialogResult.OK)
                        {
                            // Reload từ database để đảm bảo có dữ liệu mới nhất
                            _context.Entry(_context.GiaoDich.FirstOrDefault(g => g.GiaoDichID == orderId)).Reload();
                            var updatedGiaoDich = _context.GiaoDich.FirstOrDefault(g => g.GiaoDichID == orderId);

                            // CHỈ xử lý thành công khi thực sự có trạng thái "Completed"
                            if (updatedGiaoDich != null && updatedGiaoDich.TrangThaiThanhToan == "Completed")
                            {
                                // Cập nhật trạng thái cho tất cả các buổi tập trong lịch trình
                                if (_allDatLichInSchedule != null && _allDatLichInSchedule.Count > 1)
                                {
                                    // Lịch trình nhiều ngày: cập nhật tất cả các buổi tập
                                    foreach (var session in _allDatLichInSchedule)
                                    {
                                        var dbSession = _context.DatLichPT.FirstOrDefault(d => d.DatLichID == session.DatLichID);
                                        if (dbSession != null)
                                        {
                                            dbSession.TrangThai = "Confirmed";
                                            dbSession.NgayCapNhat = DateTime.Now;
                                        }
                                    }
                                }
                                else
                                {
                                    // Buổi tập đơn lẻ
                                    _datLich.TrangThai = "Confirmed";
                                    _datLich.NgayCapNhat = DateTime.Now;
                                }
                                
                                await Task.Run(() => _context.SaveChanges());

                                // Thông báo đã được hiển thị trong frm_PaymentQRCode, chỉ cần quay về trang chủ
                                System.Diagnostics.Debug.WriteLine("Thanh toán thành công, đang quay về trang chủ...");

                                // Đóng form thanh toán
                                this.Close();

                                // Quay về trang chủ
                                NavigateBackToDashboard();
                                return true;
                            }
                            else
                            {
                                // Nếu chưa completed, có thể user đã đóng form trước khi thanh toán
                                System.Diagnostics.Debug.WriteLine($"Thanh toán chưa hoàn tất. Trạng thái: {updatedGiaoDich?.TrangThaiThanhToan ?? "NULL"}");
                                MessageBox.Show(
                                    "Thanh toán chưa hoàn tất.\n\n" +
                                    "Nếu bạn đã quét mã QR và thanh toán, vui lòng đợi vài phút để hệ thống xác nhận.\n\n" +
                                    "Hoặc liên hệ hỗ trợ nếu thanh toán đã thành công nhưng chưa được cập nhật.",
                                    "Thanh toán chưa hoàn tất",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                            }
                        }
                        else
                        {
                            // User đã hủy thanh toán
                            System.Diagnostics.Debug.WriteLine("User đã hủy thanh toán.");
                            MessageBox.Show(
                                "Thanh toán đã bị hủy.\n\n" +
                                "Nếu bạn muốn thanh toán lại, vui lòng thử lại.",
                                "Đã hủy thanh toán",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                    }

                    return false;
                }
                else
                {
                    MessageBox.Show($"Lỗi khi tạo thanh toán: {result.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string GenerateGiaoDichID()
        {
            var lastGiaoDich = _context.GiaoDich
                .OrderByDescending(g => g.GiaoDichID)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastGiaoDich != null && !string.IsNullOrEmpty(lastGiaoDich.GiaoDichID))
            {
                var parts = lastGiaoDich.GiaoDichID.Split('_');
                if (parts.Length == 2 && int.TryParse(parts[1], out int number))
                {
                    nextNumber = number + 1;
                }
            }

            return $"gd_{nextNumber:D4}";
        }

        private void BtnThemThanhToan_Click(object sender, EventArgs e)
        {
            // Chức năng thêm thanh toán (có thể dùng để thêm nhiều buổi tập)
            // Hiện tại chưa cần implement
        }

        /// <summary>
        /// Quay lại Dashboard
        /// </summary>
        public void NavigateBackToDashboard()
        {
            try
            {
                if (_parentDashboard != null && !_parentDashboard.IsDisposed)
                {
                    this.Hide();
                    _parentDashboard.ShowDashboard();
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại Dashboard: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosing(e);

            // Nếu đóng form bằng nút X, quay lại Dashboard
            if (e.CloseReason == CloseReason.UserClosing && _parentDashboard != null && !_parentDashboard.IsDisposed)
            {
                e.Cancel = true;
                NavigateBackToDashboard();
            }
        }
    }
}