using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Models;

namespace HealthApp.Services.Interfaces
{
    /// <summary>
    /// Service interface cho PT (Personal Trainer) operations
    /// </summary>
    public interface IPTService
    {
        /// <summary>
        /// Đăng ký trở thành PT
        /// </summary>
        /// <param name="huanLuyenVien">Thông tin huấn luyện viên</param>
        /// <param name="userId">UserID của người đăng ký</param>
        /// <returns>Kết quả đăng ký</returns>
        Task<PTRegistrationResult> RegisterPTAsync(HuanLuyenVien huanLuyenVien, string userId);

        /// <summary>
        /// Kiểm tra user đã đăng ký PT chưa
        /// </summary>
        Task<bool> IsPTRegisteredAsync(string userId);

        /// <summary>
        /// Kiểm tra số CCCD đã được sử dụng để đăng ký PT chưa
        /// </summary>
        Task<bool> IsCCCDRegisteredAsync(string soCCCD);

        /// <summary>
        /// Tạo PTID mới
        /// </summary>
        Task<string> GeneratePTIDAsync();

        /// <summary>
        /// Lấy danh sách bài tập theo mục tiêu
        /// </summary>
        Task<IList<ThuVienBaiTap>> GetExercisesByGoalAsync(string goal);

        /// <summary>
        /// Lấy danh sách lịch đặt (DatLichPT) đã được xác nhận của một PT trong ngày
        /// </summary>
        Task<IList<DatLichPT>> GetConfirmedBookingsForPTOnDateAsync(string ptId, DateTime date);

        /// <summary>
        /// Lưu bài tập giao cho user (tạo mới)
        /// </summary>
        Task<GiaoBaiTapChoUser> CreateAssignmentAsync(GiaoBaiTapChoUser assignment);

        /// <summary>
        /// Cập nhật bài tập giao cho user
        /// </summary>
        Task<GiaoBaiTapChoUser> UpdateAssignmentAsync(GiaoBaiTapChoUser assignment);

        /// <summary>
        /// Lấy danh sách bài tập đã giao theo list DatLichID
        /// </summary>
        Task<IList<GiaoBaiTapChoUser>> GetAssignmentsByDatLichIdsAsync(IEnumerable<string> datLichIds);

        /// <summary>
        /// Lấy bài tập đã giao cụ thể cho một lịch và template
        /// </summary>
        Task<GiaoBaiTapChoUser> GetAssignmentAsync(string datLichId, string thuVienBaiTapId);

        /// <summary>
        /// Xóa toàn bộ bài tập đã giao cho một lịch DatLichID (dùng khi giao lại từ đầu)
        /// </summary>
        Task ClearAssignmentsForBookingAsync(string datLichId);

        /// <summary>
        /// Lấy danh sách bài tập đã giao của một PT trong ngày cụ thể
        /// </summary>
        Task<IList<GiaoBaiTapChoUser>> GetAssignmentsByPTAndDateAsync(string ptId, DateTime date);

        /// <summary>
        /// Lấy danh sách bài tập đã giao cho một User trong ngày cụ thể (group theo buổi)
        /// </summary>
        Task<IList<GiaoBaiTapChoUser>> GetAssignmentsByUserAndDateAsync(string userId, DateTime date);
    }

    /// <summary>
    /// Kết quả đăng ký PT
    /// </summary>
    public class PTRegistrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public HuanLuyenVien HuanLuyenVien { get; set; }
    }
}

