namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GiaoDich")]
    public partial class GiaoDich
    {
        [StringLength(20)]
        public string GiaoDichID { get; set; }

        [Required]
        [StringLength(20)]
        public string DatLichID { get; set; }

        [Required]
        [StringLength(20)]
        public string KhachHangID { get; set; }

        [Required]
        [StringLength(20)]
        public string PTID { get; set; }

        public double SoTien { get; set; }

        public double? HoaHongApp { get; set; }

        public double? SoTienHoaHong { get; set; }

        public double? SoTienPTNhan { get; set; }

        [StringLength(20)]
        public string TrangThaiThanhToan { get; set; }

        [StringLength(50)]
        public string PhuongThucThanhToan { get; set; }

        [StringLength(100)]
        public string MaGiaoDich { get; set; }

        public DateTime? NgayGiaoDich { get; set; }

        public virtual DatLichPT DatLichPT { get; set; }

        public virtual HuanLuyenVien HuanLuyenVien { get; set; }

        public virtual Users Users { get; set; }
    }
}
