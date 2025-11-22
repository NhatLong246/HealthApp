using System;
using System.Security.Cryptography;

namespace HealthApp.Common.Helpers
{
    /// <summary>
    /// Helper class để generate và validate OTP
    /// </summary>
    public static class OTPHelper
    {
        /// <summary>
        /// Generate mã OTP 6 chữ số ngẫu nhiên
        /// </summary>
        public static string GenerateOTP()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var random = BitConverter.ToUInt32(bytes, 0);
                // Tạo OTP 6 chữ số (000000 - 999999)
                return (random % 1000000).ToString("D6");
            }
        }

        /// <summary>
        /// Kiểm tra OTP có hợp lệ không (so sánh không phân biệt hoa thường)
        /// </summary>
        public static bool ValidateOTP(string inputOTP, string storedOTP)
        {
            if (string.IsNullOrWhiteSpace(inputOTP) || string.IsNullOrWhiteSpace(storedOTP))
                return false;

            return inputOTP.Trim() == storedOTP.Trim();
        }

        /// <summary>
        /// Kiểm tra OTP có hết hạn không (thời gian hết hạn mặc định là 15 phút)
        /// </summary>
        public static bool IsOTPExpired(DateTime? expiryTime, int expirationMinutes = 15)
        {
            if (!expiryTime.HasValue)
                return true;

            return DateTime.Now > expiryTime.Value.AddMinutes(expirationMinutes);
        }
    }
}

