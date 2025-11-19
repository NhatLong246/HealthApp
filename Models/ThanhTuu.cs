namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThanhTuu")]
    public partial class ThanhTuu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ThanhTuu()
        {
            ChiaSeThanhTuu = new HashSet<ChiaSeThanhTuu>();
        }

        [StringLength(20)]
        public string ThanhTuuID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiThanhTuu { get; set; }

        [Required]
        [StringLength(100)]
        public string TenThanhTuu { get; set; }

        public int? Diem { get; set; }

        public DateTime? NgayDatDuoc { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        [StringLength(200)]
        public string BieuTuong { get; set; }

        public int? CapDo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiaSeThanhTuu> ChiaSeThanhTuu { get; set; }

        public virtual Users Users { get; set; }
    }
}
