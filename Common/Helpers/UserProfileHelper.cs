using System;
using System.Linq;
using HealthApp.Models;

namespace HealthApp.Common.Helpers
{
    public static class UserProfileHelper
    {
        /// <summary>
        /// Kiểm tra người dùng có thiếu thông tin cơ bản (Ngày sinh/Giới tính) hay không.
        /// </summary>
        public static bool NeedsMissingBasicInfo()
        {
            if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
            {
                return false;
            }

            var user = CurrentUser.User;
            return !user.NgaySinh.HasValue || string.IsNullOrWhiteSpace(user.GioiTinh);
        }

        /// <summary>
        /// Chỉ ép nhập Thông tin thể trạng khi người dùng CHƯA CÓ bất kỳ bản ghi nào trong TinhTrangTongQuan.
        /// Tránh trường hợp tài khoản cũ/thiếu dữ liệu lặt vặt nhưng vẫn bị "bắt nhập" lại.
        /// </summary>
        public static bool NeedsBodyStatusMandatory()
        {
            if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
            {
                return false;
            }

            var user = CurrentUser.User;

            using (var db = new WF_HealthTracker())
            {
                return !db.TinhTrangTongQuan.Any(t => t.UserID == user.UserID);
            }
        }
    }
}

