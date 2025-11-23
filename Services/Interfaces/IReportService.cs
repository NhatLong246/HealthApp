using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthApp.Services.Interfaces
{
    public interface IReportService
    {
        // Thống kê tổng quan
        Task<int> GetTotalSessionsAsync(string userId);
        Task<double> GetTotalTimeAsync(string userId); // Tổng thời gian (phút)
        Task<int> GetTotalAchievementsAsync(string userId);
        Task<int> GetCompletedGoalsAsync(string userId);

        // Thống kê chi tiết
        Task<double> GetAverageTimePerSessionAsync(string userId); // TB thời gian/buổi (phút)
        Task<double> GetAverageSessionsPerWeekAsync(string userId); // TB buổi/tuần
        Task<double> GetAverageCaloriesBurnedAsync(string userId); // TB calo đốt

        // Dữ liệu biểu đồ tiến độ tuần (7 ngày gần nhất)
        Task<Dictionary<DateTime, double>> GetWeeklyProgressAsync(string userId); // Key: Ngày, Value: Calories đốt

        // Dữ liệu phân bố nhóm cơ
        Task<Dictionary<string, int>> GetMuscleGroupDistributionAsync(string userId); // Key: Nhóm cơ, Value: Số lần tập

        // Dữ liệu so sánh 2 tuần gần nhất
        Task<Dictionary<string, Dictionary<string, double>>> GetTwoWeeksComparisonAsync(string userId); // Key: Tuần, Value: {Sessions, Time, Calories}
    }
}

