using System;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Repositories.Interfaces;
using HealthApp.Services.Interfaces;
using HealthApp.Common.Helpers;

namespace HealthApp.Services
{
    /// <summary>
    /// Service implementation cho Authentication
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Users> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // Tìm user theo username
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null)
                return null;

            // Verify password
            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return PasswordHelper.VerifyPassword(password, hashedPassword);
        }

        public async Task<Users> RegisterAsync(Users user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // Hash password
            user.PasswordHash = PasswordHelper.HashPassword(password);

            // Set default values
            if (string.IsNullOrEmpty(user.Role))
                user.Role = "Client";

            if (string.IsNullOrEmpty(user.Theme))
                user.Theme = "Light";

            if (string.IsNullOrEmpty(user.NgonNgu))
                user.NgonNgu = "vi";

            if (user.CreatedDate == null)
                user.CreatedDate = DateTime.Now;

            return user;
        }
    }
}

