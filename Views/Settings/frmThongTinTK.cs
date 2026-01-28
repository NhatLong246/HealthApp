using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
    public partial class frmThongTinTK : Form
    {
        private readonly WF_HealthTracker _context;
        private readonly IUserRepository _userRepository;
        private readonly HealthApp.Views.Dashboard.frmDashBoard1 _parentDashboard;
        private Users _currentUser;
        private string _originalEmail; // Lưu email ban đầu để so sánh
        private string _originalSDT; // Lưu số điện thoại ban đầu để so sánh
        private bool _isEditMode = false; // Trạng thái chỉnh sửa

        public frmThongTinTK(HealthApp.Views.Dashboard.frmDashBoard1 parentDashboard = null)
        {
            InitializeComponent();
            _context = new WF_HealthTracker();
            _userRepository = new UserRepository(_context);
            _parentDashboard = parentDashboard;
            InitializeEventHandlers();
            LoadUserInfo();
        }

        private void InitializeEventHandlers()
        {
            btnAnhDaiDien.Click += BtnAnhDaiDien_Click;
            btnCapNhat.Click += BtnCapNhat_Click;
            btnDoiMatKhau.Click += BtnDoiMatKhau_Click;
            this.Load += FrmThongTinTK_Load;
            
            // Thêm event handlers cho validation real-time
            txtEmail.TextChanged += TxtEmail_TextChanged;
            txtSDT.TextChanged += TxtSDT_TextChanged;
            txtEmail.Leave += TxtEmail_Leave;
            txtSDT.Leave += TxtSDT_Leave;
        }

        private void FrmThongTinTK_Load(object sender, EventArgs e)
        {
            // Set txtTenDangNhap là read-only
            txtTenDangNhap.ReadOnly = true;
            txtTenDangNhap.FillColor = Color.FromArgb(240, 240, 240);

            // Ẩn btnCapNhat nếu user là PT
            if (_currentUser != null && _currentUser.Role == "PT")
            {
                btnCapNhat.Visible = false;
            }
            else
            {
                // Ban đầu các textbox là read-only
                SetTextBoxesReadOnly(true);
            }

            // Ẩn các label lỗi ban đầu
            lblEmailError.Text = "";
            lblSDTError.Text = "";
        }

        /// <summary>
        /// Set tất cả textbox (trừ txtTenDangNhap) là read-only hoặc editable
        /// </summary>
        private void SetTextBoxesReadOnly(bool readOnly)
        {
            txtHovaTen.ReadOnly = readOnly;
            txtEmail.ReadOnly = readOnly;
            txtSDT.ReadOnly = readOnly;
            txtAnhDaiDien.ReadOnly = readOnly;
            btnAnhDaiDien.Enabled = !readOnly;

            Color fillColor = readOnly ? Color.FromArgb(240, 240, 240) : Color.White;
            txtHovaTen.FillColor = fillColor;
            txtEmail.FillColor = fillColor;
            txtSDT.FillColor = fillColor;
            txtAnhDaiDien.FillColor = fillColor;
        }

        private void LoadUserInfo()
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem thông tin tài khoản!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                // Load user từ database để có dữ liệu mới nhất
                var userId = CurrentUser.UserID;
                _currentUser = _context.Users.FirstOrDefault(u => u.UserID == userId);

                if (_currentUser == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Load thông tin vào các textbox
                txtHovaTen.Text = _currentUser.HoTen ?? "";
                txtEmail.Text = _currentUser.Email ?? "";
                txtSDT.Text = _currentUser.SDT ?? "";
                txtAnhDaiDien.Text = _currentUser.AnhDaiDien ?? "";
                txtTenDangNhap.Text = _currentUser.Username ?? "";

                // Lưu email và số điện thoại ban đầu để so sánh
                _originalEmail = _currentUser.Email ?? "";
                _originalSDT = _currentUser.SDT ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thông tin người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAnhDaiDien_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFilePath = openFileDialog.FileName;
                        txtAnhDaiDien.Text = selectedFilePath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn ảnh: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            if (!_isEditMode)
            {
                // Chuyển sang chế độ chỉnh sửa
                _isEditMode = true;
                btnCapNhat.Text = "Lưu";
                SetTextBoxesReadOnly(false);
            }
            else
            {
                // Lưu dữ liệu
                _ = SaveUserInfoAsync();
            }
        }

        private async Task SaveUserInfoAsync()
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Clear các lỗi trước khi validate
                ClearValidationErrors();

                // Validation: Họ tên
                string hoTen = txtHovaTen.Text?.Trim();
                if (string.IsNullOrWhiteSpace(hoTen))
                {
                    MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtHovaTen.Focus();
                    return;
                }

                // Validation: Email
                string email = txtEmail.Text?.Trim();
                if (string.IsNullOrWhiteSpace(email))
                {
                    ShowEmailError("Vui lòng nhập email!");
                    txtEmail.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidGmail(email))
                {
                    ShowEmailError("Email phải là địa chỉ @gmail.com!");
                    txtEmail.Focus();
                    return;
                }

                // Kiểm tra email không được trùng với email khác trong database (trừ email hiện tại)
                if (email != _originalEmail)
                {
                    if (await _userRepository.EmailExistsAsync(email))
                    {
                        ShowEmailError("Email này đã được sử dụng bởi tài khoản khác!");
                        txtEmail.Focus();
                        return;
                    }
                }

                // Validation: Số điện thoại
                string sdt = txtSDT.Text?.Trim();
                if (string.IsNullOrWhiteSpace(sdt))
                {
                    ShowSDTError("Vui lòng nhập số điện thoại!");
                    txtSDT.Focus();
                    return;
                }

                if (!ValidationHelper.IsValidPhoneNumber(sdt))
                {
                    ShowSDTError("Số điện thoại phải có đúng 10 chữ số!");
                    txtSDT.Focus();
                    return;
                }

                // Kiểm tra số điện thoại không được trùng với số điện thoại khác trong database (trừ số điện thoại hiện tại)
                if (sdt != _originalSDT)
                {
                    if (await _userRepository.PhoneExistsAsync(sdt))
                    {
                        ShowSDTError("Số điện thoại này đã được sử dụng bởi tài khoản khác!");
                        txtSDT.Focus();
                        return;
                    }
                }

                // Lấy đường dẫn ảnh đại diện (có thể rỗng)
                string anhDaiDien = txtAnhDaiDien.Text?.Trim();

                // Cập nhật thông tin user
                _currentUser.HoTen = hoTen;
                _currentUser.Email = email;
                _currentUser.SDT = sdt;
                _currentUser.AnhDaiDien = string.IsNullOrWhiteSpace(anhDaiDien) ? null : anhDaiDien;

                // Lưu vào database
                _context.SaveChanges();

                // Reload user từ database để có dữ liệu mới nhất (bao gồm AnhDaiDien)
                var updatedUser = _context.Users.FirstOrDefault(u => u.UserID == _currentUser.UserID);
                if (updatedUser != null)
                {
                    // Cập nhật CurrentUser với dữ liệu mới nhất từ database
                    CurrentUser.User.HoTen = updatedUser.HoTen;
                    CurrentUser.User.Email = updatedUser.Email;
                    CurrentUser.User.SDT = updatedUser.SDT;
                    CurrentUser.User.AnhDaiDien = updatedUser.AnhDaiDien; // Có thể là null nếu đã xóa
                }

                // Cập nhật ảnh đại diện trong Dashboard nếu có parent
                if (_parentDashboard != null)
                {
                    _parentDashboard.ReloadUserInfo();
                }

                MessageBox.Show("Cập nhật thông tin thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cập nhật _originalEmail và _originalSDT sau khi lưu thành công
                _originalEmail = email;
                _originalSDT = sdt;

                // Quay lại chế độ xem
                _isEditMode = false;
                btnCapNhat.Text = "Cập Nhật";
                SetTextBoxesReadOnly(true);
                ClearValidationErrors();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật thông tin: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtEmail_TextChanged(object sender, EventArgs e)
        {
            if (_isEditMode)
            {
                ValidateEmail();
            }
        }

        private void TxtEmail_Leave(object sender, EventArgs e)
        {
            if (_isEditMode)
            {
                ValidateEmail();
            }
        }

        private async void ValidateEmail()
        {
            string email = txtEmail.Text?.Trim();
            
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowEmailError("Vui lòng nhập email!");
                return;
            }

            if (!ValidationHelper.IsValidGmail(email))
            {
                ShowEmailError("Email phải là địa chỉ @gmail.com!");
                return;
            }

            // Kiểm tra email trùng (chỉ khi khác email hiện tại)
            if (email != _originalEmail)
            {
                if (await _userRepository.EmailExistsAsync(email))
                {
                    ShowEmailError("Email này đã được sử dụng bởi tài khoản khác!");
                    return;
                }
            }

            // Email hợp lệ
            ClearEmailError();
        }

        private void TxtSDT_TextChanged(object sender, EventArgs e)
        {
            if (_isEditMode)
            {
                ValidateSDT();
            }
        }

        private void TxtSDT_Leave(object sender, EventArgs e)
        {
            if (_isEditMode)
            {
                ValidateSDT();
            }
        }

        private async void ValidateSDT()
        {
            string sdt = txtSDT.Text?.Trim();
            
            if (string.IsNullOrWhiteSpace(sdt))
            {
                ShowSDTError("Vui lòng nhập số điện thoại!");
                return;
            }

            if (!ValidationHelper.IsValidPhoneNumber(sdt))
            {
                ShowSDTError("Số điện thoại phải có đúng 10 chữ số!");
                return;
            }

            // Kiểm tra số điện thoại trùng (chỉ khi khác số điện thoại hiện tại)
            if (sdt != _originalSDT)
            {
                if (await _userRepository.PhoneExistsAsync(sdt))
                {
                    ShowSDTError("Số điện thoại này đã được sử dụng bởi tài khoản khác!");
                    return;
                }
            }

            // SDT hợp lệ
            ClearSDTError();
        }

        private void ShowEmailError(string message)
        {
            lblEmailError.Text = message;
            lblEmailError.ForeColor = Color.Red;
        }

        private void ShowSDTError(string message)
        {
            lblSDTError.Text = message;
            lblSDTError.ForeColor = Color.Red;
        }

        private void ClearEmailError()
        {
            lblEmailError.Text = "";
        }

        private void ClearSDTError()
        {
            lblSDTError.Text = "";
        }

        private void ClearValidationErrors()
        {
            ClearEmailError();
            ClearSDTError();
        }

        private void BtnDoiMatKhau_Click(object sender, EventArgs e)
        {
            try
            {
                var frmDoiMatKhau = new DoiMatKhau();
                frmDoiMatKhau.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form đổi mật khẩu: {ex.Message}", "Lỗi",
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
