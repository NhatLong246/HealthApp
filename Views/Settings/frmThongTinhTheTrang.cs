using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HealthApp.Common.Helpers;
using HealthApp.Models;

namespace HealthApp.Views.Settings
{
    public partial class frmThongTinhTheTrang : Form
    {
        private readonly bool _isMandatory;
        private bool _isSaved;
        private bool _isEditMode;
        private bool _isNewUser; // Theo dõi xem có phải người mới không
        private TinhTrangTongQuan _currentRecord;
        private List<TinhTrangTongQuan> _historyRecords;
        private string _selectedHistoryDate;

        public frmThongTinhTheTrang(bool isMandatory = false)
        {
            InitializeComponent();
            _isMandatory = isMandatory;
            _isEditMode = false;
            _historyRecords = new List<TinhTrangTongQuan>();

            // Ẩn nút bỏ qua nếu bắt buộc
            ControlBox = !_isMandatory;

            // Gắn event handlers
            Load += FrmThongTinhTheTrang_Load;
            FormClosing += FrmThongTinhTheTrang_FormClosing;
            btnCapNhat.Click += BtnCapNhat_Click;
            txtChieuCao.TextChanged += TxtChieuCao_TextChanged;
            txtCanNang.TextChanged += TxtCanNang_TextChanged;
            cbcLichSu.SelectedIndexChanged += CbcLichSu_SelectedIndexChanged;
        }

        private void FrmThongTinhTheTrang_Load(object sender, EventArgs e)
        {
            try
            {
                if (!CurrentUser.IsLoggedIn || CurrentUser.User == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập để xem thông tin thể trạng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                LoadUserBasicInfo();
                LoadBodyStatusData();
                InitializeTrinhDoComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load thông tin cơ bản từ user (giới tính, tuổi)
        /// </summary>
        private void LoadUserBasicInfo()
        {
            var user = CurrentUser.User;
            if (user == null) return;

            // Hiển thị giới tính (không thể chỉnh sửa)
            txtGioiTinh.Text = user.GioiTinh ?? "";
            txtGioiTinh.ReadOnly = true;
            txtGioiTinh.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);

            // Tính và hiển thị tuổi (không thể chỉnh sửa)
            if (user.NgaySinh.HasValue)
            {
                var age = CalculateAge(user.NgaySinh.Value);
                txtTuoi.Text = age.ToString();
            }
            txtTuoi.ReadOnly = true;
            txtTuoi.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);

            // Set ngày cập nhật mặc định (không thể chỉnh sửa)
            txtNgayCapNhat.Text = DateTime.Today.ToString("dd/MM/yyyy");
            txtNgayCapNhat.ReadOnly = true;
            txtNgayCapNhat.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
        }

        /// <summary>
        /// Tính tuổi từ ngày sinh
        /// </summary>
        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return Math.Max(0, age);
        }

        /// <summary>
        /// Khởi tạo ComboBox trình độ
        /// </summary>
        private void InitializeTrinhDoComboBox()
        {
            cbcTrinhDo.Items.Clear();
            cbcTrinhDo.Items.Add("Người mới");
            cbcTrinhDo.Items.Add("Có kinh nghiệm");
            cbcTrinhDo.Items.Add("Chuyên gia");
        }

        /// <summary>
        /// Load dữ liệu thể trạng từ database
        /// </summary>
        private void LoadBodyStatusData()
        {
            try
            {
                var user = CurrentUser.User;
                if (user == null) return;

                using (var db = new WF_HealthTracker())
                {
                    // Load tất cả lịch sử, sắp xếp theo NgayCapNhat (nếu có) hoặc NgayGhiNhan
                    _historyRecords = db.TinhTrangTongQuan
                        .Where(t => t.UserID == user.UserID)
                        .OrderByDescending(t => t.NgayCapNhat ?? t.NgayGhiNhan)
                        .ThenByDescending(t => t.NgayGhiNhan)
                        .ToList();

                    // Load bản ghi mới nhất
                    _currentRecord = _historyRecords.FirstOrDefault();

                    if (_currentRecord == null)
                    {
                        // Người dùng mới - chưa có dữ liệu
                        _isNewUser = true;
                        SetNewUserMode();
                    }
                    else
                    {
                        // Người dùng đã có dữ liệu
                        _isNewUser = false;
                        SetExistingUserMode();
                        LoadRecordData(_currentRecord);
                        LoadHistoryComboBox();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu thể trạng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Chế độ người dùng mới
        /// </summary>
        private void SetNewUserMode()
        {
            _isEditMode = true;
            lblLichSu.Visible = false;
            cbcLichSu.Visible = false;
            btnCapNhat.Text = "Cập nhật";
            btnCapNhat.Visible = true;

            // Cho phép nhập các trường có thể chỉnh sửa
            SetEditableFields(true);
        }

        /// <summary>
        /// Chế độ người dùng đã có dữ liệu
        /// </summary>
        private void SetExistingUserMode()
        {
            _isEditMode = false;
            lblLichSu.Visible = true;
            cbcLichSu.Visible = true;
            btnCapNhat.Text = "Cập nhật";
            btnCapNhat.Visible = true;

            // Khóa tất cả các trường (chỉ xem)
            SetEditableFields(false);
        }

        /// <summary>
        /// Set các trường có thể chỉnh sửa
        /// </summary>
        private void SetEditableFields(bool editable)
        {
            if (_isEditMode && editable)
            {
                // Chế độ chỉnh sửa - cho phép chỉnh sửa các trường được phép
                txtChieuCao.ReadOnly = false;
                txtChieuCao.FillColor = System.Drawing.Color.White;
                txtCanNang.ReadOnly = false;
                txtCanNang.FillColor = System.Drawing.Color.White;
                txtV1.ReadOnly = false;
                txtV1.FillColor = System.Drawing.Color.White;
                txtV2.ReadOnly = false;
                txtV2.FillColor = System.Drawing.Color.White;
                txtV3.ReadOnly = false;
                txtV3.FillColor = System.Drawing.Color.White;
                txtSoDoBapTay.ReadOnly = false;
                txtSoDoBapTay.FillColor = System.Drawing.Color.White;
                txtSoDoBapChan.ReadOnly = false;
                txtSoDoBapChan.FillColor = System.Drawing.Color.White;

                // Cho phép chỉnh sửa trình độ
                cbcTrinhDo.Enabled = true;

                // Khóa các trường không được chỉnh sửa
                txtBMI.ReadOnly = true;
                txtBMI.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                // lblDanhGiaTongThe không cần set ReadOnly vì là Label
                txtGioiTinh.ReadOnly = true;
                txtGioiTinh.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtTuoi.ReadOnly = true;
                txtTuoi.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtNgayCapNhat.ReadOnly = true;
                txtNgayCapNhat.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
            }
            else
            {
                // Chế độ xem - khóa tất cả
                txtChieuCao.ReadOnly = true;
                txtChieuCao.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtCanNang.ReadOnly = true;
                txtCanNang.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtBMI.ReadOnly = true;
                txtBMI.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtV1.ReadOnly = true;
                txtV1.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtV2.ReadOnly = true;
                txtV2.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtV3.ReadOnly = true;
                txtV3.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtSoDoBapTay.ReadOnly = true;
                txtSoDoBapTay.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtSoDoBapChan.ReadOnly = true;
                txtSoDoBapChan.FillColor = System.Drawing.Color.FromArgb(240, 240, 240);
                // lblDanhGiaTongThe không cần set ReadOnly vì là Label
                cbcTrinhDo.Enabled = false;
            }
        }

        /// <summary>
        /// Load dữ liệu từ một bản ghi vào form
        /// </summary>
        private void LoadRecordData(TinhTrangTongQuan record)
        {
            if (record == null) return;

            // Load các trường có thể chỉnh sửa
            txtChieuCao.Text = record.ChieuCao?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            txtCanNang.Text = record.CanNang?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            txtBMI.Text = record.BMI?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            txtV1.Text = record.SoDoVong1?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            txtV2.Text = record.SoDoVong2?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            txtV3.Text = record.SoDoVong3?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            txtSoDoBapTay.Text = record.SoDoBapTay?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            txtSoDoBapChan.Text = record.SoDoBapChan?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";

            // Load đánh giá thể trạng
            lblDanhGiaTongThe.Text = record.GhiChu ?? "";

            // Load trình độ
            if (!string.IsNullOrWhiteSpace(record.TrinhDoCaNhan))
            {
                var index = cbcTrinhDo.Items.IndexOf(record.TrinhDoCaNhan);
                if (index >= 0)
                {
                    cbcTrinhDo.SelectedIndex = index;
                }
            }

            // Load ngày cập nhật
            if (record.NgayCapNhat.HasValue)
            {
                txtNgayCapNhat.Text = record.NgayCapNhat.Value.ToString("dd/MM/yyyy");
            }
            else if (record.NgayGhiNhan != null)
            {
                txtNgayCapNhat.Text = record.NgayGhiNhan.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Load ComboBox lịch sử
        /// </summary>
        private void LoadHistoryComboBox()
        {
            cbcLichSu.Items.Clear();
            if (_historyRecords == null || _historyRecords.Count == 0)
            {
                return;
            }

            // Hiển thị tất cả các bản ghi với format rõ ràng
            foreach (var record in _historyRecords)
            {
                string dateStr;
                if (record.NgayCapNhat.HasValue)
                {
                    // Hiển thị cả ngày và giờ nếu có NgayCapNhat
                    dateStr = record.NgayCapNhat.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    // Chỉ hiển thị ngày nếu không có NgayCapNhat
                    dateStr = record.NgayGhiNhan.ToString("dd/MM/yyyy");
                }
                cbcLichSu.Items.Add(dateStr);
            }

            // Chọn bản ghi mới nhất (đầu tiên trong danh sách đã sắp xếp)
            if (cbcLichSu.Items.Count > 0)
            {
                cbcLichSu.SelectedIndex = 0;
                _selectedHistoryDate = cbcLichSu.Items[0].ToString();
            }
        }

        /// <summary>
        /// Event handler khi chọn lịch sử
        /// </summary>
        private void CbcLichSu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbcLichSu.SelectedIndex < 0 || cbcLichSu.SelectedIndex >= _historyRecords.Count)
                return;

            var selectedRecord = _historyRecords[cbcLichSu.SelectedIndex];
            _selectedHistoryDate = cbcLichSu.Items[cbcLichSu.SelectedIndex].ToString();

            // Kiểm tra xem có phải bản ghi mới nhất không (so sánh BanGhiID)
            bool isLatest = selectedRecord.BanGhiID == _currentRecord?.BanGhiID;

            // Load dữ liệu của bản ghi được chọn
            LoadRecordData(selectedRecord);

            // Ẩn/hiện nút Cập nhật và reset chế độ chỉnh sửa
            _isEditMode = false;
            btnCapNhat.Text = "Cập nhật";
            btnCapNhat.Visible = isLatest;
            SetEditableFields(false); // Khóa tất cả khi xem lịch sử
        }

        /// <summary>
        /// Tính BMI tự động khi chiều cao hoặc cân nặng thay đổi
        /// </summary>
        private void TxtChieuCao_TextChanged(object sender, EventArgs e)
        {
            CalculateBMI();
        }

        private void TxtCanNang_TextChanged(object sender, EventArgs e)
        {
            CalculateBMI();
        }

        /// <summary>
        /// Tính BMI và đánh giá thể trạng
        /// </summary>
        private void CalculateBMI()
        {
            if (!_isEditMode) return; // Chỉ tính khi đang ở chế độ chỉnh sửa

            if (double.TryParse(txtChieuCao.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out double height) &&
                double.TryParse(txtCanNang.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out double weight) &&
                height > 0 && weight > 0)
            {
                double bmi = Math.Round(weight / Math.Pow(height / 100.0, 2), 2);
                txtBMI.Text = bmi.ToString("0.##", CultureInfo.InvariantCulture);

                // Đánh giá thể trạng dựa trên BMI
                EvaluateBodyStatus(bmi, height, weight);
            }
            else
            {
                txtBMI.Text = "";
                lblDanhGiaTongThe.Text = "";
            }
        }

        /// <summary>
        /// Đánh giá thể trạng dựa trên BMI
        /// </summary>
        private void EvaluateBodyStatus(double bmi, double height, double weight)
        {
            var user = CurrentUser.User;
            string gender = user?.GioiTinh ?? "";

            string evaluation = "";
            double idealWeightMin = 0;
            double idealWeightMax = 0;

            // Tính cân nặng lý tưởng (BMI từ 18.5 đến 24.9)
            idealWeightMin = Math.Round(18.5 * Math.Pow(height / 100.0, 2), 1);
            idealWeightMax = Math.Round(24.9 * Math.Pow(height / 100.0, 2), 1);

            if (bmi < 18.5)
            {
                evaluation = $"Bạn là người có thể trạng gầy. Chiều cao {height:F0}cm rất tốt nhưng chỉ số cân nặng là {weight:F1}kg quá thấp. Bạn nên tăng cân lên khoảng {idealWeightMin:F1}kg - {idealWeightMax:F1}kg để có vóc dáng ổn định hơn.";
            }
            else if (bmi >= 18.5 && bmi < 25)
            {
                evaluation = $"Bạn có thể trạng bình thường và khỏe mạnh. Chỉ số BMI {bmi:F1} nằm trong khoảng lý tưởng. Hãy duy trì chế độ ăn uống và luyện tập hợp lý để giữ vóc dáng này.";
            }
            else if (bmi >= 25 && bmi < 30)
            {
                evaluation = $"Bạn đang ở mức thừa cân. Chỉ số BMI {bmi:F1} cho thấy bạn cần giảm cân. Cân nặng lý tưởng cho chiều cao {height:F0}cm là khoảng {idealWeightMin:F1}kg - {idealWeightMax:F1}kg. Hãy xây dựng chế độ ăn uống và luyện tập phù hợp.";
            }
            else
            {
                evaluation = $"Bạn đang ở mức béo phì. Chỉ số BMI {bmi:F1} cho thấy bạn cần giảm cân ngay. Cân nặng lý tưởng cho chiều cao {height:F0}cm là khoảng {idealWeightMin:F1}kg - {idealWeightMax:F1}kg. Hãy tham khảo ý kiến bác sĩ hoặc chuyên gia dinh dưỡng để có kế hoạch giảm cân an toàn và hiệu quả.";
            }

            lblDanhGiaTongThe.Text = evaluation;
        }

        /// <summary>
        /// Event handler cho nút Cập nhật/Lưu
        /// </summary>
        private void BtnCapNhat_Click(object sender, EventArgs e)
        {
            // Nếu là người đã có dữ liệu và chưa ở chế độ chỉnh sửa, chuyển sang chế độ chỉnh sửa
            if (!_isNewUser && !_isEditMode)
            {
                // Chuyển sang chế độ chỉnh sửa
                _isEditMode = true;
                btnCapNhat.Text = "Lưu";
                SetEditableFields(true);
                return;
            }

            // Lưu dữ liệu (cho cả người mới và người đã có dữ liệu)
            if (!ValidateInputs(out double height, out double weight, out double v1, out double v2, out double v3, out double bapTay, out double bapChan, out string trinhDo))
            {
                return;
            }

            try
            {
                var user = CurrentUser.User;
                if (user == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var db = new WF_HealthTracker())
                {
                    // Tính BMI
                    double bmi = Math.Round(weight / Math.Pow(height / 100.0, 2), 2);

                    // Để có lịch sử đầy đủ, mỗi lần cập nhật sẽ tạo bản ghi mới
                    // Nếu đã có bản ghi của ngày hôm nay, xóa bản ghi cũ trước khi tạo mới
                    var todayRecord = db.TinhTrangTongQuan
                        .Where(t => t.UserID == user.UserID && 
                                   t.NgayGhiNhan == DateTime.Today)
                        .FirstOrDefault();

                    if (todayRecord != null)
                    {
                        // Xóa bản ghi cũ của ngày hôm nay để tạo bản ghi mới
                        db.TinhTrangTongQuan.Remove(todayRecord);
                    }

                    // Tạo bản ghi mới cho ngày hôm nay
                    var newRecord = new TinhTrangTongQuan
                    {
                        BanGhiID = GenerateRecordId(),
                        UserID = user.UserID,
                        NgayGhiNhan = DateTime.Today,
                        NgayCapNhat = DateTime.Now
                    };
                    db.TinhTrangTongQuan.Add(newRecord);

                    // Cập nhật dữ liệu
                    newRecord.ChieuCao = height;
                    newRecord.CanNang = weight;
                    newRecord.BMI = bmi;
                    newRecord.SoDoVong1 = v1 > 0 ? (double?)v1 : null;
                    newRecord.SoDoVong2 = v2 > 0 ? (double?)v2 : null;
                    newRecord.SoDoVong3 = v3 > 0 ? (double?)v3 : null;
                    newRecord.SoDoBapTay = bapTay > 0 ? (double?)bapTay : null;
                    newRecord.SoDoBapChan = bapChan > 0 ? (double?)bapChan : null;
                    newRecord.TrinhDoCaNhan = trinhDo;
                    newRecord.GhiChu = lblDanhGiaTongThe.Text;

                    db.SaveChanges();

                    _isSaved = true;

                    // Nếu là người mới, đóng form và quay về trang chủ
                    if (_isNewUser)
                    {
                        MessageBox.Show("Đã lưu thông tin thể trạng thành công!", "Thành công",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DialogResult = DialogResult.OK;
                        Close();
                        return;
                    }

                    // Nếu là người đã có dữ liệu, ở lại form và chuyển về chế độ xem
                    _currentRecord = newRecord;
                    LoadBodyStatusData();

                    _isEditMode = false;
                    btnCapNhat.Text = "Cập nhật";
                    SetEditableFields(false);

                    MessageBox.Show("Đã lưu thông tin thể trạng thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể lưu thông tin: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Validate dữ liệu đầu vào
        /// </summary>
        private bool ValidateInputs(out double height, out double weight, out double v1, out double v2, out double v3, out double bapTay, out double bapChan, out string trinhDo)
        {
            height = weight = v1 = v2 = v3 = bapTay = bapChan = 0;
            trinhDo = "";

            // Validate chiều cao
            if (!double.TryParse(txtChieuCao.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out height) || height <= 0 || height > 300)
            {
                MessageBox.Show("Chiều cao không hợp lệ (cm). Vui lòng nhập từ 1-300cm.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtChieuCao.Focus();
                return false;
            }

            // Validate cân nặng
            if (!double.TryParse(txtCanNang.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out weight) || weight <= 0 || weight > 500)
            {
                MessageBox.Show("Cân nặng không hợp lệ (kg). Vui lòng nhập từ 1-500kg.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCanNang.Focus();
                return false;
            }

            // Validate số đo vòng (không bắt buộc, nhưng nếu nhập thì phải hợp lệ)
            if (!string.IsNullOrWhiteSpace(txtV1.Text))
            {
                if (!double.TryParse(txtV1.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out v1) || v1 <= 0)
                {
                    MessageBox.Show("Số đo vòng 1 không hợp lệ.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtV1.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtV2.Text))
            {
                if (!double.TryParse(txtV2.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out v2) || v2 <= 0)
                {
                    MessageBox.Show("Số đo vòng 2 không hợp lệ.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtV2.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtV3.Text))
            {
                if (!double.TryParse(txtV3.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out v3) || v3 <= 0)
                {
                    MessageBox.Show("Số đo vòng 3 không hợp lệ.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtV3.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSoDoBapTay.Text))
            {
                if (!double.TryParse(txtSoDoBapTay.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out bapTay) || bapTay <= 0)
                {
                    MessageBox.Show("Số đo bắp tay không hợp lệ.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoDoBapTay.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSoDoBapChan.Text))
            {
                if (!double.TryParse(txtSoDoBapChan.Text.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out bapChan) || bapChan <= 0)
                {
                    MessageBox.Show("Số đo bắp chân không hợp lệ.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSoDoBapChan.Focus();
                    return false;
                }
            }

            // Validate trình độ
            if (cbcTrinhDo.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn trình độ.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbcTrinhDo.Focus();
                return false;
            }
            trinhDo = cbcTrinhDo.SelectedItem.ToString();

            return true;
        }

        /// <summary>
        /// Generate Record ID
        /// </summary>
        private static string GenerateRecordId()
        {
            return $"rec_{Guid.NewGuid():N}".Substring(0, 20);
        }

        /// <summary>
        /// Event handler khi form đóng
        /// </summary>
        private void FrmThongTinhTheTrang_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isMandatory && !_isSaved && e.CloseReason == CloseReason.UserClosing)
            {
                MessageBox.Show("Vui lòng hoàn tất thông tin thể trạng trước khi tiếp tục.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            // Event handler cũ, giữ lại để tránh lỗi
        }
    }
}
