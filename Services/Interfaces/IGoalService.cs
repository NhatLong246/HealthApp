using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Models;

namespace HealthApp.Services.Interfaces
{
    public interface IGoalService
    {
        /// <summary>
        /// Lấy danh sách bài tập theo loại mục tiêu và trình độ
        /// </summary>
        Task<List<ThuVienBaiTap>> GetExercisesByGoalAndLevelAsync(string loaiMucTieu, string nhomCoChinhNhat, string searchBy, string capDo = null);

        /// <summary>
        /// Lấy tất cả bài tập từ thư viện
        /// </summary>
        Task<List<ThuVienBaiTap>> GetAllExercisesAsync();

        /// <summary>
        /// Lấy chi tiết bài tập theo ID
        /// </summary>
        Task<ThuVienBaiTap> GetExerciseDetailAsync(string baiTapId);

        /// <summary>
        /// Tạo kế hoạch luyện tập từ mục tiêu
        /// </summary>
        Task<KeHoachLuyenTap> CreateWorkoutPlanAsync(
            string userId,
            string mucTieuId,
            DateTime ngayBatDau,
            DateTime ngayKetThuc,
            string capDo,
            List<WeeklySchedule> weeklySchedules);

        /// <summary>
        /// Tạo các buổi tập từ kế hoạch
        /// </summary>
        Task<List<BuoiTap>> CreateWorkoutSessionsAsync(
            string keHoachTapId,
            List<WeeklySchedule> weeklySchedules,
            DateTime ngayBatDauMucTieu,
            DateTime ngayKetThucMucTieu);
    }

    /// <summary>
    /// Lịch tập trong tuần
    /// </summary>
    public class WeeklySchedule
    {
        public string ThuNgay { get; set; } // "Thứ 2", "Thứ 3", ...
        public TimeSpan? GioBatDau { get; set; }
        public TimeSpan? GioKetThuc { get; set; }
        public string GhiChu { get; set; }
    }
}

