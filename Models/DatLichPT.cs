namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DatLichPT")]
    public partial class DatLichPT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatLichPT()
        {
            DanhGiaPT = new HashSet<DanhGiaPT>();
            GiaoDich = new HashSet<GiaoDich>();
        }

        [Key]
        [StringLength(20)]
        public string DatLichID { get; set; }

        [Required]
        [StringLength(20)]
        public string KhachHangID { get; set; }

        [StringLength(20)]
        public string PTID { get; set; }

        public DateTime NgayGioDat { get; set; }

        public int? ThoiLuong { get; set; }

        [StringLength(50)]
        public string LoaiBuoiTap { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        [StringLength(500)]
        public string LyDoTuChoi { get; set; }

        [StringLength(20)]
        public string NguoiHuy { get; set; }

        public double? TienHoan { get; set; }

        public bool? ChoXemSucKhoe { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhGiaPT> DanhGiaPT { get; set; }

        public virtual HuanLuyenVien HuanLuyenVien { get; set; }

        public virtual Users Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoDich> GiaoDich { get; set; }
    }
}
