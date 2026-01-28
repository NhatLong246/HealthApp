using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Models;
using HealthApp.Repositories;
using HealthApp.Repositories.Interfaces;
using Guna.UI2.WinForms;

namespace HealthApp.Views.Settings
{
    public partial class DoiMatKhau : Form
    {
        private readonly WF_HealthTracker _context;
        private readonly IUserRepository _userRepository;
        private Users _currentUser;

        public DoiMatKhau()
        {
            InitializeComponent();
            _context = new WF_HealthTracker();
            _userRepository = new UserRepository(_context);
            InitializeEventHandlers();
            LoadUserInfo();
        }

        private void InitializeEventHandlers()
        {
            btnChangePassword.Click += BtnChangePassword_Click;
            this.Load += DoiMatKhau_Load;
        }

        private void DoiMatKhau_Load(object sender, EventArgs e)
        {
            // Set password character cho các textbox mật khẩu
            txtMatKhauHienTai.PasswordChar = '●';
            txtMatKhauHienTai.UseSystemPasswordChar = false;
            txtNewPassWord.PasswordChar = '●';
            txtNewPassWord.UseSystemPasswordChar = false;
            txtConfirmPassword.PasswordChar = '●';
            txtConfirmPassword.UseSystemPasswordChar = false;
        }

        private void LoadUserInfo()
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập để đổi mật khẩu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                // Load user từ database
                var userId = CurrentUser.UserID;
                _currentUser = _context.Users.FirstOrDefault(u => u.UserID == userId);

                if (_currentUser == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thông tin người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validation: Mật khẩu hiện tại
                string matKhauHienTai = txtMatKhauHienTai.Text;
                if (string.IsNullOrWhiteSpace(matKhauHienTai))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu hiện tại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMatKhauHienTai.Focus();
                    return;
                }

                // Kiểm tra mật khẩu hiện tại có đúng không
                if (!PasswordHelper.VerifyPassword(matKhauHienTai, _currentUser.PasswordHash))
                {
                    MessageBox.Show("Mật khẩu hiện tại không đúng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMatKhauHienTai.Focus();
                    txtMatKhauHienTai.Text = "";
                    return;
                }

                // Validation: Mật khẩu mới
                string matKhauMoi = txtNewPassWord.Text;
                if (string.IsNullOrWhiteSpace(matKhauMoi))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewPassWord.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidPassword(matKhauMoi))
                {
                    MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewPassWord.Focus();
                    return;
                }

                // Validation: Xác nhận mật khẩu
                string xacNhanMatKhau = txtConfirmPassword.Text;
                if (string.IsNullOrWhiteSpace(xacNhanMatKhau))
                {
                    MessageBox.Show("Vui lòng xác nhận mật khẩu mới!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfirmPassword.Focus();
                    return;
                }

                if (!ValidationHelper.PasswordsMatch(matKhauMoi, xacNhanMatKhau))
                {
                    MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtConfirmPassword.Focus();
                    return;
                }

                // Hash mật khẩu mới
                string newPasswordHash = PasswordHelper.HashPassword(matKhauMoi);

                // Cập nhật mật khẩu trong database
                _currentUser.PasswordHash = newPasswordHash;
                _context.SaveChanges();

                // Cập nhật CurrentUser
                CurrentUser.User.PasswordHash = newPasswordHash;

                MessageBox.Show("Đổi mật khẩu thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear các textbox
                txtMatKhauHienTai.Text = "";
                txtNewPassWord.Text = "";
                txtConfirmPassword.Text = "";

                // Đóng form
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đổi mật khẩu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
