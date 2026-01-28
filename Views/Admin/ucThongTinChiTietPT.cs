extern alias ef6;

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
using HealthApp.Models;
using ef6::System.Data.Entity;

namespace HealthApp.Views.Admin
{
    public partial class ucThongTinChiTietPT : UserControl
    {
        private WF_HealthTracker _dbContext;
        private HuanLuyenVien _currentPT;

        public ucThongTinChiTietPT()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            // Event handlers cho các nút xem ảnh/file
            btnAnhDaiDien.Click += BtnAnhDaiDien_Click;
            btnAnhCCCD.Click += BtnAnhCCCD_Click;
            btnAnhChanDung.Click += BtnAnhChanDung_Click;
            btnXemFile.Click += BtnXemFile_Click;
        }

        /// <summary>
        /// Load dữ liệu PT theo PTID
        /// </summary>
        public void LoadPTData(string ptId)
        {
            try
            {
                var pt = _dbContext.HuanLuyenVien
                    .Include("Users")
                    .FirstOrDefault(p => p.PTID == ptId);

                if (pt == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin PT!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadPTData(pt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load dữ liệu PT từ object HuanLuyenVien
        /// </summary>
        public void LoadPTData(HuanLuyenVien pt)
        {
            try
            {
                _currentPT = pt;

                // Load thông tin User nếu chưa có
                if (pt.Users == null)
                {
                    pt.Users = _dbContext.Users.FirstOrDefault(u => u.UserID == pt.UserID);
                }

                var user = pt.Users;

                // ========== THÔNG TIN CƠ BẢN ==========
                lblSoPTID.Text = pt.PTID ?? "N/A";
                lblSoHoVaTen.Text = user?.HoTen ?? user?.Username ?? "N/A";
                lblSoEmail.Text = user?.Email ?? "N/A";
                lblSoNamKinhNghiem.Text = pt.SoNamKinhNghiem.HasValue 
                    ? $"{pt.SoNamKinhNghiem.Value} năm" 
                    : "Chưa có";
                lblSoDiaDiem.Text = pt.ThanhPho ?? "Chưa có";
                lblSoGiaGio.Text = pt.GiaTheoGio.HasValue 
                    ? $"{pt.GiaTheoGio.Value:N0}" 
                    : "Chưa có";

                // Load ảnh đại diện
                LoadImage(ptrAnhDaiDien, pt.AnhDaiDien ?? user?.AnhDaiDien);

                // ========== CHUYÊN MÔN & CHỨNG CHỈ ==========
                lblSoChuyenMon.Text = pt.ChuyenMon ?? "Chưa có";
                lblSoChungChi.Text = pt.ChungChi ?? "Chưa có";

                // ========== TRẠNG THÁI & HIỆU SUẤT ==========
                // Xác minh
                if (pt.DaXacMinh == true)
                {
                    lblSoXacMinh.Text = "Đã xác minh";
                    lblSoXacMinh.ForeColor = Color.Lime;
                }
                else
                {
                    lblSoXacMinh.Text = "Chưa xác minh";
                    lblSoXacMinh.ForeColor = Color.Red;
                }

                // Số khách hiện tại (tính từ DatLichPT với trạng thái Confirmed hoặc Completed)
                int soKhachHienTai = _dbContext.DatLichPT
                    .Where(dl => dl.PTID == pt.PTID && 
                                (dl.TrangThai == "Confirmed" || dl.TrangThai == "Completed"))
                    .Select(dl => dl.KhachHangID)
                    .Distinct()
                    .Count();
                lblSoKhachHienTai.Text = soKhachHienTai.ToString();

                // Đánh giá trung bình (tính từ DanhGiaPT)
                var danhGiaList = _dbContext.DanhGiaPT
                    .Where(dg => dg.PTID == pt.PTID)
                    .Select(dg => (double?)dg.Diem)
                    .ToList();

                double diemTrungBinh = danhGiaList.Any() 
                    ? Math.Round(danhGiaList.Average() ?? 0.0, 1) 
                    : 0.0;
                lblSoDanhGiaTB.Text = diemTrungBinh.ToString("F1");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu PT: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load ảnh vào PictureBox
        /// </summary>
        private void LoadImage(Guna.UI2.WinForms.Guna2CirclePictureBox pictureBox, string imagePath)
        {
            try
            {
                if (pictureBox == null) return;

                if (string.IsNullOrEmpty(imagePath))
                {
                    pictureBox.Image = null;
                    return;
                }

                string fullPath = imagePath;
                if (!Path.IsPathRooted(imagePath))
                {
                    // Nếu là đường dẫn tương đối, thử tìm trong Resources
                    fullPath = Path.Combine(Application.StartupPath, "Resources", imagePath);
                }

                if (File.Exists(fullPath))
                {
                    pictureBox.Image = Image.FromFile(fullPath);
                }
                else
                {
                    pictureBox.Image = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi load ảnh: {ex.Message}");
                pictureBox.Image = null;
            }
        }

        /// <summary>
        /// Hiển thị ảnh trong form mới
        /// </summary>
        private void ShowImage(string imagePath, string title)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    MessageBox.Show("Không có ảnh để hiển thị!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string fullPath = imagePath;
                if (!Path.IsPathRooted(imagePath))
                {
                    fullPath = Path.Combine(Application.StartupPath, "Resources", imagePath);
                }

                if (!File.Exists(fullPath))
                {
                    MessageBox.Show("Không tìm thấy file ảnh!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo form để hiển thị ảnh
                var imageForm = new Form
                {
                    Text = title,
                    Size = new Size(800, 600),
                    StartPosition = FormStartPosition.CenterParent
                };

                var pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Image.FromFile(fullPath)
                };

                imageForm.Controls.Add(pictureBox);
                imageForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị ảnh: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Mở file bằng ứng dụng mặc định
        /// </summary>
        private void OpenFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Không có file để mở!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string fullPath = filePath;
                if (!Path.IsPathRooted(filePath))
                {
                    fullPath = Path.Combine(Application.StartupPath, "Resources", filePath);
                }

                if (!File.Exists(fullPath))
                {
                    MessageBox.Show("Không tìm thấy file!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                System.Diagnostics.Process.Start(fullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở file: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== EVENT HANDLERS CHO CÁC NÚT XEM ẢNH/FILE ==========

        private void BtnAnhDaiDien_Click(object sender, EventArgs e)
        {
            if (_currentPT == null) return;

            string imagePath = _currentPT.AnhDaiDien ?? _currentPT.Users?.AnhDaiDien;
            ShowImage(imagePath, "Ảnh đại diện");
        }

        private void BtnAnhCCCD_Click(object sender, EventArgs e)
        {
            if (_currentPT == null) return;
            ShowImage(_currentPT.AnhCCCD, "Ảnh CCCD");
        }

        private void BtnAnhChanDung_Click(object sender, EventArgs e)
        {
            if (_currentPT == null) return;
            ShowImage(_currentPT.AnhChanDung, "Ảnh chân dung");
        }

        private void BtnXemFile_Click(object sender, EventArgs e)
        {
            if (_currentPT == null) return;
            OpenFile(_currentPT.FileTaiLieu);
        }

        private void pnlNen_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
