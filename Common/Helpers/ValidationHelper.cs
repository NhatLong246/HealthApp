using System;
using System.Text.RegularExpressions;

namespace HealthApp.Common.Helpers
{
    /// <summary>
    /// Helper class cho validation
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Kiểm tra email có đúng format @gmail.com không
        /// </summary>
        public static bool IsValidGmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Kiểm tra format email và phải là @gmail.com
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
            return Regex.IsMatch(email.Trim(), emailPattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Kiểm tra số điện thoại có đúng 10 chữ số không
        /// </summary>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Chỉ chứa số và đúng 10 chữ số
            var phonePattern = @"^\d{10}$";
            return Regex.IsMatch(phoneNumber.Trim(), phonePattern);
        }

        /// <summary>
        /// Kiểm tra người dùng có đủ 13 tuổi không
        /// </summary>
        public static bool IsAtLeast13YearsOld(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
                return false;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;

            // Kiểm tra nếu chưa đến sinh nhật năm nay
            if (birthDate.Value.Date > today.AddYears(-age))
                age--;

            return age >= 13;
        }

        /// <summary>
        /// Kiểm tra mật khẩu và xác nhận mật khẩu có khớp không
        /// </summary>
        public static bool PasswordsMatch(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                return false;

            return password == confirmPassword;
        }

        /// <summary>
        /// Validate độ mạnh mật khẩu (tùy chọn - có thể mở rộng)
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Ít nhất 6 ký tự
            if (password.Length < 6)
                return false;

            return true;
        }
    }
}

