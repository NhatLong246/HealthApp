using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;
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

                var normalizedCCCD = soCCCD.Trim();
                if (!Regex.IsMatch(normalizedCCCD, @"^\d{12}$"))
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Số CCCD phải gồm đúng 12 chữ số!"
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

                // Kiểm tra số CCCD đã được sử dụng chưa
                var isCCCDUsed = await _ptService.IsCCCDRegisteredAsync(normalizedCCCD);
                if (isCCCDUsed)
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = "Số CCCD này đã được sử dụng để đăng ký PT trước đó!"
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
                    TieuSu = $"{{\"SoCCCD\":\"{normalizedCCCD}\",\"NoiCap\":\"{noiCap}\",\"NgayCap\":\"{ngayCap:yyyy-MM-dd}\"}}"
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

        /// <summary>
        /// Kiểm tra số CCCD đã được sử dụng để đăng ký PT hay chưa
        /// </summary>
        public Task<bool> IsCCCDAlreadyUsedAsync(string soCCCD)
        {
            return _ptService.IsCCCDRegisteredAsync(soCCCD);
        }

        /// <summary>
        /// Lấy danh sách lịch đặt đã Confirmed của PT hiện tại trong một ngày (theo CurrentUser)
        /// </summary>
        public async Task<IList<DatLichPT>> GetBookingsForCurrentPTOnDateAsync(DateTime date)
        {
           if (!Common.Helpers.CurrentUser.IsLoggedIn || Common.Helpers.CurrentUser.User == null)
           {
               return new DatLichPT[0];
           }

           var userId = Common.Helpers.CurrentUser.UserID;

           // Tìm PT tương ứng với user hiện tại
           HuanLuyenVien pt = null;
           try
           {
               // Dùng context tạm để tránh NullReference nếu _dbContext đã dispose ở nơi khác
               using (var tempContext = new WF_HealthTracker())
               {
                   pt = tempContext.HuanLuyenVien
                       .FirstOrDefault(h => h.UserID == userId);
               }
           }
           catch (Exception ex)
           {
               System.Diagnostics.Debug.WriteLine($"[PTController] Lỗi khi truy vấn PT hiện tại: {ex.Message}");
               return new DatLichPT[0];
           }

           if (pt == null || string.IsNullOrWhiteSpace(pt.PTID))
           {
               return new DatLichPT[0];
           }

           return await _ptService.GetConfirmedBookingsForPTOnDateAsync(pt.PTID, date);
        }

        public Task<IList<ThuVienBaiTap>> GetExercisesByGoalAsync(string goal)
        {
            return _ptService.GetExercisesByGoalAsync(goal);
        }

        public Task<IList<GiaoBaiTapChoUser>> GetAssignmentsForBookingsAsync(IEnumerable<DatLichPT> bookings)
        {
            var ids = bookings?
                .Where(b => !string.IsNullOrWhiteSpace(b.DatLichID))
                .Select(b => b.DatLichID)
                .ToList() ?? new List<string>();

            return _ptService.GetAssignmentsByDatLichIdsAsync(ids);
        }

        public Task<GiaoBaiTapChoUser> GetAssignmentAsync(string datLichId, string thuVienBaiTapId)
        {
            return _ptService.GetAssignmentAsync(datLichId, thuVienBaiTapId);
        }

        public Task ClearAssignmentsForBookingAsync(string datLichId)
        {
            return _ptService.ClearAssignmentsForBookingAsync(datLichId);
        }

        public async Task<GiaoBaiTapChoUser> SaveAssignmentAsync(
            DatLichPT booking,
            ThuVienBaiTap exercise,
            string customPayload,
            string customDescription = null)
        {
            if (booking == null || exercise == null)
                return null;

            if (string.IsNullOrWhiteSpace(booking.DatLichID) ||
                string.IsNullOrWhiteSpace(booking.KhachHangID))
                return null;

            var assignment = await _ptService.GetAssignmentAsync(booking.DatLichID, exercise.BaiTapID);
            var truncatedPayload = string.IsNullOrEmpty(customPayload)
                ? null
                : (customPayload.Length > 500 ? customPayload.Substring(0, 500) : customPayload);

            if (assignment == null)
            {
                assignment = new GiaoBaiTapChoUser
                {
                    PTID = booking.PTID,
                    UserID = booking.KhachHangID,
                    DatLichID = booking.DatLichID,
                    ThuVienBaiTapID = exercise.BaiTapID,
                    TieuDe = exercise.TenBaiTap,
                    MoTa = string.IsNullOrWhiteSpace(customDescription) ? exercise.MoTa : customDescription,
                    MucTieuBuoiTap = booking.MucTieuLuyenTap ?? exercise.LoaiMucTieu,
                    HanHoanThanh = booking.ThoiGianKetThuc,
                    GhiChuPT = truncatedPayload,
                    TrangThai = "Assigned"
                };

                return await _ptService.CreateAssignmentAsync(assignment);
            }

            assignment.TieuDe = exercise.TenBaiTap;
            assignment.MoTa = string.IsNullOrWhiteSpace(customDescription) ? exercise.MoTa : customDescription;
            assignment.MucTieuBuoiTap = booking.MucTieuLuyenTap ?? exercise.LoaiMucTieu;
            assignment.HanHoanThanh = booking.ThoiGianKetThuc;
            assignment.GhiChuPT = truncatedPayload;
            assignment.TrangThai = "Assigned";
            assignment.NgayHoanThanh = null;

            return await _ptService.UpdateAssignmentAsync(assignment);
        }

        public async Task<IList<GiaoBaiTapChoUser>> GetAssignmentsByPTAndDateAsync(DateTime date)
        {
            if (!Common.Helpers.CurrentUser.IsLoggedIn || Common.Helpers.CurrentUser.User == null)
                return new List<GiaoBaiTapChoUser>();

            string ptId = null;
            using (var tempContext = new WF_HealthTracker())
            {
                var pt = tempContext.HuanLuyenVien
                    .FirstOrDefault(h => h.UserID == Common.Helpers.CurrentUser.User.UserID);
                ptId = pt?.PTID;
            }

            if (string.IsNullOrWhiteSpace(ptId))
                return new List<GiaoBaiTapChoUser>();

            return await _ptService.GetAssignmentsByPTAndDateAsync(ptId, date);
        }

        public async Task<IList<GiaoBaiTapChoUser>> GetAssignmentsByUserAndDateAsync(DateTime date)
        {
            if (!Common.Helpers.CurrentUser.IsLoggedIn || Common.Helpers.CurrentUser.User == null)
                return new List<GiaoBaiTapChoUser>();

            var userId = Common.Helpers.CurrentUser.UserID;
            return await _ptService.GetAssignmentsByUserAndDateAsync(userId, date);
        }
    }
}

