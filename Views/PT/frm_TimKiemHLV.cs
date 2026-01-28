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
using HealthApp.Services;
using HealthApp.Services.Interfaces;
using HealthApp.Common.Helpers;
using Guna.UI2.WinForms;
using HealthApp.Views.PT;

namespace HealthApp.Views.PT
{
    public partial class frm_TimKiemHLV : Form
    {
        private readonly IPTSearchService _ptSearchService;
        private readonly WF_HealthTracker _context;
        private readonly HealthApp.Views.Dashboard.frmDashBoard1 _parentDashboard;
        private string _selectedPTID;
        private List<PTSearchViewModel> _currentPTList;

        public frm_TimKiemHLV(HealthApp.Views.Dashboard.frmDashBoard1 parentDashboard = null)
        {
            InitializeComponent();
            _context = new WF_HealthTracker();
            _ptSearchService = new PTSearchService(_context);
            _parentDashboard = parentDashboard;
            _currentPTList = new List<PTSearchViewModel>();
            InitializeEventHandlers();
            InitializeSearchBox(); // Khởi tạo ô tìm kiếm
            ClearPTDetail(); // Ẩn/clear thông tin chi tiết khi khởi tạo
            _ = LoadPTListAsync(); // Fire and forget
        }

        /// <summary>
        /// Khởi tạo ô tìm kiếm với placeholder text
        /// </summary>
        private void InitializeSearchBox()
        {
            if (txtTimKiem.Text == "Tìm kiếm huấn luyện viên")
            {
                txtTimKiem.ForeColor = Color.Gray;
            }
        }

        private void InitializeEventHandlers()
        {
            btnBack.Click += BtnBack_Click;
            btnTatCa.Click += BtnTatCa_Click;
            btnTinhvaTP.Click += BtnTinhvaTP_Click;
            btnChuyenMon.Click += BtnChuyenMon_Click;
            txtTimKiem.KeyDown += TxtTimKiem_KeyDown;
            txtTimKiem.GotFocus += TxtTimKiem_GotFocus;
            txtTimKiem.Leave += TxtTimKiem_Leave;
            btnGuiYeuCau.Click += BtnGuiYeuCau_Click;
        }

        private async Task LoadPTListAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var pts = await _ptSearchService.GetAllPTsAsync();
                pts = FilterOutCurrentPT(pts);
                _currentPTList = pts;
                DisplayPTList(pts);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load danh sách PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void DisplayPTList(List<PTSearchViewModel> pts)
        {
            try
            {
                // Xóa các panel cũ (trừ panel mẫu)
                var panelsToRemove = pnlHienThiDanhSach.Controls
                    .OfType<Guna2ShadowPanel>()
                    .Where(p => p.Name != "pnlDanhSach1")
                    .ToList();

                foreach (var panel in panelsToRemove)
                {
                    pnlHienThiDanhSach.Controls.Remove(panel);
                    panel.Dispose();
                }

                // Ẩn panel mẫu
                if (pnlDanhSach1 != null)
                {
                    pnlDanhSach1.Visible = false;
                }

                if (pts.Count == 0)
                {
                    return;
                }

                int yOffset = 7;
                int panelHeight = 121;
                int spacing = 10;

                for (int i = 0; i < pts.Count; i++)
                {
                    var pt = pts[i];

                    // Tạo panel mới
                    var panel = CreatePTPanel(pt, yOffset);
                    pnlHienThiDanhSach.Controls.Add(panel);

                    yOffset += panelHeight + spacing;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Guna2ShadowPanel CreatePTPanel(PTSearchViewModel pt, int yPos)
        {
            var panel = new Guna2ShadowPanel
            {
                BackColor = Color.Transparent,
                FillColor = Color.Honeydew,
                Location = new Point(11, yPos),
                Name = $"pnlPT_{pt.PTID}",
                Radius = 10,
                ShadowColor = Color.Honeydew,
                ShadowShift = 1,
                Size = new Size(396, 121),
                Tag = pt.PTID
            };

            // Click event để hiển thị chi tiết
            panel.Click += (s, e) => PTPanel_Click(pt.PTID);
            panel.Cursor = Cursors.Hand;

            // Avatar
            var avatar = new Guna2PictureBox
            {
                BorderRadius = 15,
                ImageRotate = 0F,
                Location = new Point(19, 13),
                Name = $"ptrAvatar_{pt.PTID}",
                Size = new Size(90, 73),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = pt.PTID
            };
            LoadAvatar(avatar, pt.AnhDaiDien);
            avatar.Click += (s, e) => PTPanel_Click(pt.PTID);
            avatar.Cursor = Cursors.Hand;
            panel.Controls.Add(avatar);

            // Tên PT
            var lblTen = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 12F, FontStyle.Bold),
                Location = new Point(130, 13),
                Name = $"lblTen_{pt.PTID}",
                Size = new Size(146, 23),
                Text = pt.Ten,
                Tag = pt.PTID
            };
            lblTen.Click += (s, e) => PTPanel_Click(pt.PTID);
            lblTen.Cursor = Cursors.Hand;
            panel.Controls.Add(lblTen);

            // Chuyên môn
            var lblChuyenMon = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 7.8F, FontStyle.Bold),
                ForeColor = Color.Silver,
                Location = new Point(131, 36),
                Name = $"lblChuyenMon_{pt.PTID}",
                Size = new Size(122, 15),
                Text = pt.ChuyenMon ?? "Chưa có chuyên môn",
                Tag = pt.PTID
            };
            lblChuyenMon.Click += (s, e) => PTPanel_Click(pt.PTID);
            lblChuyenMon.Cursor = Cursors.Hand;
            panel.Controls.Add(lblChuyenMon);

            // Icon sao
            var ptrStar = new Guna2PictureBox
            {
                ImageRotate = 0F,
                Location = new Point(130, 56),
                Name = $"ptrStar_{pt.PTID}",
                Size = new Size(22, 19),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = pt.PTID
            };
            // Copy image từ ptrstar nếu có
            if (ptrstar != null && ptrstar.Image != null)
            {
                ptrStar.Image = (Image)ptrstar.Image.Clone();
            }
            ptrStar.Click += (s, e) => PTPanel_Click(pt.PTID);
            ptrStar.Cursor = Cursors.Hand;
            panel.Controls.Add(ptrStar);

            // Đánh giá
            var lblDanhGia = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 7.8F, FontStyle.Bold),
                Location = new Point(155, 60),
                Name = $"lblDanhGia_{pt.PTID}",
                Size = new Size(14, 15),
                Text = pt.DiemTrungBinh > 0 ? pt.DiemTrungBinh.Value.ToString("F1") : "0",
                Tag = pt.PTID
            };
            lblDanhGia.Click += (s, e) => PTPanel_Click(pt.PTID);
            lblDanhGia.Cursor = Cursors.Hand;
            panel.Controls.Add(lblDanhGia);

            // Tính toán vị trí của lblSoDanhGia dựa trên kích thước thực tế của lblDanhGia
            // Đảm bảo có khoảng cách hợp lý (ít nhất 5 pixels)
            var danhGiaTextWidth = TextRenderer.MeasureText(lblDanhGia.Text, lblDanhGia.Font).Width;
            var soDanhGiaX = lblDanhGia.Location.X + danhGiaTextWidth + 5; // Khoảng cách 5 pixels

            // Số đánh giá
            var lblSoDanhGia = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 7.2F, FontStyle.Regular),
                ForeColor = Color.Silver,
                Location = new Point(soDanhGiaX, 60),
                Name = $"lblSoDanhGia_{pt.PTID}",
                Size = new Size(27, 15),
                Text = $"({pt.TongDanhGia})",
                Tag = pt.PTID
            };
            lblSoDanhGia.Click += (s, e) => PTPanel_Click(pt.PTID);
            lblSoDanhGia.Cursor = Cursors.Hand;
            panel.Controls.Add(lblSoDanhGia);

            // Dấu gạch ngang
            var lblNoi = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 12F, FontStyle.Bold),
                ForeColor = Color.Silver,
                Location = new Point(202, 54),
                Name = $"lblNoi_{pt.PTID}",
                Size = new Size(17, 23),
                Text = "-",
                Tag = pt.PTID
            };
            lblNoi.Click += (s, e) => PTPanel_Click(pt.PTID);
            lblNoi.Cursor = Cursors.Hand;
            panel.Controls.Add(lblNoi);

            // Số năm kinh nghiệm
            var lblSoNamKinhNghiem = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 7.8F, FontStyle.Bold),
                ForeColor = Color.Silver,
                Location = new Point(222, 60),
                Name = $"lblSoNamKinhNghiem_{pt.PTID}",
                Size = new Size(14, 15),
                Text = pt.SoNamKinhNghiem?.ToString() ?? "0",
                Tag = pt.PTID
            };
            lblSoNamKinhNghiem.Click += (s, e) => PTPanel_Click(pt.PTID);
            lblSoNamKinhNghiem.Cursor = Cursors.Hand;
            panel.Controls.Add(lblSoNamKinhNghiem);

            // Năm kinh nghiệm
            var lblNamKinhNghiem = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 7.8F, FontStyle.Bold),
                ForeColor = Color.Silver,
                Location = new Point(239, 60),
                Name = $"lblNamKinhNghiem_{pt.PTID}",
                Size = new Size(103, 15),
                Text = "năm kinh nghiệm",
                Tag = pt.PTID
            };
            lblNamKinhNghiem.Click += (s, e) => PTPanel_Click(pt.PTID);
            lblNamKinhNghiem.Cursor = Cursors.Hand;
            panel.Controls.Add(lblNamKinhNghiem);

            // Icon địa điểm
            var ptrDiaDiem = new Guna2PictureBox
            {
                ImageRotate = 0F,
                Location = new Point(130, 87),
                Name = $"ptrDiaDiem_{pt.PTID}",
                Size = new Size(22, 19),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = pt.PTID
            };
            // Copy image từ guna2PictureBox2 nếu có
            if (guna2PictureBox2 != null && guna2PictureBox2.Image != null)
            {
                ptrDiaDiem.Image = (Image)guna2PictureBox2.Image.Clone();
            }
            ptrDiaDiem.Click += (s, e) => PTPanel_Click(pt.PTID);
            ptrDiaDiem.Cursor = Cursors.Hand;
            panel.Controls.Add(ptrDiaDiem);

            // Địa điểm
            var lblDiaDiem = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 7.8F, FontStyle.Bold),
                ForeColor = Color.Silver,
                Location = new Point(155, 91),
                Name = $"lblDiaDiem_{pt.PTID}",
                Size = new Size(200, 15),
                Text = pt.ThanhPho ?? "Chưa có",
                Tag = pt.PTID
            };
            lblDiaDiem.Click += (s, e) => PTPanel_Click(pt.PTID);
            lblDiaDiem.Cursor = Cursors.Hand;
            panel.Controls.Add(lblDiaDiem);

            return panel;
        }

        private async void PTPanel_Click(string ptId)
        {
            try
            {
                _selectedPTID = ptId;
                await LoadPTDetail(ptId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load chi tiết PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPTDetail(string ptId)
        {
            try
            {
                var detail = await _ptSearchService.GetPTDetailAsync(ptId);
                if (detail == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin PT!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị các panel chi tiết
                pnlThongTinPT.Visible = true;
                pnlChiTiet.Visible = true;

                // Hiển thị thông tin trong pnlThongTinPT
                lblTen1.Text = detail.Ten;
                lblChuyenMon1.Text = detail.ChuyenMon ?? "Chưa có chuyên môn";
                lblDanhGia1.Text = detail.DiemTrungBinh > 0 ? detail.DiemTrungBinh.Value.ToString("F1") : "0";
                lblSoDanhGia1.Text = $"({detail.TongDanhGia})";
                lblSoNamKinhNghiem1.Text = detail.SoNamKinhNghiem?.ToString() ?? "0";
                lblGiaTheoGio.Text = detail.GiaTheoGio > 0 ? $"{detail.GiaTheoGio.Value:N0}/giờ" : "Chưa có";
                LoadAvatar(ptrAvatar, detail.AnhChanDung ?? detail.AnhDaiDien);

                // Hiển thị thống kê
                lblTiLe.Text = detail.TiLeThanhCong > 0 ? detail.TiLeThanhCong.Value.ToString("F0") : "0";
                lblSoHocVien.Text = $"{detail.SoKhachHienTai}+";
                lblSoChungChi.Text = detail.DanhSachChungChi?.Count.ToString() ?? "0";
                lblSoChuyenMon.Text = detail.DanhSachChuyenMon?.Count.ToString() ?? "0";

                // Hiển thị giới thiệu
                lblNoiDungGioiThieu.Text = !string.IsNullOrEmpty(detail.TieuSu) 
                    ? detail.TieuSu 
                    : "Chưa có thông tin giới thiệu.";

                // Hiển thị chuyên môn
                if (detail.DanhSachChuyenMon != null && detail.DanhSachChuyenMon.Count > 0)
                {
                    lblChiTietChuyenMon1.Text = string.Join(", ", detail.DanhSachChuyenMon);
                }
                else
                {
                    lblChiTietChuyenMon1.Text = "Chưa có chuyên môn";
                }

                // Hiển thị chứng chỉ - tạo các panel động cho từng chứng chỉ
                DisplayCertificates(detail.DanhSachChungChi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load chi tiết PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAvatar(Guna2PictureBox pictureBox, string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    pictureBox.Image = null;
                    return;
                }

                string fullPath = imagePath;

                // Nếu là đường dẫn relative (bắt đầu với PTDocuments hoặc Resources)
                if (!Path.IsPathRooted(imagePath))
                {
                    var appDirectory = Application.StartupPath;
                    fullPath = Path.Combine(appDirectory, "Resources", imagePath);
                }

                if (File.Exists(fullPath))
                {
                    pictureBox.Image = Image.FromFile(fullPath);
                }
                else
                {
                    // Thử đường dẫn trực tiếp nếu không tìm thấy
                    if (File.Exists(imagePath))
                    {
                        pictureBox.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        pictureBox.Image = null;
                    }
                }
            }
            catch
            {
                pictureBox.Image = null;
            }
        }

        /// <summary>
        /// Hiển thị danh sách chứng chỉ - tạo các panel động
        /// </summary>
        private void DisplayCertificates(List<string> danhSachChungChi)
        {
            try
            {
                // Xóa các panel chứng chỉ cũ (trừ panel mẫu)
                var panelsToRemove = pnlChiTiet.Controls
                    .OfType<Guna2ShadowPanel>()
                    .Where(p => p.Name != "pnlChiTietChungChi" && p.Name.StartsWith("pnlChungChi_"))
                    .ToList();

                foreach (var panel in panelsToRemove)
                {
                    pnlChiTiet.Controls.Remove(panel);
                    panel.Dispose();
                }

                // Ẩn panel mẫu
                pnlChiTietChungChi.Visible = false;

                if (danhSachChungChi == null || danhSachChungChi.Count == 0)
                {
                    // Hiển thị panel mẫu với text "Chưa có chứng chỉ"
                    pnlChiTietChungChi.Visible = true;
                    lblChiTietChungChi1.Text = "Chưa có chứng chỉ";
                    return;
                }

                // Tính vị trí Y bắt đầu (sau label "Chứng chỉ" và một khoảng cách)
                int startY = 156; // Vị trí Y của panel mẫu
                int panelHeight = 82;
                int spacing = 10; // Khoảng cách giữa các panel

                // Tạo panel cho từng chứng chỉ
                for (int i = 0; i < danhSachChungChi.Count; i++)
                {
                    var chungChi = danhSachChungChi[i];
                    var panel = CreateCertificatePanel(chungChi, startY + i * (panelHeight + spacing));
                    pnlChiTiet.Controls.Add(panel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị chứng chỉ: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tạo panel chứng chỉ động
        /// </summary>
        private Guna2ShadowPanel CreateCertificatePanel(string chungChiText, int yPos)
        {
            var panel = new Guna2ShadowPanel
            {
                BackColor = Color.Transparent,
                FillColor = Color.FromArgb(255, 224, 192), // Màu cam nhạt giống panel mẫu
                Location = new Point(25, yPos),
                Name = $"pnlChungChi_{Guid.NewGuid()}",
                Radius = 10,
                ShadowColor = Color.Silver,
                ShadowShift = 1,
                Size = new Size(722, 82)
            };

            // Icon chứng chỉ
            var ptrIcon = new Guna2PictureBox
            {
                BackColor = Color.White,
                Image = ptrChiTietChungChi.Image != null ? (Image)ptrChiTietChungChi.Image.Clone() : null,
                ImageRotate = 0F,
                Location = new Point(14, 17),
                Name = $"ptrChungChi_{Guid.NewGuid()}",
                Padding = new Padding(5),
                Size = new Size(51, 45),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            panel.Controls.Add(ptrIcon);

            // Label tên chứng chỉ
            var lblTen = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 10.2F, FontStyle.Bold),
                Location = new Point(71, 28),
                Name = $"lblChungChi_{Guid.NewGuid()}",
                Size = new Size(600, 19),
                Text = chungChiText
            };
            panel.Controls.Add(lblTen);

            return panel;
        }

        /// <summary>
        /// Clear/Ẩn thông tin chi tiết PT
        /// </summary>
        private void ClearPTDetail()
        {
            // Ẩn các panel chi tiết
            pnlThongTinPT.Visible = false;
            pnlChiTiet.Visible = false;
            
            // Xóa các panel chứng chỉ động
            var panelsToRemove = pnlChiTiet.Controls
                .OfType<Guna2ShadowPanel>()
                .Where(p => p.Name != "pnlChiTietChungChi" && p.Name.StartsWith("pnlChungChi_"))
                .ToList();

            foreach (var panel in panelsToRemove)
            {
                pnlChiTiet.Controls.Remove(panel);
                panel.Dispose();
            }

            // Hiển thị lại panel mẫu
            pnlChiTietChungChi.Visible = true;
            
            // Clear dữ liệu
            lblTen1.Text = "";
            lblChuyenMon1.Text = "";
            lblDanhGia1.Text = "0";
            lblSoDanhGia1.Text = "(0)";
            lblSoNamKinhNghiem1.Text = "0";
            lblGiaTheoGio.Text = "";
            ptrAvatar.Image = null;
            lblTiLe.Text = "0";
            lblSoHocVien.Text = "0+";
            lblSoChungChi.Text = "0";
            lblSoChuyenMon.Text = "0";
            lblNoiDungGioiThieu.Text = "";
            lblChiTietChuyenMon1.Text = "";
            lblChiTietChungChi1.Text = "";
            _selectedPTID = null;
        }

        /// <summary>
        /// Xử lý khi nhấn Enter trong ô tìm kiếm
        /// </summary>
        private async void TxtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Ngăn tiếng beep
                await PerformSearch();
            }
        }

        /// <summary>
        /// Xử lý khi click vào ô tìm kiếm - clear placeholder text
        /// </summary>
        private void TxtTimKiem_GotFocus(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Tìm kiếm huấn luyện viên")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Xử lý khi rời khỏi ô tìm kiếm - hiển thị lại placeholder nếu trống
        /// </summary>
        private void TxtTimKiem_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                txtTimKiem.Text = "Tìm kiếm huấn luyện viên";
                txtTimKiem.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// Thực hiện tìm kiếm
        /// </summary>
        private async Task PerformSearch()
        {
            try
            {
                var searchText = txtTimKiem.Text.Trim();
                
                // Nếu là placeholder text thì không tìm
                if (searchText == "Tìm kiếm huấn luyện viên" || string.IsNullOrEmpty(searchText))
                {
                    await LoadPTListAsync();
                    return;
                }

                var pts = await _ptSearchService.SearchPTsByNameAsync(searchText);
                pts = FilterOutCurrentPT(pts);
                _currentPTList = pts;
                DisplayPTList(pts);
                ClearPTDetail(); // Clear chi tiết khi tìm kiếm
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnTatCa_Click(object sender, EventArgs e)
        {
            try
            {
                await LoadPTListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load danh sách: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnTinhvaTP_Click(object sender, EventArgs eventArgs)
        {
            try
            {
                // Hiển thị dialog để nhập tỉnh/thành phố với Guna2 controls
                using (var form = new Form())
                {
                    form.Text = "Nhập Tỉnh/Thành phố";
                    form.Size = new Size(600, 280); // Giảm width xuống 600px
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;
                    form.BackColor = Color.White;
                    form.Padding = new Padding(0);

                    // Panel chính với border radius và shadow
                    var pnlMain = new Guna2ShadowPanel
                    {
                        Dock = DockStyle.Fill,
                        FillColor = Color.White,
                        Radius = 15, // Guna2ShadowPanel dùng Radius, không phải BorderRadius
                        ShadowColor = Color.Black,
                        ShadowShift = 2,
                        ShadowDepth = 10,
                        Padding = new Padding(30) // Padding đều 2 bên
                    };
                    form.Controls.Add(pnlMain);

                    // Label tiêu đề
                    var lblTitle = new Label
                    {
                        Text = "Nhập Tỉnh/Thành phố",
                        Location = new Point(30, 30),
                        Size = new Size(540, 35),
                        Font = new Font("Times New Roman", 15F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(64, 64, 64),
                        AutoSize = false
                    };
                    pnlMain.Controls.Add(lblTitle);

                    // Label prompt
                    var lblPrompt = new Label
                    {
                        Text = "Nhập tên tỉnh/thành phố:",
                        Location = new Point(30, 75),
                        Size = new Size(540, 30),
                        Font = new Font("Times New Roman", 12F, FontStyle.Regular),
                        ForeColor = Color.FromArgb(100, 100, 100),
                        AutoSize = false
                    };
                    pnlMain.Controls.Add(lblPrompt);

                    // TextBox với Guna2 - width 300px, giữ nguyên vị trí bên trái
                    var txtCity = new Guna2TextBox
                    {
                        Location = new Point(30, 115), // Giữ nguyên vị trí bên trái (30px từ padding)
                        Size = new Size(300, 45), // Width = 300px như yêu cầu
                        Font = new Font("Times New Roman", 12F),
                        BorderColor = Color.Silver,
                        BorderRadius = 10,
                        BorderThickness = 1,
                        PlaceholderText = "Ví dụ: Hồ Chí Minh, Hà Nội, Đà Nẵng...",
                        PlaceholderForeColor = Color.Gray,
                        Cursor = Cursors.IBeam
                    };
                    pnlMain.Controls.Add(txtCity);

                    // Panel chứa buttons - căn giữa
                    var pnlButtons = new Panel
                    {
                        Location = new Point(30, 180),
                        Size = new Size(540, 50),
                        BackColor = Color.Transparent
                    };
                    pnlMain.Controls.Add(pnlButtons);

                    // Biến để lưu kết quả tìm kiếm
                    string searchCity = null;
                    bool shouldSearch = false;

                    // Button Tìm kiếm - căn giữa
                    var btnOK = new Guna2Button
                    {
                        Text = "Tìm kiếm",
                        Location = new Point(155, 5), // Căn giữa: (540 - 130 - 130 - 20) / 2 = 155
                        Size = new Size(130, 40),
                        Font = new Font("Times New Roman", 11F, FontStyle.Bold),
                        ForeColor = Color.White,
                        FillColor = Color.FromArgb(100, 200, 100),
                        BorderRadius = 10,
                        Cursor = Cursors.Hand
                    };
                    // Event handler cho nút Tìm kiếm
                    btnOK.Click += async (btnSender, clickArgs) =>
                    {
                        var cityText = txtCity.Text?.Trim();
                        if (!string.IsNullOrWhiteSpace(cityText))
                        {
                            searchCity = cityText;
                            shouldSearch = true;
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                        }
                        else
                        {
                            MessageBox.Show("Vui lòng nhập tên tỉnh/thành phố!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    };
                    pnlButtons.Controls.Add(btnOK);

                    // Button Hủy - căn giữa, cách button Tìm kiếm 20px
                    var btnCancel = new Guna2Button
                    {
                        Text = "Hủy",
                        Location = new Point(305, 5), // Cách button Tìm kiếm 20px (155 + 130 + 20 = 305)
                        Size = new Size(130, 40),
                        Font = new Font("Times New Roman", 11F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(100, 100, 100),
                        FillColor = Color.FromArgb(240, 240, 240),
                        BorderRadius = 10,
                        Cursor = Cursors.Hand
                    };
                    // Event handler cho nút Hủy
                    btnCancel.Click += (btnSender, clickArgs) =>
                    {
                        shouldSearch = false;
                        form.DialogResult = DialogResult.Cancel;
                        form.Close();
                    };
                    pnlButtons.Controls.Add(btnCancel);

                    form.AcceptButton = btnOK;
                    form.CancelButton = btnCancel;

                    // Focus vào textbox khi mở
                    form.Shown += (formSender, shownArgs) => 
                    {
                        txtCity.Focus();
                        txtCity.Select();
                    };

                    // Xử lý khi Enter trong textbox
                    txtCity.KeyDown += (txtSender, keyArgs) =>
                    {
                        if (keyArgs.KeyCode == Keys.Enter)
                        {
                            keyArgs.SuppressKeyPress = true;
                            btnOK.PerformClick();
                        }
                    };

                    // Hiển thị dialog
                    if (form.ShowDialog() == DialogResult.OK && shouldSearch && !string.IsNullOrWhiteSpace(searchCity))
                    {
                        // Thực hiện tìm kiếm và hiển thị kết quả trong pnlHienThiDanhSach
                        try
                        {
                            Cursor = Cursors.WaitCursor;
                            var pts = await _ptSearchService.FilterPTsByCityAsync(searchCity);
                            pts = FilterOutCurrentPT(pts);
                            _currentPTList = pts;
                            DisplayPTList(pts); // Hiển thị kết quả trong pnlHienThiDanhSach
                            ClearPTDetail(); // Clear chi tiết khi filter
                        }
                        catch (Exception searchEx)
                        {
                            MessageBox.Show($"Lỗi khi tìm kiếm: {searchEx.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            Cursor = Cursors.Default;
                        }
                    }
                }
            }
            catch (Exception filterEx)
            {
                MessageBox.Show($"Lỗi khi lọc theo tỉnh/thành phố: {filterEx.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnChuyenMon_Click(object sender, EventArgs eventArgs)
        {
            try
            {
                // Hiển thị dialog với Guna2 controls
                using (var form = new Form())
                {
                    form.Text = "Chọn Chuyên môn";
                    form.Size = new Size(500, 320);
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;
                    form.BackColor = Color.White;
                    form.Padding = new Padding(0);

                    // Panel chính với border radius và shadow
                    var pnlMain = new Guna2ShadowPanel
                    {
                        Dock = DockStyle.Fill,
                        FillColor = Color.White,
                        Radius = 15,
                        ShadowColor = Color.Black,
                        ShadowShift = 2,
                        ShadowDepth = 10,
                        Padding = new Padding(30)
                    };
                    form.Controls.Add(pnlMain);

                    // Label tiêu đề chính
                    var lblTitle = new Label
                    {
                        Text = "Chọn Chuyên môn",
                        Location = new Point(30, 30),
                        Size = new Size(440, 35),
                        Font = new Font("Times New Roman", 15F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(64, 64, 64),
                        AutoSize = false
                    };
                    pnlMain.Controls.Add(lblTitle);

                    // Label prompt
                    var lblPrompt = new Label
                    {
                        Text = "Vui lòng chọn chuyên môn:",
                        Location = new Point(30, 75),
                        Size = new Size(440, 30),
                        Font = new Font("Times New Roman", 12F, FontStyle.Regular),
                        ForeColor = Color.FromArgb(100, 100, 100),
                        AutoSize = false
                    };
                    pnlMain.Controls.Add(lblPrompt);

                    // Sử dụng Guna2ComboBox thay vì ListBox để đẹp hơn
                    var comboBox = new Guna2ComboBox
                    {
                        Location = new Point(30, 115),
                        Size = new Size(440, 45),
                        Font = new Font("Times New Roman", 12F),
                        BorderColor = Color.Silver,
                        BorderRadius = 10,
                        BorderThickness = 1,
                        BackColor = Color.White,
                        ForeColor = Color.Black,
                        DropDownStyle = ComboBoxStyle.DropDownList // Chỉ cho phép chọn, không nhập
                    };
                    comboBox.Items.Add("Cân nặng");
                    comboBox.Items.Add("Tăng cơ");
                    comboBox.SelectedIndex = 0; // Chọn item đầu tiên mặc định
                    pnlMain.Controls.Add(comboBox);

                    // Panel chứa buttons - căn giữa
                    var pnlButtons = new Panel
                    {
                        Location = new Point(30, 190),
                        Size = new Size(440, 50),
                        BackColor = Color.Transparent
                    };
                    pnlMain.Controls.Add(pnlButtons);

                    // Biến để lưu kết quả
                    string selectedSpecialty = null;
                    bool shouldSearch = false;

                    // Button Tìm kiếm
                    var btnOK = new Guna2Button
                    {
                        Text = "Tìm kiếm",
                        Location = new Point(155, 5), // Căn giữa: (440 - 130 - 130 - 20) / 2 = 155
                        Size = new Size(130, 40),
                        Font = new Font("Times New Roman", 11F, FontStyle.Bold),
                        ForeColor = Color.White,
                        FillColor = Color.FromArgb(100, 200, 100),
                        BorderRadius = 10,
                        Cursor = Cursors.Hand
                    };
                    // Event handler cho nút Tìm kiếm
                    btnOK.Click += (btnSender, clickArgs) =>
                    {
                        if (comboBox.SelectedItem != null)
                        {
                            selectedSpecialty = comboBox.SelectedItem.ToString();
                            shouldSearch = true;
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                        }
                        else
                        {
                            MessageBox.Show("Vui lòng chọn một chuyên môn!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    };
                    pnlButtons.Controls.Add(btnOK);

                    // Button Hủy
                    var btnCancel = new Guna2Button
                    {
                        Text = "Hủy",
                        Location = new Point(305, 5), // Cách button Tìm kiếm 20px (155 + 130 + 20 = 305)
                        Size = new Size(130, 40),
                        Font = new Font("Times New Roman", 11F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(100, 100, 100),
                        FillColor = Color.FromArgb(240, 240, 240),
                        BorderRadius = 10,
                        Cursor = Cursors.Hand
                    };
                    // Event handler cho nút Hủy
                    btnCancel.Click += (btnSender, clickArgs) =>
                    {
                        shouldSearch = false;
                        form.DialogResult = DialogResult.Cancel;
                        form.Close();
                    };
                    pnlButtons.Controls.Add(btnCancel);

                    form.AcceptButton = btnOK;
                    form.CancelButton = btnCancel;

                    // Focus vào combobox khi mở
                    form.Shown += (formSender, shownArgs) =>
                    {
                        comboBox.Focus();
                    };

                    // Hiển thị dialog
                    if (form.ShowDialog() == DialogResult.OK && shouldSearch && !string.IsNullOrWhiteSpace(selectedSpecialty))
                    {
                        // Thực hiện tìm kiếm và hiển thị kết quả trong pnlHienThiDanhSach
                        try
                        {
                            Cursor = Cursors.WaitCursor;
                            var pts = await _ptSearchService.FilterPTsBySpecialtyAsync(selectedSpecialty);
                            pts = FilterOutCurrentPT(pts);
                            _currentPTList = pts;
                            DisplayPTList(pts); // Hiển thị kết quả trong pnlHienThiDanhSach
                            ClearPTDetail(); // Clear chi tiết khi filter
                        }
                        catch (Exception searchEx)
                        {
                            MessageBox.Show($"Lỗi khi tìm kiếm: {searchEx.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            Cursor = Cursors.Default;
                        }
                    }
                }
            }
            catch (Exception filterEx)
            {
                MessageBox.Show($"Lỗi khi lọc theo chuyên môn: {filterEx.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnGuiYeuCau_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_selectedPTID))
                {
                    MessageBox.Show("Vui lòng chọn một PT trước!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi gửi yêu cầu!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy thông tin chi tiết PT để lấy chuyên môn
                var detail = await _ptSearchService.GetPTDetailAsync(_selectedPTID);
                if (detail == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin PT!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mở form yêu cầu tập luyện
                this.Hide();
                var frmYeuCau = new YeuCauTapLuyen(this, _selectedPTID, detail.DanhSachChuyenMon);
                frmYeuCau.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                if (_parentDashboard != null && !_parentDashboard.IsDisposed)
                {
                    _parentDashboard.ShowDashboard();
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Ẩn chính bản thân PT khỏi danh sách (để PT không thể thuê chính mình)
        /// </summary>
        private List<PTSearchViewModel> FilterOutCurrentPT(List<PTSearchViewModel> pts)
        {
            try
            {
                if (pts == null || pts.Count == 0)
                    return pts;

                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                    return pts;

                // Chỉ áp dụng với tài khoản có role PT
                if (!string.Equals(CurrentUser.User.Role, "PT", StringComparison.OrdinalIgnoreCase))
                    return pts;

                var currentUserId = CurrentUser.User.UserID;
                if (string.IsNullOrWhiteSpace(currentUserId))
                    return pts;

                return pts
                    .Where(p => !string.Equals(p.UserID, currentUserId, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch
            {
                return pts;
            }
        }

        // Các event handlers cũ giữ lại để không lỗi
        private void lblSoDanhGia_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void guna2PictureBox4_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel2_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel2_Click_1(object sender, EventArgs e) { }
        private void lblGiaTheoGio_Click(object sender, EventArgs e) { }
    }
}
