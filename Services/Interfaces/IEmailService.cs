using System.Threading.Tasks;

namespace HealthApp.Services.Interfaces
{
    /// <summary>
    /// Interface cho Email Service
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email OTP đến địa chỉ email
        /// </summary>
        /// <param name="toEmail">Địa chỉ email người nhận</param>
        /// <param name="otpCode">Mã OTP cần gửi</param>
        /// <param name="userName">Tên người dùng (tùy chọn)</param>
        /// <returns>Tuple (Success, ErrorMessage)</returns>
        Task<(bool Success, string ErrorMessage)> SendOTPEmailAsync(string toEmail, string otpCode, string userName = null);
    }
}

