using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Models;

namespace HealthApp.Controllers
{
    /// <summary>
    /// Controller xử lý logic Mục Tiêu (Goals)
    /// </summary>
    public class GoalController : IDisposable
    {
        private readonly WF_HealthTracker _dbContext;

        public GoalController()
        {
            _dbContext = new WF_HealthTracker();
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

                var query = _dbContext.MucTieu.Where(m => m.UserID == userId);

                if (!string.IsNullOrWhiteSpace(trangThai))
                {
                    query = query.Where(m => m.TrangThai == trangThai);
                }

                return query.OrderByDescending(m => m.NgayTao).ToList();
            }
            catch (Exception ex)
            {
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
                await Task.Run(() => _dbContext.SaveChanges());

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

                await Task.Run(() => _dbContext.SaveChanges());

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
                await Task.Run(() => _dbContext.SaveChanges());

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

                await Task.Run(() => _dbContext.SaveChanges());

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

