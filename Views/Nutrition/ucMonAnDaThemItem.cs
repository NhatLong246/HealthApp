using HealthApp.Models;
using System;
using System.Windows.Forms;

namespace HealthApp.Views.Nutrition
{
    public partial class ucMonAnDaThemItem : UserControl
    {
        public BuaAnChiTiet MonAnDaThem { get; private set; }
        public event EventHandler<BuaAnChiTiet> XoaClicked;

        public ucMonAnDaThemItem(BuaAnChiTiet monAn)
        {
            InitializeComponent();
            MonAnDaThem = monAn;
            LoadData();
        }

        private void LoadData()
        {
            if (MonAnDaThem != null)
            {
                lblTenMonAn.Text = MonAnDaThem.TenMonAn;
                lblLoaiBuaAn.Text = MonAnDaThem.LoaiBuaAn;
                lblSoLuong.Text = $"{MonAnDaThem.KhoiLuongChuan ?? 0}{MonAnDaThem.Donvi ?? "g"}";
                lblCalories.Text = $"{MonAnDaThem.Calories ?? 0} kcal";
                lblProtein.Text = $"P: {MonAnDaThem.Protein ?? 0}g";
                lblCarbs.Text = $"C: {MonAnDaThem.Carbs ?? 0}g";
                lblFat.Text = $"F: {MonAnDaThem.Fat ?? 0}g";
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa món ăn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                XoaClicked?.Invoke(this, MonAnDaThem);
            }
        }
    }
}

