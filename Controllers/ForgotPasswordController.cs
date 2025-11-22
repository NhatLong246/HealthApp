using System;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Repositories;
using HealthApp.Repositories.Interfaces;
using HealthApp.Services;
using HealthApp.Services.Interfaces;

namespace HealthApp.Controllers
{
    /// <summary>
    /// Controller xử lý quên mật khẩu
    /// </summary>
    public class ForgotPasswordController : IDisposable
    {
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly WF_HealthTracker _dbContext;

        public ForgotPasswordController()
        {
            _dbContext = new WF_HealthTracker();
            IUserRepository userRepository = new UserRepository(_dbContext);
            IEmailService emailService = new EmailService();
            _forgotPasswordService = new ForgotPasswordService(userRepository, emailService);
        }

        /// <summary>
        /// Gửi mã OTP đến email
        /// </summary>
        public async Task<SendOTPResult> SendOTPAsync(string email)
        {
            return await _forgotPasswordService.SendOTPAsync(email);
        }

        /// <summary>
        /// Xác thực mã OTP
        /// </summary>
        public async Task<VerifyOTPResult> VerifyOTPAsync(string email, string otpCode)
        {
            return await _forgotPasswordService.VerifyOTPAsync(email, otpCode);
        }

        /// <summary>
        /// Đặt lại mật khẩu mới
        /// </summary>
        public async Task<ResetPasswordResult> ResetPasswordAsync(string email, string newPassword, string confirmPassword)
        {
            return await _forgotPasswordService.ResetPasswordAsync(email, newPassword, confirmPassword);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}

