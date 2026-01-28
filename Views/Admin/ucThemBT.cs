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
    public partial class ucThemBT : UserControl
    {
        private WF_HealthTracker _dbContext;

        private static readonly Color BorderColorError = Color.Red;
        private static readonly Color BorderColorNormal = Color.FromArgb(208, 208, 208);
        
        // Events
        public event EventHandler OnSaveSuccess;
        public event EventHandler OnCancel;

        public ucThemBT()
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
                // Cho phép phím điều khiển (Backspace, v.v.)
                if (char.IsControl(e.KeyChar)) return;

                if (allowDecimal)
                {
                    // Cho phép 1 dấu chấm hoặc phẩy
                    if (e.KeyChar == '.' || e.KeyChar == ',')
                    {
                        if (ctrl.Text.Contains(".") || ctrl.Text.Contains(",")) e.Handled = true;
                        return;
                    }
                }

                // Chỉ cho số
                if (!char.IsDigit(e.KeyChar)) e.Handled = true;
            };
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
                System.Diagnostics.Debug.WriteLine($"[ucThemBT] LoadComboBoxData error: {ex.Message}");
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

                // Lấy UserID và kiểm tra tồn tại
                string nguoiTaoID = GetCurrentUserID();
                if (!string.IsNullOrWhiteSpace(nguoiTaoID))
                {
                    // Kiểm tra UserID có tồn tại trong database không
                    var userExists = _dbContext.Users.Any(u => u.UserID == nguoiTaoID);
                    if (!userExists)
                    {
                        nguoiTaoID = null; // Set null nếu không tồn tại
                    }
                }

                // Lấy LoaiMucTieu từ database hoặc dùng giá trị mặc định
                string loaiMucTieu = "General"; // Default
                try
                {
                    var existingLoaiMucTieu = _dbContext.ThuVienBaiTap
                        .Where(bt => bt.LoaiMucTieu != null && bt.LoaiMucTieu != "")
                        .Select(bt => bt.LoaiMucTieu)
                        .FirstOrDefault();
                    
                    if (!string.IsNullOrWhiteSpace(existingLoaiMucTieu))
                    {
                        loaiMucTieu = existingLoaiMucTieu;
                    }
                }
                catch
                {
                    // Giữ giá trị mặc định nếu có lỗi
                }

                // Tạo bài tập mới
                var newExercise = new ThuVienBaiTap
                {
                    BaiTapID = GenerateBaiTapID(),
                    TenBaiTap = txtTenBT?.Text?.Trim() ?? "",
                    LoaiMucTieu = loaiMucTieu,
                    NhomCoChinhNhat = cboNhomCo?.SelectedItem?.ToString() ?? "",
                    NhomCoPhu = null,
                    CapDo = cboDoKho?.SelectedItem?.ToString(),
                    DungCu = txtThietBi?.Text?.Trim(),
                    MoTa = txtMoTa?.Text?.Trim(),
                    HuongDan = txtHuongDan?.Text?.Trim(),
                    LuuY = null,
                    AnhMinhHoa = txtLinkAnh?.Text?.Trim(),
                    VideoHuongDan = txtLinkVideo?.Text?.Trim(),
                    CaloriesMoiRep = ParseDouble(txtCaloUocTinh?.Text),
                    ThoiLuongDeNghi = null,
                    SoRep = txtSoRep?.Text?.Trim(),
                    SoSet = txtSoSet?.Text?.Trim(),
                    ThoiGianNghi = ParseInt(txtThoiGianNghi?.Text),
                    DoPhoBien = 0,
                    NguoiTao = nguoiTaoID, // Có thể null nếu không có user hợp lệ
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    TheLoaiBenh = null
                };

                // Thêm vào database
                _dbContext.ThuVienBaiTap.Add(newExercise);
                _dbContext.SaveChanges();

                MessageBox.Show("Thêm bài tập thành công!", "Thông báo",
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
                    ex.Message.Contains("cannot insert") || ex.Message.Contains("violation"))
                {
                    errorMessage = $"Lỗi database:\n{errorMessage}";
                }

                MessageBox.Show($"Lỗi khi thêm bài tập:\n{errorMessage}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucThemBT] BtnXacNhan_Click error: {errorMessage}\n{ex.StackTrace}");
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

            // chấp nhận '.' hoặc ',' cho calo
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
        /// Tạo ID mới cho bài tập
        /// </summary>
        private string GenerateBaiTapID()
        {
            var lastExercise = _dbContext.ThuVienBaiTap
                .OrderByDescending(bt => bt.BaiTapID)
                .FirstOrDefault();

            if (lastExercise == null)
            {
                return "BT_0001";
            }

            // Lấy số từ ID cuối cùng
            string lastID = lastExercise.BaiTapID;
            if (lastID.StartsWith("BT_"))
            {
                string numberPart = lastID.Substring(3);
                if (int.TryParse(numberPart, out int number))
                {
                    return $"BT_{(number + 1):D4}";
                }
            }

            // Fallback: tạo ID dựa trên số lượng
            int count = _dbContext.ThuVienBaiTap.Count() + 1;
            return $"BT_{count:D4}";
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
        /// Lấy UserID hiện tại từ CurrentUser helper
        /// </summary>
        private string GetCurrentUserID()
        {
            try
            {
                if (CurrentUser.IsLoggedIn && !string.IsNullOrWhiteSpace(CurrentUser.UserID))
                {
                    return CurrentUser.UserID;
                }
                
                // Nếu không có user đăng nhập, tìm admin user
                var adminUser = _dbContext.Users.FirstOrDefault(u => u.Role == "Admin");
                return adminUser?.UserID;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucThemBT] GetCurrentUserID error: {ex.Message}");
                return null; // Return null nếu không lấy được
            }
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
