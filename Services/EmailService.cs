using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HealthApp.Services.Interfaces;
using HealthApp.Models;

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

        /// <summary>
        /// Gửi email thông báo lịch tập luyện
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> SendWorkoutNotificationEmailAsync(
            string toEmail, 
            string userName, 
            BuoiTap buoiTap, 
            int notificationType)
        {
            string errorMessage = string.Empty;
            
            try
            {
                // Kiểm tra email và password đã được cấu hình chưa
                if (_senderEmail == "your-email@gmail.com" || _senderPassword == "your-app-password")
                {
                    errorMessage = "Email và App Password chưa được cấu hình!";
                    System.Diagnostics.Debug.WriteLine("⚠️ LỖI: Email và App Password chưa được cấu hình!");
                    return (false, errorMessage);
                }

                if (buoiTap == null || string.IsNullOrWhiteSpace(toEmail))
                {
                    errorMessage = "Thông tin buổi tập hoặc email không hợp lệ!";
                    return (false, errorMessage);
                }

                // Load BaiTapChiTiet và ThuVienBaiTap nếu chưa có
                using (var dbContext = new WF_HealthTracker())
                {
                    var buoiTapFromDb = dbContext.BuoiTap
                        .FirstOrDefault(b => b.BuoiTapID == buoiTap.BuoiTapID);
                    
                    if (buoiTapFromDb != null)
                    {
                        dbContext.Entry(buoiTapFromDb)
                            .Collection(bt => bt.BaiTapChiTiet)
                            .Load();
                        
                        foreach (var baiTapChiTiet in buoiTapFromDb.BaiTapChiTiet)
                        {
                            if (baiTapChiTiet.ThuVienBaiTap == null && !string.IsNullOrEmpty(baiTapChiTiet.BaiTapID))
                            {
                                dbContext.Entry(baiTapChiTiet)
                                    .Reference(bt => bt.ThuVienBaiTap)
                                    .Load();
                            }
                        }
                        
                        buoiTap = buoiTapFromDb;
                    }
                }

                // Tạo nội dung email dựa trên loại thông báo
                string subject = "";
                string body = "";
                
                switch (notificationType)
                {
                    case 1: // Trước 1 ngày
                        subject = "Nhắc nhở: Lịch tập luyện ngày mai";
                        body = GenerateWorkoutReminderEmailBody(userName, buoiTap, notificationType);
                        break;
                    case 2: // Ngày tập
                        subject = "Thông báo: Lịch tập luyện hôm nay";
                        body = GenerateWorkoutReminderEmailBody(userName, buoiTap, notificationType);
                        break;
                    case 3: // Quá ngày tập
                        subject = "Nhắc nhở: Bạn đã bỏ lỡ lịch tập luyện";
                        body = GenerateWorkoutReminderEmailBody(userName, buoiTap, notificationType);
                        break;
                    default:
                        errorMessage = "Loại thông báo không hợp lệ!";
                        return (false, errorMessage);
                }

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = _enableSsl;
                    client.UseDefaultCredentials = false;
                    string cleanPassword = _senderPassword?.Replace(" ", "").Trim();
                    client.Credentials = new NetworkCredential(_senderEmail, cleanPassword);
                    client.Timeout = 30000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_senderEmail, "HealthApp"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await Task.Run(() => client.Send(mailMessage));
                    System.Diagnostics.Debug.WriteLine($"✅ Email thông báo lịch tập đã được gửi thành công đến {toEmail}!");
                    return (true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Lỗi khi gửi email thông báo lịch tập: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Lỗi gửi email thông báo lịch tập: {ex.Message}");
                return (false, errorMessage);
            }
        }

        /// <summary>
        /// Tạo nội dung email thông báo lịch tập luyện
        /// </summary>
        private string GenerateWorkoutReminderEmailBody(string userName, BuoiTap buoiTap, int notificationType)
        {
            var displayName = string.IsNullOrWhiteSpace(userName) ? "Người dùng" : userName;
            
            // Lấy thông tin thời gian
            string timeInfo = "";
            if (buoiTap.ThoiGianBatDau.HasValue && buoiTap.ThoiGianKetThuc.HasValue)
            {
                timeInfo = $"từ {buoiTap.ThoiGianBatDau.Value:HH:mm} đến {buoiTap.ThoiGianKetThuc.Value:HH:mm}";
            }
            else if (buoiTap.ThoiGianBatDau.HasValue)
            {
                timeInfo = $"vào lúc {buoiTap.ThoiGianBatDau.Value:HH:mm}";
            }
            else
            {
                timeInfo = "theo lịch đã đặt";
            }

            // Lấy danh sách bài tập
            string baiTapList = "";
            if (buoiTap.BaiTapChiTiet != null && buoiTap.BaiTapChiTiet.Count > 0)
            {
                var baiTapNames = buoiTap.BaiTapChiTiet
                    .Where(bt => bt.ThuVienBaiTap != null)
                    .Select(bt => bt.ThuVienBaiTap.TenBaiTap)
                    .ToList();
                
                if (baiTapNames.Count > 0)
                {
                    baiTapList = "<ul style='list-style-type: none; padding-left: 0;'>";
                    foreach (var tenBaiTap in baiTapNames)
                    {
                        baiTapList += $"<li style='padding: 8px 0; border-bottom: 1px solid #eee;'>✓ {tenBaiTap}</li>";
                    }
                    baiTapList += "</ul>";
                }
            }

            string mainMessage = "";
            string reminderMessage = "";
            
            switch (notificationType)
            {
                case 1: // Trước 1 ngày
                    mainMessage = $"Ngày mai bạn sẽ có bài tập luyện {timeInfo}.";
                    reminderMessage = "Đừng quên luyện tập nhé! Hãy chuẩn bị tinh thần và dụng cụ cần thiết để có một buổi tập hiệu quả.";
                    break;
                case 2: // Ngày tập
                    mainMessage = $"Hôm nay bạn có bài tập luyện {timeInfo}.";
                    reminderMessage = "Hôm nay bạn có lịch tập, đừng bỏ lỡ nhé! Hãy sắp xếp thời gian và thực hiện đầy đủ các bài tập để đạt được mục tiêu của bạn.";
                    break;
                case 3: // Quá ngày tập
                    mainMessage = $"Bạn đã bỏ lỡ lịch luyện tập {timeInfo}.";
                    reminderMessage = "Hãy luyện tập một cách chăm chỉ hơn để đạt được mục tiêu của bạn. Đừng để bỏ lỡ các buổi tập tiếp theo nhé!";
                    break;
            }

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
        .workout-box {{ background: white; border-left: 4px solid #4CCBA0; padding: 20px; margin: 20px 0; border-radius: 5px; }}
        .time-info {{ font-size: 18px; font-weight: bold; color: #01958D; margin: 15px 0; }}
        .exercise-list {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; }}
        .reminder {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; border-radius: 5px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>HealthApp</h1>
            <p>Thông Báo Lịch Tập Luyện</p>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{displayName}</strong>,</p>
            
            <div class='workout-box'>
                <p style='font-size: 16px; margin: 0 0 10px 0;'>{mainMessage}</p>
                <div class='time-info'>⏰ {timeInfo}</div>
            </div>
            
            {(string.IsNullOrWhiteSpace(baiTapList) ? "" : $@"
            <div class='exercise-list'>
                <h3 style='margin-top: 0; color: #01958D;'>Danh sách bài tập:</h3>
                {baiTapList}
            </div>")}
            
            <div class='reminder'>
                <p style='margin: 0;'><strong>💪 Lời nhắc:</strong> {reminderMessage}</p>
            </div>
            
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

        /// <summary>
        /// Gửi email thông báo cho PT khi có người gửi yêu cầu thuê PT
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> SendHireRequestToPTEmailAsync(
            string ptEmail,
            string ptName,
            string customerName,
            string scheduleSummary)
        {
            string errorMessage = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(ptEmail))
                    return (false, "Email PT không hợp lệ!");

                // SMTP config check
                if (_senderEmail == "your-email@gmail.com" || _senderPassword == "your-app-password")
                {
                    errorMessage = "Email và App Password chưa được cấu hình!";
                    return (false, errorMessage);
                }

                string subject = "HealthApp - Bạn có yêu cầu thuê PT mới";
                string body = GenerateHireRequestEmailBody(ptName, customerName, scheduleSummary);

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = _enableSsl;
                    client.UseDefaultCredentials = false;
                    string cleanPassword = _senderPassword?.Replace(" ", "").Trim();
                    client.Credentials = new NetworkCredential(_senderEmail, cleanPassword);
                    client.Timeout = 30000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_senderEmail, "HealthApp"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(ptEmail);

                    await Task.Run(() => client.Send(mailMessage));
                    System.Diagnostics.Debug.WriteLine($"✅ Email yêu cầu thuê PT đã được gửi đến {ptEmail}");
                    return (true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Lỗi khi gửi email yêu cầu thuê PT: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(errorMessage);
                return (false, errorMessage);
            }
        }

        private string GenerateHireRequestEmailBody(string ptName, string customerName, string scheduleSummary)
        {
            var displayPT = string.IsNullOrWhiteSpace(ptName) ? "PT" : ptName;
            var displayCustomer = string.IsNullOrWhiteSpace(customerName) ? "User" : customerName;
            var summary = string.IsNullOrWhiteSpace(scheduleSummary) ? "Có lịch tập mới" : scheduleSummary;

            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <style>
    body {{ font-family: Arial, sans-serif; background: #f6f8fb; color: #111827; }}
    .wrap {{ max-width: 640px; margin: 0 auto; padding: 20px; }}
    .card {{ background: white; border-radius: 14px; overflow: hidden; box-shadow: 0 10px 25px rgba(0,0,0,.06); }}
    .header {{ background: linear-gradient(135deg,#60a5fa,#2563eb); color: white; padding: 20px; }}
    .content {{ padding: 22px; }}
    .pill {{ display: inline-block; padding: 8px 12px; border-radius: 999px; background: #eff6ff; color: #1d4ed8; font-weight: 700; }}
    .box {{ margin-top: 14px; padding: 14px; border-radius: 12px; background: #f9fafb; border: 1px solid #e5e7eb; }}
    .footer {{ padding: 16px 22px; color: #6b7280; font-size: 12px; }}
  </style>
</head>
<body>
  <div class='wrap'>
    <div class='card'>
      <div class='header'>
        <h2 style='margin:0'>HealthApp</h2>
        <div style='margin-top:6px'>Bạn có yêu cầu thuê PT mới</div>
      </div>
      <div class='content'>
        <div style='margin-bottom:10px'>Xin chào <b>{displayPT}</b>,</div>
        <div class='box'>
          <div><span class='pill'>Yêu cầu mới</span></div>
          <div style='margin-top:12px'><b>{displayCustomer}</b> vừa gửi yêu cầu thuê PT.</div>
          <div style='margin-top:10px'><b>Lịch:</b> {summary}</div>
        </div>
        <div style='margin-top:16px'>Vui lòng mở ứng dụng HealthApp để xem chi tiết và xử lý yêu cầu.</div>
      </div>
      <div class='footer'>
        Email này được gửi tự động từ HealthApp, vui lòng không trả lời.
      </div>
    </div>
  </div>
</body>
</html>";
        }
    }
}

