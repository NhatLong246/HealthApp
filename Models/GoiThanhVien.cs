namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GoiThanhVien")]
    public partial class GoiThanhVien
    {
        [StringLength(20)]
        public string GoiThanhVienID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [Required]
        [StringLength(20)]
        public string LoaiGoi { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayBatDau { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayKetThuc { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        public double? SoTien { get; set; }

        [StringLength(20)]
        public string ChuKyThanhToan { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayGiaHan { get; set; }

        [StringLength(50)]
        public string PhuongThucThanhToan { get; set; }

        public bool? TuDongGiaHan { get; set; }

        public DateTime? NgayDangKy { get; set; }

        public DateTime? NgayHuy { get; set; }

        [StringLength(500)]
        public string LyDoHuy { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public virtual Users Users { get; set; }
    }
}
