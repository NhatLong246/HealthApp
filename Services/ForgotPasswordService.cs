using System;
using System.Threading.Tasks;
using HealthApp.Common.Helpers;
using HealthApp.Repositories.Interfaces;
using HealthApp.Services.Interfaces;

namespace HealthApp.Services
{
    /// <summary>
    /// Service xử lý logic quên mật khẩu
    /// </summary>
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public ForgotPasswordService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        /// <summary>
        /// Gửi mã OTP đến email
        /// </summary>
        public async Task<SendOTPResult> SendOTPAsync(string email)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(email))
            {
                return new SendOTPResult
                {
                    Success = false,
                    Message = "Vui lòng nhập địa chỉ email."
                };
            }

            email = email.Trim();

            // Kiểm tra email có tồn tại trong hệ thống không
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new SendOTPResult
                {
                    Success = false,
                    Message = "Email không tồn tại trong hệ thống."
                };
            }

            // Generate OTP
            string otpCode = OTPHelper.GenerateOTP();
            DateTime expiryTime = DateTime.Now;

            // Lưu OTP vào database (dùng ResetToken và ResetTokenExpiry)
            bool updateSuccess = await _userRepository.UpdateResetTokenAsync(email, otpCode, expiryTime);
            if (!updateSuccess)
            {
                return new SendOTPResult
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lưu mã OTP. Vui lòng thử lại."
                };
            }

            // Gửi email OTP
            var (emailSent, emailError) = await _emailService.SendOTPEmailAsync(email, otpCode, user.HoTen);
            if (!emailSent)
            {
                string errorMessage = "Không thể gửi email.\n\n";
                if (!string.IsNullOrEmpty(emailError))
                {
                    errorMessage += $"Chi tiết lỗi:\n{emailError}\n\n";
                }
                errorMessage += "Vui lòng:\n";
                errorMessage += "1. Kiểm tra cấu hình email trong EmailService.cs\n";
                errorMessage += "2. Đảm bảo đã tạo App Password cho Gmail\n";
                errorMessage += "3. Xem Output Window (View > Output) để biết chi tiết lỗi";
                
                return new SendOTPResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            return new SendOTPResult
            {
                Success = true,
                Message = "Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư."
            };
        }

        /// <summary>
        /// Xác thực mã OTP
        /// </summary>
        public async Task<VerifyOTPResult> VerifyOTPAsync(string email, string otpCode)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otpCode))
            {
                return new VerifyOTPResult
                {
                    Success = false,
                    Message = "Vui lòng nhập đầy đủ email và mã OTP."
                };
            }

            email = email.Trim();
            otpCode = otpCode.Trim();

            // Lấy user từ database
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new VerifyOTPResult
                {
                    Success = false,
                    Message = "Email không tồn tại trong hệ thống."
                };
            }

            // Kiểm tra có ResetToken không
            if (string.IsNullOrWhiteSpace(user.ResetToken))
            {
                return new VerifyOTPResult
                {
                    Success = false,
                    Message = "Mã OTP không hợp lệ hoặc đã hết hạn. Vui lòng yêu cầu mã mới."
                };
            }

            // Kiểm tra OTP có hết hạn không (15 phút)
            if (OTPHelper.IsOTPExpired(user.ResetTokenExpiry, 15))
            {
                return new VerifyOTPResult
                {
                    Success = false,
                    Message = "Mã OTP đã hết hạn. Vui lòng yêu cầu mã mới."
                };
            }

            // Kiểm tra OTP có khớp không
            if (!OTPHelper.ValidateOTP(otpCode, user.ResetToken))
            {
                return new VerifyOTPResult
                {
                    Success = false,
                    Message = "Mã OTP không đúng. Vui lòng kiểm tra lại."
                };
            }

            return new VerifyOTPResult
            {
                Success = true,
                Message = "Mã OTP hợp lệ."
            };
        }

        /// <summary>
        /// Đặt lại mật khẩu mới
        /// </summary>
        public async Task<ResetPasswordResult> ResetPasswordAsync(string email, string newPassword, string confirmPassword)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResetPasswordResult
                {
                    Success = false,
                    Message = "Vui lòng nhập địa chỉ email."
                };
            }

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return new ResetPasswordResult
                {
                    Success = false,
                    Message = "Vui lòng nhập đầy đủ mật khẩu mới và xác nhận mật khẩu."
                };
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp không
            if (!ValidationHelper.PasswordsMatch(newPassword, confirmPassword))
            {
                return new ResetPasswordResult
                {
                    Success = false,
                    Message = "Mật khẩu và xác nhận mật khẩu không khớp."
                };
            }

            // Kiểm tra độ mạnh mật khẩu
            if (!ValidationHelper.IsValidPassword(newPassword))
            {
                return new ResetPasswordResult
                {
                    Success = false,
                    Message = "Mật khẩu phải có ít nhất 6 ký tự."
                };
            }

            email = email.Trim();

            // Kiểm tra user có tồn tại không
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new ResetPasswordResult
                {
                    Success = false,
                    Message = "Email không tồn tại trong hệ thống."
                };
            }

            // Kiểm tra OTP đã được verify chưa (có ResetToken không)
            if (string.IsNullOrWhiteSpace(user.ResetToken))
            {
                return new ResetPasswordResult
                {
                    Success = false,
                    Message = "Vui lòng xác thực mã OTP trước khi đặt lại mật khẩu."
                };
            }

            // Hash mật khẩu mới
            string newPasswordHash = PasswordHelper.HashPassword(newPassword);

            // Cập nhật mật khẩu mới
            bool updateSuccess = await _userRepository.UpdatePasswordAsync(email, newPasswordHash);
            if (!updateSuccess)
            {
                return new ResetPasswordResult
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật mật khẩu. Vui lòng thử lại."
                };
            }

            return new ResetPasswordResult
            {
                Success = true,
                Message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới."
            };
        }
    }
}

