using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using HealthApp.Services;
using HealthApp.Services.Interfaces;
using HealthApp.Common.Helpers;
using HealthApp.Models;

namespace HealthApp.Views.PT
{
    public partial class YeuCauTapLuyen : Form
    {
        private readonly frm_TimKiemHLV _parentForm;
        private readonly string _ptId;
        private readonly List<string> _danhSachChuyenMon;
        private List<UserControlChonNgay> _listUserControls;
        private readonly IPTDashboardService _ptDashboardService;
        private readonly WF_HealthTracker _context;

        // Danh sách mục tiêu đầy đủ
        private readonly List<string> _tatCaMucTieu = new List<string>
        {
            "Cơ Ngực", "Cơ Lưng", "Cơ Vai", "Cơ Tay", "Cơ Bụng", "Cơ Mông", "Cơ Đùi", "Cơ Cổ",
            "Tăng cân", "Giảm cân"
        };

        // Mục tiêu cho "Cân nặng"
        private readonly List<string> _mucTieuCanNang = new List<string>
        {
            "Tăng cân", "Giảm cân"
        };

        // Mục tiêu cho "Tăng cơ"
        private readonly List<string> _mucTieuTangCo = new List<string>
        {
            "Cơ Ngực", "Cơ Lưng", "Cơ Vai", "Cơ Tay", "Cơ Bụng", "Cơ Mông", "Cơ Đùi", "Cơ Cổ"
        };

        public YeuCauTapLuyen(frm_TimKiemHLV parentForm = null, string ptId = null, List<string> danhSachChuyenMon = null)
        {
            InitializeComponent();
            _parentForm = parentForm;
            _ptId = ptId;
            _danhSachChuyenMon = danhSachChuyenMon ?? new List<string>();
            _listUserControls = new List<UserControlChonNgay>();
            _context = new WF_HealthTracker();
            _ptDashboardService = new PTDashboardService(_context);
            
            InitializeEventHandlers();
            LoadMucTieu();
        }

        private void InitializeEventHandlers()
        {
            btnBack.Click += BtnBack_Click;
            btnThem.Click += BtnThem_Click;
            btnGuiYeuCau.Click += BtnGuiYeuCau_Click;
            btnHuy.Click += BtnHuy_Click;
            btnThemLichTrinh.Click += BtnThemLichTrinh_Click;
        }

        /// <summary>
        /// Load mục tiêu vào combobox dựa trên chuyên môn của PT
        /// </summary>
        private void LoadMucTieu()
        {
            try
            {
                cboMucTieuLuyenTap.Items.Clear();

                List<string> mucTieuToShow = new List<string>();

                // Kiểm tra chuyên môn của PT
                bool hasCanNang = _danhSachChuyenMon.Any(cm => cm.Contains("Cân nặng"));
                bool hasTangCo = _danhSachChuyenMon.Any(cm => cm.Contains("Tăng cơ"));

                if (hasCanNang && hasTangCo)
                {
                    // "Cân nặng Tăng cơ" -> hiển thị tất cả
                    mucTieuToShow = _tatCaMucTieu;
                }
                else if (hasCanNang)
                {
                    // Chỉ "Cân nặng" -> chỉ hiển thị Tăng cân, Giảm cân
                    mucTieuToShow = _mucTieuCanNang;
                }
                else if (hasTangCo)
                {
                    // Chỉ "Tăng cơ" -> chỉ hiển thị các cơ
                    mucTieuToShow = _mucTieuTangCo;
                }
                else
                {
                    // Không có chuyên môn -> hiển thị tất cả (mặc định)
                    mucTieuToShow = _tatCaMucTieu;
                }

                // Thêm vào combobox
                foreach (var mucTieu in mucTieuToShow)
                {
                    cboMucTieuLuyenTap.Items.Add(mucTieu);
                }

                // Chọn item đầu tiên nếu có
                if (cboMucTieuLuyenTap.Items.Count > 0)
                {
                    cboMucTieuLuyenTap.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi load mục tiêu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Thêm UserControl chọn ngày mới
        /// </summary>
        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                var userControl = new UserControlChonNgay();
                userControl.SetParentPanel(pnlChonNgay);
                userControl.OnDeleteRequested += UserControl_OnDeleteRequested;

                // Tự động tăng ngày nếu trùng với các UserControl hiện có
                bool hasConflict = true;
                int maxAttempts = 365; // Giới hạn số lần thử để tránh vòng lặp vô hạn
                int attempts = 0;

                while (hasConflict && attempts < maxAttempts)
                {
                    hasConflict = false;
                    
                    // Kiểm tra trùng ngày với các UserControl hiện có
                    foreach (var existingUC in _listUserControls)
                    {
                        if (userControl.HasSameDate(existingUC))
                        {
                            // Tự động tăng ngày lên 1 ngày
                            userControl.IncrementDate();
                            hasConflict = true;
                            attempts++;
                            break; // Thoát vòng lặp để kiểm tra lại từ đầu
                        }
                    }
                }

                // Sau khi đã xử lý trùng ngày, kiểm tra trùng giờ (cùng ngày)
                foreach (var existingUC in _listUserControls)
                {
                    if (userControl.IsOverlapping(existingUC))
                    {
                        MessageBox.Show("Lịch này trùng giờ với lịch đã chọn trước đó! Vui lòng chọn giờ khác.", 
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        userControl.Dispose();
                        return;
                    }
                }

                _listUserControls.Add(userControl);
                pnlChonNgay.Controls.Add(userControl);

                // Sắp xếp lại các control theo chiều dọc
                ArrangeUserControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm ngày: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi UserControl yêu cầu xóa
        /// </summary>
        private void UserControl_OnDeleteRequested(UserControlChonNgay userControl)
        {
            try
            {
                if (_listUserControls.Contains(userControl))
                {
                    _listUserControls.Remove(userControl);
                    pnlChonNgay.Controls.Remove(userControl);
                    userControl.Dispose();
                    ArrangeUserControls();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa ngày: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Kiểm tra trùng lịch giữa tất cả các UserControl
        /// </summary>
        private bool CheckOverlappingSchedules()
        {
            for (int i = 0; i < _listUserControls.Count; i++)
            {
                for (int j = i + 1; j < _listUserControls.Count; j++)
                {
                    if (_listUserControls[i].IsOverlapping(_listUserControls[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Sắp xếp lại các UserControl theo chiều dọc
        /// </summary>
        private void ArrangeUserControls()
        {
            int yPos = 0;
            int spacing = 10;

            foreach (var uc in _listUserControls)
            {
                uc.Location = new Point(0, yPos);
                yPos += uc.Height + spacing;
            }
        }

        /// <summary>
        /// Xử lý khi click nút Gửi Yêu Cầu
        /// </summary>
        private async void BtnGuiYeuCau_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (cboMucTieuLuyenTap.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn mục tiêu luyện tập!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_listUserControls.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một ngày tập luyện!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra đăng nhập
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi gửi yêu cầu!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate từng UserControl
                foreach (var uc in _listUserControls)
                {
                    if (!uc.ValidateData())
                    {
                        MessageBox.Show("Vui lòng kiểm tra lại thông tin ngày và giờ đã chọn!", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Kiểm tra trùng lịch trong form (giữa các UserControl)
                if (CheckOverlappingSchedules())
                {
                    MessageBox.Show("Có lịch trùng nhau trong danh sách! Vui lòng kiểm tra lại các ngày và giờ đã chọn.", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy mục tiêu
                string mucTieu = cboMucTieuLuyenTap.SelectedItem.ToString();
                string khachHangID = CurrentUser.User.UserID;

                // Chuẩn bị danh sách ngày giờ để kiểm tra trùng với database
                var danhSachNgayGio = new List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)>();
                foreach (var uc in _listUserControls)
                {
                    var (ngay, gioBatDau, gioKetThuc) = uc.GetData();
                    danhSachNgayGio.Add((ngay, gioBatDau, gioKetThuc));
                }

                // Kiểm tra trùng lịch với các lịch đã có của PT trong database
                var overlappingSchedules = await _ptDashboardService.CheckOverlappingSchedulesAsync(_ptId, danhSachNgayGio);
                if (overlappingSchedules != null && overlappingSchedules.Count > 0)
                {
                    // Tạo thông báo chi tiết về các lịch bị trùng
                    var message = new System.Text.StringBuilder();
                    message.AppendLine("Có lịch bị trùng với lịch đã có của PT:");
                    message.AppendLine();
                    
                    foreach (var (ngay, gioBatDau, gioKetThuc) in overlappingSchedules)
                    {
                        message.AppendLine($"• {ngay:dd/MM/yyyy} từ {gioBatDau:hh\\:mm} đến {gioKetThuc:hh\\:mm}");
                    }
                    message.AppendLine();
                    message.AppendLine("Vui lòng chọn thời gian khác!");

                    MessageBox.Show(message.ToString(), "Lịch bị trùng", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra trùng lịch với lịch tập của khách hàng (BuoiTap trong KeHoachLuyenTap)
                var overlappingWorkouts = await _ptDashboardService.CheckOverlappingWithCustomerWorkoutAsync(khachHangID, danhSachNgayGio);
                if (overlappingWorkouts != null && overlappingWorkouts.Count > 0)
                {
                    // Tạo thông báo chi tiết về các lịch bị trùng với lịch tập của khách hàng
                    var message = new System.Text.StringBuilder();
                    message.AppendLine("Có lịch bị trùng với lịch tập của bạn:");
                    message.AppendLine();
                    
                    foreach (var (ngay, gioBatDau, gioKetThuc, tenKeHoach) in overlappingWorkouts)
                    {
                        message.AppendLine($"• {ngay:dd/MM/yyyy} từ {gioBatDau:hh\\:mm} đến {gioKetThuc:hh\\:mm}");
                        message.AppendLine($"  (Kế hoạch: {tenKeHoach})");
                    }
                    message.AppendLine();
                    message.AppendLine("Bạn có muốn tiếp tục gửi yêu cầu không?");

                    var result = MessageBox.Show(message.ToString(), "Cảnh báo trùng lịch", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    
                    if (result == DialogResult.No)
                    {
                        return; // Không tiếp tục gửi yêu cầu
                    }
                    // Nếu chọn Yes, tiếp tục gửi yêu cầu
                }

                // Kiểm tra xem có phải lịch trình (nhiều ngày) không
                // Nếu có nhiều hơn 1 ngày, sử dụng CreateTrainingScheduleAsync để tạo cùng LichTrinhID
                if (_listUserControls.Count > 1)
                {
                    // Tạo lịch trình (nhiều ngày)
                    // Sử dụng lại danhSachNgayGio đã tạo ở trên
                    try
                    {
                        var (datLichIDs, lichTrinhID) = await _ptDashboardService.CreateTrainingScheduleAsync(
                            khachHangID,
                            _ptId,
                            danhSachNgayGio,
                            mucTieu
                        );

                        if (datLichIDs != null && datLichIDs.Count > 0)
                        {
                            MessageBox.Show($"Đã gửi thành công {datLichIDs.Count} yêu cầu tập luyện!\nLịch trình ID: {lichTrinhID}", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không thể gửi yêu cầu! Vui lòng thử lại sau.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi gửi lịch trình: {ex.Message}", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // Buổi tập đơn lẻ
                    int successCount = 0;
                    int failCount = 0;

                    foreach (var uc in _listUserControls)
                    {
                        try
                        {
                            var (ngay, gioBatDau, gioKetThuc) = uc.GetData();
                            
                            // Lưu vào database
                            string datLichID = await _ptDashboardService.CreateTrainingRequestAsync(
                                khachHangID, 
                                _ptId, 
                                ngay, 
                                gioBatDau, 
                                gioKetThuc, 
                                mucTieu
                            );

                            if (!string.IsNullOrEmpty(datLichID))
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                            }
                        }
                        catch (Exception)
                        {
                            failCount++;
                            // Log lỗi nhưng tiếp tục với các yêu cầu khác
                            // Lỗi đã được log trong Debug, không cần hiển thị cho user
                        }
                    }

                    // Hiển thị kết quả
                    if (successCount > 0)
                    {
                        string message = $"Đã gửi thành công {successCount} yêu cầu tập luyện!";
                        if (failCount > 0)
                        {
                            message += $"\nCó {failCount} yêu cầu không thể gửi.";
                        }
                        MessageBox.Show(message, "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể gửi yêu cầu! Vui lòng thử lại sau.", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Quay lại form tìm kiếm
                BtnBack_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi yêu cầu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi click nút Hủy
        /// </summary>
        private void BtnHuy_Click(object sender, EventArgs e)
        {
            BtnBack_Click(sender, e);
        }

        /// <summary>
        /// Xử lý khi click nút Thêm Lịch Trình
        /// </summary>
        private void BtnThemLichTrinh_Click(object sender, EventArgs e)
        {
            try
            {
                var formLichTrinh = new frm_ThueTheoLichTrinh();
                if (formLichTrinh.ShowDialog() == DialogResult.OK)
                {
                    // Lấy danh sách ngày đã chọn
                    var selectedDates = formLichTrinh.SelectedDates;
                    
                    if (selectedDates != null && selectedDates.Count > 0)
                    {
                        int countBefore = _listUserControls.Count;
                        
                        // Thêm từng ngày vào danh sách UserControl
                        // Mỗi ngày sẽ có giờ mặc định (6:00-7:00), user có thể chỉnh sửa sau
                        foreach (var date in selectedDates)
                        {
                            // Chỉ thêm ngày trong tương lai
                            if (date.Date > DateTime.Today)
                            {
                                AddUserControlForDate(date);
                            }
                        }
                        
                        int countAdded = _listUserControls.Count - countBefore;
                        
                        if (countAdded > 0)
                        {
                            MessageBox.Show($"Đã thêm {countAdded} ngày vào lịch trình!\nBạn có thể chỉnh sửa giờ cho từng ngày nếu cần.", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không có ngày nào được thêm. Có thể các ngày đã tồn tại trong danh sách.", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form lịch trình: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Thêm UserControl cho một ngày cụ thể
        /// </summary>
        private void AddUserControlForDate(DateTime date)
        {
            try
            {
                // Kiểm tra xem có trùng ngày với các UserControl hiện có không
                bool hasConflict = false;
                foreach (var existingUC in _listUserControls)
                {
                    if (existingUC.GetData().ngay.Date == date.Date)
                    {
                        hasConflict = true;
                        break;
                    }
                }

                if (hasConflict)
                {
                    // Ngày đã tồn tại, bỏ qua
                    return;
                }

                // Chỉ thêm ngày trong tương lai
                if (date.Date <= DateTime.Today)
                {
                    return;
                }

                var userControl = new UserControlChonNgay();
                userControl.SetParentPanel(pnlChonNgay);
                userControl.OnDeleteRequested += UserControl_OnDeleteRequested;

                // Set ngày cho UserControl sử dụng method mới
                userControl.SetDate(date);

                // Kiểm tra trùng giờ với các UserControl hiện có (nếu cùng ngày)
                // Lưu ý: Các UserControl mới sẽ có giờ mặc định (6:00-7:00), 
                // nếu trùng có thể để user tự chỉnh sửa sau

                _listUserControls.Add(userControl);
                pnlChonNgay.Controls.Add(userControl);

                // Sắp xếp lại các control theo chiều dọc
                ArrangeUserControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm ngày {date:dd/MM/yyyy}: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xử lý khi click nút Back
        /// </summary>
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                if (_parentForm != null && !_parentForm.IsDisposed)
                {
                    _parentForm.Show();
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
            // Xóa tất cả UserControls
            foreach (var uc in _listUserControls)
            {
                uc.Dispose();
            }
            _listUserControls.Clear();
            _context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
