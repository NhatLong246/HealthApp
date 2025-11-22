using System.Threading.Tasks;

namespace HealthApp.Services.Interfaces
{
    /// <summary>
    /// Result class cho SendOTP operation
    /// </summary>
    public class SendOTPResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Result class cho VerifyOTP operation
    /// </summary>
    public class VerifyOTPResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Result class cho ResetPassword operation
    /// </summary>
    public class ResetPasswordResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Interface cho Forgot Password Service
    /// </summary>
    public interface IForgotPasswordService
    {
        /// <summary>
        /// Gửi mã OTP đến email
        /// </summary>
        Task<SendOTPResult> SendOTPAsync(string email);

        /// <summary>
        /// Xác thực mã OTP
        /// </summary>
        Task<VerifyOTPResult> VerifyOTPAsync(string email, string otpCode);

        /// <summary>
        /// Đặt lại mật khẩu mới
        /// </summary>
        Task<ResetPasswordResult> ResetPasswordAsync(string email, string newPassword, string confirmPassword);
    }
}

