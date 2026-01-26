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

            // Số đánh giá
            var lblSoDanhGia = new Label
            {
                AutoSize = true,
                Font = new Font("Times New Roman", 7.2F, FontStyle.Regular),
                ForeColor = Color.Silver,
                Location = new Point(172, 60),
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

        private async void BtnTinhvaTP_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị dialog để nhập tỉnh/thành phố
                using (var form = new Form())
                {
                    form.Text = "Nhập Tỉnh/Thành phố";
                    form.Size = new Size(400, 150);
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;

                    var lblPrompt = new Label
                    {
                        Text = "Nhập tên tỉnh/thành phố:",
                        Location = new Point(20, 20),
                        Size = new Size(350, 20),
                        Font = new Font("Segoe UI", 10F)
                    };
                    form.Controls.Add(lblPrompt);

                    var txtCity = new TextBox
                    {
                        Location = new Point(20, 50),
                        Size = new Size(340, 25),
                        Font = new Font("Segoe UI", 10F)
                    };
                    form.Controls.Add(txtCity);

                    var btnOK = new Button
                    {
                        Text = "Tìm kiếm",
                        Location = new Point(200, 85),
                        Size = new Size(80, 30),
                        DialogResult = DialogResult.OK
                    };
                    form.Controls.Add(btnOK);

                    var btnCancel = new Button
                    {
                        Text = "Hủy",
                        Location = new Point(285, 85),
                        Size = new Size(80, 30),
                        DialogResult = DialogResult.Cancel
                    };
                    form.Controls.Add(btnCancel);

                    form.AcceptButton = btnOK;
                    form.CancelButton = btnCancel;

                    if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtCity.Text))
                    {
                        var selectedCity = txtCity.Text.Trim();
                        var pts = await _ptSearchService.FilterPTsByCityAsync(selectedCity);
                        _currentPTList = pts;
                        DisplayPTList(pts);
                        ClearPTDetail(); // Clear chi tiết khi filter
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc theo tỉnh/thành phố: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnChuyenMon_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị dialog với 2 lựa chọn: "Cân nặng" và "Tăng cơ"
                using (var form = new Form())
                {
                    form.Text = "Chọn Chuyên môn";
                    form.Size = new Size(350, 250); // Tăng chiều cao từ 200 lên 250
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MaximizeBox = false;
                    form.MinimizeBox = false;
                    form.BackColor = Color.White;

                    // Label tiêu đề
                    var lblTitle = new Label
                    {
                        Text = "Vui lòng chọn chuyên môn:",
                        Location = new Point(20, 20),
                        Size = new Size(300, 25),
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                    };
                    form.Controls.Add(lblTitle);

                    // ListBox với đầy đủ kích thước
                    var listBox = new ListBox
                    {
                        Location = new Point(20, 50),
                        Size = new Size(300, 100), // Tăng chiều cao từ 80 lên 100
                        Font = new Font("Segoe UI", 10F),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    listBox.Items.Add("Cân nặng");
                    listBox.Items.Add("Tăng cơ");
                    form.Controls.Add(listBox);

                    // Button Tìm kiếm
                    var btnOK = new Button
                    {
                        Text = "Tìm kiếm",
                        Location = new Point(150, 170), // Tăng vị trí Y từ 140 lên 170
                        Size = new Size(80, 35), // Tăng chiều cao từ 30 lên 35
                        DialogResult = DialogResult.OK,
                        Font = new Font("Segoe UI", 9F),
                        BackColor = Color.FromArgb(128, 255, 128),
                        FlatStyle = FlatStyle.Flat
                    };
                    btnOK.FlatAppearance.BorderSize = 0;
                    form.Controls.Add(btnOK);

                    // Button Hủy
                    var btnCancel = new Button
                    {
                        Text = "Hủy",
                        Location = new Point(240, 170), // Tăng vị trí Y từ 140 lên 170
                        Size = new Size(80, 35), // Tăng chiều cao từ 30 lên 35
                        DialogResult = DialogResult.Cancel,
                        Font = new Font("Segoe UI", 9F),
                        BackColor = Color.LightGray,
                        FlatStyle = FlatStyle.Flat
                    };
                    btnCancel.FlatAppearance.BorderSize = 0;
                    form.Controls.Add(btnCancel);

                    form.AcceptButton = btnOK;
                    form.CancelButton = btnCancel;

                    // Chọn item đầu tiên mặc định
                    if (listBox.Items.Count > 0)
                    {
                        listBox.SelectedIndex = 0;
                    }

                    if (form.ShowDialog() == DialogResult.OK && listBox.SelectedItem != null)
                    {
                        var selectedSpecialty = listBox.SelectedItem.ToString();
                        var pts = await _ptSearchService.FilterPTsBySpecialtyAsync(selectedSpecialty);
                        _currentPTList = pts;
                        DisplayPTList(pts);
                        ClearPTDetail(); // Clear chi tiết khi filter
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc theo chuyên môn: {ex.Message}", "Lỗi", 
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
