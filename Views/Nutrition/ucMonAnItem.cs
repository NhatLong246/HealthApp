using HealthApp.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HealthApp.Views.Nutrition
{
    public partial class ucMonAnItem : UserControl
    {
        public ThuVienMonAn MonAn { get; private set; }
        public event EventHandler<ThuVienMonAn> MonAnClicked;

        public ucMonAnItem(ThuVienMonAn monAn)
        {
            InitializeComponent();
            pnlMonAn.Dock = DockStyle.Fill;
            MonAn = monAn;

            // Điều chỉnh layout khi control thay đổi kích thước để vừa với mọi panel chứa
            this.Resize += UcMonAnItem_Resize;

            LoadData();
            UcMonAnItem_Resize(this, EventArgs.Empty);
        }

        private void LoadData()
        {
            if (MonAn != null)
            {
                lblTenMonAn.Text = MonAn.TenMonAn;
                lblCalories.Text = $"{MonAn.Calories ?? 0} kcal";
                lblProtein.Text = $"P: {MonAn.Protein ?? 0}g";
                lblCarbs.Text = $"C: {MonAn.Carbs ?? 0}g";
                lblFat.Text = $"F: {MonAn.Fat ?? 0}g";
                UcMonAnItem_Resize(this, EventArgs.Empty);
            }
        }

        private void UcMonAnItem_Resize(object sender, EventArgs e)
        {
            // Tăng padding bên trái để cân bằng, giảm khoảng trống bên trái
            int horizontalPadding = 25;
            int metricsSpacing = 15;

            // Vị trí hàng macro (dưới cùng)
            int metricsTop = Math.Max(lblCalories.Top, pnlMonAn.Height - 35);
            lblCalories.Location = new Point(horizontalPadding, metricsTop);

            lblProtein.Location = new Point(lblCalories.Right + metricsSpacing, metricsTop);
            lblCarbs.Location = new Point(lblProtein.Right + metricsSpacing, metricsTop);
            lblFat.Location = new Point(lblCarbs.Right + metricsSpacing, metricsTop);

            // Canh phải nếu tổng width lớn hơn panel
            int rightEdge = pnlMonAn.Width - horizontalPadding;
            if (lblFat.Right > rightEdge)
            {
                lblFat.Left = rightEdge - lblFat.Width;
                lblCarbs.Left = lblFat.Left - metricsSpacing - lblCarbs.Width;
                lblProtein.Left = lblCarbs.Left - metricsSpacing - lblProtein.Width;
            }

            // Đảm bảo tên món căng ngang panel và căn trái với padding
            lblTenMonAn.Location = new Point(horizontalPadding, 15);
            lblTenMonAn.MaximumSize = new Size(pnlMonAn.Width - horizontalPadding * 2, 0);
        }

        private void pnlMonAn_Click(object sender, EventArgs e)
        {
            MonAnClicked?.Invoke(this, MonAn);
        }

        private void lblTenMonAn_Click(object sender, EventArgs e)
        {
            pnlMonAn_Click(sender, e);
        }
    }
}

