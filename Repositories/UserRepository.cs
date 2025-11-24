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

            string trimmedEmail = email.Trim().ToLower();
            
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Searching for email: '{trimmedEmail}'");
            
            return Task.Run(() =>
            {
                try
                {
                    // Query case-insensitive và trim
                    var users = _context.Users.ToList(); // Load all để tránh EF translation issues
                    
                    // Debug: Log tất cả users và emails
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Total users loaded: {users.Count}");
                    foreach (var u in users)
                    {
                        string emailInfo = u.Email != null ? $"'{u.Email}' (len={u.Email.Length})" : "NULL";
                        System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] User: {u.Username}, Email: {emailInfo}");
                    }
                    
                    // Filter với case-insensitive và trim
                    var user = users.FirstOrDefault(u => 
                        u.Email != null && 
                        !string.IsNullOrWhiteSpace(u.Email) &&
                        u.Email.Trim().ToLower() == trimmedEmail);
                    
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Searching for: '{trimmedEmail}'");
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Found user: {(user != null ? user.Username : "NULL")}");
                    if (user != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] User Email in DB: '{user.Email}'");
                        System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] User Email trimmed/lower: '{user.Email.Trim().ToLower()}'");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] No user found matching email '{trimmedEmail}'");
                    }
                    
                    return user;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[GetByEmailAsync] Stack: {ex.StackTrace}");
                    throw;
                }
            });
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
            // Tạm thời disable vì SDT đang bị ignore trong mapping
            // Sau khi thêm column SDT vào database và bỏ ignore, uncomment dòng dưới
            // return Task.Run(() => _context.Users
            //     .Any(u => u.SDT == phoneNumber));
            
            // Tạm thời return false để tránh lỗi
            return Task.FromResult(false);
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
