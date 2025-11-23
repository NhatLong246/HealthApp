using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Repositories.Interfaces;
using HealthApp.Services.Interfaces;

namespace HealthApp.Services
{
    public class ReportService : IReportService
    {
        private readonly IUserRepository _userRepository;
        private readonly WF_HealthTracker _context;

        public ReportService(IUserRepository userRepository, WF_HealthTracker context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public Task<int> GetTotalSessionsAsync(string userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GetTotalSessionsAsync - UserID: {userId} ===");
                
                // Sử dụng raw SQL để tránh vấn đề pluralization
                var sql = @"
                    SELECT COUNT(*) 
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0 
                      AND b.TrangThai = N'Hoàn thành'";
                
                var count = _context.Database.SqlQuery<int>(sql, userId).FirstOrDefault();
                
                System.Diagnostics.Debug.WriteLine($"Total completed sessions: {count}");

                return Task.FromResult(count);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTotalSessionsAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                return Task.FromResult(0);
            }
        }

        public Task<double> GetTotalTimeAsync(string userId)
        {
            try
            {
                // Kiểm tra dữ liệu trước
                var checkSql = @"
                    SELECT b.BuoiTapID, b.ThoiGianBatDau, b.ThoiGianKetThuc, 
                           DATEDIFF(MINUTE, b.ThoiGianBatDau, b.ThoiGianKetThuc) AS Minutes
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0 
                      AND b.TrangThai = N'Hoàn thành'
                    ORDER BY b.NgayThucHien DESC";
                
                var checkData = _context.Database.SqlQuery<TimeCheckData>(checkSql, userId).Take(5).ToList();
                System.Diagnostics.Debug.WriteLine($"GetTotalTimeAsync - Found {checkData.Count} sessions with time data");
                foreach (var item in checkData)
                {
                    System.Diagnostics.Debug.WriteLine($"  - BuoiTapID: {item.BuoiTapID}, ThoiGianBatDau: {item.ThoiGianBatDau}, ThoiGianKetThuc: {item.ThoiGianKetThuc}, Minutes: {item.Minutes}");
                }
                
                var sql = @"
                    SELECT ISNULL(SUM(CAST(DATEDIFF(MINUTE, b.ThoiGianBatDau, b.ThoiGianKetThuc) AS FLOAT)), 0)
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0 
                      AND b.TrangThai = N'Hoàn thành'
                      AND b.ThoiGianBatDau IS NOT NULL 
                      AND b.ThoiGianKetThuc IS NOT NULL";
                
                var totalMinutes = _context.Database.SqlQuery<double>(sql, userId).FirstOrDefault();
                System.Diagnostics.Debug.WriteLine($"GetTotalTimeAsync - Total minutes: {totalMinutes}");
                
                return Task.FromResult(totalMinutes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTotalTimeAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(0.0);
            }
        }

        // Helper class cho debug
        private class TimeCheckData
        {
            public string BuoiTapID { get; set; }
            public DateTime? ThoiGianBatDau { get; set; }
            public DateTime? ThoiGianKetThuc { get; set; }
            public int? Minutes { get; set; }
        }

        public Task<int> GetTotalAchievementsAsync(string userId)
        {
            try
            {
                var sql = "SELECT COUNT(*) FROM ThanhTuu WHERE UserID = @p0";
                var count = _context.Database.SqlQuery<int>(sql, userId).FirstOrDefault();
                return Task.FromResult(count);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTotalAchievementsAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(0);
            }
        }

        public Task<int> GetCompletedGoalsAsync(string userId)
        {
            try
            {
                var sql = "SELECT COUNT(*) FROM MucTieu WHERE UserID = @p0 AND TrangThai = N'Hoàn thành'";
                var count = _context.Database.SqlQuery<int>(sql, userId).FirstOrDefault();
                return Task.FromResult(count);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCompletedGoalsAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(0);
            }
        }

        public Task<double> GetAverageTimePerSessionAsync(string userId)
        {
            try
            {
                var sql = @"
                    SELECT ISNULL(AVG(CAST(DATEDIFF(MINUTE, b.ThoiGianBatDau, b.ThoiGianKetThuc) AS FLOAT)), 0)
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0 
                      AND b.TrangThai = N'Hoàn thành'
                      AND b.ThoiGianBatDau IS NOT NULL 
                      AND b.ThoiGianKetThuc IS NOT NULL";
                
                var avgMinutes = _context.Database.SqlQuery<double>(sql, userId).FirstOrDefault();
                return Task.FromResult(avgMinutes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAverageTimePerSessionAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(0.0);
            }
        }

        public Task<double> GetAverageSessionsPerWeekAsync(string userId)
        {
            try
            {
                var sql = @"
                    SELECT 
                        CASE 
                            WHEN COUNT(*) = 0 THEN 0
                            WHEN DATEDIFF(day, MIN(b.NgayThucHien), MAX(b.NgayThucHien)) = 0 THEN CAST(COUNT(*) AS FLOAT)
                            ELSE CAST(COUNT(*) AS FLOAT) / (DATEDIFF(day, MIN(b.NgayThucHien), MAX(b.NgayThucHien)) / 7.0)
                        END AS AvgSessionsPerWeek
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0 
                      AND b.TrangThai = N'Hoàn thành'
                      AND b.NgayThucHien IS NOT NULL";
                
                var avgSessions = _context.Database.SqlQuery<double>(sql, userId).FirstOrDefault();
                return Task.FromResult(avgSessions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAverageSessionsPerWeekAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(0.0);
            }
        }

        public Task<double> GetAverageCaloriesBurnedAsync(string userId)
        {
            try
            {
                var sql = @"
                    SELECT ISNULL(AVG(CAST(b.Calories AS FLOAT)), 0)
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0 
                      AND b.TrangThai = N'Hoàn thành'
                      AND b.Calories IS NOT NULL";
                
                var avgCalories = _context.Database.SqlQuery<double>(sql, userId).FirstOrDefault();
                return Task.FromResult(avgCalories);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAverageCaloriesBurnedAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(0.0);
            }
        }

        public Task<Dictionary<DateTime, double>> GetWeeklyProgressAsync(string userId)
        {
            try
            {
                var result = new Dictionary<DateTime, double>();
                var endDate = DateTime.Now.Date;
                var startDate = endDate.AddDays(-6); // 7 ngày gần nhất

                // Khởi tạo tất cả các ngày với giá trị 0
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    result[date] = 0;
                }

                var sql = @"
                    SELECT CAST(b.NgayThucHien AS DATE) AS Ngay, 
                           ISNULL(SUM(CAST(DATEDIFF(MINUTE, b.ThoiGianBatDau, b.ThoiGianKetThuc) AS FLOAT)), 0) AS TotalMinutes
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0 
                      AND b.TrangThai = N'Hoàn thành'
                      AND b.NgayThucHien >= @p1
                      AND b.NgayThucHien <= @p2
                      AND b.ThoiGianBatDau IS NOT NULL
                      AND b.ThoiGianKetThuc IS NOT NULL
                    GROUP BY CAST(b.NgayThucHien AS DATE)";

                var progressData = _context.Database.SqlQuery<WeeklyProgressData>(sql, userId, startDate, endDate).ToList();

                foreach (var item in progressData)
                {
                    if (result.ContainsKey(item.Ngay))
                    {
                        result[item.Ngay] = item.TotalMinutes;
                    }
                }

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetWeeklyProgressAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(new Dictionary<DateTime, double>());
            }
        }

        // Helper class cho SQL query result
        private class WeeklyProgressData
        {
            public DateTime Ngay { get; set; }
            public double TotalMinutes { get; set; }
        }

        public Task<Dictionary<string, int>> GetMuscleGroupDistributionAsync(string userId)
        {
            try
            {
                var sql = @"
                    SELECT t.NhomCoChinhNhat, COUNT(*) AS SoLanTap
                    FROM BaiTapChiTiet bt
                    INNER JOIN BuoiTap b ON bt.BuoiTapID = b.BuoiTapID
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    INNER JOIN ThuVienBaiTap t ON bt.BaiTapID = t.BaiTapID
                    WHERE k.UserID = @p0
                      AND b.TrangThai = N'Hoàn thành'
                      AND bt.TrangThai = N'Hoàn thành'
                      AND t.NhomCoChinhNhat IS NOT NULL
                      AND t.NhomCoChinhNhat != ''
                    GROUP BY t.NhomCoChinhNhat";

                var distributionData = _context.Database.SqlQuery<MuscleGroupData>(sql, userId).ToList();

                var distribution = new Dictionary<string, int>();
                foreach (var item in distributionData)
                {
                    distribution[item.NhomCoChinhNhat] = item.SoLanTap;
                }

                return Task.FromResult(distribution);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetMuscleGroupDistributionAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(new Dictionary<string, int>());
            }
        }

        // Helper class cho SQL query result
        private class MuscleGroupData
        {
            public string NhomCoChinhNhat { get; set; }
            public int SoLanTap { get; set; }
        }

        public Task<Dictionary<string, Dictionary<string, double>>> GetTwoWeeksComparisonAsync(string userId)
        {
            try
            {
                var result = new Dictionary<string, Dictionary<string, double>>();
                var today = DateTime.Now.Date;
                var week2End = today;
                var week2Start = today.AddDays(-6);
                var week1End = week2Start.AddDays(-1);
                var week1Start = week1End.AddDays(-6);

                // Query cho tuần 1 (tuần trước)
                var sqlWeek1 = @"
                    SELECT 
                        COUNT(*) AS Sessions,
                        ISNULL(SUM(CAST(DATEDIFF(MINUTE, b.ThoiGianBatDau, b.ThoiGianKetThuc) AS FLOAT)), 0) AS Time,
                        ISNULL(SUM(CAST(b.Calories AS FLOAT)), 0) AS Calories
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0
                      AND b.TrangThai = N'Hoàn thành'
                      AND CAST(b.NgayThucHien AS DATE) >= @p1
                      AND CAST(b.NgayThucHien AS DATE) <= @p2";

                var week1Data = _context.Database.SqlQuery<WeekComparisonData>(sqlWeek1, userId, week1Start, week1End).FirstOrDefault();
                var week1Dict = new Dictionary<string, double>
                {
                    ["Sessions"] = week1Data?.Sessions ?? 0,
                    ["Time"] = week1Data?.Time ?? 0,
                    ["Calories"] = week1Data?.Calories ?? 0
                };

                // Query cho tuần 2 (tuần này)
                var sqlWeek2 = @"
                    SELECT 
                        COUNT(*) AS Sessions,
                        ISNULL(SUM(CAST(DATEDIFF(MINUTE, b.ThoiGianBatDau, b.ThoiGianKetThuc) AS FLOAT)), 0) AS Time,
                        ISNULL(SUM(CAST(b.Calories AS FLOAT)), 0) AS Calories
                    FROM BuoiTap b
                    INNER JOIN KeHoachLuyenTap k ON b.KeHoachTapID = k.KeHoachTapID
                    WHERE k.UserID = @p0
                      AND b.TrangThai = N'Hoàn thành'
                      AND CAST(b.NgayThucHien AS DATE) >= @p1
                      AND CAST(b.NgayThucHien AS DATE) <= @p2";

                var week2Data = _context.Database.SqlQuery<WeekComparisonData>(sqlWeek2, userId, week2Start, week2End).FirstOrDefault();
                var week2Dict = new Dictionary<string, double>
                {
                    ["Sessions"] = week2Data?.Sessions ?? 0,
                    ["Time"] = week2Data?.Time ?? 0,
                    ["Calories"] = week2Data?.Calories ?? 0
                };

                result["Tuần trước"] = week1Dict;
                result["Tuần này"] = week2Dict;

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTwoWeeksComparisonAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                return Task.FromResult(new Dictionary<string, Dictionary<string, double>>());
            }
        }

        // Helper class cho SQL query result
        private class WeekComparisonData
        {
            public int Sessions { get; set; }
            public double Time { get; set; }
            public double Calories { get; set; }
        }
    }
}

