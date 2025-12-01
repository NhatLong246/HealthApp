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
            // Đăng ký event handler cho btnQuanLiPT
            this.btnQuanLiPT.Click += BtnQuanLiPT_Click;
        }

        /// <summary>
        /// Xử lý sự kiện click vào nút Quản lý PT
        /// </summary>
        private void BtnQuanLiPT_Click(object sender, EventArgs e)
        {
            LoadUserControlToPanel(new ucQuanLiPT());
        }

        /// <summary>
        /// Load UserControl vào panel nội dung
        /// </summary>
        /// <param name="userControl">UserControl cần hiển thị</param>
        private void LoadUserControlToPanel(UserControl userControl)
        {
            try
            {
                // Xóa tất cả controls hiện có trong panel
                pnlNoiDung.Controls.Clear();
                
                // Thiết lập UserControl để fill toàn bộ panel
                userControl.Dock = DockStyle.Fill;
                
                // Thêm UserControl vào panel
                pnlNoiDung.Controls.Add(userControl);
                
                // Đưa UserControl lên trên cùng
                userControl.BringToFront();
                
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
