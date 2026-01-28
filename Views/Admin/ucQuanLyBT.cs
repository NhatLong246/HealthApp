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
    public partial class ucQuanLyBT : UserControl
    {
        private const int FixedUcHeight = 1355; // Giữ nguyên chiều cao ucQuanLyBT khi chuyển trang

        private WF_HealthTracker _dbContext;
        private Panel _overlayPanel; // Panel để hiển thị ucThemBT
        private ExerciseFilter _currentFilter; // Lưu filter hiện tại
        
        // Phân trang
        private const int _pageSize = 6; // Số bài tập mỗi trang
        private int _currentPage = 1; // Trang hiện tại
        private int _totalPages = 1; // Tổng số trang
        private List<ThuVienBaiTap> _allExercises = new List<ThuVienBaiTap>(); // Lưu tất cả bài tập đã filter

        public ucQuanLyBT()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            this.Load += ucQuanLyBT_Load;
            InitializeFilters();
            InitializeOverlayPanel();

            // Căn lại layout để tránh khoảng trắng quá lớn khi hiển thị
            this.SizeChanged += (_, __) => Relayout();

            // Giữ nguyên chiều cao của UserControl theo yêu cầu
            EnsureFixedUcSize();
            
            // Set size cố định cho panel và không thay đổi vị trí
            // Chiều rộng bằng với pnlChucNang (1001px), height giữ nguyên 856px
            if (pnlDanhSachHuanLuyenVien != null && pnlChucNang != null)
            {
                int panelWidth = pnlChucNang.Width; // Lấy width từ pnlChucNang (1001px)
                pnlDanhSachHuanLuyenVien.Size = new Size(panelWidth, 856);
                pnlDanhSachHuanLuyenVien.AutoScroll = false;
                // Không thay đổi vị trí, giữ nguyên vị trí ban đầu từ Designer
            }
            
            // Đảm bảo UserControl có thể scroll
            this.AutoScroll = true;
            this.HorizontalScroll.Visible = false;
            this.HorizontalScroll.Enabled = false;
            
            // Đảm bảo scroll được bật ngay từ đầu
            this.VerticalScroll.Visible = true;
            this.VerticalScroll.Enabled = true;

            // Căn giữa phân trang trong pnlDanhSachHuanLuyenVien
            LayoutPaginationControls();
        }

        private void EnsureFixedUcSize()
        {
            // Luôn giữ chiều cao cố định 1355 (không bị co lại khi chuyển trang / relayout)
            // Không ép width để tránh ảnh hưởng layout của parent
            if (this.Height != FixedUcHeight)
            {
                this.Height = FixedUcHeight;
            }

            // Khóa min/max height để tránh bị thay đổi bởi layout engine
            // Width để int.MaxValue để không giới hạn theo chiều ngang
            this.MinimumSize = new Size(this.MinimumSize.Width, FixedUcHeight);
            this.MaximumSize = new Size(int.MaxValue, FixedUcHeight);
        }

        /// <summary>
        /// Căn lại vị trí panel danh sách để không bị cách quá xa / lệch.
        /// </summary>
        private void Relayout()
        {
            EnsureFixedUcSize();

            // Không thay đổi vị trí và size của panel, giữ nguyên vị trí ban đầu từ Designer
            // Chiều rộng bằng với pnlChucNang
            if (pnlDanhSachHuanLuyenVien != null && pnlChucNang != null)
            {
                int panelWidth = pnlChucNang.Width; // Lấy width từ pnlChucNang
                pnlDanhSachHuanLuyenVien.Size = new Size(panelWidth, 856);
                // Không thay đổi vị trí (Location) - giữ nguyên từ Designer
            }
            
            // Đảm bảo scroll vẫn hoạt động sau khi relayout
            this.AutoScroll = true;
            this.VerticalScroll.Visible = true;
            this.VerticalScroll.Enabled = true;
            LayoutPaginationControls();
            UpdateAutoScrollMinSize();
        }

        /// <summary>
        /// Khởi tạo panel overlay để hiển thị ucThemBT hoặc ucSuaBT
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
            
            // Event handlers cho pagination
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
                // Load nhóm cơ từ database
                LoadNhomCoFromDatabase();

                // Load độ khó từ database
                LoadDoKhoFromDatabase();

                // Load thiết bị từ database
                LoadThietBiFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu filter: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] LoadFilterDataFromDatabase error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load danh sách nhóm cơ chính từ database
        /// </summary>
        private void LoadNhomCoFromDatabase()
        {
            cboNhomCo.Items.Clear();
            cboNhomCo.Items.Add("Tất cả");

            try
            {
                // Lấy tất cả nhóm cơ chính từ database
                // LƯU Ý: Không dùng IsNullOrWhiteSpace trong LINQ to Entities
                var nhomCoChinh = _dbContext.ThuVienBaiTap
                    .Where(bt => bt.NhomCoChinhNhat != null && bt.NhomCoChinhNhat != "")
                    .Select(bt => bt.NhomCoChinhNhat)
                    .Distinct()
                    .OrderBy(nc => nc)
                    .ToList();

                // Thêm vào combobox
                foreach (var nc in nhomCoChinh)
                {
                    cboNhomCo.Items.Add(nc);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] LoadNhomCoFromDatabase error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Lỗi khi load danh sách nhóm cơ từ database: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            cboNhomCo.SelectedIndex = 0;
        }

        /// <summary>
        /// Load danh sách độ khó từ database
        /// </summary>
        private void LoadDoKhoFromDatabase()
        {
            cboDiaChi.Items.Clear();
            cboDiaChi.Items.Add("Tất cả");

            try
            {
                // Lấy tất cả độ khó từ database
                // LƯU Ý: Không dùng IsNullOrWhiteSpace trong LINQ to Entities
                var doKhoList = _dbContext.ThuVienBaiTap
                    .Where(bt => bt.CapDo != null && bt.CapDo != "")
                    .Select(bt => bt.CapDo)
                    .Distinct()
                    .OrderBy(dk => dk)
                    .ToList();

                foreach (var dk in doKhoList)
                {
                    cboDiaChi.Items.Add(dk);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] LoadDoKhoFromDatabase error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Lỗi khi load danh sách độ khó từ database: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            cboDiaChi.SelectedIndex = 0;
        }

        /// <summary>
        /// Load danh sách thiết bị từ database
        /// </summary>
        private void LoadThietBiFromDatabase()
        {
            cboThietBi.Items.Clear();
            cboThietBi.Items.Add("Tất cả");

            try
            {
                // Lấy tất cả thiết bị từ database
                // LƯU Ý: Không dùng IsNullOrWhiteSpace trong LINQ to Entities
                var thietBiRaw = _dbContext.ThuVienBaiTap
                    .Where(bt => bt.DungCu != null && bt.DungCu != "")
                    .Select(bt => bt.DungCu)
                    .ToList();

                // Tách các thiết bị (có thể chứa nhiều thiết bị phân cách bởi dấu + hoặc dấu phẩy)
                var thietBiList = new List<string>();
                foreach (var tbRaw in thietBiRaw)
                {
                    if (string.IsNullOrWhiteSpace(tbRaw))
                        continue;

                    // Tách theo dấu + hoặc dấu phẩy
                    var parts = tbRaw.Split(new[] { '+', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        var trimmed = part.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmed))
                        {
                            thietBiList.Add(trimmed);
                        }
                    }
                }

                // Loại bỏ trùng lặp và sắp xếp
                var uniqueThietBi = thietBiList
                    .Distinct()
                    .OrderBy(tb => tb)
                    .ToList();

                // Thêm vào combobox
                foreach (var tb in uniqueThietBi)
                {
                    cboThietBi.Items.Add(tb);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] LoadThietBiFromDatabase error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Lỗi khi load danh sách thiết bị từ database: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (cboThietBi.Items.Count > 0)
            {
                cboThietBi.SelectedIndex = 0;
            }
        }

        private void ucQuanLyBT_Load(object sender, EventArgs e)
        {
            EnsureFixedUcSize();
            Relayout();
            LoadStatistics();
            LoadExerciseList();
        }

        /// <summary>
        /// Load và hiển thị các thống kê về bài tập
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                // 1. Tổng số bài tập trong hệ thống
                int totalExercises = _dbContext.ThuVienBaiTap.Count();
                lbTongBT.Text = totalExercises.ToString();

                // 2. Độ khó trung bình (tính từ CapDo)
                var exercises = _dbContext.ThuVienBaiTap
                    .Where(bt => bt.CapDo != null)
                    .ToList();

                double averageDifficulty = 0;
                if (exercises.Any())
                {
                    var difficultyValues = exercises.Select(bt =>
                    {
                        switch (bt.CapDo)
                        {
                            case "Beginner":
                                return 1.0;
                            case "Intermediate":
                                return 3.0;
                            case "Advanced":
                                return 5.0;
                            case "All Levels":
                                return 3.0;
                            default:
                                return 3.0;
                        }
                    }).ToList();

                    averageDifficulty = difficultyValues.Any() ? difficultyValues.Average() : 0.0;
                }
                lbGenDoKhoTB.Text = averageDifficulty.ToString("F1");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] LoadStatistics error: {ex.Message}\n{ex.StackTrace}");
            }
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

                // Tạo filter và lưu lại
                var filter = new ExerciseFilter
                {
                    SearchText = searchText,
                    NhomCo = cboNhomCo.SelectedItem?.ToString() ?? "",
                    DoKho = cboDiaChi.SelectedItem?.ToString() ?? "",
                    ThietBi = cboThietBi.SelectedItem?.ToString() ?? ""
                };

                // Lưu filter hiện tại
                _currentFilter = filter;
                // Load lại danh sách với filter
                LoadExerciseList(filter);
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
            cboNhomCo.SelectedIndex = 0;
            cboDiaChi.SelectedIndex = 0;
            cboThietBi.SelectedIndex = 0;
            _currentFilter = null; // Reset filter
            LoadExerciseList(); // Load lại danh sách
        }

        private void BtnThemMoi_Click(object sender, EventArgs e)
        {
            try
            {
                ShowThemBT();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form thêm mới: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hiển thị UserControl thêm bài tập
        /// </summary>
        private void ShowThemBT()
        {
            _overlayPanel.Controls.Clear();
            
            var ucThem = new ucThemBT();
            ucThem.Dock = DockStyle.Fill;
            
            // Đăng ký event khi thêm thành công
            ucThem.OnSaveSuccess += (s, e) =>
            {
                HideOverlay();
                // Refresh Entity Framework context để lấy dữ liệu mới
                RefreshDbContext();
                LoadStatistics();
                LoadFilterDataFromDatabase(); // Reload filters nếu có dữ liệu mới
                LoadExerciseList(_currentFilter); // Reload danh sách
            };
            
            // Đăng ký event khi hủy
            ucThem.OnCancel += (s, e) =>
            {
                HideOverlay();
            };
            
            _overlayPanel.Controls.Add(ucThem);
            _overlayPanel.Visible = true;
            _overlayPanel.BringToFront();
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
        /// Dispose resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                _dbContext?.Dispose();
            }
            base.Dispose(disposing);
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
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] RefreshDbContext error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load danh sách bài tập và hiển thị trong panel container
        /// </summary>
        private void LoadExerciseList()
        {
            LoadExerciseList(null);
        }

        /// <summary>
        /// Load danh sách bài tập với filter
        /// </summary>
        private void LoadExerciseList(ExerciseFilter filter)
        {
            try
            {
                // Reset về trang 1 khi filter thay đổi
                _currentPage = 1;
                
                // Xóa tất cả panel bài tập hiện có
                ClearExercisePanels();

                // Load danh sách bài tập từ database với filter
                _allExercises = GetFilteredExercises(filter);

                if (!_allExercises.Any())
                {
                    UpdatePaginationUI();
                    return;
                }

                // Tính tổng số trang
                _totalPages = (int)Math.Ceiling((double)_allExercises.Count / _pageSize);
                if (_totalPages == 0) _totalPages = 1;

                // Load trang hiện tại
                LoadCurrentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] Error loading exercise list: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load và hiển thị bài tập của trang hiện tại
        /// </summary>
        private void LoadCurrentPage()
        {
            try
            {
                EnsureFixedUcSize();

                // Reset scroll về đầu TRƯỚC KHI xóa và load lại
                // Điều này đảm bảo vị trí được tính từ đầu panel
                this.AutoScrollPosition = new Point(0, 0);
                if (pnlDanhSachHuanLuyenVien.AutoScroll)
                {
                    pnlDanhSachHuanLuyenVien.AutoScrollPosition = new Point(0, 0);
                }
                this.PerformLayout();
                Application.DoEvents();
                
                // Xóa tất cả panel bài tập hiện có
                ClearExercisePanels();

                // Lấy bài tập của trang hiện tại
                int skip = (_currentPage - 1) * _pageSize;
                var currentPageExercises = _allExercises.Skip(skip).Take(_pageSize).ToList();

                if (!currentPageExercises.Any())
                {
                    UpdatePaginationUI();
                    ResetScrollToTop();
                    return;
                }

                // Tạo và hiển thị các panel bài tập
                CreateAndDisplayExercisePanels(currentPageExercises);
                
                // Cập nhật UI phân trang
                UpdatePaginationUI();
                
                // Reset scroll về đầu sau khi tất cả đã được cập nhật
                ResetScrollToTop();

                EnsureFixedUcSize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải trang: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] Error loading current page: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Reset scroll position về đầu
        /// </summary>
        private void ResetScrollToTop()
        {
            // Reset scroll của UserControl về đầu
            this.AutoScrollPosition = new Point(0, 0);
            
            // Đảm bảo panel cũng scroll về đầu nếu có
            if (pnlDanhSachHuanLuyenVien.AutoScroll)
            {
                pnlDanhSachHuanLuyenVien.AutoScrollPosition = new Point(0, 0);
            }
            
            // Force refresh layout trước
            this.PerformLayout();
            Application.DoEvents(); // Cho phép UI cập nhật
            
            // Sau đó scroll đến control đầu tiên nếu có (để đảm bảo không có khoảng trống)
            if (lbDanhSachBT != null && pnlDanhSachHuanLuyenVien.Controls.Contains(lbDanhSachBT))
            {
                // Scroll đến label để đảm bảo không có khoảng trống ở đầu
                pnlDanhSachHuanLuyenVien.ScrollControlIntoView(lbDanhSachBT);
                // Reset lại sau khi scroll
                this.AutoScrollPosition = new Point(0, 0);
            }
            
            // Force refresh layout lần cuối
            this.PerformLayout();
        }

        /// <summary>
        /// Cập nhật UI phân trang (số trang, enable/disable buttons)
        /// </summary>
        private void UpdatePaginationUI()
        {
            // Hiển thị số trang dạng "trang hiện tại/tổng trang"
            if (lblSoTrang != null)
            {
                lblSoTrang.Text = $"{_currentPage}/{_totalPages}";
            }

            // Căn giữa và đảm bảo hiển thị
            LayoutPaginationControls();

            // Enable/disable nút Previous
            btnPrevious.Enabled = _currentPage > 1;

            // Enable/disable nút Next
            btnNext.Enabled = _currentPage < _totalPages;
            
            // Cập nhật AutoScrollMinSize sau khi cập nhật pagination
            UpdateAutoScrollMinSize();
        }

        private void LayoutPaginationControls()
        {
            if (pnlDanhSachHuanLuyenVien == null) return;
            if (btnPrevious == null || btnNext == null || lblSoTrang == null) return;

            // Luôn hiển thị
            btnPrevious.Visible = true;
            btnNext.Visible = true;
            lblSoTrang.Visible = true;

            btnPrevious.Anchor = AnchorStyles.None;
            btnNext.Anchor = AnchorStyles.None;
            lblSoTrang.Anchor = AnchorStyles.None;

            // Vị trí Y cố định trong panel (theo designer)
            const int yButtons = 816;

            // Tính kích thước label theo text hiện tại
            var labelText = string.IsNullOrWhiteSpace(lblSoTrang.Text) ? "1/1" : lblSoTrang.Text;
            var labelSize = TextRenderer.MeasureText(labelText, lblSoTrang.Font);
            int labelWidth = Math.Max(lblSoTrang.Width, labelSize.Width);
            int labelHeight = Math.Max(lblSoTrang.Height, labelSize.Height);

            const int spacing = 16;
            int totalWidth = btnPrevious.Width + spacing + labelWidth + spacing + btnNext.Width;

            int containerWidth = pnlDanhSachHuanLuyenVien.ClientSize.Width;
            if (containerWidth <= 0) containerWidth = pnlDanhSachHuanLuyenVien.Width;

            int startX = Math.Max(0, (containerWidth - totalWidth) / 2);

            btnPrevious.Location = new Point(startX, yButtons);
            lblSoTrang.Location = new Point(startX + btnPrevious.Width + spacing, yButtons + (btnPrevious.Height - labelHeight) / 2);
            btnNext.Location = new Point(startX + btnPrevious.Width + spacing + labelWidth + spacing, yButtons);

            // Đảm bảo không bị che
            btnPrevious.BringToFront();
            lblSoTrang.BringToFront();
            btnNext.BringToFront();
        }
        
        /// <summary>
        /// Cập nhật AutoScrollMinSize để UserControl có thể scroll
        /// </summary>
        private void UpdateAutoScrollMinSize()
        {
            if (pnlDanhSachHuanLuyenVien == null) return;
            
            // Đảm bảo panel luôn giữ kích thước cố định
            if (pnlChucNang != null)
            {
                int panelWidth = pnlChucNang.Width;
                pnlDanhSachHuanLuyenVien.Size = new Size(panelWidth, 856);
            }
            
            // Tìm vị trí thấp nhất của tất cả controls trong panel (bao gồm cả nút phân trang)
            int maxBottom = 0;
            
            // Tìm vị trí thấp nhất của các bài tập và các controls khác
            foreach (Control ctrl in pnlDanhSachHuanLuyenVien.Controls)
            {
                if (ctrl.Visible)
                {
                    int bottom = ctrl.Top + ctrl.Height;
                    if (bottom > maxBottom)
                        maxBottom = bottom;
                }
            }
            
            // Đảm bảo tính cả nút phân trang
            // Nút phân trang ở vị trí Y = 816, height = 36 => bottom = 852
            // Panel có height = 856, nên nút phân trang nằm gần cuối panel
            // Để đảm bảo scroll được xuống cuối và thấy hết nút phân trang, cần tính đúng
            int paginationBottom = 816 + 36; // Vị trí thấp nhất của nút phân trang (từ đầu panel)
            if (paginationBottom > maxBottom)
                maxBottom = paginationBottom;
            
            // Tính chiều cao tối thiểu cho UserControl (tuyệt đối)
            // Panel có height cố định 856, vị trí Top từ Designer = 457 => bottom = 457 + 856 = 1313
            // Cần AutoScrollMinSize LỚN HƠN chiều cao ucQuanLyBT (1355) thì mới scroll xuống hết được
            int panelTop = pnlDanhSachHuanLuyenVien.Top;
            int contentBottom = panelTop + 856; // 457 + 856 = 1313
            // Vùng cuộn phải đủ lớn để kéo xuống thấy hết nút phân trang (bottom ~1313)
            // Đặt chiều cao nội dung = bottom của panel + thêm nhiều pixel để scroll xuống hết
            int minScrollableHeight = this.Height + 580; // Ví dụ: 1355 + 400 = 1755 để scroll thoải mái
            int requiredUserControlHeight = Math.Max(contentBottom + 120, minScrollableHeight);
            
            // Set AutoScrollMinSize để UserControl biết cần scroll
            this.AutoScrollMinSize = new Size(0, requiredUserControlHeight);
            
            // Đảm bảo UserControl có thể scroll
            this.AutoScroll = true;
            this.VerticalScroll.Visible = true;
            this.VerticalScroll.Enabled = true;
            this.HorizontalScroll.Visible = false;
            this.HorizontalScroll.Enabled = false;
            this.PerformLayout();
        }

        /// <summary>
        /// Xóa tất cả panel bài tập hiện có
        /// </summary>
        private void ClearExercisePanels()
        {
            var controlsToRemove = pnlDanhSachHuanLuyenVien.Controls
                .OfType<ucBaiTap>()
                .Where(uc => uc.Name.StartsWith("ucBaiTap_"))
                .ToList();

            foreach (var uc in controlsToRemove)
            {
                pnlDanhSachHuanLuyenVien.Controls.Remove(uc);
                uc.Dispose();
            }
        }

        /// <summary>
        /// Lấy danh sách bài tập đã được filter
        /// </summary>
        private List<ThuVienBaiTap> GetFilteredExercises(ExerciseFilter filter)
        {
            var query = _dbContext.ThuVienBaiTap.AsQueryable();

            if (filter == null)
            {
                return query.OrderBy(bt => bt.TenBaiTap).ToList();
            }

            // Filter theo tên tìm kiếm
            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                string searchLower = filter.SearchText.ToLower();
                query = query.Where(bt => 
                    (bt.TenBaiTap != null && bt.TenBaiTap.ToLower().Contains(searchLower)) ||
                    (bt.BaiTapID != null && bt.BaiTapID.ToLower().Contains(searchLower)) ||
                    (bt.MoTa != null && bt.MoTa.ToLower().Contains(searchLower)));
            }

            // Filter theo độ khó (CapDo) - so sánh chính xác, case-insensitive
            if (!string.IsNullOrWhiteSpace(filter.DoKho) && filter.DoKho != "Tất cả")
            {
                string doKhoFilter = filter.DoKho.Trim();
                query = query.Where(bt => bt.CapDo != null && 
                    bt.CapDo.Trim().ToLower() == doKhoFilter.ToLower());
            }

            // Bước 2: Load vào memory để xử lý filter phức tạp
            var exercises = query.ToList();

            // Filter theo nhóm cơ chính (chỉ kiểm tra NhomCoChinhNhat)
            if (!string.IsNullOrWhiteSpace(filter.NhomCo) && filter.NhomCo != "Tất cả")
            {
                string nhomCoFilter = filter.NhomCo.Trim();
                exercises = exercises.Where(bt =>
                {
                    // Chỉ so sánh với nhóm cơ chính (case-insensitive)
                    return bt.NhomCoChinhNhat != null && 
                           bt.NhomCoChinhNhat.Trim().Equals(nhomCoFilter, StringComparison.OrdinalIgnoreCase);
                }).ToList();
            }

            // Filter theo thiết bị (xử lý trong memory vì cần tách chuỗi)
            if (!string.IsNullOrWhiteSpace(filter.ThietBi) && filter.ThietBi != "Tất cả")
            {
                string thietBiFilter = filter.ThietBi.Trim();
                exercises = exercises.Where(bt =>
                {
                    if (bt.DungCu == null || string.IsNullOrWhiteSpace(bt.DungCu))
                        return false;

                    string dungCu = bt.DungCu.Trim();
                    
                    // So sánh chính xác (case-insensitive)
                    if (dungCu.Equals(thietBiFilter, StringComparison.OrdinalIgnoreCase))
                        return true;

                    // Kiểm tra nếu DungCu chứa nhiều thiết bị (phân cách bởi dấu + hoặc dấu phẩy)
                    var thietBiList = dungCu.Split(new[] { '+', ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(tb => tb.Trim())
                        .ToList();
                    
                    // Kiểm tra xem có thiết bị nào khớp không
                    return thietBiList.Any(tb => 
                        tb.Equals(thietBiFilter, StringComparison.OrdinalIgnoreCase));
                }).ToList();
            }

            return exercises.OrderBy(bt => bt.TenBaiTap).ToList();
        }

        /// <summary>
        /// Tạo và hiển thị các panel bài tập
        /// </summary>
        private void CreateAndDisplayExercisePanels(List<ThuVienBaiTap> exercises)
        {
            const int panelHeight = 224;
            const int marginY = 10; // Khoảng cách dọc giữa các hàng
            
            // Đảm bảo scroll position về đầu trước khi tính toán
            // Điều này đảm bảo vị trí Top của referenceControl được tính đúng
            this.AutoScrollPosition = new Point(0, 0);
            if (pnlDanhSachHuanLuyenVien.AutoScroll)
            {
                pnlDanhSachHuanLuyenVien.AutoScrollPosition = new Point(0, 0);
            }
            Application.DoEvents(); // Cho phép UI cập nhật scroll position
            
            // Tính toán startY - đặt ngay dưới label "Danh sách bài tập"
            // Dùng giá trị cố định từ Designer để đảm bảo tất cả các trang giống nhau
            // lbDanhSachBT.Location = (23, 13), Size = (185, 29) => Top = 13, Height = 29
            // startY = 13 + 29 + 5 = 47
            const int startY = 47; // Vị trí cố định dưới label "Danh sách bài tập" (từ Designer)
            
            const int columnsPerRow = 2;
            
            // Đảm bảo panel luôn hiển thị
            pnlDanhSachHuanLuyenVien.Visible = true;
            
            // Tính toán kích thước và margin dựa trên ClientSize.Width thực tế của panel
            // Dùng ClientSize.Width để tính chính xác (đã trừ border/padding)
            int containerWidth = pnlDanhSachHuanLuyenVien.ClientSize.Width;
            
            // Nếu ClientSize.Width = 0 hoặc quá nhỏ, dùng Width và trừ border
            if (containerWidth <= 0 || containerWidth < 500)
            {
                containerWidth = pnlDanhSachHuanLuyenVien.Width;
                // Trừ border thickness (1px mỗi bên) và một chút cho borderRadius
                containerWidth -= 4; // Trừ 2px mỗi bên cho an toàn
            }
            
            // Đảm bảo containerWidth hợp lý (không quá lớn)
            if (containerWidth > 1001) containerWidth = 1001;
            
            const int marginLeft = 20; // Margin bên trái
            const int marginRight = 20; // Margin bên phải
            const int marginBetween = 15; // Khoảng cách giữa hai cột
            
            // Tính panelWidth: (containerWidth - marginLeft - marginRight - marginBetween) / 2
            // Đảm bảo tổng: marginLeft + panelWidth + marginBetween + panelWidth + marginRight <= containerWidth
            int totalUsedWidth = marginLeft + marginRight + marginBetween;
            int panelWidth = (containerWidth - totalUsedWidth) / columnsPerRow;
            
            // Kiểm tra và đảm bảo cột phải không vượt quá panel
            // Tính lại rightColumnRightEdge sau khi có panelWidth
            int rightColumnRightEdge = marginLeft + panelWidth + marginBetween + panelWidth;
            
            // Nếu vượt quá, giảm panelWidth cho đến khi vừa
            while (rightColumnRightEdge > containerWidth && panelWidth > 200)
            {
                panelWidth -= 2; // Giảm 2px mỗi lần
                rightColumnRightEdge = marginLeft + panelWidth + marginBetween + panelWidth;
            }
            
            // Đảm bảo panelWidth hợp lý
            if (panelWidth < 200) panelWidth = 200;
            
            // Kiểm tra lại lần cuối: đảm bảo không vượt quá
            rightColumnRightEdge = marginLeft + panelWidth + marginBetween + panelWidth;
            if (rightColumnRightEdge > containerWidth)
            {
                // Nếu vẫn vượt, giảm thêm một chút
                int excess = rightColumnRightEdge - containerWidth;
                panelWidth = Math.Max(200, panelWidth - excess - 2);
            }
            
            // Hiển thị tối đa 6 bài tập (3 hàng x 2 cột)
            int maxExercises = Math.Min(exercises.Count, _pageSize);
            
            // Tạo ucBaiTap cho mỗi bài tập
            for (int i = 0; i < maxExercises; i++)
            {
                var exercise = exercises[i];
                int row = i / columnsPerRow;
                int col = i % columnsPerRow;

                // Tính vị trí (tương đối trong container)
                int x;
                if (col == 0)
                {
                    // Cột trái: marginLeft
                    x = marginLeft;
                }
                else
                {
                    // Cột phải: marginLeft + panelWidth + marginBetween
                    x = marginLeft + panelWidth + marginBetween;
                }
                int y = startY + row * (panelHeight + marginY);

                // Tạo ucBaiTap mới
                var ucBaiTap = new ucBaiTap(exercise);
                
                // Đảm bảo kích thước chính xác
                ucBaiTap.Size = new Size(panelWidth, panelHeight);
                ucBaiTap.Location = new Point(x, y);
                ucBaiTap.Name = $"ucBaiTap_{exercise.BaiTapID}";
                ucBaiTap.Visible = true;
                ucBaiTap.AutoSize = false;
                ucBaiTap.Anchor = AnchorStyles.None;

                // Gắn event handlers cho sửa và xóa
                ucBaiTap.OnEdit += (s, e) => HandleEditExercise(ucBaiTap.GetExercise());
                ucBaiTap.OnDelete += (s, e) => HandleDeleteExercise(ucBaiTap.GetExercise());

                pnlDanhSachHuanLuyenVien.Controls.Add(ucBaiTap);
            }
            
            // Đảm bảo các nút phân trang luôn ở trên cùng (không bị che bởi các bài tập)
            if (btnPrevious != null)
            {
                btnPrevious.BringToFront();
            }
            if (btnNext != null)
            {
                btnNext.BringToFront();
            }
            if (lblSoTrang != null)
            {
                lblSoTrang.BringToFront();
            }
            
            // Cập nhật AutoScrollMinSize sau khi thêm các bài tập
            UpdateAutoScrollMinSize();
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
        /// Xử lý sự kiện Edit từ ucBaiTap
        /// </summary>
        private void HandleEditExercise(ThuVienBaiTap exercise)
        {
            if (exercise == null) return;
            ShowSuaBT(exercise);
        }
        
        /// <summary>
        /// Xử lý sự kiện Delete từ ucBaiTap
        /// </summary>
        private void HandleDeleteExercise(ThuVienBaiTap exercise)
        {
            if (exercise == null) return;
            DeleteExercise(exercise);
        }

        /// <summary>
        /// Hiển thị UserControl sửa bài tập
        /// </summary>
        private void ShowSuaBT(ThuVienBaiTap exercise)
        {
            if (exercise == null)
            {
                MessageBox.Show("Không tìm thấy bài tập!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _overlayPanel.Controls.Clear();
            
            var ucSua = new ucSuaBT(exercise);
            ucSua.Dock = DockStyle.Fill;
            
            // Đăng ký event khi sửa thành công
            ucSua.OnSaveSuccess += (s, e) =>
            {
                HideOverlay();
                // Refresh Entity Framework context để lấy dữ liệu mới
                RefreshDbContext();
                // Reload với filter hiện tại
                LoadExerciseList(_currentFilter);
                LoadStatistics();
                LoadFilterDataFromDatabase(); // Reload filters nếu có thay đổi
            };
            
            // Đăng ký event khi hủy
            ucSua.OnCancel += (s, e) =>
            {
                HideOverlay();
            };
            
            _overlayPanel.Controls.Add(ucSua);
            _overlayPanel.Visible = true;
            _overlayPanel.BringToFront();
        }

        /// <summary>
        /// Xóa bài tập với validation
        /// </summary>
        private void DeleteExercise(ThuVienBaiTap exercise)
        {
            if (exercise == null)
            {
                MessageBox.Show("Không tìm thấy bài tập!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra xem bài tập có đang được sử dụng không
            if (IsExerciseInUse(exercise.BaiTapID))
            {
                MessageBox.Show("Bài tập này đang được sử dụng, không thể xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Xác nhận xóa
            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa bài tập \"{exercise.TenBaiTap}\"?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Thực hiện xóa
                var exerciseToDelete = _dbContext.ThuVienBaiTap.FirstOrDefault(bt => bt.BaiTapID == exercise.BaiTapID);
                if (exerciseToDelete != null)
                {
                    _dbContext.ThuVienBaiTap.Remove(exerciseToDelete);
                    _dbContext.SaveChanges();
                }

                MessageBox.Show("Xóa bài tập thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Refresh Entity Framework context
                RefreshDbContext();
                // Reload sau khi xóa thành công
                LoadExerciseList(_currentFilter);
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucQuanLyBT] DeleteExercise error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Kiểm tra xem bài tập có đang được sử dụng không
        /// </summary>
        private bool IsExerciseInUse(string baiTapID)
        {
            try
            {
                return _dbContext.GiaoBaiTapChoUser.Any(gbt => gbt.ThuVienBaiTapID == baiTapID) ||
                       _dbContext.BaiTapChiTiet.Any(btct => btct.BaiTapID == baiTapID);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Class để lưu filter
        /// </summary>
        private class ExerciseFilter
        {
            public string SearchText { get; set; }
            public string NhomCo { get; set; }
            public string DoKho { get; set; }
            public string ThietBi { get; set; }
        }
    }
}
