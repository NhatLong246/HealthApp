namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("KeHoachAnUong")]
    public partial class KeHoachAnUong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KeHoachAnUong()
        {
            BuaAnChiTiet = new HashSet<BuaAnChiTiet>();
        }

        [Key]
        [StringLength(20)]
        public string KeHoachAnID { get; set; }

        [StringLength(20)]
        public string MucTieuID { get; set; }

        public double? TongCalories { get; set; }

        public double? TongProtein { get; set; }

        public double? TongCarbs { get; set; }

        public double? TongFat { get; set; }

        public double? Fiber { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BuaAnChiTiet> BuaAnChiTiet { get; set; }

        public virtual MucTieu MucTieu { get; set; }
    }
}
