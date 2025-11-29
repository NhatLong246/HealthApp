namespace HealthApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GiaoBaiTapChoUser")]
    public partial class GiaoBaiTapChoUser
    {
        [Key]
        [StringLength(20)]
        public string GiaoBaiTapID { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey(nameof(HuanLuyenVien))]
        public string PTID { get; set; }

        [Required]
        [StringLength(20)]
        [ForeignKey(nameof(Users))]
        public string UserID { get; set; }

        [StringLength(20)]
        [ForeignKey(nameof(DatLichPT))]
        public string DatLichID { get; set; }

        [StringLength(20)]
        [ForeignKey(nameof(ThuVienBaiTap))]
        public string ThuVienBaiTapID { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; }

        [StringLength(1000)]
        public string MoTa { get; set; }

        [StringLength(200)]
        public string MucTieuBuoiTap { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        public DateTime? NgayGiao { get; set; }

        public DateTime? HanHoanThanh { get; set; }

        public DateTime? NgayHoanThanh { get; set; }

        [StringLength(500)]
        public string GhiChuPT { get; set; }

        [StringLength(500)]
        public string PhanHoiUser { get; set; }

        public virtual DatLichPT DatLichPT { get; set; }

        public virtual HuanLuyenVien HuanLuyenVien { get; set; }

        public virtual ThuVienBaiTap ThuVienBaiTap { get; set; }

        public virtual Users Users { get; set; }
    }
}

