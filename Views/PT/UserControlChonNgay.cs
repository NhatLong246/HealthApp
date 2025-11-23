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

namespace HealthApp.Views.PT
{
    public partial class UserControlChonNgay : UserControl
    {
        private Guna2CustomGradientPanel _parentPanel;
        public event Action<UserControlChonNgay> OnDeleteRequested;

        public UserControlChonNgay()
        {
            InitializeComponent();
            InitializeTimeComboBoxes();
            InitializeEventHandlers();
            
            // Set ngày mặc định là ngày mai (chỉ cho phép chọn tương lai)
            dtpChonNgay.MinDate = DateTime.Today.AddDays(1);
            dtpChonNgay.Value = DateTime.Today.AddDays(1);
            dtpChonNgay.ValueChanged += DtpChonNgay_ValueChanged;
        }

        /// <summary>
        /// Set parent panel để có thể xóa chính nó
        /// </summary>
        public void SetParentPanel(Guna2CustomGradientPanel parentPanel)
        {
            _parentPanel = parentPanel;
        }

        private bool _isUpdating = false; // Flag để tránh vòng lặp khi update

        private void InitializeEventHandlers()
        {
            btnXoa.Click += BtnXoa_Click;
            cboGioBatDau.SelectedIndexChanged += CboGioBatDau_SelectedIndexChanged;
            cboGioKetThuc.SelectedIndexChanged += CboGioKetThuc_SelectedIndexChanged;
            
            // Thêm event để kiểm tra trùng lịch khi thay đổi ngày/giờ
            dtpChonNgay.ValueChanged += DtpChonNgay_ValueChanged;
            cboGioBatDau.SelectedIndexChanged += CboGioBatDau_CheckOverlap;
            cboGioKetThuc.SelectedIndexChanged += CboGioKetThuc_CheckOverlap;
        }

        /// <summary>
        /// Kiểm tra trùng lịch khi thay đổi giờ bắt đầu
        /// </summary>
        private void CboGioBatDau_CheckOverlap(object sender, EventArgs e)
        {
            if (!_isUpdating)
            {
                CheckOverlapWithOthers();
            }
        }

        /// <summary>
        /// Kiểm tra trùng lịch khi thay đổi giờ kết thúc
        /// </summary>
        private void CboGioKetThuc_CheckOverlap(object sender, EventArgs e)
        {
            if (!_isUpdating)
            {
                CheckOverlapWithOthers();
            }
        }

        /// <summary>
        /// Kiểm tra trùng lịch với các UserControl khác trong parent
        /// </summary>
        private void CheckOverlapWithOthers()
        {
            try
            {
                if (_parentPanel == null || _isUpdating)
                    return;

                // Chỉ kiểm tra nếu đã có đầy đủ thông tin
                if (cboGioBatDau.SelectedItem == null || cboGioKetThuc.SelectedItem == null)
                    return;

                foreach (Control control in _parentPanel.Controls)
                {
                    if (control is UserControlChonNgay otherUC && otherUC != this)
                    {
                        if (this.IsOverlapping(otherUC))
                        {
                            MessageBox.Show("Lịch này trùng với lịch đã chọn trước đó! Vui lòng chọn ngày/giờ khác.", 
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            
                            // Reset về giá trị an toàn (giờ đầu tiên có sẵn)
                            _isUpdating = true;
                            if (cboGioBatDau.Items.Count > 0)
                            {
                                cboGioBatDau.SelectedIndex = 0;
                            }
                            if (cboGioKetThuc.Items.Count > 0)
                            {
                                cboGioKetThuc.SelectedIndex = 0;
                            }
                            _isUpdating = false;
                            return;
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        /// <summary>
        /// Khởi tạo combobox giờ (từ 6:00 đến 22:00, mỗi 30 phút)
        /// </summary>
        private void InitializeTimeComboBoxes()
        {
            cboGioBatDau.Items.Clear();
            cboGioKetThuc.Items.Clear();

            // Tạo danh sách giờ từ 6:00 đến 22:00, mỗi 30 phút
            for (int hour = 6; hour <= 22; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    string timeString = $"{hour:D2}:{minute:D2}";
                    cboGioBatDau.Items.Add(timeString);
                    cboGioKetThuc.Items.Add(timeString);
                }
            }

            // Set DropDownHeight để chỉ hiển thị 4 items (mỗi item ~30px, cộng thêm border)
            cboGioBatDau.DropDownHeight = 4 * 30 + 2;
            cboGioKetThuc.DropDownHeight = 4 * 30 + 2;

            // Set giá trị mặc định
            if (cboGioBatDau.Items.Count > 0)
            {
                cboGioBatDau.SelectedIndex = 0; // 6:00
            }
            if (cboGioKetThuc.Items.Count > 0)
            {
                cboGioKetThuc.SelectedIndex = 2; // 7:00 (sau 1 giờ)
            }
        }

        /// <summary>
        /// Xử lý khi thay đổi giờ bắt đầu - cập nhật danh sách giờ kết thúc
        /// </summary>
        private void CboGioBatDau_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboGioBatDau.SelectedItem == null || _isUpdating)
                    return;

                _isUpdating = true;

                string gioBatDau = cboGioBatDau.SelectedItem.ToString();
                TimeSpan timeBatDau = TimeSpan.Parse(gioBatDau);

                // Lưu lại selected index hiện tại của giờ kết thúc (trong danh sách gốc)
                int currentSelectedIndexInOriginal = -1;
                if (cboGioKetThuc.SelectedItem != null)
                {
                    string currentTime = cboGioKetThuc.SelectedItem.ToString();
                    // Tìm index trong danh sách gốc
                    for (int i = 0; i < cboGioBatDau.Items.Count; i++)
                    {
                        if (cboGioBatDau.Items[i].ToString() == currentTime)
                        {
                            currentSelectedIndexInOriginal = i;
                            break;
                        }
                    }
                }

                // Xóa và thêm lại các giờ kết thúc (chỉ từ giờ bắt đầu + 30 phút trở đi)
                cboGioKetThuc.Items.Clear();
                
                int startIndex = cboGioBatDau.SelectedIndex + 1; // Bắt đầu từ giờ sau giờ bắt đầu
                
                for (int i = startIndex; i < cboGioBatDau.Items.Count; i++)
                {
                    cboGioKetThuc.Items.Add(cboGioBatDau.Items[i]);
                }

                // Set DropDownHeight
                cboGioKetThuc.DropDownHeight = 4 * 30 + 2;

                // Chọn lại giờ kết thúc (nếu còn hợp lệ)
                if (cboGioKetThuc.Items.Count > 0)
                {
                    // Nếu giờ kết thúc cũ không còn hợp lệ hoặc <= giờ bắt đầu, chọn giờ đầu tiên
                    if (currentSelectedIndexInOriginal < startIndex || currentSelectedIndexInOriginal <= cboGioBatDau.SelectedIndex)
                    {
                        cboGioKetThuc.SelectedIndex = 0;
                    }
                    else
                    {
                        // Chọn lại giờ tương ứng (đã trừ đi startIndex)
                        int newIndex = currentSelectedIndexInOriginal - startIndex;
                        if (newIndex >= 0 && newIndex < cboGioKetThuc.Items.Count)
                        {
                            cboGioKetThuc.SelectedIndex = newIndex;
                        }
                        else
                        {
                            cboGioKetThuc.SelectedIndex = 0;
                        }
                    }
                }

                _isUpdating = false;
            }
            catch
            {
                _isUpdating = false;
                // Ignore errors in time validation
            }
        }

        /// <summary>
        /// Xử lý khi thay đổi giờ kết thúc
        /// </summary>
        private void CboGioKetThuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Không cần xử lý gì vì danh sách đã được lọc trong CboGioBatDau_SelectedIndexChanged
        }

        /// <summary>
        /// Xử lý khi thay đổi ngày - kiểm tra không được chọn ngày quá khứ hoặc hôm nay
        /// </summary>
        private void DtpChonNgay_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtpChonNgay.Value.Date <= DateTime.Today)
                {
                    MessageBox.Show("Không thể chọn ngày hôm nay hoặc quá khứ! Vui lòng chọn ngày tương lai.", 
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _isUpdating = true;
                    dtpChonNgay.Value = DateTime.Today.AddDays(1);
                    _isUpdating = false;
                }
                else if (!_isUpdating)
                {
                    // Kiểm tra trùng lịch khi thay đổi ngày (chỉ kiểm tra trùng giờ, không kiểm tra trùng ngày)
                    CheckOverlapWithOthers();
                }
            }
            catch
            {
                _isUpdating = false;
                // Ignore errors
            }
        }

        /// <summary>
        /// Xử lý khi click nút Xóa
        /// </summary>
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Gọi event để parent form xử lý xóa
                OnDeleteRequested?.Invoke(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Validate dữ liệu của UserControl này
        /// </summary>
        public bool ValidateData()
        {
            try
            {
                // Kiểm tra ngày phải > ngày hôm nay (chỉ cho phép tương lai)
                if (dtpChonNgay.Value.Date <= DateTime.Today)
                {
                    MessageBox.Show("Ngày tập phải từ ngày mai trở đi!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Kiểm tra đã chọn giờ
                if (cboGioBatDau.SelectedItem == null || cboGioKetThuc.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ giờ bắt đầu và giờ kết thúc!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Kiểm tra logic giờ
                string gioBatDau = cboGioBatDau.SelectedItem.ToString();
                string gioKetThuc = cboGioKetThuc.SelectedItem.ToString();

                TimeSpan timeBatDau = TimeSpan.Parse(gioBatDau);
                TimeSpan timeKetThuc = TimeSpan.Parse(gioKetThuc);

                if (timeKetThuc <= timeBatDau)
                {
                    MessageBox.Show("Giờ kết thúc phải sau giờ bắt đầu!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra xem UserControl này có trùng lịch với UserControl khác không (cùng ngày và trùng giờ)
        /// </summary>
        public bool IsOverlapping(UserControlChonNgay other)
        {
            try
            {
                if (other == null || other == this)
                    return false;

                // Kiểm tra cùng ngày
                if (dtpChonNgay.Value.Date != other.dtpChonNgay.Value.Date)
                    return false;

                // Kiểm tra trùng giờ
                if (cboGioBatDau.SelectedItem == null || cboGioKetThuc.SelectedItem == null ||
                    other.cboGioBatDau.SelectedItem == null || other.cboGioKetThuc.SelectedItem == null)
                    return false;

                TimeSpan thisStart = TimeSpan.Parse(cboGioBatDau.SelectedItem.ToString());
                TimeSpan thisEnd = TimeSpan.Parse(cboGioKetThuc.SelectedItem.ToString());
                TimeSpan otherStart = TimeSpan.Parse(other.cboGioBatDau.SelectedItem.ToString());
                TimeSpan otherEnd = TimeSpan.Parse(other.cboGioKetThuc.SelectedItem.ToString());

                // Kiểm tra overlap: (start1 < end2) && (start2 < end1)
                return (thisStart < otherEnd) && (otherStart < thisEnd);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra xem có UserControl khác cùng ngày không (không kiểm tra giờ)
        /// </summary>
        public bool HasSameDate(UserControlChonNgay other)
        {
            try
            {
                if (other == null || other == this)
                    return false;

                return dtpChonNgay.Value.Date == other.dtpChonNgay.Value.Date;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tự động tăng ngày lên 1 ngày
        /// </summary>
        public void IncrementDate()
        {
            try
            {
                _isUpdating = true;
                dtpChonNgay.Value = dtpChonNgay.Value.AddDays(1);
                _isUpdating = false;
            }
            catch
            {
                _isUpdating = false;
            }
        }

        /// <summary>
        /// Lấy dữ liệu ngày và giờ đã chọn
        /// </summary>
        public (DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc) GetData()
        {
            DateTime ngay = dtpChonNgay.Value.Date;
            TimeSpan gioBatDau = TimeSpan.Parse(cboGioBatDau.SelectedItem.ToString());
            TimeSpan gioKetThuc = TimeSpan.Parse(cboGioKetThuc.SelectedItem.ToString());

            return (ngay, gioBatDau, gioKetThuc);
        }
    }
}
