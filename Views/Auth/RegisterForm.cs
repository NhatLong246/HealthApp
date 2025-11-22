using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Controllers;

namespace HealthApp.Views.Auth
{
    public partial class RegisterForm : Form
    {
        private readonly RegisterController _registerController;

        public RegisterForm()
        {
            InitializeComponent();
            
            // Thiết lập form hiển thị ở giữa màn hình
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Khởi tạo Controller
            _registerController = new RegisterController();

            // Thiết lập password character
            txtPassWord.PasswordChar = '●';
            txtPassWord.UseSystemPasswordChar = false;
            txtConfirmPassword.PasswordChar = '●';
            txtConfirmPassword.UseSystemPasswordChar = false;

            // Thiết lập DateTimePicker cho ngày sinh
            SetupBirthDatePicker();

            // Thiết lập giới tính
            SetupGenderComboBox();

            // Kết nối event handlers cho LinkLabel
            lnkLogin.LinkClicked += lnkLogin_LinkClicked;
            
            // Kết nối event handlers cho Buttons
            btnRegister.Click += btnRegister_Click;
            btnReset.Click += btnReset_Click;
            
            // Kết nối event handlers cho TextBox
            txtPhoneNumber.KeyPress += txtPhoneNumber_KeyPress;
            txtPhoneNumber.TextChanged += txtPhoneNumber_TextChanged;
            
            // Kết nối event handler cho Form Load
            this.Load += RegisterForm_Load;
        }

        private void SetupBirthDatePicker()
        {
            // Thiết lập các thuộc tính cho dtpNgaySinh
            if (dtpNgaySinh != null)
            {
                dtpNgaySinh.Format = DateTimePickerFormat.Short;
                dtpNgaySinh.MaxDate = DateTime.Now.AddYears(-13); // Tối đa là 13 năm trước
                dtpNgaySinh.MinDate = new DateTime(1950, 1, 1);
                dtpNgaySinh.Value = DateTime.Now.AddYears(-18); // Mặc định 18 tuổi
            }
        }

        private void SetupGenderComboBox()
        {
            cboGender.Items.Clear();
            cboGender.Items.Add("Nam");
            cboGender.Items.Add("Nữ");
            cboGender.Items.Add("Khác");
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            // Focus vào ô đầu tiên
            txtFullName.Focus();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            await PerformRegister();
        }

        private async Task PerformRegister()
        {
            try
            {
                // Disable button để tránh click nhiều lần
                btnRegister.Enabled = false;
                btnRegister.Text = "Đang đăng ký...";

                // Gọi Controller để xử lý đăng ký
                var result = await _registerController.RegisterAsync(
                    txtUserName.Text,
                    txtPassWord.Text,
                    txtConfirmPassword.Text,
                    txtGmail.Text,
                    txtPhoneNumber.Text,
                    txtFullName.Text,
                    dtpNgaySinh?.Value,
                    cboGender.SelectedItem?.ToString(),
                    txtAddress.Text
                );

                if (result.Success)
                {
                    // Hiển thị thông báo thành công
                    MessageBox.Show(result.Message, "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Đóng form đăng ký và quay về form đăng nhập
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Hiển thị thông báo lỗi
                    MessageBox.Show(result.Message, "Đăng ký thất bại", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Focus vào field có lỗi
                    FocusField(result.FieldToFocus);
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
                btnRegister.Enabled = true;
                btnRegister.Text = "Đăng Ký";
            }
        }

        private void FocusField(string fieldName)
        {
            switch (fieldName?.ToLower())
            {
                case "username":
                    txtUserName.Focus();
                    break;
                case "password":
                    txtPassWord.Focus();
                    break;
                case "confirmpassword":
                    txtConfirmPassword.Focus();
                    break;
                case "email":
                    txtGmail.Focus();
                    break;
                case "phonenumber":
                    txtPhoneNumber.Focus();
                    break;
                case "fullname":
                    txtFullName.Focus();
                    break;
                case "birthdate":
                    dtpNgaySinh?.Focus();
                    break;
                case "gender":
                    cboGender.Focus();
                    break;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Clear tất cả các field
            txtFullName.Text = "";
            txtUserName.Text = "";
            txtGmail.Text = "";
            txtPhoneNumber.Text = "";
            txtPassWord.Text = "";
            txtConfirmPassword.Text = "";
            txtAddress.Text = "";
            cboGender.SelectedIndex = -1;
            if (dtpNgaySinh != null)
                dtpNgaySinh.Value = DateTime.Now.AddYears(-18);

            // Focus vào ô đầu tiên
            txtFullName.Focus();
        }

        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Quay về form đăng nhập
            NavigateToLogin();
        }

        /// <summary>
        /// Điều hướng về form đăng nhập
        /// </summary>
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

        private void txtPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtPhoneNumber_TextChanged(object sender, EventArgs e)
        {
            // Giới hạn 10 chữ số
            if (txtPhoneNumber.Text.Length > 10)
            {
                txtPhoneNumber.Text = txtPhoneNumber.Text.Substring(0, 10);
                txtPhoneNumber.SelectionStart = txtPhoneNumber.Text.Length;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Dispose Controller khi form đóng
            _registerController?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
