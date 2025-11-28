using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Controllers;
using HealthApp.Views.Nutrition;

namespace HealthApp.Views.Dashboard
{
    public partial class ucDashBoard : UserControl
    {
        private frmDashBoard _parentForm;

        public ucDashBoard()
        {
            InitializeComponent();
        }

        public ucDashBoard(frmDashBoard parentForm) : this()
        {
            _parentForm = parentForm;
        }

        private void ucDashBoard_Load(object sender, EventArgs e)
        {
            AttachMealPlanNavigation(pnlKeHoachAnUong);
            AttachMealPlanNavigation(label9);
            AttachMealPlanNavigation(guna2PictureBox1);

            btnTinhNhanhBMITDEE.Click -= BtnTinhNhanhBMITDEE_Click;
            btnTinhNhanhBMITDEE.Click += BtnTinhNhanhBMITDEE_Click;

            LoadUserGreeting();
            LoadGoalSummary();
        }

        private void AttachMealPlanNavigation(Control control)
        {
            if (control == null) return;
            control.Cursor = Cursors.Hand;
            control.Click -= NavigateToMealPlan;
            control.Click += NavigateToMealPlan;
        }

        private void NavigateToMealPlan(object sender, EventArgs e)
        {
            var parentForm = _parentForm
                ?? this.FindForm() as frmDashBoard
                ?? Application.OpenForms.OfType<frmDashBoard>().FirstOrDefault();

            if (parentForm != null)
            {
                parentForm.LoadUserControl(new ucCheDoAnUongDeXuat());
            }
            else
            {
                MessageBox.Show("Không thể mở trang kế hoạch ăn uống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnTinhNhanhBMITDEE_Click(object sender, EventArgs e)
        {
            if (!TryParseInput(guna2TextBox1.Text, out double heightCm) || heightCm <= 0)
            {
                ShowInvalidMessage("chiều cao (cm)");
                return;
            }

            if (!TryParseInput(guna2TextBox2.Text, out double weightKg) || weightKg <= 0)
            {
                ShowInvalidMessage("cân nặng (kg)");
                return;
            }

            if (!TryParseInput(guna2TextBox3.Text, out double age) || age <= 0)
            {
                ShowInvalidMessage("tuổi");
                return;
            }

            var gender = (guna2ComboBox1.SelectedItem as string)?.Trim();
            if (string.IsNullOrEmpty(gender))
            {
                MessageBox.Show("Vui lòng chọn giới tính.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var activityFactor = GetActivityFactor(guna2ComboBox2.SelectedItem as string);
            if (activityFactor <= 0)
            {
                MessageBox.Show("Vui lòng chọn mức độ lao động.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var heightM = heightCm / 100d;
            var bmi = weightKg / (heightM * heightM);

            var bmr = CalculateBmr(gender, weightKg, heightCm, age);
            var tdee = bmr * activityFactor;

            lblKetQuaTinhBMI.Text = bmi.ToString("0.0", CultureInfo.InvariantCulture);
            lblKetQuaTinhTDEE.Text = Math.Round(tdee).ToString("0", CultureInfo.InvariantCulture);
        }

        private static double CalculateBmr(string gender, double weightKg, double heightCm, double age)
        {
            var baseValue = (10 * weightKg) + (6.25 * heightCm) - (5 * age);
            return gender.Equals("Nam", StringComparison.OrdinalIgnoreCase)
                ? baseValue + 5
                : baseValue - 161;
        }

        private static double GetActivityFactor(string selection)
        {
            if (string.IsNullOrWhiteSpace(selection))
            {
                return 0;
            }

            var trimmed = selection.Trim();

            if (trimmed.StartsWith("Ít", StringComparison.OrdinalIgnoreCase))
            {
                return 1.2;
            }

            if (trimmed.StartsWith("Hoạt Động Nhẹ", StringComparison.OrdinalIgnoreCase))
            {
                return 1.375;
            }

            if (trimmed.StartsWith("Hoạt Động Vừa", StringComparison.OrdinalIgnoreCase))
            {
                return 1.55;
            }

            if (trimmed.StartsWith("Hoạt Động Cao", StringComparison.OrdinalIgnoreCase))
            {
                return 1.725;
            }

            if (trimmed.StartsWith("Hoạt Động Rất Cao", StringComparison.OrdinalIgnoreCase))
            {
                return 1.9;
            }

            return 0;
        }

        private static bool TryParseInput(string text, out double value)
        {
            var normalized = (text ?? string.Empty).Trim()
                .Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)
                .Replace(" ", string.Empty);

            return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static void ShowInvalidMessage(string fieldName)
        {
            MessageBox.Show($"Giá trị {fieldName} không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void LoadUserGreeting()
        {
            if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
            {
                lblTenNgDung.Text = "Khách mới";
                lblMucTieuTuanNay.Text = "Đăng nhập để đồng bộ mục tiêu.";
                return;
            }

            var user = CurrentUser.User;
            lblTenNgDung.Text = string.IsNullOrWhiteSpace(user.HoTen)
                ? user.Username
                : user.HoTen;
        }

        private void LoadGoalSummary()
        {
            if (!CurrentUser.IsLoggedIn || string.IsNullOrWhiteSpace(CurrentUser.UserID))
            {
                lblMucTieuTuanNay.Text = "Đăng nhập để theo dõi mục tiêu.";
                return;
            }

            try
            {
                using (var goalController = new GoalController())
                {
                    var goals = goalController.GetGoalsByUser(CurrentUser.UserID, "Đang thực hiện");
                    var activeGoal = goals?.FirstOrDefault();

                    if (activeGoal != null)
                    {
                        var name = !string.IsNullOrWhiteSpace(activeGoal.TenMucTieu)
                            ? activeGoal.TenMucTieu
                            : activeGoal.LoaiMucTieu;

                        var deadline = activeGoal.NgayKetThucDuKien.ToString("dd/MM", CultureInfo.InvariantCulture);
                        lblMucTieuTuanNay.Text = string.IsNullOrWhiteSpace(deadline)
                            ? $"Mục tiêu tuần này: {name}"
                            : $"Mục tiêu tuần này: {name} (đến {deadline})";
                    }
                    else
                    {
                        lblMucTieuTuanNay.Text = "Bạn chưa có mục tiêu tuần này.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMucTieuTuanNay.Text = "Không thể tải mục tiêu.";
                System.Diagnostics.Debug.WriteLine($"LoadGoalSummary error: {ex.Message}");
            }
        }
    }
}
