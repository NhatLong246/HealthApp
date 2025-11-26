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

namespace HealthApp.Views.PT
{
    public partial class GiaoBaiTap : Form
    {
        private readonly WF_HealthTracker _dbContext;
        private readonly DatLichPT _booking;

        public GiaoBaiTap()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
        }

        public GiaoBaiTap(DatLichPT booking) : this()
        {
            _booking = booking ?? throw new ArgumentNullException(nameof(booking));
            this.Load += GiaoBaiTap_Load;
        }

        private void GiaoBaiTap_Load(object sender, EventArgs e)
        {
            try
            {
                // Sau này có thể bind thêm thông tin user / buổi tập lên form
                this.Text = $"Giao bài tập cho {_booking.KhachHangID} - {_booking.NgayGioDat:dd/MM/yyyy}";
            }
            catch
            {
                // Ignore binding errors tạm thời
            }
        }
    }
}
