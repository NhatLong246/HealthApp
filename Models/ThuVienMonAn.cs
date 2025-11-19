namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThuVienMonAn")]
    public partial class ThuVienMonAn
    {
        [Key]
        [StringLength(20)]
        public string MonAnID { get; set; }

        [StringLength(500)]
        public string imageURL { get; set; }

        [Required]
        [StringLength(200)]
        public string TenMonAn { get; set; }

        [StringLength(100)]
        public string Loai { get; set; }

        [StringLength(10)]
        public string Donvi { get; set; }

        public double? KhoiLuongChuan { get; set; }

        public double? Calories { get; set; }

        public double? Protein { get; set; }

        public double? Carbs { get; set; }

        public double? Fat { get; set; }

        public double? Fiber { get; set; }

        public DateTime? NgayTao { get; set; }
    }
}
