using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Views.Food;
using HealthApp.Views.KeHoachLuyenTap;
using HealthApp.Views.Reports;
using HealthApp.Views.PT;
using HealthApp.Views.MucTieu;
using HealthApp.Views.Nutrition;
using HealthApp.Views.Settings;
using HealthApp.Views.Auth;
using HealthApp.Models;

namespace HealthApp.Views.Dashboard
{
    public partial class frmDashBoard1 : Form, IDashboardForm
    {
        private HealthApp.Views.PT.frm_DangKy _frmDangKy;

        public frmDashBoard1()
        {
            InitializeComponent();
            InitializeEventHandlers();
            LoadUserInfo();
            UpdatePTButtonState();
        }

        private ContextMenuStrip _accountMenu;

        private void InitializeEventHandlers()
        {
            // Header buttons
            btnHome.Click += BtnHome_Click;
            btnFood.Click += BtnFood_Click;
            btnLichLuyenTap.Click += BtnLichLuyenTap_Click;
            btnThongke.Click += BtnThongke_Click;
            btnHoaDon.Click += BtnHoaDon_Click;

            // Body buttons
            btnMucTieu.Click += BtnMucTieu_Click;
            btnLenKeHoachAnUong.Click += BtnLenKeHoachAnUong_Click;
            btnThuePT.Click += BtnThuePT_Click;
            btnLichPT.Click += BtnLichPT_Click;
            btnDangKyLamPT.Click += BtnDangKyLamPT_Click;

            // Account panel dropdown menu
            InitializeAccountMenu();
            pnlTaiKhoang.Click += PnlTaiKhoang_Click;
        }

        /// <summary>
        /// Khởi tạo dropdown menu cho pnlTaiKhoang
        /// </summary>
        private void InitializeAccountMenu()
        {
            _accountMenu = new ContextMenuStrip();
            _accountMenu.Font = new Font("Segoe UI", 10F);

            // Menu item "Thông tin cơ bản"
            var menuItemThongTin = new ToolStripMenuItem("Thông tin cơ bản");
            menuItemThongTin.Click += MenuItemThongTin_Click;
            _accountMenu.Items.Add(menuItemThongTin);

            // Menu item "Đăng xuất"
            var menuItemDangXuat = new ToolStripMenuItem("Đăng xuất");
            menuItemDangXuat.Click += MenuItemDangXuat_Click;
            _accountMenu.Items.Add(menuItemDangXuat);
        }

        /// <summary>
        /// Event handler khi click vào pnlTaiKhoang - hiển thị dropdown menu
        /// </summary>
        private void PnlTaiKhoang_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị menu tại vị trí dưới pnlTaiKhoang
                var location = pnlTaiKhoang.PointToScreen(new Point(0, pnlTaiKhoang.Height));
                _accountMenu.Show(location);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị menu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho menu item "Thông tin cơ bản"
        /// </summary>
        private void MenuItemThongTin_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem thông tin cơ bản!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mở form thông tin thể trạng (không bắt buộc)
                var frmThongTin = new frmThongTinhTheTrang(isMandatory: false);
                var result = frmThongTin.ShowDialog(this);

                // Nếu lưu thành công, reload lại thông tin user
                if (result == DialogResult.OK)
                {
                    ReloadUserInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thông tin cơ bản: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load thông tin user vào label
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
                    lblTenNguoiDung.Text = displayName;
                }
                else
                {
                    lblTenNguoiDung.Text = "Khách";
                }
            }
            catch (Exception ex)
            {
                lblTenNguoiDung.Text = "Khách";
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load thông tin user: {ex.Message}");
            }
        }

        /// <summary>
        /// Public method để reload thông tin user (được gọi từ bên ngoài)
        /// </summary>
        public void ReloadUserInfo()
        {
            LoadUserInfo();
            UpdatePTButtonState();
        }

        /// <summary>
        /// Cập nhật trạng thái button Đăng ký PT dựa trên role của user
        /// </summary>
        private void UpdatePTButtonState()
        {
            try
            {
                if (CurrentUser.IsLoggedIn && CurrentUser.User != null)
                {
                    var user = CurrentUser.User;
                    if (user.Role == "PT")
                    {
                        btnDangKyLamPT.Text = "Chế độ PT";
                    }
                    else
                    {
                        btnDangKyLamPT.Text = "Đăng ký PT";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật trạng thái button PT: {ex.Message}");
            }
        }

        /// <summary>
        /// Hiển thị lại form Dashboard (được gọi từ các form con khi quay lại)
        /// </summary>
        public void ShowDashboard()
        {
            this.Show();
            this.BringToFront();
            this.Activate();
            LoadUserInfo();
            UpdatePTButtonState();
        }

        /// <summary>
        /// Load một UserControl vào pnlBody (nếu cần thiết cho tương lai)
        /// </summary>
        /// <param name="userControl">UserControl cần load</param>
        public void LoadUserControl(UserControl userControl)
        {
            try
            {
                userControl.Dock = DockStyle.Fill;
                userControl.AutoSize = false;

                // Xóa các control cũ trong pnlBody (trừ các control cố định nếu có)
                // Lưu ý: frmDashBoard1 có giao diện cố định, nên method này có thể không cần thiết
                // Nhưng thêm vào để tương thích với code hiện tại
                pnlBody.Controls.Clear();
                pnlBody.Controls.Add(userControl);

                // KHÓA SCROLL NGANG HOÀN TOÀN
                pnlBody.HorizontalScroll.Maximum = 0;
                pnlBody.HorizontalScroll.Visible = false;
                pnlBody.HorizontalScroll.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load UserControl: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Public method để reload trang chủ (được gọi từ bên ngoài)
        /// </summary>
        public void ReloadDashboard()
        {
            LoadUserInfo();
            UpdatePTButtonState();
        }

        /// <summary>
        /// Event handler cho button Home - reload trang chủ
        /// </summary>
        private void BtnHome_Click(object sender, EventArgs e)
        {
            try
            {
                // Reload lại form hiện tại (refresh)
                LoadUserInfo();
                UpdatePTButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lại trang chủ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Food - điều hướng tới frm_FoodLibrary
        /// </summary>
        private void BtnFood_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem form đã mở chưa, nếu có thì chỉ hiển thị lại
                frm_FoodLibrary existingForm = null;
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is frm_FoodLibrary)
                    {
                        existingForm = openForm as frm_FoodLibrary;
                        break;
                    }
                }

                if (existingForm != null)
                {
                    // Form đã tồn tại, chỉ hiển thị lại
                    this.Hide();
                    existingForm.Show();
                    existingForm.BringToFront();
                }
                else
                {
                    // Tạo form mới
                    this.Hide();
                    var frmFoodLibrary = new frm_FoodLibrary();
                    frmFoodLibrary.FormClosed += (s, args) => this.ShowDashboard();
                    frmFoodLibrary.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang thư viện thực phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ShowDashboard();
            }
        }

        /// <summary>
        /// Event handler cho button Lịch Luyện Tập - điều hướng tới ucKeHoachLuyenTap
        /// </summary>
        private void BtnLichLuyenTap_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                // Tạo form container để hiển thị UserControl
                var containerForm = new Form
                {
                    Text = "Kế Hoạch Luyện Tập",
                    StartPosition = FormStartPosition.CenterScreen,
                    Size = new Size(1200, 800),
                    AutoScroll = true
                };

                var ucKeHoachLuyenTap = new ucKeHoachLuyenTap();
                ucKeHoachLuyenTap.Dock = DockStyle.Fill;
                containerForm.Controls.Add(ucKeHoachLuyenTap);
                
                // Khi form đóng, quay lại dashboard
                containerForm.FormClosed += (s, args) => this.ShowDashboard();
                
                // Tag form container với reference đến dashboard để UserControl có thể truy cập
                containerForm.Tag = this;
                
                containerForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang kế hoạch luyện tập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ShowDashboard();
            }
        }

        /// <summary>
        /// Event handler cho button Thống Kê - điều hướng tới ReportForm
        /// </summary>
        private void BtnThongke_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem báo cáo!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem form đã mở chưa
                ReportForm existingForm = null;
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is ReportForm)
                    {
                        existingForm = openForm as ReportForm;
                        break;
                    }
                }

                if (existingForm != null)
                {
                    this.Hide();
                    existingForm.Show();
                    existingForm.BringToFront();
                }
                else
                {
                    this.Hide();
                    var reportForm = new ReportForm();
                    reportForm.FormClosed += (s, args) => this.ShowDashboard();
                    reportForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang thống kê: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ShowDashboard();
            }
        }

        /// <summary>
        /// Event handler cho button Hóa Đơn - điều hướng tới frm_ThanhToanPT
        /// </summary>
        private void BtnHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi xem hóa đơn!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tìm các yêu cầu đã được PT đồng ý (có PTID) nhưng chưa thanh toán (TrangThai = "Pending")
                using (var context = new WF_HealthTracker())
                {
                    var pendingPayments = context.DatLichPT
                        .Where(d => d.KhachHangID == CurrentUser.User.UserID &&
                                   d.TrangThai == "Pending" &&
                                   !string.IsNullOrEmpty(d.PTID))
                        .OrderByDescending(d => d.NgayTao)
                        .ToList();

                    if (pendingPayments.Count == 0)
                    {
                        MessageBox.Show("Bạn không có hóa đơn nào cần thanh toán!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Nếu có nhiều yêu cầu, chọn yêu cầu đầu tiên
                    var firstPayment = pendingPayments.First();

                    // Ẩn Dashboard và hiển thị form thanh toán PT
                    this.Hide();
                    var frmThanhToan = new frm_ThanhToanPT(this, firstPayment.DatLichID);
                    frmThanhToan.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang hóa đơn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Mục Tiêu - điều hướng tới frmMucTieu
        /// </summary>
        private void BtnMucTieu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem form đã mở chưa
                frmMucTieu existingForm = null;
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is frmMucTieu)
                    {
                        existingForm = openForm as frmMucTieu;
                        break;
                    }
                }

                if (existingForm != null)
                {
                    this.Hide();
                    existingForm.Show();
                    existingForm.BringToFront();
                }
                else
                {
                    this.Hide();
                    var frmMucTieu = new frmMucTieu();
                    frmMucTieu.FormClosed += (s, args) => this.ShowDashboard();
                    frmMucTieu.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang mục tiêu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ShowDashboard();
            }
        }

        /// <summary>
        /// Event handler cho button Lên Kế Hoạch Ăn Uống - điều hướng tới ucCheDoAnUongDeXuat
        /// </summary>
        private void BtnLenKeHoachAnUong_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                // Tạo form container để hiển thị UserControl
                var containerForm = new Form
                {
                    Text = "Chế Độ Ăn Uống Đề Xuất",
                    StartPosition = FormStartPosition.CenterScreen,
                    Size = new Size(1200, 800),
                    AutoScroll = true
                };

                var ucCheDoAnUong = new ucCheDoAnUongDeXuat();
                ucCheDoAnUong.Dock = DockStyle.Fill;
                containerForm.Controls.Add(ucCheDoAnUong);
                
                // Khi form đóng, quay lại dashboard
                containerForm.FormClosed += (s, args) => this.ShowDashboard();
                
                // Tag form container với reference đến dashboard
                containerForm.Tag = this;
                
                containerForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang chế độ ăn uống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ShowDashboard();
            }
        }

        /// <summary>
        /// Event handler cho button Thuê PT - điều hướng tới frm_TimKiemHLV
        /// </summary>
        private void BtnThuePT_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi tìm kiếm PT!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ẩn Dashboard và mở form tìm kiếm PT
                this.Hide();
                var frmTimKiemPT = new frm_TimKiemHLV(this);
                frmTimKiemPT.FormClosed += (s, args) => this.ShowDashboard();
                frmTimKiemPT.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang tìm kiếm PT: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ShowDashboard();
            }
        }

        /// <summary>
        /// Event handler cho button Lịch PT - điều hướng tới LichLuyenTapUser
        /// </summary>
        private void BtnLichPT_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi xem lịch PT!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra xem form đã mở chưa
                HealthApp.Views.GiaoBTChoUser.LichLuyenTapUser existingForm = null;
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is HealthApp.Views.GiaoBTChoUser.LichLuyenTapUser)
                    {
                        existingForm = openForm as HealthApp.Views.GiaoBTChoUser.LichLuyenTapUser;
                        break;
                    }
                }

                if (existingForm != null)
                {
                    this.Hide();
                    existingForm.Show();
                    existingForm.BringToFront();
                }
                else
                {
                    this.Hide();
                    var lichLuyenTapUser = new HealthApp.Views.GiaoBTChoUser.LichLuyenTapUser(this);
                    lichLuyenTapUser.FormClosed += (s, args) => this.ShowDashboard();
                    lichLuyenTapUser.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang lịch PT: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ShowDashboard();
            }
        }

        /// <summary>
        /// Event handler cho button Đăng Ký Làm PT - chuyển sang form đăng ký PT hoặc PT Dashboard
        /// </summary>
        private void BtnDangKyLamPT_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi đăng ký làm PT!", "Thông báo",
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
                        _frmDangKy = new frm_DangKy(this);
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

        private void guna2Button1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        /// <summary>
        /// Event handler cho menu item "Đăng xuất" (nếu có menu dropdown)
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
                        var newDashboard = new frmDashBoard1();
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
    }
}
