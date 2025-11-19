namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HoSoBenhLi")]
    public partial class HoSoBenhLi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HoSoBenhLi()
        {
            TinhTrangTongQuan = new HashSet<TinhTrangTongQuan>();
        }

        [Key]
        [StringLength(20)]
        public string BenhID { get; set; }

        [StringLength(200)]
        public string TenBenh { get; set; }

        [StringLength(200)]
        public string LoaiBenh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TinhTrangTongQuan> TinhTrangTongQuan { get; set; }
    }
}
