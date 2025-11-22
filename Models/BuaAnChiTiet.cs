namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Sử dụng NhatKyDinhDuong thay vì BuaAnChiTiet vì table BuaAnChiTiet không tồn tại trong database
    [Table("NhatKyDinhDuong")]
    public partial class BuaAnChiTiet
    {
        [Key]
        [StringLength(20)]
        [Column("DinhDuongID")]
        public string BuaAnID { get; set; }

        // Không có KeHoachAnID trong NhatKyDinhDuong, dùng UserID thay thế (tạm thời dùng "default_user")
        [Required]
        [StringLength(20)]
        [Column("UserID")]
        public string KeHoachAnID { get; set; }

        [Required]
        [StringLength(20)]
        [Column("MonAnID")]
        public string MonAnID { get; set; }

        // LoaiBuaAn lưu trong GhiChu (format: "LoaiBuaAn: Breakfast|GhiChu khác")
        [StringLength(50)]
        [NotMapped] // Không map vào database, lưu trong GhiChu
        public string LoaiBuaAn { get; set; }

        [Required]
        [Column("NgayGhiLog", TypeName = "date")]
        public DateTime NgayAn { get; set; }

        // TenMonAn không có trong NhatKyDinhDuong, lấy từ ThuVienMonAn khi load
        [StringLength(200)]
        [NotMapped]
        public string TenMonAn { get; set; }

        // Donvi không có trong NhatKyDinhDuong, lấy từ ThuVienMonAn khi load
        [StringLength(10)]
        [NotMapped]
        public string Donvi { get; set; }

        [Column("LuongThucAn")]
        public double? KhoiLuongChuan { get; set; }

        // Calories, Protein, Carbs, Fat không có trong NhatKyDinhDuong
        // Tính toán từ ThuVienMonAn và LuongThucAn khi load
        [NotMapped]
        public double? Calories { get; set; }

        [NotMapped]
        public double? Protein { get; set; }

        [NotMapped]
        public double? Carbs { get; set; }

        [NotMapped]
        public double? Fat { get; set; }

        [NotMapped]
        public double? Fiber { get; set; }

        [StringLength(500)]
        [Column("GhiChu")]
        public string GhiChu { get; set; }

        // NgayCapNhat không có trong NhatKyDinhDuong
        [NotMapped]
        public DateTime? NgayCapNhat { get; set; }

        // Navigation properties
        public virtual Users Users { get; set; }
        public virtual ThuVienMonAn ThuVienMonAn { get; set; }
    }
}
