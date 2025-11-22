using System;
using System.Linq;
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
    /// Controller xử lý logic Đăng ký
    /// </summary>
    public class RegisterController
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly WF_HealthTracker _dbContext;

        public RegisterController()
        {
            _dbContext = new WF_HealthTracker();
            _userRepository = new UserRepository(_dbContext);
            _authService = new AuthService(_userRepository);
        }

        /// <summary>
        /// Thực hiện đăng ký với validation đầy đủ
        /// </summary>
        public async Task<RegisterResult> RegisterAsync(
            string username,
            string password,
            string confirmPassword,
            string email,
            string phoneNumber,
            string fullName,
            DateTime? birthDate,
            string gender,
            string address)
        {
            try
            {
                // Validation: Tên đăng nhập
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập tên đăng nhập!",
                        FieldToFocus = "username"
                    };
                }

                // Validation: Mật khẩu
                if (string.IsNullOrWhiteSpace(password))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập mật khẩu!",
                        FieldToFocus = "password"
                    };
                }

                if (!ValidationHelper.IsValidPassword(password))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Mật khẩu phải có ít nhất 6 ký tự!",
                        FieldToFocus = "password"
                    };
                }

                // Validation: Xác nhận mật khẩu
                if (string.IsNullOrWhiteSpace(confirmPassword))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vui lòng xác nhận mật khẩu!",
                        FieldToFocus = "confirmPassword"
                    };
                }

                if (!ValidationHelper.PasswordsMatch(password, confirmPassword))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Mật khẩu và xác nhận mật khẩu không khớp!",
                        FieldToFocus = "confirmPassword"
                    };
                }

                // Validation: Email
                if (string.IsNullOrWhiteSpace(email))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập email!",
                        FieldToFocus = "email"
                    };
                }

                if (!ValidationHelper.IsValidGmail(email))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Email phải là địa chỉ @gmail.com!",
                        FieldToFocus = "email"
                    };
                }

                // Validation: Số điện thoại
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập số điện thoại!",
                        FieldToFocus = "phoneNumber"
                    };
                }

                if (!ValidationHelper.IsValidPhoneNumber(phoneNumber))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Số điện thoại phải có đúng 10 chữ số!",
                        FieldToFocus = "phoneNumber"
                    };
                }

                // Validation: Họ tên
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập họ tên!",
                        FieldToFocus = "fullName"
                    };
                }

                // Validation: Ngày sinh và tuổi
                if (!birthDate.HasValue)
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập ngày sinh!",
                        FieldToFocus = "birthDate"
                    };
                }

                if (!ValidationHelper.IsAtLeast13YearsOld(birthDate))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Bạn phải đủ 13 tuổi để đăng ký!",
                        FieldToFocus = "birthDate"
                    };
                }

                // Validation: Giới tính
                if (string.IsNullOrWhiteSpace(gender))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Vui lòng chọn giới tính!",
                        FieldToFocus = "gender"
                    };
                }

                // Kiểm tra username đã tồn tại
                if (await _userRepository.UsernameExistsAsync(username.Trim()))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Tên đăng nhập đã tồn tại!",
                        FieldToFocus = "username"
                    };
                }

                // Kiểm tra email đã tồn tại (case-sensitive - phân biệt hoa thường)
                // Email phân biệt hoa thường: Test@gmail.com và test@gmail.com là 2 email khác nhau
                string trimmedEmail = email.Trim();
                if (await _userRepository.EmailExistsAsync(trimmedEmail))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Email này đã được sử dụng để đăng ký tài khoản khác. Mỗi email chỉ có thể đăng ký một tài khoản!",
                        FieldToFocus = "email"
                    };
                }

                // Kiểm tra số điện thoại đã tồn tại
                if (await _userRepository.PhoneExistsAsync(phoneNumber.Trim()))
                {
                    return new RegisterResult
                    {
                        Success = false,
                        Message = "Số điện thoại đã được sử dụng!",
                        FieldToFocus = "phoneNumber"
                    };
                }

                // Tạo UserID tự động
                string userId = GenerateUserId();

                // Tạo user mới
                // Lưu email đúng như người dùng nhập (phân biệt hoa thường)
                var newUser = new Users
                {
                    UserID = userId,
                    Username = username.Trim(),
                    Email = trimmedEmail, // Lưu email đúng như người dùng nhập
                    SDT = phoneNumber.Trim(),
                    HoTen = fullName.Trim(),
                    NgaySinh = birthDate.Value,
                    GioiTinh = gender,
                    Role = "Client",
                    Theme = "Light",
                    NgonNgu = "vi",
                    CreatedDate = DateTime.Now
                };

                // Hash password và set vào user
                newUser = await _authService.RegisterAsync(newUser, password);

                // Lưu vào database
                await _userRepository.CreateAsync(newUser);

                return new RegisterResult
                {
                    Success = true,
                    Message = "Đăng ký thành công!",
                    User = newUser
                };
            }
            catch (Exception ex)
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Tạo UserID tự động (format: user_0001, user_0002, ...)
        /// </summary>
        private string GenerateUserId()
        {
            // Lấy user có UserID lớn nhất
            var lastUser = _dbContext.Users
                .OrderByDescending(u => u.UserID)
                .FirstOrDefault();

            if (lastUser == null)
            {
                return "user_0001";
            }

            // Extract số từ UserID cuối cùng
            if (lastUser.UserID.StartsWith("user_"))
            {
                string numberPart = lastUser.UserID.Substring(5);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    int newNumber = lastNumber + 1;
                    return $"user_{newNumber:D4}";
                }
            }

            // Fallback: đếm số lượng user và tạo ID mới
            int userCount = _dbContext.Users.Count();
            return $"user_{(userCount + 1):D4}";
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
    /// Kết quả đăng ký
    /// </summary>
    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FieldToFocus { get; set; }
        public Users User { get; set; }
    }
}

