using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Models;

namespace HealthApp.Services.Interfaces
{
    /// <summary>
    /// Service interface cho PT Dashboard operations
    /// </summary>
    public interface IPTDashboardService
    {
        /// <summary>
        /// Lấy PTID từ UserID
        /// </summary>
        Task<string> GetPTIDByUserIDAsync(string userId);

        /// <summary>
        /// Lấy tổng số khách hàng của PT
        /// </summary>
        Task<int> GetTotalCustomersAsync(string ptId);

        /// <summary>
        /// Lấy số buổi tập hôm nay của PT
        /// </summary>
        Task<int> GetTodaySessionsAsync(string ptId);

        /// <summary>
        /// Lấy thu nhập tháng này của PT
        /// </summary>
        Task<double> GetMonthlyIncomeAsync(string ptId);

        /// <summary>
        /// Lấy điểm đánh giá trung bình của PT
        /// </summary>
        Task<double> GetAverageRatingAsync(string ptId);

        /// <summary>
        /// Lấy danh sách yêu cầu thuê PT (Pending, chưa có PTID)
        /// </summary>
        Task<List<PTRequestViewModel>> GetPTRequestsAsync(string ptId);

        /// <summary>
        /// Lấy danh sách khách hàng đang tập (Confirmed/Completed)
        /// </summary>
        Task<List<PTCustomerViewModel>> GetActiveCustomersAsync(string ptId);

        /// <summary>
        /// Lấy lịch trình hôm nay của PT
        /// </summary>
        Task<List<PTScheduleViewModel>> GetTodayScheduleAsync(string ptId);

        /// <summary>
        /// Đồng ý yêu cầu thuê PT
        /// </summary>
        Task<bool> AcceptRequestAsync(string datLichID, string ptId);

        /// <summary>
        /// Từ chối/Xóa yêu cầu thuê PT
        /// </summary>
        Task<bool> RejectRequestAsync(string datLichID);

        /// <summary>
        /// Tạo yêu cầu tập luyện mới (buổi tập đơn lẻ)
        /// </summary>
        Task<string> CreateTrainingRequestAsync(string khachHangID, string ptId, DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc, string mucTieu);

        /// <summary>
        /// Tạo nhiều yêu cầu tập luyện cùng một lịch trình (package)
        /// </summary>
        /// <param name="khachHangID">ID khách hàng</param>
        /// <param name="ptId">ID PT</param>
        /// <param name="danhSachNgayGio">Danh sách (ngày, giờ bắt đầu, giờ kết thúc)</param>
        /// <param name="mucTieu">Mục tiêu luyện tập</param>
        /// <returns>Danh sách DatLichID đã tạo và LichTrinhID chung</returns>
        Task<(List<string> datLichIDs, string lichTrinhID)> CreateTrainingScheduleAsync(
            string khachHangID, 
            string ptId, 
            List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)> danhSachNgayGio, 
            string mucTieu);

        /// <summary>
        /// Kiểm tra trùng lịch với các lịch đã có của PT
        /// </summary>
        /// <param name="ptId">ID PT</param>
        /// <param name="danhSachNgayGio">Danh sách (ngày, giờ bắt đầu, giờ kết thúc) cần kiểm tra</param>
        /// <returns>Danh sách các lịch bị trùng (ngày, giờ bắt đầu, giờ kết thúc) hoặc null nếu không trùng</returns>
        Task<List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)>> CheckOverlappingSchedulesAsync(
            string ptId,
            List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)> danhSachNgayGio);

        /// <summary>
        /// Kiểm tra trùng lịch với lịch tập của khách hàng (BuoiTap trong KeHoachLuyenTap)
        /// </summary>
        /// <param name="khachHangID">ID khách hàng</param>
        /// <param name="danhSachNgayGio">Danh sách (ngày, giờ bắt đầu, giờ kết thúc) cần kiểm tra</param>
        /// <returns>Danh sách các lịch bị trùng với thông tin chi tiết (ngày, giờ bắt đầu, giờ kết thúc, tên kế hoạch)</returns>
        Task<List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc, string tenKeHoach)>> CheckOverlappingWithCustomerWorkoutAsync(
            string khachHangID,
            List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)> danhSachNgayGio);
    }

    /// <summary>
    /// ViewModel cho yêu cầu thuê PT
    /// </summary>
    public class PTRequestViewModel
    {
        public string DatLichID { get; set; }
        public string KhachHangID { get; set; }
        public string TenKhachHang { get; set; }
        public string AnhDaiDien { get; set; }
        public string MucTieu { get; set; }
        public DateTime NgayGioDat { get; set; }
        public string ThoiGian { get; set; }
        public int? ThoiLuong { get; set; }
        public string LichTrinhID { get; set; } // ID của lịch trình (nếu có)
    }

    /// <summary>
    /// ViewModel cho khách hàng đang tập
    /// </summary>
    public class PTCustomerViewModel
    {
        public string DatLichID { get; set; }
        public string KhachHangID { get; set; }
        public string TenKhachHang { get; set; }
        public string AnhDaiDien { get; set; }
        public DateTime NgayGioDat { get; set; }
        public string ThoiGian { get; set; }
        public string TrangThai { get; set; }
    }

    /// <summary>
    /// ViewModel cho lịch trình
    /// </summary>
    public class PTScheduleViewModel
    {
        public string DatLichID { get; set; }
        public string KhachHangID { get; set; }
        public string TenKhachHang { get; set; }
        public string LoaiBuoiTap { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string ThoiGian { get; set; }
        public string TrangThai { get; set; }
    }
}

