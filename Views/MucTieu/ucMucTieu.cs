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
using Microsoft.Web.WebView2.Core;
using HealthApp.Views.Dashboard;
using HealthApp.Controllers;
using HealthApp.Models;
using HealthApp.Common.Helpers;
using HealthApp.Services.Interfaces;
using WeeklySchedule = HealthApp.Services.Interfaces.WeeklySchedule;

namespace HealthApp.Views.MucTieu
{
    public partial class ucMucTieu : UserControl
    {
        private GoalController _goalController;
        private NutritionController _nutritionController;
        private string _selectedLoaiMucTieu = null; // Loại mục tiêu đã chọn
        private DateTime? _selectedStartDate = null; // Ngày bắt đầu đã chọn (null = chưa chọn)
        private DateTime? _selectedEndDate = null; // Ngày kết thúc đã chọn (null = chưa chọn)
        private bool _isSelectingStartDate = true; // true = đang chọn ngày bắt đầu, false = đang chọn ngày kết thúc
        private DateTime _currentMonth = DateTime.Now; // Tháng hiện tại trong calendar
        private Dictionary<string, bool> _selectedDaysOfWeek = new Dictionary<string, bool>(); // Các thứ đã chọn
        private Dictionary<string, WeeklySchedule> _weeklySchedules = new Dictionary<string, WeeklySchedule>(); // Lịch tập theo thứ
        private List<ThuVienMonAn> _selectedFoods = new List<ThuVienMonAn>(); // Danh sách món ăn đã chọn
        private ThuVienMonAn _currentSelectedFood = null; // Món ăn đang được chọn để xem chi tiết
        private List<ThuVienMonAn> _foodLibraryCache = new List<ThuVienMonAn>(); // Cache toàn bộ thư viện món ăn
        private bool _suppressFoodSelectionChanged = false;

        public ucMucTieu()
        {
            InitializeComponent();
            _goalController = new GoalController();
            _nutritionController = new NutritionController();
            InitializeData();
        }

        private void InitializeData()
        {
            // Khởi tạo dictionary cho các thứ
            _selectedDaysOfWeek["Thứ 2"] = false;
            _selectedDaysOfWeek["Thứ 3"] = false;
            _selectedDaysOfWeek["Thứ 4"] = false;
            _selectedDaysOfWeek["Thứ 5"] = false;
            _selectedDaysOfWeek["Thứ 6"] = false;
            _selectedDaysOfWeek["Thứ 7"] = false;
            _selectedDaysOfWeek["Chủ nhật"] = false;

            // Khởi tạo giờ bắt đầu và kết thúc
            InitializeTimeComboBoxes();

            // Load calendar
            LoadCalendar();

            // Load danh sách món ăn
            LoadFoodList(forceReload: true);

            // Đăng ký event handlers
            RegisterEventHandlers();
        }

        private void InitializeTimeComboBoxes()
        {
            cboGioBatDau.Items.Clear();
            cboGioKetThuc.Items.Clear();

            // Thêm các giờ từ 5:00 đến 23:00
            for (int hour = 5; hour <= 23; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    string timeStr = $"{hour:D2}:{minute:D2}";
                    cboGioBatDau.Items.Add(timeStr);
                    cboGioKetThuc.Items.Add(timeStr);
                }
            }

            // Set mặc định
            cboGioBatDau.SelectedIndex = 0; // 5:00
            cboGioKetThuc.SelectedIndex = cboGioKetThuc.Items.Count - 1; // 23:00
        }

        private void RegisterEventHandlers()
        {
            // Event handlers cho các panel mục tiêu
            guna2Panel15.Click += (s, e) => SelectGoalType("Cơ Ngực");
            guna2Panel16.Click += (s, e) => SelectGoalType("Cơ Mông");
            guna2Panel17.Click += (s, e) => SelectGoalType("Cơ Đùi");
            guna2Panel18.Click += (s, e) => SelectGoalType("Cơ Lưng");
            guna2Panel19.Click += (s, e) => SelectGoalType("Cơ Cổ");
            guna2Panel20.Click += (s, e) => SelectGoalType("Cơ Vai");
            guna2Panel21.Click += (s, e) => SelectGoalType("Tăng Cân");
            guna2Panel22.Click += (s, e) => SelectGoalType("Cơ Tay");
            guna2Panel23.Click += (s, e) => SelectGoalType("Giảm Cân");
            guna2Panel24.Click += (s, e) => SelectGoalType("Cơ Bụng");

            // Event handlers cho các thứ trong tuần
            btnThu2.Click += (s, e) => ToggleDayOfWeek("Thứ 2", btnThu2);
            btnThu3.Click += (s, e) => ToggleDayOfWeek("Thứ 3", btnThu3);
            btnThu4.Click += (s, e) => ToggleDayOfWeek("Thứ 4", btnThu4);
            btnThu5.Click += (s, e) => ToggleDayOfWeek("Thứ 5", btnThu5);
            btnThu6.Click += (s, e) => ToggleDayOfWeek("Thứ 6", btnThu6);
            btnThu7.Click += (s, e) => ToggleDayOfWeek("Thứ 7", btnThu7);
            btnChuNhat.Click += (s, e) => ToggleDayOfWeek("Chủ nhật", btnChuNhat);

            // Event handlers cho radio buttons trình độ
            rdoTatCa.CheckedChanged += (s, e) => { if (rdoTatCa.Checked) LoadExercises(); };
            rdoNguoiMoi.CheckedChanged += (s, e) => { if (rdoNguoiMoi.Checked) LoadExercises(); };
            rdoTrungCap.CheckedChanged += (s, e) => { if (rdoTrungCap.Checked) LoadExercises(); };
            rdoNangCao.CheckedChanged += (s, e) => { if (rdoNangCao.Checked) LoadExercises(); };

            // Event handlers cho calendar navigation
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;

            // Event handler cho dgv bài tập
            dgvBaiTapDeXuat.SelectionChanged += DgvBaiTapDeXuat_SelectionChanged;

            // Event handler cho nút tạo mục tiêu
            btnTaoMucTieu.Click += BtnTaoMucTieu_Click;

            // Event handler khi thay đổi giờ
            cboGioBatDau.SelectedIndexChanged += CboGio_SelectedIndexChanged;
            cboGioKetThuc.SelectedIndexChanged += CboGio_SelectedIndexChanged;

            // Event handlers cho món ăn
            txtTimMonAn.TextChanged += TxtTimMonAn_TextChanged;
            dgvDanhSachMonAn.SelectionChanged += DgvDanhSachMonAn_SelectionChanged;
            dgvDanhSachMonAn.CellDoubleClick += DgvDanhSachMonAn_CellDoubleClick;
        }

        private void SelectGoalType(string loaiMucTieu)
        {
            _selectedLoaiMucTieu = loaiMucTieu;
            
            // Highlight panel được chọn
            ResetGoalPanels();
            HighlightSelectedPanel(loaiMucTieu);

            // Load danh sách bài tập
            LoadExercises();
        }

        private void ResetGoalPanels()
        {
            var panels = new[] { guna2Panel15, guna2Panel16, guna2Panel17, guna2Panel18, guna2Panel19,
                                guna2Panel20, guna2Panel21, guna2Panel22, guna2Panel23, guna2Panel24 };
            
            foreach (var panel in panels)
            {
                panel.FillColor = Color.FromArgb(233, 252, 255);
                panel.BorderThickness = 1;
            }
        }

        private void HighlightSelectedPanel(string loaiMucTieu)
        {
            Guna.UI2.WinForms.Guna2Panel selectedPanel = null;
            
            switch (loaiMucTieu)
            {
                case "Cơ Ngực": selectedPanel = guna2Panel15; break;
                case "Cơ Mông": selectedPanel = guna2Panel16; break;
                case "Cơ Đùi": selectedPanel = guna2Panel17; break;
                case "Cơ Lưng": selectedPanel = guna2Panel18; break;
                case "Cơ Cổ": selectedPanel = guna2Panel19; break;
                case "Cơ Vai": selectedPanel = guna2Panel20; break;
                case "Tăng Cân": selectedPanel = guna2Panel21; break;
                case "Cơ Tay": selectedPanel = guna2Panel22; break;
                case "Giảm Cân": selectedPanel = guna2Panel23; break;
                case "Cơ Bụng": selectedPanel = guna2Panel24; break;
            }

            if (selectedPanel != null)
            {
                selectedPanel.FillColor = Color.FromArgb(100, 88, 255);
                selectedPanel.BorderThickness = 3;
            }
        }

        private void ToggleDayOfWeek(string thuNgay, Guna.UI2.WinForms.Guna2Button button)
        {
            _selectedDaysOfWeek[thuNgay] = !_selectedDaysOfWeek[thuNgay];
            
            if (_selectedDaysOfWeek[thuNgay])
            {
                button.FillColor = Color.FromArgb(100, 88, 255);
                button.ForeColor = Color.White;
                
                // Tạo hoặc cập nhật lịch cho thứ này
                if (!_weeklySchedules.ContainsKey(thuNgay))
                {
                    _weeklySchedules[thuNgay] = new WeeklySchedule
                    {
                        ThuNgay = thuNgay,
                        GioBatDau = ParseTime(cboGioBatDau.SelectedItem?.ToString()),
                        GioKetThuc = ParseTime(cboGioKetThuc.SelectedItem?.ToString()),
                        GhiChu = txtGhiChu.Text
                    };
                }
            }
            else
            {
                button.FillColor = Color.White;
                button.ForeColor = Color.Black;
                _weeklySchedules.Remove(thuNgay);
            }
        }

        private TimeSpan? ParseTime(string timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr))
                return null;

            if (TimeSpan.TryParse(timeStr, out TimeSpan result))
                return result;

            return null;
        }

        private void CboGio_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cập nhật giờ cho các thứ đã chọn
            foreach (var day in _selectedDaysOfWeek.Where(d => d.Value))
            {
                if (_weeklySchedules.ContainsKey(day.Key))
                {
                    _weeklySchedules[day.Key].GioBatDau = ParseTime(cboGioBatDau.SelectedItem?.ToString());
                    _weeklySchedules[day.Key].GioKetThuc = ParseTime(cboGioKetThuc.SelectedItem?.ToString());
                }
            }
        }

        private async void LoadExercises()
        {
            try
            {
                // Clear DataGridView trước
                dgvBaiTapDeXuat.DataSource = null;
                dgvBaiTapDeXuat.Rows.Clear();
                dgvBaiTapDeXuat.Columns.Clear();

                if (string.IsNullOrWhiteSpace(_selectedLoaiMucTieu))
                {
                    return;
                }

                // Map từ tên mục tiêu UI sang giá trị trong database
                var (loaiMucTieuDB, nhomCoChinhNhatDB, searchBy) = MapGoalTypeToDatabase(_selectedLoaiMucTieu);
                
                string capDo = null;
                if (rdoNguoiMoi.Checked) capDo = "Người mới";
                else if (rdoTrungCap.Checked) capDo = "Trung cấp";
                else if (rdoNangCao.Checked) capDo = "Nâng cao";
                // Nếu rdoTatCa.Checked thì capDo = null

                System.Diagnostics.Debug.WriteLine($"=== LoadExercises ===");
                System.Diagnostics.Debug.WriteLine($"Selected Goal Type (UI): {_selectedLoaiMucTieu}");
                System.Diagnostics.Debug.WriteLine($"Mapped - LoaiMucTieu: {loaiMucTieuDB ?? "null"}, NhomCoChinhNhat: {nhomCoChinhNhatDB ?? "null"}, SearchBy: {searchBy}");
                System.Diagnostics.Debug.WriteLine($"Selected Level: {capDo ?? "Tất cả"}");

                var exercises = await _goalController.GetExercisesByGoalAndLevelAsync(loaiMucTieuDB, nhomCoChinhNhatDB, searchBy, capDo);
                
                System.Diagnostics.Debug.WriteLine($"Found {exercises.Count} exercises");

                if (exercises == null || exercises.Count == 0)
                {
                    // Hiển thị thông báo không có dữ liệu
                    dgvBaiTapDeXuat.Columns.Clear();
                    dgvBaiTapDeXuat.Columns.Add("Message", "Thông báo");
                    dgvBaiTapDeXuat.Rows.Add("Không tìm thấy bài tập nào phù hợp với mục tiêu và trình độ đã chọn.");
                    dgvBaiTapDeXuat.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    return;
                }

                // Tạo DataTable để bind dữ liệu
                DataTable dt = new DataTable();
                dt.Columns.Add("BaiTapID", typeof(string));
                dt.Columns.Add("TenBaiTap", typeof(string));
                dt.Columns.Add("NhomCoChinhNhat", typeof(string));
                dt.Columns.Add("CapDo", typeof(string));
                dt.Columns.Add("DungCu", typeof(string));
                dt.Columns.Add("DoPhoBien", typeof(int));

                // Điền dữ liệu vào DataTable
                foreach (var exercise in exercises)
                {
                    DataRow row = dt.NewRow();
                    row["BaiTapID"] = exercise.BaiTapID ?? "";
                    row["TenBaiTap"] = exercise.TenBaiTap ?? "";
                    row["NhomCoChinhNhat"] = exercise.NhomCoChinhNhat ?? "";
                    row["CapDo"] = MapCapDoToVietnamese(exercise.CapDo);
                    row["DungCu"] = exercise.DungCu ?? "";
                    row["DoPhoBien"] = exercise.DoPhoBien ?? 0;
                    dt.Rows.Add(row);
                }

                // Bind DataTable vào DataGridView
                dgvBaiTapDeXuat.DataSource = dt;

                // Cấu hình columns
                if (dgvBaiTapDeXuat.Columns.Count > 0)
                {
                    // Ẩn cột BaiTapID
                    if (dgvBaiTapDeXuat.Columns["BaiTapID"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["BaiTapID"].Visible = false;
                    }

                    // Set header text
                    if (dgvBaiTapDeXuat.Columns["TenBaiTap"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["TenBaiTap"].HeaderText = "Tên bài tập";
                        dgvBaiTapDeXuat.Columns["TenBaiTap"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    if (dgvBaiTapDeXuat.Columns["NhomCoChinhNhat"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["NhomCoChinhNhat"].HeaderText = "Nhóm cơ";
                        dgvBaiTapDeXuat.Columns["NhomCoChinhNhat"].Width = 120;
                    }

                    if (dgvBaiTapDeXuat.Columns["CapDo"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["CapDo"].HeaderText = "Trình độ";
                        dgvBaiTapDeXuat.Columns["CapDo"].Width = 100;
                    }

                    if (dgvBaiTapDeXuat.Columns["DungCu"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["DungCu"].HeaderText = "Dụng cụ";
                        dgvBaiTapDeXuat.Columns["DungCu"].Width = 150;
                    }

                    if (dgvBaiTapDeXuat.Columns["DoPhoBien"] != null)
                    {
                        dgvBaiTapDeXuat.Columns["DoPhoBien"].HeaderText = "Độ phổ biến";
                        dgvBaiTapDeXuat.Columns["DoPhoBien"].Width = 100;
                        dgvBaiTapDeXuat.Columns["DoPhoBien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }

                // Cấu hình DataGridView
                dgvBaiTapDeXuat.AllowUserToAddRows = false;
                dgvBaiTapDeXuat.AllowUserToDeleteRows = false;
                dgvBaiTapDeXuat.ReadOnly = true;
                dgvBaiTapDeXuat.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvBaiTapDeXuat.MultiSelect = false;

                // Clear selection
                dgvBaiTapDeXuat.ClearSelection();

                System.Diagnostics.Debug.WriteLine($"DataGridView loaded with {dt.Rows.Count} rows");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadExercises error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                MessageBox.Show($"Lỗi khi tải danh sách bài tập:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Clear DataGridView nếu có lỗi
                dgvBaiTapDeXuat.DataSource = null;
                dgvBaiTapDeXuat.Rows.Clear();
                dgvBaiTapDeXuat.Columns.Clear();
            }
        }

        /// <summary>
        /// Map từ tên mục tiêu trong UI sang giá trị trong database
        /// Trả về tuple: (LoaiMucTieu, NhomCoChinhNhat, SearchBy)
        /// SearchBy: "LoaiMucTieu" hoặc "NhomCoChinhNhat"
        /// </summary>
        private (string loaiMucTieu, string nhomCoChinhNhat, string searchBy) MapGoalTypeToDatabase(string goalTypeUI)
        {
            // Dựa vào dữ liệu mẫu:
            // - "Tăng Cân", "Giảm Cân" -> tìm theo LoaiMucTieu
            // - "Cơ Ngực", "Cơ Mông", etc. -> tìm theo NhomCoChinhNhat
            switch (goalTypeUI)
            {
                case "Cơ Ngực":
                    return (null, "Ngực", "NhomCoChinhNhat");
                case "Cơ Mông":
                    return (null, "Mông", "NhomCoChinhNhat");
                case "Cơ Đùi":
                    return (null, "Chân", "NhomCoChinhNhat");
                case "Cơ Lưng":
                    return (null, "Lưng", "NhomCoChinhNhat");
                case "Cơ Cổ":
                    return (null, "Cổ", "NhomCoChinhNhat");
                case "Cơ Vai":
                    return (null, "Vai", "NhomCoChinhNhat");
                case "Cơ Tay":
                    return (null, "Tay", "NhomCoChinhNhat");
                case "Cơ Bụng":
                    return (null, "Bụng", "NhomCoChinhNhat");
                case "Tăng Cân":
                    return ("Tăng cân", null, "LoaiMucTieu");
                case "Giảm Cân":
                    return ("Giảm cân", null, "LoaiMucTieu");
                default:
                    // Mặc định tìm theo cả hai
                    return (goalTypeUI, goalTypeUI, "Both");
            }
        }

        /// <summary>
        /// Map từ trình độ tiếng Anh sang tiếng Việt để hiển thị
        /// </summary>
        private string MapCapDoToVietnamese(string capDoEn)
        {
            if (string.IsNullOrWhiteSpace(capDoEn))
                return "N/A";

            switch (capDoEn.ToLower())
            {
                case "beginner":
                    return "Người mới";
                case "intermediate":
                    return "Trung cấp";
                case "advanced":
                    return "Nâng cao";
                case "all levels":
                    return "Tất cả";
                default:
                    return capDoEn;
            }
        }

        private async void DgvBaiTapDeXuat_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBaiTapDeXuat.SelectedRows.Count == 0 || dgvBaiTapDeXuat.DataSource == null)
                return;

            try
            {
                // Lấy BaiTapID từ row được chọn
                DataGridViewRow selectedRow = dgvBaiTapDeXuat.SelectedRows[0];
                
                // Kiểm tra xem có cột BaiTapID không (có thể bị ẩn)
                string baiTapId = null;
                if (selectedRow.Cells["BaiTapID"] != null && selectedRow.Cells["BaiTapID"].Value != null)
                {
                    baiTapId = selectedRow.Cells["BaiTapID"].Value.ToString();
                }
                else
                {
                    // Nếu không có cột BaiTapID, thử lấy từ DataBoundItem
                    if (selectedRow.DataBoundItem is DataRowView rowView)
                    {
                        baiTapId = rowView["BaiTapID"]?.ToString();
                    }
                    else if (dgvBaiTapDeXuat.DataSource is DataTable dt)
                    {
                        int rowIndex = selectedRow.Index;
                        if (rowIndex >= 0 && rowIndex < dt.Rows.Count)
                        {
                            baiTapId = dt.Rows[rowIndex]["BaiTapID"]?.ToString();
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(baiTapId))
                {
                    System.Diagnostics.Debug.WriteLine("BaiTapID is null or empty");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Loading exercise detail for BaiTapID: {baiTapId}");

                var exercise = await _goalController.GetExerciseDetailAsync(baiTapId);
                if (exercise == null)
                {
                    System.Diagnostics.Debug.WriteLine("Exercise not found");
                    return;
                }

                // Hiển thị chi tiết bài tập
                await DisplayExerciseDetail(exercise);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DgvBaiTapDeXuat_SelectionChanged error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                MessageBox.Show($"Lỗi khi tải chi tiết bài tập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DisplayExerciseDetail(ThuVienBaiTap exercise)
        {
            if (exercise == null)
            {
                // Clear tất cả thông tin nếu exercise null
                lblSoKcal.Text = "0";
                lblSoRep.Text = "N/A";
                lblSoSet.Text = "N/A";
                lblThoiLuong.Text = "0";
                lblGioNghi.Text = "0";
                lblDoPhoBien.Text = "0";
                txtHuongDan.Text = "";
                txtLuuY.Text = "";
                return;
            }

            try
            {
                // Hiển thị thông tin chi tiết
                // Kcal (Calories mỗi rep)
                lblSoKcal.Text = exercise.CaloriesMoiRep?.ToString("F1") ?? "0";

                // Số Rep (VD: "8-12")
                lblSoRep.Text = exercise.SoRep ?? "N/A";

                // Số Set (VD: "3-4")
                lblSoSet.Text = exercise.SoSet ?? "N/A";

                // Thời lượng (giây) - format: "X giây" hoặc "X phút Y giây"
                if (exercise.ThoiLuongDeNghi.HasValue && exercise.ThoiLuongDeNghi.Value > 0)
                {
                    int seconds = exercise.ThoiLuongDeNghi.Value;
                    if (seconds >= 60)
                    {
                        int minutes = seconds / 60;
                        int remainingSeconds = seconds % 60;
                        if (remainingSeconds > 0)
                            lblThoiLuong.Text = $"{minutes}p {remainingSeconds}s";
                        else
                            lblThoiLuong.Text = $"{minutes} phút";
                    }
                    else
                    {
                        lblThoiLuong.Text = $"{seconds} giây";
                    }
                }
                else
                {
                    lblThoiLuong.Text = "0";
                }

                // Giờ nghỉ (giây) - format tương tự
                if (exercise.ThoiGianNghi.HasValue && exercise.ThoiGianNghi.Value > 0)
                {
                    int seconds = exercise.ThoiGianNghi.Value;
                    if (seconds >= 60)
                    {
                        int minutes = seconds / 60;
                        int remainingSeconds = seconds % 60;
                        if (remainingSeconds > 0)
                            lblGioNghi.Text = $"{minutes}p {remainingSeconds}s";
                        else
                            lblGioNghi.Text = $"{minutes} phút";
                    }
                    else
                    {
                        lblGioNghi.Text = $"{seconds} giây";
                    }
                }
                else
                {
                    lblGioNghi.Text = "0";
                }

                // Độ phổ biến
                lblDoPhoBien.Text = exercise.DoPhoBien?.ToString() ?? "0";

                // Hướng dẫn
                txtHuongDan.Text = exercise.HuongDan ?? "";

                // Lưu ý
                txtLuuY.Text = exercise.LuuY ?? "";

                // Load video nếu có
                if (!string.IsNullOrWhiteSpace(exercise.VideoHuongDan))
                {
                    try
                    {
                        // Đảm bảo WebView2 được khởi tạo trước khi navigate
                        if (webViewVideoHuongDan.CoreWebView2 == null)
                        {
                            await webViewVideoHuongDan.EnsureCoreWebView2Async();
                        }

                        if (webViewVideoHuongDan.CoreWebView2 != null)
                        {
                            webViewVideoHuongDan.CoreWebView2.Navigate(exercise.VideoHuongDan);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading video: {ex.Message}");
                        // Không hiển thị lỗi cho user, chỉ log
                    }
                }
                else
                {
                    // Nếu không có video, clear WebView2
                    if (webViewVideoHuongDan.CoreWebView2 != null)
                    {
                        webViewVideoHuongDan.CoreWebView2.Navigate("about:blank");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DisplayExerciseDetail error: {ex.Message}");
                MessageBox.Show($"Lỗi khi hiển thị chi tiết bài tập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCalendar()
        {
            UpdateCalendarDisplay();
            UpdateCalendarButtons();
        }

        private void UpdateCalendarDisplay()
        {
            lblThangNam.Text = _currentMonth.ToString("MMMM yyyy", new System.Globalization.CultureInfo("vi-VN"));
            
            // Lấy ngày đầu tiên và cuối cùng của tháng
            DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            
            // Tính ngày đầu tiên trong tuần (có thể là ngày cuối tháng trước)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            // Chuyển đổi: Sunday = 0 -> 7, Monday = 1 -> 1, ..., Saturday = 6 -> 6
            if (firstDayOfWeek == 0) firstDayOfWeek = 7; // Chủ nhật = 7
            firstDayOfWeek -= 1; // Chuyển về 0-based (Thứ 2 = 0, Chủ nhật = 6)
            
            // Lấy danh sách các nút ngày (bỏ qua 7 nút đầu là thứ trong tuần)
            var dateButtons = flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Button>()
                .Where(b => b.Name.StartsWith("btnLich"))
                .OrderBy(b => int.Parse(b.Name.Replace("btnLich", "")))
                .ToList();
            
            // Điền các ngày vào calendar
            DateTime currentDate = firstDayOfMonth.AddDays(-firstDayOfWeek); // Bắt đầu từ ngày đầu tiên hiển thị
            
            for (int i = 0; i < dateButtons.Count; i++)
            {
                var btn = dateButtons[i];
                btn.Text = currentDate.Day.ToString();
                btn.Tag = currentDate; // Lưu ngày vào Tag để dễ truy xuất
                btn.Enabled = true;
                
                // Xác định màu sắc dựa trên việc ngày có thuộc tháng hiện tại không
                if (currentDate.Month == _currentMonth.Month && currentDate.Year == _currentMonth.Year)
                {
                    // Ngày thuộc tháng hiện tại
                    btn.FillColor = Color.FromArgb(233, 252, 255);
                    btn.ForeColor = Color.FromArgb(0, 64, 64);
                }
                else
                {
                    // Ngày thuộc tháng trước hoặc sau
                    btn.FillColor = Color.FromArgb(240, 240, 240);
                    btn.ForeColor = Color.FromArgb(150, 150, 150);
                }
                
                // Highlight nếu nằm trong khoảng đã chọn
                if (_selectedStartDate.HasValue && _selectedEndDate.HasValue)
                {
                    if (currentDate >= _selectedStartDate.Value && currentDate <= _selectedEndDate.Value)
                    {
                        btn.FillColor = Color.FromArgb(100, 88, 255);
                        btn.ForeColor = Color.White;
                    }
                }
                else if (_selectedStartDate.HasValue && currentDate.Date == _selectedStartDate.Value.Date)
                {
                    // Chỉ highlight ngày bắt đầu nếu chưa có ngày kết thúc
                    btn.FillColor = Color.FromArgb(100, 88, 255);
                    btn.ForeColor = Color.White;
                }
                
                // Đăng ký event handler
                btn.Click -= DateButton_Click;
                btn.Click += DateButton_Click;
                
                currentDate = currentDate.AddDays(1);
            }
        }

        private void DateButton_Click(object sender, EventArgs e)
        {
            var button = sender as Guna.UI2.WinForms.Guna2Button;
            if (button == null || button.Tag == null) return;

            // Lấy ngày từ Tag (đã được lưu khi load calendar)
            DateTime selectedDate = (DateTime)button.Tag;
            
            // Chỉ cho phép chọn ngày thuộc tháng hiện tại
            if (selectedDate.Month != _currentMonth.Month || selectedDate.Year != _currentMonth.Year)
            {
                // Nếu click vào ngày tháng trước/sau, chuyển tháng
                if (selectedDate < _currentMonth)
                {
                    _currentMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                    LoadCalendar();
                    return;
                }
                else
                {
                    _currentMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                    LoadCalendar();
                    return;
                }
            }
            
            // Kiểm tra ngày không được trong quá khứ (trước hôm nay)
            if (selectedDate.Date < DateTime.Today)
            {
                MessageBox.Show("Không thể chọn ngày trong quá khứ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Logic chọn ngày: lần đầu = bắt đầu, lần 2 = kết thúc
            if (_isSelectingStartDate)
            {
                // Chọn ngày bắt đầu
                _selectedStartDate = selectedDate;
                _selectedEndDate = null; // Reset ngày kết thúc
                _isSelectingStartDate = false; // Lần sau sẽ chọn ngày kết thúc
            }
            else
            {
                // Chọn ngày kết thúc
                if (!_selectedStartDate.HasValue)
                {
                    // Nếu chưa có ngày bắt đầu, chọn làm ngày bắt đầu
                    _selectedStartDate = selectedDate;
                    _isSelectingStartDate = false;
                }
                else if (selectedDate <= _selectedStartDate.Value)
                {
                    // Nếu chọn ngày trước hoặc bằng ngày bắt đầu, reset và chọn làm ngày bắt đầu mới
                    MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _selectedStartDate = selectedDate;
                    _selectedEndDate = null;
                    _isSelectingStartDate = false;
                }
                else
                {
                    // Chọn ngày kết thúc hợp lệ
                    _selectedEndDate = selectedDate;
                    _isSelectingStartDate = true; // Reset để lần sau chọn lại từ đầu
                }
            }

            UpdateDateButtons();
        }

        private void UpdateDateButtons()
        {
            var dateButtons = flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Button>()
                .Where(b => b.Name.StartsWith("btnLich"))
                .OrderBy(b => int.Parse(b.Name.Replace("btnLich", "")))
                .ToList();

            foreach (var btn in dateButtons)
            {
                if (btn.Tag != null)
                {
                    DateTime btnDate = (DateTime)btn.Tag;
                    
                    // Xác định màu sắc dựa trên việc ngày có thuộc tháng hiện tại không
                    bool isCurrentMonth = btnDate.Month == _currentMonth.Month && btnDate.Year == _currentMonth.Year;
                    
                    // Kiểm tra nếu ngày nằm trong khoảng đã chọn
                    bool isInSelectedRange = false;
                    if (_selectedStartDate.HasValue && _selectedEndDate.HasValue)
                    {
                        isInSelectedRange = btnDate >= _selectedStartDate.Value && btnDate <= _selectedEndDate.Value;
                    }
                    else if (_selectedStartDate.HasValue)
                    {
                        // Chỉ có ngày bắt đầu, highlight ngày đó
                        isInSelectedRange = btnDate.Date == _selectedStartDate.Value.Date;
                    }
                    
                    if (isInSelectedRange)
                    {
                        // Ngày được chọn (trong khoảng hoặc là ngày bắt đầu)
                        btn.FillColor = Color.FromArgb(100, 88, 255);
                        btn.ForeColor = Color.White;
                    }
                    else if (btnDate.Date == _selectedStartDate?.Date)
                    {
                        // Ngày bắt đầu
                        btn.FillColor = Color.FromArgb(100, 88, 255);
                        btn.ForeColor = Color.White;
                    }
                    else if (btnDate.Date == _selectedEndDate?.Date)
                    {
                        // Ngày kết thúc
                        btn.FillColor = Color.FromArgb(100, 88, 255);
                        btn.ForeColor = Color.White;
                    }
                    else if (isCurrentMonth)
                    {
                        // Ngày thuộc tháng hiện tại nhưng chưa chọn
                        btn.FillColor = Color.FromArgb(233, 252, 255);
                        btn.ForeColor = Color.FromArgb(0, 64, 64);
                    }
                    else
                    {
                        // Ngày thuộc tháng trước/sau
                        btn.FillColor = Color.FromArgb(240, 240, 240);
                        btn.ForeColor = Color.FromArgb(150, 150, 150);
                    }
                }
            }
        }

        private void ResetDateButtons()
        {
            var dateButtons = flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Button>()
                .Where(b => b.Name.StartsWith("btnLich")).ToList();
            
            foreach (var btn in dateButtons)
            {
                btn.FillColor = Color.White;
                btn.ForeColor = Color.Black;
            }
        }

        private void UpdateCalendarButtons()
        {
            // Update day-of-week buttons (Thứ 2, Thứ 3, ...)
            // This is a simplified version - you may need to adjust based on your actual calendar implementation
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            LoadCalendar();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            LoadCalendar();
        }

        private async void BtnTaoMucTieu_Click(object sender, EventArgs e)
        {
            if (!CurrentUser.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập để tạo mục tiêu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_selectedLoaiMucTieu))
            {
                MessageBox.Show("Vui lòng chọn loại mục tiêu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_weeklySchedules.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một ngày trong tuần để tập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_selectedStartDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu mục tiêu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_selectedEndDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày kết thúc mục tiêu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedEndDate.Value <= _selectedStartDate.Value)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedStartDate.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Ngày bắt đầu không được trong quá khứ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string userId = CurrentUser.UserID;
                string capDo = rdoNguoiMoi.Checked ? "Beginner" : 
                              (rdoTrungCap.Checked ? "Intermediate" : 
                              (rdoNangCao.Checked ? "Advanced" : "Beginner"));

                // Tạo mục tiêu
                var goalResult = await _goalController.CreateGoalAsync(
                    userId: userId,
                    loaiMucTieu: _selectedLoaiMucTieu,
                    tenMucTieu: $"Mục tiêu {_selectedLoaiMucTieu}",
                    giaTriMucTieu: null,
                    ngayBatDau: _selectedStartDate.Value,
                    ngayKetThucDuKien: _selectedEndDate.Value
                );

                if (!goalResult.Success)
                {
                    MessageBox.Show(goalResult.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo kế hoạch luyện tập
                var weeklySchedulesList = _weeklySchedules.Values.ToList();
                var workoutPlan = await _goalController.CreateWorkoutPlanAsync(
                    userId: userId,
                    mucTieuId: goalResult.Goal.MucTieuID,
                    ngayBatDau: _selectedStartDate.Value,
                    ngayKetThuc: _selectedEndDate.Value,
                    capDo: capDo,
                    weeklySchedules: weeklySchedulesList
                );

                // Lấy bài tập đã chọn từ DataGridView
                string selectedBaiTapId = null;
                if (dgvBaiTapDeXuat.SelectedRows.Count > 0 && dgvBaiTapDeXuat.DataSource != null)
                {
                    var selectedRow = dgvBaiTapDeXuat.SelectedRows[0];
                    if (selectedRow.Cells["BaiTapID"] != null && selectedRow.Cells["BaiTapID"].Value != null)
                    {
                        selectedBaiTapId = selectedRow.Cells["BaiTapID"].Value.ToString();
                    }
                    else if (dgvBaiTapDeXuat.DataSource is DataTable dt)
                    {
                        int rowIndex = selectedRow.Index;
                        if (rowIndex >= 0 && rowIndex < dt.Rows.Count)
                        {
                            selectedBaiTapId = dt.Rows[rowIndex]["BaiTapID"]?.ToString();
                        }
                    }
                }

                // Tạo BaiTapChiTiet cho các buổi tập nếu có bài tập được chọn
                if (!string.IsNullOrWhiteSpace(selectedBaiTapId) && workoutPlan != null)
                {
                    // Lấy tất cả BuoiTap của kế hoạch này
                    var buoiTapList = await _goalController.GetBuoiTapByKeHoachTapIdAsync(workoutPlan.KeHoachTapID);
                    
                    // Lấy số bắt đầu cho BaiTapChiTietID để tránh trùng
                    int startBaiTapChiTietNumber = await _goalController.GetNextBaiTapChiTietNumberAsync();
                    
                    int counter = 0;
                    foreach (var buoiTap in buoiTapList)
                    {
                        await _goalController.AddBaiTapChiTietAsync(
                            buoiTapId: buoiTap.BuoiTapID,
                            baiTapId: selectedBaiTapId,
                            startNumber: startBaiTapChiTietNumber + counter
                        );
                        counter++;
                    }
                }

                // Tạo kế hoạch ăn uống
                string keHoachAnId = null;
                if (_selectedFoods.Count > 0)
                {
                    // Tính tổng dinh dưỡng từ các món ăn đã chọn
                    double tongCalories = _selectedFoods.Sum(f => f.Calories ?? 0);
                    double tongProtein = _selectedFoods.Sum(f => f.Protein ?? 0);
                    double tongCarbs = _selectedFoods.Sum(f => f.Carbs ?? 0);
                    double tongFat = _selectedFoods.Sum(f => f.Fat ?? 0);
                    double tongFiber = _selectedFoods.Sum(f => f.Fiber ?? 0);

                    // Tạo KeHoachAnUong
                    keHoachAnId = await _nutritionController.CreateMealPlanAsync(
                        mucTieuId: goalResult.Goal.MucTieuID,
                        tongCalories: tongCalories,
                        tongProtein: tongProtein,
                        tongCarbs: tongCarbs,
                        tongFat: tongFat,
                        tongFiber: tongFiber,
                        moTa: $"Kế hoạch ăn uống cho mục tiêu {_selectedLoaiMucTieu}"
                    );

                    // Tạo BuaAnChiTiet cho mỗi món ăn (mặc định là bữa trưa, ngày bắt đầu)
                    foreach (var food in _selectedFoods)
                    {
                        await _nutritionController.AddMealToPlanAsync(
                            keHoachAnId: keHoachAnId,
                            monAnId: food.MonAnID,
                            loaiBuaAn: "Trưa", // Mặc định bữa trưa
                            ngayAn: _selectedStartDate.Value,
                            tenMonAn: food.TenMonAn,
                            khoiLuongChuan: food.KhoiLuongChuan,
                            donVi: food.Donvi
                        );
                    }
                }

                // Tạo thông báo chi tiết
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Tạo mục tiêu và kế hoạch luyện tập thành công!");
                sb.AppendLine($"Mục tiêu: {_selectedLoaiMucTieu}");
                sb.AppendLine($"Thời gian: {_selectedStartDate.Value:dd/MM/yyyy} - {_selectedEndDate.Value:dd/MM/yyyy}");
                sb.AppendLine($"Trình độ: {capDo}");
                sb.AppendLine($"Số ngày tập trong tuần: {_weeklySchedules.Count}");
                
                if (!string.IsNullOrWhiteSpace(selectedBaiTapId))
                {
                    var exercise = await _goalController.GetExerciseDetailAsync(selectedBaiTapId);
                    if (exercise != null)
                    {
                        sb.AppendLine($"\nBài tập đã chọn: {exercise.TenBaiTap}");
                    }
                }
                
                if (_selectedFoods.Count > 0)
                {
                    sb.AppendLine($"\nDanh sách món ăn đã chọn ({_selectedFoods.Count} món):");
                    foreach (var food in _selectedFoods)
                    {
                        sb.AppendLine($"  - {food.TenMonAn}");
                    }
                }

                MessageBox.Show(sb.ToString(), "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Reset form
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo mục tiêu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            _selectedLoaiMucTieu = null;
            _selectedStartDate = null;
            _selectedEndDate = null;
            _isSelectingStartDate = true;
            _selectedDaysOfWeek.Keys.ToList().ForEach(k => _selectedDaysOfWeek[k] = false);
            _weeklySchedules.Clear();
            _selectedFoods.Clear();
            _currentSelectedFood = null;
            ResetGoalPanels();
            LoadCalendar();
            LoadExercises();
            LoadFoodList(forceReload: true);
            UpdateSelectedFoodsGrid();
            ClearFoodDetails();
        }

        // ==================== MÓN ĂN ====================

        /// <summary>
        /// Load danh sách món ăn vào DataGridView
        /// </summary>
        private void LoadFoodList(string keyword = null, bool forceReload = false)
        {
            try
            {
                if (forceReload || _foodLibraryCache == null || _foodLibraryCache.Count == 0)
                {
                    _foodLibraryCache = _nutritionController.GetAllFoods();
                }

                IEnumerable<ThuVienMonAn> filteredFoods = _foodLibraryCache;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    string lowerKeyword = keyword.Trim().ToLower();
                    filteredFoods = filteredFoods.Where(food =>
                        (!string.IsNullOrWhiteSpace(food.TenMonAn) && food.TenMonAn.ToLower().Contains(lowerKeyword)) ||
                        (!string.IsNullOrWhiteSpace(food.Loai) && food.Loai.ToLower().Contains(lowerKeyword)));
                }

                var filteredList = filteredFoods.ToList();

                // Tạo DataTable chỉ với 3 cột: Tên món, Loại, Đơn vị (và MonAnID ẩn)
                DataTable dt = new DataTable();
                dt.Columns.Add("MonAnID", typeof(string)); // Ẩn, dùng để lấy chi tiết sau
                dt.Columns.Add("TenMonAn", typeof(string));
                dt.Columns.Add("Loai", typeof(string));
                dt.Columns.Add("Donvi", typeof(string));

                foreach (var food in filteredList)
                {
                    DataRow row = dt.NewRow();
                    row["MonAnID"] = food.MonAnID ?? "";
                    row["TenMonAn"] = food.TenMonAn ?? "";
                    row["Loai"] = string.IsNullOrWhiteSpace(food.Loai) ? "Khác" : food.Loai;
                    row["Donvi"] = string.IsNullOrWhiteSpace(food.Donvi) ? "g" : food.Donvi;
                    dt.Rows.Add(row);
                }

                _suppressFoodSelectionChanged = true;
                dgvDanhSachMonAn.AutoGenerateColumns = true;
                dgvDanhSachMonAn.DataSource = dt;
                dgvDanhSachMonAn.Refresh();

                // Cấu hình columns
                if (dgvDanhSachMonAn.Columns.Count > 0)
                {
                    if (dgvDanhSachMonAn.Columns["MonAnID"] != null)
                        dgvDanhSachMonAn.Columns["MonAnID"].Visible = false;

                    if (dgvDanhSachMonAn.Columns["TenMonAn"] != null)
                    {
                        dgvDanhSachMonAn.Columns["TenMonAn"].HeaderText = "Tên món ăn";
                        dgvDanhSachMonAn.Columns["TenMonAn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    if (dgvDanhSachMonAn.Columns["Loai"] != null)
                    {
                        dgvDanhSachMonAn.Columns["Loai"].HeaderText = "Loại";
                        dgvDanhSachMonAn.Columns["Loai"].Width = 150;
                    }

                    if (dgvDanhSachMonAn.Columns["Donvi"] != null)
                    {
                        dgvDanhSachMonAn.Columns["Donvi"].HeaderText = "Đơn vị";
                        dgvDanhSachMonAn.Columns["Donvi"].Width = 100;
                        dgvDanhSachMonAn.Columns["Donvi"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }

                dgvDanhSachMonAn.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvDanhSachMonAn.MultiSelect = false;
                dgvDanhSachMonAn.ReadOnly = true;
                dgvDanhSachMonAn.ClearSelection();

                _suppressFoodSelectionChanged = false;

                if (filteredList.Count > 0)
                {
                    dgvDanhSachMonAn.Rows[0].Selected = true;
                    DisplayFoodDetails(filteredList[0]);
                }
                else
                {
                    ClearFoodDetails();
                }
                
                System.Diagnostics.Debug.WriteLine($"LoadFoodList: Loaded {filteredList.Count} foods (keyword='{keyword}')");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFoodList error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                _suppressFoodSelectionChanged = false;
                dgvDanhSachMonAn.DataSource = null;
                dgvDanhSachMonAn.Rows.Clear();
                ClearFoodDetails();
            }
        }

        /// <summary>
        /// Xử lý tìm kiếm món ăn
        /// </summary>
        private void TxtTimMonAn_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtTimMonAn.Text?.Trim();
                LoadFoodList(keyword: keyword);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TxtTimMonAn_TextChanged error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý khi chọn món ăn từ danh sách
        /// </summary>
        private void DgvDanhSachMonAn_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressFoodSelectionChanged)
                return;

            if (dgvDanhSachMonAn.SelectedRows.Count == 0 || dgvDanhSachMonAn.DataSource == null)
            {
                ClearFoodDetails();
                return;
            }

            try
            {
                DataGridViewRow selectedRow = dgvDanhSachMonAn.SelectedRows[0];
                string monAnId = null;

                // Lấy MonAnID
                if (selectedRow.Cells["MonAnID"] != null && selectedRow.Cells["MonAnID"].Value != null)
                {
                    monAnId = selectedRow.Cells["MonAnID"].Value.ToString();
                }
                else if (dgvDanhSachMonAn.DataSource is DataTable dt)
                {
                    int rowIndex = selectedRow.Index;
                    if (rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        monAnId = dt.Rows[rowIndex]["MonAnID"]?.ToString();
                    }
                }

                if (string.IsNullOrWhiteSpace(monAnId))
                    return;

                // Lấy thông tin món ăn
                var food = _foodLibraryCache?.FirstOrDefault(f => f.MonAnID == monAnId) 
                           ?? _nutritionController.GetFoodById(monAnId);
                if (food == null)
                    return;

                _currentSelectedFood = food;

                // Hiển thị thông tin chi tiết
                DisplayFoodDetails(food);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DgvDanhSachMonAn_SelectionChanged error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý khi double click vào món ăn để thêm vào danh sách đã chọn
        /// </summary>
        private void DgvDanhSachMonAn_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvDanhSachMonAn.DataSource == null)
                return;

            try
            {
                DataGridViewRow row = dgvDanhSachMonAn.Rows[e.RowIndex];
                string monAnId = null;

                // Lấy MonAnID
                if (row.Cells["MonAnID"] != null && row.Cells["MonAnID"].Value != null)
                {
                    monAnId = row.Cells["MonAnID"].Value.ToString();
                }
                else if (dgvDanhSachMonAn.DataSource is DataTable dt)
                {
                    if (e.RowIndex >= 0 && e.RowIndex < dt.Rows.Count)
                    {
                        monAnId = dt.Rows[e.RowIndex]["MonAnID"]?.ToString();
                    }
                }

                if (string.IsNullOrWhiteSpace(monAnId))
                    return;

                // Lấy thông tin món ăn
                var food = _foodLibraryCache?.FirstOrDefault(f => f.MonAnID == monAnId)
                           ?? _nutritionController.GetFoodById(monAnId);
                if (food == null)
                    return;

                // Kiểm tra xem đã có trong danh sách chưa
                if (_selectedFoods.Any(f => f.MonAnID == food.MonAnID))
                {
                    MessageBox.Show("Món ăn này đã có trong danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Thêm vào danh sách đã chọn
                _selectedFoods.Add(food);
                UpdateSelectedFoodsGrid();

                MessageBox.Show($"Đã thêm '{food.TenMonAn}' vào danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DgvDanhSachMonAn_CellDoubleClick error: {ex.Message}");
                MessageBox.Show($"Lỗi khi thêm món ăn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hiển thị thông tin chi tiết món ăn
        /// </summary>
        private void DisplayFoodDetails(ThuVienMonAn food)
        {
            if (food == null)
            {
                ClearFoodDetails();
                return;
            }

            // Hiển thị thông tin dinh dưỡng
            lblKcal.Text = (food.Calories ?? 0).ToString("F0");
            lblProtein.Text = (food.Protein ?? 0).ToString("F1");
            lblCarbs.Text = (food.Carbs ?? 0).ToString("F1");
            lblFat.Text = (food.Fat ?? 0).ToString("F1");
            lblChatXo.Text = (food.Fiber ?? 0).ToString("F1");
            lblLoai.Text = food.Loai ?? "N/A";

            // Load ảnh nếu có
            picMonAn.Image = null;
            if (!string.IsNullOrWhiteSpace(food.imageURL))
            {
                try
                {
                    if (Uri.IsWellFormedUriString(food.imageURL, UriKind.Absolute))
                    {
                        picMonAn.LoadAsync(food.imageURL);
                    }
                    else
                    {
                        string resolvedPath = ResolveFoodImagePath(food.imageURL);
                        if (!string.IsNullOrWhiteSpace(resolvedPath) && File.Exists(resolvedPath))
                        {
                            using (var img = Image.FromFile(resolvedPath))
                            {
                                picMonAn.Image = new Bitmap(img);
                            }
                        }
                    }
                }
                catch (Exception imgEx)
                {
                    System.Diagnostics.Debug.WriteLine($"DisplayFoodDetails image error: {imgEx.Message}");
                    picMonAn.Image = null;
                }
            }
        }

        /// <summary>
        /// Xóa thông tin chi tiết món ăn
        /// </summary>
        private void ClearFoodDetails()
        {
            lblKcal.Text = "0";
            lblProtein.Text = "0";
            lblCarbs.Text = "0";
            lblFat.Text = "0";
            lblChatXo.Text = "0";
            lblLoai.Text = "N/A";
            picMonAn.Image = null;
            _currentSelectedFood = null;
        }

        private string ResolveFoodImagePath(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
                return null;

            try
            {
                string normalized = rawPath.Replace("/", "\\").Trim('\\');
                if (Path.IsPathRooted(normalized) && File.Exists(normalized))
                {
                    return normalized;
                }

                string baseDir = Application.StartupPath;
                string directPath = Path.Combine(baseDir, normalized);
                if (File.Exists(directPath))
                    return directPath;

                string resourcesPath = Path.Combine(baseDir, "Resources", normalized);
                if (File.Exists(resourcesPath))
                    return resourcesPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ResolveFoodImagePath error: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Cập nhật DataGridView món ăn đã chọn
        /// </summary>
        private void UpdateSelectedFoodsGrid()
        {
            try
            {
                // Tạo DataTable
                DataTable dt = new DataTable();
                dt.Columns.Add("MonAnID", typeof(string));
                dt.Columns.Add("TenMonAn", typeof(string));
                dt.Columns.Add("Loai", typeof(string));
                dt.Columns.Add("Calories", typeof(double));
                dt.Columns.Add("Protein", typeof(double));
                dt.Columns.Add("Carbs", typeof(double));
                dt.Columns.Add("Fat", typeof(double));
                dt.Columns.Add("Fiber", typeof(double));

                // Điền dữ liệu từ danh sách đã chọn
                foreach (var food in _selectedFoods)
                {
                    DataRow row = dt.NewRow();
                    row["MonAnID"] = food.MonAnID ?? "";
                    row["TenMonAn"] = food.TenMonAn ?? "";
                    row["Loai"] = food.Loai ?? "";
                    row["Calories"] = food.Calories ?? 0;
                    row["Protein"] = food.Protein ?? 0;
                    row["Carbs"] = food.Carbs ?? 0;
                    row["Fat"] = food.Fat ?? 0;
                    row["Fiber"] = food.Fiber ?? 0;
                    dt.Rows.Add(row);
                }

                // Bind vào DataGridView
                dgvMonAnDaChon.DataSource = dt;

                // Cấu hình columns
                if (dgvMonAnDaChon.Columns.Count > 0)
                {
                    if (dgvMonAnDaChon.Columns["MonAnID"] != null)
                        dgvMonAnDaChon.Columns["MonAnID"].Visible = false;

                    if (dgvMonAnDaChon.Columns["TenMonAn"] != null)
                    {
                        dgvMonAnDaChon.Columns["TenMonAn"].HeaderText = "Tên món ăn";
                        dgvMonAnDaChon.Columns["TenMonAn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    if (dgvMonAnDaChon.Columns["Loai"] != null)
                    {
                        dgvMonAnDaChon.Columns["Loai"].HeaderText = "Loại";
                        dgvMonAnDaChon.Columns["Loai"].Width = 120;
                    }

                    if (dgvMonAnDaChon.Columns["Calories"] != null)
                    {
                        dgvMonAnDaChon.Columns["Calories"].HeaderText = "Calories";
                        dgvMonAnDaChon.Columns["Calories"].Width = 100;
                        dgvMonAnDaChon.Columns["Calories"].DefaultCellStyle.Format = "F0";
                    }

                    if (dgvMonAnDaChon.Columns["Protein"] != null)
                    {
                        dgvMonAnDaChon.Columns["Protein"].HeaderText = "Protein (g)";
                        dgvMonAnDaChon.Columns["Protein"].Width = 100;
                        dgvMonAnDaChon.Columns["Protein"].DefaultCellStyle.Format = "F1";
                    }

                    if (dgvMonAnDaChon.Columns["Carbs"] != null)
                    {
                        dgvMonAnDaChon.Columns["Carbs"].HeaderText = "Carbs (g)";
                        dgvMonAnDaChon.Columns["Carbs"].Width = 100;
                        dgvMonAnDaChon.Columns["Carbs"].DefaultCellStyle.Format = "F1";
                    }

                    if (dgvMonAnDaChon.Columns["Fat"] != null)
                    {
                        dgvMonAnDaChon.Columns["Fat"].HeaderText = "Fat (g)";
                        dgvMonAnDaChon.Columns["Fat"].Width = 100;
                        dgvMonAnDaChon.Columns["Fat"].DefaultCellStyle.Format = "F1";
                    }

                    if (dgvMonAnDaChon.Columns["Fiber"] != null)
                    {
                        dgvMonAnDaChon.Columns["Fiber"].HeaderText = "Fiber (g)";
                        dgvMonAnDaChon.Columns["Fiber"].Width = 100;
                        dgvMonAnDaChon.Columns["Fiber"].DefaultCellStyle.Format = "F1";
                    }
                }

                dgvMonAnDaChon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvMonAnDaChon.MultiSelect = false;
                dgvMonAnDaChon.ReadOnly = true;
                dgvMonAnDaChon.ClearSelection();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateSelectedFoodsGrid error: {ex.Message}");
            }
        }

        private void ucMucTieu_Load(object sender, EventArgs e)
        {
            // Initialize WebView2 if needed
            if (webViewVideoHuongDan != null && webViewVideoHuongDan.CoreWebView2 == null)
            {
                try
                {
                    webViewVideoHuongDan.EnsureCoreWebView2Async();
                }
                catch
                {
                    // Ignore WebView2 initialization errors
                }
            }

            // Load danh sách món ăn nếu chưa load
            if (dgvDanhSachMonAn.DataSource == null)
            {
                LoadFoodList(forceReload: true);
            }
        }

        /// <summary>
        /// Dispose controllers - được gọi từ Designer
        /// </summary>
        private void DisposeControllers()
        {
            _goalController?.Dispose();
            _nutritionController?.Dispose();
        }

        /// <summary>
        /// Event handler cho button Trở Về - điều hướng về trang chủ (Dashboard)
        /// </summary>
        private void btnTroVe_Click(object sender, EventArgs e)
        {
            try
            {
                // Tìm frmDashBoard
                frmDashBoard parentForm = null;

                // Cách 1: Tìm qua FindForm()
                Form form = this.FindForm();
                if (form is frmDashBoard)
                {
                    parentForm = form as frmDashBoard;
                }
                // Cách 2: Tìm qua Application.OpenForms
                else
                {
                    foreach (Form openForm in Application.OpenForms)
                    {
                        if (openForm is frmDashBoard)
                        {
                            parentForm = openForm as frmDashBoard;
                            break;
                        }
                    }
                }

                if (parentForm != null)
                {
                    // Tạo và load ucDashBoard (trang chủ)
                    ucDashBoard ucDashBoard = new ucDashBoard(parentForm);
                    parentForm.LoadUserControl(ucDashBoard);
                }
                else
                {
                    MessageBox.Show("Không thể tìm thấy form chính để điều hướng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
