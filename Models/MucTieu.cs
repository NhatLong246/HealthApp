namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MucTieu")]
    public partial class MucTieu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MucTieu()
        {
            KeHoachAnUong = new HashSet<KeHoachAnUong>();
            KeHoachLuyenTap = new HashSet<KeHoachLuyenTap>();
        }

        [StringLength(20)]
        public string MucTieuID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiMucTieu { get; set; }

        [StringLength(200)]
        public string TenMucTieu { get; set; }

        public double? GiaTriMucTieu { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayBatDau { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayKetThucDuKien { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayKetThucThucTe { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [StringLength(20)]
        public string PTID { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime? NgayTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KeHoachAnUong> KeHoachAnUong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KeHoachLuyenTap> KeHoachLuyenTap { get; set; }

        public virtual Users Users { get; set; }
    }
}
