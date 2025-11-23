using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HealthApp.Services.Interfaces;
using HealthApp.Controllers;

namespace HealthApp.Services
{
    public class ExportService : IExportService
    {
        public async Task<bool> ExportToExcelAsync(string filePath, object data)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Tạo file Excel đơn giản bằng XML format (Office Open XML)
                    if (data is ReportStatistics stats)
                    {
                        ExportToExcelXML(stats, filePath);
                        return true;
                    }
                    
                    return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ExportToExcelAsync error: {ex.Message}");
                    if (ex.InnerException != null)
                        System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                    return false;
                }
            });
        }

        private void ExportToExcelXML(ReportStatistics stats, string filePath)
        {
            // Log để xác minh tất cả dữ liệu đều từ database
            System.Diagnostics.Debug.WriteLine("=== ExportToExcelXML - Data from Database ===");
            System.Diagnostics.Debug.WriteLine($"TotalSessions: {stats.TotalSessions} (from DB)");
            System.Diagnostics.Debug.WriteLine($"TotalTime: {stats.TotalTime} (from DB)");
            System.Diagnostics.Debug.WriteLine($"TotalAchievements: {stats.TotalAchievements} (from DB)");
            System.Diagnostics.Debug.WriteLine($"CompletedGoals: {stats.CompletedGoals} (from DB)");
            System.Diagnostics.Debug.WriteLine($"AverageTimePerSession: {stats.AverageTimePerSession} (from DB)");
            System.Diagnostics.Debug.WriteLine($"AverageSessionsPerWeek: {stats.AverageSessionsPerWeek} (from DB)");
            System.Diagnostics.Debug.WriteLine($"AverageCaloriesBurned: {stats.AverageCaloriesBurned} (from DB)");
            System.Diagnostics.Debug.WriteLine($"WeeklyProgress count: {stats.WeeklyProgress?.Count ?? 0} (from DB)");
            System.Diagnostics.Debug.WriteLine($"MuscleGroupDistribution count: {stats.MuscleGroupDistribution?.Count ?? 0} (from DB)");
            System.Diagnostics.Debug.WriteLine($"TwoWeeksComparison count: {stats.TwoWeeksComparison?.Count ?? 0} (from DB)");
            
            // Tạo file Excel đơn giản bằng XML format
            // TẤT CẢ DỮ LIỆU ĐỀU LẤY TỪ stats (đã được query từ database)
            using (var sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Excel XML Header
                sw.WriteLine("<?xml version=\"1.0\"?>");
                sw.WriteLine("<?mso-application progid=\"Excel.Sheet\"?>");
                sw.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                sw.WriteLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
                sw.WriteLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
                sw.WriteLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                sw.WriteLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
                
                // Styles
                sw.WriteLine("<Styles>");
                sw.WriteLine("<Style ss:ID=\"Header\">");
                sw.WriteLine("<Font ss:Bold=\"1\"/>");
                sw.WriteLine("<Interior ss:Color=\"#CCCCCC\" ss:Pattern=\"Solid\"/>");
                sw.WriteLine("</Style>");
                sw.WriteLine("</Styles>");
                
                // Worksheet 1: Tổng quan
                sw.WriteLine("<Worksheet ss:Name=\"Tổng quan\">");
                sw.WriteLine("<Table>");
                
                // Header
                sw.WriteLine("<Row>");
                sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Chỉ số</Data></Cell>");
                sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Giá trị</Data></Cell>");
                sw.WriteLine("</Row>");
                
                // Data
                WriteExcelRow(sw, "Tổng buổi tập", stats.TotalSessions.ToString());
                WriteExcelRow(sw, "Tổng thời gian", FormatTime(stats.TotalTime));
                WriteExcelRow(sw, "Thành tựu", stats.TotalAchievements.ToString());
                WriteExcelRow(sw, "Mục tiêu đạt được", stats.CompletedGoals.ToString());
                WriteExcelRow(sw, "TB thời gian/buổi", FormatTime(stats.AverageTimePerSession));
                WriteExcelRow(sw, "TB buổi/tuần", stats.AverageSessionsPerWeek.ToString("F1"));
                WriteExcelRow(sw, "TB calo đốt", stats.AverageCaloriesBurned.ToString("F0"));
                
                sw.WriteLine("</Table>");
                sw.WriteLine("</Worksheet>");
                
                // Worksheet 2: Tiến độ tuần
                if (stats.WeeklyProgress != null && stats.WeeklyProgress.Count > 0)
                {
                    sw.WriteLine("<Worksheet ss:Name=\"Tiến độ tuần\">");
                    sw.WriteLine("<Table>");
                    
                    sw.WriteLine("<Row>");
                    sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Ngày</Data></Cell>");
                    sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Số phút</Data></Cell>");
                    sw.WriteLine("</Row>");
                    
                    foreach (var item in stats.WeeklyProgress.OrderBy(x => x.Key))
                    {
                        WriteExcelRow(sw, item.Key.ToString("dd/MM/yyyy"), item.Value.ToString("F0"));
                    }
                    
                    sw.WriteLine("</Table>");
                    sw.WriteLine("</Worksheet>");
                }
                
                // Worksheet 3: Phân bố nhóm cơ
                if (stats.MuscleGroupDistribution != null && stats.MuscleGroupDistribution.Count > 0)
                {
                    sw.WriteLine("<Worksheet ss:Name=\"Phân bố nhóm cơ\">");
                    sw.WriteLine("<Table>");
                    
                    sw.WriteLine("<Row>");
                    sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Nhóm cơ</Data></Cell>");
                    sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Số lần tập</Data></Cell>");
                    sw.WriteLine("</Row>");
                    
                    foreach (var item in stats.MuscleGroupDistribution.OrderByDescending(x => x.Value))
                    {
                        WriteExcelRow(sw, item.Key, item.Value.ToString());
                    }
                    
                    sw.WriteLine("</Table>");
                    sw.WriteLine("</Worksheet>");
                }
                
                // Worksheet 4: So sánh 2 tuần
                if (stats.TwoWeeksComparison != null && stats.TwoWeeksComparison.Count > 0)
                {
                    sw.WriteLine("<Worksheet ss:Name=\"So sánh 2 tuần\">");
                    sw.WriteLine("<Table>");
                    
                    sw.WriteLine("<Row>");
                    sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Tuần</Data></Cell>");
                    sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Số buổi tập</Data></Cell>");
                    sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Thời gian (phút)</Data></Cell>");
                    sw.WriteLine("<Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Calories</Data></Cell>");
                    sw.WriteLine("</Row>");
                    
                    foreach (var week in stats.TwoWeeksComparison.Keys)
                    {
                        var weekData = stats.TwoWeeksComparison[week];
                        sw.WriteLine("<Row>");
                        sw.WriteLine($"<Cell><Data ss:Type=\"String\">{week}</Data></Cell>");
                        sw.WriteLine($"<Cell><Data ss:Type=\"Number\">{(weekData.ContainsKey("Sessions") ? weekData["Sessions"] : 0)}</Data></Cell>");
                        sw.WriteLine($"<Cell><Data ss:Type=\"Number\">{(weekData.ContainsKey("Time") ? weekData["Time"] : 0)}</Data></Cell>");
                        sw.WriteLine($"<Cell><Data ss:Type=\"Number\">{(weekData.ContainsKey("Calories") ? weekData["Calories"] : 0)}</Data></Cell>");
                        sw.WriteLine("</Row>");
                    }
                    
                    sw.WriteLine("</Table>");
                    sw.WriteLine("</Worksheet>");
                }
                
                sw.WriteLine("</Workbook>");
            }
        }

        private void WriteExcelRow(StreamWriter sw, string col1, string col2)
        {
            sw.WriteLine("<Row>");
            sw.WriteLine($"<Cell><Data ss:Type=\"String\">{EscapeXML(col1)}</Data></Cell>");
            sw.WriteLine($"<Cell><Data ss:Type=\"String\">{EscapeXML(col2)}</Data></Cell>");
            sw.WriteLine("</Row>");
        }

        private string EscapeXML(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
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
    }
}

