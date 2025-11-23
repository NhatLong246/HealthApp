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
                    // Lấy các yêu cầu Pending, chưa có PTID (PTID = null hoặc empty)
                    // Chỉ hiển thị các yêu cầu chưa được PT nào đồng ý
                    var requests = _context.DatLichPT
                        .Where(d => d.TrangThai == "Pending" && string.IsNullOrEmpty(d.PTID))
                        .Select(d => new PTRequestViewModel
                        {
                            DatLichID = d.DatLichID,
                            KhachHangID = d.KhachHangID,
                            TenKhachHang = d.Users.HoTen ?? d.Users.Username,
                            AnhDaiDien = d.Users.AnhDaiDien,
                            NgayGioDat = d.ThoiGianBatDau, // Sử dụng ThoiGianBatDau
                            ThoiLuong = d.ThoiLuong
                        })
                        .ToList();

                    // Lấy mục tiêu của từng khách hàng và format thời gian
                    foreach (var request in requests)
                    {
                        var datLich = _context.DatLichPT
                            .FirstOrDefault(d => d.DatLichID == request.DatLichID);
                        
                        if (datLich != null)
                        {
                            // Lấy mục tiêu từ GhiChu (đã lưu khi tạo yêu cầu)
                            request.MucTieu = !string.IsNullOrEmpty(datLich.GhiChu) 
                                ? datLich.GhiChu 
                                : "Chưa có mục tiêu";

                            // Format thời gian từ ThoiGianBatDau và ThoiGianKetThuc
                            request.ThoiGian = $"{datLich.ThoiGianBatDau:HH:mm} - {datLich.ThoiGianKetThuc:HH:mm}";
                            request.NgayGioDat = datLich.ThoiGianBatDau; // Cập nhật lại để đúng
                        }
                        else
                        {
                            // Fallback nếu không tìm thấy (không nên xảy ra)
                            request.MucTieu = "Chưa có mục tiêu";
                            var endTime = request.NgayGioDat.AddMinutes(request.ThoiLuong ?? 60);
                            request.ThoiGian = $"{request.NgayGioDat:HH:mm} - {endTime:HH:mm}";
                        }
                    }

                    return requests;
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

                    // Cập nhật PTID và TrangThai
                    datLich.PTID = ptId;
                    datLich.TrangThai = "Confirmed";
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
                        PTID = null, // Chưa được PT chấp nhận
                        NgayGioDat = thoiGianBatDau,
                        ThoiGianBatDau = thoiGianBatDau,
                        ThoiGianKetThuc = thoiGianKetThuc,
                        ThoiLuong = thoiLuong,
                        LoaiBuoiTap = "In-person",
                        TrangThai = "Pending",
                        GhiChu = mucTieu, // Lưu mục tiêu vào GhiChu
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

