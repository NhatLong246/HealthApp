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
    public partial class ucQuanLyDinhDuong : UserControl
    {
        private WF_HealthTracker _dbContext;
        private ThuVienMonAn _selectedMonAn;
        private Panel _overlayPanel; // Panel để hiển thị ucThemMonAn hoặc ucSuaMonAn
        private MonAnFilter _currentFilter; // Lưu filter hiện tại

        // Phân trang danh sách món ăn
        private const int _pageSize = 4; // 4 món ăn mỗi trang (4 card)
        private int _currentPage = 1;
        private int _totalPages = 1;
        private List<ThuVienMonAn> _allMonAn = new List<ThuVienMonAn>();

        public ucQuanLyDinhDuong()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            this.Load += ucQuanLyDinhDuong_Load;
            this.Disposed += UcQuanLyDinhDuong_Disposed;
            InitializeFilters();
            InitializeOverlayPanel();
        }

        /// <summary>
        /// Dispose resources khi control bị dispose
        /// </summary>
        private void UcQuanLyDinhDuong_Disposed(object sender, EventArgs e)
        {
            _dbContext?.Dispose();
        }

        /// <summary>
        /// Khởi tạo panel overlay để hiển thị ucThemMonAn hoặc ucSuaMonAn
        /// </summary>
        private void InitializeOverlayPanel()
        {
            _overlayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(200, 0, 0, 0), // Nền đen mờ
                Visible = false
            };
            this.Controls.Add(_overlayPanel);
            _overlayPanel.BringToFront();
        }

        /// <summary>
        /// Khởi tạo các filter controls
        /// </summary>
        private void InitializeFilters()
        {
            // Xử lý placeholder cho txtTiemKiem
            txtTiemKiem.Enter += TxtTiemKiem_Enter;
            txtTiemKiem.Leave += TxtTiemKiem_Leave;

            // Load dữ liệu từ database
            LoadFilterDataFromDatabase();

            // Event handlers cho buttons
            btnApDung.Click += BtnApDung_Click;
            btnDatLai.Click += BtnDatLai_Click;

            guna2GradientButton3.Click += BtnThemMoi_Click;

            // Event handlers cho phân trang
            btnNext.Click += BtnNext_Click;
            btnPrevious.Click += BtnPrevious_Click;
        }

        /// <summary>
        /// Load dữ liệu filter từ database
        /// </summary>
        private void LoadFilterDataFromDatabase()
        {
            try
            {
                // Load loại món ăn từ database
                LoadLoaiMonAnFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu filter: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] LoadFilterDataFromDatabase error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load danh sách loại món ăn từ database
        /// </summary>
        private void LoadLoaiMonAnFromDatabase()
        {
            cboLoai.Items.Clear();
            cboLoai.Items.Add("Tất cả");

            try
            {
                var loaiList = _dbContext.ThuVienMonAn
                    .Where(ma => ma.Loai != null && ma.Loai != "")
                    .Select(ma => ma.Loai)
                    .Distinct()
                    .OrderBy(loai => loai)
                    .ToList();

                foreach (var loai in loaiList)
                {
                    cboLoai.Items.Add(loai);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] LoadLoaiMonAnFromDatabase error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Lỗi khi load danh sách loại món ăn từ database: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (cboLoai.Items.Count > 0)
            {
                cboLoai.SelectedIndex = 0;
            }
        }

        private void ucQuanLyDinhDuong_Load(object sender, EventArgs e)
        {
            LoadStatistics();
            LoadMonAnList();
        }

        /// <summary>
        /// Load và hiển thị các thống kê về món ăn
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                // 1. Tổng số món ăn trong hệ thống
                int totalMonAn = _dbContext.ThuVienMonAn.Count();
                lbTongMonAn.Text = totalMonAn.ToString();

                // 2. Calo trung bình
                var monAnList = _dbContext.ThuVienMonAn
                    .Where(ma => ma.Calories != null)
                    .ToList();

                double averageCalo = 0;
                if (monAnList.Any())
                {
                    averageCalo = monAnList.Average(ma => ma.Calories ?? 0);
                }
                lbGenCaloTB.Text = averageCalo.ToString("F1");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] LoadStatistics error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load danh sách món ăn và hiển thị trong pnlDanhSachHuanLuyenVien
        /// </summary>
        private void LoadMonAnList()
        {
            LoadMonAnList(null);
        }

        /// <summary>
        /// Load danh sách món ăn với filter
        /// </summary>
        private void LoadMonAnList(MonAnFilter filter)
        {
            try
            {
                // Lưu filter hiện tại
                _currentFilter = filter;

                // Ẩn các card mẫu và xóa các item cũ
                HideTemplatePanels();
                ClearMonAnPanels();

                // Load danh sách món ăn từ database với filter
                _allMonAn = GetFilteredMonAn(filter);

                // Tính tổng số trang
                int totalItems = _allMonAn.Count;
                _totalPages = (int)Math.Ceiling(totalItems / (double)_pageSize);
                if (_totalPages <= 0) _totalPages = 1;

                // Reset về trang 1 mỗi khi load lại danh sách
                _currentPage = 1;

                // Load trang hiện tại
                LoadCurrentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách món ăn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] Error loading mon an list: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Xóa tất cả panel món ăn hiện có
        /// </summary>
        private void ClearMonAnPanels()
        {
            // Ẩn toàn bộ 4 card có sẵn; khi load trang sẽ bật lại những card có dữ liệu
            foreach (var panel in GetCardPanels())
            {
                panel.Visible = false;
            }
        }

        /// <summary>
        /// Ẩn các panel template
        /// </summary>
        private void HideTemplatePanels()
        {
            // Không cần ẩn template cố định nữa; việc hiển thị/ẩn đã được ClearMonAnPanels + CreateAndDisplayMonAnPanels xử lý
        }

        /// <summary>
        /// Lấy danh sách món ăn đã được filter
        /// </summary>
        private List<ThuVienMonAn> GetFilteredMonAn(MonAnFilter filter)
        {
            var query = _dbContext.ThuVienMonAn.AsQueryable();

            if (filter == null)
            {
                return query.OrderBy(ma => ma.TenMonAn).ToList();
            }

            // Filter theo tên tìm kiếm
            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                string searchLower = filter.SearchText.ToLower();
                query = query.Where(ma => 
                    (ma.TenMonAn != null && ma.TenMonAn.ToLower().Contains(searchLower)) ||
                    (ma.MonAnID != null && ma.MonAnID.ToLower().Contains(searchLower)));
            }

            // Filter theo loại
            if (!string.IsNullOrWhiteSpace(filter.Loai) && filter.Loai != "Tất cả")
            {
                query = query.Where(ma => ma.Loai != null && ma.Loai.Trim() == filter.Loai.Trim());
            }

            // Filter theo Calo (từ - đến)
            if (filter.CaloMin.HasValue)
            {
                query = query.Where(ma => ma.Calories >= filter.CaloMin.Value);
            }
            if (filter.CaloMax.HasValue)
            {
                query = query.Where(ma => ma.Calories <= filter.CaloMax.Value);
            }

            // Filter theo Protein (từ - đến)
            if (filter.ProteinMin.HasValue)
            {
                query = query.Where(ma => ma.Protein >= filter.ProteinMin.Value);
            }
            if (filter.ProteinMax.HasValue)
            {
                query = query.Where(ma => ma.Protein <= filter.ProteinMax.Value);
            }

            // Filter theo Fat (từ - đến)
            if (filter.FatMin.HasValue)
            {
                query = query.Where(ma => ma.Fat >= filter.FatMin.Value);
            }
            if (filter.FatMax.HasValue)
            {
                query = query.Where(ma => ma.Fat <= filter.FatMax.Value);
            }

            // Filter theo Carb (từ - đến)
            if (filter.CarbMin.HasValue)
            {
                query = query.Where(ma => ma.Carbs >= filter.CarbMin.Value);
            }
            if (filter.CarbMax.HasValue)
            {
                query = query.Where(ma => ma.Carbs <= filter.CarbMax.Value);
            }

            return query.OrderBy(ma => ma.TenMonAn).ToList();
        }

        /// <summary>
        /// Tạo và hiển thị các card món ăn trên 4 card có sẵn
        /// </summary>
        private void CreateAndDisplayMonAnPanels(List<ThuVienMonAn> monAnList)
        {
            var cards = GetCardPanels();
            int maxItems = Math.Min(monAnList.Count, cards.Count);

            // Gán dữ liệu cho từng card
            for (int i = 0; i < maxItems; i++)
            {
                var monAn = monAnList[i];
                var card = cards[i];
                BindMonAnToCard(card, monAn);
                card.Visible = true;
            }

            // Ẩn các card thừa ở trang cuối
            for (int i = maxItems; i < cards.Count; i++)
            {
                cards[i].Visible = false;
            }
        }

        /// <summary>
        /// Lấy danh sách đúng 4 card món ăn có sẵn trong pnDanhSachMonAn
        /// Thứ tự: trên trái, trên phải, dưới trái, dưới phải
        /// (theo tên control trong Designer: pnlMonAn1, guna2Panel1, guna2Panel3, guna2Panel7)
        /// </summary>
        private List<Control> GetCardPanels()
        {
            if (pnDanhSachMonAn == null)
                return new List<Control>();

            // Lấy theo tên để tránh phụ thuộc vào field trong partial class
            var nameOrder = new[] { "pnlMonAn1", "guna2Panel1", "guna2Panel3", "guna2Panel7" };

            var cards = pnDanhSachMonAn.Controls
                .Cast<Control>()
                .Where(c => nameOrder.Contains(c.Name))
                .OrderBy(c => Array.IndexOf(nameOrder, c.Name))
                .ToList();

            return cards;
        }

        /// <summary>
        /// Đổ dữ liệu món ăn vào 1 card có sẵn
        /// </summary>
        private void BindMonAnToCard(Control card, ThuVienMonAn monAn)
        {
            if (card == null || monAn == null) return;

            // Tỷ lệ để tính dinh dưỡng theo khối lượng thực tế
            double khoiLuong = monAn.KhoiLuongChuan ?? 100;
            string donVi = string.IsNullOrWhiteSpace(monAn.Donvi) ? "g" : monAn.Donvi;
            string khoiLuongText = $"{khoiLuong:G0}{donVi}";
            double khoiLuongChuan = monAn.KhoiLuongChuan ?? 100;
            if (khoiLuongChuan <= 0) khoiLuongChuan = 100;
            double tiLe = khoiLuong / khoiLuongChuan;

            double calories = (monAn.Calories ?? 0) * tiLe;
            double protein = (monAn.Protein ?? 0) * tiLe;
            double carbs = (monAn.Carbs ?? 0) * tiLe;
            double fat = (monAn.Fat ?? 0) * tiLe;
            double fiber = (monAn.Fiber ?? 0) * tiLe;

            // Gán vào đúng các label theo card.Name
            switch (card.Name)
            {
                case "pnlMonAn1":
                    SetMonAnTexts(
                        FindControl<Label>(card, "lblMonAn1"),
                        FindControl<Label>(card, "lblDonViKhoiLuongChuan"),
                        FindControl<Label>(card, "lblCalories"),
                        FindControl<Label>(card, "lblProtein"),
                        FindControl<Label>(card, "lblCarbs"),
                        FindControl<Label>(card, "lblFat"),
                        FindControl<Label>(card, "lblFiber"),
                        FindControl<Label>(card, "lblLoaiMonAn1"),
                        monAn, khoiLuongText, calories, protein, carbs, fat, fiber);
                    break;

                case "guna2Panel1":
                    SetMonAnTexts(
                        FindControl<Label>(card, "label8"),
                        FindControl<Label>(card, "label7"),
                        FindControl<Label>(card, "label6"),
                        FindControl<Label>(card, "label5"),
                        FindControl<Label>(card, "label4"),
                        FindControl<Label>(card, "label3"),
                        FindControl<Label>(card, "label2"),
                        FindControl<Label>(card, "label1"),
                        monAn, khoiLuongText, calories, protein, carbs, fat, fiber);
                    break;

                case "guna2Panel3":
                    SetMonAnTexts(
                        FindControl<Label>(card, "label16"),
                        FindControl<Label>(card, "label15"),
                        FindControl<Label>(card, "label14"),
                        FindControl<Label>(card, "label13"),
                        FindControl<Label>(card, "label12"),
                        FindControl<Label>(card, "label11"),
                        FindControl<Label>(card, "label10"),
                        FindControl<Label>(card, "label9"),
                        monAn, khoiLuongText, calories, protein, carbs, fat, fiber);
                    break;

                case "guna2Panel7":
                    SetMonAnTexts(
                        FindControl<Label>(card, "label32"),
                        FindControl<Label>(card, "label31"),
                        FindControl<Label>(card, "label30"),
                        FindControl<Label>(card, "label29"),
                        FindControl<Label>(card, "label28"),
                        FindControl<Label>(card, "label27"),
                        FindControl<Label>(card, "label26"),
                        FindControl<Label>(card, "label25"),
                        monAn, khoiLuongText, calories, protein, carbs, fat, fiber);
                    break;
            }

            // Gắn Tag + event cho nút Sửa / Xóa trong card
            var btnEdit = FindControl<Guna.UI2.WinForms.Guna2GradientButton>(card, "btnSua")
                          ?? FindControl<Guna.UI2.WinForms.Guna2GradientButton>(card, "guna2GradientButton2")
                          ?? FindControl<Guna.UI2.WinForms.Guna2GradientButton>(card, "guna2GradientButton5")
                          ?? FindControl<Guna.UI2.WinForms.Guna2GradientButton>(card, "guna2GradientButton8");

            var btnDelete = FindControl<Guna.UI2.WinForms.Guna2GradientButton>(card, "btnXoa")
                            ?? FindControl<Guna.UI2.WinForms.Guna2GradientButton>(card, "guna2GradientButton1")
                            ?? FindControl<Guna.UI2.WinForms.Guna2GradientButton>(card, "guna2GradientButton4")
                            ?? FindControl<Guna.UI2.WinForms.Guna2GradientButton>(card, "guna2GradientButton9");

            if (btnEdit != null)
            {
                btnEdit.Tag = monAn.MonAnID;
                // Tránh gắn trùng nhiều lần
                btnEdit.Click -= BtnSua_Click;
                btnEdit.Click += BtnSua_Click;
            }

            if (btnDelete != null)
            {
                btnDelete.Tag = monAn.MonAnID;
                btnDelete.Click -= BtnXoa_Click;
                btnDelete.Click += BtnXoa_Click;
            }
        }

        /// <summary>
        /// Lấy tất cả label trong 1 control (đệ quy)
        /// </summary>
        private List<Label> GetAllLabels(Control parent)
        {
            var result = new List<Label>();
            foreach (Control c in parent.Controls)
            {
                if (c is Label lbl)
                {
                    result.Add(lbl);
                }

                if (c.HasChildren)
                {
                    result.AddRange(GetAllLabels(c));
                }
            }
            return result;
        }

        /// <summary>
        /// Gán toàn bộ text cho 1 card món ăn
        /// </summary>
        private void SetMonAnTexts(
            Label tenLabel,
            Label weightLabel,
            Label caloLabel,
            Label proteinLabel,
            Label carbsLabel,
            Label fatLabel,
            Label fiberLabel,
            Label loaiLabel,
            ThuVienMonAn monAn,
            string khoiLuongText,
            double calories,
            double protein,
            double carbs,
            double fat,
            double fiber)
        {
            if (tenLabel != null)
            {
                tenLabel.Text = string.IsNullOrWhiteSpace(monAn.TenMonAn)
                    ? (monAn.MonAnID ?? "N/A")
                    : monAn.TenMonAn.Trim();
            }

            if (weightLabel != null)
            {
                weightLabel.Text = khoiLuongText;
            }

            if (caloLabel != null)
            {
                caloLabel.Text = $"{calories:F0} kcal";
            }

            if (proteinLabel != null)
            {
                proteinLabel.Text = $"{protein:F1}g protein";
            }

            if (carbsLabel != null)
            {
                carbsLabel.Text = $"{carbs:F1}g carbs";
            }

            if (fatLabel != null)
            {
                fatLabel.Text = $"{fat:F1}g fat";
            }

            if (fiberLabel != null)
            {
                fiberLabel.Text = $"{fiber:F1}g chất xơ";
            }

            if (loaiLabel != null)
            {
                loaiLabel.Text = string.IsNullOrWhiteSpace(monAn.Loai)
                    ? loaiLabel.Text
                    : monAn.Loai.Trim();
            }
        }

        /// <summary>
        /// Xử lý khi click vào panel món ăn để hiển thị chi tiết
        /// </summary>
        private void MonAnPanel_Click(ThuVienMonAn monAn)
        {
            _selectedMonAn = monAn;
            LoadMonAnDetails(monAn.MonAnID);
        }

        /// <summary>
        /// Load và hiển thị món ăn của trang hiện tại
        /// </summary>
        private void LoadCurrentPage()
        {
            try
            {
                if (_allMonAn == null) _allMonAn = new List<ThuVienMonAn>();

                // Đảm bảo currentPage nằm trong khoảng hợp lệ
                if (_currentPage < 1) _currentPage = 1;
                if (_currentPage > _totalPages) _currentPage = _totalPages;

                // Xóa các item hiện có và ẩn template
                ClearMonAnPanels();
                HideTemplatePanels();

                if (!_allMonAn.Any())
                {
                    UpdatePaginationUI();
                    return;
                }

                int skip = (_currentPage - 1) * _pageSize;
                var pageMonAn = _allMonAn
                    .Skip(skip)
                    .Take(_pageSize)
                    .ToList();

                CreateAndDisplayMonAnPanels(pageMonAn);
                UpdatePaginationUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải trang món ăn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] LoadCurrentPage error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load thông tin chi tiết của món ăn vào panel bên cạnh
        /// </summary>
        private void LoadMonAnDetails(string monAnID)
        {
            try
            {
                var monAn = _dbContext.ThuVienMonAn.FirstOrDefault(ma => ma.MonAnID == monAnID);
                if (monAn == null)
                {
                    ClearMonAnDetails();
                    return;
                }

                // Các control hiển thị chi tiết không còn tồn tại trong Designer
                // Đã bị xóa hoặc không được sử dụng nữa
                // lblTenBT.Text = monAn.TenMonAn ?? "N/A";
                // lbNhomCo.Text = $"Cal: {monAn.Calories?.ToString("F1") ?? "N/A"}";
                // lbDungcutap.Text = $"Protein: {monAn.Protein?.ToString("F1") ?? "N/A"}";
                // lbDoKho.Text = $"Fat: {monAn.Fat?.ToString("F1") ?? "N/A"}";
                // lbLuongCaloTieuHao.Text = $"Carb: {monAn.Carbs?.ToString("F1") ?? "N/A"}";

                // Load ảnh món ăn - control picAnhMonAn không còn tồn tại
                // if (!string.IsNullOrWhiteSpace(monAn.imageURL))
                // {
                //     try
                //     {
                //         if (Uri.IsWellFormedUriString(monAn.imageURL, UriKind.Absolute))
                //         {
                //             picAnhMonAn.LoadAsync(monAn.imageURL);
                //         }
                //         else if (System.IO.File.Exists(monAn.imageURL))
                //         {
                //             picAnhMonAn.Image = System.Drawing.Image.FromFile(monAn.imageURL);
                //         }
                //     }
                //     catch
                //     {
                //         // Nếu URL không hợp lệ, giữ mặc định
                //     }
                // }

                // Hiển thị panel chi tiết - panel không còn tồn tại
                // pnlDanhSachDanhGia.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thông tin chi tiết: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] LoadMonAnDetails error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Xóa thông tin chi tiết
        /// </summary>
        private void ClearMonAnDetails()
        {
            // Các control hiển thị chi tiết không còn tồn tại trong Designer
            // lblTenBT.Text = "N/A";
            // lbNhomCo.Text = "Cal: N/A";
            // lbDungcutap.Text = "Protein: N/A";
            // lbDoKho.Text = "Fat: N/A";
            // lbLuongCaloTieuHao.Text = "Carb: N/A";
            // pnlDanhSachDanhGia.Visible = false;
        }

        /// <summary>
        /// Clone panel và tất cả controls bên trong
        /// </summary>
        private Guna.UI2.WinForms.Guna2CustomGradientPanel ClonePanel(Guna.UI2.WinForms.Guna2CustomGradientPanel sourcePanel)
        {
            // Không còn sử dụng clone panel trong phiên bản hiện tại
            return new Guna.UI2.WinForms.Guna2CustomGradientPanel();
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
                    BackColor = sourceLabel.BackColor,
                    Anchor = AnchorStyles.None,
                    Dock = DockStyle.None
                };
            }
            else if (source is System.Windows.Forms.PictureBox)
            {
                var sourcePic = source as System.Windows.Forms.PictureBox;
                cloned = new System.Windows.Forms.PictureBox
                {
                    Name = sourcePic.Name,
                    Location = sourcePic.Location,
                    Size = sourcePic.Size,
                    SizeMode = sourcePic.SizeMode,
                    BackColor = sourcePic.BackColor,
                    Anchor = AnchorStyles.None,
                    Dock = DockStyle.None
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
                    Font = sourceBtn.Font,
                    Anchor = AnchorStyles.None,
                    Dock = DockStyle.None
                };
            }
            else if (source is Guna.UI2.WinForms.Guna2CustomGradientPanel)
            {
                var sourcePanel = source as Guna.UI2.WinForms.Guna2CustomGradientPanel;
                cloned = ClonePanel(sourcePanel);
                if (cloned != null)
                {
                    cloned.Anchor = AnchorStyles.None;
                    cloned.Dock = DockStyle.None;
                }
            }

            return cloned ?? new Control();
        }

        /// <summary>
        /// Tìm control trong panel theo tên
        /// </summary>
        private T FindControl<T>(Control parent, string name) where T : Control
        {
            foreach (Control control in parent.Controls)
            {
                if (control.Name == name && control is T)
                {
                    return control as T;
                }

                var found = FindControl<T>(control, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        /// <summary>
        /// Set text cho control trong panel
        /// </summary>
        private void SetControlText(Control parent, string controlName, string text)
        {
            var control = FindControl<Guna.UI2.WinForms.Guna2HtmlLabel>(parent, controlName);
            if (control != null)
            {
                control.Text = text;
            }
        }

        /// <summary>
        /// Xử lý placeholder cho txtTiemKiem
        /// </summary>
        private void TxtTiemKiem_Enter(object sender, EventArgs e)
        {
            if (txtTiemKiem.Text == "Tìm kiếm..")
            {
                txtTiemKiem.Text = "";
                txtTiemKiem.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void TxtTiemKiem_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTiemKiem.Text))
            {
                txtTiemKiem.Text = "Tìm kiếm..";
                txtTiemKiem.ForeColor = System.Drawing.Color.Gray;
            }
        }

        /// <summary>
        /// Xử lý khi click nút Áp dụng
        /// </summary>
        private void BtnApDung_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy text tìm kiếm
                string searchText = txtTiemKiem.Text;
                if (searchText == "Tìm kiếm..")
                {
                    searchText = "";
                }

                // Parse các giá trị filter
                double? caloMin = ParseDouble(guna2TextBox1?.Text);
                double? caloMax = ParseDouble(guna2TextBox1?.Text); // Có thể cần 2 textbox riêng
                double? proteinMin = ParseDouble(guna2TextBox2?.Text);
                double? proteinMax = ParseDouble(guna2TextBox2?.Text);
                double? fatMin = ParseDouble(guna2TextBox3?.Text);
                double? fatMax = ParseDouble(guna2TextBox3?.Text);
                double? carbMin = ParseDouble(guna2TextBox4?.Text);
                double? carbMax = ParseDouble(guna2TextBox4?.Text);

                // Tạo filter
                var filter = new MonAnFilter
                {
                    SearchText = searchText,
                    Loai = cboLoai.SelectedItem?.ToString() ?? "",
                    CaloMin = caloMin,
                    CaloMax = caloMax,
                    ProteinMin = proteinMin,
                    ProteinMax = proteinMax,
                    FatMin = fatMin,
                    FatMax = fatMax,
                    CarbMin = carbMin,
                    CarbMax = carbMax
                };

                // Lưu filter hiện tại
                _currentFilter = filter;

                // Load lại danh sách với filter (reset về trang 1)
                LoadMonAnList(filter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng filter: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi click nút Đặt lại
        /// </summary>
        private void BtnDatLai_Click(object sender, EventArgs e)
        {
            txtTiemKiem.Text = "Tìm kiếm..";
            txtTiemKiem.ForeColor = System.Drawing.Color.Gray;
            cboLoai.SelectedIndex = 0;
            guna2TextBox1.Text = "";
            guna2TextBox2.Text = "";
            guna2TextBox3.Text = "";
            guna2TextBox4.Text = "";
            _currentFilter = null; // Reset filter
            _currentPage = 1;
            LoadMonAnList();
        }

        /// <summary>
        /// Cập nhật UI phân trang (số trang, enable/disable nút)
        /// </summary>
        private void UpdatePaginationUI()
        {
            if (lblSoTrang != null)
            {
                lblSoTrang.Text = $"{_currentPage}/{_totalPages}";
            }

            if (btnPrevious != null)
            {
                btnPrevious.Enabled = _currentPage > 1;
            }

            if (btnNext != null)
            {
                btnNext.Enabled = _currentPage < _totalPages;
            }
        }

        /// <summary>
        /// Xử lý khi click nút Sửa
        /// </summary>
        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                string monAnID = GetSelectedMonAnID(sender);
                if (string.IsNullOrWhiteSpace(monAnID))
                {
                    MessageBox.Show("Vui lòng chọn món ăn cần sửa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ShowSuaMonAn(monAnID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa món ăn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] BtnSua_Click error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Xử lý khi click nút Xóa
        /// </summary>
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                string monAnID = GetSelectedMonAnID(sender);
                if (string.IsNullOrWhiteSpace(monAnID))
                {
                    MessageBox.Show("Vui lòng chọn món ăn cần xóa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!DeleteMonAn(monAnID))
                {
                    return; // Đã có thông báo lỗi trong method
                }

                // Refresh Entity Framework context
                RefreshDbContext();
                // Reload sau khi xóa thành công
                LoadMonAnList(_currentFilter);
                LoadStatistics();
                ClearMonAnDetails();
                _selectedMonAn = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa món ăn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] BtnXoa_Click error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Xử lý khi click nút Thêm mới món ăn
        /// </summary>
        private void BtnThemMoi_Click(object sender, EventArgs e)
        {
            try
            {
                ShowThemMonAn();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thêm mới: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi click nút Next (sang trang tiếp theo)
        /// </summary>
        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadCurrentPage();
            }
        }

        /// <summary>
        /// Xử lý khi click nút Previous (về trang trước)
        /// </summary>
        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadCurrentPage();
            }
        }

        /// <summary>
        /// Hiển thị Form thêm món ăn
        /// </summary>
        private void ShowThemMonAn()
        {
            try
            {
                using (var form = new ThemMonAn())
                {
                    if (form.ShowDialog() == DialogResult.OK && form.IsSaved)
                    {
                        // Refresh Entity Framework context để lấy dữ liệu mới
                        RefreshDbContext();
                        // Reload danh sách và thống kê sau khi thêm thành công
                        LoadMonAnList(_currentFilter);
                        LoadStatistics();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thêm món ăn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] ShowThemMonAn error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Hiển thị Form sửa món ăn
        /// </summary>
        private void ShowSuaMonAn(string monAnID)
        {
            try
            {
                var monAn = _dbContext.ThuVienMonAn.FirstOrDefault(ma => ma.MonAnID == monAnID);
                if (monAn == null)
                {
                    MessageBox.Show("Không tìm thấy món ăn!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var form = new SuaMonAn(monAn))
                {
                    if (form.ShowDialog() == DialogResult.OK && form.IsSaved)
                    {
                        // Refresh Entity Framework context để lấy dữ liệu mới
                        RefreshDbContext();
                        // Reload danh sách và thống kê sau khi sửa thành công
                        LoadMonAnList(_currentFilter);
                        LoadStatistics();
                        
                        // Cập nhật chi tiết nếu đang hiển thị món ăn này
                        if (_selectedMonAn != null && _selectedMonAn.MonAnID == monAnID)
                        {
                            LoadMonAnDetails(monAnID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form sửa món ăn: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] ShowSuaMonAn error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Ẩn overlay panel
        /// </summary>
        private void HideOverlay()
        {
            _overlayPanel.Visible = false;
            _overlayPanel.Controls.Clear();
        }

        /// <summary>
        /// Lấy ID món ăn được chọn từ sender hoặc _selectedMonAn
        /// </summary>
        private string GetSelectedMonAnID(object sender)
        {
            if (sender is Guna.UI2.WinForms.Guna2GradientButton btn && btn.Tag != null)
            {
                return btn.Tag.ToString();
            }
            
            if (_selectedMonAn != null)
            {
                return _selectedMonAn.MonAnID;
            }

            return null;
        }

        /// <summary>
        /// Xóa món ăn với validation
        /// </summary>
        private bool DeleteMonAn(string monAnID)
        {
            var monAn = _dbContext.ThuVienMonAn.FirstOrDefault(ma => ma.MonAnID == monAnID);
            if (monAn == null)
            {
                MessageBox.Show("Không tìm thấy món ăn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra xem món ăn có đang được sử dụng không
            if (IsMonAnInUse(monAnID))
            {
                MessageBox.Show("Món ăn này đang được sử dụng, không thể xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Xác nhận xóa
            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa món ăn \"{monAn.TenMonAn}\"?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return false;
            }

            // Thực hiện xóa
            _dbContext.ThuVienMonAn.Remove(monAn);
            _dbContext.SaveChanges();

            MessageBox.Show("Xóa món ăn thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            return true;
        }

        /// <summary>
        /// Kiểm tra xem món ăn có đang được sử dụng không
        /// </summary>
        private bool IsMonAnInUse(string monAnID)
        {
            return _dbContext.BuaAnChiTiet.Any(bact => bact.MonAnID == monAnID);
        }

        /// <summary>
        /// Parse string to double
        /// </summary>
        private double? ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            
            if (double.TryParse(value, out double result))
                return result;
            
            return null;
        }


        /// <summary>
        /// Refresh Entity Framework context để lấy dữ liệu mới từ database
        /// </summary>
        private void RefreshDbContext()
        {
            try
            {
                // Dispose context cũ
                _dbContext?.Dispose();
                // Tạo context mới
                _dbContext = new WF_HealthTracker();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyDinhDuong] RefreshDbContext error: {ex.Message}");
            }
        }

        /// <summary>
        /// Class để lưu filter
        /// </summary>
        private class MonAnFilter
        {
            public string SearchText { get; set; }
            public string Loai { get; set; }
            public double? CaloMin { get; set; }
            public double? CaloMax { get; set; }
            public double? ProteinMin { get; set; }
            public double? ProteinMax { get; set; }
            public double? FatMin { get; set; }
            public double? FatMax { get; set; }
            public double? CarbMin { get; set; }
            public double? CarbMax { get; set; }
        }
    }
}
