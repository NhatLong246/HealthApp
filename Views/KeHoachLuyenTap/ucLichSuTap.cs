using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HealthApp.Views.KeHoachLuyenTap
{
    public partial class ucLichSuTap : UserControl
    {
        private TimeSpan _thoiGianTap;

        public ucLichSuTap()
        {
            InitializeComponent();
            // Disable các textbox để chỉ hiển thị
            txtGio.ReadOnly = true;
            txtGio.Enabled = false;
            txtPhut.ReadOnly = true;
            txtPhut.Enabled = false;
            txtGiay.ReadOnly = true;
            txtGiay.Enabled = false;
        }

        /// <summary>
        /// Set thông tin cho item lịch sử
        /// </summary>
        public void SetHistoryInfo(int lanTap, TimeSpan thoiGianTap)
        {
            _thoiGianTap = thoiGianTap;
            lbGenLan.Text = lanTap.ToString();
            txtGio.Text = thoiGianTap.Hours.ToString("D2");
            txtPhut.Text = thoiGianTap.Minutes.ToString("D2");
            txtGiay.Text = thoiGianTap.Seconds.ToString("D2");
        }

        /// <summary>
        /// Lấy thời gian tập
        /// </summary>
        public TimeSpan GetThoiGianTap()
        {
            return _thoiGianTap;
        }

        /// <summary>
        /// Cập nhật số lần tập
        /// </summary>
        public void SetLanTap(int lanTap)
        {
            lbGenLan.Text = lanTap.ToString();
        }
    }
}
