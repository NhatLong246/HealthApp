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

            // Tạm thời chỉ thông báo vì các UserControl dinh dưỡng chi tiết đã được gỡ bỏ
            MessageBox.Show("Tính năng 'Ăn Uống Tự Do' đang được cập nhật giao diện mới.\nVui lòng sử dụng phần 'Chế độ ăn uống đề xuất' trong Dashboard.", 
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
