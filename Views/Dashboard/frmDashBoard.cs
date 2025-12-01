using HealthApp.Views.Nutrition;
using HealthApp.Common.Helpers;
using HealthApp.Views.PT;
using HealthApp.Views.Auth;
using HealthApp.Views.Settings;
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
    public partial class frmDashBoard : Form
    {
        private HealthApp.Views.PT.frm_DangKy _frmDangKy;

        public frmDashBoard()
        {
            InitializeComponent();
            InitializeEventHandlers();
            LoadUserInfo();
            LoadUserControl();
        }

        private void InitializeEventHandlers()
        {
            // Gắn event handler cho các button trong footer
            picHome.Click += PicHome_Click;
            picAnUong.Click += PicAnUong_Click;
            ptrDangKyLamPT.Click += PtrDangKyLamPT_Click;
            
            // Nút cài đặt (hình bánh răng)
            btnSettings.Click += BtnSettings_Click;

            Back.Click += BtnBack_Click;

            // Menu dropdown
            btnDropDown.Click += BtnDropDown_Click;
            btnQuanLyLuyenTapVoiPT.Click += BtnQuanLyLuyenTapVoiPT_Click;
            btnCheDoPT.Click += BtnCheDoPT_Click;
            btnThanhToan.Click += BtnThanhToan_Click;
        }

        /// <summary>
        /// Load thông tin user vào dropdown button
        /// </summary>
        private void LoadUserInfo()
        {
            try
            {
                if (CurrentUser.IsLoggedIn && CurrentUser.User != null)
                {
                    var user = CurrentUser.User;
                    string displayName = string.IsNullOrWhiteSpace(user.HoTen) 
                        ? user.Username 
                        : user.HoTen;
                    btnDropDown.Text = displayName;
                }
                else
                {
                    btnDropDown.Text = "Tài khoản";
                }
            }
            catch (Exception ex)
            {
                btnDropDown.Text = "Tài khoản";
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load thông tin user: {ex.Message}");
            }
        }

        /// <summary>
        /// Public method để reload thông tin user (được gọi từ bên ngoài)
        /// </summary>
        public void ReloadUserInfo()
        {
            LoadUserInfo();
        }

        private void LoadUserControl()
        {
            ucDashBoard ucDashBoard = new ucDashBoard(this);
            LoadUserControl(ucDashBoard);
        }

        /// <summary>
        /// Public method để reload trang chủ (được gọi từ bên ngoài)
        /// </summary>
        public void ReloadDashboard()
        {
            LoadUserControl();
        }

        /// <summary>
        /// Load một UserControl vào pnlBody, giữ nguyên header và footer
        /// </summary>
        /// <param name="userControl">UserControl cần load</param>
        public void LoadUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Top;
            userControl.AutoSize = true;
            userControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            pnlBody.Controls.Clear();
            pnlBody.Controls.Add(userControl);

            // KHÓA SCROLL NGANG HOÀN TOÀN
            pnlBody.HorizontalScroll.Maximum = 0;
            pnlBody.HorizontalScroll.Visible = false;
            pnlBody.HorizontalScroll.Enabled = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pnlBody_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler cho button Home - điều hướng về trang chủ
        /// </summary>
        private void PicHome_Click(object sender, EventArgs e)
        {
            try
            {
                // Load lại ucDashBoard (trang chủ)
                ucDashBoard ucDashBoard = new ucDashBoard(this);
                LoadUserControl(ucDashBoard);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng về trang chủ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Ăn Uống - điều hướng tới trang chế độ ăn uống đề xuất
        /// </summary>
        private void PicAnUong_Click(object sender, EventArgs e)
        {
            try
            {
                // Load ucCheDoAnUongDeXuat (trang chế độ ăn uống đề xuất)
                ucCheDoAnUongDeXuat ucCheDoAnUongDeXuat = new ucCheDoAnUongDeXuat();
                LoadUserControl(ucCheDoAnUongDeXuat);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng tới trang chế độ ăn uống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Đăng ký làm PT - chuyển sang form đăng ký PT hoặc PT Dashboard
        /// </summary>
        private void PtrDangKyLamPT_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã đăng nhập chưa
                if (!Common.Helpers.CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi đăng ký làm PT!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = Common.Helpers.CurrentUser.User;
                
                // Kiểm tra nếu đã là PT thì mở PT Dashboard
                if (user != null && user.Role == "PT")
                {
                    // Ẩn Dashboard và hiển thị PT Dashboard
                    this.Hide();
                    var frmPT = new HealthApp.Views.PT.frm_HuanLuyenVien(this);
                    frmPT.Show();
                }
                else
                {
                    // Chưa là PT, mở form đăng ký
                    if (_frmDangKy == null || _frmDangKy.IsDisposed)
                    {
                        _frmDangKy = new HealthApp.Views.PT.frm_DangKy(this);
                    }

                    // Ẩn form Dashboard và hiển thị form đăng ký
                    this.Hide();
                    _frmDangKy.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chuyển sang form PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hiển thị lại form Dashboard (được gọi từ frm_DangKy hoặc frm_ThanhToanPT khi quay lại)
        /// </summary>
        public void ShowDashboard()
        {
            this.Show();
            this.BringToFront();
            this.Activate();
            
            // Reload trang chủ để cập nhật dữ liệu sau khi thanh toán
            LoadUserControl();
        }

        /// <summary>

        /// Event handler cho menu item "Đăng xuất"
        /// </summary>
        private void MenuItemDangXuat_Click(object sender, EventArgs e)
        {
            try
            {
                // Xác nhận đăng xuất
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Đăng xuất
                    CurrentUser.Logout();

                    // Đóng form Dashboard
                    this.Hide();

                    // Mở lại form đăng nhập
                    var loginForm = new LoginForm();
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // Nếu đăng nhập thành công, mở lại Dashboard và đóng form cũ
                        var newDashboard = new frmDashBoard();
                        this.Close();
                        newDashboard.Show();
                    }
                    else
                    {
                        // Nếu không đăng nhập, đóng ứng dụng
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho menu item "Thanh toán PT"
        /// </summary>
        private void MenuItemThanhToanPT_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi thanh toán!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tìm các yêu cầu đã được PT đồng ý (có PTID) nhưng chưa thanh toán (TrangThai = "Pending")
                using (var context = new HealthApp.Models.WF_HealthTracker())
                {
                    var pendingPayments = context.DatLichPT
                        .Where(d => d.KhachHangID == CurrentUser.User.UserID && 
                                   d.TrangThai == "Pending" &&
                                   !string.IsNullOrEmpty(d.PTID))
                        .OrderByDescending(d => d.NgayTao)
                        .ToList();

                    if (pendingPayments.Count == 0)
                    {
                        MessageBox.Show("Bạn không có yêu cầu nào cần thanh toán!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Nếu có nhiều yêu cầu, chọn yêu cầu đầu tiên (có thể mở dialog chọn sau)
                    // Hoặc mở form với yêu cầu đầu tiên
                    var firstPayment = pendingPayments.First();
                    
                    // Ẩn Dashboard và hiển thị form thanh toán PT
                    this.Hide();
                    var frmThanhToan = new HealthApp.Views.PT.frm_ThanhToanPT(this, firstPayment.DatLichID);
                    frmThanhToan.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang thanh toán PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Mở trang cài đặt trong vùng nội dung chính
        /// </summary>
        private void BtnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                var ucSetting = new ucSetting();
                LoadUserControl(ucSetting);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang cài đặt: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                LoadUserControl(); // quay lại dashboard chính
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể quay lại màn hình chính: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblGreeting_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler cho button dropdown - toggle menu
        /// </summary>
        private void BtnDropDown_Click(object sender, EventArgs e)
        {
            try
            {
                // Toggle visibility của dropdown menu
                guna2Panel3.Visible = !guna2Panel3.Visible;
                
                // Đảm bảo dropdown hiển thị trên cùng (overlay, không đẩy controls khác)
                if (guna2Panel3.Visible)
                {
                    // Đưa dropdown lên trên cùng (z-order cao nhất) - overlay trên tất cả controls
                    guna2Panel3.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở menu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button "Quản Lý Luyện Tập Với PT" - điều hướng đến BaiTapCuaPTGiao
        /// </summary>
        private void BtnQuanLyLuyenTapVoiPT_Click(object sender, EventArgs e)
        {
            try
            {
                // Đóng dropdown
                guna2Panel3.Visible = false;

                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi xem bài tập đã giao!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Load BaiTapCuaPTGiao UserControl
                var baiTapCuaPTGiao = new BaiTapCuaPTGiao();
                LoadUserControl(baiTapCuaPTGiao);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang quản lý luyện tập với PT: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button "Chế Độ PT" - điều hướng đến frm_DangKy nếu chưa là PT, hoặc frm_HuanLuyenVien nếu đã là PT
        /// </summary>
        private void BtnCheDoPT_Click(object sender, EventArgs e)
        {
            try
            {
                // Đóng dropdown
                guna2Panel3.Visible = false;

                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi mở chế độ PT!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = CurrentUser.User;

                // Kiểm tra nếu đã là PT thì mở PT Dashboard
                if (user != null && user.Role == "PT")
                {
                    // Ẩn Dashboard và hiển thị PT Dashboard
                    this.Hide();
                    var frmPT = new frm_HuanLuyenVien(this);
                    frmPT.Show();
                }
                else
                {
                    // Chưa là PT, mở form đăng ký
                    if (_frmDangKy == null || _frmDangKy.IsDisposed)
                    {
                        _frmDangKy = new HealthApp.Views.PT.frm_DangKy(this);
                    }

                    // Ẩn form Dashboard và hiển thị form đăng ký
                    this.Hide();
                    _frmDangKy.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chế độ PT: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button "Thanh Toán" - điều hướng đến frm_ThanhToanPT
        /// </summary>
        private void BtnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                // Đóng dropdown
                guna2Panel3.Visible = false;

                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi thanh toán!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tìm các yêu cầu đã được PT đồng ý (có PTID) nhưng chưa thanh toán (TrangThai = "Pending")
                using (var context = new HealthApp.Models.WF_HealthTracker())
                {
                    var pendingPayments = context.DatLichPT
                        .Where(d => d.KhachHangID == CurrentUser.User.UserID &&
                                   d.TrangThai == "Pending" &&
                                   !string.IsNullOrEmpty(d.PTID))
                        .OrderByDescending(d => d.NgayTao)
                        .ToList();

                    if (pendingPayments.Count == 0)
                    {
                        MessageBox.Show("Bạn không có yêu cầu nào cần thanh toán!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Nếu có nhiều yêu cầu, chọn yêu cầu đầu tiên (có thể mở dialog chọn sau)
                    var firstPayment = pendingPayments.First();

                    // Ẩn Dashboard và hiển thị form thanh toán PT
                    this.Hide();
                    var frmThanhToan = new frm_ThanhToanPT(this, firstPayment.DatLichID);
                    frmThanhToan.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang thanh toán PT: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
