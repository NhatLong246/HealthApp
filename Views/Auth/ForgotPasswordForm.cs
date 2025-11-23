using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Controllers;
using HealthApp.Common.Helpers;

namespace HealthApp.Views.Auth
{
    public partial class ForgotPasswordForm : Form
    {
        private readonly ForgotPasswordController _forgotPasswordController;

        public ForgotPasswordForm()
        {
            InitializeComponent();
            
            // Thiết lập form hiển thị ở giữa màn hình
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Khởi tạo Controller
            _forgotPasswordController = new ForgotPasswordController();
            
            // Kết nối event handlers
            txtSendOTP.Click += BtnSendOTP_Click;
            lnkBackLogin.LinkClicked += lnkBackLogin_LinkClicked;
            this.Load += ForgotPasswordForm_Load;
        }

        private void ForgotPasswordForm_Load(object sender, EventArgs e)
        {
            if (txtEmail != null)
                txtEmail.Focus();
        }

        private async void BtnSendOTP_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text?.Trim();
                
                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Vui lòng nhập địa chỉ email.", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidGmail(email))
                {
                    MessageBox.Show("Email không hợp lệ. Vui lòng nhập email @gmail.com.", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                // Disable button
                txtSendOTP.Enabled = false;
                txtSendOTP.Text = "Đang gửi...";

                var result = await _forgotPasswordController.SendOTPAsync(email);

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Mở form OTP và truyền email
                    this.Hide();
                    using (var otpForm = new ForgotPasswordForm_OTP(email))
                    {
                        if (otpForm.ShowDialog() == DialogResult.OK)
                        {
                            // OTP đã được xác thực, form OTP sẽ mở ChangePasswordForm
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            // Người dùng hủy hoặc quay lại
                            this.Show();
                        }
                    }
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
                txtSendOTP.Enabled = true;
                txtSendOTP.Text = "Gửi mã OTP";
            }
        }

        private void lnkBackLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NavigateToLogin();
        }

        private void NavigateToLogin()
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay về form đăng nhập: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _forgotPasswordController?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
