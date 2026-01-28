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
using Models = HealthApp.Models;

namespace HealthApp.Views.KeHoachLuyenTap
{
    public partial class ucTrienKhaiBaiTap : UserControl
    {
        private Models.BuoiTap _currentBuoiTap;
        private List<Models.BaiTapChiTiet> _baiTapChiTietList;
        private Timer _timer;
        private TimeSpan _elapsedTime;
        private bool _isRunning;
        private bool _isPaused;
        private DateTime _startTime;
        private TimeSpan _pausedTime;
        private WF_HealthTracker _dbContext;
        private List<ucLichSuTap> _lichSuTapList; // Danh sách các lần tập đã lưu
        private int _soLanTap; // Số lần tập hiện tại

        public ucTrienKhaiBaiTap()
        {
            InitializeComponent();
            _baiTapChiTietList = new List<Models.BaiTapChiTiet>();
            _dbContext = new WF_HealthTracker();
            _lichSuTapList = new List<ucLichSuTap>();
            _soLanTap = 0;
            InitializeTimer();
            ResetTimer();
            InitializeEventHandlers();
            InitializeLichSuTapPanel();
        }


        /// <summary>
        /// Khởi tạo event handlers cho các nút
        /// </summary>
        private void InitializeEventHandlers()
        {
            btnBatDauTap.Click += BtnBatDauTap_Click;
            btnTamNghi.Click += BtnTamNghi_Click;
            btnXong.Click += BtnXong_Click;
            guna2Button2.Click += BtnDatLai_Click;
            if (btnHoanThanh != null)
            {
                btnHoanThanh.Click += BtnHoanThanh_Click;
            }
            if (btnTroVe != null)
            {
                btnTroVe.Click += BtnTroVe_Click;
            }
        }

        /// <summary>
        /// Khởi tạo Timer
        /// </summary>
        private void InitializeTimer()
        {
            _timer = new Timer();
            _timer.Interval = 1000; // 1 giây
            _timer.Tick += Timer_Tick;
            _isRunning = false;
            _isPaused = false;
            _elapsedTime = TimeSpan.Zero;
            _pausedTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Event handler cho Timer Tick
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isRunning && !_isPaused)
            {
                _elapsedTime = DateTime.Now - _startTime - _pausedTime;
                UpdateTimerDisplay();
            }
        }

        /// <summary>
        /// Cập nhật hiển thị thời gian
        /// </summary>
        private void UpdateTimerDisplay()
        {
            txtGio.Text = _elapsedTime.Hours.ToString("D2");
            txtPhut.Text = _elapsedTime.Minutes.ToString("D2");
            txtGiay.Text = _elapsedTime.Seconds.ToString("D2");
        }

        /// <summary>
        /// Khởi tạo panel lịch sử tập
        /// </summary>
        private void InitializeLichSuTapPanel()
        {
            if (pnlLichSuTap != null)
            {
                pnlLichSuTap.Controls.Clear();
                pnlLichSuTap.AutoScroll = true;
            }
        }

        /// <summary>
        /// Reset timer về 00:00:00
        /// </summary>
        private void ResetTimer()
        {
            _elapsedTime = TimeSpan.Zero;
            _pausedTime = TimeSpan.Zero;
            _isRunning = false;
            _isPaused = false;
            _timer.Stop();
            UpdateTimerDisplay();
            UpdateButtonStates();
        }

        /// <summary>
        /// Set BuoiTap để hiển thị và thực hiện bài tập
        /// </summary>
        public void SetBuoiTap(Models.BuoiTap buoiTap)
        {
            _currentBuoiTap = buoiTap;
            if (buoiTap?.BaiTapChiTiet != null)
            {
                _baiTapChiTietList = buoiTap.BaiTapChiTiet.ToList();
            }
            
            // Reset lịch sử tập khi load buổi tập mới
            _soLanTap = 0;
            _lichSuTapList.Clear();
            if (pnlLichSuTap != null)
            {
                pnlLichSuTap.Controls.Clear();
            }
            
            LoadBaiTapData();
        }

        /// <summary>
        /// Load dữ liệu bài tập vào UI
        /// </summary>
        private void LoadBaiTapData()
        {
            try
            {
                if (_currentBuoiTap == null || _baiTapChiTietList == null || _baiTapChiTietList.Count == 0)
                {
                    MessageBox.Show("Không có bài tập nào để hiển thị!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Load ThuVienBaiTap cho mỗi BaiTapChiTiet
                using (var dbContext = new WF_HealthTracker())
                {
                    foreach (var baiTapChiTiet in _baiTapChiTietList)
                    {
                        if (baiTapChiTiet.ThuVienBaiTap == null && !string.IsNullOrEmpty(baiTapChiTiet.BaiTapID))
                        {
                            dbContext.Entry(baiTapChiTiet)
                                .Reference(bt => bt.ThuVienBaiTap)
                                .Load();
                        }
                    }
                }

                // Tính toán và hiển thị thông tin
                LoadWorkoutStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"LoadBaiTapData error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load và hiển thị thống kê bài tập
        /// </summary>
        private void LoadWorkoutStatistics()
        {
            try
            {
                if (_baiTapChiTietList == null || _baiTapChiTietList.Count == 0)
                    return;

                // 1. Số Buổi Tập
                int soBuoiTap = _baiTapChiTietList.Count;
                if (lbGenSoBuoiTap != null)
                {
                    lbGenSoBuoiTap.Text = soBuoiTap.ToString();
                }

                // 2. Thời Lượng Phút (tổng thời lượng đề nghị)
                int tongThoiLuongGiay = 0;
                int tongSet = 0;
                double tongCalories = 0;
                string capDo = "";
                string dungCu = "";
                int? soSet = null;
                int? soRep = null;

                foreach (var btc in _baiTapChiTietList)
                {
                    // Thời lượng: nếu có ThoiLuongDeNghi thì tính, không thì dùng giá trị mặc định
                    if (btc.ThoiLuongDeNghi.HasValue)
                    {
                        tongThoiLuongGiay += btc.ThoiLuongDeNghi.Value * (btc.SoSet ?? 1);
                    }
                    else if (btc.ThuVienBaiTap?.ThoiLuongDeNghi.HasValue == true)
                    {
                        tongThoiLuongGiay += btc.ThuVienBaiTap.ThoiLuongDeNghi.Value * (btc.SoSet ?? 1);
                    }

                    // Tổng Set
                    if (btc.SoSet.HasValue)
                    {
                        tongSet += btc.SoSet.Value;
                        if (!soSet.HasValue) soSet = btc.SoSet.Value;
                    }

                    // Tổng Calories
                    if (btc.Calories.HasValue)
                    {
                        tongCalories += btc.Calories.Value;
                    }
                    else if (btc.ThuVienBaiTap?.CaloriesMoiRep.HasValue == true && btc.SoRep.HasValue)
                    {
                        tongCalories += btc.ThuVienBaiTap.CaloriesMoiRep.Value * btc.SoRep.Value * (btc.SoSet ?? 1);
                    }

                    // CapDo và DungCu (lấy từ bài tập đầu tiên)
                    if (string.IsNullOrEmpty(capDo) && btc.ThuVienBaiTap != null)
                    {
                        capDo = btc.ThuVienBaiTap.CapDo ?? "";
                        dungCu = btc.ThuVienBaiTap.DungCu ?? "";
                    }

                    // Set-Rep (lấy từ bài tập đầu tiên)
                    if (!soRep.HasValue && btc.SoRep.HasValue)
                    {
                        soRep = btc.SoRep.Value;
                    }
                }

                // Hiển thị Thời Lượng Phút
                int thoiLuongPhut = tongThoiLuongGiay / 60;
                if (lbGenThoiLuong != null)
                {
                    lbGenThoiLuong.Text = thoiLuongPhut.ToString();
                }

                // 3. Hiệu Quả (tính từ số bài tập hoàn thành / tổng số bài tập)
                int soBaiTapHoanThanh = _baiTapChiTietList.Count(btc => btc.TrangThai == "Hoàn thành");
                double hieuQua = soBuoiTap > 0 ? (double)soBaiTapHoanThanh / soBuoiTap * 100 : 0;
                if (lbGenHieuQua != null)
                {
                    lbGenHieuQua.Text = $"{hieuQua:F0}%";
                }

                // 4. Calo-Buổi (trung bình calories mỗi buổi)
                double caloTrungBinh = soBuoiTap > 0 ? tongCalories / soBuoiTap : 0;
                if (lbGenCalo != null)
                {
                    lbGenCalo.Text = caloTrungBinh.ToString("F0");
                }

                // 5. Set-Rep
                if (guna2HtmlLabel8 != null && guna2HtmlLabel9 != null)
                {
                    if (soSet.HasValue && soRep.HasValue)
                    {
                        guna2HtmlLabel8.Text = soSet.Value.ToString();
                        guna2HtmlLabel9.Text = soRep.Value.ToString();
                    }
                    else
                    {
                        // Lấy từ ThuVienBaiTap nếu không có trong BaiTapChiTiet
                        var firstBaiTap = _baiTapChiTietList.FirstOrDefault(btc => btc.ThuVienBaiTap != null);
                        if (firstBaiTap?.ThuVienBaiTap != null)
                        {
                            var tvbt = firstBaiTap.ThuVienBaiTap;
                            if (!string.IsNullOrEmpty(tvbt.SoSet))
                            {
                                if (int.TryParse(tvbt.SoSet.Split('-').FirstOrDefault(), out int setValue))
                                {
                                    guna2HtmlLabel8.Text = setValue.ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(tvbt.SoRep))
                            {
                                if (int.TryParse(tvbt.SoRep.Split('-').FirstOrDefault(), out int repValue))
                                {
                                    guna2HtmlLabel9.Text = repValue.ToString();
                                }
                            }
                        }
                    }
                }

                // 6. Mức Độ
                if (lbGenMucDo != null)
                {
                    string mucDoText = "";
                    switch (capDo?.ToLower())
                    {
                        case "beginner":
                            mucDoText = "Người mới";
                            break;
                        case "intermediate":
                            mucDoText = "Trung cấp";
                            break;
                        case "advanced":
                            mucDoText = "Nâng cao";
                            break;
                        case "all levels":
                            mucDoText = "Tất cả";
                            break;
                        default:
                            mucDoText = capDo ?? "N/A";
                            break;
                    }
                    lbGenMucDo.Text = mucDoText;
                }

                // 7. Thiết bị cần
                if (lbGenThietBi != null)
                {
                    lbGenThietBi.Text = string.IsNullOrEmpty(dungCu) ? "Không cần" : dungCu;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadWorkoutStatistics error: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật trạng thái các nút
        /// </summary>
        private void UpdateButtonStates()
        {
            btnBatDauTap.Enabled = !_isRunning;
            btnTamNghi.Enabled = _isRunning;
            btnTamNghi.Text = _isPaused ? "Tiếp tục" : "Tạm Nghỉ";
            btnXong.Enabled = _isRunning;
            guna2Button2.Enabled = true; // Nút Đặt lại luôn enabled (có thể reset khi bắt đầu)
        }

        /// <summary>
        /// Xử lý khi click nút Bắt đầu
        /// </summary>
        private void BtnBatDauTap_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    _isPaused = false;
                    _startTime = DateTime.Now;
                    _pausedTime = TimeSpan.Zero;
                    _timer.Start();
                    UpdateButtonStates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi bắt đầu bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi click nút Tạm nghỉ/Tiếp tục
        /// </summary>
        private void BtnTamNghi_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isRunning)
                {
                    if (_isPaused)
                    {
                        // Tiếp tục
                        _isPaused = false;
                        _pausedTime = _pausedTime.Add(DateTime.Now - _pauseStartTime);
                        _timer.Start();
                    }
                    else
                    {
                        // Tạm nghỉ
                        _isPaused = true;
                        _pauseStartTime = DateTime.Now;
                        _timer.Stop();
                    }
                    UpdateButtonStates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạm nghỉ: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DateTime _pauseStartTime;

        /// <summary>
        /// Xử lý khi click nút Xong (lưu lần tập hiện tại)
        /// </summary>
        private void BtnXong_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isRunning)
                {
                    // Dừng timer
                    _isRunning = false;
                    _isPaused = false;
                    _timer.Stop();

                    // Tăng số lần tập
                    _soLanTap++;
                    
                    // Lưu thời gian tập hiện tại
                    TimeSpan thoiGianTap = _elapsedTime;

                    // Tạo và thêm ucLichSuTap vào panel
                    AddLichSuTapItem(_soLanTap, thoiGianTap);

                    MessageBox.Show($"Đã lưu lần tập: {thoiGianTap.Hours:D2}:{thoiGianTap.Minutes:D2}:{thoiGianTap.Seconds:D2}", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset timer để có thể tập tiếp
                    ResetTimer();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu lần tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Thêm một item lịch sử tập vào panel
        /// </summary>
        private void AddLichSuTapItem(int lanTap, TimeSpan thoiGianTap)
        {
            try
            {
                if (pnlLichSuTap == null)
                    return;

                // Tạo ucLichSuTap mới
                var ucLichSu = new ucLichSuTap();
                ucLichSu.SetHistoryInfo(lanTap, thoiGianTap);
                
                // Tắt AutoSize để không tự động thay đổi kích thước
                ucLichSu.AutoSize = false;
                
                // Chỉ set Anchor để giữ vị trí khi panel resize (không ảnh hưởng kích thước)
                ucLichSu.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                
                // Tính vị trí Y dựa trên kích thước gốc từ Designer (462x42)
                int spacing = 2; // Khoảng cách giữa các item
                int itemHeight = 42; // Chiều cao cố định từ Designer
                int yPosition = _lichSuTapList.Count * (itemHeight + spacing);
                
                // Set vị trí
                ucLichSu.Location = new Point(0, yPosition);
                
                // Set kích thước cố định TRƯỚC KHI thêm vào panel (462 = chiều rộng của panel)
                ucLichSu.Size = new Size(462, 42);
                ucLichSu.Width = 462;
                ucLichSu.Height = 42;

                // Thêm vào panel và list
                pnlLichSuTap.Controls.Add(ucLichSu);
                _lichSuTapList.Add(ucLichSu);

                // QUAN TRỌNG: Set lại kích thước SAU KHI thêm vào panel để override bất kỳ scaling nào
                // Điều này đảm bảo kích thước không bị thay đổi bởi AutoScaleMode hoặc layout
                ucLichSu.Size = new Size(462, 42);
                ucLichSu.Width = 462;
                ucLichSu.Height = 42;

                // Scroll xuống item mới nhất
                pnlLichSuTap.ScrollControlIntoView(ucLichSu);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddLichSuTapItem error: {ex.Message}");
            }
        }

        /// <summary>
        /// Xử lý khi click nút Hoàn thành (hoàn thành bài tập và quay về)
        /// </summary>
        private void BtnHoanThanh_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Bạn có chắc chắn muốn hoàn thành buổi tập và quay về kế hoạch luyện tập?",
                    "Xác nhận hoàn thành",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Dừng timer nếu đang chạy
                    if (_isRunning)
                    {
                        _isRunning = false;
                        _isPaused = false;
                        _timer.Stop();
                    }

                    // Lưu kết quả vào database (chỉ khi nhấn Hoàn thành)
                    SaveWorkoutResult();

                    MessageBox.Show("Đã hoàn thành buổi tập!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Quay về ucKeHoachLuyenTap
                    NavigateBackToWorkoutPlan();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hoàn thành bài tập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi click nút Trở về
        /// </summary>
        private void BtnTroVe_Click(object sender, EventArgs e)
        {
            NavigateBackToWorkoutPlan();
        }

        /// <summary>
        /// Điều hướng về ucKeHoachLuyenTap
        /// </summary>
        private void NavigateBackToWorkoutPlan()
        {
            try
            {
                // Tìm frmDashBoard1 bằng cách đi lên parent controls
                Control parent = this.Parent;
                HealthApp.Views.Dashboard.frmDashBoard1 dashboard = null;

                while (parent != null)
                {
                    if (parent is HealthApp.Views.Dashboard.frmDashBoard1)
                    {
                        dashboard = parent as HealthApp.Views.Dashboard.frmDashBoard1;
                        break;
                    }
                    parent = parent.Parent;
                }

                // Nếu không tìm thấy trong parent, thử FindForm
                if (dashboard == null)
                {
                    Form form = this.FindForm();
                    dashboard = form as HealthApp.Views.Dashboard.frmDashBoard1;
                }

                if (dashboard != null)
                {
                    var ucKeHoach = new ucKeHoachLuyenTap();
                    dashboard.LoadUserControl(ucKeHoach);
                }
                else
                {
                    // Nếu không tìm thấy dashboard, đóng form hiện tại
                    Form parentForm = this.ParentForm ?? this.FindForm();
                    if (parentForm != null && parentForm != Application.OpenForms[0])
                    {
                        parentForm.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NavigateBackToWorkoutPlan error: {ex.Message}");
                MessageBox.Show($"Lỗi khi điều hướng: {ex.Message}", "Lỗi",
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
                if (_isRunning)
                {
                    var result = MessageBox.Show(
                        "Bạn có chắc chắn muốn đặt lại thời gian? Thời gian hiện tại sẽ bị mất.",
                        "Xác nhận",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        ResetTimer();
                    }
                }
                else
                {
                    // Nếu không đang chạy, reset luôn không cần hỏi
                    ResetTimer();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt lại: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Lưu kết quả buổi tập vào database
        /// </summary>
        private void SaveWorkoutResult()
        {
            try
            {
                if (_currentBuoiTap == null)
                    return;

                using (var dbContext = new WF_HealthTracker())
                {
                    var buoiTap = dbContext.BuoiTap.FirstOrDefault(b => b.BuoiTapID == _currentBuoiTap.BuoiTapID);
                    if (buoiTap != null)
                    {
                        buoiTap.TrangThai = "Hoàn thành";
                        // Ngày thực hiện dùng ngày hiện tại, còn ThoiGianBatDau/ThoiGianKetThuc
                        // vẫn giữ nguyên giá trị lịch gốc để không làm lệch lịch tuần
                        buoiTap.NgayThucHien = DateTime.Now;
                        buoiTap.NgayCapNhat = DateTime.Now;
                        
                        // Thời gian thực tế chỉ lưu vào GhiChu để tham khảo
                        DateTime thoiGianKetThucThucTe = DateTime.Now;
                        DateTime thoiGianBatDauThucTe = thoiGianKetThucThucTe - _elapsedTime;
                        
                        string thoiGianThucTeText =
                            $"Thời gian tập thực tế: {_elapsedTime.Hours:D2}:{_elapsedTime.Minutes:D2}:{_elapsedTime.Seconds:D2} " +
                            $"(bắt đầu: {thoiGianBatDauThucTe:HH:mm}, kết thúc: {thoiGianKetThucThucTe:HH:mm})";
                        
                        // Lưu thời gian tập vào GhiChu để dễ đọc
                        if (string.IsNullOrEmpty(buoiTap.GhiChu))
                        {
                            buoiTap.GhiChu = thoiGianThucTeText;
                        }
                        else
                        {
                            buoiTap.GhiChu += $" | {thoiGianThucTeText}";
                        }
                        
                        // Cập nhật calories nếu có
                        if (buoiTap.Calories == null || buoiTap.Calories == 0)
                        {
                            // Tính calories từ BaiTapChiTiet
                            double tongCalories = 0;
                            foreach (var btc in _baiTapChiTietList)
                            {
                                if (btc.Calories.HasValue)
                                {
                                    tongCalories += btc.Calories.Value;
                                }
                            }
                            buoiTap.Calories = tongCalories > 0 ? (double?)tongCalories : null;
                        }

                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveWorkoutResult error: {ex.Message}");
                // Không hiển thị lỗi cho user, chỉ log
            }
        }

        /// <summary>
        /// Dispose timer resources
        /// </summary>
        private void DisposeTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Dispose database context
        /// </summary>
        private void DisposeDbContext()
        {
            _dbContext?.Dispose();
        }
    }
}
