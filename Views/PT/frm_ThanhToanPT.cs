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
        private readonly frmDashBoard _parentDashboard;
        private readonly string _datLichID;
        private readonly WF_HealthTracker _context;
        private DatLichPT _datLich;
        private HuanLuyenVien _pt;
        private Users _khachHang;
        private string _selectedPaymentMethod = ""; // "MoMo" hoặc "ZaloPay"
        private List<Guna2CustomGradientPanel> _paymentPanels = new List<Guna2CustomGradientPanel>();

        public frm_ThanhToanPT(frmDashBoard parentDashboard = null, string datLichID = null)
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
        }

        private async void LoadData()
        {
            try
            {
                if (string.IsNullOrEmpty(_datLichID))
                {
                    MessageBox.Show("Không có thông tin đặt lịch!", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
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

                // Hiển thị dữ liệu
                DisplayData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    lblThoiGian.Text = $"{_datLich.ThoiGianBatDau:HH:mm} - {_datLich.ThoiGianKetThuc:HH:mm}";
                    lblNgayTap.Text = _datLich.ThoiGianBatDau.ToString("dd/MM/yyyy");
                    
                    // Tính thứ trong tuần
                    string thu = GetDayOfWeekVietnamese(_datLich.ThoiGianBatDau.DayOfWeek);
                    lblThu.Text = thu;

                    // Hiển thị mục tiêu từ GhiChu
                    if (!string.IsNullOrEmpty(_datLich.GhiChu))
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

            var lblThoiGianCopy = new Label
            {
                AutoSize = true,
                BackColor = Color.White,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                ForeColor = SystemColors.ActiveBorder,
                Location = new Point(57, 161),
                Name = $"lblThoiGian_{_datLichID}",
                Text = _datLich != null ? $"{_datLich.ThoiGianBatDau:HH:mm} - {_datLich.ThoiGianKetThuc:HH:mm}" : ""
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
                Text = _datLich != null ? GetDayOfWeekVietnamese(_datLich.ThoiGianBatDau.DayOfWeek) : ""
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
                Text = _datLich != null ? _datLich.ThoiGianBatDau.ToString("dd/MM/yyyy") : ""
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
                return 0;
            }

            // Tính số giờ
            TimeSpan duration = _datLich.ThoiGianKetThuc - _datLich.ThoiGianBatDau;
            double hours = duration.TotalHours;

            // Tính tiền = số giờ * giá theo giờ
            double price = hours * _pt.GiaTheoGio.Value;

            return price;
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
                MessageBox.Show($"Lỗi khi thanh toán: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<bool> ProcessPayment()
        {
            try
            {
                var paymentService = new PaymentService();
                var orderId = GenerateGiaoDichID();
                var amount = (long)CalculatePrice();
                var orderInfo = $"Thanh toán PT - {_pt.HoTen} - {_datLich.NgayTap:dd/MM/yyyy}";
                
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
                }
                else
                {
                    MessageBox.Show("Phương thức thanh toán không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (result.Success && !string.IsNullOrEmpty(result.PaymentUrl))
                {
                    // Lưu thông tin giao dịch vào database với trạng thái "Pending"
                    var giaoDich = new GiaoDich
                    {
                        GiaoDichID = orderId,
                        DatLichID = _datLich.DatLichID,
                        KhachHangID = _datLich.KhachHangID,
                        PTID = _pt.PTID,
                        SoTien = CalculatePrice(),
                        PhuongThucThanhToan = _selectedPaymentMethod,
                        TrangThaiThanhToan = "Pending",
                        MaGiaoDich = result.TransactionId,
                        NgayGiaoDich = DateTime.Now
                    };

                    _context.GiaoDich.Add(giaoDich);
                    await Task.Run(() => _context.SaveChanges());

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
                            // Kiểm tra lại trạng thái từ database
                            var updatedGiaoDich = _context.GiaoDich.FirstOrDefault(g => g.GiaoDichID == orderId);
                            if (updatedGiaoDich != null && updatedGiaoDich.TrangThaiThanhToan == "Completed")
                            {
                                // Cập nhật trạng thái DatLichPT
                                _datLich.TrangThai = "Confirmed";
                                _datLich.NgayCapNhat = DateTime.Now;
                                await Task.Run(() => _context.SaveChanges());

                                MessageBox.Show("Thanh toán thành công!", "Thành công",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                NavigateBackToDashboard();
                                return true;
                            }
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
