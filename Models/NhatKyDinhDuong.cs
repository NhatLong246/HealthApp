namespace HealthApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NhatKyDinhDuong")]
    public partial class NhatKyDinhDuong
    {
        [Key]
        [StringLength(20)]
        [Column("DinhDuongID")]
        public string DinhDuongID { get; set; }

        [Required]
        [StringLength(20)]
        [Column("UserID")]
        public string UserID { get; set; }

        [Required]
        [Column("NgayGhiLog", TypeName = "date")]
        public DateTime NgayGhiLog { get; set; }

        [Required]
        [StringLength(20)]
        [Column("MonAnID")]
        public string MonAnID { get; set; }

        [Column("LuongThucAn")]
        public double? LuongThucAn { get; set; }

        [StringLength(500)]
        [Column("GhiChu")]
        public string GhiChu { get; set; }

        // Navigation properties
        public virtual Users Users { get; set; }
        public virtual ThuVienMonAn ThuVienMonAn { get; set; }
    }
}

