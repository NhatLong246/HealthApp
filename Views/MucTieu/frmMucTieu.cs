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
using HealthApp.Models;
using HealthApp.Views.Dashboard;
using HealthApp.Views.Food;
using HealthApp.Views.Reports;
using HealthApp.Views.Settings;
using HealthApp.Views.Auth;

namespace HealthApp.Views.MucTieu
{
    public partial class frmMucTieu : Form
    {
        private ContextMenuStrip _accountMenu;

        public frmMucTieu()
        {
            InitializeComponent();
            LoadUserControl();
            InitializeHeaderEvents();
            LoadUserInfo();
        }

        /// <summary>
        /// Khởi tạo UserControl mục tiêu vào panel
        /// </summary>
        private void LoadUserControl()
        {
            ucMucTieu ucMucTieu = new ucMucTieu
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            pnlMucTieu.Controls.Clear();
            pnlMucTieu.Controls.Add(ucMucTieu);

            // KHÓA SCROLL NGANG HOÀN TOÀN
            pnlMucTieu.HorizontalScroll.Maximum = 0;
            pnlMucTieu.HorizontalScroll.Visible = false;
            pnlMucTieu.HorizontalScroll.Enabled = false;
        }

        /// <summary>
        /// Khởi tạo event cho header giống frmDashBoard1
        /// </summary>
        private void InitializeHeaderEvents()
        {
            // Header buttons
            btnHome.Click += BtnHome_Click;
            btnFood.Click += BtnFood_Click;
            btnLichLuyenTap.Click += BtnLichLuyenTap_Click;
            btnThongke.Click += BtnThongke_Click;
            btnHoaDon.Click += BtnHoaDon_Click;

            // Account panel dropdown menu
            InitializeAccountMenu();
            pnlTaiKhoang.Click += PnlTaiKhoang_Click;
            lblTenNguoiDung.Click += PnlTaiKhoang_Click;
            ptrAnhNguoiDung.Click += PnlTaiKhoang_Click;
        }

        /// <summary>
        /// Khởi tạo dropdown menu cho pnlTaiKhoang
        /// </summary>
        private void InitializeAccountMenu()
        {
            _accountMenu = new ContextMenuStrip
            {
                Font = new Font("Segoe UI", 10F)
            };

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
        /// Hiển thị dropdown menu tài khoản
        /// </summary>
        private void PnlTaiKhoang_Click(object sender, EventArgs e)
        {
            try
            {
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
        /// Mở form thông tin cơ bản người dùng
        /// </summary>
        private void MenuItemThongTin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem thông tin cơ bản!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var frmThongTin = new frmThongTinhTheTrang(isMandatory: false);
                var result = frmThongTin.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    LoadUserInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thông tin cơ bản: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Đăng xuất và quay về màn hình đăng nhập
        /// </summary>
        private void MenuItemDangXuat_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CurrentUser.Logout();

                    this.Hide();

                    var loginForm = new LoginForm();
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        var newDashboard = new frmDashBoard1();
                        this.Close();
                        newDashboard.Show();
                    }
                    else
                    {
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
        /// Load tên người dùng lên header
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
            catch (Exception)
            {
                lblTenNguoiDung.Text = "Khách";
            }
        }

        /// <summary>
        /// Quay về Dashboard chính
        /// </summary>
        private void ReturnToDashboard()
        {
            var dashboard = Application.OpenForms.OfType<frmDashBoard1>().FirstOrDefault();

            if (dashboard != null)
            {
                this.Hide();
                dashboard.ShowDashboard();
            }
            else
            {
                this.Hide();
                var newDashboard = new frmDashBoard1();
                newDashboard.Show();
            }
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            ReturnToDashboard();
        }

        private void BtnFood_Click(object sender, EventArgs e)
        {
            try
            {
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
                    this.Hide();
                    existingForm.Show();
                    existingForm.BringToFront();
                }
                else
                {
                    this.Hide();
                    var frmFoodLibrary = new frm_FoodLibrary();
                    frmFoodLibrary.FormClosed += (s, args) => ReturnToDashboard();
                    frmFoodLibrary.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang thư viện thực phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReturnToDashboard();
            }
        }

        private void BtnLichLuyenTap_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                var containerForm = new Form
                {
                    Text = "Kế Hoạch Luyện Tập",
                    StartPosition = FormStartPosition.CenterScreen,
                    Size = new Size(1200, 800),
                    AutoScroll = true
                };

                var ucKeHoach = new HealthApp.Views.KeHoachLuyenTap.ucKeHoachLuyenTap
                {
                    Dock = DockStyle.Fill
                };
                containerForm.Controls.Add(ucKeHoach);

                containerForm.FormClosed += (s, args) => ReturnToDashboard();

                containerForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang kế hoạch luyện tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReturnToDashboard();
            }
        }

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
                    reportForm.FormClosed += (s, args) => ReturnToDashboard();
                    reportForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReturnToDashboard();
            }
        }

        private void BtnHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi xem hóa đơn!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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

                    var firstPayment = pendingPayments.First();

                    this.Hide();
                    var frmThanhToan = new HealthApp.Views.PT.frm_ThanhToanPT(null, firstPayment.DatLichID);
                    frmThanhToan.FormClosed += (s, args) => ReturnToDashboard();
                    frmThanhToan.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang hóa đơn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReturnToDashboard();
            }
        }

        private void pnlMucTieu_Paint(object sender, PaintEventArgs e)
        {
            // Paint event - không cần xử lý gì ở đây
        }
    }
}
