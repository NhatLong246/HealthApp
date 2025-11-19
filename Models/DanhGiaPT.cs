namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DanhGiaPT")]
    public partial class DanhGiaPT
    {
        [Key]
        [StringLength(20)]
        public string DanhGiaID { get; set; }

        [Required]
        [StringLength(20)]
        public string DatLichID { get; set; }

        [Required]
        [StringLength(20)]
        public string KhachHangID { get; set; }

        [Required]
        [StringLength(20)]
        public string PTID { get; set; }

        public int Diem { get; set; }

        [StringLength(500)]
        public string BinhLuan { get; set; }

        public DateTime? NgayDanhGia { get; set; }

        public virtual DatLichPT DatLichPT { get; set; }

        public virtual HuanLuyenVien HuanLuyenVien { get; set; }

        public virtual Users Users { get; set; }
    }
}
