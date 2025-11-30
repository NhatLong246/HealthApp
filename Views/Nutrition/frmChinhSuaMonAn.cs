using HealthApp.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace HealthApp.Views.Nutrition
{
    public partial class frmChinhSuaMonAn : Form
    {
        public bool IsDeleted { get; private set; } = false;
        public BuaAnChiTiet MonAnDaCapNhat { get; private set; }
        private BuaAnChiTiet _monAnGoc;
        private WF_HealthTracker _dbContext;
        private ThuVienMonAn _thuVienMonAn;

        public frmChinhSuaMonAn(BuaAnChiTiet monAn, WF_HealthTracker dbContext)
        {
            InitializeComponent();
            _monAnGoc = monAn;
            _dbContext = dbContext;
            InitializeData();
        }

        private void InitializeData()
        {
            if (_monAnGoc == null)
            {
                MessageBox.Show("Không tìm thấy thông tin món ăn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Load thông tin từ ThuVienMonAn để hiển thị giá trị dinh dưỡng chuẩn
            if (!string.IsNullOrEmpty(_monAnGoc.MonAnID))
            {
                _thuVienMonAn = _dbContext.ThuVienMonAn.FirstOrDefault(m => m.MonAnID == _monAnGoc.MonAnID);
            }

            // Hiển thị thông tin món ăn
            lblTenMonAn.Text = _monAnGoc.TenMonAn ?? "N/A";
            
            // Hiển thị giá trị dinh dưỡng hiện tại (từ BuaAnChiTiet)
            lblCalories.Text = $"Calories: {_monAnGoc.Calories ?? 0} kcal";
            lblProtein.Text = $"Protein: {_monAnGoc.Protein ?? 0}g";
            lblCarbs.Text = $"Carbs: {_monAnGoc.Carbs ?? 0}g";
            lblFat.Text = $"Fat: {_monAnGoc.Fat ?? 0}g";

            // Set đơn vị
            if (!string.IsNullOrEmpty(_monAnGoc.Donvi))
            {
                lblDonVi.Text = $"Đơn vị: {_monAnGoc.Donvi}";
            }

            // Load dữ liệu vào các field
            txtSoLuong.Text = (_monAnGoc.KhoiLuongChuan ?? 100).ToString();

            // Tính toán lại khi số lượng thay đổi
            txtSoLuong.TextChanged += TxtSoLuong_TextChanged;
        }

        private void TxtSoLuong_TextChanged(object sender, EventArgs e)
        {
            CalculateNutrition();
        }

        private void CalculateNutrition()
        {
            if (_thuVienMonAn == null) return;

            if (double.TryParse(txtSoLuong.Text, out double soLuong))
            {
                // Tính theo 100g (vì ThuVienMonAn lưu giá trị dinh dưỡng cho 100g)
                double tiLe = soLuong / 100.0;

                double calories = (_thuVienMonAn.Calories ?? 0) * tiLe;
                double protein = (_thuVienMonAn.Protein ?? 0) * tiLe;
                double carbs = (_thuVienMonAn.Carbs ?? 0) * tiLe;
                double fat = (_thuVienMonAn.Fat ?? 0) * tiLe;

                lblCalories.Text = $"Calories: {calories:F0} kcal";
                lblProtein.Text = $"Protein: {protein:F1}g";
                lblCarbs.Text = $"Carbs: {carbs:F1}g";
                lblFat.Text = $"Fat: {fat:F1}g";
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa món ăn này?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Đánh dấu đã xóa
                    IsDeleted = true;

                    // Xóa từ database
                    var itemToDelete = _dbContext.BuaAnChiTiet.Find(_monAnGoc.BuaAnID);
                    if (itemToDelete != null)
                    {
                        _dbContext.BuaAnChiTiet.Remove(itemToDelete);
                        _dbContext.SaveChanges();
                    }

                    MessageBox.Show("Đã xóa món ăn thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL khi xóa món ăn:\n\n{sqlEx.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa món ăn:\n\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(txtSoLuong.Text))
                {
                    MessageBox.Show("Vui lòng nhập số lượng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtSoLuong.Text, out double soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Số lượng phải là số dương!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tính toán dinh dưỡng
                double tiLe = soLuong / 100.0;
                double calories = (_thuVienMonAn?.Calories ?? 0) * tiLe;
                double protein = (_thuVienMonAn?.Protein ?? 0) * tiLe;
                double carbs = (_thuVienMonAn?.Carbs ?? 0) * tiLe;
                double fat = (_thuVienMonAn?.Fat ?? 0) * tiLe;

                // Tìm và cập nhật BuaAnChiTiet trong database
                var itemToUpdate = _dbContext.BuaAnChiTiet.Find(_monAnGoc.BuaAnID);
                if (itemToUpdate == null)
                {
                    MessageBox.Show("Không tìm thấy món ăn để cập nhật!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Chỉ cập nhật số lượng và dinh dưỡng (giữ nguyên LoaiBuaAn, NgayAn, GhiChu)
                itemToUpdate.KhoiLuongChuan = soLuong;
                itemToUpdate.Calories = calories;
                itemToUpdate.Protein = protein;
                itemToUpdate.Carbs = carbs;
                itemToUpdate.Fat = fat;
                itemToUpdate.Fiber = (_thuVienMonAn?.Fiber ?? 0) * tiLe;
                itemToUpdate.NgayCapNhat = DateTime.Now;

                // Lưu vào database
                _dbContext.SaveChanges();

                // Tạo object mới để trả về
                MonAnDaCapNhat = itemToUpdate;

                MessageBox.Show("Đã cập nhật món ăn thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL khi cập nhật món ăn:\n\n{sqlEx.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật món ăn:\n\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
