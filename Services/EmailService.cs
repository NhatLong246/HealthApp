using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HealthApp.Services.Interfaces;

namespace HealthApp.Services
{
    /// <summary>
    /// Service để gửi email qua SMTP
    /// </summary>
    public class EmailService : IEmailService
    {
        // Cấu hình SMTP - có thể di chuyển vào App.config sau
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly bool _enableSsl = true;
        
       
        private readonly string _senderEmail = "anhnksss5@gmail.com"; // ⚠️ THAY BẰNG EMAIL GMAIL THẬT CỦA BẠN
        private readonly string _senderPassword = "crxt jkkn fgav ondq"; // ⚠️ THAY BẰNG APP PASSWORD 16 KÝ TỰ (code sẽ tự động bỏ dấu cách)

        /// <summary>
        /// Gửi email OTP đến địa chỉ email
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> SendOTPEmailAsync(string toEmail, string otpCode, string userName = null)
        {
            string errorMessage = string.Empty;
            
            try
            {
                // Kiểm tra email và password đã được cấu hình chưa
                if (_senderEmail == "your-email@gmail.com" || _senderPassword == "your-app-password")
                {
                    errorMessage = "Email và App Password chưa được cấu hình!\n\n";
                    errorMessage += "Vui lòng cập nhật _senderEmail và _senderPassword trong EmailService.cs (dòng 28-29)";
                    System.Diagnostics.Debug.WriteLine("⚠️ LỖI: Email và App Password chưa được cấu hình!");
                    return (false, errorMessage);
                }

                // Log thông tin cấu hình (ẩn password)
                System.Diagnostics.Debug.WriteLine($"Đang gửi email từ: {_senderEmail}");
                System.Diagnostics.Debug.WriteLine($"Đến: {toEmail}");
                System.Diagnostics.Debug.WriteLine($"SMTP Server: {_smtpServer}:{_smtpPort}");

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    // Cấu hình SMTP client
                    client.EnableSsl = _enableSsl;
                    client.UseDefaultCredentials = false;
                    
                    // Loại bỏ dấu cách trong App Password nếu có
                    string cleanPassword = _senderPassword?.Replace(" ", "").Trim();
                    
                    client.Credentials = new NetworkCredential(_senderEmail, cleanPassword);
                    client.Timeout = 30000; // 30 giây timeout
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_senderEmail, "HealthApp"),
                        Subject = "Mã OTP Khôi Phục Mật Khẩu",
                        Body = GenerateEmailBody(otpCode, userName),
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    // Log thông tin để debug (không log password)
                    System.Diagnostics.Debug.WriteLine($"Đang gửi email từ: {_senderEmail}");
                    System.Diagnostics.Debug.WriteLine($"Đến: {toEmail}");
                    System.Diagnostics.Debug.WriteLine($"App Password length: {cleanPassword?.Length ?? 0} ký tự");
                    System.Diagnostics.Debug.WriteLine("Đang kết nối và gửi email...");
                    
                    await Task.Run(() => client.Send(mailMessage));
                    System.Diagnostics.Debug.WriteLine("✅ Email đã được gửi thành công!");
                    return (true, string.Empty);
                }
            }
            catch (SmtpException smtpEx)
            {
                // Xử lý lỗi SMTP cụ thể
                string detailedError = $"Status Code: {smtpEx.StatusCode}\nMessage: {smtpEx.Message}";
                
                if (smtpEx.Message.Contains("Authentication Required") || 
                    smtpEx.Message.Contains("5.7.0") ||
                    smtpEx.StatusCode == SmtpStatusCode.MustIssueStartTlsFirst)
                {
                    errorMessage = "⚠️ LỖI XÁC THỰC EMAIL!\n\n";
                    errorMessage += "Chi tiết: " + smtpEx.Message + "\n\n";
                    errorMessage += "Các bước kiểm tra:\n";
                    errorMessage += "1. Kiểm tra email đã được cập nhật trong EmailService.cs (dòng 28)\n";
                    errorMessage += "2. Kiểm tra App Password đã được cập nhật (dòng 29)\n";
                    errorMessage += "3. Đảm bảo đã tạo App Password (KHÔNG phải mật khẩu thường)\n";
                    errorMessage += "4. Đảm bảo đã bật 2-Step Verification\n";
                    errorMessage += "5. App Password phải có đúng 16 ký tự\n\n";
                    errorMessage += "Hướng dẫn: https://myaccount.google.com/apppasswords";
                }
                else if (smtpEx.StatusCode == SmtpStatusCode.GeneralFailure)
                {
                    errorMessage = "⚠️ LỖI KẾT NỐI SMTP!\n\n";
                    errorMessage += "Có thể do:\n";
                    errorMessage += "1. Không có kết nối Internet\n";
                    errorMessage += "2. Firewall chặn kết nối SMTP\n";
                    errorMessage += "3. SMTP server không khả dụng\n";
                    errorMessage += "\nChi tiết: " + smtpEx.Message;
                }
                else
                {
                    errorMessage = $"Lỗi SMTP: {smtpEx.Message}\nStatus Code: {smtpEx.StatusCode}";
                }
                
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("LỖI GỬI EMAIL:");
                System.Diagnostics.Debug.WriteLine(detailedError);
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════");
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                // Log lỗi khác
                errorMessage = $"Lỗi: {ex.GetType().Name}\nMessage: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nInner Exception: {ex.InnerException.Message}";
                }
                
                string errorDetails = errorMessage + $"\nStack Trace: {ex.StackTrace}";
                
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════");
                System.Diagnostics.Debug.WriteLine("LỖI GỬI EMAIL:");
                System.Diagnostics.Debug.WriteLine(errorDetails);
                System.Diagnostics.Debug.WriteLine("═══════════════════════════════════");
                return (false, errorMessage);
            }
        }

        /// <summary>
        /// Tạo nội dung email HTML
        /// </summary>
        private string GenerateEmailBody(string otpCode, string userName)
        {
            var displayName = string.IsNullOrWhiteSpace(userName) ? "Người dùng" : userName;
            
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #4CCBA0 0%, #01958D 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .otp-box {{ background: white; border: 2px dashed #4CCBA0; padding: 20px; text-align: center; margin: 20px 0; border-radius: 5px; }}
        .otp-code {{ font-size: 32px; font-weight: bold; color: #01958D; letter-spacing: 5px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
        .warning {{ color: #d9534f; font-size: 14px; margin-top: 15px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>HealthApp</h1>
            <p>Khôi Phục Mật Khẩu</p>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{displayName}</strong>,</p>
            <p>Chúng tôi nhận được yêu cầu khôi phục mật khẩu cho tài khoản của bạn.</p>
            <p>Vui lòng sử dụng mã OTP sau để đặt lại mật khẩu:</p>
            
            <div class='otp-box'>
                <p style='margin: 0 0 10px 0; color: #666;'>Mã OTP của bạn:</p>
                <div class='otp-code'>{otpCode}</div>
            </div>
            
            <p class='warning'>
                <strong>⚠️ Lưu ý:</strong> Mã OTP này có hiệu lực trong <strong>15 phút</strong>.
                Nếu bạn không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua email này.
            </p>
            
            <p>Trân trọng,<br><strong>Đội ngũ HealthApp</strong></p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
            <p>&copy; 2024 HealthApp. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}

