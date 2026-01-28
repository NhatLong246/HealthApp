extern alias ef6;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HealthApp.Models;
using ef6::System.Data.Entity;

namespace HealthApp.Views.Admin
{
    public partial class ucHieuSuat : UserControl
    {
        private WF_HealthTracker _dbContext;

        public ucHieuSuat()
        {
            InitializeComponent();
            _dbContext = new WF_HealthTracker();
            Load += UcHieuSuat_Load;
        }

        private void UcHieuSuat_Load(object sender, EventArgs e)
        {
            LoadStatistics();
            LoadAgeChart();
            LoadPTUserRatioChart();
        }

        /// <summary>
        /// Load các thống kê tổng quan
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                // 1. Tổng số người dùng
                int totalUsers = _dbContext.Users.Count();
                lbGenTongNguoiDung.Text = totalUsers.ToString();

                // 2. Tổng số PT
                int totalPTs = _dbContext.Users.Count(u => u.Role == "PT");
                lbGenTongPT.Text = totalPTs.ToString();

                // 3. Tổng số tiền hoa hồng trong tháng hiện tại
                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                double totalRevenue = _dbContext.GiaoDich
                    .Where(gd => gd.TrangThaiThanhToan == "Completed" && 
                                gd.NgayGiaoDich.HasValue &&
                                gd.NgayGiaoDich.Value >= startOfMonth &&
                                gd.NgayGiaoDich.Value <= endOfMonth &&
                                gd.SoTienHoaHong.HasValue)
                    .Sum(gd => (double?)gd.SoTienHoaHong) ?? 0;
                lbGenTienHoaHong.Text = totalRevenue.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thống kê: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load biểu đồ cột hiển thị độ tuổi của người dùng
        /// </summary>
        private void LoadAgeChart()
        {
            try
            {
                // Clear chart
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();
                chart1.Legends.Clear();
                chart1.Titles.Clear();

                // Tạo ChartArea
                ChartArea chartArea = new ChartArea("ChartArea1");
                chartArea.AxisX.Title = "Độ tuổi";
                chartArea.AxisX.TitleFont = new Font("Times New Roman", 10F, FontStyle.Bold);
                chartArea.AxisY.Title = "Số lượng người dùng";
                chartArea.AxisY.TitleFont = new Font("Times New Roman", 10F, FontStyle.Bold);
                chartArea.AxisX.MajorGrid.Enabled = false;  // Bỏ đường kẻ dọc cho dễ nhìn
                chartArea.AxisY.MajorGrid.Enabled = true;
                chartArea.AxisY.Minimum = 0;
                chartArea.BackColor = Color.FromArgb(248, 250, 252);  // Nền nhạt
                chart1.ChartAreas.Add(chartArea);
                chart1.BackColor = Color.FromArgb(248, 250, 252);

                // Tạo Legend
                Legend legend = new Legend("Legend1");
                legend.Docking = Docking.Top;
                legend.Font = new Font("Times New Roman", 9F);
                chart1.Legends.Add(legend);

                // Tạo Series cho biểu đồ cột (chỉ hiển thị số lượng, không hiển thị %)
                Series series = new Series("Số người dùng");
                series.ChartType = SeriesChartType.Column;
                series.Color = Color.FromArgb(59, 130, 246); // Màu xanh
                series.IsValueShownAsLabel = true;
                series.LabelForeColor = Color.Black;
                series.Font = new Font("Times New Roman", 9F, FontStyle.Bold);
                series.LabelFormat = "#,##0";

                // Lấy danh sách người dùng có ngày sinh
                var usersWithBirthday = _dbContext.Users
                    .Where(u => u.NgaySinh.HasValue)
                    .ToList();

                // Tính tuổi và nhóm theo khoảng tuổi
                var today = DateTime.Now;
                var ageGroups = new Dictionary<string, int>
                {
                    { "18-25", 0 },
                    { "26-35", 0 },
                    { "36-45", 0 },
                    { "46-55", 0 },
                    { "56+", 0 }
                };

                foreach (var user in usersWithBirthday)
                {
                    var birthDate = user.NgaySinh.Value;
                    int age = today.Year - birthDate.Year;
                    if (birthDate.Date > today.AddYears(-age)) age--;

                    if (age >= 18 && age <= 25)
                        ageGroups["18-25"]++;
                    else if (age >= 26 && age <= 35)
                        ageGroups["26-35"]++;
                    else if (age >= 36 && age <= 45)
                        ageGroups["36-45"]++;
                    else if (age >= 46 && age <= 55)
                        ageGroups["46-55"]++;
                    else if (age >= 56)
                        ageGroups["56+"]++;
                }

                // Thêm dữ liệu vào biểu đồ cột (chỉ số lượng)
                foreach (var group in ageGroups)
                {
                    series.Points.AddXY(group.Key, group.Value);
                }

                chart1.Series.Add(series);

                // Thêm tiêu đề
                chart1.Titles.Add("Phân bố độ tuổi người dùng");
                chart1.Titles[0].Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải biểu đồ độ tuổi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load biểu đồ tròn hiển thị tỉ lệ PT so với người dùng
        /// </summary>
        private void LoadPTUserRatioChart()
        {
            try
            {
                // Clear chart
                chart2.Series.Clear();
                chart2.ChartAreas.Clear();
                chart2.Legends.Clear();
                chart2.Titles.Clear();

                // Tạo ChartArea
                ChartArea chartArea = new ChartArea("ChartArea1");
                chartArea.BackColor = Color.FromArgb(248, 250, 252);  // Nền nhạt
                chart2.ChartAreas.Add(chartArea);
                chart2.BackColor = Color.FromArgb(248, 250, 252);

                // Tạo Legend
                Legend legend = new Legend("Legend1");
                legend.Docking = Docking.Bottom;
                legend.Font = new Font("Times New Roman", 9F);
                chart2.Legends.Add(legend);

                // Tạo Series cho biểu đồ tròn — hiển thị phần trăm trong biểu đồ tròn
                Series series = new Series("Tỉ lệ");
                series.ChartType = SeriesChartType.Pie;
                // Mình set label thủ công để tránh lỗi format token (#VALY...)
                series.IsValueShownAsLabel = false;
                series.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                series.LabelForeColor = Color.Black;
                series["PieLabelStyle"] = "Inside"; // Phần trăm hiển thị bên trong miếng tròn

                // Đếm theo yêu cầu: PT / (Tổng user). "Người dùng" = Tổng user - PT (bao gồm cả Admin nếu có).
                int totalUsers = _dbContext.Users.Count();
                int totalPTs = _dbContext.Users.Count(u => u.Role == "PT");
                int totalNonPT = Math.Max(0, totalUsers - totalPTs);
                int total = totalUsers;

                // Thêm dữ liệu vào biểu đồ (chỉ PT và Người dùng)
                if (total > 0)
                {
                    double ptPct = Math.Round((double)totalPTs * 100.0 / total, 1);
                    double userPct = Math.Round(100.0 - ptPct, 1);

                    int idxPt = series.Points.AddXY("PT", ptPct);
                    series.Points[idxPt].Color = Color.FromArgb(59, 130, 246);
                    series.Points[idxPt].LegendText = "Personal Trainer";
                    series.Points[idxPt].Label = $"{ptPct:F1}%";

                    int idxUser = series.Points.AddXY("Người dùng", userPct);
                    series.Points[idxUser].Color = Color.FromArgb(34, 198, 94);
                    series.Points[idxUser].LegendText = $"Người dùng ({totalNonPT})";
                    series.Points[idxUser].Label = $"{userPct:F1}%";
                }

                // Nếu không có dữ liệu, hiển thị thông báo
                if (total == 0)
                {
                    series.Points.AddXY("Không có dữ liệu", 1);
                    series.Points[0].Color = Color.Gray;
                }

                chart2.Series.Add(series);

                // Thêm tiêu đề
                chart2.Titles.Add("Tỉ lệ PT và Người dùng");
                chart2.Titles[0].Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải biểu đồ tỉ lệ: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
