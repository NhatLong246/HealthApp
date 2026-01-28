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
    public partial class SuaMonAn : Form
    {
        private WF_HealthTracker _dbContext;
        private ThuVienMonAn _monAn;
        public bool IsSaved { get; private set; } = false;

        public SuaMonAn(ThuVienMonAn monAn)
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            _monAn = monAn;
            InitializeControls();
            LoadMonAnData();
            this.Load += SuaMonAn_Load;
        }

        private void SuaMonAn_Load(object sender, EventArgs e)
        {
            // Data đã được load trong constructor
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
        }

        /// <summary>
        /// Load dữ liệu món ăn vào form
        /// </summary>
        private void LoadMonAnData()
        {
            if (_monAn == null)
                return;

            try
            {
                // Load lại từ database để đảm bảo dữ liệu mới nhất
                var monAnFromDb = _dbContext.ThuVienMonAn.FirstOrDefault(ma => ma.MonAnID == _monAn.MonAnID);
                if (monAnFromDb == null)
                {
                    MessageBox.Show("Không tìm thấy món ăn!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                _monAn = monAnFromDb;

                // Load dữ liệu vào các controls
                if (txtTenMonAn != null)
                    txtTenMonAn.Text = _monAn.TenMonAn ?? "";

                // Load Loai vào combobox
                if (cboLoai != null)
                {
                    // Đảm bảo danh sách loại đã được load
                    if (cboLoai.Items.Count == 0)
                    {
                        LoadLoaiToComboBox();
                    }
                    
                    // Chọn đúng loại hiện tại
                    int index = cboLoai.Items.IndexOf(_monAn.Loai);
                    if (index >= 0)
                        cboLoai.SelectedIndex = index;
                    else if (cboLoai.Items.Count > 0)
                        cboLoai.SelectedIndex = 0;
                }

                if (cboDonVi != null)
                {
                    int index = cboDonVi.Items.IndexOf(_monAn.Donvi);
                    if (index >= 0)
                        cboDonVi.SelectedIndex = index;
                    else if (cboDonVi.Items.Count > 0)
                        cboDonVi.SelectedIndex = 0;
                }

                // Load khối lượng chuẩn
                if (txtKhoiLuongChuan != null)
                    txtKhoiLuongChuan.Text = _monAn.KhoiLuongChuan?.ToString() ?? "";

                if (txtSoCalories != null)
                    txtSoCalories.Text = _monAn.Calories?.ToString() ?? "";

                if (txtProtein != null)
                    txtProtein.Text = _monAn.Protein?.ToString() ?? "";

                if (txtCarb != null)
                    txtCarb.Text = _monAn.Carbs?.ToString() ?? "";

                if (txtFat != null)
                    txtFat.Text = _monAn.Fat?.ToString() ?? "";

                if (txtFiber != null)
                    txtFiber.Text = _monAn.Fiber?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SuaMonAn] LoadMonAnData error: {ex.Message}");
                MessageBox.Show($"Lỗi khi load dữ liệu món ăn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load dữ liệu cho các combobox
        /// </summary>
        private void LoadComboBoxData()
        {
            try
            {
                // Load đơn vị (trùng với schema ThuVienMonAn)
                if (cboDonVi != null)
                {
                    cboDonVi.Items.Clear();
                    cboDonVi.Items.AddRange(new[] { "g", "ml" });
                }

                // Load loại món ăn từ DB
                LoadLoaiToComboBox();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SuaMonAn] LoadComboBoxData error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load danh sách loại món ăn từ DB vào combobox
        /// </summary>
        private void LoadLoaiToComboBox()
        {
            try
            {
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
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SuaMonAn] LoadLoaiToComboBox error: {ex.Message}");
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

                // Load lại từ database để đảm bảo dữ liệu mới nhất
                var monAnToUpdate = _dbContext.ThuVienMonAn.FirstOrDefault(ma => ma.MonAnID == _monAn.MonAnID);
                if (monAnToUpdate == null)
                {
                    MessageBox.Show("Không tìm thấy món ăn để cập nhật!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật thông tin (mapping đúng với ThuVienMonAn)
                monAnToUpdate.TenMonAn = txtTenMonAn?.Text?.Trim() ?? "";
                monAnToUpdate.Loai = cboLoai?.SelectedItem?.ToString();
                monAnToUpdate.Donvi = cboDonVi?.SelectedItem?.ToString();
                monAnToUpdate.KhoiLuongChuan = ParseDouble(txtKhoiLuongChuan?.Text);
                monAnToUpdate.Calories = ParseDouble(txtSoCalories?.Text);
                monAnToUpdate.Protein = ParseDouble(txtProtein?.Text);
                monAnToUpdate.Carbs = ParseDouble(txtCarb?.Text);
                monAnToUpdate.Fat = ParseDouble(txtFat?.Text);
                monAnToUpdate.Fiber = ParseDouble(txtFiber?.Text);

                // Lưu thay đổi
                _dbContext.SaveChanges();

                MessageBox.Show("Sửa món ăn thành công!", "Thông báo",
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
                    ex.Message.Contains("cannot insert") || ex.Message.Contains("violation") ||
                    ex.Message.Contains("cannot update"))
                {
                    errorMessage = $"Lỗi database:\n{errorMessage}";
                }

                MessageBox.Show($"Lỗi khi sửa món ăn:\n{errorMessage}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[SuaMonAn] BtnXacNhan_Click error: {errorMessage}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Validate dữ liệu đầu vào
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTenMonAn?.Text))
            {
                MessageBox.Show("Vui lòng nhập tên món ăn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenMonAn?.Focus();
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
        /// Xử lý khi click nút Hủy
        /// </summary>
        private void BtnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SuaMonAn_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dbContext?.Dispose();
        }
    }
}
