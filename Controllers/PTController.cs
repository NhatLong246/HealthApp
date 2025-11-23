extern alias ef6;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ef6::System.Data.Entity;
using HealthApp.Models;
using HealthApp.Services;
using HealthApp.Services.Interfaces;

namespace HealthApp.Controllers
{
    /// <summary>
    /// Controller xử lý logic đăng ký PT
    /// </summary>
    public class PTController
    {
        private readonly IPTService _ptService;
        private readonly WF_HealthTracker _dbContext;

        public PTController()
        {
            _dbContext = new WF_HealthTracker();
            _ptService = new PTService(_dbContext);
        }

        /// <summary>
        /// Đăng ký trở thành PT
        /// </summary>
        public async Task<PTRegistrationResult> RegisterPTAsync(
            string soCCCD,
            string noiCap,
            DateTime? ngayCap,
            string anhChanDungPath,
            string anhCCCDPath,
            string bangCapPath,
            string chuyenMon = null,
            string chungChi = null,
            int? soNamKinhNghiem = null,
            double? giaTheoGio = null,
            string thanhPho = null)
        {
            try
            {
                // Validation - kiểm tra các trường bắt buộc
                if (string.IsNullOrWhiteSpace(soCCCD))
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập số CCCD!"
                    };
                }

                if (string.IsNullOrWhiteSpace(noiCap))
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Vui lòng nhập nơi cấp CCCD!"
                    };
                }

                if (ngayCap == null)
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Vui lòng chọn ngày cấp CCCD!"
                    };
                }

                // Validation - kiểm tra ảnh chân dung và ảnh CCCD
                if (string.IsNullOrWhiteSpace(anhChanDungPath))
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Vui lòng chọn ảnh chân dung!"
                    };
                }

                if (string.IsNullOrWhiteSpace(anhCCCDPath))
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Vui lòng chọn ảnh CCCD!"
                    };
                }

                // Kiểm tra user đã đăng nhập chưa
                if (!Common.Helpers.CurrentUser.IsLoggedIn)
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Vui lòng đăng nhập trước khi đăng ký!"
                    };
                }

                var userId = Common.Helpers.CurrentUser.UserID;

                // Kiểm tra đã đăng ký PT chưa
                var isRegistered = await _ptService.IsPTRegisteredAsync(userId);
                if (isRegistered)
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Bạn đã đăng ký làm PT rồi!"
                    };
                }

                // Tạo đối tượng HuanLuyenVien
                var huanLuyenVien = new HuanLuyenVien
                {
                    AnhChanDung = anhChanDungPath,
                    AnhDaiDien = anhChanDungPath, // Sử dụng ảnh chân dung làm ảnh đại diện
                    AnhCCCD = anhCCCDPath,
                    FileTaiLieu = bangCapPath,
                    ChuyenMon = chuyenMon,
                    ChungChi = chungChi,
                    SoNamKinhNghiem = soNamKinhNghiem,
                    GiaTheoGio = giaTheoGio,
                    ThanhPho = thanhPho,
                    // Lưu thông tin CCCD vào TieuSu dưới dạng JSON
                    TieuSu = $"{{\"SoCCCD\":\"{soCCCD}\",\"NoiCap\":\"{noiCap}\",\"NgayCap\":\"{ngayCap:yyyy-MM-dd}\"}}"
                };

                // Gọi service để đăng ký
                var result = await _ptService.RegisterPTAsync(huanLuyenVien, userId);

                if (result.Success)
                {
                    // Cập nhật CurrentUser để refresh Role
                    var user = _dbContext.Users.Find(userId);
                    if (user != null)
                    {
                        Common.Helpers.CurrentUser.User = user;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new PTRegistrationResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Lưu file và trả về đường dẫn
        /// </summary>
        public string SaveFile(string sourcePath, string fileName, string folderName = "PTDocuments")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    return null;

                // Tạo thư mục nếu chưa có
                var appDirectory = Application.StartupPath;
                var targetFolder = Path.Combine(appDirectory, "Resources", folderName);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                // Tạo tên file unique
                var extension = Path.GetExtension(sourcePath);
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}{extension}";
                var targetPath = Path.Combine(targetFolder, uniqueFileName);

                // Copy file
                File.Copy(sourcePath, targetPath, true);

                // Trả về đường dẫn relative
                return Path.Combine(folderName, uniqueFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}

