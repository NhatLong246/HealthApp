using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Services.Interfaces;

namespace HealthApp.Services
{
    public class GoalService : IGoalService
    {
        private readonly WF_HealthTracker _dbContext;

        public GoalService(WF_HealthTracker dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<ThuVienBaiTap>> GetExercisesByGoalAndLevelAsync(string loaiMucTieu, string nhomCoChinhNhat, string searchBy, string capDo = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GetExercisesByGoalAndLevelAsync ===");
                System.Diagnostics.Debug.WriteLine($"LoaiMucTieu: {loaiMucTieu ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"NhomCoChinhNhat: {nhomCoChinhNhat ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"SearchBy: {searchBy}");
                System.Diagnostics.Debug.WriteLine($"CapDo: {capDo ?? "null"}");

                IQueryable<ThuVienBaiTap> query = _dbContext.ThuVienBaiTap;

                // Xây dựng query dựa trên searchBy
                if (searchBy == "LoaiMucTieu" && !string.IsNullOrWhiteSpace(loaiMucTieu))
                {
                    // Tìm theo LoaiMucTieu (ví dụ: "Tăng cân", "Giảm cân")
                    query = query.Where(b => b.LoaiMucTieu == loaiMucTieu);
                }
                else if (searchBy == "NhomCoChinhNhat" && !string.IsNullOrWhiteSpace(nhomCoChinhNhat))
                {
                    // Tìm theo NhomCoChinhNhat (ví dụ: "Ngực", "Lưng", "Chân")
                    query = query.Where(b => b.NhomCoChinhNhat == nhomCoChinhNhat);
                }
                else if (searchBy == "Both")
                {
                    // Tìm theo cả hai
                    query = query.Where(b => 
                        (!string.IsNullOrWhiteSpace(loaiMucTieu) && b.LoaiMucTieu == loaiMucTieu) ||
                        (!string.IsNullOrWhiteSpace(nhomCoChinhNhat) && b.NhomCoChinhNhat == nhomCoChinhNhat));
                }
                else
                {
                    // Mặc định: tìm theo cả hai nếu có giá trị
                    if (!string.IsNullOrWhiteSpace(loaiMucTieu) && !string.IsNullOrWhiteSpace(nhomCoChinhNhat))
                    {
                        query = query.Where(b => b.LoaiMucTieu == loaiMucTieu || b.NhomCoChinhNhat == nhomCoChinhNhat);
                    }
                    else if (!string.IsNullOrWhiteSpace(loaiMucTieu))
                    {
                        query = query.Where(b => b.LoaiMucTieu == loaiMucTieu);
                    }
                    else if (!string.IsNullOrWhiteSpace(nhomCoChinhNhat))
                    {
                        query = query.Where(b => b.NhomCoChinhNhat == nhomCoChinhNhat);
                    }
                }

                // Filter theo trình độ
                if (!string.IsNullOrWhiteSpace(capDo) && capDo != "Tất cả")
                {
                    // Map từ tiếng Việt sang tiếng Anh
                    string capDoEn = MapCapDoToEnglish(capDo);
                    if (!string.IsNullOrEmpty(capDoEn))
                    {
                        query = query.Where(b => b.CapDo == capDoEn || b.CapDo == "All Levels");
                    }
                }

                var result = query
                    .OrderByDescending(b => b.DoPhoBien)
                    .ThenBy(b => b.TenBaiTap)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {result.Count} exercises");
                foreach (var ex in result.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"  - {ex.TenBaiTap} (LoaiMucTieu: {ex.LoaiMucTieu}, NhomCo: {ex.NhomCoChinhNhat}, CapDo: {ex.CapDo})");
                }

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetExercisesByGoalAndLevelAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Task.FromResult(new List<ThuVienBaiTap>());
            }
        }

        public Task<List<ThuVienBaiTap>> GetAllExercisesAsync()
        {
            try
            {
                var exercises = _dbContext.ThuVienBaiTap
                    .OrderByDescending(b => b.DoPhoBien)
                    .ThenBy(b => b.TenBaiTap)
                    .ToList();

                return Task.FromResult(exercises);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllExercisesAsync error: {ex.Message}");
                return Task.FromResult(new List<ThuVienBaiTap>());
            }
        }

        public Task<ThuVienBaiTap> GetExerciseDetailAsync(string baiTapId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(baiTapId))
                {
                    return Task.FromResult<ThuVienBaiTap>(null);
                }

                var exercise = _dbContext.ThuVienBaiTap
                    .FirstOrDefault(b => b.BaiTapID == baiTapId);

                return Task.FromResult(exercise);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetExerciseDetailAsync error: {ex.Message}");
                return Task.FromResult<ThuVienBaiTap>(null);
            }
        }

        public Task<KeHoachLuyenTap> CreateWorkoutPlanAsync(
            string userId,
            string mucTieuId,
            DateTime ngayBatDau,
            DateTime ngayKetThuc,
            string capDo,
            List<WeeklySchedule> weeklySchedules)
        {
            try
            {
                // Tạo KeHoachTapID
                string keHoachTapId = GenerateKeHoachTapId();

                var keHoach = new KeHoachLuyenTap
                {
                    KeHoachTapID = keHoachTapId,
                    UserID = userId,
                    MucTieuID = mucTieuId,
                    CapDo = capDo,
                    TrangThai = "Đang hoạt động",
                    MoTa = $"Kế hoạch luyện tập từ {ngayBatDau:dd/MM/yyyy} đến {ngayKetThuc:dd/MM/yyyy}",
                    NgayCapNhat = DateTime.Now
                };

                _dbContext.KeHoachLuyenTap.Add(keHoach);
                _dbContext.SaveChanges();

                // Tạo các buổi tập
                CreateWorkoutSessionsAsync(keHoachTapId, weeklySchedules, ngayBatDau, ngayKetThuc).Wait();

                return Task.FromResult(keHoach);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateWorkoutPlanAsync error: {ex.Message}");
                throw;
            }
        }

        public Task<List<BuoiTap>> CreateWorkoutSessionsAsync(
            string keHoachTapId,
            List<WeeklySchedule> weeklySchedules,
            DateTime ngayBatDauMucTieu,
            DateTime ngayKetThucMucTieu)
        {
            try
            {
                var buoiTapList = new List<BuoiTap>();

                // Lấy ID bắt đầu trước khi tạo nhiều BuoiTap để tránh trùng
                int startNumber = GetNextBuoiTapNumber();
                int counter = 0;

                // Tạo BuoiTap cho mỗi thứ trong tuần, lặp lại từ ngày bắt đầu đến ngày kết thúc
                foreach (var schedule in weeklySchedules)
                {
                    if (string.IsNullOrWhiteSpace(schedule.ThuNgay))
                        continue;

                    // Chuyển đổi thứ trong tuần sang DayOfWeek
                    DayOfWeek targetDayOfWeek = GetDayOfWeekFromThuNgay(schedule.ThuNgay);
                    
                    // Tìm ngày đầu tiên của thứ này từ ngày bắt đầu mục tiêu
                    DateTime currentDate = GetFirstOccurrenceOfDay(ngayBatDauMucTieu, targetDayOfWeek);

                    // Tạo BuoiTap cho mỗi tuần trong khoảng thời gian mục tiêu
                    while (currentDate <= ngayKetThucMucTieu)
                    {
                        // Tạo BuoiTapID unique
                        string buoiTapId = $"session_{(startNumber + counter):D4}";
                        counter++;

                        // Kiểm tra ID đã tồn tại chưa (phòng trường hợp có race condition)
                        while (_dbContext.BuoiTap.Any(b => b.BuoiTapID == buoiTapId))
                        {
                            startNumber++;
                            buoiTapId = $"session_{startNumber:D4}";
                        }

                        // Tạo DateTime cho ThoiGianBatDau và ThoiGianKetThuc với ngày thực tế
                        DateTime? thoiGianBatDau = null;
                        DateTime? thoiGianKetThuc = null;
                        
                        if (schedule.GioBatDau.HasValue && schedule.GioKetThuc.HasValue)
                        {
                            // Tạo DateTime với ngày thực tế và giờ từ schedule
                            thoiGianBatDau = currentDate.Date.Add(schedule.GioBatDau.Value);
                            thoiGianKetThuc = currentDate.Date.Add(schedule.GioKetThuc.Value);
                        }

                        var buoiTap = new BuoiTap
                        {
                            BuoiTapID = buoiTapId,
                            KeHoachTapID = keHoachTapId,
                            ThuNgay = schedule.ThuNgay,
                            TrangThai = "Chưa thực hiện",
                            GhiChu = schedule.GhiChu,
                            ThoiGianBatDau = thoiGianBatDau,
                            ThoiGianKetThuc = thoiGianKetThuc,
                            NgayCapNhat = DateTime.Now,
                            NgayThucHien = null // Chưa thực hiện nên để null
                        };

                        // Thêm thông tin giờ vào GhiChu để dễ đọc
                        if (schedule.GioBatDau.HasValue && schedule.GioKetThuc.HasValue)
                        {
                            string timeInfo = $"Giờ: {schedule.GioBatDau.Value:hh\\:mm} - {schedule.GioKetThuc.Value:hh\\:mm}";
                            if (!string.IsNullOrEmpty(buoiTap.GhiChu))
                            {
                                buoiTap.GhiChu += $" | {timeInfo}";
                            }
                            else
                            {
                                buoiTap.GhiChu = timeInfo;
                            }
                        }

                        _dbContext.BuoiTap.Add(buoiTap);
                        buoiTapList.Add(buoiTap);

                        // Chuyển sang tuần tiếp theo (thêm 7 ngày)
                        currentDate = currentDate.AddDays(7);
                    }
                }

                _dbContext.SaveChanges();
                return Task.FromResult(buoiTapList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateWorkoutSessionsAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                throw;
            }
        }

        /// <summary>
        /// Chuyển đổi "Thứ X" hoặc "Chủ nhật" sang DayOfWeek
        /// </summary>
        private DayOfWeek GetDayOfWeekFromThuNgay(string thuNgay)
        {
            if (string.IsNullOrWhiteSpace(thuNgay))
                return DayOfWeek.Monday;

            thuNgay = thuNgay.Trim().ToLower();
            
            if (thuNgay.Contains("thứ 2") || thuNgay == "thứ hai" || thuNgay == "monday")
                return DayOfWeek.Monday;
            if (thuNgay.Contains("thứ 3") || thuNgay == "thứ ba" || thuNgay == "tuesday")
                return DayOfWeek.Tuesday;
            if (thuNgay.Contains("thứ 4") || thuNgay == "thứ tư" || thuNgay == "wednesday")
                return DayOfWeek.Wednesday;
            if (thuNgay.Contains("thứ 5") || thuNgay == "thứ năm" || thuNgay == "thursday")
                return DayOfWeek.Thursday;
            if (thuNgay.Contains("thứ 6") || thuNgay == "thứ sáu" || thuNgay == "friday")
                return DayOfWeek.Friday;
            if (thuNgay.Contains("thứ 7") || thuNgay == "thứ bảy" || thuNgay == "saturday")
                return DayOfWeek.Saturday;
            if (thuNgay.Contains("chủ nhật") || thuNgay == "chủ nhật" || thuNgay == "sunday")
                return DayOfWeek.Sunday;

            return DayOfWeek.Monday; // Default
        }

        /// <summary>
        /// Tìm ngày đầu tiên của thứ trong tuần từ ngày bắt đầu mục tiêu
        /// </summary>
        private DateTime GetFirstOccurrenceOfDay(DateTime startDate, DayOfWeek targetDay)
        {
            // Nếu ngày bắt đầu mục tiêu đã là thứ cần tìm, dùng luôn
            if (startDate.DayOfWeek == targetDay)
            {
                return startDate;
            }

            // Tính số ngày cần thêm để đến thứ cần tìm
            int daysUntilTarget = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
            
            return startDate.AddDays(daysUntilTarget);
        }

        private string MapCapDoToEnglish(string capDoVi)
        {
            switch (capDoVi?.ToLower())
            {
                case "người mới":
                case "beginner":
                    return "Beginner";
                case "trung cấp":
                case "intermediate":
                    return "Intermediate";
                case "nâng cao":
                case "advanced":
                    return "Advanced";
                case "tất cả":
                case "all levels":
                    return "All Levels";
                default:
                    return null;
            }
        }

        private string GenerateKeHoachTapId()
        {
            var lastPlan = _dbContext.KeHoachLuyenTap
                .OrderByDescending(k => k.KeHoachTapID)
                .FirstOrDefault();

            if (lastPlan == null || !lastPlan.KeHoachTapID.StartsWith("workout_"))
            {
                return "workout_0001";
            }

            string numberPart = lastPlan.KeHoachTapID.Substring(8);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                int newNumber = lastNumber + 1;
                return $"workout_{newNumber:D4}";
            }

            int planCount = _dbContext.KeHoachLuyenTap.Count();
            return $"workout_{(planCount + 1):D4}";
        }

        /// <summary>
        /// Lấy số tiếp theo cho BuoiTapID (không tạo full ID, chỉ lấy số)
        /// </summary>
        private int GetNextBuoiTapNumber()
        {
            var lastSession = _dbContext.BuoiTap
                .OrderByDescending(b => b.BuoiTapID)
                .FirstOrDefault();

            if (lastSession == null || !lastSession.BuoiTapID.StartsWith("session_"))
            {
                return 1;
            }

            string numberPart = lastSession.BuoiTapID.Substring(9);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                return lastNumber + 1;
            }

            int sessionCount = _dbContext.BuoiTap.Count();
            return sessionCount + 1;
        }

        /// <summary>
        /// Tạo BuoiTapID tự động (dùng cho single session)
        /// </summary>
        private string GenerateBuoiTapId()
        {
            int nextNumber = GetNextBuoiTapNumber();
            string buoiTapId = $"session_{nextNumber:D4}";

            // Kiểm tra ID đã tồn tại chưa (phòng trường hợp có race condition)
            while (_dbContext.BuoiTap.Any(b => b.BuoiTapID == buoiTapId))
            {
                nextNumber++;
                buoiTapId = $"session_{nextNumber:D4}";
            }

            return buoiTapId;
        }
    }
}

