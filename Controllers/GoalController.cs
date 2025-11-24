using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Models;
using HealthApp.Services;
using HealthApp.Services.Interfaces;
using WeeklySchedule = HealthApp.Services.Interfaces.WeeklySchedule;

namespace HealthApp.Controllers
{
    /// <summary>
    /// Controller xử lý logic Mục Tiêu (Goals)
    /// </summary>
    public class GoalController : IDisposable
    {
        private readonly WF_HealthTracker _dbContext;
        private readonly IGoalService _goalService;

        public GoalController()
        {
            _dbContext = new WF_HealthTracker();
            _goalService = new GoalService(_dbContext);
        }

        /// <summary>
        /// Lấy danh sách mục tiêu của user
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <param name="trangThai">Trạng thái mục tiêu (null = tất cả)</param>
        /// <returns>Danh sách mục tiêu</returns>
        public List<MucTieu> GetGoalsByUser(string userId, string trangThai = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new List<MucTieu>();
                }

                System.Diagnostics.Debug.WriteLine($"=== GetGoalsByUser START ===");
                System.Diagnostics.Debug.WriteLine($"UserId: {userId}, TrangThai: {trangThai}");

                // Thử dùng raw SQL query để tránh lỗi mapping
                string sqlQuery;
                object[] parameters;

                if (!string.IsNullOrWhiteSpace(trangThai))
                {
                    sqlQuery = "SELECT MucTieuID, UserID, LoaiMucTieu, TenMucTieu, GiaTriMucTieu, NgayBatDau, NgayKetThucDuKien, NgayKetThucThucTe, TrangThai, PTID, GhiChu, NgayTao FROM MucTieu WHERE UserID = {0} AND TrangThai = {1} ORDER BY NgayTao DESC";
                    parameters = new object[] { userId, trangThai };
                }
                else
                {
                    sqlQuery = "SELECT MucTieuID, UserID, LoaiMucTieu, TenMucTieu, GiaTriMucTieu, NgayBatDau, NgayKetThucDuKien, NgayKetThucThucTe, TrangThai, PTID, GhiChu, NgayTao FROM MucTieu WHERE UserID = {0} ORDER BY NgayTao DESC";
                    parameters = new object[] { userId };
                }

                var goals = _dbContext.Database.SqlQuery<MucTieu>(sqlQuery, parameters).ToList();

                System.Diagnostics.Debug.WriteLine($"GetGoalsByUser trả về {goals?.Count ?? 0} mục tiêu");
                System.Diagnostics.Debug.WriteLine($"=== GetGoalsByUser END ===");

                return goals ?? new List<MucTieu>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR in GetGoalsByUser ===");
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw new Exception($"Lỗi khi lấy danh sách mục tiêu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy mục tiêu theo ID
        /// </summary>
        public MucTieu GetGoalById(string mucTieuId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mucTieuId))
                {
                    return null;
                }

                return _dbContext.MucTieu.FirstOrDefault(m => m.MucTieuID == mucTieuId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy mục tiêu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo mục tiêu mới
        /// </summary>
        public async Task<GoalResult> CreateGoalAsync(
            string userId,
            string loaiMucTieu,
            string tenMucTieu,
            double? giaTriMucTieu,
            DateTime ngayBatDau,
            DateTime ngayKetThucDuKien,
            string ptId = null,
            string ghiChu = null)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new GoalResult
                    {
                        Success = false,
                        Message = "UserID không được để trống!"
                    };
                }

                if (string.IsNullOrWhiteSpace(loaiMucTieu))
                {
                    return new GoalResult
                    {
                        Success = false,
                        Message = "Loại mục tiêu không được để trống!"
                    };
                }

                if (ngayKetThucDuKien <= ngayBatDau)
                {
                    return new GoalResult
                    {
                        Success = false,
                        Message = "Ngày kết thúc phải sau ngày bắt đầu!"
                    };
                }

                // Tạo MucTieuID tự động
                string mucTieuId = GenerateGoalId();

                // Tạo mục tiêu mới
                var newGoal = new MucTieu
                {
                    MucTieuID = mucTieuId,
                    UserID = userId,
                    LoaiMucTieu = loaiMucTieu.Trim(),
                    TenMucTieu = tenMucTieu?.Trim(),
                    GiaTriMucTieu = giaTriMucTieu,
                    NgayBatDau = ngayBatDau,
                    NgayKetThucDuKien = ngayKetThucDuKien,
                    TrangThai = "Đang thực hiện",
                    PTID = ptId?.Trim(),
                    GhiChu = ghiChu?.Trim(),
                    NgayTao = DateTime.Now
                };

                _dbContext.MucTieu.Add(newGoal);
                _dbContext.SaveChanges();

                return new GoalResult
                {
                    Success = true,
                    Message = "Tạo mục tiêu thành công!",
                    Goal = newGoal
                };
            }
            catch (Exception ex)
            {
                return new GoalResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Cập nhật mục tiêu
        /// </summary>
        public async Task<GoalResult> UpdateGoalAsync(
            string mucTieuId,
            string tenMucTieu = null,
            double? giaTriMucTieu = null,
            DateTime? ngayKetThucDuKien = null,
            string trangThai = null,
            string ghiChu = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mucTieuId))
                {
                    return new GoalResult
                    {
                        Success = false,
                        Message = "MucTieuID không được để trống!"
                    };
                }

                var goal = _dbContext.MucTieu.FirstOrDefault(m => m.MucTieuID == mucTieuId);
                if (goal == null)
                {
                    return new GoalResult
                    {
                        Success = false,
                        Message = "Không tìm thấy mục tiêu!"
                    };
                }

                // Cập nhật các trường nếu có giá trị
                if (!string.IsNullOrWhiteSpace(tenMucTieu))
                    goal.TenMucTieu = tenMucTieu.Trim();

                if (giaTriMucTieu.HasValue)
                    goal.GiaTriMucTieu = giaTriMucTieu.Value;

                if (ngayKetThucDuKien.HasValue)
                {
                    if (ngayKetThucDuKien.Value <= goal.NgayBatDau)
                    {
                        return new GoalResult
                        {
                            Success = false,
                            Message = "Ngày kết thúc phải sau ngày bắt đầu!"
                        };
                    }
                    goal.NgayKetThucDuKien = ngayKetThucDuKien.Value;
                }

                if (!string.IsNullOrWhiteSpace(trangThai))
                    goal.TrangThai = trangThai.Trim();

                if (ghiChu != null)
                    goal.GhiChu = ghiChu.Trim();

                _dbContext.SaveChanges();

                return new GoalResult
                {
                    Success = true,
                    Message = "Cập nhật mục tiêu thành công!",
                    Goal = goal
                };
            }
            catch (Exception ex)
            {
                return new GoalResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Xóa mục tiêu
        /// </summary>
        public async Task<GoalResult> DeleteGoalAsync(string mucTieuId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mucTieuId))
                {
                    return new GoalResult
                    {
                        Success = false,
                        Message = "MucTieuID không được để trống!"
                    };
                }

                var goal = _dbContext.MucTieu.FirstOrDefault(m => m.MucTieuID == mucTieuId);
                if (goal == null)
                {
                    return new GoalResult
                    {
                        Success = false,
                        Message = "Không tìm thấy mục tiêu!"
                    };
                }

                _dbContext.MucTieu.Remove(goal);
                _dbContext.SaveChanges();

                return new GoalResult
                {
                    Success = true,
                    Message = "Xóa mục tiêu thành công!"
                };
            }
            catch (Exception ex)
            {
                return new GoalResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Đánh dấu mục tiêu hoàn thành
        /// </summary>
        public async Task<GoalResult> CompleteGoalAsync(string mucTieuId)
        {
            try
            {
                var goal = _dbContext.MucTieu.FirstOrDefault(m => m.MucTieuID == mucTieuId);
                if (goal == null)
                {
                    return new GoalResult
                    {
                        Success = false,
                        Message = "Không tìm thấy mục tiêu!"
                    };
                }

                goal.TrangThai = "Hoàn thành";
                goal.NgayKetThucThucTe = DateTime.Now;

                _dbContext.SaveChanges();

                return new GoalResult
                {
                    Success = true,
                    Message = "Đánh dấu mục tiêu hoàn thành thành công!",
                    Goal = goal
                };
            }
            catch (Exception ex)
            {
                return new GoalResult
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Tạo MucTieuID tự động (format: goal_0001, goal_0002, ...)
        /// </summary>
        private string GenerateGoalId()
        {
            var lastGoal = _dbContext.MucTieu
                .OrderByDescending(g => g.MucTieuID)
                .FirstOrDefault();

            if (lastGoal == null || !lastGoal.MucTieuID.StartsWith("goal_"))
            {
                return "goal_0001";
            }

            string numberPart = lastGoal.MucTieuID.Substring(5);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                int newNumber = lastNumber + 1;
                return $"goal_{newNumber:D4}";
            }

            int goalCount = _dbContext.MucTieu.Count();
            return $"goal_{(goalCount + 1):D4}";
        }

        /// <summary>
        /// Lấy danh sách bài tập theo loại mục tiêu và trình độ
        /// </summary>
        public async Task<List<ThuVienBaiTap>> GetExercisesByGoalAndLevelAsync(string loaiMucTieu, string nhomCoChinhNhat, string searchBy, string capDo = null)
        {
            return await _goalService.GetExercisesByGoalAndLevelAsync(loaiMucTieu, nhomCoChinhNhat, searchBy, capDo);
        }

        /// <summary>
        /// Lấy chi tiết bài tập
        /// </summary>
        public async Task<ThuVienBaiTap> GetExerciseDetailAsync(string baiTapId)
        {
            return await _goalService.GetExerciseDetailAsync(baiTapId);
        }

        /// <summary>
        /// Tạo kế hoạch luyện tập từ mục tiêu
        /// </summary>
        public async Task<KeHoachLuyenTap> CreateWorkoutPlanAsync(
            string userId,
            string mucTieuId,
            DateTime ngayBatDau,
            DateTime ngayKetThuc,
            string capDo,
            List<WeeklySchedule> weeklySchedules)
        {
            return await _goalService.CreateWorkoutPlanAsync(userId, mucTieuId, ngayBatDau, ngayKetThuc, capDo, weeklySchedules);
        }

        /// <summary>
        /// Lấy danh sách BuoiTap theo KeHoachTapID (bao gồm BaiTapChiTiet)
        /// </summary>
        public Task<List<BuoiTap>> GetBuoiTapByKeHoachTapIdAsync(string keHoachTapId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keHoachTapId))
                {
                    return Task.FromResult(new List<BuoiTap>());
                }

                var buoiTapList = _dbContext.BuoiTap
                    .Where(b => b.KeHoachTapID == keHoachTapId)
                    .ToList();

                // Load BaiTapChiTiet và ThuVienBaiTap cho mỗi BuoiTap
                foreach (var buoiTap in buoiTapList)
                {
                    _dbContext.Entry(buoiTap)
                        .Collection(b => b.BaiTapChiTiet)
                        .Load();

                    // Load ThuVienBaiTap cho mỗi BaiTapChiTiet
                    foreach (var baiTapChiTiet in buoiTap.BaiTapChiTiet)
                    {
                        _dbContext.Entry(baiTapChiTiet)
                            .Reference(bt => bt.ThuVienBaiTap)
                            .Load();
                    }
                }

                return Task.FromResult(buoiTapList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetBuoiTapByKeHoachTapIdAsync error: {ex.Message}");
                throw new Exception($"Lỗi khi lấy danh sách buổi tập: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm bài tập vào buổi tập
        /// </summary>
        public async Task<bool> AddBaiTapChiTietAsync(string buoiTapId, string baiTapId, int? startNumber = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(buoiTapId) || string.IsNullOrWhiteSpace(baiTapId))
                {
                    return false;
                }

                // Lấy thông tin bài tập để lấy SoRep, SoSet, etc.
                var baiTap = _dbContext.ThuVienBaiTap.FirstOrDefault(b => b.BaiTapID == baiTapId);
                if (baiTap == null)
                {
                    return false;
                }

                // Tạo BaiTapChiTietID - nếu có startNumber thì dùng, không thì generate mới
                string baiTapChiTietId;
                if (startNumber.HasValue)
                {
                    int currentNumber = startNumber.Value;
                    baiTapChiTietId = $"btct_{currentNumber:D4}";
                    // Kiểm tra ID đã tồn tại chưa
                    while (_dbContext.BaiTapChiTiet.Any(b => b.BaiTapChiTietID == baiTapChiTietId))
                    {
                        currentNumber++;
                        baiTapChiTietId = $"btct_{currentNumber:D4}";
                    }
                }
                else
                {
                    baiTapChiTietId = GenerateBaiTapChiTietId();
                }

                // Parse SoRep và SoSet (VD: "8-12" -> lấy giá trị trung bình)
                int? soRep = ParseRepSet(baiTap.SoRep);
                int? soSet = ParseRepSet(baiTap.SoSet);

                var baiTapChiTiet = new BaiTapChiTiet
                {
                    BaiTapChiTietID = baiTapChiTietId,
                    BuoiTapID = buoiTapId,
                    BaiTapID = baiTapId,
                    SoRep = soRep,
                    SoSet = soSet,
                    ThoiLuongDeNghi = baiTap.ThoiLuongDeNghi,
                    ThoiGianNghi = baiTap.ThoiGianNghi,
                    TrangThai = "Chưa thực hiện",
                    ThuTuThucHien = 1, // Mặc định là bài tập đầu tiên
                    NgayCapNhat = DateTime.Now
                };

                _dbContext.BaiTapChiTiet.Add(baiTapChiTiet);
                _dbContext.SaveChanges();

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddBaiTapChiTietAsync error: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException.Message}");
                throw new Exception($"Lỗi khi thêm bài tập vào buổi tập: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Parse Rep/Set string (VD: "8-12" -> 10)
        /// </summary>
        private int? ParseRepSet(string repSet)
        {
            if (string.IsNullOrWhiteSpace(repSet))
                return null;

            // Tách chuỗi "8-12" thành [8, 12]
            var parts = repSet.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int min) && int.TryParse(parts[1].Trim(), out int max))
            {
                return (min + max) / 2; // Lấy giá trị trung bình
            }

            // Nếu chỉ có 1 số
            if (int.TryParse(repSet.Trim(), out int singleValue))
            {
                return singleValue;
            }

            return null;
        }

        /// <summary>
        /// Lấy số tiếp theo cho BaiTapChiTietID (không tạo full ID, chỉ lấy số)
        /// </summary>
        public async Task<int> GetNextBaiTapChiTietNumberAsync()
        {
            try
            {
                var last = _dbContext.BaiTapChiTiet
                    .OrderByDescending(b => b.BaiTapChiTietID)
                    .FirstOrDefault();

                if (last == null || !last.BaiTapChiTietID.StartsWith("btct_"))
                {
                    return 1;
                }

                string numberPart = last.BaiTapChiTietID.Substring(5);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    return lastNumber + 1;
                }

                int count = _dbContext.BaiTapChiTiet.Count();
                return count + 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetNextBaiTapChiTietNumberAsync error: {ex.Message}");
                // Trả về số lớn để tránh trùng
                return _dbContext.BaiTapChiTiet.Count() + 1;
            }
        }

        /// <summary>
        /// Tạo BaiTapChiTietID tự động (dùng cho single item)
        /// </summary>
        private string GenerateBaiTapChiTietId()
        {
            var last = _dbContext.BaiTapChiTiet
                .OrderByDescending(b => b.BaiTapChiTietID)
                .FirstOrDefault();

            if (last == null || !last.BaiTapChiTietID.StartsWith("btct_"))
            {
                return "btct_0001";
            }

            string numberPart = last.BaiTapChiTietID.Substring(5);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                int newNumber = lastNumber + 1;
                string baiTapChiTietId = $"btct_{newNumber:D4}";
                
                // Kiểm tra ID đã tồn tại chưa
                while (_dbContext.BaiTapChiTiet.Any(b => b.BaiTapChiTietID == baiTapChiTietId))
                {
                    newNumber++;
                    baiTapChiTietId = $"btct_{newNumber:D4}";
                }
                
                return baiTapChiTietId;
            }

            int count = _dbContext.BaiTapChiTiet.Count();
            return $"btct_{(count + 1):D4}";
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }

    /// <summary>
    /// Kết quả thao tác với mục tiêu
    /// </summary>
    public class GoalResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public MucTieu Goal { get; set; }
    }
}

