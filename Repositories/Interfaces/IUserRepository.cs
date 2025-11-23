using System;
using System.Threading.Tasks;
using HealthApp.Models;

namespace HealthApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface cho User operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Tìm user theo username
        /// </summary>
        Task<Users> GetByUsernameAsync(string username);

        /// <summary>
        /// Tìm user theo email
        /// </summary>
        Task<Users> GetByEmailAsync(string email);

        /// <summary>
        /// Tìm user theo UserID
        /// </summary>
        Task<Users> GetByIdAsync(string userId);

        /// <summary>
        /// Kiểm tra username đã tồn tại chưa
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        Task<bool> PhoneExistsAsync(string phoneNumber);

        /// <summary>
        /// Tạo user mới
        /// </summary>
        Task<Users> CreateAsync(Users user);

        /// <summary>
        /// Cập nhật ResetToken và ResetTokenExpiry cho user
        /// </summary>
        Task<bool> UpdateResetTokenAsync(string email, string resetToken, DateTime? expiryTime);

        /// <summary>
        /// Cập nhật mật khẩu mới cho user
        /// </summary>
        Task<bool> UpdatePasswordAsync(string email, string newPasswordHash);
    }
}

