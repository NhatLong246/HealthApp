using System;
using System.Windows.Forms;
using HealthApp.Common.Helpers;

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
    }
}
