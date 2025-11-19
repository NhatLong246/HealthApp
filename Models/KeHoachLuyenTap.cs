namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KeHoachLuyenTap")]
    public partial class KeHoachLuyenTap
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KeHoachLuyenTap()
        {
            BuoiTap = new HashSet<BuoiTap>();
        }

        [Key]
        [StringLength(20)]
        public string KeHoachTapID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [StringLength(20)]
        public string MucTieuID { get; set; }

        public double? TongCalories { get; set; }

        [StringLength(50)]
        public string CapDo { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuoiTap> BuoiTap { get; set; }

        public virtual MucTieu MucTieu { get; set; }

        public virtual Users Users { get; set; }
    }
}
