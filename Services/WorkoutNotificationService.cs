using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Services.Interfaces;

namespace HealthApp.Services
{
    /// <summary>
    /// Service để gửi email thông báo lịch tập luyện tự động
    /// </summary>
    public class WorkoutNotificationService : IDisposable
    {
        private readonly IEmailService _emailService;
        private readonly WF_HealthTracker _dbContext;
        private Timer _timer;
        private bool _disposed = false;
        private readonly object _lockObject = new object();

        // Dictionary để lưu trữ các email đã gửi (tránh gửi trùng)
        // Key: BuoiTapID_NotificationType_Date, Value: true nếu đã gửi
        private readonly Dictionary<string, bool> _sentNotifications = new Dictionary<string, bool>();

        public WorkoutNotificationService(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _dbContext = new WF_HealthTracker();
        }

        /// <summary>
        /// Bắt đầu service - kiểm tra mỗi giờ
        /// </summary>
        public void Start()
        {
            // Kiểm tra ngay lập tức
            CheckAndSendNotificationsAsync().ConfigureAwait(false);

            // Sau đó kiểm tra mỗi giờ (3600000 milliseconds)
            _timer = new Timer(async _ => await CheckAndSendNotificationsAsync(), null, TimeSpan.Zero, TimeSpan.FromHours(1));
            
            System.Diagnostics.Debug.WriteLine("WorkoutNotificationService đã được khởi động - kiểm tra mỗi giờ");
        }

        /// <summary>
        /// Dừng service
        /// </summary>
        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
            System.Diagnostics.Debug.WriteLine("WorkoutNotificationService đã được dừng");
        }

        /// <summary>
        /// Kiểm tra và gửi thông báo email
        /// </summary>
        private async Task CheckAndSendNotificationsAsync()
        {
            try
            {
                lock (_lockObject)
                {
                    // Xóa các notification cũ (quá 2 ngày) để tránh memory leak
                    var keysToRemove = _sentNotifications.Keys
                        .Where(k => k.Contains(DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd")))
                        .ToList();
                    foreach (var key in keysToRemove)
                    {
                        _sentNotifications.Remove(key);
                    }
                }

                DateTime now = DateTime.Now;
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);
                DateTime yesterday = today.AddDays(-1);

                // Lấy tất cả các kế hoạch luyện tập đang hoạt động
                var activePlans = _dbContext.KeHoachLuyenTap
                    .Where(k => k.TrangThai == "Đang hoạt động")
                    .ToList();

                foreach (var plan in activePlans)
                {
                    // Load User
                    var user = _dbContext.Users.FirstOrDefault(u => u.UserID == plan.UserID);
                    if (user == null || string.IsNullOrWhiteSpace(user.Email))
                    {
                        continue; // Bỏ qua nếu không có email
                    }

                    // Load tất cả BuoiTap của kế hoạch
                    var buoiTapList = _dbContext.BuoiTap
                        .Where(b => b.KeHoachTapID == plan.KeHoachTapID)
                        .ToList();

                    // Load BaiTapChiTiet và ThuVienBaiTap cho mỗi BuoiTap
                    foreach (var buoiTap in buoiTapList)
                    {
                        _dbContext.Entry(buoiTap)
                            .Collection(bt => bt.BaiTapChiTiet)
                            .Load();

                        foreach (var baiTapChiTiet in buoiTap.BaiTapChiTiet)
                        {
                            if (baiTapChiTiet.ThuVienBaiTap == null && !string.IsNullOrEmpty(baiTapChiTiet.BaiTapID))
                            {
                                _dbContext.Entry(baiTapChiTiet)
                                    .Reference(bt => bt.ThuVienBaiTap)
                                    .Load();
                            }
                        }
                    }

                    foreach (var buoiTap in buoiTapList)
                    {
                        if (!buoiTap.ThoiGianBatDau.HasValue)
                        {
                            continue; // Bỏ qua nếu không có thời gian bắt đầu
                        }

                        DateTime workoutDate = buoiTap.ThoiGianBatDau.Value.Date;
                        string userName = string.IsNullOrWhiteSpace(user.HoTen) ? user.Username : user.HoTen;

                        // 1. Kiểm tra và gửi email trước 1 ngày
                        if (workoutDate == tomorrow)
                        {
                            string key1 = $"{buoiTap.BuoiTapID}_1_{tomorrow:yyyy-MM-dd}";
                            if (!IsNotificationSent(key1))
                            {
                                await SendNotificationAsync(user.Email, userName, buoiTap, 1);
                                MarkNotificationSent(key1);
                            }
                        }

                        // 2. Kiểm tra và gửi email vào ngày tập (vào buổi sáng, 8h)
                        if (workoutDate == today && now.Hour >= 8 && now.Hour < 9)
                        {
                            string key2 = $"{buoiTap.BuoiTapID}_2_{today:yyyy-MM-dd}";
                            if (!IsNotificationSent(key2))
                            {
                                await SendNotificationAsync(user.Email, userName, buoiTap, 2);
                                MarkNotificationSent(key2);
                            }
                        }

                        // 3. Kiểm tra và gửi email quá ngày tập (ngày hôm sau, sau 9h sáng, nếu chưa hoàn thành)
                        if (workoutDate == yesterday && now.Hour >= 9 && buoiTap.TrangThai != "Hoàn thành")
                        {
                            // Kiểm tra xem có bài tập nào đã được thực hiện không
                            bool hasCompletedExercise = buoiTap.BaiTapChiTiet != null &&
                                buoiTap.BaiTapChiTiet.Any(bt => bt.TrangThai == "Hoàn thành");

                            if (!hasCompletedExercise)
                            {
                                string key3 = $"{buoiTap.BuoiTapID}_3_{yesterday:yyyy-MM-dd}";
                                if (!IsNotificationSent(key3))
                                {
                                    await SendNotificationAsync(user.Email, userName, buoiTap, 3);
                                    MarkNotificationSent(key3);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong CheckAndSendNotificationsAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// Gửi thông báo email
        /// </summary>
        private async Task SendNotificationAsync(string email, string userName, BuoiTap buoiTap, int notificationType)
        {
            try
            {
                var result = await _emailService.SendWorkoutNotificationEmailAsync(email, userName, buoiTap, notificationType);
                if (result.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Đã gửi email thông báo loại {notificationType} đến {email} cho buổi tập {buoiTap.BuoiTapID}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Lỗi gửi email đến {email}: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi email thông báo: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra xem notification đã được gửi chưa
        /// </summary>
        private bool IsNotificationSent(string key)
        {
            lock (_lockObject)
            {
                return _sentNotifications.ContainsKey(key) && _sentNotifications[key];
            }
        }

        /// <summary>
        /// Đánh dấu notification đã được gửi
        /// </summary>
        private void MarkNotificationSent(string key)
        {
            lock (_lockObject)
            {
                _sentNotifications[key] = true;
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _dbContext?.Dispose();
                _disposed = true;
            }
        }
    }
}
