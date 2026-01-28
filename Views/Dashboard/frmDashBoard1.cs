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
using Guna.UI2.WinForms;
using HealthApp.Services;

namespace HealthApp.Views.Dashboard
{
    public partial class frmDashBoard1 : Form, IDashboardForm
    {
        private HealthApp.Views.PT.frm_DangKy _frmDangKy;
        private Panel _userControlContainer; // Panel để chứa UserControl
        private UserControl _currentUserControl; // UserControl hiện tại đang hiển thị
        private Timer _notificationRefreshTimer;
        private Timer _notificationRuleTimer;

        public frmDashBoard1()
        {
            InitializeComponent();
            InitializeUserControlContainer();
            InitializeEventHandlers();
            LoadUserInfo();
            UpdatePTButtonState();
            // Đảm bảo panel tiêu đề luôn hiển thị phía trên
            pnlTieuDe?.BringToFront();
        }

        /// <summary>
        /// Khởi tạo panel container cho UserControl
        /// </summary>
        private void InitializeUserControlContainer()
        {
            _userControlContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false,
                AutoScroll = true
            };
            
            // Thêm panel vào pnlBackground (sau pnlBody để hiển thị trên cùng)
            pnlBackground.Controls.Add(_userControlContainer);
            _userControlContainer.BringToFront();
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
            
            // Thêm event handler cho các control con trong pnlTaiKhoang
            if (lblTenNguoiDung != null)
            {
                lblTenNguoiDung.Click += PnlTaiKhoang_Click;
                lblTenNguoiDung.Cursor = Cursors.Hand; // Đổi cursor thành hand để người dùng biết có thể click
            }
            
            if (ptrAnhNguoiDung != null)
            {
                ptrAnhNguoiDung.Click += PnlTaiKhoang_Click;
                ptrAnhNguoiDung.Cursor = Cursors.Hand; // Đổi cursor thành hand để người dùng biết có thể click
            }

            // Notification bell + badge
            InitializeNotificationHandlers();

            // Social media circle picture boxes (Facebook, YouTube, TikTok)
            InitializeSocialLinks();
        }

        /// <summary>
        /// Gán sự kiện click cho các icon mạng xã hội để mở link tương ứng.
        /// </summary>
        private void InitializeSocialLinks()
        {
            try
            {
                if (guna2CirclePictureBox4 != null)
                {
                    guna2CirclePictureBox4.Click += guna2CirclePictureBox4_Click;
                    guna2CirclePictureBox4.Cursor = Cursors.Hand;
                }

                if (guna2CirclePictureBox3 != null)
                {
                    guna2CirclePictureBox3.Click += guna2CirclePictureBox3_Click;
                    guna2CirclePictureBox3.Cursor = Cursors.Hand;
                }

                if (guna2CirclePictureBox2 != null)
                {
                    guna2CirclePictureBox2.Click += guna2CirclePictureBox2_Click;
                    guna2CirclePictureBox2.Cursor = Cursors.Hand;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi khởi tạo link mạng xã hội: {ex.Message}");
            }
        }

        private void OpenUrlInBrowser(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return;

                // .NET Core / modern Windows: UseProcessStartInfo
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở đường dẫn: {url}\nChi tiết: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Facebook group: https://www.facebook.com/groups/1775647979239578
        private void guna2CirclePictureBox4_Click(object sender, EventArgs e)
        {
            OpenUrlInBrowser("https://www.facebook.com/groups/1775647979239578");
        }

        // YouTube: https://www.youtube.com/@tapgymcungthantuong6827
        private void guna2CirclePictureBox3_Click(object sender, EventArgs e)
        {
            OpenUrlInBrowser("https://www.youtube.com/@tapgymcungthantuong6827");
        }

        // TikTok: https://www.tiktok.com/@sv.gym.247
        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {
            OpenUrlInBrowser("https://www.tiktok.com/@sv.gym.247");
        }

        private void InitializeNotificationHandlers()
        {
            if (ptrThongBao != null)
            {
                ptrThongBao.Cursor = Cursors.Hand;
                ptrThongBao.Click += PtrThongBao_Click;
            }

            if (lblnumberThongBao != null)
            {
                lblnumberThongBao.Cursor = Cursors.Hand;
                lblnumberThongBao.Click += PtrThongBao_Click;
            }

            // refresh badge định kỳ
            _notificationRefreshTimer = new Timer();
            _notificationRefreshTimer.Interval = 30_000; // 30s
            _notificationRefreshTimer.Tick += (s, e) => RefreshUnreadNotificationCount();
            _notificationRefreshTimer.Start();

            RefreshUnreadNotificationCount();

            // chạy rule engine mỗi 5 phút để tạo các thông báo theo thời gian
            _notificationRuleTimer = new Timer();
            _notificationRuleTimer.Interval = 5 * 60 * 1000;
            _notificationRuleTimer.Tick += (s, e) =>
            {
                try
                {
                    if (CurrentUser.IsLoggedIn && !string.IsNullOrWhiteSpace(CurrentUser.UserID))
                    {
                        NotificationRuleEngine.RunForUser(CurrentUser.UserID);
                        RefreshUnreadNotificationCount();
                    }
                }
                catch { }
            };
            _notificationRuleTimer.Start();

            // chạy 1 lần ngay khi mở form
            try
            {
                if (CurrentUser.IsLoggedIn && !string.IsNullOrWhiteSpace(CurrentUser.UserID))
                {
                    NotificationRuleEngine.RunForUser(CurrentUser.UserID);
                    RefreshUnreadNotificationCount();
                }
            }
            catch { }
        }

        /// <summary>
        /// Khởi tạo dropdown menu cho pnlTaiKhoang
        /// </summary>
        private void InitializeAccountMenu()
        {
            _accountMenu = new ContextMenuStrip();
            _accountMenu.Font = new Font("Segoe UI", 10F);

            // Menu item "Thông tin tài khoản"
            var menuItemThongTinTK = new ToolStripMenuItem("Thông tin tài khoản");
            menuItemThongTinTK.Click += MenuItemThongTinTK_Click;
            _accountMenu.Items.Add(menuItemThongTinTK);

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
        /// Event handler cho menu item "Thông tin tài khoản"
        /// </summary>
        private void MenuItemThongTinTK_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã đăng nhập chưa
                if (!CurrentUser.IsLoggedIn)
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem thông tin tài khoản!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mở form thông tin tài khoản
                var frmThongTinTK = new HealthApp.Views.Settings.frmThongTinTK(this);
                frmThongTinTK.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thông tin tài khoản: {ex.Message}", "Lỗi",
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
        /// Load thông tin user vào label và ảnh đại diện
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

                    // Load ảnh đại diện
                    LoadUserAvatar(user.AnhDaiDien);

                    // refresh badge thông báo
                    RefreshUnreadNotificationCount();
                }
                else
                {
                    lblTenNguoiDung.Text = "Khách";
                    // Reset về ảnh mặc định
                    LoadUserAvatar(null);
                    SetNotificationBadge(0);
                }
            }
            catch (Exception ex)
            {
                lblTenNguoiDung.Text = "Khách";
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load thông tin user: {ex.Message}");
                LoadUserAvatar(null);
                SetNotificationBadge(0);
            }
        }

        private void PtrThongBao_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || string.IsNullOrWhiteSpace(CurrentUser.UserID))
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem thông báo!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var dlg = BuildNotificationListDialog(CurrentUser.UserID))
                {
                    dlg.ShowDialog(this);
                }

                // Sau khi đóng dialog, refresh badge
                RefreshUnreadNotificationCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở thông báo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshUnreadNotificationCount()
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || string.IsNullOrWhiteSpace(CurrentUser.UserID))
                {
                    SetNotificationBadge(0);
                    return;
                }

                using (var context = new WF_HealthTracker())
                {
                    var userId = CurrentUser.UserID;
                    int unread = context.ThongBao.Count(t => t.UserID == userId && (t.DaDoc != true));
                    SetNotificationBadge(unread);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshUnreadNotificationCount error: {ex.Message}");
            }
        }

        private void SetNotificationBadge(int count)
        {
            try
            {
                if (lblnumberThongBao == null) return;

                if (count <= 0)
                {
                    lblnumberThongBao.Visible = false;
                    lblnumberThongBao.Text = "";
                    return;
                }

                lblnumberThongBao.Visible = true;
                lblnumberThongBao.Text = count > 99 ? "99+" : count.ToString();
            }
            catch { }
        }

        private sealed class NotificationListItem
        {
            public string ThongBaoID { get; set; }
            public string TieuDe { get; set; }
            public string NoiDung { get; set; }
            public string Loai { get; set; }
            public string MaLienQuan { get; set; }
            public bool DaDoc { get; set; }
            public DateTime? NgayTao { get; set; }
        }

        private Form BuildNotificationListDialog(string userId)
        {
            var form = new Form
            {
                Text = "Thông báo",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 247, 250),
                Size = new Size(920, 620)
            };

            var pnlMain = new Guna2ShadowPanel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                Radius = 15,
                ShadowColor = Color.Black,
                ShadowDepth = 10,
                Padding = new Padding(22)
            };
            form.Controls.Add(pnlMain);

            var lblTitle = new Label
            {
                Text = "Thông báo",
                Font = new Font("Times New Roman", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                AutoSize = false,
                Location = new Point(22, 18),
                Size = new Size(600, 40)
            };
            pnlMain.Controls.Add(lblTitle);

            var lblSub = new Label
            {
                Text = "Click vào 1 thông báo để xem chi tiết (tự đánh dấu đã đọc)",
                Font = new Font("Times New Roman", 11.5F),
                ForeColor = Color.FromArgb(110, 110, 110),
                AutoSize = false,
                Location = new Point(24, 58),
                Size = new Size(820, 24)
            };
            pnlMain.Controls.Add(lblSub);

            var btnClose = new Guna2Button
            {
                Text = "Đóng",
                Size = new Size(140, 42),
                BorderRadius = 10,
                FillColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(740, 520),
                Cursor = Cursors.Hand
            };
            btnClose.Click += (s, e) => form.Close();
            pnlMain.Controls.Add(btnClose);

            var btnRefresh = new Guna2Button
            {
                Text = "Làm mới",
                Size = new Size(140, 42),
                BorderRadius = 10,
                FillColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Location = new Point(590, 520),
                Cursor = Cursors.Hand
            };
            pnlMain.Controls.Add(btnRefresh);

            var lblEmpty = new Label
            {
                Text = "",
                Font = new Font("Times New Roman", 12F, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(24, 520),
                Visible = false
            };
            pnlMain.Controls.Add(lblEmpty);

            var flp = new FlowLayoutPanel
            {
                Location = new Point(22, 95),
                Size = new Size(858, 410),
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.White
            };
            pnlMain.Controls.Add(flp);

            Action loadList = null;
            loadList = () =>
            {
                flp.Controls.Clear();
                lblEmpty.Visible = false;

                List<NotificationListItem> items;
                using (var context = new WF_HealthTracker())
                {
                    items = context.ThongBao
                        .Where(t => t.UserID == userId)
                        .OrderByDescending(t => t.NgayTao)
                        .Take(200)
                        .Select(t => new NotificationListItem
                        {
                            ThongBaoID = t.ThongBaoID,
                            TieuDe = t.TieuDe,
                            NoiDung = t.NoiDung,
                            Loai = t.Loai,
                            MaLienQuan = t.MaLienQuan,
                            DaDoc = (t.DaDoc == true),
                            NgayTao = t.NgayTao
                        })
                        .ToList();
                }

                if (items.Count == 0)
                {
                    lblEmpty.Text = "Bạn chưa có thông báo nào.";
                    lblEmpty.Visible = true;
                    return;
                }

                foreach (var it in items)
                {
                    flp.Controls.Add(BuildNotificationCard(it, () =>
                    {
                        // open detail
                        using (var detail = BuildNotificationDetailDialog(it.ThongBaoID))
                        {
                            detail.ShowDialog(form);
                        }

                        // reload list + badge
                        loadList();
                        RefreshUnreadNotificationCount();
                    }));
                }
            };

            btnRefresh.Click += (s, e) => loadList();

            loadList();

            form.AcceptButton = btnClose;
            form.CancelButton = btnClose;
            return form;
        }

        private Control BuildNotificationCard(NotificationListItem it, Action onOpen)
        {
            var card = new Guna2ShadowPanel
            {
                Width = 830,
                Height = 86,
                FillColor = it.DaDoc ? Color.White : Color.FromArgb(239, 246, 255),
                Radius = 12,
                ShadowColor = Color.Black,
                ShadowDepth = 6,
                Padding = new Padding(12),
                Margin = new Padding(0, 0, 0, 10),
                Cursor = Cursors.Hand
            };

            var title = new Label
            {
                Text = string.IsNullOrWhiteSpace(it.TieuDe) ? "Thông báo" : it.TieuDe,
                Font = new Font("Times New Roman", 12.5F, it.DaDoc ? FontStyle.Bold : FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                AutoSize = false,
                Location = new Point(12, 10),
                Size = new Size(660, 24)
            };
            card.Controls.Add(title);

            var preview = new Label
            {
                Text = (it.NoiDung ?? "").Length > 90 ? (it.NoiDung.Substring(0, 90) + "...") : (it.NoiDung ?? ""),
                Font = new Font("Times New Roman", 10.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(90, 90, 90),
                AutoSize = false,
                Location = new Point(12, 36),
                Size = new Size(760, 20)
            };
            card.Controls.Add(preview);

            var meta = new Label
            {
                Text = $"{(string.IsNullOrWhiteSpace(it.Loai) ? "Chung" : it.Loai)} • {(it.NgayTao.HasValue ? it.NgayTao.Value.ToString("dd/MM/yyyy HH:mm") : "N/A")}",
                Font = new Font("Times New Roman", 10F, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = false,
                Location = new Point(12, 58),
                Size = new Size(760, 20)
            };
            card.Controls.Add(meta);

            var dot = new Guna2CirclePictureBox
            {
                Size = new Size(10, 10),
                Location = new Point(800, 14),
                BackColor = Color.Transparent,
                FillColor = it.DaDoc ? Color.Transparent : Color.FromArgb(239, 68, 68)
            };
            card.Controls.Add(dot);

            void handler(object s, EventArgs e) => onOpen?.Invoke();
            card.Click += handler;
            title.Click += handler;
            preview.Click += handler;
            meta.Click += handler;

            return card;
        }

        private Form BuildNotificationDetailDialog(string thongBaoId)
        {
            var form = new Form
            {
                Text = "Chi tiết thông báo",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 247, 250),
                Size = new Size(780, 520)
            };

            var pnlMain = new Guna2ShadowPanel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                Radius = 15,
                ShadowColor = Color.Black,
                ShadowDepth = 10,
                Padding = new Padding(22)
            };
            form.Controls.Add(pnlMain);

            ThongBao tb;
            using (var context = new WF_HealthTracker())
            {
                tb = context.ThongBao.FirstOrDefault(t => t.ThongBaoID == thongBaoId);
            }

            if (tb == null)
            {
                var lbl = new Label
                {
                    Text = "Không tìm thấy thông báo!",
                    Font = new Font("Times New Roman", 12F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(22, 22)
                };
                pnlMain.Controls.Add(lbl);
                return form;
            }

            // Mark as read on open
            try
            {
                using (var context = new WF_HealthTracker())
                {
                    var db = context.ThongBao.FirstOrDefault(t => t.ThongBaoID == thongBaoId);
                    if (db != null && db.DaDoc != true)
                    {
                        db.DaDoc = true;
                        context.SaveChanges();
                    }
                }
            }
            catch { }

            // Layout constants
            const int pad = 22;
            const int topTitle = 18;
            const int titleHeight = 42;
            const int gapSmall = 8;
            const int gap = 16;
            const int buttonRowHeight = 46;

            var lblTitle = new Label
            {
                Text = string.IsNullOrWhiteSpace(tb.TieuDe) ? "Thông báo" : tb.TieuDe,
                Font = new Font("Times New Roman", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                AutoSize = false,
                Location = new Point(pad, topTitle),
                Size = new Size(pnlMain.Width - (pad * 2), titleHeight),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(lblTitle);

            var lblMeta = new Label
            {
                Text = $"{(string.IsNullOrWhiteSpace(tb.Loai) ? "Chung" : tb.Loai)} • {(tb.NgayTao.HasValue ? tb.NgayTao.Value.ToString("dd/MM/yyyy HH:mm") : "N/A")}",
                Font = new Font("Times New Roman", 11F, FontStyle.Italic),
                ForeColor = Color.FromArgb(110, 110, 110),
                AutoSize = false,
                Location = new Point(pad, topTitle + titleHeight + gapSmall),
                Size = new Size(pnlMain.Width - (pad * 2), 22),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(lblMeta);

            // Buttons row panel (bottom)
            var pnlButtons = new Panel
            {
                Height = buttonRowHeight,
                Dock = DockStyle.Bottom,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 0)
            };
            pnlMain.Controls.Add(pnlButtons);

            var txt = new Guna2TextBox
            {
                Location = new Point(pad, topTitle + titleHeight + 22 + gap),
                Size = new Size(pnlMain.Width - (pad * 2), pnlMain.Height - (topTitle + titleHeight + 22 + gap) - (buttonRowHeight + 35)),
                Multiline = true,
                ReadOnly = true,
                Text = tb.NoiDung ?? "",
                BorderRadius = 12,
                BorderColor = Color.FromArgb(229, 231, 235),
                FillColor = Color.White,
                Font = new Font("Times New Roman", 12F),
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(txt);

            var btnDelete = new Guna2Button
            {
                Text = "Xóa",
                Size = new Size(140, 42),
                BorderRadius = 10,
                FillColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Location = new Point(0, 2),
                Cursor = Cursors.Hand
            };
            pnlButtons.Controls.Add(btnDelete);

            var btnClose = new Guna2Button
            {
                Text = "Đóng",
                Size = new Size(140, 42),
                BorderRadius = 10,
                FillColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(pnlButtons.Width - 140, 2),
                Cursor = Cursors.Hand
            };
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlButtons.Controls.Add(btnClose);

            // Keep spacing between buttons and edges
            pnlButtons.Padding = new Padding(pad, 0, pad, 10);
            btnDelete.Location = new Point(pad, 2);
            btnClose.Location = new Point(pnlButtons.Width - pad - btnClose.Width, 2);

            btnClose.Click += (s, e) => form.Close();
            btnDelete.Click += (s, e) =>
            {
                var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa thông báo này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                try
                {
                    using (var context = new WF_HealthTracker())
                    {
                        var db = context.ThongBao.FirstOrDefault(t => t.ThongBaoID == thongBaoId);
                        if (db != null)
                        {
                            context.ThongBao.Remove(db);
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                form.Close();
            };

            form.AcceptButton = btnClose;
            form.CancelButton = btnClose;

            // Re-layout on resize (DPI/Font changes)
            form.Shown += (s, e) =>
            {
                // ensure bottom panel right aligned
                btnClose.Location = new Point(pnlButtons.Width - pad - btnClose.Width, 2);
                txt.Size = new Size(pnlMain.Width - (pad * 2), pnlMain.Height - txt.Location.Y - (buttonRowHeight + 20));
            };
            form.SizeChanged += (s, e) =>
            {
                btnClose.Location = new Point(pnlButtons.Width - pad - btnClose.Width, 2);
                txt.Size = new Size(pnlMain.Width - (pad * 2), pnlMain.Height - txt.Location.Y - (buttonRowHeight + 20));
                lblTitle.Size = new Size(pnlMain.Width - (pad * 2), titleHeight);
                lblMeta.Size = new Size(pnlMain.Width - (pad * 2), 22);
            };

            return form;
        }

        /// <summary>
        /// Load ảnh đại diện của user vào ptrAnhNguoiDung
        /// </summary>
        private void LoadUserAvatar(string imagePath)
        {
            try
            {
                if (ptrAnhNguoiDung == null)
                    return;

                if (string.IsNullOrEmpty(imagePath))
                {
                    // Reset về ảnh mặc định từ Resources/Icons/icons8-bench-press.gif
                    LoadDefaultAvatar();
                    return;
                }

                string fullPath = imagePath;

                // Nếu là đường dẫn relative, thử tìm trong thư mục Resources
                if (!Path.IsPathRooted(imagePath))
                {
                    var appDirectory = Application.StartupPath;
                    fullPath = Path.Combine(appDirectory, "Resources", imagePath);
                }

                if (File.Exists(fullPath))
                {
                    ptrAnhNguoiDung.Image = Image.FromFile(fullPath);
                }
                else if (File.Exists(imagePath))
                {
                    // Thử đường dẫn trực tiếp nếu không tìm thấy
                    ptrAnhNguoiDung.Image = Image.FromFile(imagePath);
                }
                else
                {
                    // Không tìm thấy file, load ảnh mặc định
                    System.Diagnostics.Debug.WriteLine($"Không tìm thấy ảnh đại diện: {imagePath}");
                    LoadDefaultAvatar();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load ảnh đại diện: {ex.Message}");
                // Load ảnh mặc định nếu có lỗi
                LoadDefaultAvatar();
            }
        }

        /// <summary>
        /// Load ảnh mặc định từ Resources/Icons/icons8-bench-press.gif
        /// </summary>
        private void LoadDefaultAvatar()
        {
            try
            {
                if (ptrAnhNguoiDung == null)
                    return;

                var appDirectory = Application.StartupPath;
                var defaultImagePath = Path.Combine(appDirectory, "Resources", "Icons", "icons8-bench-press.gif");

                if (File.Exists(defaultImagePath))
                {
                    ptrAnhNguoiDung.Image = Image.FromFile(defaultImagePath);
                }
                else
                {
                    // Nếu không tìm thấy, thử các đường dẫn khác
                    var alternativePaths = new[]
                    {
                        Path.Combine(appDirectory, "Resources", "icons8-bench-press.gif"),
                        Path.Combine(appDirectory, "Icons", "icons8-bench-press.gif")
                    };

                    foreach (var path in alternativePaths)
                    {
                        if (File.Exists(path))
                        {
                            ptrAnhNguoiDung.Image = Image.FromFile(path);
                            return;
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Không tìm thấy ảnh mặc định: {defaultImagePath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load ảnh mặc định: {ex.Message}");
            }
        }

        /// <summary>
        /// Public method để reload thông tin user (được gọi từ bên ngoài)
        /// </summary>
        public void ReloadUserInfo()
        {
            try
            {
                // Reload user từ database để có dữ liệu mới nhất
                if (CurrentUser.IsLoggedIn && !string.IsNullOrWhiteSpace(CurrentUser.UserID))
                {
                    using (var context = new WF_HealthTracker())
                    {
                        var updatedUser = context.Users.FirstOrDefault(u => u.UserID == CurrentUser.UserID);
                        if (updatedUser != null)
                        {
                            // Cập nhật CurrentUser với dữ liệu mới nhất
                            CurrentUser.User.HoTen = updatedUser.HoTen;
                            CurrentUser.User.Email = updatedUser.Email;
                            CurrentUser.User.SDT = updatedUser.SDT;
                            CurrentUser.User.AnhDaiDien = updatedUser.AnhDaiDien; // Có thể là null nếu đã xóa
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi reload user info: {ex.Message}");
            }

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
        /// Load một UserControl vào pnlBackground
        /// </summary>
        /// <typeparam name="T">Loại UserControl cần load</typeparam>
        private void LoadUserControlIntoBackground<T>() where T : UserControl, new()
        {
            try
            {
                // Dispose UserControl cũ nếu có
                if (_currentUserControl != null)
                {
                    _userControlContainer.Controls.Remove(_currentUserControl);
                    _currentUserControl.Dispose();
                    _currentUserControl = null;
                }

                // Tạo UserControl mới
                _currentUserControl = new T();
                // Không set Dock = Fill để UserControl giữ kích thước tự nhiên và kích hoạt scroll
                // Không set AutoSize = false để UserControl tự điều chỉnh kích thước
                _currentUserControl.Location = new Point(0, 0);
                _currentUserControl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                // Đảm bảo UserControl có thể hiển thị đầy đủ chiều rộng (nếu container đã có kích thước)
                if (_userControlContainer.ClientSize.Width > 0)
                {
                    _currentUserControl.Width = Math.Max(_currentUserControl.Width, _userControlContainer.ClientSize.Width);
                }

                // Thêm vào container
                _userControlContainer.Controls.Clear();
                _userControlContainer.Controls.Add(_currentUserControl);

                // Ẩn pnlBody và hiển thị container
                pnlBody.Visible = false;
                _userControlContainer.Visible = true;
                _userControlContainer.BringToFront();
                // Đảm bảo thanh tiêu đề vẫn hiển thị
                pnlTieuDe?.BringToFront();

                // KHÓA SCROLL NGANG HOÀN TOÀN
                _userControlContainer.HorizontalScroll.Maximum = 0;
                _userControlContainer.HorizontalScroll.Visible = false;
                _userControlContainer.HorizontalScroll.Enabled = false;
                
                // Đảm bảo scroll dọc hoạt động
                _userControlContainer.AutoScroll = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load UserControl: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hiển thị lại nội dung mặc định (pnlBody) và ẩn UserControl
        /// </summary>
        private void ShowDefaultContent()
        {
            try
            {
                // Ẩn container UserControl
                _userControlContainer.Visible = false;

                // Hiển thị lại pnlBody
                pnlBody.Visible = true;
                pnlBody.BringToFront();
                // Đảm bảo thanh tiêu đề vẫn hiển thị
                pnlTieuDe?.BringToFront();

                // Dispose UserControl cũ nếu có
                if (_currentUserControl != null)
                {
                    _userControlContainer.Controls.Remove(_currentUserControl);
                    _currentUserControl.Dispose();
                    _currentUserControl = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị nội dung mặc định: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        /// Event handler cho button Home - reload trang chủ và hiển thị lại pnlBody
        /// </summary>
        private void BtnHome_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị lại pnlBody và ẩn UserControl
                ShowDefaultContent();
                
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
        /// Event handler cho button Lịch Luyện Tập - load ucKeHoachLuyenTap vào pnlBackground
        /// </summary>
        private void BtnLichLuyenTap_Click(object sender, EventArgs e)
        {
            try
            {
                LoadUserControlIntoBackground<ucKeHoachLuyenTap>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang kế hoạch luyện tập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    var reportForm = new ReportForm(this);
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

                    // Nếu không có pending payments, vẫn cho phép mở form để xem lịch sử thanh toán
                    var firstPayment = pendingPayments.FirstOrDefault();
                    string datLichId = firstPayment?.DatLichID; // có thể null

                    // Ẩn Dashboard và hiển thị form thanh toán PT
                    this.Hide();
                    var frmThanhToan = new frm_ThanhToanPT(this, datLichId);
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
        /// Event handler cho button Mục Tiêu - load ucMucTieu vào pnlBackground
        /// </summary>
        private void BtnMucTieu_Click(object sender, EventArgs e)
        {
            try
            {
                LoadUserControlIntoBackground<ucMucTieu>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang mục tiêu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Lên Kế Hoạch Ăn Uống - load ucKeHoachAnUong vào pnlBackground
        /// </summary>
        private void BtnLenKeHoachAnUong_Click(object sender, EventArgs e)
        {
            try
            {
                LoadUserControlIntoBackground<ucKeHoachAnUong>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang chế độ ăn uống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    // Đóng tất cả các form con đang mở (trừ form chính)
                    CloseAllChildForms();

                    // Ẩn form Dashboard (không đóng vì đây là form chính)
                    this.Hide();

                    // Mở lại form đăng nhập
                    var loginForm = new LoginForm();
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // Nếu đăng nhập thành công, reload lại Dashboard
                        LoadUserInfo();
                        UpdatePTButtonState();
                        this.Show();
                        this.BringToFront();
                        this.Activate();
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
                // Đảm bảo form vẫn hiển thị nếu có lỗi
                this.Show();
            }
        }

        /// <summary>
        /// Đóng tất cả các form con đang mở (trừ form chính)
        /// </summary>
        private void CloseAllChildForms()
        {
            try
            {
                var formsToClose = new List<Form>();
                
                // Lấy danh sách tất cả form đang mở (trừ form chính)
                foreach (Form form in Application.OpenForms)
                {
                    if (form != this && !form.IsDisposed)
                    {
                        formsToClose.Add(form);
                    }
                }

                // Đóng từng form
                foreach (var form in formsToClose)
                {
                    try
                    {
                        form.Close();
                    }
                    catch
                    {
                        // Bỏ qua lỗi khi đóng form
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi đóng các form con: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Dispose UserControl hiện tại nếu có
            if (_currentUserControl != null)
            {
                try
                {
                    _userControlContainer.Controls.Remove(_currentUserControl);
                    _currentUserControl.Dispose();
                    _currentUserControl = null;
                }
                catch
                {
                    // Bỏ qua lỗi khi dispose
                }
            }

            base.OnFormClosing(e);
        }
    }
}
