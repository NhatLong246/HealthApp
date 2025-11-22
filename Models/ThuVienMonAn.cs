namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DinhDuongMonAn")]
    public partial class ThuVienMonAn
    {
        [Key]
        [StringLength(20)]
        [Column("MonAnID")]
        public string MonAnID { get; set; }

        [StringLength(200)]
        [Column("HinhAnh")]
        public string imageURL { get; set; }

        [StringLength(200)]
        [Column("TenMonAn")]
        public string TenMonAn { get; set; }

        [StringLength(20)]
        [Column("DonViTinh")]
        public string Donvi { get; set; }

        [Column("LuongCalo")]
        public double? Calories { get; set; }

        [Column("Protein")]
        public double? Protein { get; set; }

        [Column("ChatBeo")]
        public double? Fat { get; set; }

        [Column("Carbohydrate")]
        public double? Carbs { get; set; }
    }
}
