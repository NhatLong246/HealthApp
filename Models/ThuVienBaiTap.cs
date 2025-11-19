namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThuVienBaiTap")]
    public partial class ThuVienBaiTap
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ThuVienBaiTap()
        {
            BaiTapChiTiet = new HashSet<BaiTapChiTiet>();
        }

        [Key]
        [StringLength(20)]
        public string BaiTapID { get; set; }

        [Required]
        [StringLength(200)]
        public string TenBaiTap { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiMucTieu { get; set; }

        [Required]
        [StringLength(100)]
        public string NhomCoChinhNhat { get; set; }

        [StringLength(200)]
        public string NhomCoPhu { get; set; }

        [StringLength(50)]
        public string CapDo { get; set; }

        [StringLength(200)]
        public string DungCu { get; set; }

        [StringLength(1000)]
        public string MoTa { get; set; }

        [StringLength(2000)]
        public string HuongDan { get; set; }

        [StringLength(1000)]
        public string LuuY { get; set; }

        [StringLength(500)]
        public string AnhMinhHoa { get; set; }

        [StringLength(500)]
        public string VideoHuongDan { get; set; }

        public double? CaloriesMoiRep { get; set; }

        public int? ThoiLuongDeNghi { get; set; }

        [StringLength(50)]
        public string SoRep { get; set; }

        [StringLength(50)]
        public string SoSet { get; set; }

        public int? ThoiGianNghi { get; set; }

        public int? DoPhoBien { get; set; }

        [StringLength(20)]
        public string NguoiTao { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        [StringLength(100)]
        public string TheLoaiBenh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaiTapChiTiet> BaiTapChiTiet { get; set; }

        public virtual Users Users { get; set; }
    }
}
