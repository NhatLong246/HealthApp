namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            BanBe = new HashSet<BanBe>();
            BanBe1 = new HashSet<BanBe>();
            ChiaSeThanhTuu = new HashSet<ChiaSeThanhTuu>();
            DanhGiaPT = new HashSet<DanhGiaPT>();
            DatLichPT = new HashSet<DatLichPT>();
            GoiThanhVien = new HashSet<GoiThanhVien>();
            GiaoDich = new HashSet<GiaoDich>();
            HuanLuyenVien = new HashSet<HuanLuyenVien>();
            KeHoachLuyenTap = new HashSet<KeHoachLuyenTap>();
            LuotThichChiaSeThanhTuu = new HashSet<LuotThichChiaSeThanhTuu>();
            MucTieu = new HashSet<MucTieu>();
            TapTin = new HashSet<TapTin>();
            TinhTrangTongQuan = new HashSet<TinhTrangTongQuan>();
            ThanhTuu = new HashSet<ThanhTuu>();
            ThongBao = new HashSet<ThongBao>();
            ThuVienBaiTap = new HashSet<ThuVienBaiTap>();
        }

        [Key]
        [StringLength(20)]
        public string UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; }

        [StringLength(20)]
        public string Role { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string SDT { get; set; }

        [StringLength(100)]
        public string HoTen { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string GioiTinh { get; set; }

        [StringLength(200)]
        public string AnhDaiDien { get; set; }

        [StringLength(10)]
        public string Theme { get; set; }

        [StringLength(10)]
        public string NgonNgu { get; set; }

        [StringLength(50)]
        public string TimeZone { get; set; }

        [StringLength(256)]
        public string ResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }

        public DateTime? CreatedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BanBe> BanBe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BanBe> BanBe1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiaSeThanhTuu> ChiaSeThanhTuu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhGiaPT> DanhGiaPT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatLichPT> DatLichPT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GoiThanhVien> GoiThanhVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoDich> GiaoDich { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HuanLuyenVien> HuanLuyenVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KeHoachLuyenTap> KeHoachLuyenTap { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LuotThichChiaSeThanhTuu> LuotThichChiaSeThanhTuu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MucTieu> MucTieu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TapTin> TapTin { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TinhTrangTongQuan> TinhTrangTongQuan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanhTuu> ThanhTuu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThongBao> ThongBao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThuVienBaiTap> ThuVienBaiTap { get; set; }
    }
}
