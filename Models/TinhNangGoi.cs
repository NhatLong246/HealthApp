namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TinhNangGoi")]
    public partial class TinhNangGoi
    {
        [Key]
        [StringLength(20)]
        public string TinhNangID { get; set; }

        [Required]
        [StringLength(100)]
        public string TenTinhNang { get; set; }

        [StringLength(20)]
        public string GoiToiThieu { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        public bool? ConHoatDong { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }
    }
}
