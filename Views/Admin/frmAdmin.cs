using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HealthApp.Views.Admin
{
    public partial class frmAdmin : Form
    {
        public frmAdmin()
        {
            InitializeComponent();
            // Đăng ký event handlers cho tất cả các button
            this.btnQuanLiNguoiDung.Click += BtnQuanLiNguoiDung_Click;
            this.btnQuanLiPT.Click += BtnQuanLiPT_Click;
            this.btnQuanLiBaiTap.Click += BtnQuanLiBaiTap_Click;
            this.QuanLiDinhDuong.Click += QuanLiDinhDuong_Click;
            this.btnQuanLiGiaoDich.Click += BtnQuanLiGiaoDich_Click;
            this.btnThongKeTongQuan.Click += BtnThongKeTongQuan_Click;
            
            // Tự động hiển thị ucHieuSuat khi form được load
            this.Load += FrmAdmin_Load;
        }

        /// <summary>
        /// Event handler khi form được load - hiển thị ucHieuSuat mặc định
        /// </summary>
        private void FrmAdmin_Load(object sender, EventArgs e)
        {
            // Tự động load ucHieuSuat khi mở form admin
            LoadUserControlToPanel(new ucHieuSuat());
        }

        /// <summary>
        /// Xử lý sự kiện click vào nút Quản lý người dùng
        /// </summary>
        private void BtnQuanLiNguoiDung_Click(object sender, EventArgs e)
        {
            LoadUserControlToPanel(new ucKhachHang());
        }

        /// <summary>
        /// Xử lý sự kiện click vào nút Quản lý PT
        /// </summary>
        private void BtnQuanLiPT_Click(object sender, EventArgs e)
        {
            LoadUserControlToPanel(new ucQuanLiPT());
        }

        /// <summary>
        /// Xử lý sự kiện click vào nút Quản lý bài tập
        /// </summary>
        private void BtnQuanLiBaiTap_Click(object sender, EventArgs e)
        {
            LoadUserControlToPanel(new ucQuanLyBT());
        }

        /// <summary>
        /// Xử lý sự kiện click vào nút Quản lý dinh dưỡng
        /// </summary>
        private void QuanLiDinhDuong_Click(object sender, EventArgs e)
        {
            LoadUserControlToPanel(new ucQuanLyDinhDuong());
        }

        /// <summary>
        /// Xử lý sự kiện click vào nút Quản lý giao dịch
        /// </summary>
        private void BtnQuanLiGiaoDich_Click(object sender, EventArgs e)
        {
            LoadUserControlToPanel(new ucGiaoDich());
        }

        /// <summary>
        /// Xử lý sự kiện click vào nút Thống kê tổng quan
        /// </summary>
        private void BtnThongKeTongQuan_Click(object sender, EventArgs e)
        {
            LoadUserControlToPanel(new ucHieuSuat());
        }

        /// <summary>
        /// Load UserControl vào panel nội dung
        /// </summary>
        /// <param name="userControl">UserControl cần hiển thị</param>
        private void LoadUserControlToPanel(UserControl userControl)
        {
            try
            {
                if (userControl == null)
                {
                    MessageBox.Show("Không thể tải giao diện: UserControl không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Dispose các UserControl cũ để giải phóng tài nguyên
                foreach (Control control in pnlNoiDung.Controls.OfType<UserControl>().ToList())
                {
                    control.Dispose();
                }
                
                // Xóa tất cả controls hiện có trong panel
                pnlNoiDung.Controls.Clear();
                
                // Thiết lập UserControl để fill toàn bộ panel
                userControl.Dock = DockStyle.Fill;
                
                // Thêm UserControl vào panel
                pnlNoiDung.Controls.Add(userControl);
                
                // Đưa UserControl lên trên cùng
                userControl.BringToFront();
                
                // Focus vào UserControl
                userControl.Focus();
                
                System.Diagnostics.Debug.WriteLine($"[frmAdmin] Loaded {userControl.GetType().Name} into pnlNoiDung");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải giao diện: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[frmAdmin] Error loading UserControl: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
