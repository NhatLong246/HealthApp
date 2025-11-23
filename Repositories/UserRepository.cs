extern alias ef6;

using System;
using System.Linq;
using System.Threading.Tasks;
using ef6::System.Data.Entity;
using HealthApp.Models;
using HealthApp.Repositories.Interfaces;

namespace HealthApp.Repositories
{
    /// <summary>
    /// Repository implementation cho User operations
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly WF_HealthTracker _context;

        public UserRepository(WF_HealthTracker context)
        {
            _context = context;
        }

        public Task<Users> GetByUsernameAsync(string username)
        {
            return Task.Run(() => _context.Users
                .FirstOrDefault(u => u.Username == username));
        }

        public Task<Users> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Task.FromResult<Users>(null);

            string trimmedEmail = email.Trim();
            return Task.Run(() => _context.Users
                .FirstOrDefault(u => u.Email != null && u.Email.Trim() == trimmedEmail));
        }

        public Task<Users> GetByIdAsync(string userId)
        {
            return Task.Run(() => _context.Users
                .FirstOrDefault(u => u.UserID == userId));
        }

        public Task<bool> UsernameExistsAsync(string username)
        {
            return Task.Run(() => _context.Users
                .Any(u => u.Username == username));
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Task.FromResult(false);

            string trimmedEmail = email.Trim();
            return Task.Run(() => _context.Users
                .Any(u => u.Email != null && u.Email.Trim() == trimmedEmail));
        }

        public Task<bool> PhoneExistsAsync(string phoneNumber)
        {
            return Task.Run(() => _context.Users
                .Any(u => u.SDT == phoneNumber));
        }

        public async Task<Users> CreateAsync(Users user)
        {
            return await Task.Run(() =>
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            });
        }

        public async Task<bool> UpdateResetTokenAsync(string email, string resetToken, DateTime? expiryTime)
        {
            return await Task.Run(() =>
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    return false;

                user.ResetToken = resetToken;
                user.ResetTokenExpiry = expiryTime;
                _context.SaveChanges();
                return true;
            });
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPasswordHash)
        {
            return await Task.Run(() =>
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    return false;

                user.PasswordHash = newPasswordHash;
                user.ResetToken = null; // Xóa token sau khi đổi mật khẩu thành công
                user.ResetTokenExpiry = null;
                _context.SaveChanges();
                return true;
            });
        }
    }
}
