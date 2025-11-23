using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HealthApp.Views.MucTieu
{
    public partial class frmMucTieu : Form
    {
        public frmMucTieu()
        {
            InitializeComponent();
            LoadUserControl();
        }

        private void LoadUserControl()
        {
            ucMucTieu ucMucTieu = new ucMucTieu();
            //ucMucTieu.Dock = DockStyle.None;
            //ucMucTieu.Location = new Point(0, 0);
            //// Đảm bảo chiều rộng bằng panel cha để không có scroll ngang
            //ucMucTieu.Width = pnlMucTieu.Width;
            //pnlMucTieu.Controls.Clear();
            //pnlMucTieu.Controls.Add(ucMucTieu);

            //// Chỉ cho phép scroll dọc bằng cách set AutoScrollMinSize
            //// Chiều rộng = 0 để không có scroll ngang, chiều cao = chiều cao của control con
            //pnlMucTieu.AutoScrollMinSize = new Size(0, ucMucTieu.Height);

            //// Ẩn thanh scroll ngang nếu có
            //pnlMucTieu.HorizontalScroll.Visible = false;
            //pnlMucTieu.HorizontalScroll.Enabled = false;
            ucMucTieu.Dock = DockStyle.Top;            
            ucMucTieu.AutoSize = true;               
            ucMucTieu.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            pnlMucTieu.Controls.Clear();
            pnlMucTieu.Controls.Add(ucMucTieu);

            // KHÓA SCROLL NGANG HOÀN TOÀN
            pnlMucTieu.HorizontalScroll.Maximum = 0;
            pnlMucTieu.HorizontalScroll.Visible = false;
            pnlMucTieu.HorizontalScroll.Enabled = false;
        }

        private void pnlMucTieu_Paint(object sender, PaintEventArgs e)
        {
            // Paint event - không cần xử lý gì ở đây
        }
    }
}
