using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace HealthApp.Common.Helpers
{
    /// <summary>
    /// Helper class để hash và verify mật khẩu sử dụng PBKDF2
    /// </summary>
    public static class PasswordHelper
    {
        private const int SaltSize = 128 / 8; // 16 bytes
        private const int HashSize = 256 / 8; // 32 bytes
        private const int IterationCount = 10000;

        /// <summary>
        /// Hash mật khẩu với salt ngẫu nhiên
        /// </summary>
        /// <param name="password">Mật khẩu cần hash</param>
        /// <returns>Chuỗi hash bao gồm salt và hash (format: salt:hash)</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // Tạo salt ngẫu nhiên
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash password với salt
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashSize
            );

            // Kết hợp salt và hash thành một chuỗi
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verify mật khẩu với hash đã lưu
        /// </summary>
        /// <param name="password">Mật khẩu cần verify</param>
        /// <param name="hashedPassword">Hash đã lưu trong database</param>
        /// <returns>True nếu mật khẩu đúng, False nếu sai</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                // Giải mã hash từ base64
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                if (hashBytes.Length != SaltSize + HashSize)
                    return false;

                // Tách salt và hash
                byte[] salt = new byte[SaltSize];
                byte[] storedHash = new byte[HashSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);
                Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

                // Hash password nhập vào với salt đã lưu
                byte[] computedHash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: IterationCount,
                    numBytesRequested: HashSize
                );

                // So sánh hash
                return SlowEquals(storedHash, computedHash);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// So sánh hai mảng byte một cách an toàn (constant-time comparison)
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }
}

