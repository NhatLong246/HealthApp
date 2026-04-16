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
            try
            {
                // Set DPI Awareness TRƯỚC KHI khởi tạo Application
                if (Environment.OSVersion.Version.Major >= 6)
                    SetProcessDPIAware();

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

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                try
                {
                    var emailService = new EmailService();
                    _workoutNotificationService = new WorkoutNotificationService(emailService);
                    _workoutNotificationService.Start();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WorkoutNotificationService: {ex.Message}");
                }

                using (var loginForm = new LoginForm())
                {
                    var result = loginForm.ShowDialog();
                    if (result == DialogResult.OK && CurrentUser.IsLoggedIn)
                    {
                        string userRole = CurrentUser.Role;
                        if (userRole == "Admin")
                        {
                            Application.Run(new frmAdmin());
                        }
                        else
                        {
                            try
                            {
                                if (UserProfileHelper.NeedsMissingBasicInfo())
                                {
                                    using (var f = new frmChangeInformationforNewuser(isMandatory: true))
                                        f.ShowDialog();
                                }
                                else if (UserProfileHelper.NeedsBodyStatusMandatory())
                                {
                                    using (var f = new frmThongTinhTheTrang(isMandatory: true))
                                        f.ShowDialog();
                                }
                            }
                            catch { }
                            Application.Run(new frmDashBoard1());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khởi động ứng dụng:\n\n" + ex.ToString() + "\n\n---\nKiểm tra: SQL Server đang chạy, connection string trong App.config, Output > Build.",
                    "HealthApp – Lỗi khởi động",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _workoutNotificationService?.Stop();
                _workoutNotificationService?.Dispose();
            }
        }
    }
}
