extern alias ef6;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Models;
using ef6::System.Data.Entity;

namespace HealthApp.Views.Admin
{
    public partial class ThemMonAn : Form
    {
        private WF_HealthTracker _dbContext;
        public bool IsSaved { get; private set; } = false;

        public ThemMonAn()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            InitializeControls();
        }

        /// <summary>
        /// Khởi tạo các controls
        /// </summary>
        private void InitializeControls()
        {
            // Load dữ liệu cho combobox
            LoadComboBoxData();
            
            // Event handlers
            if (btnXacNhan != null)
                btnXacNhan.Click += BtnXacNhan_Click;
            
            if (btnHuy != null)
                btnHuy.Click += BtnHuy_Click;

            // Khi người dùng sửa ô → bỏ viền đỏ
            if (txtTenMonAn != null) txtTenMonAn.TextChanged += (s, e) => SetTextBoxBorder(txtTenMonAn, BorderColorNormal);
            if (txtSoCalories != null) txtSoCalories.TextChanged += (s, e) => SetTextBoxBorder(txtSoCalories, BorderColorNormal);
            if (txtKhoiLuongChuan != null) txtKhoiLuongChuan.TextChanged += (s, e) => SetTextBoxBorder(txtKhoiLuongChuan, BorderColorNormal);
            if (txtProtein != null) txtProtein.TextChanged += (s, e) => SetTextBoxBorder(txtProtein, BorderColorNormal);
            if (txtCarb != null) txtCarb.TextChanged += (s, e) => SetTextBoxBorder(txtCarb, BorderColorNormal);
            if (txtFat != null) txtFat.TextChanged += (s, e) => SetTextBoxBorder(txtFat, BorderColorNormal);
            if (txtFiber != null) txtFiber.TextChanged += (s, e) => SetTextBoxBorder(txtFiber, BorderColorNormal);
            if (cboLoai != null) cboLoai.SelectedIndexChanged += (s, e) => SetComboBoxBorder(cboLoai, BorderColorNormal);
            if (cboDonVi != null) cboDonVi.SelectedIndexChanged += (s, e) => SetComboBoxBorder(cboDonVi, BorderColorNormal);
        }

        /// <summary>
        /// Load dữ liệu cho các combobox
        /// </summary>
        private void LoadComboBoxData()
        {
            try
            {
                // Load đơn vị (trùng với cột Donvi trong ThuVienMonAn: thường là "g")
                if (cboDonVi != null)
                {
                    cboDonVi.Items.Clear();
                    cboDonVi.Items.AddRange(new[] { "g", "ml" });
                    if (cboDonVi.Items.Count > 0)
                        cboDonVi.SelectedIndex = 0;
                }

                // Load loại món ăn (cột Loai trong ThuVienMonAn) từ DB hiện có
                if (cboLoai != null)
                {
                    cboLoai.Items.Clear();

                    var loaiList = _dbContext.ThuVienMonAn
                        .Where(ma => ma.Loai != null && ma.Loai != "")
                        .Select(ma => ma.Loai)
                        .Distinct()
                        .OrderBy(loai => loai)
                        .ToList();

                    foreach (var loai in loaiList)
                    {
                        cboLoai.Items.Add(loai);
                    }

                    if (cboLoai.Items.Count > 0)
                        cboLoai.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ThemMonAn] LoadComboBoxData error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý khi click nút Xác nhận
        /// </summary>
        private void BtnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate dữ liệu
                if (!ValidateInput())
                {
                    return;
                }

                // Tạo món ăn mới (mapping đúng schema bảng ThuVienMonAn)
                var newMonAn = new ThuVienMonAn
                {
                    MonAnID = GenerateMonAnID(),
                    TenMonAn = txtTenMonAn?.Text?.Trim() ?? "",
                    Loai = cboLoai?.SelectedItem?.ToString(),
                    Donvi = cboDonVi?.SelectedItem?.ToString(),
                    KhoiLuongChuan = ParseDouble(txtKhoiLuongChuan?.Text),
                    Calories = ParseDouble(txtSoCalories?.Text),
                    Protein = ParseDouble(txtProtein?.Text),
                    Carbs = ParseDouble(txtCarb?.Text),
                    Fat = ParseDouble(txtFat?.Text),
                    Fiber = ParseDouble(txtFiber?.Text),
                    imageURL = null, // Có thể thêm field để nhập URL ảnh nếu cần
                    NgayTao = DateTime.Now
                };

                // Thêm vào database
                _dbContext.ThuVienMonAn.Add(newMonAn);
                _dbContext.SaveChanges();

                MessageBox.Show("Thêm món ăn thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                IsSaved = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Lấy message chi tiết nhất có thể
                string errorMessage = ex.Message;
                Exception innerEx = ex.InnerException;
                int depth = 0;
                
                // Đi sâu vào các inner exception để lấy thông tin chi tiết
                while (innerEx != null && depth < 3)
                {
                    errorMessage += $"\nChi tiết (cấp {depth + 1}): {innerEx.Message}";
                    innerEx = innerEx.InnerException;
                    depth++;
                }

                // Kiểm tra xem có phải lỗi database không
                if (ex.Message.Contains("foreign key") || ex.Message.Contains("constraint") || 
                    ex.Message.Contains("cannot insert") || ex.Message.Contains("violation"))
                {
                    errorMessage = $"Lỗi database:\n{errorMessage}";
                }

                MessageBox.Show($"Lỗi khi thêm món ăn:\n{errorMessage}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ThemMonAn] BtnXacNhan_Click error: {errorMessage}\n{ex.StackTrace}");
            }
        }

        private static readonly Color BorderColorError = Color.Red;
        private static readonly Color BorderColorNormal = Color.FromArgb(208, 208, 208);

        /// <summary>
        /// Reset viền tất cả ô về bình thường (trước khi validate)
        /// </summary>
        private void ResetAllBorders()
        {
            SetTextBoxBorder(txtTenMonAn, BorderColorNormal);
            SetTextBoxBorder(txtSoCalories, BorderColorNormal);
            SetTextBoxBorder(txtKhoiLuongChuan, BorderColorNormal);
            SetTextBoxBorder(txtProtein, BorderColorNormal);
            SetTextBoxBorder(txtCarb, BorderColorNormal);
            SetTextBoxBorder(txtFat, BorderColorNormal);
            SetTextBoxBorder(txtFiber, BorderColorNormal);
            SetComboBoxBorder(cboLoai, BorderColorNormal);
            SetComboBoxBorder(cboDonVi, BorderColorNormal);
        }

        private void SetTextBoxBorder(Guna.UI2.WinForms.Guna2TextBox ctrl, Color color)
        {
            if (ctrl == null) return;
            ctrl.BorderColor = color;
            ctrl.FocusedState.BorderColor = color;
            ctrl.HoverState.BorderColor = color;
        }

        private void SetComboBoxBorder(Guna.UI2.WinForms.Guna2ComboBox ctrl, Color color)
        {
            if (ctrl == null) return;
            ctrl.BorderColor = color;
            ctrl.FocusedState.BorderColor = color;
            ctrl.HoverState.BorderColor = color;
        }

        /// <summary>
        /// Validate dữ liệu đầu vào theo quy tắc: bắt buộc, kiểu dữ liệu, tên 3–100 ký tự, không trùng.
        /// </summary>
        private bool ValidateInput()
        {
            ResetAllBorders();

            bool hasError = false;

            // 1. Tên món ăn: bắt buộc, 3–100 ký tự, không phải toàn số, không trùng
            string tenMon = txtTenMonAn?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(tenMon))
            {
                SetTextBoxBorder(txtTenMonAn, BorderColorError);
                MessageBox.Show("Vui lòng nhập tên món ăn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenMonAn?.Focus();
                return false;
            }
            if (tenMon.Length < 3 || tenMon.Length > 100)
            {
                SetTextBoxBorder(txtTenMonAn, BorderColorError);
                MessageBox.Show("Tên món ăn phải từ 3 đến 100 ký tự!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenMonAn?.Focus();
                return false;
            }
            if (tenMon.All(char.IsDigit))
            {
                SetTextBoxBorder(txtTenMonAn, BorderColorError);
                MessageBox.Show("Tên món ăn không được chỉ gồm số!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenMonAn?.Focus();
                return false;
            }
            if (_dbContext.ThuVienMonAn.Any(ma => ma.TenMonAn != null && ma.TenMonAn.Trim().ToLower() == tenMon.ToLower()))
            {
                SetTextBoxBorder(txtTenMonAn, BorderColorError);
                MessageBox.Show("Tên món ăn này đã tồn tại trong hệ thống!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenMonAn?.Focus();
                return false;
            }

            // 2. Loại: bắt buộc
            string msgFirst = null;
            Control focusFirst = null;
            if (cboLoai == null || cboLoai.SelectedItem == null || string.IsNullOrWhiteSpace(cboLoai.SelectedItem.ToString()))
            {
                SetComboBoxBorder(cboLoai, BorderColorError);
                if (msgFirst == null) { msgFirst = "Vui lòng chọn loại món ăn!"; focusFirst = cboLoai; }
                hasError = true;
            }

            // 3. Đơn vị: bắt buộc
            if (cboDonVi == null || cboDonVi.SelectedItem == null || string.IsNullOrWhiteSpace(cboDonVi.SelectedItem.ToString()))
            {
                SetComboBoxBorder(cboDonVi, BorderColorError);
                if (msgFirst == null) { msgFirst = "Vui lòng chọn đơn vị!"; focusFirst = cboDonVi; }
                hasError = true;
            }

            // 4. Calories: bắt buộc, phải là số và > 0
            string calStr = txtSoCalories?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(calStr))
            {
                SetTextBoxBorder(txtSoCalories, BorderColorError);
                if (msgFirst == null) { msgFirst = "Vui lòng nhập Calories!"; focusFirst = txtSoCalories; }
                hasError = true;
            }
            else if (!double.TryParse(calStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double calVal) || calVal <= 0)
            {
                SetTextBoxBorder(txtSoCalories, BorderColorError);
                if (msgFirst == null) { msgFirst = "Calories phải là số lớn hơn 0!"; focusFirst = txtSoCalories; }
                hasError = true;
            }

            // 5. Khối lượng chuẩn: bắt buộc, không được null, phải là số > 0
            string klStr = txtKhoiLuongChuan?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(klStr))
            {
                SetTextBoxBorder(txtKhoiLuongChuan, BorderColorError);
                if (msgFirst == null) { msgFirst = "Vui lòng nhập Khối lượng chuẩn!"; focusFirst = txtKhoiLuongChuan; }
                hasError = true;
            }
            else if (!double.TryParse(klStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double klVal) || klVal <= 0)
            {
                SetTextBoxBorder(txtKhoiLuongChuan, BorderColorError);
                if (msgFirst == null) { msgFirst = "Khối lượng chuẩn phải là số lớn hơn 0!"; focusFirst = txtKhoiLuongChuan; }
                hasError = true;
            }

            if (hasError)
            {
                if (!string.IsNullOrEmpty(msgFirst))
                    MessageBox.Show(msgFirst, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                focusFirst?.Focus();
                return false;
            }

            // 6. Carb, Protein, Fat, Fiber: nếu có nhập thì phải là số >= 0
            if (!ValidateSoLieuLonHonHoacBangKhong(txtCarb, "Carb")) return false;
            if (!ValidateSoLieuLonHonHoacBangKhong(txtProtein, "Protein")) return false;
            if (!ValidateSoLieuLonHonHoacBangKhong(txtFat, "Fat")) return false;
            if (!ValidateSoLieuLonHonHoacBangKhong(txtFiber, "Fiber")) return false;

            return true;
        }

        /// <summary>
        /// Ô số tùy chọn: trống thì bỏ qua, có nhập thì phải là số >= 0
        /// </summary>
        private bool ValidateSoLieuLonHonHoacBangKhong(Guna.UI2.WinForms.Guna2TextBox ctrl, string tenTruong)
        {
            string v = ctrl?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(v)) return true;
            if (!double.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double num) || num < 0)
            {
                SetTextBoxBorder(ctrl, BorderColorError);
                MessageBox.Show($"{tenTruong} phải là số lớn hơn hoặc bằng 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ctrl?.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tạo ID mới cho món ăn
        /// </summary>
        private string GenerateMonAnID()
        {
            // Trong DB hiện tại, MonAnID có dạng "food_0001"
            var lastMonAn = _dbContext.ThuVienMonAn
                .OrderByDescending(ma => ma.MonAnID)
                .FirstOrDefault();

            if (lastMonAn == null || string.IsNullOrWhiteSpace(lastMonAn.MonAnID))
            {
                return "food_0001";
            }

            string lastID = lastMonAn.MonAnID;
            if (lastID.StartsWith("food_"))
            {
                string numberPart = lastID.Substring("food_".Length);
                if (int.TryParse(numberPart, out int number))
                {
                    return $"food_{(number + 1):D4}";
                }
            }

            // Fallback: tạo ID dựa trên số lượng
            int count = _dbContext.ThuVienMonAn.Count() + 1;
            return $"food_{count:D4}";
        }

        /// <summary>
        /// Parse string to double
        /// </summary>
        private double? ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            
            if (double.TryParse(value, out double result))
                return result;
            
            return null;
        }

        /// <summary>
        /// Xử lý khi click nút Hủy
        /// </summary>
        private void BtnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ThemMonAn_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dbContext?.Dispose();
        }
    }
}
