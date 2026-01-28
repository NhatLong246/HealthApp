using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Models;

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

        /// <summary>
        /// Gửi email thông báo lịch tập luyện
        /// </summary>
        /// <param name="toEmail">Địa chỉ email người nhận</param>
        /// <param name="userName">Tên người dùng</param>
        /// <param name="buoiTap">Buổi tập cần thông báo</param>
        /// <param name="notificationType">Loại thông báo: 1 = Trước 1 ngày, 2 = Ngày tập, 3 = Quá ngày tập</param>
        /// <returns>Tuple (Success, ErrorMessage)</returns>
        Task<(bool Success, string ErrorMessage)> SendWorkoutNotificationEmailAsync(
            string toEmail, 
            string userName, 
            BuoiTap buoiTap, 
            int notificationType);
    }
}

