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
    public partial class ChiTietGiaoDich : Form
    {
        private WF_HealthTracker _dbContext;
        private string _giaoDichID;

        public ChiTietGiaoDich()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            this.Load += ChiTietGiaoDich_Load;
            this.FormClosing += ChiTietGiaoDich_FormClosing;
        }

        public ChiTietGiaoDich(string giaoDichID) : this()
        {
            _giaoDichID = giaoDichID ?? throw new ArgumentNullException(nameof(giaoDichID));
        }

        private void ChiTietGiaoDich_Load(object sender, EventArgs e)
        {
            try
            {
                LoadGiaoDichDetail();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin giao dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[ChiTietGiaoDich] Load error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Load và hiển thị thông tin chi tiết giao dịch
        /// </summary>
        private void LoadGiaoDichDetail()
        {
            if (string.IsNullOrWhiteSpace(_giaoDichID))
            {
                MessageBox.Show("Không tìm thấy mã giao dịch!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Query với join để lấy thông tin đầy đủ
            var giaoDich = (from gd in _dbContext.GiaoDich
                           join kh in _dbContext.Users on gd.KhachHangID equals kh.UserID
                           join pt in _dbContext.HuanLuyenVien on gd.PTID equals pt.PTID
                           join ptUser in _dbContext.Users on pt.UserID equals ptUser.UserID
                           where gd.GiaoDichID == _giaoDichID
                           select new
                           {
                               gd.GiaoDichID,
                               KhachHangTen = kh.HoTen ?? kh.UserID,
                               KhachHangSDT = kh.SDT ?? "N/A",
                               KhachHangEmail = kh.Email ?? "N/A",
                               PTTen = ptUser.HoTen ?? pt.PTID,
                               PTSDT = ptUser.SDT ?? "N/A",
                               PTEmail = ptUser.Email ?? "N/A",
                               gd.SoTien,
                               gd.HoaHongApp,
                               gd.SoTienHoaHong,
                               gd.SoTienPTNhan,
                               gd.TrangThaiThanhToan,
                               gd.PhuongThucThanhToan,
                               gd.NgayGiaoDich,
                               gd.MaGiaoDich
                           }).FirstOrDefault();

            if (giaoDich == null)
            {
                MessageBox.Show("Không tìm thấy thông tin giao dịch!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Hiển thị thông tin người dùng
            lbHoTenNguoiMua.Text = $"Người Mua: {giaoDich.KhachHangTen}";
            guna2HtmlLabel2.Text = $"Người Nhận (PT): {giaoDich.PTTen}";

            // Hiển thị thông tin thanh toán
            guna2HtmlLabel4.Text = $"Tổng tiền: {FormatCurrency(giaoDich.SoTien)}";
            guna2HtmlLabel3.Text = $"PT Nhận: {FormatCurrency(giaoDich.SoTienPTNhan ?? 0)}";
            guna2HtmlLabel6.Text = $"Hoa Hồng App: {FormatCurrency(giaoDich.SoTienHoaHong ?? 0)}";
            guna2HtmlLabel7.Text = $"Phương Thức: {giaoDich.PhuongThucThanhToan ?? "N/A"}";
            
            if (giaoDich.NgayGiaoDich.HasValue)
            {
                guna2HtmlLabel8.Text = $"Ngày giao dịch: {giaoDich.NgayGiaoDich.Value.ToString("dd/MM/yyyy HH:mm")}";
            }
            else
            {
                guna2HtmlLabel8.Text = "Ngày giao dịch: N/A";
            }
        }

        /// <summary>
        /// Format tiền tệ
        /// </summary>
        private string FormatCurrency(double amount)
        {
            return $"{amount:N0} VNĐ";
        }

        private void ChiTietGiaoDich_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dbContext?.Dispose();
        }
    }
}
