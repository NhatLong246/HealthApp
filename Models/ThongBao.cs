namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThongBao")]
    public partial class ThongBao
    {
        [StringLength(20)]
        public string ThongBaoID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [Required]
        [StringLength(500)]
        public string NoiDung { get; set; }

        [StringLength(200)]
        public string TieuDe { get; set; }

        [StringLength(50)]
        public string Loai { get; set; }

        [StringLength(20)]
        public string MaLienQuan { get; set; }

        public bool? DaDoc { get; set; }

        public DateTime? NgayTao { get; set; }

        public virtual Users Users { get; set; }
    }
}
