extern alias ef6;

using System;
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
    }
}

