extern alias ef6;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ef6::System.Data.Entity;
using HealthApp.Models;
using HealthApp.Services.Interfaces;
using PTRequestViewModel = HealthApp.Services.Interfaces.PTRequestViewModel;
using PTCustomerViewModel = HealthApp.Services.Interfaces.PTCustomerViewModel;
using PTScheduleViewModel = HealthApp.Services.Interfaces.PTScheduleViewModel;

namespace HealthApp.Services
{
    /// <summary>
    /// Service implementation cho PT Dashboard operations
    /// </summary>
    public class PTDashboardService : IPTDashboardService
    {
        private readonly WF_HealthTracker _context;

        public PTDashboardService(WF_HealthTracker context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> GetPTIDByUserIDAsync(string userId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var pt = _context.HuanLuyenVien
                        .FirstOrDefault(h => h.UserID == userId);
                    return pt?.PTID;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy PTID: {ex.Message}", ex);
                }
            });
        }

        public async Task<int> GetTotalCustomersAsync(string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Đếm số khách hàng đã có booking với PT (Confirmed hoặc Completed)
                    var count = _context.DatLichPT
                        .Where(d => d.PTID == ptId && 
                                   (d.TrangThai == "Confirmed" || d.TrangThai == "Completed"))
                        .Select(d => d.KhachHangID)
                        .Distinct()
                        .Count();
                    return count;
                }
                catch (SqlException sqlEx)
                {
                    // Lỗi database - có thể bảng chưa tồn tại
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra:\n1. Bảng DatLichPT đã được tạo trong database chưa?\n2. Connection string có đúng không?\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    // Lỗi database - bảng chưa tồn tại
                    throw new Exception($"Lỗi database: Bảng DatLichPT chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql để tạo bảng.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy tổng số khách hàng: {ex.Message}", ex);
                }
            });
        }

        public async Task<int> GetTodaySessionsAsync(string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var today = DateTime.Now.Date;
                    var tomorrow = today.AddDays(1);

                    // Đếm số buổi tập hôm nay (Confirmed hoặc Completed) - sử dụng ThoiGianBatDau
                    var count = _context.DatLichPT
                        .Count(d => d.PTID == ptId &&
                                   d.ThoiGianBatDau >= today &&
                                   d.ThoiGianBatDau < tomorrow &&
                                   (d.TrangThai == "Confirmed" || d.TrangThai == "Completed"));
                    return count;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra bảng DatLichPT đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Bảng DatLichPT chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy số buổi tập hôm nay: {ex.Message}", ex);
                }
            });
        }

        public async Task<double> GetMonthlyIncomeAsync(string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var now = DateTime.Now;
                    var startOfMonth = new DateTime(now.Year, now.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1);

                    // Tính tổng thu nhập từ GiaoDich trong tháng này
                    var income = _context.GiaoDich
                        .Where(g => g.PTID == ptId &&
                                   g.NgayGiaoDich >= startOfMonth &&
                                   g.NgayGiaoDich < endOfMonth &&
                                   g.TrangThaiThanhToan == "Completed")
                        .Sum(g => (double?)(g.SoTienPTNhan ?? 0)) ?? 0;
                    return income;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra bảng GiaoDich đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Bảng GiaoDich chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy thu nhập tháng: {ex.Message}", ex);
                }
            });
        }

        public async Task<double> GetAverageRatingAsync(string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Lấy điểm trung bình từ DanhGiaPT
                    var avgRating = _context.DanhGiaPT
                        .Where(d => d.PTID == ptId)
                        .Average(d => (double?)d.Diem) ?? 0;
                    return Math.Round(avgRating, 1);
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra bảng DanhGiaPT đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Bảng DanhGiaPT chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy điểm đánh giá: {ex.Message}", ex);
                }
            });
        }

        public async Task<List<PTRequestViewModel>> GetPTRequestsAsync(string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Lấy các yêu cầu Pending:
                    // 1. Yêu cầu chưa có PTID (PTID = null) - chưa được PT nào chấp nhận
                    // 2. Yêu cầu đã có PTID = ptId và vẫn Pending - đã được PT này chấp nhận nhưng chưa thanh toán
                    // Include Users để lấy thông tin khách hàng
                    var allRequests = _context.DatLichPT
                        .Include("Users")
                        .Where(d => d.TrangThai == "Pending" && 
                                   (string.IsNullOrEmpty(d.PTID) || d.PTID == ptId))
                        .ToList();

                    // Lấy danh sách UserID để query Users riêng
                    var khachHangIDs = allRequests.Select(d => d.KhachHangID).Distinct().ToList();
                    
                    // Query Users trực tiếp từ database để đảm bảo lấy đúng dữ liệu
                    var users = _context.Users
                        .Where(u => khachHangIDs.Contains(u.UserID))
                        .ToList()
                        .ToDictionary(u => u.UserID, u => u);

                    // Group theo LichTrinhID + KhachHangID + PTID: 
                    // Các yêu cầu có cùng LichTrinhID, cùng KhachHangID, và cùng PTID sẽ chỉ hiển thị 1 cái
                    // Điều này đảm bảo các yêu cầu khác nhau (khác PT hoặc khác khách hàng) không bị nhóm lại với nhau
                    var groupedRequests = allRequests
                        .GroupBy(d => 
                        {
                            if (!string.IsNullOrEmpty(d.LichTrinhID))
                            {
                                // Với lịch trình: group theo LichTrinhID + KhachHangID + PTID để đảm bảo tính duy nhất
                                return $"LichTrinh_{d.LichTrinhID}_{d.KhachHangID}_{d.PTID ?? "NULL"}";
                            }
                            else
                            {
                                // Buổi tập đơn lẻ: dùng DatLichID (mỗi DatLichID là duy nhất)
                                return $"Single_{d.DatLichID}";
                            }
                        })
                        .Select(g => 
                        {
                            var groupList = g.ToList();
                            var firstItem = groupList.First();
                            
                            // Đảm bảo tất cả các bản ghi trong group có cùng KhachHangID (nếu có LichTrinhID)
                            string khachHangID = firstItem.KhachHangID;
                            if (!string.IsNullOrEmpty(firstItem.LichTrinhID))
                            {
                                // Kiểm tra xem tất cả các bản ghi có cùng KhachHangID không
                                var distinctKhachHangIDs = groupList.Select(d => d.KhachHangID).Distinct().ToList();
                                if (distinctKhachHangIDs.Count > 1)
                                {
                                    // Nếu có nhiều KhachHangID khác nhau trong cùng LichTrinhID, lấy KhachHangID đầu tiên
                                    // (Trường hợp này không nên xảy ra nhưng để an toàn)
                                    khachHangID = distinctKhachHangIDs.First();
                                }
                            }
                            
                            // Lấy thông tin User từ dictionary dựa trên KhachHangID đã xác định
                            var user = users.ContainsKey(khachHangID) 
                                ? users[khachHangID] 
                                : null;
                            
                            // Nếu có LichTrinhID (không null và không empty), đây là lịch trình (có thể có nhiều buổi tập)
                            if (!string.IsNullOrEmpty(firstItem.LichTrinhID))
                            {
                                // Lấy ngày đầu và cuối của lịch trình
                                var first = groupList.OrderBy(d => d.ThoiGianBatDau).First();
                                var last = groupList.OrderByDescending(d => d.ThoiGianKetThuc).First();
                                
                                // Tính số tuần: (ngày cuối - ngày đầu) / 7, làm tròn lên
                                var soNgay = (last.ThoiGianKetThuc.Date - first.ThoiGianBatDau.Date).TotalDays + 1;
                                var soTuan = (int)Math.Ceiling(soNgay / 7.0);
                                
                                return new PTRequestViewModel
                                {
                                    DatLichID = first.DatLichID, // Dùng DatLichID đầu tiên để xử lý
                                    KhachHangID = khachHangID, // Sử dụng KhachHangID đã xác định
                                    TenKhachHang = user != null ? (user.HoTen ?? user.Username) : "Không xác định",
                                    AnhDaiDien = user?.AnhDaiDien,
                                    NgayGioDat = first.ThoiGianBatDau, // Ngày bắt đầu
                                    ThoiLuong = (int)(last.ThoiGianKetThuc - first.ThoiGianBatDau).TotalMinutes, // Tổng thời lượng
                                    MucTieu = !string.IsNullOrEmpty(first.MucTieuLuyenTap) 
                                        ? first.MucTieuLuyenTap 
                                        : "Chưa có mục tiêu",
                                    ThoiGian = $"{soTuan} tuần", // Hiển thị số tuần cho lịch trình
                                    LichTrinhID = first.LichTrinhID // Lưu LichTrinhID để xử lý
                                };
                            }
                            else
                            {
                                // Buổi tập đơn lẻ (LichTrinhID là null)
                                var d = firstItem;
                                return new PTRequestViewModel
                                {
                                    DatLichID = d.DatLichID,
                                    KhachHangID = d.KhachHangID, // Sử dụng KhachHangID từ bản ghi
                                    TenKhachHang = user != null ? (user.HoTen ?? user.Username) : "Không xác định",
                                    AnhDaiDien = user?.AnhDaiDien,
                                    NgayGioDat = d.ThoiGianBatDau,
                                    ThoiLuong = d.ThoiLuong,
                                    MucTieu = !string.IsNullOrEmpty(d.MucTieuLuyenTap) 
                                        ? d.MucTieuLuyenTap 
                                        : "Chưa có mục tiêu",
                                    ThoiGian = $"{d.ThoiGianBatDau:HH:mm} - {d.ThoiGianKetThuc:HH:mm}",
                                    LichTrinhID = d.LichTrinhID
                                };
                            }
                        })
                        .ToList();

                    return groupedRequests;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra các bảng DatLichPT, Users, MucTieu đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Các bảng DatLichPT, Users, hoặc MucTieu chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy danh sách yêu cầu: {ex.Message}", ex);
                }
            });
        }

        public async Task<List<PTCustomerViewModel>> GetActiveCustomersAsync(string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Lấy khách hàng đang tập (Confirmed hoặc Completed, đã thanh toán)
                    var customers = _context.DatLichPT
                        .Where(d => d.PTID == ptId &&
                                   (d.TrangThai == "Confirmed" || d.TrangThai == "Completed"))
                        .OrderByDescending(d => d.NgayGioDat)
                        .Select(d => new PTCustomerViewModel
                        {
                            DatLichID = d.DatLichID,
                            KhachHangID = d.KhachHangID,
                            TenKhachHang = d.Users.HoTen ?? d.Users.Username,
                            AnhDaiDien = d.Users.AnhDaiDien,
                            NgayGioDat = d.NgayGioDat,
                            TrangThai = d.TrangThai
                        })
                        .ToList();

                    // Format thời gian từ ThoiGianBatDau và ThoiGianKetThuc
                    foreach (var customer in customers)
                    {
                        var datLich = _context.DatLichPT
                            .FirstOrDefault(d => d.DatLichID == customer.DatLichID);
                        if (datLich != null)
                        {
                            customer.ThoiGian = $"{datLich.ThoiGianBatDau:HH:mm} - {datLich.ThoiGianKetThuc:HH:mm}";
                            customer.NgayGioDat = datLich.ThoiGianBatDau; // Cập nhật để đúng
                        }
                        else
                        {
                            // Fallback nếu không tìm thấy
                            var endTime = customer.NgayGioDat.AddMinutes(60);
                            customer.ThoiGian = $"{customer.NgayGioDat:HH:mm} - {endTime:HH:mm}";
                        }
                    }

                    return customers;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra các bảng DatLichPT, Users đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Các bảng DatLichPT hoặc Users chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy danh sách khách hàng: {ex.Message}", ex);
                }
            });
        }

        public async Task<List<PTScheduleViewModel>> GetTodayScheduleAsync(string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var today = DateTime.Now.Date;
                    var tomorrow = today.AddDays(1);

                    // Lấy lịch trình hôm nay (sử dụng ThoiGianBatDau để lọc)
                    var schedules = _context.DatLichPT
                        .Where(d => d.PTID == ptId &&
                                   d.ThoiGianBatDau >= today &&
                                   d.ThoiGianBatDau < tomorrow &&
                                   (d.TrangThai == "Confirmed" || d.TrangThai == "Completed"))
                        .OrderBy(d => d.ThoiGianBatDau)
                        .Select(d => new PTScheduleViewModel
                        {
                            DatLichID = d.DatLichID,
                            KhachHangID = d.KhachHangID,
                            TenKhachHang = d.Users.HoTen ?? d.Users.Username,
                            LoaiBuoiTap = d.LoaiBuoiTap,
                            ThoiGianBatDau = d.ThoiGianBatDau,
                            ThoiGianKetThuc = d.ThoiGianKetThuc,
                            TrangThai = d.TrangThai
                        })
                        .ToList();

                    // Format thời gian từ ThoiGianBatDau và ThoiGianKetThuc
                    foreach (var schedule in schedules)
                    {
                        schedule.ThoiGian = $"{schedule.ThoiGianBatDau:HH:mm} - {schedule.ThoiGianKetThuc:HH:mm}";
                    }

                    return schedules;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra các bảng DatLichPT, Users đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Các bảng DatLichPT hoặc Users chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy lịch trình: {ex.Message}", ex);
                }
            });
        }

        public async Task<bool> AcceptRequestAsync(string datLichID, string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var datLich = _context.DatLichPT
                        .FirstOrDefault(d => d.DatLichID == datLichID);
                    
                    if (datLich == null)
                        return false;

                    // Nếu có LichTrinhID, chấp nhận tất cả các yêu cầu có cùng LichTrinhID
                    if (!string.IsNullOrEmpty(datLich.LichTrinhID))
                    {
                        var allRequests = _context.DatLichPT
                            .Where(d => d.LichTrinhID == datLich.LichTrinhID 
                                     && d.TrangThai == "Pending" 
                                     && string.IsNullOrEmpty(d.PTID))
                            .ToList();

                        foreach (var request in allRequests)
                        {
                            request.PTID = ptId;
                            request.TrangThai = "Pending"; // Giữ nguyên "Pending" vì CHECK constraint không cho phép "PendingPayment"
                            request.NgayCapNhat = DateTime.Now;
                        }
                    }
                    else
                    {
                        // Buổi tập đơn lẻ
                        datLich.PTID = ptId;
                        datLich.TrangThai = "Pending"; // Giữ nguyên "Pending" vì CHECK constraint không cho phép "PendingPayment"
                        datLich.NgayCapNhat = DateTime.Now;
                    }

                    _context.SaveChanges();
                    return true;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database khi cập nhật yêu cầu.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Bảng DatLichPT chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi đồng ý yêu cầu: {ex.Message}", ex);
                }
            });
        }

        public async Task<bool> RejectRequestAsync(string datLichID)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var datLich = _context.DatLichPT
                        .FirstOrDefault(d => d.DatLichID == datLichID);
                    
                    if (datLich == null)
                        return false;

                    // Xóa hoặc đánh dấu Cancelled
                    datLich.TrangThai = "Cancelled";
                    datLich.NgayCapNhat = DateTime.Now;

                    _context.SaveChanges();
                    return true;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database khi cập nhật yêu cầu.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Bảng DatLichPT chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi từ chối yêu cầu: {ex.Message}", ex);
                }
            });
        }

        public async Task<string> CreateTrainingRequestAsync(string khachHangID, string ptId, DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc, string mucTieu)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Generate DatLichID
                    string datLichID = GenerateDatLichID();

                    // Tính thời lượng (phút)
                    int thoiLuong = (int)(gioKetThuc - gioBatDau).TotalMinutes;

                    // Tạo DateTime từ ngày và giờ
                    DateTime thoiGianBatDau = ngay.Date.Add(gioBatDau);
                    DateTime thoiGianKetThuc = ngay.Date.Add(gioKetThuc);

                    // Tạo bản ghi DatLichPT
                    var datLich = new DatLichPT
                    {
                        DatLichID = datLichID,
                        KhachHangID = khachHangID,
                        PTID = !string.IsNullOrEmpty(ptId) ? ptId : null, // Set PTID nếu có, null nếu chưa chọn PT
                        NgayGioDat = thoiGianBatDau,
                        ThoiGianBatDau = thoiGianBatDau,
                        ThoiGianKetThuc = thoiGianKetThuc,
                        ThoiLuong = thoiLuong,
                        LoaiBuoiTap = "In-person",
                        TrangThai = "Pending",
                        MucTieuLuyenTap = mucTieu, // Lưu mục tiêu vào MucTieuLuyenTap
                        LichTrinhID = null, // Buổi tập đơn lẻ không có LichTrinhID
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    };

                    _context.DatLichPT.Add(datLich);
                    _context.SaveChanges();

                    return datLichID;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra bảng DatLichPT đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Bảng DatLichPT chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql để tạo bảng.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi tạo yêu cầu tập luyện: {ex.Message}", ex);
                }
            });
        }

        /// <summary>
        /// Tạo nhiều yêu cầu tập luyện cùng một lịch trình (package)
        /// </summary>
        public async Task<(List<string> datLichIDs, string lichTrinhID)> CreateTrainingScheduleAsync(
            string khachHangID, 
            string ptId, 
            List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)> danhSachNgayGio, 
            string mucTieu)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (danhSachNgayGio == null || danhSachNgayGio.Count == 0)
                    {
                        throw new ArgumentException("Danh sách ngày giờ không được rỗng");
                    }

                    // Generate LichTrinhID chung cho tất cả các buổi tập trong lịch trình này
                    // Đảm bảo mỗi lần gửi yêu cầu (kể cả cùng khách hàng, cùng PT) sẽ có LichTrinhID riêng
                    string lichTrinhID = GenerateLichTrinhID();
                    
                    // Kiểm tra xem LichTrinhID này đã tồn tại chưa (tránh trùng lặp)
                    // Nếu trùng, tạo lại cho đến khi không trùng
                    int maxRetries = 10;
                    int retryCount = 0;
                    while (_context.DatLichPT.Any(d => d.LichTrinhID == lichTrinhID) && retryCount < maxRetries)
                    {
                        lichTrinhID = GenerateLichTrinhID();
                        retryCount++;
                    }
                    
                    // Nếu vẫn trùng sau nhiều lần thử, dùng timestamp để đảm bảo duy nhất
                    if (retryCount >= maxRetries && _context.DatLichPT.Any(d => d.LichTrinhID == lichTrinhID))
                    {
                        lichTrinhID = $"sch_{DateTime.Now:yyyyMMddHHmmssfff}_{khachHangID.Substring(0, Math.Min(5, khachHangID.Length))}";
                    }
                    
                    List<string> datLichIDs = new List<string>();

                    // Lấy DatLichID lớn nhất một lần để tránh trùng lặp khi tạo nhiều bản ghi
                    string lastDatLichID = _context.DatLichPT
                        .OrderByDescending(d => d.DatLichID)
                        .Select(d => d.DatLichID)
                        .FirstOrDefault();
                    
                    int startNumber = 1;
                    if (!string.IsNullOrEmpty(lastDatLichID) && lastDatLichID.StartsWith("bkg_"))
                    {
                        string numberPart = lastDatLichID.Substring(4);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            startNumber = lastNumber + 1;
                        }
                    }

                    // Tạo từng buổi tập với cùng LichTrinhID
                    int currentNumber = startNumber;
                    foreach (var (ngay, gioBatDau, gioKetThuc) in danhSachNgayGio)
                    {
                        // Tạo DatLichID tuần tự để tránh trùng lặp
                        string datLichID = $"bkg_{currentNumber:D4}";
                        currentNumber++;
                        
                        int thoiLuong = (int)(gioKetThuc - gioBatDau).TotalMinutes;
                        DateTime thoiGianBatDau = ngay.Date.Add(gioBatDau);
                        DateTime thoiGianKetThuc = ngay.Date.Add(gioKetThuc);

                        var datLich = new DatLichPT
                        {
                            DatLichID = datLichID,
                            KhachHangID = khachHangID,
                            PTID = !string.IsNullOrEmpty(ptId) ? ptId : null, // Set PTID nếu có
                            NgayGioDat = thoiGianBatDau,
                            ThoiGianBatDau = thoiGianBatDau,
                            ThoiGianKetThuc = thoiGianKetThuc,
                            ThoiLuong = thoiLuong,
                            LoaiBuoiTap = "In-person",
                            TrangThai = "Pending",
                            MucTieuLuyenTap = mucTieu,
                            LichTrinhID = lichTrinhID, // Nhóm các buổi tập lại với nhau
                            NgayTao = DateTime.Now,
                            NgayCapNhat = DateTime.Now
                        };

                        _context.DatLichPT.Add(datLich);
                        datLichIDs.Add(datLichID);
                    }

                    _context.SaveChanges();
                    return (datLichIDs, lichTrinhID);
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra bảng DatLichPT đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Bảng DatLichPT chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql để tạo bảng.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi tạo lịch trình tập luyện: {ex.Message}", ex);
                }
            });
        }

        /// <summary>
        /// Generate LichTrinhID tự động (format: sch_0001, sch_0002, ...)
        /// Đảm bảo mỗi lần gửi yêu cầu tạo một LichTrinhID mới và duy nhất
        /// </summary>
        private string GenerateLichTrinhID()
        {
            try
            {
                // Lấy LichTrinhID lớn nhất (chỉ lấy các ID có format sch_xxxx)
                // Sử dụng Max để đảm bảo lấy được ID lớn nhất ngay cả khi có nhiều ID cùng giá trị
                var lastLichTrinh = _context.DatLichPT
                    .Where(d => d.LichTrinhID != null && d.LichTrinhID.StartsWith("sch_"))
                    .Select(d => d.LichTrinhID)
                    .Distinct()
                    .OrderByDescending(id => id)
                    .FirstOrDefault();

                if (lastLichTrinh == null)
                {
                    return "sch_0001";
                }

                // Extract số từ LichTrinhID cuối cùng
                string numberPart = lastLichTrinh.Substring(4);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    // Tăng số lên và đảm bảo không trùng bằng cách kiểm tra lại database
                    int newNumber = lastNumber + 1;
                    string newLichTrinhID = $"sch_{newNumber:D4}";
                    
                    // Kiểm tra xem ID mới đã tồn tại chưa (tránh trường hợp race condition)
                    int maxAttempts = 100;
                    int attempts = 0;
                    while (_context.DatLichPT.Any(d => d.LichTrinhID == newLichTrinhID) && attempts < maxAttempts)
                    {
                        newNumber++;
                        newLichTrinhID = $"sch_{newNumber:D4}";
                        attempts++;
                    }
                    
                    if (attempts >= maxAttempts)
                    {
                        // Nếu vẫn trùng sau nhiều lần thử, dùng timestamp để đảm bảo duy nhất
                        return $"sch_{DateTime.Now:yyyyMMddHHmmss}";
                    }
                    
                    return newLichTrinhID;
                }

                // Fallback: dùng timestamp để đảm bảo duy nhất
                return $"sch_{DateTime.Now:yyyyMMddHHmmss}";
            }
            catch
            {
                // Fallback nếu có lỗi: dùng timestamp để đảm bảo duy nhất
                return $"sch_{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        /// <summary>
        /// Kiểm tra trùng lịch với các lịch đã có của PT
        /// </summary>
        public async Task<List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)>> CheckOverlappingSchedulesAsync(
            string ptId,
            List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)> danhSachNgayGio)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(ptId) || danhSachNgayGio == null || danhSachNgayGio.Count == 0)
                    {
                        return new List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)>();
                    }

                    // Lấy tất cả các lịch đã có của PT (Confirmed hoặc Pending)
                    var existingBookings = _context.DatLichPT
                        .Where(d => d.PTID == ptId && 
                                   (d.TrangThai == "Confirmed" || d.TrangThai == "Pending"))
                        .ToList();

                    var overlappingSchedules = new List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)>();

                    // Kiểm tra từng lịch mới với các lịch đã có
                    foreach (var (ngay, gioBatDau, gioKetThuc) in danhSachNgayGio)
                    {
                        DateTime thoiGianBatDau = ngay.Date.Add(gioBatDau);
                        DateTime thoiGianKetThuc = ngay.Date.Add(gioKetThuc);

                        // Kiểm tra trùng với các lịch đã có
                        foreach (var booking in existingBookings)
                        {
                            // Kiểm tra trùng thời gian: hai khoảng thời gian trùng nhau nếu:
                            // (start1 < end2) && (start2 < end1)
                            if (thoiGianBatDau < booking.ThoiGianKetThuc && 
                                booking.ThoiGianBatDau < thoiGianKetThuc)
                            {
                                // Trùng lịch, thêm vào danh sách
                                overlappingSchedules.Add((ngay, gioBatDau, gioKetThuc));
                                break; // Chỉ cần thêm một lần cho mỗi lịch mới
                            }
                        }
                    }

                    return overlappingSchedules;
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không throw để không làm crash UI
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi kiểm tra trùng lịch: {ex.Message}");
                    return new List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)>();
                }
            });
        }

        /// <summary>
        /// Kiểm tra trùng lịch với lịch tập của khách hàng (BuoiTap trong KeHoachLuyenTap)
        /// </summary>
        public async Task<List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc, string tenKeHoach)>> CheckOverlappingWithCustomerWorkoutAsync(
            string khachHangID,
            List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc)> danhSachNgayGio)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(khachHangID) || danhSachNgayGio == null || danhSachNgayGio.Count == 0)
                    {
                        return new List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc, string tenKeHoach)>();
                    }

                    // Lấy tất cả các BuoiTap của khách hàng từ các KeHoachLuyenTap đang hoạt động
                    // Chỉ lấy các buổi tập chưa thực hiện hoặc đang thực hiện
                    var customerWorkouts = _context.BuoiTap
                        .Where(bt => bt.KeHoachLuyenTap.UserID == khachHangID &&
                                     bt.KeHoachLuyenTap.TrangThai == "Đang hoạt động" &&
                                     bt.ThoiGianBatDau.HasValue &&
                                     bt.ThoiGianKetThuc.HasValue &&
                                     (bt.TrangThai == "Chưa thực hiện" || bt.TrangThai == "Đang thực hiện"))
                        .Select(bt => new
                        {
                            bt.ThoiGianBatDau,
                            bt.ThoiGianKetThuc,
                            bt.KeHoachLuyenTap.MoTa
                        })
                        .ToList();

                    var overlappingWorkouts = new List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc, string tenKeHoach)>();

                    // Kiểm tra từng lịch yêu cầu mới với các buổi tập của khách hàng
                    foreach (var (ngay, gioBatDau, gioKetThuc) in danhSachNgayGio)
                    {
                        DateTime thoiGianBatDau = ngay.Date.Add(gioBatDau);
                        DateTime thoiGianKetThuc = ngay.Date.Add(gioKetThuc);

                        // Kiểm tra trùng với các buổi tập của khách hàng
                        foreach (var workout in customerWorkouts)
                        {
                            if (workout.ThoiGianBatDau.HasValue && workout.ThoiGianKetThuc.HasValue)
                            {
                                DateTime workoutStart = workout.ThoiGianBatDau.Value;
                                DateTime workoutEnd = workout.ThoiGianKetThuc.Value;

                                // Kiểm tra trùng thời gian: hai khoảng thời gian trùng nhau nếu:
                                // (start1 < end2) && (start2 < end1)
                                if (thoiGianBatDau < workoutEnd && workoutStart < thoiGianKetThuc)
                                {
                                    // Trùng lịch, thêm vào danh sách
                                    string tenKeHoach = !string.IsNullOrWhiteSpace(workout.MoTa) 
                                        ? workout.MoTa 
                                        : "Kế hoạch luyện tập";
                                    overlappingWorkouts.Add((ngay, gioBatDau, gioKetThuc, tenKeHoach));
                                    break; // Chỉ cần thêm một lần cho mỗi lịch mới
                                }
                            }
                        }
                    }

                    return overlappingWorkouts;
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không throw để không làm crash UI
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi kiểm tra trùng lịch với lịch tập của khách hàng: {ex.Message}");
                    return new List<(DateTime ngay, TimeSpan gioBatDau, TimeSpan gioKetThuc, string tenKeHoach)>();
                }
            });
        }

        /// <summary>
        /// Generate DatLichID tự động (format: bkg_0001, bkg_0002, ...)
        /// </summary>
        private string GenerateDatLichID()
        {
            try
            {
                // Lấy DatLichID lớn nhất
                var lastDatLich = _context.DatLichPT
                    .OrderByDescending(d => d.DatLichID)
                    .FirstOrDefault();

                if (lastDatLich == null)
                {
                    return "bkg_0001";
                }

                // Extract số từ DatLichID cuối cùng
                if (lastDatLich.DatLichID.StartsWith("bkg_"))
                {
                    string numberPart = lastDatLich.DatLichID.Substring(4);
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        int newNumber = lastNumber + 1;
                        return $"bkg_{newNumber:D4}";
                    }
                }

                // Fallback: đếm số lượng và tạo ID mới
                int count = _context.DatLichPT.Count();
                return $"bkg_{(count + 1):D4}";
            }
            catch
            {
                // Fallback nếu có lỗi
                int count = _context.DatLichPT.Count();
                return $"bkg_{(count + 1):D4}";
            }
        }
    }
}

