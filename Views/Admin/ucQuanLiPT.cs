extern alias ef6;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Models;
using ef6::System.Data.Entity;

namespace HealthApp.Views.Admin
{
    public partial class ucQuanLiPT : UserControl
    {
        private WF_HealthTracker _dbContext;

        public ucQuanLiPT()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            this.Load += ucQuanLiPT_Load;
            InitializeFilters();
        }

        /// <summary>
        /// Khởi tạo các filter controls
        /// </summary>
        private void InitializeFilters()
        {
            // Xử lý placeholder cho txtTiemKiem
            // Guna2TextBox sử dụng DefaultText làm placeholder, khi focus vào sẽ tự động clear
            // Nhưng chúng ta cần xử lý thêm để đảm bảo placeholder hiển thị đúng
            txtTiemKiem.Enter += TxtTiemKiem_Enter;
            txtTiemKiem.Leave += TxtTiemKiem_Leave;

            // Populate cboChuyenMon
            cboChuyenMon.Items.Clear();
            cboChuyenMon.Items.Add("Cân nặng");
            cboChuyenMon.Items.Add("Tăng cơ");

            // Populate cboDiaChi với 34 tỉnh thành
            cboDiaChi.Items.Clear();
            string[] tinhThanh = {
                "Hà Nội", "TP. Hồ Chí Minh", "Hải Phòng", "Đà Nẵng", "Cần Thơ", "Huế",
                "Lai Châu", "Điện Biên", "Sơn La", "Lạng Sơn", "Quảng Ninh", "Thanh Hóa",
                "Nghệ An", "Hà Tĩnh", "Cao Bằng", "Tuyên Quang", "Lào Cai", "Thái Nguyên",
                "Phú Thọ", "Bắc Ninh", "Hưng Yên", "Ninh Bình", "Quảng Trị", "Quảng Ngãi",
                "Gia Lai", "Khánh Hòa", "Lâm Đồng", "Đắk Lắk", "Đồng Nai", "Tây Ninh",
                "Vĩnh Long", "Đồng Tháp", "Cà Mau", "An Giang"
            };
            foreach (var tinh in tinhThanh)
            {
                cboDiaChi.Items.Add(tinh);
            }
            // Set DropDownHeight để hiển thị tối đa 4 items (mỗi item ~30px)
            cboDiaChi.DropDownHeight = 4 * 30 + 2; // 4 items + border

            // Populate cboNhanKhach
            cboNhanKhach.Items.Clear();
            cboNhanKhach.Items.Add("Đang nhận");
            cboNhanKhach.Items.Add("Không có khách");

            // Set min/max cho nudDanhGia
            nudDanhGia.Minimum = 0;
            nudDanhGia.Maximum = 5;
            nudDanhGia.DecimalPlaces = 1;
            nudDanhGia.Increment = 0.1m;
            nudDanhGia.Value = 0;

            // Event handlers cho buttons
            btnApDung.Click += BtnApDung_Click;
            btnDatLai.Click += BtnDatLai_Click;
            btnXacMinhDangKy.Click += BtnXacMinhDangKy_Click;
        }

        private void ucQuanLiPT_Load(object sender, EventArgs e)
        {
            // Đảm bảo panel container có AutoScroll
            if (pnlDanhSachHuanLuyenVien != null)
            {
                pnlDanhSachHuanLuyenVien.AutoScroll = true;
            }
            
            LoadStatistics();
            LoadPTList();
        }

        /// <summary>
        /// Load và hiển thị các thống kê về PT
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                // 1. Tổng số PT trong hệ thống
                int totalPTs = _dbContext.HuanLuyenVien.Count();
                lblTongSoPT.Text = totalPTs.ToString();

                // 2. Doanh thu trung bình PT/tháng (tính từ 12 tháng gần nhất)
                DateTime oneYearAgo = DateTime.Now.AddYears(-1);
                var completedTransactions = _dbContext.GiaoDich
                    .Where(gd => gd.TrangThaiThanhToan == "Completed" && 
                                 gd.NgayGiaoDich.HasValue && 
                                 gd.NgayGiaoDich >= oneYearAgo &&
                                 gd.SoTienPTNhan.HasValue)
                    .ToList();

                double averageMonthlyRevenuePerPT = 0;
                if (completedTransactions.Any() && totalPTs > 0)
                {
                    // Tính tổng doanh thu trong 12 tháng qua
                    double totalRevenueLastYear = completedTransactions.Sum(gd => gd.SoTienPTNhan ?? 0);
                    
                    // Tính số tháng có giao dịch
                    int distinctMonths = completedTransactions
                        .Select(gd => new { 
                            Year = gd.NgayGiaoDich.Value.Year, 
                            Month = gd.NgayGiaoDich.Value.Month 
                        })
                        .Distinct()
                        .Count();
                    
                    // Doanh thu trung bình/tháng = Tổng doanh thu / Số tháng / Số PT
                    if (distinctMonths > 0)
                    {
                        averageMonthlyRevenuePerPT = totalRevenueLastYear / distinctMonths / totalPTs;
                    }
                }
                lblSoDoanhThuTrungBinh.Text = FormatCurrency(averageMonthlyRevenuePerPT);

                // 3. Đánh giá trung bình của tất cả PT (tính từ bảng DanhGiaPT)
                // Tính đánh giá trung bình cho từng PT, sau đó lấy trung bình của tất cả PT
                var verifiedPTs = _dbContext.HuanLuyenVien
                    .Where(pt => pt.DaXacMinh == true) // Chỉ tính PT đã được duyệt
                    .Select(pt => pt.PTID)
                    .ToList();
                
                var ptAverageRatings = new List<double>();
                
                foreach (var ptId in verifiedPTs)
                {
                    var ratings = _dbContext.DanhGiaPT
                        .Where(dg => dg.PTID == ptId && dg.Diem >= 1 && dg.Diem <= 5)
                        .Select(dg => (double?)dg.Diem)
                        .ToList();
                    
                    double ptAvg = ratings.Any() 
                        ? ratings.Average() ?? 0.0 
                        : 0.0;
                    
                    ptAverageRatings.Add(ptAvg);
                }
                
                double averageRating = ptAverageRatings.Any() 
                    ? Math.Round(ptAverageRatings.Average(), 1) 
                    : 0.0;
                lblSoDanhGiaTrungBinh.Text = averageRating.ToString("F1");

                // 4. Trung bình khách hàng đang thuê PT/tháng (trong 1 năm gần nhất)
                var activeCustomersPerMonth = _dbContext.DatLichPT
                    .Where(dl => (dl.TrangThai == "Confirmed" || dl.TrangThai == "Completed") && 
                                 dl.NgayTao.HasValue && 
                                 dl.NgayTao >= oneYearAgo)
                    .GroupBy(dl => new { 
                        Year = dl.NgayTao.Value.Year, 
                        Month = dl.NgayTao.Value.Month 
                    })
                    .Select(g => g.Select(dl => dl.KhachHangID).Distinct().Count())
                    .ToList();

                double averageCustomersPerMonth = activeCustomersPerMonth.Any() 
                    ? activeCustomersPerMonth.Average() 
                    : 0.0;
                lblSoTrungBinhKhachHangDangThue.Text = averageCustomersPerMonth.ToString("F1");

                // 5. Tỉ lệ client thuê PT
                int totalClients = _dbContext.Users.Count(u => u.Role == "Client");
                
                // Lấy danh sách UserID của các client
                var clientUserIDs = _dbContext.Users
                    .Where(u => u.Role == "Client")
                    .Select(u => u.UserID)
                    .ToList();
                
                // Đếm số client unique có DatLichPT với trạng thái Confirmed hoặc Completed
                int clientsWithPT = _dbContext.DatLichPT
                    .Where(dl => clientUserIDs.Contains(dl.KhachHangID) && 
                                 (dl.TrangThai == "Confirmed" || dl.TrangThai == "Completed"))
                    .Select(dl => dl.KhachHangID)
                    .Distinct()
                    .Count();
                
                double clientPTPercentage = (totalClients > 0) 
                    ? ((double)clientsWithPT / totalClients * 100) 
                    : 0.0;
                lblSoTyLeKhachThuePT.Text = clientPTPercentage.ToString("F1");

                // 6. Trung bình PT đang có lịch thuê theo buổi
                var sessionsPerPT = _dbContext.DatLichPT
                    .Where(dl => (dl.TrangThai == "Confirmed" || dl.TrangThai == "Completed") && 
                                 dl.PTID != null)
                    .GroupBy(dl => dl.PTID)
                    .Select(g => g.Count())
                    .ToList();
                
                double averageSessionsPerPT = sessionsPerPT.Any() 
                    ? sessionsPerPT.Average() 
                    : 0.0;
                lblSoTrungBinhPTDangCoLich.Text = averageSessionsPerPT.ToString("F1");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thống kê: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLiPT] Error loading statistics: {ex.Message}\n{ex.StackTrace}");
                
                // Set default values on error
                lblTongSoPT.Text = "0";
                lblSoDoanhThuTrungBinh.Text = "0 VND";
                lblSoDanhGiaTrungBinh.Text = "0.0";
                lblSoTrungBinhKhachHangDangThue.Text = "0.0";
                lblSoTyLeKhachThuePT.Text = "0.0%";
                lblSoTrungBinhPTDangCoLich.Text = "0.0";
            }
        }

        /// <summary>
        /// Format số tiền thành định dạng dễ đọc (VND, K, M)
        /// </summary>
        private string FormatCurrency(double amount)
        {
            if (amount >= 1000000) // Millions
            {
                return (amount / 1000000).ToString("F1") + "M";
            }
            else if (amount >= 1000) // Thousands
            {
                return (amount / 1000).ToString("F0") + "K";
            }
            return amount.ToString("F0");
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
        }

        private void pnlTrungBinhKhachDangThuePT_Paint(object sender, PaintEventArgs e)
        {
            // Event handler cho Paint event của panel
            // Có thể thêm custom painting logic nếu cần
        }

        /// <summary>
        /// Load danh sách PT và hiển thị trong pnlDanhSachHuanLuyenVien
        /// </summary>
        private void LoadPTList()
        {
            LoadPTList(null);
        }

        /// <summary>
        /// Load danh sách PT với filter
        /// </summary>
        private void LoadPTList(PTFilter filter)
        {
            try
            {
                // Xóa tất cả panel PT hiện có (trừ panel thiết kế ban đầu và các control khác)
                var panelsToRemove = pnlDanhSachHuanLuyenVien.Controls
                    .OfType<Guna.UI2.WinForms.Guna2CustomGradientPanel>()
                    .Where(p => p.Name != null && 
                                p.Name.StartsWith("pnlThongTinPT_") && 
                                p.Name != "pnlThongTinPT" &&
                                p.Name != "pnlThongTinPT2" &&
                                p.Name != "pnlThongTinPT3")
                    .ToList();

                foreach (var panel in panelsToRemove)
                {
                    pnlDanhSachHuanLuyenVien.Controls.Remove(panel);
                    panel.Dispose();
                }

                // Ẩn tất cả panel thiết kế ban đầu (mẫu)
                pnlThongTinPT.Visible = false;
                
                // Ẩn các panel mẫu khác nếu có
                var samplePanels = pnlDanhSachHuanLuyenVien.Controls
                    .OfType<Guna.UI2.WinForms.Guna2CustomGradientPanel>()
                    .Where(p => p.Name == "pnlThongTinPT" || 
                                p.Name == "pnlThongTinPT2" || 
                                p.Name == "pnlThongTinPT3")
                    .ToList();
                
                foreach (var panel in samplePanels)
                {
                    panel.Visible = false;
                }

                // Load danh sách PT từ database với filter (chỉ lấy PT đã được xác minh)
                var query = _dbContext.HuanLuyenVien
                    .Include("Users")
                    .Where(pt => pt.DaXacMinh == true) // Chỉ lấy PT đã được duyệt
                    .AsQueryable();

                // Áp dụng filter nếu có
                if (filter != null)
                {
                    // Filter theo tên tìm kiếm
                    if (!string.IsNullOrWhiteSpace(filter.SearchText))
                    {
                        string searchLower = filter.SearchText.ToLower();
                        query = query.Where(pt => 
                            (pt.Users.HoTen != null && pt.Users.HoTen.ToLower().Contains(searchLower)) ||
                            (pt.Users.Username != null && pt.Users.Username.ToLower().Contains(searchLower)) ||
                            (pt.PTID != null && pt.PTID.ToLower().Contains(searchLower)));
                    }

                    // Filter theo chuyên môn
                    if (!string.IsNullOrWhiteSpace(filter.ChuyenMon))
                    {
                        query = query.Where(pt => pt.ChuyenMon != null && pt.ChuyenMon.Contains(filter.ChuyenMon));
                    }

                    // Filter theo địa chỉ
                    if (!string.IsNullOrWhiteSpace(filter.DiaChi))
                    {
                        query = query.Where(pt => pt.ThanhPho == filter.DiaChi);
                    }

                    // Filter theo nhận khách
                    if (!string.IsNullOrWhiteSpace(filter.NhanKhach))
                    {
                        // Lấy danh sách PT có khách hàng đã thuê và thanh toán
                        var ptIdsWithPaidCustomers = _dbContext.GiaoDich
                            .Where(gd => gd.TrangThaiThanhToan == "Completed" && 
                                       gd.PTID != null)
                            .Select(gd => gd.PTID)
                            .Distinct()
                            .ToList();

                        if (filter.NhanKhach == "Đang nhận")
                        {
                            // PT có khách hàng đã thuê và thanh toán
                            query = query.Where(pt => ptIdsWithPaidCustomers.Contains(pt.PTID));
                        }
                        else if (filter.NhanKhach == "Không có khách")
                        {
                            // PT không có khách hàng nào thuê và thanh toán
                            query = query.Where(pt => !ptIdsWithPaidCustomers.Contains(pt.PTID));
                        }
                    }

                    // Filter theo điểm đánh giá (tính từ DanhGiaPT)
                    if (filter.DiemDanhGiaMin.HasValue && filter.DiemDanhGiaMin.Value > 0)
                    {
                        // Lấy danh sách PT có điểm trung bình >= giá trị filter
                        var ptIdsWithRating = _dbContext.DanhGiaPT
                            .GroupBy(dg => dg.PTID)
                            .Select(g => new { 
                                PTID = g.Key, 
                                AvgRating = g.Average(dg => (double?)dg.Diem) 
                            })
                            .Where(x => x.AvgRating.HasValue && x.AvgRating.Value >= filter.DiemDanhGiaMin.Value)
                            .Select(x => x.PTID)
                            .ToList();
                        
                        // Chỉ lấy PT có điểm >= giá trị filter
                        query = query.Where(pt => ptIdsWithRating.Contains(pt.PTID));
                    }
                    // Nếu filter = 0, lấy tất cả PT (có hoặc không có đánh giá) - không filter gì cả
                }

                var pts = query.ToList();

                if (!pts.Any())
                {
                    return;
                }

                // Kích thước và khoảng cách
                const int panelWidth = 328;
                const int panelHeight = 461;
                const int marginX = 40; // Khoảng cách giữa các cột (tăng từ 23 lên 40)
                const int marginY = 25; // Khoảng cách giữa các hàng (tăng từ 20 lên 25)
                const int startY = 71; // Vị trí Y bắt đầu
                
                // Tính số cột mỗi hàng dựa trên chiều rộng container
                int containerWidth = pnlDanhSachHuanLuyenVien.Width;
                int columnsPerRow = Math.Max(1, (containerWidth - marginX) / (panelWidth + marginX));
                
                // Đảm bảo tối thiểu 2 cột và tối đa 3 cột để hiển thị đẹp
                columnsPerRow = Math.Max(2, Math.Min(3, columnsPerRow));

                // Tạo panel cho mỗi PT
                for (int i = 0; i < pts.Count; i++)
                {
                    var pt = pts[i];
                    int row = i / columnsPerRow;
                    int col = i % columnsPerRow;

                    // Tính vị trí - đảm bảo không bị chồng
                    // Card đầu tiên (col = 0) dịch sang trái thêm một chút để tránh chồng
                    int x = marginX + col * (panelWidth + marginX);
                    if (col == 0)
                    {
                        x = Math.Max(10, marginX - 15); // Dịch sang trái 15 pixels cho card đầu tiên
                    }
                    int y = startY + row * (panelHeight + marginY);

                    // Tạo panel mới
                    var ptPanel = CreatePTPanel(pt, i);
                    ptPanel.Location = new Point(x, y);
                    ptPanel.Name = $"pnlThongTinPT_{pt.PTID}";
                    ptPanel.Visible = true;
                    ptPanel.BringToFront(); // Đảm bảo panel ở trên cùng

                    pnlDanhSachHuanLuyenVien.Controls.Add(ptPanel);
                }
                
                // Đảm bảo panel container có AutoScroll
                if (!pnlDanhSachHuanLuyenVien.AutoScroll)
                {
                    pnlDanhSachHuanLuyenVien.AutoScroll = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLiPT] Error loading PT list: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Tạo panel hiển thị thông tin PT
        /// </summary>
        private Guna.UI2.WinForms.Guna2CustomGradientPanel CreatePTPanel(HuanLuyenVien pt, int index)
        {
            // Clone panel thiết kế ban đầu
            var panel = ClonePanel(pnlThongTinPT);
            panel.Name = $"pnlThongTinPT_{pt.PTID}";

            // Lấy thông tin User
            var user = pt.Users;
            if (user == null)
            {
                user = _dbContext.Users.FirstOrDefault(u => u.UserID == pt.UserID);
            }

            // Tính toán các thông tin
            int soKhachHang = _dbContext.DatLichPT
                .Where(dl => dl.PTID == pt.PTID && 
                            (dl.TrangThai == "Confirmed" || dl.TrangThai == "Completed"))
                .Select(dl => dl.KhachHangID)
                .Distinct()
                .Count();

            double doanhThu = _dbContext.GiaoDich
                .Where(gd => gd.PTID == pt.PTID && 
                            gd.TrangThaiThanhToan == "Completed" &&
                            gd.SoTienPTNhan.HasValue)
                .Select(gd => gd.SoTienPTNhan ?? 0)
                .DefaultIfEmpty(0)
                .Sum();

            // Tính điểm trung bình từ bảng DanhGiaPT
            var danhGiaList = _dbContext.DanhGiaPT
                .Where(dg => dg.PTID == pt.PTID)
                .ToList();
            
            double? diemTrungBinh = danhGiaList.Any() 
                ? (double?)Math.Round(danhGiaList.Average(dg => (double)dg.Diem), 1) 
                : (double?)0.0;

            // Populate dữ liệu vào các control
            SetControlText(panel, "lblHovaTen", user?.HoTen ?? user?.Username ?? "N/A");
            SetControlText(panel, "lblMaPT", pt.PTID ?? "N/A");
            SetControlText(panel, "lblGmail", user?.Email ?? "N/A");
            SetControlText(panel, "lblSoDienThoai", user?.SDT ?? "N/A");
            SetControlText(panel, "lblDiaChi", pt.ThanhPho ?? "N/A");
            SetControlText(panel, "lblGiaThue", pt.GiaTheoGio.HasValue 
                ? $"{pt.GiaTheoGio.Value:F0}/giờ" 
                : "N/A");
            SetControlText(panel, "lblSoDanhGiaPT", diemTrungBinh.HasValue 
                ? diemTrungBinh.Value.ToString("F1") 
                : "0.0");
            SetControlText(panel, "lblSoKhachHangDaThue", soKhachHang.ToString());
            SetControlText(panel, "lblSoDoanhThuPT", FormatCurrency(doanhThu));

            // Load ảnh đại diện
            var ptrAnhDaiDien = FindControl<Guna.UI2.WinForms.Guna2CirclePictureBox>(panel, "ptrAnhDaiDien");
            if (ptrAnhDaiDien != null)
            {
                try
                {
                    string imagePath = pt.AnhDaiDien ?? user?.AnhDaiDien;
                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                    {
                        ptrAnhDaiDien.Image = Image.FromFile(imagePath);
                    }
                }
                catch
                {
                    // Nếu không load được ảnh, giữ ảnh mặc định
                }
            }

            // Gắn event handler cho nút Xem chi tiết và Xóa
            var btnXemChiTiet = FindControl<Guna.UI2.WinForms.Guna2GradientButton>(panel, "btnXemChiTietPT");
            if (btnXemChiTiet != null)
            {
                btnXemChiTiet.Tag = pt.PTID;
                btnXemChiTiet.Click += BtnXemChiTietPT_Click;
            }

            var btnXoa = FindControl<Guna.UI2.WinForms.Guna2Button>(panel, "btnXoa");
            if (btnXoa != null)
            {
                btnXoa.Tag = pt.PTID;
                btnXoa.Click += BtnXoa_Click;
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
                    Image = sourcePic.Image != null ? (Image)sourcePic.Image.Clone() : null
                };
            }
            else if (source is PictureBox)
            {
                var sourcePic = source as PictureBox;
                cloned = new PictureBox
                {
                    Name = sourcePic.Name,
                    Location = sourcePic.Location,
                    Size = sourcePic.Size,
                    SizeMode = sourcePic.SizeMode,
                    Image = sourcePic.Image != null ? (Image)sourcePic.Image.Clone() : null,
                    BackColor = sourcePic.BackColor,
                    BorderStyle = sourcePic.BorderStyle
                };
            }
            else if (source is Guna.UI2.WinForms.Guna2PictureBox)
            {
                var sourcePic = source as Guna.UI2.WinForms.Guna2PictureBox;
                cloned = new Guna.UI2.WinForms.Guna2PictureBox
                {
                    Name = sourcePic.Name,
                    Location = sourcePic.Location,
                    Size = sourcePic.Size,
                    SizeMode = sourcePic.SizeMode,
                    Image = sourcePic.Image != null ? (Image)sourcePic.Image.Clone() : null,
                    BackColor = sourcePic.BackColor,
                    BorderRadius = sourcePic.BorderRadius,
                    ImageRotate = sourcePic.ImageRotate
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
            else if (source is Label)
            {
                var sourceLabel = source as Label;
                cloned = new Label
                {
                    Name = sourceLabel.Name,
                    Text = sourceLabel.Text,
                    Location = sourceLabel.Location,
                    Size = sourceLabel.Size,
                    Font = sourceLabel.Font,
                    ForeColor = sourceLabel.ForeColor,
                    BackColor = sourceLabel.BackColor,
                    AutoSize = sourceLabel.AutoSize,
                    TextAlign = sourceLabel.TextAlign
                };
            }
            else if (source is Panel)
            {
                var sourcePanel = source as Panel;
                cloned = new Panel
                {
                    Name = sourcePanel.Name,
                    Location = sourcePanel.Location,
                    Size = sourcePanel.Size,
                    BackColor = sourcePanel.BackColor,
                    BorderStyle = sourcePanel.BorderStyle
                };

                // Clone controls bên trong panel
                foreach (Control child in sourcePanel.Controls)
                {
                    cloned.Controls.Add(CloneControl(child));
                }
            }

            // Nếu không match với bất kỳ loại nào ở trên, tạo control cơ bản
            if (cloned == null)
            {
                cloned = new Control 
                { 
                    Name = source.Name, 
                    Location = source.Location, 
                    Size = source.Size,
                    BackColor = source.BackColor,
                    ForeColor = source.ForeColor
                };
            }

            return cloned;
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
        /// Event handler cho nút Xem chi tiết PT
        /// </summary>
        private void BtnXemChiTietPT_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2GradientButton;
            if (button?.Tag != null)
            {
                string ptId = button.Tag.ToString();
                try
                {
                    // Lấy thông tin PT
                    var pt = _dbContext.HuanLuyenVien
                        .Include("Users")
                        .FirstOrDefault(p => p.PTID == ptId);
                    
                    if (pt == null)
                    {
                        MessageBox.Show("Không tìm thấy thông tin PT!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Mở form chi tiết PT
                    var form = new Form
                    {
                        Text = $"Chi tiết PT: {ptId}",
                        Size = new Size(900, 700),
                        StartPosition = FormStartPosition.CenterParent
                    };

                    var ucChiTiet = new ucThongTinChiTietPT();
                    ucChiTiet.Dock = DockStyle.Fill;
                    form.Controls.Add(ucChiTiet);
                    
                    // Load dữ liệu PT vào ucChiTiet
                    ucChiTiet.LoadPTData(pt);

                    form.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở chi tiết PT: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Event handler cho nút Xóa PT
        /// </summary>
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2Button;
            if (button?.Tag != null)
            {
                string ptId = button.Tag.ToString();
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa PT {ptId}?\n\nLưu ý: Hành động này sẽ xóa tất cả dữ liệu liên quan đến PT này.", 
                    "Xác nhận xóa", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var pt = _dbContext.HuanLuyenVien.FirstOrDefault(p => p.PTID == ptId);
                        if (pt != null)
                        {
                            // Xóa PT (cascade delete sẽ xóa các bản ghi liên quan)
                            _dbContext.HuanLuyenVien.Remove(pt);
                            _dbContext.SaveChanges();
                            
                            MessageBox.Show($"Đã xóa PT {ptId} thành công!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Reload danh sách và thống kê
                            LoadPTList();
                            LoadStatistics();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy PT để xóa!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa PT: {ex.Message}", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler cho txtTiemKiem Enter (khi focus vào)
        /// </summary>
        private void TxtTiemKiem_Enter(object sender, EventArgs e)
        {
            // Guna2TextBox tự động clear DefaultText khi focus, nhưng chúng ta đảm bảo nó được clear
            if (txtTiemKiem.Text == "Tìm kiếm.." || txtTiemKiem.Text == "Tìm kiếm..,")
            {
                txtTiemKiem.Text = "";
            }
        }

        /// <summary>
        /// Event handler cho txtTiemKiem Leave (khi mất focus)
        /// </summary>
        private void TxtTiemKiem_Leave(object sender, EventArgs e)
        {
            // Nếu text rỗng, không cần làm gì vì Guna2TextBox sẽ tự động hiển thị DefaultText
            // Nhưng chúng ta có thể set lại nếu cần
            if (string.IsNullOrWhiteSpace(txtTiemKiem.Text))
            {
                txtTiemKiem.Text = "";
            }
        }

        /// <summary>
        /// Event handler cho btnApDung - Áp dụng filter
        /// </summary>
        private void BtnApDung_Click(object sender, EventArgs e)
        {
            // Lấy text từ txtTiemKiem, loại bỏ placeholder
            string searchText = txtTiemKiem.Text;
            if (searchText == "Tìm kiếm.." || searchText == "Tìm kiếm..," || string.IsNullOrWhiteSpace(searchText))
            {
                searchText = null;
            }
            else
            {
                searchText = searchText.Trim();
            }

            var filter = new PTFilter
            {
                SearchText = searchText,
                ChuyenMon = cboChuyenMon.SelectedItem?.ToString(),
                DiaChi = cboDiaChi.SelectedItem?.ToString(),
                NhanKhach = cboNhanKhach.SelectedItem?.ToString(),
                DiemDanhGiaMin = nudDanhGia.Value > 0 ? (double?)nudDanhGia.Value : null
            };

            LoadPTList(filter);
        }

        /// <summary>
        /// Event handler cho btnDatLai - Reset filter
        /// </summary>
        private void BtnDatLai_Click(object sender, EventArgs e)
        {
            // Reset tất cả filter
            txtTiemKiem.Text = "";
            cboChuyenMon.SelectedIndex = -1;
            cboDiaChi.SelectedIndex = -1;
            cboNhanKhach.SelectedIndex = -1;
            nudDanhGia.Value = 0;

            // Reload danh sách không filter
            LoadPTList();
        }

        /// <summary>
        /// Event handler cho nút Xác minh đăng ký
        /// </summary>
        private void BtnXacMinhDangKy_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở form duyệt PT
                var frmDuyet = new frmDuyetPT();
                frmDuyet.StartPosition = FormStartPosition.CenterParent;
                frmDuyet.ShowDialog();
                
                // Reload danh sách và thống kê sau khi đóng form duyệt
                LoadPTList();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form duyệt PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Class để lưu filter criteria
        /// </summary>
        private class PTFilter
        {
            public string SearchText { get; set; }
            public string ChuyenMon { get; set; }
            public string DiaChi { get; set; }
            public string NhanKhach { get; set; }
            public double? DiemDanhGiaMin { get; set; }
        }
    }
}
