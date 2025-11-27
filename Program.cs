using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HealthApp.Views.Dashboard;

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


            // Tạm thời: mở trực tiếp Dashboard để test giao diện
            Application.Run(new frmDashBoard());
        }
    }
}
