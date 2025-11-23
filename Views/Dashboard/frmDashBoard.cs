using HealthApp.Views.Nutrition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HealthApp.Views.Dashboard
{
    public partial class frmDashBoard : Form
    {
        public frmDashBoard()
        {
            InitializeComponent();
            InitializeEventHandlers();
            LoadUserControl();
        }

        private void InitializeEventHandlers()
        {
            // Gắn event handler cho các button trong footer
            picHome.Click += PicHome_Click;
            picAnUong.Click += PicAnUong_Click;
        }

        private void LoadUserControl()
        {
            ucDashBoard ucDashBoard = new ucDashBoard(this);
            LoadUserControl(ucDashBoard);
        }

        /// <summary>
        /// Load một UserControl vào pnlBody, giữ nguyên header và footer
        /// </summary>
        /// <param name="userControl">UserControl cần load</param>
        public void LoadUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Top;
            userControl.AutoSize = true;
            userControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            pnlBody.Controls.Clear();
            pnlBody.Controls.Add(userControl);

            // KHÓA SCROLL NGANG HOÀN TOÀN
            pnlBody.HorizontalScroll.Maximum = 0;
            pnlBody.HorizontalScroll.Visible = false;
            pnlBody.HorizontalScroll.Enabled = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pnlBody_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler cho button Home - điều hướng về trang chủ
        /// </summary>
        private void PicHome_Click(object sender, EventArgs e)
        {
            try
            {
                // Load lại ucDashBoard (trang chủ)
                ucDashBoard ucDashBoard = new ucDashBoard(this);
                LoadUserControl(ucDashBoard);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng về trang chủ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler cho button Ăn Uống - điều hướng tới trang chế độ ăn uống đề xuất
        /// </summary>
        private void PicAnUong_Click(object sender, EventArgs e)
        {
            try
            {
                // Load ucCheDoAnUongDeXuat (trang chế độ ăn uống đề xuất)
                ucCheDoAnUongDeXuat ucCheDoAnUongDeXuat = new ucCheDoAnUongDeXuat();
                LoadUserControl(ucCheDoAnUongDeXuat);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi điều hướng tới trang chế độ ăn uống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
