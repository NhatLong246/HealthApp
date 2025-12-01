using System;
using System.Linq;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Views.Auth;

namespace HealthApp.Views.Settings
{
    public partial class ucSetting : UserControl
    {
        public ucSetting()
        {
            InitializeComponent();
            Load += UcSetting_Load;
            RegisterInteractiveEvents();
        }

        private void UcSetting_Load(object sender, EventArgs e)
        {
            LoadUserInformation();
        }

        private void RegisterInteractiveEvents()
        {
            AttachProfileEditHandler(guna2Panel5);
            AttachProfileEditHandler(label5);
            AttachProfileEditHandler(guna2CirclePictureBox8);
            AttachProfileEditHandler(guna2CirclePictureBox3);
            
            // Gắn event handler cho nút đăng xuất
            if (guna2Button1 != null)
            {
                guna2Button1.Click -= BtnDangXuat_Click;
                guna2Button1.Click += BtnDangXuat_Click;
            }
        }

        private void AttachProfileEditHandler(Control control)
        {
            if (control == null) return;
            control.Click -= OpenProfileEditor;
            control.Click += OpenProfileEditor;
        }

        private void OpenProfileEditor(object sender, EventArgs e)
        {
            using (var form = new frmChangeInformationforNewuser())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadUserInformation();
                }
            }
        }

        private void LoadUserInformation()
        {
            if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
            {
                lblTenNgDung.Text = "Khách mới";
                lblEmailNgDung.Text = "Đăng nhập để xem thông tin tài khoản.";
                return;
            }

            var user = CurrentUser.User;
            lblTenNgDung.Text = string.IsNullOrWhiteSpace(user.HoTen)
                ? user.Username
                : user.HoTen;

            lblEmailNgDung.Text = string.IsNullOrWhiteSpace(user.Email)
                ? "Chưa cập nhật email"
                : user.Email;
        }

        /// <summary>
        /// Event handler cho nút đăng xuất
        /// </summary>
        private void BtnDangXuat_Click(object sender, EventArgs e)
        {
            try
            {
                // Xác nhận đăng xuất
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Đăng xuất
                    CurrentUser.Logout();

                    // Tìm form Dashboard
                    var dashboardForm = this.FindForm() as Dashboard.frmDashBoard
                        ?? Application.OpenForms.OfType<Dashboard.frmDashBoard>().FirstOrDefault();

                    if (dashboardForm != null)
                    {
                        // Ẩn form Dashboard
                        dashboardForm.Hide();

                        // Mở lại form đăng nhập
                        var loginForm = new LoginForm();
                        if (loginForm.ShowDialog() == DialogResult.OK)
                        {
                            // Nếu đăng nhập thành công, reload lại Dashboard
                            // Không đóng form cũ vì nó là form chính của ứng dụng
                            
                            // Reload lại thông tin user trong header
                            dashboardForm.ReloadUserInfo();
                            
                            // Reload lại trang chủ để cập nhật dữ liệu
                            dashboardForm.ReloadDashboard();
                            
                            // Reload lại thông tin user trong Settings
                            LoadUserInformation();
                            
                            // Hiển thị lại Dashboard
                            dashboardForm.Show();
                            dashboardForm.BringToFront();
                            dashboardForm.Activate();
                        }
                        else
                        {
                            // Nếu không đăng nhập, đóng ứng dụng
                            Application.Exit();
                        }
                    }
                    else
                    {
                        // Nếu không tìm thấy Dashboard, mở form đăng nhập
                        var loginForm = new LoginForm();
                        if (loginForm.ShowDialog() == DialogResult.OK)
                        {
                            // Tạo Dashboard mới
                            var newDashboard = new Dashboard.frmDashBoard();
                            newDashboard.Show();
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
