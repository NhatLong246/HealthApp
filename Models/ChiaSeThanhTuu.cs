namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChiaSeThanhTuu")]
    public partial class ChiaSeThanhTuu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChiaSeThanhTuu()
        {
            LuotThichChiaSeThanhTuu = new HashSet<LuotThichChiaSeThanhTuu>();
        }

        [Key]
        [StringLength(20)]
        public string ChiaSeID { get; set; }

        [Required]
        [StringLength(20)]
        public string ThanhTuuID { get; set; }

        [Required]
        [StringLength(20)]
        public string NguoiChiaSe { get; set; }

        public DateTime? NgayChiaSe { get; set; }

        [StringLength(20)]
        public string DoiTuongXem { get; set; }

        [StringLength(500)]
        public string ChuThich { get; set; }

        public int? SoLuongThich { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public virtual ThanhTuu ThanhTuu { get; set; }

        public virtual Users Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LuotThichChiaSeThanhTuu> LuotThichChiaSeThanhTuu { get; set; }
    }
}
