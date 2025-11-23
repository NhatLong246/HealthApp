using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Controllers;
using HealthApp.Views.Dashboard;

namespace HealthApp.Views.Auth
{
    public partial class LoginForm : Form
    {
        private readonly AuthController _authController;

        public LoginForm()
        {
            InitializeComponent();
            
            // Thiết lập form hiển thị ở giữa màn hình
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Khởi tạo Controller
            _authController = new AuthController();

            // Thiết lập password character
            txtPassword.PasswordChar = '●';
            txtPassword.UseSystemPasswordChar = false;

            // Kết nối event handlers cho LinkLabels
            llRegister.LinkClicked += llRegister_LinkClicked;
            llForgotPassword.LinkClicked += llForgotPassword_LinkClicked;
            
            // Kết nối event handlers cho Button
            btnLogin.Click += btnLogin_Click;
            
            // Kết nối event handlers cho TextBox
            txtPassword.KeyDown += txtPassword_KeyDown;
            txtUserName.KeyDown += txtUserName_KeyDown;
            
            // Kết nối event handler cho Form Load
            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Focus vào ô username khi form load
            txtUserName.Focus();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            await PerformLogin();
        }

        private async Task PerformLogin()
        {
            try
            {
                // Disable button để tránh click nhiều lần
                btnLogin.Enabled = false;
                btnLogin.Text = "Đang đăng nhập...";

                // Gọi Controller để xử lý đăng nhập
                var result = await _authController.LoginAsync(txtUserName.Text, txtPassword.Text);

                if (result.Success)
                {
                    // Hiển thị thông báo thành công
                    MessageBox.Show(result.Message, "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở form Dashboard
                    NavigateToDashboard();
                }
                else
                {
                    // Hiển thị thông báo lỗi
                    MessageBox.Show(result.Message, result.Success ? "Thông báo" : "Đăng nhập thất bại", 
                        MessageBoxButtons.OK, 
                        result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                    // Focus vào field cần thiết
                    if (result.FieldToFocus == "username")
                    {
                        txtUserName.Focus();
                    }
                    else if (result.FieldToFocus == "password")
                    {
                        txtPassword.Text = "";
                        txtPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Enable lại button
                btnLogin.Enabled = true;
                btnLogin.Text = "Đăng nhập";
            }
        }

        private void llRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Mở form đăng ký
            NavigateToRegister();
        }

        private void llForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Mở form quên mật khẩu
            NavigateToForgotPassword();
        }

        /// <summary>
        /// Điều hướng đến form đăng ký
        /// </summary>
        private void NavigateToRegister()
        {
            try
            {
                var registerForm = new RegisterForm();
                this.Hide();
                
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu đăng ký thành công, có thể tự động điền username
                    // Hoặc chỉ hiển thị lại form đăng nhập
                    this.Show();
                    txtUserName.Focus();
                }
                else
                {
                    // Người dùng đóng form hoặc quay lại
                    this.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form đăng ký: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        /// <summary>
        /// Điều hướng đến form quên mật khẩu
        /// </summary>
        private void NavigateToForgotPassword()
        {
            try
            {
                var forgotPasswordForm = new ForgotPasswordForm();
                this.Hide();
                
                forgotPasswordForm.ShowDialog();
                
                // Quay lại form đăng nhập
                this.Show();
                txtUserName.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form quên mật khẩu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        /// <summary>
        /// Điều hướng đến form Dashboard sau khi đăng nhập thành công
        /// </summary>
        private void NavigateToDashboard()
        {
            try
            {
                // Set DialogResult để Program.cs biết đăng nhập thành công
                this.DialogResult = DialogResult.OK;
                
                // Đóng form đăng nhập
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            // Cho phép đăng nhập bằng phím Enter
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                await PerformLogin();
            }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            // Cho phép chuyển sang ô password bằng phím Enter
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                txtPassword.Focus();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Dispose Controller khi form đóng
            _authController?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
