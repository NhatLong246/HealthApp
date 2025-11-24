using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using QRCoder;
using HealthApp.Models;
using System.Linq;
using System.Drawing.Imaging;

namespace HealthApp.Views.PT
{
    /// <summary>
    /// Form hiển thị QR code và WebView để thanh toán
    /// </summary>
    public partial class frm_PaymentQRCode : Form
    {
        private readonly string _paymentUrl;
        private readonly string _qrCodeUrl;
        private readonly string _orderId;
        private readonly string _paymentMethod;
        private readonly double _amount;
        private readonly WF_HealthTracker _context;
        private System.Windows.Forms.Timer _checkStatusTimer;

        public frm_PaymentQRCode(string paymentUrl, string qrCodeUrl, string orderId, string paymentMethod, double amount, WF_HealthTracker context)
        {
            InitializeComponent();
            _paymentUrl = paymentUrl;
            _qrCodeUrl = qrCodeUrl;
            _orderId = orderId;
            _paymentMethod = paymentMethod;
            _amount = amount;
            _context = context;
            
            InitializeComponents();
            LoadQRCode();
            LoadWebView();
            StartStatusCheck();
        }

        private void InitializeComponents()
        {
            this.Text = $"Thanh toán {_paymentMethod} - Mã đơn: {_orderId}";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void LoadQRCode()
        {
            try
            {
                // Tạo QR code từ payment URL hoặc QR code URL
                string qrData = !string.IsNullOrEmpty(_qrCodeUrl) ? _qrCodeUrl : _paymentUrl;

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, true);
                    
                    // Tạo PictureBox để hiển thị QR code
                    var picQR = new PictureBox
                    {
                        Image = qrCodeImage,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(300, 300),
                        Location = new Point(20, 80)
                    };

                    // Label hướng dẫn
                    var lblInstruction = new Label
                    {
                        Text = $"Quét mã QR bằng ứng dụng {_paymentMethod} để thanh toán",
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        AutoSize = true,
                        Location = new Point(20, 20)
                    };

                    // Label thông tin
                    var lblInfo = new Label
                    {
                        Text = $"Mã đơn hàng: {_orderId}\n" +
                               $"Số tiền: {_amount:N0}đ\n" +
                               $"Phương thức: {_paymentMethod}",
                        Font = new Font("Segoe UI", 9),
                        AutoSize = true,
                        Location = new Point(20, 50)
                    };

                    this.Controls.Add(lblInstruction);
                    this.Controls.Add(lblInfo);
                    this.Controls.Add(picQR);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo QR code: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadWebView()
        {
            try
            {
                // Tạo WebView2 để hiển thị trang thanh toán
                var webView = new WebView2
                {
                    Location = new Point(350, 20),
                    Size = new Size(520, 600),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
                };

                this.Controls.Add(webView);

                // Đợi WebView2 khởi tạo
                await webView.EnsureCoreWebView2Async(null);

                // Navigate đến payment URL
                webView.CoreWebView2.Navigate(_paymentUrl);

                // Xử lý navigation completed để kiểm tra thanh toán
                webView.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    if (e.IsSuccess)
                    {
                        // Có thể kiểm tra URL để xác định thanh toán thành công
                        var currentUrl = webView.CoreWebView2.Source;
                        if (currentUrl.Contains("success") || currentUrl.Contains("completed"))
                        {
                            CheckPaymentStatus();
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải WebView: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartStatusCheck()
        {
            // Tạo timer để kiểm tra trạng thái thanh toán mỗi 3 giây
            _checkStatusTimer = new System.Windows.Forms.Timer
            {
                Interval = 3000 // 3 giây
            };
            _checkStatusTimer.Tick += (s, e) => CheckPaymentStatus();
            _checkStatusTimer.Start();
        }

        private void CheckPaymentStatus()
        {
            try
            {
                var giaoDich = _context.GiaoDich.FirstOrDefault(g => g.GiaoDichID == _orderId);
                if (giaoDich != null && giaoDich.TrangThaiThanhToan == "Completed")
                {
                    _checkStatusTimer?.Stop();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking payment status: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _checkStatusTimer?.Stop();
            _checkStatusTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}

