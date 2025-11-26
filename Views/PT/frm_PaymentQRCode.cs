using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using HealthApp.Models;
using System.Linq;

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
        private WebView2 _webView; // Lưu reference để dùng ở các method khác

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
            LoadWebView();
            StartStatusCheck();
        }

        private void InitializeComponents()
        {
            this.Text = $"Thanh toán {_paymentMethod} - Mã đơn: {_orderId}";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }


        private async void LoadWebView()
        {
            try
            {
                // Tạo WebView2 để hiển thị trang thanh toán (chiếm toàn bộ form)
                _webView = new WebView2
                {
                    Dock = DockStyle.Fill, // Chiếm toàn bộ form
                    Location = new Point(0, 0)
                };

                this.Controls.Add(_webView);

                // Đợi WebView2 khởi tạo
                await _webView.EnsureCoreWebView2Async(null);

                // Xử lý navigation starting - phát hiện khi user click vào link/button trong web
                _webView.CoreWebView2.NavigationStarting += (s, e) =>
                {
                    var url = e.Uri.ToLower();
                    System.Diagnostics.Debug.WriteLine($"Navigation starting to: {url}");

                    // Phát hiện callback URL với status=1 (thanh toán thành công)
                    if (url.Contains("status=1") || url.Contains("status=1&") || url.Contains("&status=1"))
                    {
                        // Chặn navigation
                        e.Cancel = true;
                        
                        System.Diagnostics.Debug.WriteLine("Detected payment success callback (status=1)!");
                        
                        // Tự động cập nhật database thành "Completed"
                        UpdatePaymentStatusToCompleted();
                        
                        // Đợi một chút để database được cập nhật
                        System.Threading.Thread.Sleep(1000);
                        
                        // Kiểm tra trạng thái thanh toán và đóng form
                        CheckPaymentStatusAndClose();
                        return;
                    }

                    // Phát hiện các URL liên quan đến thanh toán thành công hoặc quay về
                    if (url.Contains("success") || 
                        url.Contains("completed") || 
                        url.Contains("return") ||
                        url.Contains("callback") ||
                        url.Contains("home") ||
                        url.Contains("dashboard") ||
                        url.Contains("back") ||
                        url.Contains("close") ||
                        url.Contains("finish"))
                    {
                        // Chặn navigation
                        e.Cancel = true;
                        
                        // Kiểm tra trạng thái thanh toán và đóng form
                        System.Threading.Thread.Sleep(500); // Đợi một chút để database được cập nhật
                        CheckPaymentStatusAndClose();
                    }
                };

                // Xử lý navigation completed để kiểm tra thanh toán
                _webView.CoreWebView2.NavigationCompleted += async (s, e) =>
                {
                    if (e.IsSuccess)
                    {
                        var currentUrl = _webView.CoreWebView2.Source.ToLower();
                        System.Diagnostics.Debug.WriteLine($"Navigation completed to: {currentUrl}");

                        // Phát hiện callback URL với status=1 (thanh toán thành công)
                        if (currentUrl.Contains("status=1") || currentUrl.Contains("status=1&") || currentUrl.Contains("&status=1"))
                        {
                            System.Diagnostics.Debug.WriteLine("Detected payment success callback (status=1) in NavigationCompleted!");
                            
                            // Tự động cập nhật database thành "Completed"
                            UpdatePaymentStatusToCompleted();
                            
                            // Đợi một chút để database được cập nhật
                            await Task.Delay(1000);
                            
                            // Kiểm tra trạng thái thanh toán và đóng form
                            CheckPaymentStatusAndClose();
                            return;
                        }

                        // Kiểm tra URL để xác định thanh toán thành công
                        if (currentUrl.Contains("success") || 
                            currentUrl.Contains("completed") ||
                            currentUrl.Contains("thanh-toan-thanh-cong"))
                        {
                            // Đợi một chút để database được cập nhật
                            await Task.Delay(2000);
                            
                            // Inject script để kiểm tra nội dung trang
                            try
                            {
                                string checkScript = @"
                                    (function() {
                                        var bodyText = document.body.innerText || document.body.textContent || '';
                                        if (bodyText.includes('THANH TOÁN THÀNH CÔNG') || 
                                            bodyText.includes('thanh toán thành công') ||
                                            bodyText.includes('PAYMENT SUCCESSFUL')) {
                                            return 'payment_success_page';
                                        }
                                        return 'not_success';
                                    })();
                                ";
                                var result = await _webView.CoreWebView2.ExecuteScriptAsync(checkScript);
                                
                                if (result != null && result.Contains("payment_success_page"))
                                {
                                    System.Diagnostics.Debug.WriteLine("Detected payment success page!");
                                    
                                    // Tự động cập nhật database thành "Completed"
                                    UpdatePaymentStatusToCompleted();
                                    
                                    await Task.Delay(1000);
                                    CheckPaymentStatusAndClose();
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error checking page content: {ex.Message}");
                                // Fallback: vẫn kiểm tra database
                                CheckPaymentStatusAndClose();
                            }
                        }
                    }
                };

                // Lắng nghe messages từ JavaScript trong web page
                _webView.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    try
                    {
                        var message = e.TryGetWebMessageAsString();
                        System.Diagnostics.Debug.WriteLine($"WebMessage received: {message}");

                        if (message == "navigate_home" || 
                            message == "payment_success" || 
                            message == "payment_success_page")
                        {
                            // Tự động cập nhật database thành "Completed" khi nhận message từ trang success
                            if (message == "payment_success" || message == "payment_success_page")
                            {
                                UpdatePaymentStatusToCompleted();
                            }
                            
                            // Kiểm tra trạng thái thanh toán và đóng form
                            System.Threading.Thread.Sleep(1000); // Đợi database cập nhật
                            CheckPaymentStatusAndClose();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error handling WebMessage: {ex.Message}");
                    }
                };

                // Inject JavaScript sau khi page load để lắng nghe các sự kiện
                _webView.CoreWebView2.DOMContentLoaded += async (s, e) =>
                {
                    try
                    {
                        // Đợi một chút để DOM hoàn toàn load
                        await Task.Delay(500);
                        
                        // Inject script để lắng nghe click events trên các button/link
                        string script = @"
                            (function() {
                                // Kiểm tra xem có phải trang success không
                                var bodyText = document.body.innerText || document.body.textContent || '';
                                if (bodyText.includes('THANH TOÁN THÀNH CÔNG') || 
                                    bodyText.includes('thanh toán thành công') ||
                                    bodyText.includes('PAYMENT SUCCESSFUL')) {
                                    // Gửi message ngay lập tức
                                    if (window.chrome && window.chrome.webview) {
                                        window.chrome.webview.postMessage('payment_success_page');
                                    }
                                }
                                
                                // Lắng nghe click trên tất cả các button và link
                                document.addEventListener('click', function(e) {
                                    var target = e.target;
                                    var text = (target.textContent || target.innerText || '').toLowerCase();
                                    var href = target.href || target.getAttribute('href') || '';
                                    
                                    // Phát hiện các button/link liên quan đến quay về
                                    if (text.includes('về ngay') ||
                                        text.includes('về trang chủ') || 
                                        text.includes('quay lại') || 
                                        text.includes('trang chủ') ||
                                        text.includes('home') ||
                                        text.includes('dashboard') ||
                                        href.includes('home') ||
                                        href.includes('dashboard') ||
                                        href.includes('return')) {
                                        // Chặn navigation mặc định
                                        e.preventDefault();
                                        e.stopPropagation();
                                        
                                        // Gửi message về C# để xử lý
                                        if (window.chrome && window.chrome.webview) {
                                            window.chrome.webview.postMessage('navigate_home');
                                        }
                                        return false;
                                    }
                                }, true);
                                
                                // Lắng nghe các form submit
                                document.addEventListener('submit', function(e) {
                                    var form = e.target;
                                    var action = form.action || '';
                                    if (action.includes('success') || action.includes('completed') || action.includes('return')) {
                                        e.preventDefault();
                                        if (window.chrome && window.chrome.webview) {
                                            window.chrome.webview.postMessage('payment_success');
                                        }
                                        return false;
                                    }
                                }, true);
                            })();
                        ";
                        await _webView.CoreWebView2.ExecuteScriptAsync(script);
                        System.Diagnostics.Debug.WriteLine("JavaScript injected successfully");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error injecting script: {ex.Message}");
                    }
                };

                // Navigate đến payment URL
                _webView.CoreWebView2.Navigate(_paymentUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải WebView: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tự động cập nhật trạng thái thanh toán thành "Completed" khi phát hiện callback thành công
        /// </summary>
        private void UpdatePaymentStatusToCompleted()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Updating payment status to Completed for {_orderId}");
                
                using (var context = new WF_HealthTracker())
                {
                    var giaoDich = context.GiaoDich.FirstOrDefault(g => g.GiaoDichID == _orderId);
                    if (giaoDich != null && giaoDich.TrangThaiThanhToan != "Completed")
                    {
                        giaoDich.TrangThaiThanhToan = "Completed";
                        giaoDich.NgayGiaoDich = DateTime.Now;
                        context.SaveChanges();
                        
                        System.Diagnostics.Debug.WriteLine($"Successfully updated payment status to Completed for {_orderId}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"GiaoDich not found or already Completed: {_orderId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating payment status: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái thanh toán và đóng form nếu thành công
        /// </summary>
        private void CheckPaymentStatusAndClose()
        {
            try
            {
                // Reload từ database để đảm bảo có dữ liệu mới nhất
                using (var freshContext = new WF_HealthTracker())
                {
                    var giaoDich = freshContext.GiaoDich.FirstOrDefault(g => g.GiaoDichID == _orderId);

                    if (giaoDich != null && giaoDich.TrangThaiThanhToan == "Completed")
                    {
                        _checkStatusTimer?.Stop();

                        // Hiển thị thông báo thành công trên UI thread
                        this.Invoke(new Action(() =>
                        {
                            // Hiển thị thông báo thành công
                            MessageBox.Show(
                                "✓ Thanh toán thành công!\n\n" +
                                $"Mã đơn hàng: {_orderId}\n" +
                                $"Số tiền: {_amount:N0} VNĐ\n" +
                                $"Phương thức: {_paymentMethod}\n\n" +
                                "Đang quay lại trang chủ...",
                                "Thanh toán thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            // Đóng form và trả về OK để quay về trang chủ
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CheckPaymentStatusAndClose: {ex.Message}");
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
                // Reload từ database để đảm bảo có dữ liệu mới nhất
                // Tạo context mới để tránh cache
                using (var freshContext = new WF_HealthTracker())
                {
                    var giaoDich = freshContext.GiaoDich.FirstOrDefault(g => g.GiaoDichID == _orderId);
                
                if (giaoDich != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Checking payment status for {_orderId}: {giaoDich.TrangThaiThanhToan}");
                    
                    // CHỈ báo thành công khi thực sự có trạng thái "Completed"
                    if (giaoDich.TrangThaiThanhToan == "Completed")
                    {
                        _checkStatusTimer?.Stop();
                        
                        // Hiển thị thông báo thành công trên UI thread
                        this.Invoke(new Action(() =>
                        {
                            // Hiển thị thông báo thành công
                            var result = MessageBox.Show(
                                "✓ Thanh toán thành công!\n\n" +
                                $"Mã đơn hàng: {_orderId}\n" +
                                $"Số tiền: {_amount:N0} VNĐ\n" +
                                $"Phương thức: {_paymentMethod}\n\n" +
                                "Bấm OK để quay lại trang chủ.",
                                "Thanh toán thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            
                            // Đóng form và trả về OK để quay về trang chủ
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }));
                    }
                    // Nếu vẫn là Pending, tiếp tục chờ
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"GiaoDich not found for {_orderId}");
                }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking payment status: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Nếu user đóng form bằng X và chưa thanh toán, hủy thanh toán
            if (e.CloseReason == CloseReason.UserClosing && this.DialogResult != DialogResult.OK)
            {
                try
                {
                    // Kiểm tra lại trạng thái trước khi đóng - tạo context mới để tránh disposed
                    using (var freshContext = new WF_HealthTracker())
                    {
                        var giaoDich = freshContext.GiaoDich.FirstOrDefault(g => g.GiaoDichID == _orderId);
                        if (giaoDich != null && giaoDich.TrangThaiThanhToan != "Completed")
                        {
                            var result = MessageBox.Show(
                                "Bạn có chắc chắn muốn hủy thanh toán?\n\n" +
                                "Nếu đã quét mã QR và thanh toán, vui lòng đợi vài giây để hệ thống xác nhận.",
                                "Xác nhận hủy",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );

                            if (result == DialogResult.No)
                            {
                                e.Cancel = true; // Không đóng form
                                return;
                            }
                        }
                        
                        // Đóng với Cancel nếu chưa thanh toán
                        if (giaoDich == null || giaoDich.TrangThaiThanhToan != "Completed")
                        {
                            this.DialogResult = DialogResult.Cancel;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in OnFormClosing: {ex.Message}");
                    // Nếu có lỗi, vẫn cho phép đóng form
                    this.DialogResult = DialogResult.Cancel;
                }
            }

            _checkStatusTimer?.Stop();
            _checkStatusTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}

