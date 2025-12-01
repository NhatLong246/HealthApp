using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Models;
using HealthApp.Repositories;
using HealthApp.Repositories.Interfaces;
using HealthApp.Services;
using HealthApp.Services.Interfaces;
using HealthApp.Common.Helpers;

namespace HealthApp.Controllers
{
    /// <summary>
    /// Controller xử lý logic Authentication
    /// </summary>
    public class AuthController
    {
        private readonly IAuthService _authService;
        private readonly WF_HealthTracker _dbContext;

        public AuthController()
        {
            try
            {
                _dbContext = new WF_HealthTracker();
                // Test connection
                _dbContext.Database.Connection.Open();
                _dbContext.Database.Connection.Close();
                
                IUserRepository userRepository = new UserRepository(_dbContext);
                _authService = new AuthService(userRepository);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể kết nối đến database. Vui lòng kiểm tra connection string trong App.config.\n\nChi tiết: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thực hiện đăng nhập
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Kết quả đăng nhập (true nếu thành công, false nếu thất bại)</returns>
        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập tên đăng nhập!",
                        FieldToFocus = "username"
                    };
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập mật khẩu!",
                        FieldToFocus = "password"
                    };
                }

                // Thực hiện đăng nhập
                var user = await _authService.LoginAsync(username.Trim(), password);

                if (user != null)
                {
                    // Lưu user vào session
                    CurrentUser.User = user;
                    
                    System.Diagnostics.Debug.WriteLine($"=== Login Success ===");
                    System.Diagnostics.Debug.WriteLine($"User.UserID: '{user.UserID}'");
                    System.Diagnostics.Debug.WriteLine($"User.Username: '{user.Username}'");
                    System.Diagnostics.Debug.WriteLine($"User.Role: '{user.Role}'");
                    System.Diagnostics.Debug.WriteLine($"CurrentUser.UserID: '{CurrentUser.UserID}'");
                    System.Diagnostics.Debug.WriteLine($"CurrentUser.Role: '{CurrentUser.Role}'");
                    System.Diagnostics.Debug.WriteLine($"CurrentUser.IsLoggedIn: {CurrentUser.IsLoggedIn}");

                    return new LoginResult
                    {
                        Success = true,
                        Message = $"Đăng nhập thành công!\nXin chào, {user.HoTen ?? user.Username}!",
                        User = user
                    };
                }
                else
                {
                    return new LoginResult
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng!",
                        FieldToFocus = "password"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Kiểm tra đã đăng nhập chưa
        /// </summary>
        public bool IsLoggedIn()
        {
            return CurrentUser.IsLoggedIn;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        public void Logout()
        {
            CurrentUser.Logout();
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }

    /// <summary>
    /// Kết quả đăng nhập
    /// </summary>
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FieldToFocus { get; set; }
        public Users User { get; set; }
    }
}

