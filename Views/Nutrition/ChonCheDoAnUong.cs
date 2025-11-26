using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Common.Helpers;

namespace HealthApp.Views.Nutrition
{
    public partial class ChonCheDoAnUong : Form
    {
        public ChonCheDoAnUong()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            // Button "Ăn Uống Theo Thực Đơn"
            btnTheoThucDon.Click += BtnTheoThucDon_Click;
            
            // Button "Ăn Uống Tự Do"
            guna2GradientButton1.Click += BtnAnUongTuDo_Click;
        }

        private void BtnTheoThucDon_Click(object sender, EventArgs e)
        {
            // TODO: Hiển thị form chọn thực đơn hệ thống
            MessageBox.Show("Tính năng 'Ăn Uống Theo Thực Đơn' đang được phát triển!", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAnUongTuDo_Click(object sender, EventArgs e)
        {
            // Kiểm tra đăng nhập
            if (!CurrentUser.IsLoggedIn)
            {
                MessageBox.Show("Vui lòng đăng nhập để sử dụng tính năng này!", 
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị ucNutrition trong form mới hoặc panel
            var nutritionForm = new Form
            {
                Text = "Kế Hoạch Ăn Uống Tự Do",
                Size = new Size(1200, 800),
                StartPosition = FormStartPosition.CenterScreen,
                WindowState = FormWindowState.Maximized
            };

            var ucNutrition = new ucNutrition();
            ucNutrition.Dock = DockStyle.Fill;
            nutritionForm.Controls.Add(ucNutrition);

            // Đóng form hiện tại và mở form mới
            this.Hide();
            nutritionForm.FormClosed += (s, args) => this.Close();
            nutritionForm.Show();
        }
    }
}
