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
    /// Service implementation cho tìm kiếm PT
    /// </summary>
    public class PTSearchService : IPTSearchService
    {
        private readonly WF_HealthTracker _context;

        public PTSearchService(WF_HealthTracker context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<PTSearchViewModel>> GetAllPTsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Lấy tất cả PT đã xác minh (DaXacMinh = true)
                    var pts = _context.HuanLuyenVien
                        .Where(h => h.DaXacMinh == true)
                        .Select(h => new PTSearchViewModel
                        {
                            PTID = h.PTID,
                            UserID = h.UserID,
                            Ten = h.Users.HoTen ?? h.Users.Username,
                            AnhDaiDien = h.AnhDaiDien ?? h.Users.AnhDaiDien,
                            ChuyenMon = h.ChuyenMon,
                            SoNamKinhNghiem = h.SoNamKinhNghiem,
                            ThanhPho = h.ThanhPho,
                            GiaTheoGio = h.GiaTheoGio,
                            DiemTrungBinh = h.DiemTrungBinh ?? 0,
                            TongDanhGia = h.TongDanhGia ?? 0
                        })
                        .ToList();

                    return pts;
                }
                catch (SqlException sqlEx)
                {
                    throw new Exception($"Lỗi kết nối database. Vui lòng kiểm tra các bảng HuanLuyenVien, Users đã được tạo chưa.\n\nChi tiết: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex) when (ex.Message.Contains("Invalid object name") || ex.Message.Contains("does not exist"))
                {
                    throw new Exception($"Lỗi database: Các bảng HuanLuyenVien hoặc Users chưa được tạo trong database.\nVui lòng chạy script SQL trong file WF_HealthTracker.sql.\n\nChi tiết: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy danh sách PT: {ex.Message}", ex);
                }
            });
        }

        public async Task<List<PTSearchViewModel>> SearchPTsByNameAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetAllPTsAsync();

            return await Task.Run(() =>
            {
                try
                {

                    var pts = _context.HuanLuyenVien
                        .Where(h => h.DaXacMinh == true &&
                                   (h.Users.HoTen.Contains(searchText) || 
                                    h.Users.Username.Contains(searchText) ||
                                    h.ChuyenMon.Contains(searchText)))
                        .Select(h => new PTSearchViewModel
                        {
                            PTID = h.PTID,
                            UserID = h.UserID,
                            Ten = h.Users.HoTen ?? h.Users.Username,
                            AnhDaiDien = h.AnhDaiDien ?? h.Users.AnhDaiDien,
                            ChuyenMon = h.ChuyenMon,
                            SoNamKinhNghiem = h.SoNamKinhNghiem,
                            ThanhPho = h.ThanhPho,
                            GiaTheoGio = h.GiaTheoGio,
                            DiemTrungBinh = h.DiemTrungBinh ?? 0,
                            TongDanhGia = h.TongDanhGia ?? 0
                        })
                        .ToList();

                    return pts;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi tìm kiếm PT: {ex.Message}", ex);
                }
            });
        }

        public async Task<List<PTSearchViewModel>> FilterPTsByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return await GetAllPTsAsync();

            return await Task.Run(() =>
            {
                try
                {

                    var pts = _context.HuanLuyenVien
                        .Where(h => h.DaXacMinh == true &&
                                   h.ThanhPho.Contains(city))
                        .Select(h => new PTSearchViewModel
                        {
                            PTID = h.PTID,
                            UserID = h.UserID,
                            Ten = h.Users.HoTen ?? h.Users.Username,
                            AnhDaiDien = h.AnhDaiDien ?? h.Users.AnhDaiDien,
                            ChuyenMon = h.ChuyenMon,
                            SoNamKinhNghiem = h.SoNamKinhNghiem,
                            ThanhPho = h.ThanhPho,
                            GiaTheoGio = h.GiaTheoGio,
                            DiemTrungBinh = h.DiemTrungBinh ?? 0,
                            TongDanhGia = h.TongDanhGia ?? 0
                        })
                        .ToList();

                    return pts;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lọc PT theo thành phố: {ex.Message}", ex);
                }
            });
        }

        public async Task<List<PTSearchViewModel>> FilterPTsBySpecialtyAsync(string specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty))
                return await GetAllPTsAsync();

            return await Task.Run(() =>
            {
                try
                {

                    var pts = _context.HuanLuyenVien
                        .Where(h => h.DaXacMinh == true &&
                                   h.ChuyenMon.Contains(specialty))
                        .Select(h => new PTSearchViewModel
                        {
                            PTID = h.PTID,
                            UserID = h.UserID,
                            Ten = h.Users.HoTen ?? h.Users.Username,
                            AnhDaiDien = h.AnhDaiDien ?? h.Users.AnhDaiDien,
                            ChuyenMon = h.ChuyenMon,
                            SoNamKinhNghiem = h.SoNamKinhNghiem,
                            ThanhPho = h.ThanhPho,
                            GiaTheoGio = h.GiaTheoGio,
                            DiemTrungBinh = h.DiemTrungBinh ?? 0,
                            TongDanhGia = h.TongDanhGia ?? 0
                        })
                        .ToList();

                    return pts;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lọc PT theo chuyên môn: {ex.Message}", ex);
                }
            });
        }

        public async Task<PTDetailViewModel> GetPTDetailAsync(string ptId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var pt = _context.HuanLuyenVien
                        .FirstOrDefault(h => h.PTID == ptId && h.DaXacMinh == true);

                    if (pt == null)
                        return null;

                    // Parse chuyên môn từ string
                    // Logic: "Cân nặng Tăng cơ" = 2 chuyên môn, "Cân nặng" hoặc "Tăng cơ" = 1 chuyên môn
                    var danhSachChuyenMon = new List<string>();
                    if (!string.IsNullOrEmpty(pt.ChuyenMon))
                    {
                        var chuyenMon = pt.ChuyenMon.Trim();
                        
                        // Nếu có dấu phẩy, tách ra
                        if (chuyenMon.Contains(","))
                        {
                            danhSachChuyenMon = chuyenMon.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                        }
                        // Nếu chứa cả "Cân nặng" và "Tăng cơ" (có thể là "Cân nặng Tăng cơ" hoặc "Tăng cơ Cân nặng") → tách thành 2
                        else if (chuyenMon.Contains("Cân nặng") && chuyenMon.Contains("Tăng cơ"))
                        {
                            // Tách thành 2 chuyên môn riêng biệt
                            danhSachChuyenMon.Add("Cân nặng");
                            danhSachChuyenMon.Add("Tăng cơ");
                        }
                        else
                        {
                            // Chỉ có 1 chuyên môn: "Cân nặng" hoặc "Tăng cơ"
                            danhSachChuyenMon.Add(chuyenMon);
                        }
                    }

                    // Parse chứng chỉ từ string
                    // Logic: mỗi phần sau dấu phẩy = 1 chứng chỉ (ví dụ: "eg,da,ot" = 3 chứng chỉ)
                    var danhSachChungChi = new List<string>();
                    if (!string.IsNullOrEmpty(pt.ChungChi))
                    {
                        var chungChi = pt.ChungChi.Trim();
                        
                        // Nếu có dấu phẩy, tách ra
                        if (chungChi.Contains(","))
                        {
                            danhSachChungChi = chungChi.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                        }
                        else
                        {
                            // Chỉ có 1 chứng chỉ
                            danhSachChungChi.Add(chungChi);
                        }
                    }

                    // Đếm số học viên (khách hàng đã có booking với PT)
                    var soHocVien = _context.DatLichPT
                        .Where(d => d.PTID == ptId &&
                                   (d.TrangThai == "Confirmed" || d.TrangThai == "Completed"))
                        .Select(d => d.KhachHangID)
                        .Distinct()
                        .Count();

                    var detail = new PTDetailViewModel
                    {
                        PTID = pt.PTID,
                        UserID = pt.UserID,
                        Ten = pt.Users.HoTen ?? pt.Users.Username,
                        AnhDaiDien = pt.AnhDaiDien ?? pt.Users.AnhDaiDien,
                        AnhChanDung = pt.AnhChanDung,
                        ChuyenMon = pt.ChuyenMon,
                        ChungChi = pt.ChungChi,
                        SoNamKinhNghiem = pt.SoNamKinhNghiem,
                        ThanhPho = pt.ThanhPho,
                        GiaTheoGio = pt.GiaTheoGio,
                        TieuSu = pt.TieuSu,
                        DiemTrungBinh = pt.DiemTrungBinh ?? 0,
                        TongDanhGia = pt.TongDanhGia ?? 0,
                        TiLeThanhCong = pt.TiLeThanhCong ?? 0,
                        SoKhachHienTai = soHocVien,
                        DanhSachChuyenMon = danhSachChuyenMon,
                        DanhSachChungChi = danhSachChungChi
                    };

                    return detail;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lấy chi tiết PT: {ex.Message}", ex);
                }
            });
        }
    }
}

