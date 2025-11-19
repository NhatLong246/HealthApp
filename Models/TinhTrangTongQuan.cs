namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TinhTrangTongQuan")]
    public partial class TinhTrangTongQuan
    {
        [Key]
        [StringLength(20)]
        public string BanGhiID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayGhiNhan { get; set; }

        public double? CanNang { get; set; }

        public double? ChieuCao { get; set; }

        public double? BMI { get; set; }

        public double? SoDoVong1 { get; set; }

        public double? SoDoVong2 { get; set; }

        public double? SoDoVong3 { get; set; }

        public double? SoDoBapTay { get; set; }

        public double? SoDoBapChan { get; set; }

        [StringLength(100)]
        public string TheTrang { get; set; }

        [StringLength(20)]
        public string BenhID { get; set; }

        [StringLength(200)]
        public string TrinhDoCaNhan { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public virtual HoSoBenhLi HoSoBenhLi { get; set; }

        public virtual Users Users { get; set; }
    }
}
