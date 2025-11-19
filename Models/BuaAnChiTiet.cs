namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BuaAnChiTiet")]
    public partial class BuaAnChiTiet
    {
        [Key]
        [StringLength(20)]
        public string BuaAnID { get; set; }

        [Required]
        [StringLength(20)]
        public string KeHoachAnID { get; set; }

        [Required]
        [StringLength(20)]
        public string MonAnID { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiBuaAn { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayAn { get; set; }

        [Required]
        [StringLength(200)]
        public string TenMonAn { get; set; }

        [StringLength(10)]
        public string Donvi { get; set; }

        public double? KhoiLuongChuan { get; set; }

        public double? Calories { get; set; }

        public double? Protein { get; set; }

        public double? Carbs { get; set; }

        public double? Fat { get; set; }

        public double? Fiber { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public virtual KeHoachAnUong KeHoachAnUong { get; set; }
    }
}
