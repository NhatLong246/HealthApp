using System.Threading.Tasks;
using HealthApp.Models;

namespace HealthApp.Services.Interfaces
{
    /// <summary>
    /// Service interface cho Authentication operations
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Đăng nhập với username và password
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>User object nếu đăng nhập thành công, null nếu thất bại</returns>
        Task<Users> LoginAsync(string username, string password);

        /// <summary>
        /// Kiểm tra mật khẩu có đúng không
        /// </summary>
        bool VerifyPassword(string password, string hashedPassword);

        /// <summary>
        /// Đăng ký user mới
        /// </summary>
        Task<Users> RegisterAsync(Users user, string password);
    }
}

