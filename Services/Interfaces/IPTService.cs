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
        /// Lấy danh sách lịch đặt (DatLichPT) đã được xác nhận của một PT trong ngày
        /// </summary>
        Task<IList<DatLichPT>> GetConfirmedBookingsForPTOnDateAsync(string ptId, DateTime date);
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

