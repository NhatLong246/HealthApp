using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Views.Auth;

namespace HealthApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
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

            // Hiển thị form đăng nhập
            using (var loginForm = new LoginForm())
            {
                // Nếu đăng nhập thành công
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // TODO: Điều hướng đến DashboardForm sau khi đăng nhập thành công
                    // Application.Run(new DashboardForm());
                }
            }
        }
    }
}
