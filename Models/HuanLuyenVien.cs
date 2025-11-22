namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HuanLuyenVien")]
    public partial class HuanLuyenVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HuanLuyenVien()
        {
            DanhGiaPT = new HashSet<DanhGiaPT>();
            DatLichPT = new HashSet<DatLichPT>();
            GiaoDich = new HashSet<GiaoDich>();
        }

        [Key]
        [StringLength(20)]
        public string PTID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [StringLength(500)]
        public string ChungChi { get; set; }

        [StringLength(200)]
        public string ChuyenMon { get; set; }

        public int? SoNamKinhNghiem { get; set; }

        [StringLength(50)]
        public string ThanhPho { get; set; }

        public double? GiaTheoGio { get; set; }

        [StringLength(1000)]
        public string TieuSu { get; set; }

        [StringLength(255)]
        public string AnhDaiDien { get; set; }

        [StringLength(255)]
        public string AnhCCCD { get; set; }

        [StringLength(255)]
        public string AnhChanDung { get; set; }

        [StringLength(255)]
        public string FileTaiLieu { get; set; }

        public bool? DaXacMinh { get; set; }

        [StringLength(500)]
        public string GioRanh { get; set; }

        public int? SoKhachHienTai { get; set; }

        public bool? NhanKhach { get; set; }

        public int? TongDanhGia { get; set; }

        public double? DiemTrungBinh { get; set; }

        public double? TiLeThanhCong { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhGiaPT> DanhGiaPT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatLichPT> DatLichPT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoDich> GiaoDich { get; set; }

        public virtual Users Users { get; set; }
    }
}
