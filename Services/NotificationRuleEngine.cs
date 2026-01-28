using System;
using System.Linq;
using HealthApp.Models;

namespace HealthApp.Services
{
    internal static class NotificationRuleEngine
    {
        // chạy mỗi 5 phút
        private const int IntervalMinutes = 5;

        public static void RunForUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;

            try
            {
                using (var context = new WF_HealthTracker())
                {
                    var user = context.Users.FirstOrDefault(u => u.UserID == userId);
                    if (user == null) return;

                    var now = DateTime.Now;

                    RunUserSide(context, userId, now);

                    if (string.Equals(user.Role, "PT", StringComparison.OrdinalIgnoreCase))
                    {
                        RunPTSide(context, userId, now);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationRuleEngine] {ex.Message}");
            }
        }

        private static void RunUserSide(WF_HealthTracker context, string userId, DateTime now)
        {
            // quét các buổi tập trong khoảng: quá khứ 1 ngày tới tương lai 2 ngày
            var from = now.AddDays(-1);
            var to = now.AddDays(2);

            var sessions = context.DatLichPT
                .Where(d =>
                    d.KhachHangID == userId &&
                    d.ThoiGianBatDau >= from &&
                    d.ThoiGianBatDau <= to &&
                    (d.TrangThai == "Confirmed" || d.TrangThai == "Pending"))
                .ToList();

            foreach (var s in sessions)
            {
                // chỉ nhắc lịch khi đã confirmed (đã thanh toán)
                if (s.TrangThai == "Confirmed")
                {
                    MaybeCreateReminder(context, userId, s, now, TimeSpan.FromHours(24), "USER_REMIND_24H", "Nhắc lịch tập (trước 24h)");
                    MaybeCreateReminder(context, userId, s, now, TimeSpan.FromHours(1), "USER_REMIND_1H", "Nhắc lịch tập (trước 1h)");
                    MaybeCreateReminder(context, userId, s, now, TimeSpan.FromMinutes(10), "USER_REMIND_10M", "Nhắc lịch tập (trước 10 phút)");
                }

                // bỏ lỡ buổi tập: quá giờ kết thúc 30 phút mà vẫn chưa Completed
                if (now > s.ThoiGianKetThuc.AddMinutes(30) && s.TrangThai != "Completed" && s.TrangThai != "Cancelled")
                {
                    var title = "Bạn đã bỏ lỡ buổi tập";
                    var content = $"Bạn đã bỏ lỡ buổi tập lúc {s.ThoiGianBatDau:dd/MM/yyyy HH:mm}.";
                    NotificationService.EnsureCreate(context, userId, title, content, "USER_MISSED_SESSION", s.DatLichID);
                }
            }
        }

        private static void RunPTSide(WF_HealthTracker context, string ptUserId, DateTime now)
        {
            // tìm PTID từ userId
            var pt = context.HuanLuyenVien.FirstOrDefault(p => p.UserID == ptUserId);
            if (pt == null || string.IsNullOrWhiteSpace(pt.PTID)) return;

            var from = now;
            var to = now.AddDays(2);

            var sessions = context.DatLichPT
                .Where(d =>
                    d.PTID == pt.PTID &&
                    d.ThoiGianBatDau >= from &&
                    d.ThoiGianBatDau <= to &&
                    d.TrangThai == "Confirmed")
                .ToList();

            foreach (var s in sessions)
            {
                // chưa giao bài: không có assignment theo DatLichID
                bool hasAssignments = context.GiaoBaiTapChoUser.Any(a => a.DatLichID == s.DatLichID);
                if (!hasAssignments)
                {
                    // nhắc PT trước 24h và 1h
                    MaybeCreatePTReminder(context, ptUserId, s, now, TimeSpan.FromHours(24), "PT_NO_ASSIGN_24H");
                    MaybeCreatePTReminder(context, ptUserId, s, now, TimeSpan.FromHours(1), "PT_NO_ASSIGN_1H");
                }
            }
        }

        private static void MaybeCreateReminder(
            WF_HealthTracker context,
            string userId,
            DatLichPT session,
            DateTime now,
            TimeSpan before,
            string type,
            string title)
        {
            var triggerAt = session.ThoiGianBatDau - before;
            if (IsWithinInterval(now, triggerAt))
            {
                var content = $"Bạn có lịch tập vào {session.ThoiGianBatDau:dd/MM/yyyy HH:mm}.";
                NotificationService.EnsureCreate(context, userId, title, content, type, session.DatLichID);
            }
        }

        private static void MaybeCreatePTReminder(
            WF_HealthTracker context,
            string ptUserId,
            DatLichPT session,
            DateTime now,
            TimeSpan before,
            string type)
        {
            var triggerAt = session.ThoiGianBatDau - before;
            if (!IsWithinInterval(now, triggerAt)) return;

            var kh = context.Users.FirstOrDefault(u => u.UserID == session.KhachHangID);
            var khName = kh?.HoTen ?? kh?.Username ?? session.KhachHangID;

            var title = "Chưa giao bài tập cho buổi tập";
            var content = $"Buổi tập của user {khName} vào {session.ThoiGianBatDau:dd/MM/yyyy HH:mm} vẫn chưa được giao bài tập.";
            NotificationService.EnsureCreate(context, ptUserId, title, content, type, session.DatLichID);
        }

        private static bool IsWithinInterval(DateTime now, DateTime triggerAt)
        {
            // window [triggerAt, triggerAt + IntervalMinutes)
            return now >= triggerAt && now < triggerAt.AddMinutes(IntervalMinutes);
        }
    }
}

