using HealthApp.Models;

namespace HealthApp.Common.Helpers
{
    /// <summary>
    /// Class để quản lý user hiện tại đang đăng nhập (Session)
    /// </summary>
    public static class CurrentUser
    {
        private static Users _currentUser;

        /// <summary>
        /// User hiện tại đang đăng nhập
        /// </summary>
        public static Users User
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        /// <summary>
        /// Kiểm tra đã đăng nhập chưa
        /// </summary>
        public static bool IsLoggedIn => _currentUser != null;

        /// <summary>
        /// UserID của user hiện tại
        /// </summary>
        public static string UserID => _currentUser?.UserID;

        /// <summary>
        /// Username của user hiện tại
        /// </summary>
        public static string Username => _currentUser?.Username;

        /// <summary>
        /// Role của user hiện tại
        /// </summary>
        public static string Role => _currentUser?.Role;

        /// <summary>
        /// Đăng xuất
        /// </summary>
        public static void Logout()
        {
            _currentUser = null;
        }
    }
}

