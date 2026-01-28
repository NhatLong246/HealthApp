using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Views.Auth;
using HealthApp.Views.Dashboard;
using HealthApp.Views.Settings;
using HealthApp.Views.Admin;
using HealthApp.Services;

namespace HealthApp
{
    internal static class Program
    {
        // DPI Awareness để xử lý scaling đúng cách trên màn hình high-DPI
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        // Service để gửi email thông báo lịch tập tự động
        private static WorkoutNotificationService _workoutNotificationService;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Set DPI Awareness TRƯỚC KHI khởi tạo Application
            // Điều này rất quan trọng để form hiển thị đúng trên màn hình high-DPI
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, args) =>
            {
                MessageBox.Show($"Unhandled UI exception: {args.Exception}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                MessageBox.Show($"Unhandled non-UI exception: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // Enable visual styles - QUAN TRỌNG cho Guna.UI2 controls
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi động WorkoutNotificationService để gửi email thông báo tự động
            try
            {
                var emailService = new EmailService();
                _workoutNotificationService = new WorkoutNotificationService(emailService);
                _workoutNotificationService.Start();
                System.Diagnostics.Debug.WriteLine("WorkoutNotificationService đã được khởi động thành công");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi khởi động WorkoutNotificationService: {ex.Message}");
                // Không dừng ứng dụng nếu service không khởi động được
            }

            try
            {
            // Hiển thị form đăng nhập
            using (var loginForm = new LoginForm())
            {
                var result = loginForm.ShowDialog();
                if (result == DialogResult.OK && CurrentUser.IsLoggedIn)
                {
                    // Kiểm tra role để hiển thị form phù hợp
                    string userRole = CurrentUser.Role;

                    System.Diagnostics.Debug.WriteLine($"[Program] User logged in - Role: '{userRole}'");

                    if (userRole == "Admin")
                    {
                        // Mở form Admin cho user có role Admin
                        System.Diagnostics.Debug.WriteLine("[Program] Opening frmAdmin for Admin user");
                        Application.Run(new frmAdmin());
                    }
                    else
                    {
                        // Mở form Dashboard cho Client và PT
                        if (UserProfileHelper.NeedsBasicInfo())
                        {
                            using (var infoForm = new frmThongTinhTheTrang(isMandatory: true))
                            {
                                infoForm.ShowDialog();
                            }
                        }
                        System.Diagnostics.Debug.WriteLine("[Program] Opening frmDashBoard1 for Client/PT user");
                        Application.Run(new frmDashBoard1());
                        }
                    }
                }
            }
            finally
            {
                // Dừng và dispose service khi ứng dụng đóng
                _workoutNotificationService?.Stop();
                _workoutNotificationService?.Dispose();
            }
        }
    }
}
