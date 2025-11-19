using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace HealthApp.Models
{
    public partial class WF_HealthTracker : DbContext
    {
        public WF_HealthTracker()
            : base("name=WF_HealthTracker1")
        {
        }

        public virtual DbSet<BaiTapChiTiet> BaiTapChiTiet { get; set; }
        public virtual DbSet<BanBe> BanBe { get; set; }
        public virtual DbSet<BuaAnChiTiet> BuaAnChiTiet { get; set; }
        public virtual DbSet<BuoiTap> BuoiTap { get; set; }
        public virtual DbSet<ChiaSeThanhTuu> ChiaSeThanhTuu { get; set; }
        public virtual DbSet<DanhGiaPT> DanhGiaPT { get; set; }
        public virtual DbSet<DatLichPT> DatLichPT { get; set; }
        public virtual DbSet<GoiThanhVien> GoiThanhVien { get; set; }
        public virtual DbSet<GiaoDich> GiaoDich { get; set; }
        public virtual DbSet<HoSoBenhLi> HoSoBenhLi { get; set; }
        public virtual DbSet<HuanLuyenVien> HuanLuyenVien { get; set; }
        public virtual DbSet<KeHoachAnUong> KeHoachAnUong { get; set; }
        public virtual DbSet<KeHoachLuyenTap> KeHoachLuyenTap { get; set; }
        public virtual DbSet<LuotThichChiaSeThanhTuu> LuotThichChiaSeThanhTuu { get; set; }
        public virtual DbSet<MucTieu> MucTieu { get; set; }
        public virtual DbSet<TapTin> TapTin { get; set; }
        public virtual DbSet<TinhNangGoi> TinhNangGoi { get; set; }
        public virtual DbSet<TinhTrangTongQuan> TinhTrangTongQuan { get; set; }
        public virtual DbSet<ThanhTuu> ThanhTuu { get; set; }
        public virtual DbSet<ThongBao> ThongBao { get; set; }
        public virtual DbSet<ThuVienBaiTap> ThuVienBaiTap { get; set; }
        public virtual DbSet<ThuVienMonAn> ThuVienMonAn { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaiTapChiTiet>()
                .Property(e => e.BaiTapChiTietID)
                .IsUnicode(false);

            modelBuilder.Entity<BaiTapChiTiet>()
                .Property(e => e.BuoiTapID)
                .IsUnicode(false);

            modelBuilder.Entity<BaiTapChiTiet>()
                .Property(e => e.BaiTapID)
                .IsUnicode(false);

            modelBuilder.Entity<BanBe>()
                .Property(e => e.BanBeID)
                .IsUnicode(false);

            modelBuilder.Entity<BanBe>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<BanBe>()
                .Property(e => e.NguoiNhanID)
                .IsUnicode(false);

            modelBuilder.Entity<BuaAnChiTiet>()
                .Property(e => e.BuaAnID)
                .IsUnicode(false);

            modelBuilder.Entity<BuaAnChiTiet>()
                .Property(e => e.KeHoachAnID)
                .IsUnicode(false);

            modelBuilder.Entity<BuaAnChiTiet>()
                .Property(e => e.MonAnID)
                .IsUnicode(false);

            modelBuilder.Entity<BuoiTap>()
                .Property(e => e.BuoiTapID)
                .IsUnicode(false);

            modelBuilder.Entity<BuoiTap>()
                .Property(e => e.KeHoachTapID)
                .IsUnicode(false);

            modelBuilder.Entity<BuoiTap>()
                .Property(e => e.ThuNgay)
                .IsUnicode(false);

            modelBuilder.Entity<BuoiTap>()
                .Property(e => e.ThoiGianNgoaiLe)
                .IsUnicode(false);

            modelBuilder.Entity<ChiaSeThanhTuu>()
                .Property(e => e.ChiaSeID)
                .IsUnicode(false);

            modelBuilder.Entity<ChiaSeThanhTuu>()
                .Property(e => e.ThanhTuuID)
                .IsUnicode(false);

            modelBuilder.Entity<ChiaSeThanhTuu>()
                .Property(e => e.NguoiChiaSe)
                .IsUnicode(false);

            modelBuilder.Entity<DanhGiaPT>()
                .Property(e => e.DanhGiaID)
                .IsUnicode(false);

            modelBuilder.Entity<DanhGiaPT>()
                .Property(e => e.DatLichID)
                .IsUnicode(false);

            modelBuilder.Entity<DanhGiaPT>()
                .Property(e => e.KhachHangID)
                .IsUnicode(false);

            modelBuilder.Entity<DanhGiaPT>()
                .Property(e => e.PTID)
                .IsUnicode(false);

            modelBuilder.Entity<DatLichPT>()
                .Property(e => e.DatLichID)
                .IsUnicode(false);

            modelBuilder.Entity<DatLichPT>()
                .Property(e => e.KhachHangID)
                .IsUnicode(false);

            modelBuilder.Entity<DatLichPT>()
                .Property(e => e.PTID)
                .IsUnicode(false);

            modelBuilder.Entity<DatLichPT>()
                .Property(e => e.NguoiHuy)
                .IsUnicode(false);

            modelBuilder.Entity<GoiThanhVien>()
                .Property(e => e.GoiThanhVienID)
                .IsUnicode(false);

            modelBuilder.Entity<GoiThanhVien>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<GiaoDich>()
                .Property(e => e.GiaoDichID)
                .IsUnicode(false);

            modelBuilder.Entity<GiaoDich>()
                .Property(e => e.DatLichID)
                .IsUnicode(false);

            modelBuilder.Entity<GiaoDich>()
                .Property(e => e.KhachHangID)
                .IsUnicode(false);

            modelBuilder.Entity<GiaoDich>()
                .Property(e => e.PTID)
                .IsUnicode(false);

            modelBuilder.Entity<HoSoBenhLi>()
                .Property(e => e.BenhID)
                .IsUnicode(false);

            modelBuilder.Entity<HuanLuyenVien>()
                .Property(e => e.PTID)
                .IsUnicode(false);

            modelBuilder.Entity<HuanLuyenVien>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<HuanLuyenVien>()
                .HasMany(e => e.DanhGiaPT)
                .WithRequired(e => e.HuanLuyenVien)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HuanLuyenVien>()
                .HasMany(e => e.GiaoDich)
                .WithRequired(e => e.HuanLuyenVien)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KeHoachAnUong>()
                .Property(e => e.KeHoachAnID)
                .IsUnicode(false);

            modelBuilder.Entity<KeHoachAnUong>()
                .Property(e => e.MucTieuID)
                .IsUnicode(false);

            modelBuilder.Entity<KeHoachLuyenTap>()
                .Property(e => e.KeHoachTapID)
                .IsUnicode(false);

            modelBuilder.Entity<KeHoachLuyenTap>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<KeHoachLuyenTap>()
                .Property(e => e.MucTieuID)
                .IsUnicode(false);

            modelBuilder.Entity<LuotThichChiaSeThanhTuu>()
                .Property(e => e.ThichID)
                .IsUnicode(false);

            modelBuilder.Entity<LuotThichChiaSeThanhTuu>()
                .Property(e => e.ChiaSeID)
                .IsUnicode(false);

            modelBuilder.Entity<LuotThichChiaSeThanhTuu>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<MucTieu>()
                .Property(e => e.MucTieuID)
                .IsUnicode(false);

            modelBuilder.Entity<MucTieu>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<MucTieu>()
                .Property(e => e.PTID)
                .IsUnicode(false);

            modelBuilder.Entity<MucTieu>()
                .HasMany(e => e.KeHoachAnUong)
                .WithOptional(e => e.MucTieu)
                .WillCascadeOnDelete();

            modelBuilder.Entity<TapTin>()
                .Property(e => e.TapTinID)
                .IsUnicode(false);

            modelBuilder.Entity<TapTin>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<TinhNangGoi>()
                .Property(e => e.TinhNangID)
                .IsUnicode(false);

            modelBuilder.Entity<TinhTrangTongQuan>()
                .Property(e => e.BanGhiID)
                .IsUnicode(false);

            modelBuilder.Entity<TinhTrangTongQuan>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<TinhTrangTongQuan>()
                .Property(e => e.BenhID)
                .IsUnicode(false);

            modelBuilder.Entity<ThanhTuu>()
                .Property(e => e.ThanhTuuID)
                .IsUnicode(false);

            modelBuilder.Entity<ThanhTuu>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<ThongBao>()
                .Property(e => e.ThongBaoID)
                .IsUnicode(false);

            modelBuilder.Entity<ThongBao>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<ThongBao>()
                .Property(e => e.MaLienQuan)
                .IsUnicode(false);

            modelBuilder.Entity<ThuVienBaiTap>()
                .Property(e => e.BaiTapID)
                .IsUnicode(false);

            modelBuilder.Entity<ThuVienBaiTap>()
                .Property(e => e.NguoiTao)
                .IsUnicode(false);

            modelBuilder.Entity<ThuVienBaiTap>()
                .HasMany(e => e.BaiTapChiTiet)
                .WithRequired(e => e.ThuVienBaiTap)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ThuVienMonAn>()
                .Property(e => e.MonAnID)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.BanBe)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.NguoiNhanID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.BanBe1)
                .WithRequired(e => e.Users1)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.ChiaSeThanhTuu)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.NguoiChiaSe)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DanhGiaPT)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.KhachHangID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DatLichPT)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.KhachHangID);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.GiaoDich)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.KhachHangID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.LuotThichChiaSeThanhTuu)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.ThuVienBaiTap)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.NguoiTao);
        }
    }
}
