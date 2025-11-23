using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.Models;

namespace HealthApp.Services.Interfaces
{
    /// <summary>
    /// Service interface cho tìm kiếm PT
    /// </summary>
    public interface IPTSearchService
    {
        /// <summary>
        /// Lấy danh sách tất cả PT (đã xác minh)
        /// </summary>
        Task<List<PTSearchViewModel>> GetAllPTsAsync();

        /// <summary>
        /// Tìm kiếm PT theo tên
        /// </summary>
        Task<List<PTSearchViewModel>> SearchPTsByNameAsync(string searchText);

        /// <summary>
        /// Lọc PT theo tỉnh/thành phố
        /// </summary>
        Task<List<PTSearchViewModel>> FilterPTsByCityAsync(string city);

        /// <summary>
        /// Lọc PT theo chuyên môn
        /// </summary>
        Task<List<PTSearchViewModel>> FilterPTsBySpecialtyAsync(string specialty);

        /// <summary>
        /// Lấy chi tiết PT theo PTID
        /// </summary>
        Task<PTDetailViewModel> GetPTDetailAsync(string ptId);
    }

    /// <summary>
    /// ViewModel cho danh sách tìm kiếm PT
    /// </summary>
    public class PTSearchViewModel
    {
        public string PTID { get; set; }
        public string UserID { get; set; }
        public string Ten { get; set; }
        public string AnhDaiDien { get; set; }
        public string ChuyenMon { get; set; }
        public int? SoNamKinhNghiem { get; set; }
        public string ThanhPho { get; set; }
        public double? GiaTheoGio { get; set; }
        public double? DiemTrungBinh { get; set; }
        public int? TongDanhGia { get; set; }
    }

    /// <summary>
    /// ViewModel cho chi tiết PT
    /// </summary>
    public class PTDetailViewModel
    {
        public string PTID { get; set; }
        public string UserID { get; set; }
        public string Ten { get; set; }
        public string AnhDaiDien { get; set; }
        public string AnhChanDung { get; set; }
        public string ChuyenMon { get; set; }
        public string ChungChi { get; set; }
        public int? SoNamKinhNghiem { get; set; }
        public string ThanhPho { get; set; }
        public double? GiaTheoGio { get; set; }
        public string TieuSu { get; set; }
        public double? DiemTrungBinh { get; set; }
        public int? TongDanhGia { get; set; }
        public double? TiLeThanhCong { get; set; }
        public int? SoKhachHienTai { get; set; }
        public List<string> DanhSachChuyenMon { get; set; }
        public List<string> DanhSachChungChi { get; set; }
    }
}

