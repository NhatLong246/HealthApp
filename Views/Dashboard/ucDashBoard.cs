using HealthApp.Views.Nutrition;
using HealthApp.Views.MucTieu;
using HealthApp.Views.PT;
using HealthApp.Views.Food;
using HealthApp.Views.KeHoachLuyenTap;
using HealthApp.Views.GiaoBTChoUser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            // Gắn event handler cho button Nutri sau khi controls đã được khởi tạo
            // Xóa tất cả event handlers cũ trước khi gắn mới để tránh duplicate
            if (btnNutri != null)
            {
                btnNutri.Click -= BtnNutri_Click; // Xóa event handler cũ (nếu có)
                btnNutri.Click += BtnNutri_Click; // Gắn event handler mới
            }

            // Gắn event handler cho label Dinh dưỡng (cũng mở FoodLibrary)
            if (lblDinhDuong != null)
            {
                lblDinhDuong.Click -= BtnNutri_Click; // Xóa event handler cũ (nếu có)
                lblDinhDuong.Click += BtnNutri_Click; // Gắn event handler mới
                lblDinhDuong.Cursor = Cursors.Hand; // Đổi cursor thành hand để user biết có thể click
            }

            // Gắn event handler cho button Thuê Ngay
            if (btnThueNgay != null)
            {
                btnThueNgay.Click += BtnThueNgay_Click;
            }

            // Gắn event handler cho button "Lên Kế Hoạch Ngay"
            if (btnLenKeHoachNgay != null)
            {
                btnLenKeHoachNgay.Click += BtnLenKeHoachNgay_Click;
            }

            // Gắn event handler cho button Thống kê
            if (btnThongke != null)
            {
                btnThongke.Click += BtnThongke_Click;
            }

            // Gắn event handler cho label Thống kê
            if (lblThongKe != null)
            {
                lblThongKe.Click += BtnThongke_Click;
                lblThongKe.Cursor = Cursors.Hand;
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Tính BMI và TDEE khi click button "Tính nhanh"
        /// </summary>
        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate và lấy dữ liệu đầu vào
                if (!double.TryParse(txtChieuCao.Text, out double chieuCao) || chieuCao <= 0)
                {
                    MessageBox.Show("Vui lòng nhập chiều cao hợp lệ (cm)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtChieuCao.Focus();
                    return;
                }

                if (!double.TryParse(txtCanNang.Text, out double canNang) || canNang <= 0)
                {
                    MessageBox.Show("Vui lòng nhập cân nặng hợp lệ (kg)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCanNang.Focus();
                    return;
                }

                if (!int.TryParse(txtTuoi.Text, out int tuoi) || tuoi <= 0 || tuoi > 150)
                {
                    MessageBox.Show("Vui lòng nhập tuổi hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTuoi.Focus();
                    return;
                }

                if (cboGioiTinh.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn giới tính!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboGioiTinh.Focus();
                    return;
                }

                if (cboHoatDong.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn mức độ hoạt động!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboHoatDong.Focus();
                    return;
                }

                // Tính BMI
                // BMI = weight (kg) / (height (m))^2
                double chieuCaoM = chieuCao / 100.0; // Chuyển từ cm sang m
                double bmi = canNang / (chieuCaoM * chieuCaoM);
                
                // Hiển thị BMI
                lblBMI.Text = $"BMI : {bmi:F1}";

                // Tính BMR (Basal Metabolic Rate) dựa trên giới tính
                // Công thức Mifflin-St Jeor (chính xác hơn Harris-Benedict)
                double bmr;
                string gioiTinh = cboGioiTinh.SelectedItem.ToString().ToLower();
                
                if (gioiTinh.Contains("nam") || gioiTinh.Contains("male"))
                {
                    // BMR cho nam: 10 × weight(kg) + 6.25 × height(cm) - 5 × age(years) + 5
                    bmr = (10 * canNang) + (6.25 * chieuCao) - (5 * tuoi) + 5;
                }
                else
                {
                    // BMR cho nữ: 10 × weight(kg) + 6.25 × height(cm) - 5 × age(years) - 161
                    bmr = (10 * canNang) + (6.25 * chieuCao) - (5 * tuoi) - 161;
                }

                // Lấy Activity Factor từ combobox
                string hoatDong = cboHoatDong.SelectedItem.ToString();
                double activityFactor = GetActivityFactor(hoatDong);

                // Tính TDEE (Total Daily Energy Expenditure)
                // TDEE = BMR × Activity Factor
                double tdee = bmr * activityFactor;

                // Hiển thị TDEE
                lblTDEE.Text = $"TDEE : {tdee:F0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính toán: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Lấy Activity Factor dựa trên mức độ hoạt động
        /// </summary>
        private double GetActivityFactor(string hoatDong)
        {
            string hoatDongLower = hoatDong.ToLower();
            
            if (hoatDongLower.Contains("ít") || hoatDongLower.Contains("sedentary") || hoatDongLower.Contains("không"))
            {
                return 1.2; // Ít vận động
            }
            else if (hoatDongLower.Contains("nhẹ") || hoatDongLower.Contains("light"))
            {
                return 1.375; // Nhẹ (1-3 buổi/tuần)
            }
            else if (hoatDongLower.Contains("vừa") || hoatDongLower.Contains("trung bình") || hoatDongLower.Contains("moderate"))
            {
                return 1.55; // Vừa/Trung bình (3-5 buổi/tuần)
            }
            else if (hoatDongLower.Contains("nhiều") || hoatDongLower.Contains("năng động") || hoatDongLower.Contains("active"))
            {
                return 1.725; // Nhiều/Năng động (6-7 buổi/tuần)
            }
            else if (hoatDongLower.Contains("rất") || hoatDongLower.Contains("very") || hoatDongLower.Contains("lao động"))
            {
                return 1.9; // Rất nhiều/Rất năng động
            }
            else
            {
                // Mặc định là nhẹ
                return 1.375;
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void lblThuePT_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void pnlChuyenMuc_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler cho button Nutri - điều hướng tới trang FoodLibrary
        /// </summary>
        private void BtnNutri_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== BtnNutri_Click được gọi - Mở frm_FoodLibrary ===");
                
                // Tìm frmDashBoard
                frmDashBoard parentForm = _parentForm;

                // Nếu chưa có reference, thử tìm qua các cách khác
                if (parentForm == null)
                {
                    // Cách 1: Tìm qua FindForm()
                    Form form = this.FindForm();
                    if (form is frmDashBoard)
                    {
                        parentForm = form as frmDashBoard;
                        System.Diagnostics.Debug.WriteLine("Tìm thấy parentForm qua FindForm()");
                    }
                    // Cách 2: Tìm qua Application.OpenForms
                    else
                    {
                        foreach (Form openForm in Application.OpenForms)
                        {
                            if (openForm is frmDashBoard)
                            {
                                parentForm = openForm as frmDashBoard;
                                System.Diagnostics.Debug.WriteLine("Tìm thấy parentForm qua Application.OpenForms");
                                break;
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"parentForm: {(parentForm != null ? "NOT NULL" : "NULL")}");
                System.Diagnostics.Debug.WriteLine("Đang tạo và mở frm_FoodLibrary...");

                // Mở form FoodLibrary
                var frmFoodLibrary = new frm_FoodLibrary();
                
                if (parentForm != null)
                {
                    System.Diagnostics.Debug.WriteLine("Mở frm_FoodLibrary với ShowDialog");
                    frmFoodLibrary.ShowDialog(parentForm);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Mở frm_FoodLibrary độc lập với Show");
                    frmFoodLibrary.Show();
                }
                
                System.Diagnostics.Debug.WriteLine("Đã mở frm_FoodLibrary thành công");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong BtnNutri_Click: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                MessageBox.Show($"Lỗi khi mở trang thư viện món ăn: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Mục Tiêu - điều hướng tới trang mục tiêu
        /// </summary>
        private void btnMuctieu_Click(object sender, EventArgs e)
        {
            try
            {
                // Tìm frmDashBoard
                frmDashBoard parentForm = _parentForm;

                // Nếu chưa có reference, thử tìm qua các cách khác
                if (parentForm == null)
                {
                    // Cách 1: Tìm qua FindForm()
                    Form form = this.FindForm();
                    if (form is frmDashBoard)
                    {
                        parentForm = form as frmDashBoard;
                    }
                    // Cách 2: Tìm qua Application.OpenForms
                    else
                    {
                        foreach (Form openForm in Application.OpenForms)
                        {
                            if (openForm is frmDashBoard)
                            {
                                parentForm = openForm as frmDashBoard;
                                break;
                            }
                        }
                    }
                }

                if (parentForm != null)
                {
                    // Tạo và load ucMucTieu
                    ucMucTieu ucMucTieu = new ucMucTieu();
                    parentForm.LoadUserControl(ucMucTieu);
                }
                else
                {
                    MessageBox.Show("Không thể tìm thấy form chính để điều hướng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Thuê Ngay - mở form tìm kiếm HLV
        /// </summary>
        private void BtnThueNgay_Click(object sender, EventArgs e)
        {
            try
            {
                // Tìm frmDashBoard
                frmDashBoard parentForm = _parentForm;

                // Nếu chưa có reference, thử tìm qua các cách khác
                if (parentForm == null)
                {
                    // Cách 1: Tìm qua FindForm()
                    Form form = this.FindForm();
                    if (form is frmDashBoard)
                    {
                        parentForm = form as frmDashBoard;
                    }
                    // Cách 2: Tìm qua Application.OpenForms
                    else
                    {
                        foreach (Form openForm in Application.OpenForms)
                        {
                            if (openForm is frmDashBoard)
                            {
                                parentForm = openForm as frmDashBoard;
                                break;
                            }
                        }
                    }
                }

                if (parentForm != null)
                {
                    // Ẩn Dashboard và hiển thị form tìm kiếm HLV
                    parentForm.Hide();
                    var frmTimKiemHLV = new frm_TimKiemHLV(parentForm);
                    frmTimKiemHLV.Show();
                }
                else
                {
                    // Nếu không tìm thấy parent, mở form độc lập
                    var frmTimKiemHLV = new frm_TimKiemHLV();
                    frmTimKiemHLV.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form tìm kiếm HLV: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button "Lên Kế Hoạch Ngay" - điều hướng tới trang kế hoạch luyện tập
        /// </summary>
        private void BtnLenKeHoachNgay_Click(object sender, EventArgs e)
        {
            try
            {
                // Tìm frmDashBoard
                frmDashBoard parentForm = _parentForm;

                // Nếu chưa có reference, thử tìm qua các cách khác
                if (parentForm == null)
                {
                    // Cách 1: Tìm qua FindForm()
                    Form form = this.FindForm();
                    if (form is frmDashBoard)
                    {
                        parentForm = form as frmDashBoard;
                    }
                    // Cách 2: Tìm qua Application.OpenForms
                    else
                    {
                        foreach (Form openForm in Application.OpenForms)
                        {
                            if (openForm is frmDashBoard)
                            {
                                parentForm = openForm as frmDashBoard;
                                break;
                            }
                        }
                    }
                }

                if (parentForm != null)
                {
                    // Tạo và load ucKeHoachLuyenTap
                    ucKeHoachLuyenTap ucKeHoachLuyenTap = new ucKeHoachLuyenTap();
                    parentForm.LoadUserControl(ucKeHoachLuyenTap);
                }
                else
                {
                    MessageBox.Show("Không thể tìm thấy form chính để điều hướng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Thống kê - điều hướng tới trang quản lý luyện tập với PT
        /// </summary>
        private void BtnThongke_Click(object sender, EventArgs e)
        {
            try
            {
                // Tìm frmDashBoard
                frmDashBoard parentForm = _parentForm;

                // Nếu chưa có reference, thử tìm qua các cách khác
                if (parentForm == null)
                {
                    // Cách 1: Tìm qua FindForm()
                    Form form = this.FindForm();
                    if (form is frmDashBoard)
                    {
                        parentForm = form as frmDashBoard;
                    }
                    // Cách 2: Tìm qua Application.OpenForms
                    else
                    {
                        foreach (Form openForm in Application.OpenForms)
                        {
                            if (openForm is frmDashBoard)
                            {
                                parentForm = openForm as frmDashBoard;
                                break;
                            }
                        }
                    }
                }

                if (parentForm != null)
                {
                    // Tạo và load BaiTapCuaPTGiao
                    var ucQuanLyLuyenTap = new BaiTapCuaPTGiao();
                    parentForm.LoadUserControl(ucQuanLyLuyenTap);
                }
                else
                {
                    MessageBox.Show("Không thể tìm thấy form chính để điều hướng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
