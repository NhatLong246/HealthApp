namespace HealthApp.Views.Dashboard
{
    /// <summary>
    /// Interface cho các form Dashboard để hỗ trợ điều hướng
    /// </summary>
    public interface IDashboardForm
    {
        /// <summary>
        /// Hiển thị lại form Dashboard (được gọi từ các form con khi quay lại)
        /// </summary>
        void ShowDashboard();
    }
}
