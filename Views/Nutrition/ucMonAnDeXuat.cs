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

namespace HealthApp.Views.Nutrition
{
    public partial class ucMonAnDeXuat : UserControl
    {
        public ucMonAnDeXuat()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set dữ liệu món ăn để hiển thị
        /// </summary>
        public void SetData(ThuVienMonAn monAn, string loaiBuaAn = "Sáng", double? khoiLuong = null)
        {
            if (monAn == null) return;

            // Set tên món ăn
            lblTenMonAn.Text = monAn.TenMonAn ?? "Không có tên";

            // Set loại bữa ăn
            lblLoaiBuaAn.Text = loaiBuaAn;

            // Tính toán khối lượng thực tế
            double heSo = 1.0;
            if (khoiLuong.HasValue && monAn.KhoiLuongChuan.HasValue && monAn.KhoiLuongChuan.Value > 0)
            {
                heSo = khoiLuong.Value / monAn.KhoiLuongChuan.Value;
            }
            else if (monAn.KhoiLuongChuan.HasValue && monAn.KhoiLuongChuan.Value > 0)
            {
                khoiLuong = monAn.KhoiLuongChuan.Value;
            }

            // Set số lượng
            if (khoiLuong.HasValue)
            {
                lblSoLuong.Text = $"Số lượng: {khoiLuong.Value:F1} {monAn.Donvi ?? "g"}";
            }
            else
            {
                lblSoLuong.Text = "Số lượng: --";
            }

            // Tính toán và hiển thị dinh dưỡng
            double calories = (monAn.Calories ?? 0) * heSo;
            double protein = (monAn.Protein ?? 0) * heSo;
            double carbs = (monAn.Carbs ?? 0) * heSo;
            double fat = (monAn.Fat ?? 0) * heSo;

            lblCalories.Text = $"{calories:F0} kcal";
            lblProtein.Text = $"P: {protein:F1}g";
            lblCarbs.Text = $"C: {carbs:F1}g";
            lblFat.Text = $"F: {fat:F1}g";
        }

        /// <summary>
        /// Set dữ liệu từ BuaAnChiTiet
        /// </summary>
        public void SetDataFromBuaAn(BuaAnChiTiet buaAn)
        {
            if (buaAn == null) return;

            lblTenMonAn.Text = buaAn.TenMonAn ?? "Không có tên";
            lblLoaiBuaAn.Text = buaAn.LoaiBuaAn ?? "Sáng";

            if (buaAn.KhoiLuongChuan.HasValue)
            {
                lblSoLuong.Text = $"Số lượng: {buaAn.KhoiLuongChuan.Value:F1} {buaAn.Donvi ?? "g"}";
            }
            else
            {
                lblSoLuong.Text = "Số lượng: --";
            }

            lblCalories.Text = $"{(buaAn.Calories ?? 0):F0} kcal";
            lblProtein.Text = $"P: {(buaAn.Protein ?? 0):F1}g";
            lblCarbs.Text = $"C: {(buaAn.Carbs ?? 0):F1}g";
            lblFat.Text = $"F: {(buaAn.Fat ?? 0):F1}g";
        }
    }
}
