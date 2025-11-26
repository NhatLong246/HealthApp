using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Repositories;
using HealthApp.Repositories.Interfaces;
using HealthApp.Services;
using HealthApp.Services.Interfaces;

namespace HealthApp.Controllers
{
    public class ReportController : IDisposable
    {
        private readonly IReportService _reportService;
        private readonly WF_HealthTracker _dbContext;

        public ReportController()
        {
            _dbContext = new WF_HealthTracker();
            IUserRepository userRepository = new UserRepository(_dbContext);
            _reportService = new ReportService(userRepository, _dbContext);
        }

        public async Task<ReportStatistics> GetStatisticsAsync(string userId)
        {
            var stats = new ReportStatistics
            {
                TotalSessions = await _reportService.GetTotalSessionsAsync(userId),
                TotalTime = await _reportService.GetTotalTimeAsync(userId),
                TotalAchievements = await _reportService.GetTotalAchievementsAsync(userId),
                CompletedGoals = await _reportService.GetCompletedGoalsAsync(userId),
                AverageTimePerSession = await _reportService.GetAverageTimePerSessionAsync(userId),
                AverageSessionsPerWeek = await _reportService.GetAverageSessionsPerWeekAsync(userId),
                AverageCaloriesBurned = await _reportService.GetAverageCaloriesBurnedAsync(userId),
                WeeklyProgress = await _reportService.GetWeeklyProgressAsync(userId),
                MuscleGroupDistribution = await _reportService.GetMuscleGroupDistributionAsync(userId),
                TwoWeeksComparison = await _reportService.GetTwoWeeksComparisonAsync(userId)
            };

            return stats;
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }

    public class ReportStatistics
    {
        public int TotalSessions { get; set; }
        public double TotalTime { get; set; } // phút
        public int TotalAchievements { get; set; }
        public int CompletedGoals { get; set; }
        public double AverageTimePerSession { get; set; } // phút
        public double AverageSessionsPerWeek { get; set; }
        public double AverageCaloriesBurned { get; set; }
        public Dictionary<DateTime, double> WeeklyProgress { get; set; }
        public Dictionary<string, int> MuscleGroupDistribution { get; set; }
        public Dictionary<string, Dictionary<string, double>> TwoWeeksComparison { get; set; }
    }
}

