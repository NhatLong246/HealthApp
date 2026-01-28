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
