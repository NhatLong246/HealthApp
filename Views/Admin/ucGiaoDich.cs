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
    public partial class ucGiaoDich : UserControl
    {
        private WF_HealthTracker _dbContext;
        private GiaoDichFilter _currentFilter; // Lưu filter hiện tại

        public ucGiaoDich()
        {
            try
            {
                InitializeComponent();
                _dbContext = new WF_HealthTracker();
                this.Load += ucGiaoDich_Load;
                this.Disposed += UcGiaoDich_Disposed;
                
                // Đảm bảo panel chức năng và textbox có thể tương tác
                pnlChucNang.Enabled = true;
                pnlChucNang.BringToFront();
                
                InitializeDataGridView();
                InitializeFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] Constructor error: {ex.Message}\n{ex.StackTrace}");
                // Không throw exception để tránh crash, nhưng log để debug
            }
        }

        /// <summary>
        /// Dispose resources khi control bị dispose
        /// </summary>
        private void UcGiaoDich_Disposed(object sender, EventArgs e)
        {
            _dbContext?.Dispose();
        }

        /// <summary>
        /// Khởi tạo các filter controls
        /// </summary>
        private void InitializeFilters()
        {
            // Đảm bảo textbox được enable và có thể focus
            txtTiemKiem.Enabled = true;
            txtTiemKiem.ReadOnly = false;
            
            // Xử lý placeholder cho txtTiemKiem
            // Với Guna2TextBox, PlaceholderText tự động xử lý, nhưng chúng ta vẫn cần xử lý khi nhập
            txtTiemKiem.Enter += TxtTiemKiem_Enter;
            txtTiemKiem.Leave += TxtTiemKiem_Leave;
            // Xử lý khi nhấn Enter trong textbox tìm kiếm
            txtTiemKiem.KeyDown += TxtTiemKiem_KeyDown;

            // Load dữ liệu từ database
            LoadFilterDataFromDatabase();

            // Event handlers cho buttons
            btnApDung.Click += BtnApDung_Click;
            btnDatLai.Click += BtnDatLai_Click;

            
            // Đảm bảo buttons được enable
            btnApDung.Enabled = true;
            btnDatLai.Enabled = true;
        }

        /// <summary>
        /// Khởi tạo DataGridView
        /// </summary>
        private void InitializeDataGridView()
        {
            dgvGiaoDich.AutoGenerateColumns = false;
            dgvGiaoDich.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGiaoDich.MultiSelect = false;
            dgvGiaoDich.ReadOnly = true;
            dgvGiaoDich.ColumnHeadersVisible = true; // Đảm bảo hiển thị header
            dgvGiaoDich.ColumnHeadersHeight = 30; // Đặt chiều cao header
            
            // Tạo columns nếu chưa có
            if (dgvGiaoDich.Columns.Count == 0)
            {
                CreateDataGridViewColumns();
            }
            
            // Thêm event handler cho double click
            dgvGiaoDich.CellDoubleClick += DgvGiaoDich_CellDoubleClick;
        }

        /// <summary>
        /// Tạo các columns cho DataGridView
        /// </summary>
        private void CreateDataGridViewColumns()
        {
            dgvGiaoDich.Columns.Clear();
            
            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "GiaoDichID", HeaderText = "Mã GD", Width = 120 });
            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "KhachHang", HeaderText = "Khách Hàng", Width = 150 });
            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "PT", HeaderText = "PT", Width = 150 });
            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoTien", HeaderText = "Số Tiền", Width = 120 });

            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "TienHoaHong", HeaderText = "Tiền Hoa Hồng", Width = 130 });
            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "TienPTNhan", HeaderText = "Tiền PT Nhận", Width = 130 });
            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng Thái", Width = 100 });
            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhuongThuc", HeaderText = "Phương Thức", Width = 120 });
            dgvGiaoDich.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayGiaoDich", HeaderText = "Ngày GD", Width = 150 });
        }

        /// <summary>
        /// Load dữ liệu filter từ database
        /// </summary>
        private void LoadFilterDataFromDatabase()
        {
            try
            {
                // Load phương thức thanh toán từ database
                LoadPhuongThucThanhToanFromDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load dữ liệu filter: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] LoadFilterDataFromDatabase error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load danh sách phương thức thanh toán từ database
        /// </summary>
        private void LoadPhuongThucThanhToanFromDatabase()
        {
            cboPhuongThucGiaoDich.Items.Clear();
            cboPhuongThucGiaoDich.Items.Add("Tất cả");

            try
            {
                var phuongThucList = _dbContext.GiaoDich
                    .Where(gd => gd.PhuongThucThanhToan != null && gd.PhuongThucThanhToan != "")
                    .Select(gd => gd.PhuongThucThanhToan)
                    .Distinct()
                    .OrderBy(pt => pt)
                    .ToList();

                foreach (var pt in phuongThucList)
                {
                    cboPhuongThucGiaoDich.Items.Add(pt);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] LoadPhuongThucThanhToanFromDatabase error: {ex.Message}\n{ex.StackTrace}");
            }

            if (cboPhuongThucGiaoDich.Items.Count > 0)
            {
                cboPhuongThucGiaoDich.SelectedIndex = 0;
            }
        }

        private void ucGiaoDich_Load(object sender, EventArgs e)
        {
            // Đảm bảo UserControl và tất cả controls được enable
            this.Enabled = true;
            pnlChucNang.Enabled = true;
            txtTiemKiem.Enabled = true;
            cboPhuongThucGiaoDich.Enabled = true;
            guna2DateTimePicker1.Enabled = true;
            guna2DateTimePicker2.Enabled = true;
            btnApDung.Enabled = true;
            btnDatLai.Enabled = true;
            
            LoadStatistics();
            LoadGiaoDichList();
        }

        /// <summary>
        /// Load và hiển thị các thống kê về giao dịch
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                // 1. Tổng số giao dịch
                int totalGiaoDich = _dbContext.GiaoDich.Count();
                lbGenTongGiaoDich.Text = totalGiaoDich.ToString();

                // 2. Tổng doanh thu (tổng số tiền)
                double totalDoanhThu = _dbContext.GiaoDich
                    .Where(gd => gd.TrangThaiThanhToan == "Completed")
                    .Sum(gd => (double?)gd.SoTien) ?? 0;
                lbGenDoanhThuPT.Text = FormatCurrency(totalDoanhThu);

                // 3. Tổng hoa hồng app
                double totalHoaHong = _dbContext.GiaoDich
                    .Where(gd => gd.TrangThaiThanhToan == "Completed")
                    .Sum(gd => (double?)gd.SoTienHoaHong) ?? 0;
                lbGenTienHoaHong.Text = FormatCurrency(totalHoaHong);

                // 4. Tổng tiền PT nhận
                double totalTienPT = _dbContext.GiaoDich
                    .Where(gd => gd.TrangThaiThanhToan == "Completed")
                    .Sum(gd => (double?)gd.SoTienPTNhan) ?? 0;
                lblTongTien.Text = FormatCurrency(totalTienPT);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] LoadStatistics error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Format số tiền thành chuỗi VNĐ
        /// </summary>
        private string FormatCurrency(double amount)
        {
            return amount.ToString("N0") + " VNĐ";
        }

        /// <summary>
        /// Xử lý khi double click vào một row trong DataGridView
        /// </summary>
        private void DgvGiaoDich_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Chỉ xử lý khi click vào row (không phải header)
                if (e.RowIndex < 0)
                    return;

                // Lấy GiaoDichID từ Tag của row
                var row = dgvGiaoDich.Rows[e.RowIndex];
                string giaoDichID = row.Tag?.ToString();

                if (string.IsNullOrWhiteSpace(giaoDichID))
                {
                    MessageBox.Show("Không tìm thấy thông tin giao dịch!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mở form chi tiết giao dịch
                using (var frmChiTiet = new ChiTietGiaoDich(giaoDichID))
                {
                    frmChiTiet.StartPosition = FormStartPosition.CenterParent;
                    frmChiTiet.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chi tiết giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] Error opening ChiTietGiaoDich: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load danh sách giao dịch và hiển thị trong DataGridView
        /// </summary>
        private void LoadGiaoDichList()
        {
            LoadGiaoDichList(null);
        }

        /// <summary>
        /// Load danh sách giao dịch với filter
        /// </summary>
        private void LoadGiaoDichList(GiaoDichFilter filter)
        {
            try
            {
                // Lấy danh sách giao dịch với join Users và HuanLuyenVien
                var query = from gd in _dbContext.GiaoDich
                            join kh in _dbContext.Users on gd.KhachHangID equals kh.UserID
                            join pt in _dbContext.HuanLuyenVien on gd.PTID equals pt.PTID
                            join ptUser in _dbContext.Users on pt.UserID equals ptUser.UserID
                            join dl in _dbContext.DatLichPT on gd.DatLichID equals dl.DatLichID
                            select new
                            {
                                gd.GiaoDichID,
                                gd.DatLichID,
                                KhachHangTen = kh.HoTen ?? kh.UserID,
                                PTTen = ptUser.HoTen ?? pt.PTID,
                                gd.SoTien,
                               
                                gd.SoTienHoaHong,
                                gd.SoTienPTNhan,
                                gd.TrangThaiThanhToan,
                                gd.PhuongThucThanhToan,
                                gd.NgayGiaoDich,
                                dl.NgayGioDat
                            };

                // Áp dụng filter
                if (filter != null)
                {
                    // Filter theo ngày
                    if (filter.NgayBatDau.HasValue)
                    {
                        query = query.Where(x => x.NgayGiaoDich >= filter.NgayBatDau.Value);
                    }
                    if (filter.NgayKetThuc.HasValue)
                    {
                        var endDate = filter.NgayKetThuc.Value.Date.AddDays(1);
                        query = query.Where(x => x.NgayGiaoDich < endDate);
                    }

                    // Filter theo phương thức thanh toán
                    if (!string.IsNullOrWhiteSpace(filter.PhuongThucThanhToan) && filter.PhuongThucThanhToan != "Tất cả")
                    {
                        query = query.Where(x => x.PhuongThucThanhToan == filter.PhuongThucThanhToan);
                    }

                    // Filter theo trạng thái
                    if (!string.IsNullOrWhiteSpace(filter.TrangThai) && filter.TrangThai != "Tất cả")
                    {
                        query = query.Where(x => x.TrangThaiThanhToan == filter.TrangThai);
                    }

                }

                // Sắp xếp theo ngày giao dịch mới nhất và chuyển sang list
                var giaoDichList = query.OrderByDescending(x => x.NgayGiaoDich).ToList();

                // Filter theo tìm kiếm (sau khi đã load về memory để có thể dùng ToLower)
                if (filter != null && !string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string searchLower = filter.SearchText.ToLower();
                    giaoDichList = giaoDichList.Where(x =>
                        (x.GiaoDichID != null && x.GiaoDichID.ToLower().Contains(searchLower)) ||
                        (x.KhachHangTen != null && x.KhachHangTen.ToLower().Contains(searchLower)) ||
                        (x.PTTen != null && x.PTTen.ToLower().Contains(searchLower)) ||
                        (x.DatLichID != null && x.DatLichID.ToLower().Contains(searchLower))).ToList();
                }

                // Bind vào DataGridView (cast sang dynamic để tránh lỗi type)
                BindToDataGridView(giaoDichList.Cast<dynamic>().ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] Error loading giao dich list: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Bind dữ liệu vào DataGridView
        /// </summary>
        private void BindToDataGridView(List<dynamic> giaoDichList)
        {
            dgvGiaoDich.Rows.Clear();

            foreach (var gd in giaoDichList)
            {
                int rowIndex = dgvGiaoDich.Rows.Add(
                    gd.GiaoDichID,
                    gd.KhachHangTen,
                    gd.PTTen,
                    FormatCurrency(gd.SoTien),
                   
                    FormatCurrency(gd.SoTienHoaHong ?? 0),
                    FormatCurrency(gd.SoTienPTNhan ?? 0),
                    gd.TrangThaiThanhToan,
                    gd.PhuongThucThanhToan ?? "N/A",
                    gd.NgayGiaoDich?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"
                );

                // Đánh dấu tag để có thể lấy GiaoDichID
                dgvGiaoDich.Rows[rowIndex].Tag = gd.GiaoDichID;
            }
        }

        /// <summary>
        /// Xử lý placeholder cho txtTiemKiem
        /// Với Guna2TextBox, PlaceholderText tự động xử lý, không cần set text thủ công
        /// </summary>
        private void TxtTiemKiem_Enter(object sender, EventArgs e)
        {
            // Guna2TextBox tự động xử lý placeholder, không cần làm gì
            // Nhưng đảm bảo textbox có thể nhận focus
            txtTiemKiem.Focus();
        }

        private void TxtTiemKiem_Leave(object sender, EventArgs e)
        {
            // Guna2TextBox tự động hiển thị placeholder khi text rỗng
            // Không cần làm gì thêm
        }

        /// <summary>
        /// Xử lý khi nhấn phím trong textbox tìm kiếm
        /// </summary>
        private void TxtTiemKiem_KeyDown(object sender, KeyEventArgs e)
        {
            // Khi nhấn Enter, tự động áp dụng filter
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Ngăn tiếng beep
                BtnApDung_Click(btnApDung, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Xử lý khi click nút Áp dụng
        /// </summary>
        private void BtnApDung_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[ucGiaoDich] BtnApDung_Click called");

                // Lấy text tìm kiếm
                // Với Guna2TextBox, nếu text rỗng thì PlaceholderText sẽ hiển thị nhưng Text vẫn là ""
                string searchText = txtTiemKiem.Text?.Trim() ?? "";

                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] SearchText: {searchText}");

                // Tạo filter
                var filter = new GiaoDichFilter
                {
                    SearchText = searchText,
                    NgayBatDau = guna2DateTimePicker1.Value.Date,
                    NgayKetThuc = guna2DateTimePicker2.Value.Date,
                    PhuongThucThanhToan = cboPhuongThucGiaoDich.SelectedItem?.ToString() ?? "",
                    TrangThai = "Tất cả" // Có thể thêm combobox cho trạng thái nếu cần
                };

                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] Filter created - From: {filter.NgayBatDau}, To: {filter.NgayKetThuc}, Method: {filter.PhuongThucThanhToan}");

                // Lưu filter hiện tại
                _currentFilter = filter;
                // Load lại danh sách với filter
                LoadGiaoDichList(filter);
                // Reload thống kê với filter
                LoadStatisticsWithFilter(filter);

                System.Diagnostics.Debug.WriteLine("[ucGiaoDich] Filter applied successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng filter: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] BtnApDung_Click error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load thống kê với filter
        /// </summary>
        private void LoadStatisticsWithFilter(GiaoDichFilter filter)
        {
            try
            {
                var query = _dbContext.GiaoDich.AsQueryable();

                // Áp dụng filter tương tự
                if (filter.NgayBatDau.HasValue)
                {
                    query = query.Where(gd => gd.NgayGiaoDich >= filter.NgayBatDau.Value);
                }
                if (filter.NgayKetThuc.HasValue)
                {
                    var endDate = filter.NgayKetThuc.Value.Date.AddDays(1);
                    query = query.Where(gd => gd.NgayGiaoDich < endDate);
                }
                if (!string.IsNullOrWhiteSpace(filter.PhuongThucThanhToan) && filter.PhuongThucThanhToan != "Tất cả")
                {
                    query = query.Where(gd => gd.PhuongThucThanhToan == filter.PhuongThucThanhToan);
                }

                // Tính toán thống kê
                var completedGiaoDich = query.Where(gd => gd.TrangThaiThanhToan == "Completed");

                int totalGiaoDich = query.Count();
                double totalDoanhThu = completedGiaoDich.Sum(gd => (double?)gd.SoTien) ?? 0;
                double totalHoaHong = completedGiaoDich.Sum(gd => (double?)gd.SoTienHoaHong) ?? 0;
                double totalTienPT = completedGiaoDich.Sum(gd => (double?)gd.SoTienPTNhan) ?? 0;

                lbGenTongGiaoDich.Text = totalGiaoDich.ToString();
                lbGenDoanhThuPT.Text = FormatCurrency(totalDoanhThu);
                lbGenTienHoaHong.Text = FormatCurrency(totalHoaHong);
                lblTongTien.Text = FormatCurrency(totalTienPT);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] LoadStatisticsWithFilter error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý khi click nút Đặt lại
        /// </summary>
        private void BtnDatLai_Click(object sender, EventArgs e)
        {
            txtTiemKiem.Text = "Tìm kiếm..";
            txtTiemKiem.ForeColor = System.Drawing.Color.Gray;
            cboPhuongThucGiaoDich.SelectedIndex = 0;
            // Set default date range: 1 tháng trước đến hiện tại
            guna2DateTimePicker1.Value = DateTime.Now.AddMonths(-1);
            guna2DateTimePicker2.Value = DateTime.Now;
            _currentFilter = null; // Reset filter
            LoadGiaoDichList();
            LoadStatistics();
        }

        /// <summary>
        /// Xử lý khi click nút Xác minh đăng ký (có thể dùng để xác nhận giao dịch)
        /// </summary>
        private void BtnXacMinhDangKy_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGiaoDich.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn giao dịch cần xác nhận!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = dgvGiaoDich.SelectedRows[0];
                string giaoDichID = selectedRow.Tag?.ToString();

                if (string.IsNullOrWhiteSpace(giaoDichID))
                {
                    MessageBox.Show("Không tìm thấy thông tin giao dịch!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var giaoDich = _dbContext.GiaoDich.FirstOrDefault(gd => gd.GiaoDichID == giaoDichID);
                if (giaoDich == null)
                {
                    MessageBox.Show("Không tìm thấy giao dịch!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra trạng thái hiện tại
                if (giaoDich.TrangThaiThanhToan == "Completed")
                {
                    MessageBox.Show("Giao dịch này đã được xác nhận!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Xác nhận giao dịch
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xác nhận giao dịch {giaoDichID}?",
                    "Xác nhận giao dịch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    giaoDich.TrangThaiThanhToan = "Completed";
                    _dbContext.SaveChanges();

                    MessageBox.Show("Xác nhận giao dịch thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload danh sách và thống kê
                    RefreshDbContext();
                    LoadGiaoDichList(_currentFilter);
                    LoadStatisticsWithFilter(_currentFilter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác nhận giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] BtnXacMinhDangKy_Click error: {ex.Message}\n{ex.StackTrace}");
            }
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
                System.Diagnostics.Debug.WriteLine($"[ucGiaoDich] RefreshDbContext error: {ex.Message}");
            }
        }

        /// <summary>
        /// Class để lưu filter
        /// </summary>
        private class GiaoDichFilter
        {
            public string SearchText { get; set; }
            public DateTime? NgayBatDau { get; set; }
            public DateTime? NgayKetThuc { get; set; }
            public string PhuongThucThanhToan { get; set; }
            public string TrangThai { get; set; }
        }
    }
}
