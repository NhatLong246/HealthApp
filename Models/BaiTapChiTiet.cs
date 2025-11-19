namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BaiTapChiTiet")]
    public partial class BaiTapChiTiet
    {
        [StringLength(20)]
        public string BaiTapChiTietID { get; set; }

        [Required]
        [StringLength(20)]
        public string BuoiTapID { get; set; }

        [Required]
        [StringLength(20)]
        public string BaiTapID { get; set; }

        public int? SoSet { get; set; }

        public int? SoRep { get; set; }

        public int? ThoiLuongDeNghi { get; set; }

        public int? ThoiGianNghi { get; set; }

        public double? TrongLuong { get; set; }

        public double? Calories { get; set; }

        public int? ThuTuThucHien { get; set; }

        public DateTime? ThoiGianBatDau { get; set; }

        public DateTime? ThoiGianKetThuc { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public virtual BuoiTap BuoiTap { get; set; }

        public virtual ThuVienBaiTap ThuVienBaiTap { get; set; }
    }
}
