namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BuoiTap")]
    public partial class BuoiTap
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BuoiTap()
        {
            BaiTapChiTiet = new HashSet<BaiTapChiTiet>();
        }

        [StringLength(20)]
        public string BuoiTapID { get; set; }

        [Required]
        [StringLength(20)]
        public string KeHoachTapID { get; set; }

        [StringLength(50)]
        public string ThuNgay { get; set; }

        [StringLength(1000)]
        public string ThoiGianNgoaiLe { get; set; }

        public DateTime? ThoiGianBatDau { get; set; }

        public DateTime? ThoiGianKetThuc { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        public double? Calories { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public DateTime? NgayThucHien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaiTapChiTiet> BaiTapChiTiet { get; set; }

        public virtual KeHoachLuyenTap KeHoachLuyenTap { get; set; }
    }
}
