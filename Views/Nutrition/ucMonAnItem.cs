using HealthApp.Models;
using System;
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
            MonAn = monAn;
            LoadData();
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
            }
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

