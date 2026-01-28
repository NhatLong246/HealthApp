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
    public partial class ucKhachHang : UserControl
    {
        private WF_HealthTracker _dbContext;

        public ucKhachHang()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            this.Load += ucKhachHang_Load;
            InitializeFilters();
        }

        /// <summary>
        /// Khởi tạo các filter controls
        /// </summary>
        private void InitializeFilters()
        {
            // Xử lý placeholder cho txtTiemKiem
            txtTiemKiem.Enter += TxtTiemKiem_Enter;
            txtTiemKiem.Leave += TxtTiemKiem_Leave;

            // Event handlers cho buttons
            btnApDung.Click += BtnApDung_Click;
            btnDatLai.Click += BtnDatLai_Click;
            btnExcel.Click += BtnExcel_Click;
        }

        private void ucKhachHang_Load(object sender, EventArgs e)
        {
            LoadStatistics();
            LoadUserList();
            InitializeDataGridView();
        }

        /// <summary>
        /// Khởi tạo DataGridView
        /// </summary>
        private void InitializeDataGridView()
        {
            // Thiết lập DataGridView
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;

            // Thêm event handler cho selection changed
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }

        /// <summary>
        /// Load và hiển thị các thống kê về người dùng
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                // 1. Tổng số người dùng trong hệ thống
                int totalUsers = _dbContext.Users.Count();
                lbTongUsers.Text = totalUsers.ToString();

                // 2. Độ tuổi trung bình của người dùng
                var usersWithAge = _dbContext.Users
                    .Where(u => u.NgaySinh.HasValue)
                    .ToList();

                double averageAge = 0;
                if (usersWithAge.Any())
                {
                    var today = DateTime.Now;
                    var ages = usersWithAge.Select(u =>
                    {
                        var birthDate = u.NgaySinh.Value;
                        int age = today.Year - birthDate.Year;
                        if (birthDate.Date > today.AddYears(-age)) age--;
                        return (double)age;
                    }).ToList();

                    averageAge = ages.Any() ? ages.Average() : 0.0;
                }
                lbGenTuoiTB.Text = averageAge.ToString("F1");

                // 3. Tỷ lệ hoàn thành mục tiêu (%)
                int totalUsersWithGoals = _dbContext.Users
                    .Where(u => u.MucTieu.Any())
                    .Count();

                int totalCompletedGoals = _dbContext.MucTieu
                    .Where(m => m.TrangThai == "Hoàn thành")
                    .Count();

                int totalActiveGoals = _dbContext.MucTieu
                    .Where(m => m.TrangThai == "Đang thực hiện")
                    .Count();

                double completionRate = 0;
                int totalGoals = totalCompletedGoals + totalActiveGoals;
                if (totalGoals > 0)
                {
                    completionRate = ((double)totalCompletedGoals / totalGoals) * 100;
                }
                lbgenTyLe.Text = completionRate.ToString("F1");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thống kê: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucKhachHang] LoadStatistics error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Xử lý khi focus vào textbox tìm kiếm
        /// </summary>
        private void TxtTiemKiem_Enter(object sender, EventArgs e)
        {
            if (txtTiemKiem.Text == "Tìm kiếm..")
            {
                txtTiemKiem.Text = "";
                txtTiemKiem.ForeColor = System.Drawing.Color.Black;
            }
        }

        /// <summary>
        /// Xử lý khi rời khỏi textbox tìm kiếm
        /// </summary>
        private void TxtTiemKiem_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTiemKiem.Text))
            {
                txtTiemKiem.Text = "Tìm kiếm..";
                txtTiemKiem.ForeColor = System.Drawing.Color.Gray;
            }
        }

        /// <summary>
        /// Load danh sách người dùng vào DataGridView
        /// </summary>
        private void LoadUserList(string searchText = "")
        {
            try
            {
                var query = _dbContext.Users.AsQueryable();

                // Áp dụng filter tìm kiếm
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    string searchLower = searchText.ToLower();
                    query = query.Where(u =>
                        (u.HoTen != null && u.HoTen.ToLower().Contains(searchLower)) ||
                        (u.Username != null && u.Username.ToLower().Contains(searchLower)) ||
                        (u.UserID != null && u.UserID.ToLower().Contains(searchLower)) ||
                        (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                        (u.SDT != null && u.SDT.Contains(searchText)));
                }

                var users = query
                    .OrderBy(u => u.Username)
                    .Select(u => new
                    {
                        u.UserID,
                        u.Username,
                        u.HoTen,
                        u.Email,
                        u.SDT,
                        u.Role,
                        u.NgaySinh,
                        u.GioiTinh,
                        u.CreatedDate
                    })
                    .ToList();

                // Bind data vào DataGridView
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();

                // Thêm columns nếu chưa có
                if (dataGridView1.Columns.Count == 0)
                {
                    dataGridView1.Columns.Add("UserID", "ID");
                    dataGridView1.Columns.Add("HoTen", "Tên");
                    dataGridView1.Columns.Add("Email", "Email");
                    dataGridView1.Columns.Add("Role", "Vai trò");

                    // Ẩn cột UserID (dùng để lấy dữ liệu)
                    dataGridView1.Columns["UserID"].Visible = false;
                }

                // Thêm dữ liệu
                foreach (var user in users)
                {
                    int rowIndex = dataGridView1.Rows.Add(
                        user.UserID,
                        !string.IsNullOrWhiteSpace(user.HoTen) ? user.HoTen : user.Username ?? "",
                        user.Email ?? "",
                        user.Role ?? ""
                    );
                }

                // Auto resize columns
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load danh sách người dùng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucKhachHang] LoadUserList error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Xử lý khi chọn một row trong DataGridView
        /// </summary>
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    var selectedRow = dataGridView1.SelectedRows[0];
                    string userID = selectedRow.Cells["UserID"].Value?.ToString();

                    if (!string.IsNullOrEmpty(userID))
                    {
                        LoadUserDetails(userID);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucKhachHang] DataGridView1_SelectionChanged error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load thông tin chi tiết của user vào panel bên cạnh
        /// </summary>
        private void LoadUserDetails(string userID)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.UserID == userID);
                if (user == null)
                {
                    ClearUserDetails();
                    return;
                }

                // Lấy thông tin địa chỉ từ HuanLuyenVien nếu user là PT
                string diaChi = "";
                if (user.Role == "PT")
                {
                    var pt = _dbContext.HuanLuyenVien.FirstOrDefault(p => p.UserID == userID);
                    diaChi = pt?.ThanhPho ?? "";
                }

                // Hiển thị thông tin
                lblHovaTen.Text = !string.IsNullOrWhiteSpace(user.HoTen) ? user.HoTen : user.Username ?? "N/A";
                lblMaUser.Text = user.UserID ?? "N/A";
                lblGmail.Text = user.Email ?? "N/A";
                lblSoDienThoai.Text = user.SDT ?? "N/A";
                //lblDiaChi.Text = !string.IsNullOrWhiteSpace(diaChi) ? diaChi : "N/A";
                lbNgaySinh.Text = user.NgaySinh?.ToString("dd/MM/yyyy") ?? "N/A";
                lbGioiTinh.Text = user.GioiTinh ?? "N/A";

                // Load ảnh đại diện
                if (pictureBox3 != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(user.AnhDaiDien) && System.IO.File.Exists(user.AnhDaiDien))
                        {
                            pictureBox3.Image = Image.FromFile(user.AnhDaiDien);
                        }
                        else
                        {
                            // Giữ ảnh mặc định hoặc set null
                            pictureBox3.Image = null;
                        }
                    }
                    catch
                    {
                        pictureBox3.Image = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load thông tin chi tiết: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucKhachHang] LoadUserDetails error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Xóa thông tin chi tiết (khi không có user nào được chọn)
        /// </summary>
        private void ClearUserDetails()
        {
            lblHovaTen.Text = "N/A";
            lblMaUser.Text = "N/A";
            lblGmail.Text = "N/A";
            lblSoDienThoai.Text = "N/A";
            //lblDiaChi.Text = "N/A";
            lbNgaySinh.Text = "N/A";
            lbGioiTinh.Text = "N/A";
            if (pictureBox3 != null)
            {
                pictureBox3.Image = null;
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

                // Load lại danh sách với filter
                LoadUserList(searchText);
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
            try
            {
                // Reset textbox tìm kiếm
                txtTiemKiem.Text = "Tìm kiếm..";
                txtTiemKiem.ForeColor = System.Drawing.Color.Gray;

                // Load lại danh sách không filter
                LoadUserList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt lại: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi click nút Xuất Excel
        /// </summary>
        private void BtnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị SaveFileDialog
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
                    saveDialog.FilterIndex = 1;
                    saveDialog.FileName = $"DanhSachNguoiDung_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    saveDialog.Title = "Lưu file Excel";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Disable button để tránh click nhiều lần
                        btnExcel.Enabled = false;
                        btnExcel.Text = "Đang xuất...";

                        // Lấy dữ liệu từ DataGridView hoặc từ database
                        var users = GetUsersForExport();

                        // Xuất file Excel
                        ExportUsersToExcel(users, saveDialog.FileName);

                        btnExcel.Enabled = true;
                        btnExcel.Text = "Xuất excel";

                        MessageBox.Show("Xuất file Excel thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                btnExcel.Enabled = true;
                btnExcel.Text = "Xuất excel";
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ucKhachHang] BtnExcel_Click error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Lấy danh sách users để xuất Excel (lấy từ database với filter hiện tại)
        /// </summary>
        private List<Users> GetUsersForExport()
        {
            try
            {
                var query = _dbContext.Users.AsQueryable();

                // Áp dụng filter tìm kiếm nếu có
                string searchText = txtTiemKiem.Text;
                if (!string.IsNullOrWhiteSpace(searchText) && searchText != "Tìm kiếm..")
                {
                    string searchLower = searchText.ToLower();
                    query = query.Where(u =>
                        (u.HoTen != null && u.HoTen.ToLower().Contains(searchLower)) ||
                        (u.Username != null && u.Username.ToLower().Contains(searchLower)) ||
                        (u.UserID != null && u.UserID.ToLower().Contains(searchLower)) ||
                        (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                        (u.SDT != null && u.SDT.Contains(searchText)));
                }

                return query.OrderBy(u => u.Username).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ucKhachHang] GetUsersForExport error: {ex.Message}");
                return new List<Users>();
            }
        }

        /// <summary>
        /// Xuất danh sách users ra file Excel
        /// </summary>
        private void ExportUsersToExcel(List<Users> users, string filePath)
        {
            try
            {
                using (var sw = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // Excel XML Header
                    sw.WriteLine("<?xml version=\"1.0\"?>");
                    sw.WriteLine("<?mso-application progid=\"Excel.Sheet\"?>");
                    sw.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                    sw.WriteLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
                    sw.WriteLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
                    sw.WriteLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                    sw.WriteLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");

                    // Styles
                    sw.WriteLine("<Styles>");
                    sw.WriteLine("<Style ss:ID=\"Header\">");
                    sw.WriteLine("<Font ss:Bold=\"1\"/>");
                    sw.WriteLine("<Interior ss:Color=\"#CCCCCC\" ss:Pattern=\"Solid\"/>");
                    sw.WriteLine("<Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/>");
                    sw.WriteLine("</Style>");
                    sw.WriteLine("<Style ss:ID=\"Cell\">");
                    sw.WriteLine("<Alignment ss:Vertical=\"Center\"/>");
                    sw.WriteLine("</Style>");
                    sw.WriteLine("</Styles>");

                    // Worksheet
                    sw.WriteLine("<Worksheet ss:Name=\"Danh sách người dùng\">");
                    sw.WriteLine("<Table>");

                    // Header row
                    sw.WriteLine("<Row>");
                    WriteExcelCell(sw, "ID", "Header");
                    WriteExcelCell(sw, "Tên", "Header");
                    WriteExcelCell(sw, "Email", "Header");
                    WriteExcelCell(sw, "Số điện thoại", "Header");
                    WriteExcelCell(sw, "Vai trò", "Header");
                    WriteExcelCell(sw, "Ngày sinh", "Header");
                    WriteExcelCell(sw, "Giới tính", "Header");
                    WriteExcelCell(sw, "Ngày tạo", "Header");
                    sw.WriteLine("</Row>");

                    // Data rows
                    foreach (var user in users)
                    {
                        sw.WriteLine("<Row>");
                        WriteExcelCell(sw, user.UserID ?? "", "Cell");
                        WriteExcelCell(sw, !string.IsNullOrWhiteSpace(user.HoTen) ? user.HoTen : user.Username ?? "", "Cell");
                        WriteExcelCell(sw, user.Email ?? "", "Cell");
                        WriteExcelCell(sw, user.SDT ?? "", "Cell");
                        WriteExcelCell(sw, user.Role ?? "", "Cell");
                        WriteExcelCell(sw, user.NgaySinh?.ToString("dd/MM/yyyy") ?? "", "Cell");
                        WriteExcelCell(sw, user.GioiTinh ?? "", "Cell");
                        WriteExcelCell(sw, user.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? "", "Cell");
                        sw.WriteLine("</Row>");
                    }

                    sw.WriteLine("</Table>");
                    sw.WriteLine("</Worksheet>");
                    sw.WriteLine("</Workbook>");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi ghi file Excel: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ghi một cell vào Excel XML
        /// </summary>
        private void WriteExcelCell(System.IO.StreamWriter sw, string value, string styleId)
        {
            // Escape XML special characters
            string escapedValue = value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");

            sw.WriteLine($"<Cell ss:StyleID=\"{styleId}\"><Data ss:Type=\"String\">{escapedValue}</Data></Cell>");
        }
    }
}
