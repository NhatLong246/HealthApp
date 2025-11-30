using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Views.Auth;
using HealthApp.Views.Dashboard;
using HealthApp.Views.Settings;

namespace HealthApp
{
    internal static class Program
    {
        // DPI Awareness để xử lý scaling đúng cách trên màn hình high-DPI
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

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

            // Hiển thị form đăng nhập
            using (var loginForm = new LoginForm())
            {
                var result = loginForm.ShowDialog();
                if (result == DialogResult.OK && CurrentUser.IsLoggedIn)
                {
                    if (UserProfileHelper.NeedsBasicInfo())
                    {
                        using (var infoForm = new frmChangeInformationforNewuser(isMandatory: true))
                        {
                            infoForm.ShowDialog();
                        }
                    }
                    Application.Run(new frmDashBoard());
                }
            }
        }
    }
}
