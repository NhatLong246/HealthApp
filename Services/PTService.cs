extern alias ef6;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ef6::System.Data.Entity;
using HealthApp.Models;
using HealthApp.Services.Interfaces;

namespace HealthApp.Services
{
    /// <summary>
    /// Service implementation cho PT operations
    /// </summary>
    public class PTService : IPTService
    {
        private readonly WF_HealthTracker _context;

        public PTService(WF_HealthTracker context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PTRegistrationResult> RegisterPTAsync(HuanLuyenVien huanLuyenVien, string userId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Kiểm tra user đã đăng ký PT chưa (sử dụng try-catch riêng để xử lý lỗi database)
                    HuanLuyenVien existingPT = null;
                    try
                    {
                        existingPT = _context.HuanLuyenVien
                            .FirstOrDefault(h => h.UserID == userId);
                    }
                    catch (SqlException sqlEx)
                    {
                        // Lỗi database - có thể bảng chưa tồn tại
                        return new PTRegistrationResult
                        {
                            Success = false,
                            Message = $"Lỗi kết nối database. Vui lòng kiểm tra:\n1. Bảng HuanLuyenVien đã được tạo trong database chưa?\n2. Connection string có đúng không?\n\nChi tiết: {sqlEx.Message}"
                        };
                    }
                    catch (Exception dbEx) when (dbEx.Message.Contains("Invalid object name") || dbEx.Message.Contains("does not exist"))
                    {
                        // Lỗi database - bảng chưa tồn tại
                        return new PTRegistrationResult
                        {
                            Success = false,
                            Message = $"Lỗi database: Bảng HuanLuyenVien chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql để tạo bảng.\n\nChi tiết: {dbEx.InnerException?.Message ?? dbEx.Message}"
                        };
                    }

                    if (existingPT != null)
                    {
                        return new PTRegistrationResult
                        {
                            Success = false,
                            Message = "Bạn đã đăng ký làm PT rồi!"
                        };
                    }

                    // Lấy user từ database
                    var user = _context.Users.Find(userId);
                    if (user == null)
                    {
                        return new PTRegistrationResult
                        {
                            Success = false,
                            Message = "Không tìm thấy thông tin người dùng!"
                        };
                    }

                    // Tạo PTID mới
                    huanLuyenVien.PTID = GeneratePTIDAsync().Result;
                    huanLuyenVien.UserID = userId;
                    huanLuyenVien.DaXacMinh = true; // Tự động xác minh khi đăng ký
                    huanLuyenVien.NgayTao = DateTime.Now;
                    huanLuyenVien.NgayCapNhat = DateTime.Now;
                    huanLuyenVien.SoKhachHienTai = 0;
                    huanLuyenVien.NhanKhach = true;
                    huanLuyenVien.TongDanhGia = 0;

                    // Thêm vào database
                    _context.HuanLuyenVien.Add(huanLuyenVien);

                    // Cập nhật Role của user thành "PT"
                    user.Role = "PT";

                    // Lưu thay đổi
                    _context.SaveChanges();

                    return new PTRegistrationResult
                    {
                        Success = true,
                        Message = "Đăng ký làm PT thành công!",
                        HuanLuyenVien = huanLuyenVien
                    };
                }
                catch (SqlException sqlEx)
                {
                    // Lỗi database cụ thể
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = $"Lỗi database: {sqlEx.Message}\n\nVui lòng kiểm tra:\n1. Bảng HuanLuyenVien đã được tạo trong database chưa?\n2. Connection string có đúng không?"
                    };
                }
                catch (Exception dbEx) when (dbEx.Message.Contains("Invalid object name") || dbEx.InnerException?.Message?.Contains("Invalid object name") == true)
                {
                    // Lỗi database - bảng chưa tồn tại
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = $"Lỗi database: Bảng HuanLuyenVien chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql để tạo bảng.\n\nChi tiết: {dbEx.InnerException?.Message ?? dbEx.Message}"
                    };
                }
                catch (Exception ex)
                {
                    return new PTRegistrationResult
                    {
                        Success = false,
                        Message = $"Đã xảy ra lỗi: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message ?? "Không có"}"
                    };
                }
            });
        }

        public Task<bool> IsPTRegisteredAsync(string userId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return _context.HuanLuyenVien
                        .Any(h => h.UserID == userId);
                }
                catch (SqlException)
                {
                    // Nếu bảng chưa tồn tại, trả về false
                    return false;
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.InnerException?.Message?.Contains("Invalid object name") == true)
                {
                    // Nếu bảng chưa tồn tại, trả về false
                    return false;
                }
                catch
                {
                    // Các lỗi khác, trả về false
                    return false;
                }
            });
        }

        public Task<bool> IsCCCDRegisteredAsync(string soCCCD)
        {
            return Task.Run(() =>
            {
                try
                {
                    var normalized = soCCCD?.Trim();
                    if (string.IsNullOrEmpty(normalized))
                    {
                        return false;
                    }

                    var pattern = $"\"SoCCCD\":\"{normalized}\"";
                    return _context.HuanLuyenVien
                        .Any(h => h.TieuSu != null && h.TieuSu.Contains(pattern));
                }
                catch (SqlException)
                {
                    // Nếu bảng chưa tồn tại, xem như chưa có CCCD nào được sử dụng
                    return false;
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.InnerException?.Message?.Contains("Invalid object name") == true)
                {
                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Task<string> GeneratePTIDAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    // Lấy PTID lớn nhất hiện tại
                    var lastPT = _context.HuanLuyenVien
                        .OrderByDescending(h => h.PTID)
                        .FirstOrDefault();

                    int nextNumber = 1;
                    if (lastPT != null && !string.IsNullOrEmpty(lastPT.PTID))
                    {
                        // Extract số từ PTID (ví dụ: PT_0001 -> 1)
                        var parts = lastPT.PTID.Split('_');
                        if (parts.Length > 1 && int.TryParse(parts[1], out int lastNumber))
                        {
                            nextNumber = lastNumber + 1;
                        }
                    }

                    // Format: PT_0001, PT_0002, ...
                    return $"PT_{nextNumber:D4}";
                }
                catch (SqlException)
                {
                    // Nếu bảng chưa tồn tại, bắt đầu từ 1
                    return "PT_0001";
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.InnerException?.Message?.Contains("Invalid object name") == true)
                {
                    // Nếu bảng chưa tồn tại, bắt đầu từ 1
                    return "PT_0001";
                }
                catch
                {
                    // Các lỗi khác, bắt đầu từ 1
                    return "PT_0001";
                }
            });
        }

        /// <summary>
        /// Lấy danh sách lịch đặt đã Confirmed của một PT trong một ngày cụ thể
        /// </summary>
        public Task<IList<DatLichPT>> GetConfirmedBookingsForPTOnDateAsync(string ptId, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(ptId))
                throw new ArgumentNullException(nameof(ptId));

            return Task.Run<IList<DatLichPT>>(() =>
            {
                try
                {
                    var start = date.Date;
                    var end = start.AddDays(1);

                    using (var dbContext = new WF_HealthTracker())
                    {
                        return dbContext.DatLichPT
                            .Include("Users")
                            .Where(d =>
                                d.PTID == ptId &&
                                d.NgayGioDat >= start &&
                                d.NgayGioDat < end &&
                                d.TrangThai == "Confirmed")
                            .OrderBy(d => d.NgayGioDat)
                            .ToList();
                    }
                }
                catch (SqlException)
                {
                    return new DatLichPT[0];
                }
                catch
                {
                    return new DatLichPT[0];
                }
            });
        }

        public Task<IList<ThuVienBaiTap>> GetExercisesByGoalAsync(string goal)
        {
            return Task.Run<IList<ThuVienBaiTap>>(() =>
            {
                try
                {
                    var normalizedGoal = (goal ?? string.Empty).Trim();

                    var query = _context.ThuVienBaiTap.AsQueryable();

                    if (!string.IsNullOrEmpty(normalizedGoal))
                    {
                        query = query.Where(e =>
                            e.LoaiMucTieu == normalizedGoal ||
                            e.LoaiMucTieu.Contains(normalizedGoal));
                    }

                    return query
                        .OrderByDescending(e => e.DoPhoBien)
                        .ThenBy(e => e.TenBaiTap)
                        .ToList();
                }
                catch (SqlException)
                {
                    return new List<ThuVienBaiTap>();
                }
                catch
                {
                    return new List<ThuVienBaiTap>();
                }
            });
        }

        public Task<GiaoBaiTapChoUser> CreateAssignmentAsync(GiaoBaiTapChoUser assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            return Task.Run(() =>
            {
                try
                {
                    using (var dbContext = new WF_HealthTracker())
                    {
                        assignment.GiaoBaiTapID = assignment.GiaoBaiTapID ?? GenerateAssignmentId(dbContext);
                        assignment.NgayGiao = assignment.NgayGiao ?? DateTime.Now;
                        assignment.TrangThai = assignment.TrangThai ?? "Assigned";

                        dbContext.GiaoBaiTapChoUser.Add(assignment);
                        dbContext.SaveChanges();
                        return assignment;
                    }
                }
                catch (SqlException)
                {
                    return null;
                }
            });
        }

        public Task<GiaoBaiTapChoUser> UpdateAssignmentAsync(GiaoBaiTapChoUser assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            return Task.Run(() =>
            {
                try
                {
                    using (var dbContext = new WF_HealthTracker())
                    {
                        var entity = dbContext.GiaoBaiTapChoUser
                            .FirstOrDefault(a => a.GiaoBaiTapID == assignment.GiaoBaiTapID);

                        if (entity == null)
                            return (GiaoBaiTapChoUser)null;

                        dbContext.Entry(entity).CurrentValues.SetValues(assignment);
                        dbContext.SaveChanges();
                        return entity;
                    }
                }
                catch (SqlException)
                {
                    return null;
                }
            });
        }

        public Task<IList<GiaoBaiTapChoUser>> GetAssignmentsByDatLichIdsAsync(IEnumerable<string> datLichIds)
        {
            if (datLichIds == null)
                throw new ArgumentNullException(nameof(datLichIds));

            var idList = datLichIds.Where(id => !string.IsNullOrWhiteSpace(id))
                                   .Select(id => id.Trim())
                                   .ToList();

            if (idList.Count == 0)
                return Task.FromResult<IList<GiaoBaiTapChoUser>>(new List<GiaoBaiTapChoUser>());

            return Task.Run<IList<GiaoBaiTapChoUser>>(() =>
            {
                try
                {
                    using (var dbContext = new WF_HealthTracker())
                    {
                        return dbContext.GiaoBaiTapChoUser
                            .Where(a => a.DatLichID != null && idList.Contains(a.DatLichID))
                            .ToList();
                    }
                }
                catch (SqlException)
                {
                    return new List<GiaoBaiTapChoUser>();
                }
            });
        }

        public Task<GiaoBaiTapChoUser> GetAssignmentAsync(string datLichId, string thuVienBaiTapId)
        {
            if (string.IsNullOrWhiteSpace(datLichId) || string.IsNullOrWhiteSpace(thuVienBaiTapId))
                return Task.FromResult<GiaoBaiTapChoUser>(null);

            var datLich = datLichId.Trim();
            var template = thuVienBaiTapId.Trim();

            return Task.Run(() =>
            {
                try
                {
                    using (var dbContext = new WF_HealthTracker())
                    {
                        return dbContext.GiaoBaiTapChoUser
                            .FirstOrDefault(a => a.DatLichID == datLich && a.ThuVienBaiTapID == template);
                    }
                }
                catch (SqlException)
                {
                    return null;
                }
            });
        }

        public Task ClearAssignmentsForBookingAsync(string datLichId)
        {
            if (string.IsNullOrWhiteSpace(datLichId))
                return Task.CompletedTask;

            var trimmed = datLichId.Trim();
            return Task.Run(() =>
            {
                try
                {
                    using (var dbContext = new WF_HealthTracker())
                    {
                        var list = dbContext.GiaoBaiTapChoUser
                            .Where(a => a.DatLichID == trimmed)
                            .ToList();
                        if (list.Count == 0)
                            return;

                        dbContext.GiaoBaiTapChoUser.RemoveRange(list);
                        dbContext.SaveChanges();
                    }
                }
                catch (SqlException)
                {
                    // bỏ qua lỗi DB, không để crash UI
                }
            });
        }

        public Task<IList<GiaoBaiTapChoUser>> GetAssignmentsByPTAndDateAsync(string ptId, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(ptId))
                return Task.FromResult<IList<GiaoBaiTapChoUser>>(new List<GiaoBaiTapChoUser>());

            var trimmedPtId = ptId.Trim();
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return Task.Run<IList<GiaoBaiTapChoUser>>(() =>
            {
                try
                {
                    using (var dbContext = new WF_HealthTracker())
                    {
                        return dbContext.GiaoBaiTapChoUser
                            .Include("Users")
                            .Include("ThuVienBaiTap")
                            .Include("DatLichPT")
                            .Where(a => a.PTID == trimmedPtId &&
                                       a.NgayGiao != null &&
                                       a.NgayGiao >= startDate &&
                                       a.NgayGiao < endDate)
                            .OrderByDescending(a => a.NgayGiao)
                            .ToList();
                    }
                }
                catch (SqlException)
                {
                    return new List<GiaoBaiTapChoUser>();
                }
            });
        }

        public Task<IList<GiaoBaiTapChoUser>> GetAssignmentsByUserAndDateAsync(string userId, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Task.FromResult<IList<GiaoBaiTapChoUser>>(new List<GiaoBaiTapChoUser>());

            var trimmedUserId = userId.Trim();
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return Task.Run<IList<GiaoBaiTapChoUser>>(() =>
            {
                try
                {
                    using (var dbContext = new WF_HealthTracker())
                    {
                        return dbContext.GiaoBaiTapChoUser
                            .Include("HuanLuyenVien")
                            .Include("HuanLuyenVien.Users")
                            .Include("ThuVienBaiTap")
                            .Include("DatLichPT")
                            .Include("DatLichPT.HuanLuyenVien")
                            .Include("DatLichPT.HuanLuyenVien.Users")
                            .Where(a => a.UserID == trimmedUserId &&
                                       (
                                           (a.DatLichPT != null &&
                                            DbFunctions.TruncateTime(a.DatLichPT.ThoiGianBatDau) == startDate) ||
                                           (a.DatLichPT == null &&
                                            a.NgayGiao != null &&
                                            DbFunctions.TruncateTime(a.NgayGiao) == startDate)
                                       ))
                            .OrderBy(a => a.DatLichPT != null ? a.DatLichPT.ThoiGianBatDau : DateTime.MaxValue)
                            .ThenBy(a => a.NgayGiao)
                            .ToList();
                    }
                }
                catch (SqlException)
                {
                    // Lỗi database (ví dụ bảng chưa tồn tại), không để crash UI
                    return new List<GiaoBaiTapChoUser>();
                }
                catch (Exception ex)
                {
                    // Một số lỗi EF bọc SqlException trong EntityCommandExecutionException
                    // hoặc các lỗi khác – log lại rồi trả về danh sách rỗng để tránh văng app.
                    System.Diagnostics.Debug.WriteLine($"[PTService] Lỗi khi load assignments theo User + Date: {ex.Message} | Inner: {ex.InnerException?.Message}");
                    return new List<GiaoBaiTapChoUser>();
                }
            });
        }

        private string GenerateAssignmentId(WF_HealthTracker context = null)
        {
            try
            {
                context = context ?? new WF_HealthTracker();

                var last = context.GiaoBaiTapChoUser
                    .OrderByDescending(a => a.GiaoBaiTapID)
                    .FirstOrDefault();

                if (last == null || string.IsNullOrEmpty(last.GiaoBaiTapID))
                    return "assign_0001";

                if (last.GiaoBaiTapID.StartsWith("assign_") &&
                    int.TryParse(last.GiaoBaiTapID.Substring(7), out int lastNumber))
                {
                    return $"assign_{(lastNumber + 1):D4}";
                }

                var count = _context.GiaoBaiTapChoUser.Count();
                return $"assign_{(count + 1):D4}";
            }
            catch
            {
                using (var dbContext = new WF_HealthTracker())
                {
                    var count = dbContext.GiaoBaiTapChoUser.Count();
                    return $"assign_{(count + 1):D4}";
                }
            }
        }
    }
}

