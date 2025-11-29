using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Models;

namespace HealthApp.Views.Settings
{
    public partial class frmChangeInformationforNewuser : Form
    {
        private readonly bool _isMandatory;
        private bool _isSaved;

        public frmChangeInformationforNewuser(bool isMandatory = false)
        {
            InitializeComponent();
            _isMandatory = isMandatory;

            btnBoQua.Visible = !_isMandatory;
            ControlBox = !_isMandatory;

            btnXacNhanThongTin.Click += BtnXacNhanThongTin_Click;
            btnBoQua.Click += BtnBoQua_Click;
            Load += frmChangeInformationforNewuser_Load;
            FormClosing += FrmChangeInformationforNewuser_FormClosing;
        }

        private void frmChangeInformationforNewuser_Load(object sender, EventArgs e)
        {
            PopulateExistingData();
        }

        private void BtnBoQua_Click(object sender, EventArgs e)
        {
            _isSaved = true;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FrmChangeInformationforNewuser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isMandatory && !_isSaved && e.CloseReason == CloseReason.UserClosing)
            {
                MessageBox.Show("Vui lòng hoàn tất thông tin cơ bản trước khi tiếp tục.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        private void BtnXacNhanThongTin_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out double height, out double weight, out int age, out string gender, out string activityLevel))
            {
                return;
            }

            var user = CurrentUser.User;
            if (user == null)
            {
                MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var estimatedBirthday = EstimateBirthdayFromAge(age);

            try
            {
                using (var db = new WF_HealthTracker())
                {
                    var dbUser = db.Users.FirstOrDefault(u => u.UserID == user.UserID);
                    if (dbUser == null)
                    {
                        MessageBox.Show("Không tìm thấy người dùng trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    dbUser.GioiTinh = gender;
                    dbUser.NgaySinh = estimatedBirthday;

                    var latestRecord = db.TinhTrangTongQuan
                        .Where(t => t.UserID == user.UserID)
                        .OrderByDescending(t => t.NgayGhiNhan)
                        .FirstOrDefault();

                    if (latestRecord == null)
                    {
                        latestRecord = new TinhTrangTongQuan
                        {
                            BanGhiID = GenerateRecordId(),
                            UserID = user.UserID,
                            NgayGhiNhan = DateTime.Today
                        };

                        db.TinhTrangTongQuan.Add(latestRecord);
                    }
                    else
                    {
                        latestRecord.NgayCapNhat = DateTime.Now;
                    }

                    latestRecord.CanNang = weight;
                    latestRecord.ChieuCao = height;
                    latestRecord.BMI = Math.Round(weight / Math.Pow(height / 100d, 2), 2);
                    latestRecord.TrinhDoCaNhan = activityLevel;

                    db.SaveChanges();
                }

                // Update cached user info
                user.GioiTinh = gender;
                user.NgaySinh = estimatedBirthday;

                _isSaved = true;
                DialogResult = DialogResult.OK;

                MessageBox.Show("Đã lưu thông tin cơ bản thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể lưu thông tin: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateExistingData()
        {
            if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
            {
                return;
            }

            var user = CurrentUser.User;

            if (!string.IsNullOrWhiteSpace(user.GioiTinh))
            {
                cboGioiTinh.SelectedItem = cboGioiTinh.Items.Cast<object>()
                    .FirstOrDefault(i => string.Equals(i.ToString(), user.GioiTinh, StringComparison.OrdinalIgnoreCase));
            }

            if (user.NgaySinh.HasValue)
            {
                var age = Math.Max(0, DateTime.Today.Year - user.NgaySinh.Value.Year -
                    (DateTime.Today.DayOfYear < user.NgaySinh.Value.DayOfYear ? 1 : 0));
                txtTuoi.Text = age > 0 ? age.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }

            using (var db = new WF_HealthTracker())
            {
                var latestRecord = db.TinhTrangTongQuan
                    .Where(t => t.UserID == user.UserID)
                    .OrderByDescending(t => t.NgayGhiNhan)
                    .FirstOrDefault();

                if (latestRecord != null)
                {
                    txtChieuCao.Text = latestRecord.ChieuCao?.ToString("0.##", CultureInfo.InvariantCulture) ?? string.Empty;
                    txtCanNang.Text = latestRecord.CanNang?.ToString("0.##", CultureInfo.InvariantCulture) ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(latestRecord.TrinhDoCaNhan))
                    {
                        cboHoatDong.SelectedItem = cboHoatDong.Items.Cast<object>()
                            .FirstOrDefault(i => string.Equals(i.ToString(), latestRecord.TrinhDoCaNhan, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
        }

        private static DateTime EstimateBirthdayFromAge(int age)
        {
            var today = DateTime.Today;
            var estimatedYear = Math.Max(1900, today.Year - age);
            return new DateTime(estimatedYear, today.Month, today.Day);
        }

        private bool ValidateInputs(out double height, out double weight, out int age, out string gender, out string activityLevel)
        {
            height = weight = 0;
            age = 0;
            gender = string.Empty;
            activityLevel = string.Empty;

            if (!double.TryParse(txtChieuCao.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out height) || height <= 0)
            {
                MessageBox.Show("Chiều cao không hợp lệ (cm).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChieuCao.Focus();
                return false;
            }

            if (!double.TryParse(txtCanNang.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out weight) || weight <= 0)
            {
                MessageBox.Show("Cân nặng không hợp lệ (kg).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCanNang.Focus();
                return false;
            }

            if (!int.TryParse(txtTuoi.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out age) || age < 10 || age > 100)
            {
                MessageBox.Show("Tuổi phải nằm trong khoảng 10 - 100.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTuoi.Focus();
                return false;
            }

            gender = cboGioiTinh.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(gender))
            {
                MessageBox.Show("Vui lòng chọn giới tính.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboGioiTinh.Focus();
                return false;
            }

            activityLevel = cboHoatDong.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(activityLevel))
            {
                MessageBox.Show("Vui lòng chọn mức độ lao động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboHoatDong.Focus();
                return false;
            }

            return true;
        }

        private static string GenerateRecordId()
        {
            return $"rec_{Guid.NewGuid():N}".Substring(0, 20);
        }
    }
}
