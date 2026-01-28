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
using HealthApp.Common.Helpers;
using ef6::System.Data.Entity;

namespace HealthApp.Views.Admin
{
    public partial class ucSuaBT : UserControl
    {
        private WF_HealthTracker _dbContext;
        private ThuVienBaiTap _exercise;

        private static readonly Color BorderColorError = Color.Red;
        private static readonly Color BorderColorNormal = Color.FromArgb(208, 208, 208);
        
        // Events
        public event EventHandler OnSaveSuccess;
        public event EventHandler OnCancel;

        public ucSuaBT(ThuVienBaiTap exercise)
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            _exercise = exercise;
            InitializeControls();
            LoadExerciseData();
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

            if (btnBack != null)
                btnBack.Click += (s, e) => BtnHuy_Click(s, e);

            // Chặn nhập chữ/ký tự đặc biệt cho các trường số
            AttachNumericOnly(txtSoSet, allowDecimal: false);
            AttachNumericOnly(txtSoRep, allowDecimal: false);
            AttachNumericOnly(txtThoiGianNghi, allowDecimal: false);
            AttachNumericOnly(txtCaloUocTinh, allowDecimal: true);

            // Khi user sửa ô → bỏ viền đỏ
            if (txtTenBT != null) txtTenBT.TextChanged += (s, e) => SetTextBoxBorder(txtTenBT, BorderColorNormal);
            if (txtSoSet != null) txtSoSet.TextChanged += (s, e) => SetTextBoxBorder(txtSoSet, BorderColorNormal);
            if (txtSoRep != null) txtSoRep.TextChanged += (s, e) => SetTextBoxBorder(txtSoRep, BorderColorNormal);
            if (txtThoiGianNghi != null) txtThoiGianNghi.TextChanged += (s, e) => SetTextBoxBorder(txtThoiGianNghi, BorderColorNormal);
            if (txtCaloUocTinh != null) txtCaloUocTinh.TextChanged += (s, e) => SetTextBoxBorder(txtCaloUocTinh, BorderColorNormal);
            if (cboNhomCo != null) cboNhomCo.SelectedIndexChanged += (s, e) => SetComboBoxBorder(cboNhomCo, BorderColorNormal);
            if (cboDoKho != null) cboDoKho.SelectedIndexChanged += (s, e) => SetComboBoxBorder(cboDoKho, BorderColorNormal);
        }

        private void ResetAllBorders()
        {
            SetTextBoxBorder(txtTenBT, BorderColorNormal);
            SetTextBoxBorder(txtSoSet, BorderColorNormal);
            SetTextBoxBorder(txtSoRep, BorderColorNormal);
            SetTextBoxBorder(txtThoiGianNghi, BorderColorNormal);
            SetTextBoxBorder(txtCaloUocTinh, BorderColorNormal);
            SetComboBoxBorder(cboNhomCo, BorderColorNormal);
            SetComboBoxBorder(cboDoKho, BorderColorNormal);
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

        private void AttachNumericOnly(Guna.UI2.WinForms.Guna2TextBox ctrl, bool allowDecimal)
        {
            if (ctrl == null) return;
            ctrl.KeyPress += (s, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;

                if (allowDecimal)
                {
                    if (e.KeyChar == '.' || e.KeyChar == ',')
                    {
                        if (ctrl.Text.Contains(".") || ctrl.Text.Contains(",")) e.Handled = true;
                        return;
                    }
                }

                if (!char.IsDigit(e.KeyChar)) e.Handled = true;
            };
        }

        /// <summary>
        /// Load dữ liệu bài tập vào form
        /// </summary>
        private void LoadExerciseData()
        {
            if (_exercise == null)
                return;

            try
            {
                // Load dữ liệu vào các controls
                if (txtTenBT != null)
                    txtTenBT.Text = _exercise.TenBaiTap ?? "";

                if (cboNhomCo != null)
                {
                    int index = cboNhomCo.Items.IndexOf(_exercise.NhomCoChinhNhat);
                    if (index >= 0)
                        cboNhomCo.SelectedIndex = index;
                }

                if (cboDoKho != null)
                {
                    int index = cboDoKho.Items.IndexOf(_exercise.CapDo);
                    if (index >= 0)
                        cboDoKho.SelectedIndex = index;
                }

                if (txtThietBi != null)
                    txtThietBi.Text = _exercise.DungCu ?? "";

                if (txtMoTa != null)
                    txtMoTa.Text = _exercise.MoTa ?? "";

                if (txtHuongDan != null)
                    txtHuongDan.Text = _exercise.HuongDan ?? "";

                if (txtLinkAnh != null)
                    txtLinkAnh.Text = _exercise.AnhMinhHoa ?? "";

                if (txtLinkVideo != null)
                    txtLinkVideo.Text = _exercise.VideoHuongDan ?? "";

                if (txtCaloUocTinh != null)
                    txtCaloUocTinh.Text = _exercise.CaloriesMoiRep?.ToString() ?? "";

                if (txtSoRep != null)
                    txtSoRep.Text = _exercise.SoRep ?? "";

                if (txtSoSet != null)
                    txtSoSet.Text = _exercise.SoSet ?? "";

                if (txtThoiGianNghi != null)
                    txtThoiGianNghi.Text = _exercise.ThoiGianNghi?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucSuaBT] LoadExerciseData error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load dữ liệu cho các combobox
        /// </summary>
        private void LoadComboBoxData()
        {
            try
            {
                // Load nhóm cơ chính
                if (cboNhomCo != null)
                {
                    cboNhomCo.Items.Clear();
                    var nhomCo = _dbContext.ThuVienBaiTap
                        .Where(bt => bt.NhomCoChinhNhat != null && bt.NhomCoChinhNhat != "")
                        .Select(bt => bt.NhomCoChinhNhat)
                        .Distinct()
                        .OrderBy(nc => nc)
                        .ToList();
                    
                    foreach (var nc in nhomCo)
                    {
                        cboNhomCo.Items.Add(nc);
                    }
                }

                // Load độ khó
                if (cboDoKho != null)
                {
                    cboDoKho.Items.Clear();
                    cboDoKho.Items.AddRange(new[] { "Beginner", "Intermediate", "Advanced", "All Levels" });
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucSuaBT] LoadComboBoxData error: {ex.Message}");
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

                // Reload từ database để đảm bảo có dữ liệu mới nhất
                var exercise = _dbContext.ThuVienBaiTap.FirstOrDefault(bt => bt.BaiTapID == _exercise.BaiTapID);
                if (exercise == null)
                {
                    MessageBox.Show("Không tìm thấy bài tập!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật dữ liệu
                exercise.TenBaiTap = txtTenBT?.Text?.Trim() ?? "";
                exercise.NhomCoChinhNhat = cboNhomCo?.SelectedItem?.ToString() ?? "";
                exercise.CapDo = cboDoKho?.SelectedItem?.ToString();
                exercise.DungCu = txtThietBi?.Text?.Trim();
                exercise.MoTa = txtMoTa?.Text?.Trim();
                exercise.HuongDan = txtHuongDan?.Text?.Trim();
                exercise.AnhMinhHoa = txtLinkAnh?.Text?.Trim();
                exercise.VideoHuongDan = txtLinkVideo?.Text?.Trim();
                exercise.CaloriesMoiRep = ParseDouble(txtCaloUocTinh?.Text);
                exercise.SoRep = txtSoRep?.Text?.Trim();
                exercise.SoSet = txtSoSet?.Text?.Trim();
                exercise.ThoiGianNghi = ParseInt(txtThoiGianNghi?.Text);
                exercise.NgayCapNhat = DateTime.Now;

                // Lưu vào database
                _dbContext.SaveChanges();

                MessageBox.Show("Sửa bài tập thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Trigger event
                OnSaveSuccess?.Invoke(this, EventArgs.Empty);
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
                    ex.Message.Contains("cannot insert") || ex.Message.Contains("violation") ||
                    ex.Message.Contains("cannot update"))
                {
                    errorMessage = $"Lỗi database:\n{errorMessage}";
                }

                MessageBox.Show($"Lỗi khi sửa bài tập:\n{errorMessage}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucSuaBT] BtnXacNhan_Click error: {errorMessage}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Validate dữ liệu đầu vào
        /// </summary>
        private bool ValidateInput()
        {
            ResetAllBorders();

            // 1) Bắt buộc
            string tenBT = txtTenBT?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(tenBT))
            {
                MessageBox.Show("Vui lòng nhập tên bài tập!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtTenBT, BorderColorError);
                txtTenBT?.Focus();
                return false;
            }

            if (cboNhomCo?.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn nhóm cơ chính!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetComboBoxBorder(cboNhomCo, BorderColorError);
                cboNhomCo?.Focus();
                return false;
            }

            if (cboDoKho == null || cboDoKho.SelectedItem == null || cboDoKho.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn độ khó!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetComboBoxBorder(cboDoKho, BorderColorError);
                cboDoKho?.Focus();
                return false;
            }

            string soSetStr = txtSoSet?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(soSetStr))
            {
                MessageBox.Show("Vui lòng nhập số set!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtSoSet, BorderColorError);
                txtSoSet?.Focus();
                return false;
            }

            string soRepStr = txtSoRep?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(soRepStr))
            {
                MessageBox.Show("Vui lòng nhập số rep!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtSoRep, BorderColorError);
                txtSoRep?.Focus();
                return false;
            }

            string caloStr = txtCaloUocTinh?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(caloStr))
            {
                MessageBox.Show("Vui lòng nhập Calo ước tính (cal/rep)!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtCaloUocTinh, BorderColorError);
                txtCaloUocTinh?.Focus();
                return false;
            }

            string nghiStr = txtThoiGianNghi?.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(nghiStr))
            {
                MessageBox.Show("Vui lòng nhập thời gian nghỉ giữa các set!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtThoiGianNghi, BorderColorError);
                txtThoiGianNghi?.Focus();
                return false;
            }

            // 2) Kiểu dữ liệu & giá trị + 3) logic tập luyện
            if (!int.TryParse(soSetStr, out int soSet) || soSet < 1)
            {
                MessageBox.Show("Số set phải là số nguyên ≥ 1!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtSoSet, BorderColorError);
                txtSoSet?.Focus();
                return false;
            }
            if (soSet < 1 || soSet > 10)
            {
                MessageBox.Show("Số set hợp lý trong khoảng 1 – 10!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtSoSet, BorderColorError);
                txtSoSet?.Focus();
                return false;
            }

            if (!int.TryParse(soRepStr, out int soRep) || soRep < 1)
            {
                MessageBox.Show("Số rep phải là số nguyên ≥ 1!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtSoRep, BorderColorError);
                txtSoRep?.Focus();
                return false;
            }
            if (soRep < 1 || soRep > 100)
            {
                MessageBox.Show("Số rep hợp lý trong khoảng 1 – 100!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtSoRep, BorderColorError);
                txtSoRep?.Focus();
                return false;
            }

            string caloNormalized = caloStr.Replace(',', '.');
            if (!double.TryParse(caloNormalized, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double calo) || calo <= 0)
            {
                MessageBox.Show("Calo / rep phải là số > 0!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtCaloUocTinh, BorderColorError);
                txtCaloUocTinh?.Focus();
                return false;
            }

            if (!int.TryParse(nghiStr, out int nghi) || nghi < 0)
            {
                MessageBox.Show("Thời gian nghỉ phải là số nguyên ≥ 0 (giây)!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetTextBoxBorder(txtThoiGianNghi, BorderColorError);
                txtThoiGianNghi?.Focus();
                return false;
            }

            return true;
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
        /// Parse string to int
        /// </summary>
        private int? ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            
            if (int.TryParse(value, out int result))
                return result;
            
            return null;
        }

        /// <summary>
        /// Xử lý khi click nút Hủy
        /// </summary>
        private void BtnHuy_Click(object sender, EventArgs e)
        {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }

    }
}
