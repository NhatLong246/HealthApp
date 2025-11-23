using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Controllers;
using HealthApp.Common.Helpers;

namespace HealthApp.Views.Auth
{
    public partial class ForgotPasswordForm_OTP : Form
    {
        private readonly ForgotPasswordController _forgotPasswordController;
        private readonly string _email;
        private Guna.UI2.WinForms.Guna2TextBox[] _otpTextBoxes;

        public ForgotPasswordForm_OTP(string email)
        {
            InitializeComponent();
            
            _email = email;
            _forgotPasswordController = new ForgotPasswordController();
            
            // Thiết lập form hiển thị ở giữa màn hình
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Khởi tạo mảng textbox OTP
            _otpTextBoxes = new[]
            {
                txtNumberOTP1,
                txtNumberOTP2,
                txtNumberOTP3,
                txtNumberOTP4,
                txtNumberOTP5,
                txtNumberOTP6
            };
            
            // Thiết lập các textbox chỉ nhận 1 ký tự số
            SetupOTPTextBoxes();
            
            // Hiển thị email
            lbNameEmail.Text = email;
            
            // Kết nối event handlers
            btnSendOTP.Click += BtnSendOTP_Click;
            lnkBackLogin.LinkClicked += LnkBackLogin_LinkClicked;
            lnkChangeEmail.LinkClicked += LnkChangeEmail_LinkClicked;
            this.Load += ForgotPasswordForm_OTP_Load;
        }

        private void SetupOTPTextBoxes()
        {
            foreach (var txtBox in _otpTextBoxes)
            {
                txtBox.MaxLength = 1;
                txtBox.TextAlign = HorizontalAlignment.Center;
                txtBox.KeyPress += TxtOTP_KeyPress;
                txtBox.TextChanged += TxtOTP_TextChanged;
            }
        }

        private void ForgotPasswordForm_OTP_Load(object sender, EventArgs e)
        {
            if (txtNumberOTP1 != null)
                txtNumberOTP1.Focus();
        }

        private void TxtOTP_KeyPress(object sender, KeyPressEventArgs e)
        {
            var currentTextBox = sender as Guna.UI2.WinForms.Guna2TextBox;
            if (currentTextBox == null) return;

            int currentIndex = Array.IndexOf(_otpTextBoxes, currentTextBox);

            // Chỉ cho phép nhập số
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            // Nếu nhập số
            if (char.IsDigit(e.KeyChar))
            {
                // Set text ngay lập tức
                currentTextBox.Text = e.KeyChar.ToString();
                e.Handled = true; // Ngăn ký tự được nhập lại

                // Tự động chuyển sang textbox tiếp theo (bên phải)
                if (currentIndex < _otpTextBoxes.Length - 1)
                {
                    // Sử dụng BeginInvoke để đảm bảo chuyển focus sau khi text đã được set
                    this.BeginInvoke(new Action(() =>
                    {
                        _otpTextBoxes[currentIndex + 1].Focus();
                        _otpTextBoxes[currentIndex + 1].SelectAll(); // Select all để dễ dàng thay thế
                    }));
                }
                else
                {
                    // Nếu là ô cuối cùng, focus vào button
                    this.BeginInvoke(new Action(() =>
                    {
                        btnSendOTP.Focus();
                    }));
                }
            }
            else if (e.KeyChar == (char)Keys.Back)
            {
                // Nếu Backspace và ô hiện tại đang trống
                if (string.IsNullOrEmpty(currentTextBox.Text))
                {
                    e.Handled = true;
                    // Chuyển về textbox trước (bên trái) và xóa text
                    if (currentIndex > 0)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            _otpTextBoxes[currentIndex - 1].Focus();
                            _otpTextBoxes[currentIndex - 1].Text = "";
                            _otpTextBoxes[currentIndex - 1].SelectAll();
                        }));
                    }
                }
            }
        }

        private void TxtOTP_TextChanged(object sender, EventArgs e)
        {
            // Đảm bảo chỉ có 1 ký tự
            var txtBox = sender as Guna.UI2.WinForms.Guna2TextBox;
            if (txtBox != null && txtBox.Text.Length > 1)
            {
                txtBox.Text = txtBox.Text.Substring(txtBox.Text.Length - 1); // Lấy ký tự cuối cùng
            }
        }

        private string GetOTPCode()
        {
            return string.Join("", _otpTextBoxes.Select(txt => txt.Text ?? ""));
        }

        private async void BtnSendOTP_Click(object sender, EventArgs e)
        {
            try
            {
                string otpCode = GetOTPCode();

                if (otpCode.Length != 6)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ 6 chữ số OTP.", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNumberOTP1.Focus();
                    return;
                }

                // Disable button
                btnSendOTP.Enabled = false;
                btnSendOTP.Text = "Đang xác thực...";

                var result = await _forgotPasswordController.VerifyOTPAsync(_email, otpCode);

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Mở form đổi mật khẩu
                    this.Hide();
                    using (var changePasswordForm = new ChangePasswordForm(_email))
                    {
                        if (changePasswordForm.ShowDialog() == DialogResult.OK)
                        {
                            // Đặt lại mật khẩu thành công
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            // Người dùng hủy
                            this.Show();
                        }
                    }
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    // Xóa các textbox OTP
                    foreach (var txtBox in _otpTextBoxes)
                    {
                        txtBox.Text = "";
                    }
                    txtNumberOTP1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSendOTP.Enabled = true;
                btnSendOTP.Text = "Gửi mã OTP";
            }
        }

        private async void LnkBackLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Gửi lại mã OTP
            try
            {
                var result = await _forgotPasswordController.SendOTPAsync(_email);
                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Xóa các textbox OTP
                    foreach (var txtBox in _otpTextBoxes)
                    {
                        txtBox.Text = "";
                    }
                    txtNumberOTP1.Focus();
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
        }

        private void LnkChangeEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Quay lại form nhập email
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _forgotPasswordController?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
