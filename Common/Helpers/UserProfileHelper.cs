using System;
using System.Linq;
using HealthApp.Models;

namespace HealthApp.Common.Helpers
{
    public static class UserProfileHelper
    {
        /// <summary>
        /// Kiểm tra người dùng đã có đủ thông tin cơ bản chưa.
        /// </summary>
        public static bool NeedsBasicInfo()
        {
            if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
            {
                return false;
            }

            var user = CurrentUser.User;
            if (!user.NgaySinh.HasValue || string.IsNullOrWhiteSpace(user.GioiTinh))
            {
                return true;
            }

            using (var db = new WF_HealthTracker())
            {
                var latestRecord = db.TinhTrangTongQuan
                    .Where(t => t.UserID == user.UserID)
                    .OrderByDescending(t => t.NgayGhiNhan)
                    .FirstOrDefault();

                return latestRecord == null
                    || !latestRecord.CanNang.HasValue || latestRecord.CanNang <= 0
                    || !latestRecord.ChieuCao.HasValue || latestRecord.ChieuCao <= 0;
            }
        }
    }
}

