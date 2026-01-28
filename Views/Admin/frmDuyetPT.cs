extern alias ef6;

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
using HealthApp.Models;
using ef6::System.Data.Entity;

namespace HealthApp.Views.Admin
{
    public partial class frmDuyetPT : Form
    {
        private WF_HealthTracker _dbContext;
        private List<HuanLuyenVien> _pendingPTs;
        private HuanLuyenVien _currentPT;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel _currentPanel;

        public frmDuyetPT()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            _pendingPTs = new List<HuanLuyenVien>();
            InitializeEventHandlers();
            LoadPendingPTs();
        }

        private void InitializeEventHandlers()
        {
            // Event handlers cho buttons
            btnDongY.Click += BtnDongY_Click;
            btnTuChoi.Click += BtnTuChoi_Click;
            
            // Event handlers cho xem ảnh/file
            btnAnhDaiDien.Click += BtnAnhDaiDien_Click;
            btnAnhChanDung.Click += BtnAnhChanDung_Click;
            btnAnhCCCD.Click += BtnAnhCCCD_Click;
            btnXemFile.Click += BtnXemFile_Click;
            
            // Event handlers cho tìm kiếm và filter
            txtTiemKiem.TextChanged += TxtTiemKiem_TextChanged;
            cboChuyenMon.SelectedIndexChanged += CboChuyenMon_SelectedIndexChanged;
            
            // Populate combo box chuyên môn
            cboChuyenMon.Items.Clear();
            cboChuyenMon.Items.Add("Tất cả");
            cboChuyenMon.Items.Add("Cân nặng");
            cboChuyenMon.Items.Add("Tăng cơ");
            cboChuyenMon.Items.Add("Cân nặng & Tăng cơ");
            cboChuyenMon.SelectedIndex = 0;
        }

        /// <summary>
        /// Load danh sách PT chờ duyệt (DaXacMinh = false)
        /// </summary>
        private void LoadPendingPTs()
        {
            try
            {
                // Lấy danh sách PT chờ duyệt
                _pendingPTs = _dbContext.HuanLuyenVien
                    .Include("Users")
                    .Where(pt => pt.DaXacMinh == false || pt.DaXacMinh == null)
                    .OrderByDescending(pt => pt.NgayTao)
                    .ToList();

                DisplayPTList(_pendingPTs);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách PT chờ duyệt: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hiển thị danh sách PT trong panel
        /// </summary>
        private void DisplayPTList(List<HuanLuyenVien> pts)
        {
            try
            {
                // Xóa tất cả panel cũ (trừ panel mẫu)
                var panelsToRemove = guna2CustomGradientPanel1.Controls
                    .OfType<Guna.UI2.WinForms.Guna2CustomGradientPanel>()
                    .Where(p => p.Name != "pnlThongTinPT" && p.Name.StartsWith("pnlPT_"))
                    .ToList();

                foreach (var panel in panelsToRemove)
                {
                    guna2CustomGradientPanel1.Controls.Remove(panel);
                    panel.Dispose();
                }

                // Ẩn panel mẫu
                pnlThongTinPT.Visible = false;

                if (!pts.Any())
                {
                    return;
                }

                // Kích thước và khoảng cách
                const int panelWidth = 328;
                const int panelHeight = 528;
                const int marginX = 23;
                const int marginY = 20;
                const int startY = 35;
                const int columnsPerRow = 2;

                // Tạo panel cho mỗi PT
                for (int i = 0; i < pts.Count; i++)
                {
                    var pt = pts[i];
                    int row = i / columnsPerRow;
                    int col = i % columnsPerRow;

                    // Tính vị trí
                    int x = marginX + col * (panelWidth + marginX);
                    int y = startY + row * (panelHeight + marginY);

                    // Tạo panel mới
                    var ptPanel = CreatePTPanel(pt, i);
                    ptPanel.Location = new Point(x, y);
                    ptPanel.Name = $"pnlPT_{pt.PTID}";
                    ptPanel.Visible = true;

                    guna2CustomGradientPanel1.Controls.Add(ptPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tạo panel hiển thị thông tin PT
        /// </summary>
        private Guna.UI2.WinForms.Guna2CustomGradientPanel CreatePTPanel(HuanLuyenVien pt, int index)
        {
            // Clone panel mẫu
            var panel = ClonePanel(pnlThongTinPT);
            panel.Name = $"pnlPT_{pt.PTID}";
            panel.Tag = pt;

            // Lấy thông tin User
            var user = pt.Users;
            if (user == null)
            {
                user = _dbContext.Users.FirstOrDefault(u => u.UserID == pt.UserID);
            }

            // Parse thông tin CCCD từ TieuSu (JSON)
            string soCCCD = "";
            string noiCap = "";
            DateTime? ngayCap = null;
            if (!string.IsNullOrEmpty(pt.TieuSu) && pt.TieuSu.StartsWith("{"))
            {
                try
                {
                    // Parse JSON đơn giản
                    var json = pt.TieuSu;
                    var soCCCDMatch = System.Text.RegularExpressions.Regex.Match(json, @"""SoCCCD"":""([^""]+)""");
                    var noiCapMatch = System.Text.RegularExpressions.Regex.Match(json, @"""NoiCap"":""([^""]+)""");
                    var ngayCapMatch = System.Text.RegularExpressions.Regex.Match(json, @"""NgayCap"":""([^""]+)""");
                    
                    if (soCCCDMatch.Success) soCCCD = soCCCDMatch.Groups[1].Value;
                    if (noiCapMatch.Success) noiCap = noiCapMatch.Groups[1].Value;
                    if (ngayCapMatch.Success && DateTime.TryParse(ngayCapMatch.Groups[1].Value, out DateTime ngayCapValue))
                        ngayCap = ngayCapValue;
                }
                catch { }
            }

            // Populate dữ liệu vào các control
            SetControlText(panel, "lblHovaTen", user?.HoTen ?? user?.Username ?? "N/A");
            SetControlText(panel, "lblMaPT", pt.PTID ?? "N/A");
            SetControlText(panel, "lblGmail", user?.Email ?? "N/A");
            SetControlText(panel, "lblSoDienThoai", user?.SDT ?? "N/A");
            SetControlText(panel, "lblDiaChi", pt.ThanhPho ?? "N/A");
            SetControlText(panel, "lblGiaThue", pt.GiaTheoGio.HasValue 
                ? $"{pt.GiaTheoGio.Value:N0}/giờ" 
                : "Chưa có");
            SetControlText(panel, "lblNgayDangKy", pt.NgayTao.HasValue 
                ? $"Ngày đăng ký: {pt.NgayTao.Value:dd/MM/yyyy}" 
                : "Ngày đăng ký: N/A");
            SetControlText(panel, "lblNamKinhNghiem", pt.SoNamKinhNghiem.HasValue 
                ? $"Kinh nghiệm: {pt.SoNamKinhNghiem.Value} năm" 
                : "Kinh nghiệm: Chưa có");
            SetControlText(panel, "lblChuyenMonPT", $"Chuyên môn: {pt.ChuyenMon ?? "Chưa có"}");
            SetControlText(panel, "lblChungChi", $"Chứng chỉ: {pt.ChungChi ?? "Chưa có"}");

            // Load ảnh đại diện
            var ptrAnhDaiDien = FindControl<Guna.UI2.WinForms.Guna2CirclePictureBox>(panel, "ptrAnhDaiDien");
            if (ptrAnhDaiDien != null)
            {
                try
                {
                    string imagePath = pt.AnhDaiDien ?? user?.AnhDaiDien;
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        string fullPath = imagePath;
                        if (!Path.IsPathRooted(imagePath))
                        {
                            fullPath = Path.Combine(Application.StartupPath, "Resources", imagePath);
                        }
                        if (File.Exists(fullPath))
                        {
                            ptrAnhDaiDien.Image = Image.FromFile(fullPath);
                        }
                    }
                }
                catch { }
            }

            // Gắn event handlers cho buttons
            var btnDongY = FindControl<Guna.UI2.WinForms.Guna2GradientButton>(panel, "btnDongY");
            if (btnDongY != null)
            {
                btnDongY.Tag = pt;
                btnDongY.Click += BtnDongY_Click;
            }

            var btnTuChoi = FindControl<Guna.UI2.WinForms.Guna2Button>(panel, "btnTuChoi");
            if (btnTuChoi != null)
            {
                btnTuChoi.Tag = pt;
                btnTuChoi.Click += BtnTuChoi_Click;
            }

            // Gắn event handlers cho xem ảnh/file
            var btnAnhDaiDien = FindControl<Guna.UI2.WinForms.Guna2GradientButton>(panel, "btnAnhDaiDien");
            if (btnAnhDaiDien != null)
            {
                btnAnhDaiDien.Tag = new ImageViewInfo { PT = pt, Type = "AnhDaiDien" };
                btnAnhDaiDien.Click += BtnAnhDaiDien_Click;
            }

            var btnAnhChanDung = FindControl<Guna.UI2.WinForms.Guna2GradientButton>(panel, "btnAnhChanDung");
            if (btnAnhChanDung != null)
            {
                btnAnhChanDung.Tag = new ImageViewInfo { PT = pt, Type = "AnhChanDung" };
                btnAnhChanDung.Click += BtnAnhChanDung_Click;
            }

            var btnAnhCCCD = FindControl<Guna.UI2.WinForms.Guna2GradientButton>(panel, "btnAnhCCCD");
            if (btnAnhCCCD != null)
            {
                btnAnhCCCD.Tag = new ImageViewInfo { PT = pt, Type = "AnhCCCD" };
                btnAnhCCCD.Click += BtnAnhCCCD_Click;
            }

            var btnXemFile = FindControl<Guna.UI2.WinForms.Guna2GradientButton>(panel, "btnXemFile");
            if (btnXemFile != null)
            {
                btnXemFile.Tag = new ImageViewInfo { PT = pt, Type = "FileTaiLieu" };
                btnXemFile.Click += BtnXemFile_Click;
            }

            return panel;
        }

        /// <summary>
        /// Clone panel và tất cả controls bên trong
        /// </summary>
        private Guna.UI2.WinForms.Guna2CustomGradientPanel ClonePanel(Guna.UI2.WinForms.Guna2CustomGradientPanel sourcePanel)
        {
            var newPanel = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            
            // Copy properties
            newPanel.Size = sourcePanel.Size;
            newPanel.BackColor = sourcePanel.BackColor;
            newPanel.BorderRadius = sourcePanel.BorderRadius;
            newPanel.BorderThickness = sourcePanel.BorderThickness;
            newPanel.BorderColor = sourcePanel.BorderColor;
            newPanel.FillColor = sourcePanel.FillColor;
            newPanel.FillColor2 = sourcePanel.FillColor2;
            newPanel.FillColor3 = sourcePanel.FillColor3;
            newPanel.FillColor4 = sourcePanel.FillColor4;

            // Clone tất cả controls
            foreach (Control control in sourcePanel.Controls)
            {
                Control clonedControl = CloneControl(control);
                newPanel.Controls.Add(clonedControl);
            }

            return newPanel;
        }

        /// <summary>
        /// Clone một control
        /// </summary>
        private Control CloneControl(Control source)
        {
            Control cloned = null;

            if (source is Guna.UI2.WinForms.Guna2HtmlLabel)
            {
                var sourceLabel = source as Guna.UI2.WinForms.Guna2HtmlLabel;
                cloned = new Guna.UI2.WinForms.Guna2HtmlLabel
                {
                    Name = sourceLabel.Name,
                    Text = sourceLabel.Text,
                    Location = sourceLabel.Location,
                    Size = sourceLabel.Size,
                    Font = sourceLabel.Font,
                    ForeColor = sourceLabel.ForeColor,
                    BackColor = sourceLabel.BackColor
                };
            }
            else if (source is Guna.UI2.WinForms.Guna2CirclePictureBox)
            {
                var sourcePic = source as Guna.UI2.WinForms.Guna2CirclePictureBox;
                cloned = new Guna.UI2.WinForms.Guna2CirclePictureBox
                {
                    Name = sourcePic.Name,
                    Location = sourcePic.Location,
                    Size = sourcePic.Size,
                    SizeMode = sourcePic.SizeMode,
                    Image = sourcePic.Image
                };
            }
            else if (source is Guna.UI2.WinForms.Guna2GradientButton)
            {
                var sourceBtn = source as Guna.UI2.WinForms.Guna2GradientButton;
                cloned = new Guna.UI2.WinForms.Guna2GradientButton
                {
                    Name = sourceBtn.Name,
                    Text = sourceBtn.Text,
                    Location = sourceBtn.Location,
                    Size = sourceBtn.Size,
                    BorderRadius = sourceBtn.BorderRadius,
                    FillColor = sourceBtn.FillColor,
                    FillColor2 = sourceBtn.FillColor2,
                    ForeColor = sourceBtn.ForeColor,
                    Font = sourceBtn.Font
                };
            }
            else if (source is Guna.UI2.WinForms.Guna2Button)
            {
                var sourceBtn = source as Guna.UI2.WinForms.Guna2Button;
                cloned = new Guna.UI2.WinForms.Guna2Button
                {
                    Name = sourceBtn.Name,
                    Location = sourceBtn.Location,
                    Size = sourceBtn.Size,
                    BorderRadius = sourceBtn.BorderRadius,
                    FillColor = sourceBtn.FillColor,
                    ForeColor = sourceBtn.ForeColor,
                    Image = sourceBtn.Image,
                    ImageSize = sourceBtn.ImageSize
                };
            }
            else if (source is Guna.UI2.WinForms.Guna2CustomGradientPanel)
            {
                var sourcePanel = source as Guna.UI2.WinForms.Guna2CustomGradientPanel;
                cloned = new Guna.UI2.WinForms.Guna2CustomGradientPanel
                {
                    Name = sourcePanel.Name,
                    Location = sourcePanel.Location,
                    Size = sourcePanel.Size,
                    BorderRadius = sourcePanel.BorderRadius,
                    BorderThickness = sourcePanel.BorderThickness,
                    FillColor = sourcePanel.FillColor,
                    FillColor2 = sourcePanel.FillColor2,
                    FillColor3 = sourcePanel.FillColor3,
                    FillColor4 = sourcePanel.FillColor4
                };

                // Clone controls bên trong panel
                foreach (Control child in sourcePanel.Controls)
                {
                    cloned.Controls.Add(CloneControl(child));
                }
            }

            return cloned ?? new Control { Name = source.Name, Location = source.Location, Size = source.Size };
        }

        /// <summary>
        /// Tìm control theo tên trong panel
        /// </summary>
        private T FindControl<T>(Control parent, string name) where T : Control
        {
            foreach (Control control in parent.Controls)
            {
                if (control.Name == name && control is T)
                {
                    return control as T;
                }

                // Tìm đệ quy trong các control con
                var found = FindControl<T>(control, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        /// <summary>
        /// Set text cho control theo tên
        /// </summary>
        private void SetControlText(Control parent, string controlName, string text)
        {
            var control = FindControl<Control>(parent, controlName);
            if (control != null)
            {
                control.Text = text;
            }
        }

        /// <summary>
        /// Event handler cho nút Đồng ý (Duyệt PT)
        /// </summary>
        private void BtnDongY_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2GradientButton;
            if (button?.Tag != null)
            {
                var pt = button.Tag as HuanLuyenVien;
                if (pt != null)
                {
                    DuyetPT(pt);
                }
            }
        }

        /// <summary>
        /// Event handler cho nút Từ chối
        /// </summary>
        private void BtnTuChoi_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2Button;
            if (button?.Tag != null)
            {
                var pt = button.Tag as HuanLuyenVien;
                if (pt != null)
                {
                    TuChoiPT(pt);
                }
            }
        }

        /// <summary>
        /// Duyệt PT - set DaXacMinh = true và cập nhật Role = "PT"
        /// </summary>
        private void DuyetPT(HuanLuyenVien pt)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn duyệt đơn đăng ký của PT {pt.PTID}?", 
                    "Xác nhận duyệt", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Set DaXacMinh = true
                    pt.DaXacMinh = true;
                    pt.NgayCapNhat = DateTime.Now;

                    // Cập nhật Role của user thành "PT"
                    var user = _dbContext.Users.FirstOrDefault(u => u.UserID == pt.UserID);
                    if (user != null)
                    {
                        user.Role = "PT";
                    }

                    // Lưu thay đổi
                    _dbContext.SaveChanges();

                    MessageBox.Show($"Đã duyệt đơn đăng ký của PT {pt.PTID} thành công!", "Thành công", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload danh sách
                    LoadPendingPTs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi duyệt PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Từ chối PT - xóa bản ghi HuanLuyenVien
        /// </summary>
        private void TuChoiPT(HuanLuyenVien pt)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn từ chối đơn đăng ký của PT {pt.PTID}?\n\nLưu ý: Hành động này sẽ xóa đơn đăng ký và không thể hoàn tác.", 
                    "Xác nhận từ chối", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Xóa bản ghi HuanLuyenVien
                    _dbContext.HuanLuyenVien.Remove(pt);
                    _dbContext.SaveChanges();

                    MessageBox.Show($"Đã từ chối đơn đăng ký của PT {pt.PTID}!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload danh sách
                    LoadPendingPTs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi từ chối PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Class để lưu thông tin xem ảnh/file
        /// </summary>
        private class ImageViewInfo
        {
            public HuanLuyenVien PT { get; set; }
            public string Type { get; set; }
        }

        /// <summary>
        /// Event handler cho xem ảnh đại diện
        /// </summary>
        private void BtnAnhDaiDien_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2GradientButton;
            if (button?.Tag != null && button.Tag is ImageViewInfo info)
            {
                string imagePath = info.PT?.AnhDaiDien;
                ShowImage(imagePath, "Ảnh đại diện");
            }
        }

        /// <summary>
        /// Event handler cho xem ảnh chân dung
        /// </summary>
        private void BtnAnhChanDung_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2GradientButton;
            if (button?.Tag != null && button.Tag is ImageViewInfo info)
            {
                string imagePath = info.PT?.AnhChanDung;
                ShowImage(imagePath, "Ảnh chân dung");
            }
        }

        /// <summary>
        /// Event handler cho xem ảnh CCCD
        /// </summary>
        private void BtnAnhCCCD_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2GradientButton;
            if (button?.Tag != null && button.Tag is ImageViewInfo info)
            {
                string imagePath = info.PT?.AnhCCCD;
                ShowImage(imagePath, "Ảnh CCCD");
            }
        }

        /// <summary>
        /// Event handler cho xem file tài liệu
        /// </summary>
        private void BtnXemFile_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2GradientButton;
            if (button?.Tag != null && button.Tag is ImageViewInfo info)
            {
                string filePath = info.PT?.FileTaiLieu;
                
                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Không có file tài liệu!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    string fullPath = filePath;
                    if (!Path.IsPathRooted(filePath))
                    {
                        fullPath = Path.Combine(Application.StartupPath, "Resources", filePath);
                    }

                    if (File.Exists(fullPath))
                    {
                        System.Diagnostics.Process.Start(fullPath);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy file!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Hiển thị ảnh trong form mới
        /// </summary>
        private void ShowImage(string imagePath, string title)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("Không có ảnh!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string fullPath = imagePath;
                if (!Path.IsPathRooted(imagePath))
                {
                    fullPath = Path.Combine(Application.StartupPath, "Resources", imagePath);
                }

                if (File.Exists(fullPath))
                {
                    var form = new Form
                    {
                        Text = title,
                        Size = new Size(800, 600),
                        StartPosition = FormStartPosition.CenterParent
                    };

                    var pictureBox = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(fullPath)
                    };

                    form.Controls.Add(pictureBox);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy ảnh!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị ảnh: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho tìm kiếm
        /// </summary>
        private void TxtTiemKiem_TextChanged(object sender, EventArgs e)
        {
            FilterPTList();
        }

        /// <summary>
        /// Event handler cho filter chuyên môn
        /// </summary>
        private void CboChuyenMon_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterPTList();
        }

        /// <summary>
        /// Filter danh sách PT
        /// </summary>
        private void FilterPTList()
        {
            try
            {
                var filtered = _pendingPTs.AsQueryable();

                // Filter theo tìm kiếm
                string searchText = txtTiemKiem.Text?.Trim();
                if (!string.IsNullOrWhiteSpace(searchText) && searchText != "Tìm kiếm...")
                {
                    string searchLower = searchText.ToLower();
                    filtered = filtered.Where(pt => 
                        (pt.Users != null && 
                         ((pt.Users.HoTen != null && pt.Users.HoTen.ToLower().Contains(searchLower)) ||
                          (pt.Users.Username != null && pt.Users.Username.ToLower().Contains(searchLower)) ||
                          (pt.Users.Email != null && pt.Users.Email.ToLower().Contains(searchLower)))) ||
                        (pt.PTID != null && pt.PTID.ToLower().Contains(searchLower)));
                }

                // Filter theo chuyên môn
                if (cboChuyenMon.SelectedIndex > 0)
                {
                    string chuyenMon = cboChuyenMon.SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(chuyenMon))
                    {
                        filtered = filtered.Where(pt => pt.ChuyenMon != null && pt.ChuyenMon.Contains(chuyenMon));
                    }
                }

                DisplayPTList(filtered.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc danh sách: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pnlNoiDung_Paint(object sender, PaintEventArgs e)
        {
            // Event handler cho Paint event
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dbContext?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
