using System;
using System.Linq;
using HealthApp.Models;

namespace HealthApp.Services
{
    internal static class NotificationService
    {
        public static bool EnsureCreate(
            WF_HealthTracker context,
            string userId,
            string title,
            string content,
            string type,
            string relatedId = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(userId)) return false;
            if (string.IsNullOrWhiteSpace(type)) return false;

            bool exists = context.ThongBao.Any(t =>
                t.UserID == userId &&
                t.Loai == type &&
                t.MaLienQuan == relatedId);

            if (exists) return false;

            Create(context, userId, title, content, type, relatedId);
            return true;
        }

        public static string Create(
            WF_HealthTracker context,
            string userId,
            string title,
            string content,
            string type,
            string relatedId = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(content)) content = "";

            var id = GenerateThongBaoId(context);
            var tb = new ThongBao
            {
                ThongBaoID = id,
                UserID = userId,
                TieuDe = title,
                NoiDung = content,
                Loai = type,
                MaLienQuan = relatedId,
                DaDoc = false,
                NgayTao = DateTime.Now
            };

            context.ThongBao.Add(tb);
            context.SaveChanges();
            return id;
        }

        private static string GenerateThongBaoId(WF_HealthTracker context)
        {
            // format: notif_0001
            var last = context.ThongBao
                .OrderByDescending(t => t.ThongBaoID)
                .Select(t => t.ThongBaoID)
                .FirstOrDefault();

            int next = 1;
            if (!string.IsNullOrWhiteSpace(last) && last.StartsWith("notif_", StringComparison.OrdinalIgnoreCase))
            {
                var parts = last.Split('_');
                if (parts.Length == 2 && int.TryParse(parts[1], out int n))
                    next = n + 1;
            }

            return $"notif_{next:D4}";
        }
    }
}

