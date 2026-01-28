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
using HealthApp.Controllers;
using HealthApp.Common.Helpers;
using System.Text.RegularExpressions;

namespace HealthApp.Views.PT
{
    public partial class frm_DangKy : Form
    {
        private readonly PTController _ptController;
        private readonly HealthApp.Views.Dashboard.frmDashBoard1 _parentDashboard;
        private string _anhChanDungPath;
        private string _anhCCCDPath;
        private string _bangCapPath;
        private DateTime? _minNgayCapCccd;

        public frm_DangKy(HealthApp.Views.Dashboard.frmDashBoard1 parentDashboard = null)
        {
            InitializeComponent();
            _ptController = new PTController();
            _parentDashboard = parentDashboard;
            LoadUserInfo();
            InitializeComboBoxItems();
            InitializeEventHandlers();
        }
        
        /// <summary>
        /// Khởi tạo items cho ComboBox chuyên môn
        /// </summary>
        private void InitializeComboBoxItems()
        {
            // Thêm 3 lựa chọn vào ComboBox chuyên môn (cbo_ChuyenMon)
            cbo_ChuyenMon.Items.Clear();
            cbo_ChuyenMon.Items.Add("Cân nặng & Tăng cơ");
            cbo_ChuyenMon.Items.Add("Cân nặng");
            cbo_ChuyenMon.Items.Add("Tăng cơ");
        }

        private void InitializeEventHandlers()
        {
            // Event handlers cho các button chọn file
            btn_ChonAnh.Click += Btn_ChonAnh_Click;
            btn_CCCD.Click += Btn_CCCD_Click;
            btn_ChonTep.Click += Btn_ChonTep_Click;
            
            // Event handler cho nút gửi đơn
            btn_GuiDonDangKy.Click += Btn_GuiDonDangKy_Click;
            
            // Event handler cho nút quay lại
            btnBack.Click += BtnBack_Click;
            
            // Chỉ cho phép nhập số vào txt_ChuTaiKhoan (Số năm kinh nghiệm)
            txt_ChuTaiKhoan.KeyPress += Txt_SoNamKinhNghiem_KeyPress;
            
            // Chỉ cho phép nhập số vào txt_TienTheoGio
            txt_TienTheoGio.KeyPress += Txt_TienTheoGio_KeyPress;
        }
        
        /// <summary>
        /// Chỉ cho phép nhập số vào txt_ChuTaiKhoan (Số năm kinh nghiệm)
        /// </summary>
        private void Txt_SoNamKinhNghiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số và phím Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Chỉ cho phép nhập số vào txt_TienTheoGio
        /// </summary>
        private void Txt_TienTheoGio_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số, dấu chấm và phím Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            
            // Chỉ cho phép một dấu chấm
            if (e.KeyChar == '.' && ((Guna.UI2.WinForms.Guna2TextBox)sender).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Load thông tin cá nhân từ CurrentUser (chỉ đọc)
        /// </summary>
        private void LoadUserInfo()
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi đăng ký!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                var user = CurrentUser.User;

                // Load thông tin cá nhân (chỉ đọc)
                txt_HoVaTen.Text = user.HoTen ?? "";
                txt_HoVaTen.ReadOnly = true;
                txt_HoVaTen.FillColor = Color.LightGray;

                txt_Email.Text = user.Email ?? "";
                txt_Email.ReadOnly = true;
                txt_Email.FillColor = Color.LightGray;

                txt_SDT.Text = user.SDT ?? "";
                txt_SDT.ReadOnly = true;
                txt_SDT.FillColor = Color.LightGray;

                if (user.NgaySinh.HasValue)
                {
                    guna2DateTimePicker1.Value = user.NgaySinh.Value;
                    var minDate = user.NgaySinh.Value.AddYears(14);
                    _minNgayCapCccd = minDate;
                    dtp_NgayCap.MinDate = minDate > dtp_NgayCap.MinDate ? minDate : dtp_NgayCap.MinDate;
                    dtp_NgayCap.MaxDate = DateTime.Today;

                    if (minDate > DateTime.Today)
                    {
                        MessageBox.Show("Bạn chưa đủ 14 tuổi để được cấp CCCD, nên không thể đăng ký làm PT.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btn_GuiDonDangKy.Enabled = false;
                    }
                    else if (dtp_NgayCap.Value < minDate)
                    {
                        dtp_NgayCap.Value = minDate;
                    }
                }
                else
                {
                    dtp_NgayCap.MaxDate = DateTime.Today;
                }
                guna2DateTimePicker1.Enabled = false;
                guna2DateTimePicker1.FillColor = Color.LightGray;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thông tin người dùng: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void pnl_background_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// Xử lý chọn ảnh chân dung
        /// </summary>
        private void Btn_ChonAnh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _anhChanDungPath = openFileDialog.FileName;
                    ptr_AnhChanDung.Image = Image.FromFile(_anhChanDungPath);
                    Ibl_AnhChanDung.Text = Path.GetFileName(_anhChanDungPath);
                }
            }
        }

        /// <summary>
        /// Xử lý chọn ảnh CCCD
        /// </summary>
        private void Btn_CCCD_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _anhCCCDPath = openFileDialog.FileName;
                    txt_TaiLieu_CCCD.Text = Path.GetFileName(_anhCCCDPath);
                }
            }
        }

        /// <summary>
        /// Xử lý chọn file bằng cấp
        /// </summary>
        private void Btn_ChonTep_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.*)|*.*|PDF files (*.pdf)|*.pdf|Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _bangCapPath = openFileDialog.FileName;
                    txt_BangCap.Text = Path.GetFileName(_bangCapPath);
                }
            }
        }

        /// <summary>
        /// Xử lý gửi đơn đăng ký
        /// </summary>
        private async void Btn_GuiDonDangKy_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable button để tránh click nhiều lần
                btn_GuiDonDangKy.Enabled = false;
                btn_GuiDonDangKy.Text = "Đang xử lý...";

                // Validation
                var soCCCDInput = txt_SoCCCD.Text?.Trim();

                if (string.IsNullOrWhiteSpace(soCCCDInput))
                {
                    MessageBox.Show("Vui lòng nhập số CCCD!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                if (!Regex.IsMatch(soCCCDInput, @"^\d{12}$"))
                {
                    MessageBox.Show("Số CCCD phải gồm đúng 12 chữ số!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                if (string.IsNullOrWhiteSpace(txt_NoiCap.Text))
                {
                    MessageBox.Show("Vui lòng nhập nơi cấp CCCD!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                var cccdAlreadyUsed = await _ptController.IsCCCDAlreadyUsedAsync(soCCCDInput);
                if (cccdAlreadyUsed)
                {
                    MessageBox.Show("Số CCCD này đã được sử dụng để đăng ký PT!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                // Validation - Chuyên môn
                if (cbo_ChuyenMon.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui lòng chọn chuyên môn!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                // Validation - Số năm kinh nghiệm
                if (string.IsNullOrWhiteSpace(txt_ChuTaiKhoan.Text))
                {
                    MessageBox.Show("Vui lòng nhập số năm kinh nghiệm!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                if (!int.TryParse(txt_ChuTaiKhoan.Text, out int soNamKinhNghiem) || soNamKinhNghiem < 0)
                {
                    MessageBox.Show("Số năm kinh nghiệm phải là số nguyên dương!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                // Kiểm tra tuổi >= 18
                var user = CurrentUser.User;
                if (user == null || !user.NgaySinh.HasValue)
                {
                    MessageBox.Show("Không tìm thấy thông tin ngày sinh!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                var tuoi = DateTime.Now.Year - user.NgaySinh.Value.Year;
                if (user.NgaySinh.Value.Date > DateTime.Now.AddYears(-tuoi)) tuoi--;

                if (tuoi < 18)
                {
                    MessageBox.Show("Bạn phải đủ 18 tuổi trở lên mới có thể đăng ký làm PT!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                // Kiểm tra số năm kinh nghiệm hợp lý (tối đa = tuổi - 18, vì phải đủ 18 tuổi mới bắt đầu làm PT)
                int maxNamKinhNghiem = Math.Max(0, tuoi - 18);
                if (soNamKinhNghiem > maxNamKinhNghiem)
                {
                    MessageBox.Show($"Số năm kinh nghiệm không hợp lý! Bạn {tuoi} tuổi, tối đa chỉ có thể có {maxNamKinhNghiem} năm kinh nghiệm.", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                // Validation - Giá theo giờ (nếu có nhập)
                double? giaTheoGio = null;
                if (!string.IsNullOrWhiteSpace(txt_TienTheoGio.Text))
                {
                    if (!double.TryParse(txt_TienTheoGio.Text, out double gia) || gia < 0)
                    {
                        MessageBox.Show("Giá theo giờ phải là số dương!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btn_GuiDonDangKy.Enabled = true;
                        btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                        return;
                    }
                    giaTheoGio = gia;
                }

                // Lưu các file đã chọn
                string savedAnhChanDung = null;
                string savedAnhCCCD = null;
                string savedBangCap = null;

                if (!string.IsNullOrEmpty(_anhChanDungPath))
                {
                    savedAnhChanDung = _ptController.SaveFile(_anhChanDungPath, "AnhChanDung", "PTDocuments");
                }

                if (!string.IsNullOrEmpty(_anhCCCDPath))
                {
                    savedAnhCCCD = _ptController.SaveFile(_anhCCCDPath, "AnhCCCD", "PTDocuments");
                }

                if (!string.IsNullOrEmpty(_bangCapPath))
                {
                    savedBangCap = _ptController.SaveFile(_bangCapPath, "BangCap", "PTDocuments");
                }

                // Gọi controller để đăng ký
                if (_minNgayCapCccd.HasValue && dtp_NgayCap.Value.Date < _minNgayCapCccd.Value.Date)
                {
                    MessageBox.Show($"Ngày cấp CCCD không hợp lệ. CCCD chỉ hợp lệ từ ngày {_minNgayCapCccd.Value:dd/MM/yyyy} (sau 14 tuổi).", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_GuiDonDangKy.Enabled = true;
                    btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
                    return;
                }

                var result = await _ptController.RegisterPTAsync(
                    soCCCDInput,
                    txt_NoiCap.Text.Trim(),
                    dtp_NgayCap.Value,
                    savedAnhChanDung,
                    savedAnhCCCD,
                    savedBangCap,
                    cbo_ChuyenMon.SelectedItem?.ToString(), // Chuyên môn
                    txt_ChungChi.Text.Trim(), // Chứng chỉ
                    soNamKinhNghiem, // Số năm kinh nghiệm
                    giaTheoGio, // Giá theo giờ
                    txt_ThanhPho.Text.Trim()); // Thành phố

                if (result.Success)
                {
                    MessageBox.Show(result.Message, "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Quay lại Dashboard thay vì chuyển đến PT Dashboard
                    // Vì chưa được duyệt nên chưa thể vào PT Dashboard
                    NavigateBackToDashboard();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btn_GuiDonDangKy.Enabled = true;
                btn_GuiDonDangKy.Text = "Gửi Đơn Đăng Ký";
            }
        }

        /// <summary>
        /// Xử lý click nút quay lại
        /// </summary>
        private void BtnBack_Click(object sender, EventArgs e)
        {
            NavigateBackToDashboard();
        }

        /// <summary>
        /// Quay lại form Dashboard
        /// </summary>
        private void NavigateBackToDashboard()
        {
            try
            {
                // Ẩn form đăng ký
                this.Hide();

                // Hiển thị lại form Dashboard nếu có
                if (_parentDashboard != null && !_parentDashboard.IsDisposed)
                {
                    _parentDashboard.ShowDashboard();
                }
                else
                {
                    // Nếu không có parent, đóng form này
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Chuyển đến form PT Dashboard sau khi đăng ký thành công
        /// </summary>
        private void NavigateToPTDashboard()
        {
            try
            {   
                // Mở form PT Dashboard trước
                var frmPT = new frm_HuanLuyenVien(_parentDashboard);
                frmPT.StartPosition = FormStartPosition.CenterScreen;
                frmPT.Show();

                // Ẩn tất cả form khác (Dashboard, form đăng ký, v.v.) để chỉ còn lại PT Dashboard
                foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                {
                    if (form != frmPT)
                    {
                        form.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển đến PT Dashboard: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Nếu có lỗi, quay lại Dashboard (nếu còn tồn tại)
                if (_parentDashboard != null && !_parentDashboard.IsDisposed)
                {
                    _parentDashboard.Show();
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Nếu đóng form, quay lại Dashboard thay vì đóng hoàn toàn
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                NavigateBackToDashboard();
                return;
            }

            _ptController?.Dispose();
            base.OnFormClosing(e);
        }

        private void txt_ThanhPho_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbl_ThanhPho_Click(object sender, EventArgs e)
        {

        }
    }
}
