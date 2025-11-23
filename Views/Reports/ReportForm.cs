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
using HealthApp.Controllers;
using HealthApp.Common.Helpers;

namespace HealthApp.Views.Reports
{
    public partial class ReportForm : Form
    {
        private readonly ReportController _reportController;
        private readonly Services.Interfaces.IExportService _exportService;
        private ReportStatistics _currentStats;

        public ReportForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            _reportController = new ReportController();
            _exportService = new Services.ExportService();
            
            // Kết nối event handlers
            this.Load += ReportForm_Load;
            btnExportEX.Click += BtnExportEX_Click;
           
        }

        private async void ReportForm_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== ReportForm_Load START ===");
            System.Diagnostics.Debug.WriteLine($"CurrentUser.IsLoggedIn: {CurrentUser.IsLoggedIn}");
            System.Diagnostics.Debug.WriteLine($"CurrentUser.User: {(CurrentUser.User != null ? "NOT NULL" : "NULL")}");
            System.Diagnostics.Debug.WriteLine($"CurrentUser.UserID: {CurrentUser.UserID ?? "NULL"}");
            System.Diagnostics.Debug.WriteLine($"CurrentUser.Username: {CurrentUser.Username ?? "NULL"}");

            if (!CurrentUser.IsLoggedIn)
            {
                System.Diagnostics.Debug.WriteLine("User not logged in, closing form");
                MessageBox.Show("Vui lòng đăng nhập để xem báo cáo!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            await LoadReportData();
            System.Diagnostics.Debug.WriteLine("=== ReportForm_Load END ===");
        }

        private async Task LoadReportData()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LoadReportData START ===");
                
                var userId = CurrentUser.UserID;
                System.Diagnostics.Debug.WriteLine($"UserID from CurrentUser: '{userId}'");
                
                if (string.IsNullOrEmpty(userId))
                {
                    System.Diagnostics.Debug.WriteLine("UserID is null or empty!");
                    MessageBox.Show("Không tìm thấy thông tin người dùng! Vui lòng đăng nhập lại.", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                // Test: Kiểm tra database connection trực tiếp
                try
                {
                    using (var testContext = new Models.WF_HealthTracker())
                    {
                        var testUser = testContext.Users.FirstOrDefault(u => u.UserID == userId);
                        System.Diagnostics.Debug.WriteLine($"Test DB Query - User found: {testUser != null}");
                        if (testUser != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - UserID: '{testUser.UserID}'");
                            System.Diagnostics.Debug.WriteLine($"  - Username: '{testUser.Username}'");
                        }
                        
                        var testKeHoachTap = testContext.KeHoachLuyenTap.Where(k => k.UserID == userId).ToList();
                        System.Diagnostics.Debug.WriteLine($"Test DB Query - KeHoachTap count: {testKeHoachTap.Count}");
                        
                        var testBuoiTap = testContext.BuoiTap
                            .Where(b => testKeHoachTap.Select(k => k.KeHoachTapID).Contains(b.KeHoachTapID))
                            .ToList();
                        System.Diagnostics.Debug.WriteLine($"Test DB Query - BuoiTap count: {testBuoiTap.Count}");
                        foreach (var b in testBuoiTap.Take(3))
                        {
                            System.Diagnostics.Debug.WriteLine($"  - BuoiTapID: {b.BuoiTapID}, TrangThai: '{b.TrangThai}'");
                        }
                    }
                }
                catch (Exception dbEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Test DB Query Error: {dbEx.Message}");
                }

                System.Diagnostics.Debug.WriteLine($"Loading report data for UserID: '{userId}'");
                
                var stats = await _reportController.GetStatisticsAsync(userId);
                _currentStats = stats; // Lưu lại để xuất Excel
                
                System.Diagnostics.Debug.WriteLine($"Stats loaded:");
                System.Diagnostics.Debug.WriteLine($"  - TotalSessions: {stats.TotalSessions}");
                System.Diagnostics.Debug.WriteLine($"  - TotalTime: {stats.TotalTime}");
                System.Diagnostics.Debug.WriteLine($"  - TotalAchievements: {stats.TotalAchievements}");
                System.Diagnostics.Debug.WriteLine($"  - CompletedGoals: {stats.CompletedGoals}");
                System.Diagnostics.Debug.WriteLine($"  - AverageTimePerSession: {stats.AverageTimePerSession}");
                System.Diagnostics.Debug.WriteLine($"  - AverageSessionsPerWeek: {stats.AverageSessionsPerWeek}");
                System.Diagnostics.Debug.WriteLine($"  - AverageCaloriesBurned: {stats.AverageCaloriesBurned}");
                System.Diagnostics.Debug.WriteLine($"  - WeeklyProgress count: {stats.WeeklyProgress?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"  - MuscleGroupDistribution count: {stats.MuscleGroupDistribution?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"  - TwoWeeksComparison count: {stats.TwoWeeksComparison?.Count ?? 0}");

                // Hiển thị thông tin debug trong MessageBox (tạm thời để kiểm tra)
                var debugInfo = $"DEBUG INFO:\n\n" +
                               $"UserID: {userId}\n" +
                               $"TotalSessions: {stats.TotalSessions}\n" +
                               $"TotalTime: {stats.TotalTime} phút\n" +
                               $"TotalAchievements: {stats.TotalAchievements}\n" +
                               $"CompletedGoals: {stats.CompletedGoals}\n" +
                               $"WeeklyProgress: {stats.WeeklyProgress?.Count ?? 0} ngày\n" +
                               $"MuscleGroupDistribution: {stats.MuscleGroupDistribution?.Count ?? 0} nhóm cơ\n" +
                               $"TwoWeeksComparison: {stats.TwoWeeksComparison?.Count ?? 0} tuần\n\n" +
                               $"Xem chi tiết trong Output Window (View → Output → Debug)";
                
                // Uncomment dòng này để hiển thị MessageBox debug (nhớ comment lại sau khi kiểm tra xong)
                // MessageBox.Show(debugInfo, "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 1. Hiển thị 4 thống kê đầu trang
                lbGenSession.Text = stats.TotalSessions.ToString();
                label2.Text = FormatTime(stats.TotalTime);
                //lbGenAchivements.Text = stats.TotalAchievements.ToString();
                lbgenTarget.Text = stats.CompletedGoals.ToString();
                System.Diagnostics.Debug.WriteLine("Updated 4 summary statistics");

                // 2. Thống kê chi tiết bên phải
                lbGenTBTime.Text = FormatTime(stats.AverageTimePerSession);
                label1.Text = stats.AverageSessionsPerWeek.ToString("F1");
                label3.Text = stats.AverageCaloriesBurned.ToString("F0");
                System.Diagnostics.Debug.WriteLine("Updated detailed statistics");

                // 3. Biểu đồ miền - Tiến độ luyện tập trong tuần (chart2)
                LoadWeeklyProgressChart(stats.WeeklyProgress);
                System.Diagnostics.Debug.WriteLine("Loaded weekly progress chart");

                // 4. Biểu đồ quạt - Phân bố nhóm cơ (chart3)
                LoadMuscleGroupPieChart(stats.MuscleGroupDistribution);
                System.Diagnostics.Debug.WriteLine("Loaded muscle group pie chart");

                // 5. Biểu đồ 2 cột - So sánh 2 tuần gần nhất (chart1)
                LoadTwoWeeksComparisonChart(stats.TwoWeeksComparison);
                System.Diagnostics.Debug.WriteLine("Loaded two weeks comparison chart");

                System.Diagnostics.Debug.WriteLine("=== LoadReportData END (SUCCESS) ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== LoadReportData ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"Inner Message: {ex.InnerException.Message}");
                }
                
                string errorMessage = $"Lỗi khi tải dữ liệu báo cáo:\n{ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nChi tiết: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMessage, "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWeeklyProgressChart(Dictionary<DateTime, double> weeklyProgress)
        {
            chart2.Series.Clear();
            chart2.ChartAreas.Clear();
            chart2.Legends.Clear();

            // Tạo ChartArea
            ChartArea chartArea = new ChartArea("ChartArea1");
            chartArea.AxisX.Title = "Ngày";
            chartArea.AxisY.Title = "Phút";
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.Minimum = 0; // Đảm bảo trục Y bắt đầu từ 0
            chart2.ChartAreas.Add(chartArea);

            // Tạo Series cho biểu đồ miền
            Series series = new Series("Tiến độ luyện tập");
            series.ChartType = SeriesChartType.Area;
            series.Color = Color.FromArgb(34, 198, 94);
            series.BorderWidth = 2;
            series.BorderColor = Color.FromArgb(22, 163, 74);

            // Sắp xếp theo ngày
            var sortedData = weeklyProgress != null && weeklyProgress.Count > 0
                ? weeklyProgress.OrderBy(kvp => kvp.Key).ToList()
                : new List<KeyValuePair<DateTime, double>>();

            // Nếu không có dữ liệu, tạo dữ liệu mặc định (7 ngày với giá trị 0)
            if (sortedData.Count == 0)
            {
                var today = DateTime.Now.Date;
                for (int i = 6; i >= 0; i--)
                {
                    var date = today.AddDays(-i);
                    string dayLabel = date.ToString("dd/MM");
                    series.Points.AddXY(dayLabel, 0);
                }
            }
            else
            {
                foreach (var item in sortedData)
                {
                    string dayLabel = item.Key.ToString("dd/MM");
                    series.Points.AddXY(dayLabel, item.Value);
                }
            }

            chart2.Series.Add(series);
            chart2.Titles.Clear();
            chart2.Titles.Add("Tiến độ luyện tập trong tuần");
            chart2.Titles[0].Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
        }

        private void LoadMuscleGroupPieChart(Dictionary<string, int> muscleGroupDistribution)
        {
            chart3.Series.Clear();
            chart3.ChartAreas.Clear();
            chart3.Legends.Clear();

            // Tạo ChartArea
            ChartArea chartArea = new ChartArea("ChartArea1");
            chart3.ChartAreas.Add(chartArea);

            // Tạo Legend
            Legend legend = new Legend("Legend1");
            legend.Docking = Docking.Bottom;
            chart3.Legends.Add(legend);

            // Tạo Series cho biểu đồ quạt
            Series series = new Series("Phân bố nhóm cơ");
            series.ChartType = SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";

            // Màu sắc cho các phần
            Color[] colors = new Color[]
            {
                Color.FromArgb(34, 198, 94),
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(168, 85, 247),
                Color.FromArgb(239, 68, 68),
                Color.FromArgb(251, 146, 60),
                Color.FromArgb(236, 72, 153),
                Color.FromArgb(14, 165, 233)
            };

            // Kiểm tra có dữ liệu không
            if (muscleGroupDistribution == null || muscleGroupDistribution.Count == 0)
            {
                // Hiển thị message "Chưa có dữ liệu"
                DataPoint point = new DataPoint();
                point.SetValueXY("Chưa có dữ liệu", 1);
                point.Color = Color.LightGray;
                point.Label = "Chưa có dữ liệu";
                point.LegendText = "Chưa có dữ liệu";
                series.Points.Add(point);
            }
            else
            {
                int colorIndex = 0;
                foreach (var item in muscleGroupDistribution.OrderByDescending(kvp => kvp.Value))
                {
                    DataPoint point = new DataPoint();
                    point.SetValueXY(item.Key, item.Value);
                    point.Color = colors[colorIndex % colors.Length];
                    point.Label = $"{item.Key}\n{item.Value} lần";
                    point.LegendText = $"{item.Key}: {item.Value} lần";
                    series.Points.Add(point);
                    colorIndex++;
                }
            }

            chart3.Series.Add(series);
            chart3.Titles.Clear();
            chart3.Titles.Add("Phân bố nhóm cơ tập luyện");
            chart3.Titles[0].Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
        }

        private void LoadTwoWeeksComparisonChart(Dictionary<string, Dictionary<string, double>> twoWeeksComparison)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            // Tạo ChartArea
            ChartArea chartArea = new ChartArea("ChartArea1");
            chartArea.AxisX.Title = "Tuần";
            chartArea.AxisY.Title = "Giá trị";
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.Minimum = 0; // Đảm bảo trục Y bắt đầu từ 0
            chart1.ChartAreas.Add(chartArea);

            // Tạo Legend
            Legend legend = new Legend("Legend1");
            legend.Docking = Docking.Top;
            chart1.Legends.Add(legend);

            // Series 1: Số buổi tập
            Series seriesSessions = new Series("Số buổi tập");
            seriesSessions.ChartType = SeriesChartType.Column;
            seriesSessions.Color = Color.FromArgb(59, 130, 246);

            // Series 2: Thời gian (phút)
            Series seriesTime = new Series("Thời gian (phút)");
            seriesTime.ChartType = SeriesChartType.Column;
            seriesTime.Color = Color.FromArgb(34, 198, 94);

            // Series 3: Calories
            Series seriesCalories = new Series("Calories");
            seriesCalories.ChartType = SeriesChartType.Column;
            seriesCalories.Color = Color.FromArgb(239, 68, 68);

            // Thêm dữ liệu
            if (twoWeeksComparison != null && twoWeeksComparison.Count > 0)
            {
                foreach (var week in twoWeeksComparison.Keys)
                {
                    var data = twoWeeksComparison[week];
                    seriesSessions.Points.AddXY(week, data.ContainsKey("Sessions") ? data["Sessions"] : 0);
                    seriesTime.Points.AddXY(week, data.ContainsKey("Time") ? data["Time"] : 0);
                    seriesCalories.Points.AddXY(week, data.ContainsKey("Calories") ? data["Calories"] : 0);
                }
            }
            else
            {
                // Nếu không có dữ liệu, hiển thị 0 cho cả 2 tuần
                seriesSessions.Points.AddXY("Tuần trước", 0);
                seriesSessions.Points.AddXY("Tuần này", 0);
                seriesTime.Points.AddXY("Tuần trước", 0);
                seriesTime.Points.AddXY("Tuần này", 0);
                seriesCalories.Points.AddXY("Tuần trước", 0);
                seriesCalories.Points.AddXY("Tuần này", 0);
            }

            chart1.Series.Add(seriesSessions);
            chart1.Series.Add(seriesTime);
            chart1.Series.Add(seriesCalories);
            chart1.Titles.Clear();
            chart1.Titles.Add("So sánh hiệu suất 2 tuần gần nhất");
            chart1.Titles[0].Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
        }

        private string FormatTime(double minutes)
        {
            if (minutes < 60)
                return $"{minutes:F0} phút";
            
            int hours = (int)(minutes / 60);
            int remainingMinutes = (int)(minutes % 60);
            
            if (remainingMinutes == 0)
                return $"{hours} giờ";
            
            return $"{hours} giờ {remainingMinutes} phút";
        }

        private async void BtnExportEX_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentStats == null)
                {
                    MessageBox.Show("Chưa có dữ liệu để xuất! Vui lòng đợi dữ liệu tải xong.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị SaveFileDialog
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
                    saveDialog.FilterIndex = 1;
                    saveDialog.FileName = $"BaoCaoThongKe_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    saveDialog.Title = "Lưu file Excel";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Disable button để tránh click nhiều lần
                        btnExportEX.Enabled = false;
                        btnExportEX.Text = "Đang xuất...";

                        // Xuất file
                        bool success = await _exportService.ExportToExcelAsync(saveDialog.FileName, _currentStats);

                        btnExportEX.Enabled = true;
                        btnExportEX.Text = "Xuất Excel";

                        if (success)
                        {
                            MessageBox.Show($"Xuất file Excel thành công!\n\nFile: {saveDialog.FileName}", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi xảy ra khi xuất file Excel!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                btnExportEX.Enabled = true;
                btnExportEX.Text = "Xuất Excel";
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _reportController?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
