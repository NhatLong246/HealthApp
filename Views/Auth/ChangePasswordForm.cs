using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Controllers;

namespace HealthApp.Views.Auth
{
    public partial class ChangePasswordForm : Form
    {
        private readonly ForgotPasswordController _forgotPasswordController;
        private readonly string _email;

        public ChangePasswordForm(string email)
        {
            InitializeComponent();
            
            _email = email;
            _forgotPasswordController = new ForgotPasswordController();
            
            // Thiết lập form hiển thị ở giữa màn hình
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Thiết lập password character
            txtNewPassWord.PasswordChar = '●';
            txtNewPassWord.UseSystemPasswordChar = false;
            txtConfirmPassword.PasswordChar = '●';
            txtConfirmPassword.UseSystemPasswordChar = false;
            
            // Kết nối event handlers
            btnResetPassword.Click += BtnResetPassword_Click;
            this.Load += ChangePasswordForm_Load;
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {
            if (txtNewPassWord != null)
                txtNewPassWord.Focus();
        }

        private async void BtnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                string newPassword = txtNewPassWord.Text;
                string confirmPassword = txtConfirmPassword.Text;

                if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu mới và xác nhận mật khẩu.", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Disable button
                btnResetPassword.Enabled = false;
                btnResetPassword.Text = "Đang xử lý...";

                var result = await _forgotPasswordController.ResetPasswordAsync(_email, newPassword, confirmPassword);

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Quay về form đăng nhập
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnResetPassword.Enabled = true;
                btnResetPassword.Text = "Đặt lại mật khẩu";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _forgotPasswordController?.Dispose();
            base.OnFormClosing(e);
        }

      
    }
}
