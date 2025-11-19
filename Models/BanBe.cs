namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BanBe")]
    public partial class BanBe
    {
        [StringLength(20)]
        public string BanBeID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [Required]
        [StringLength(20)]
        public string NguoiNhanID { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; }

        public DateTime? NgayGui { get; set; }

        public DateTime? NgayChapNhan { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public virtual Users Users { get; set; }

        public virtual Users Users1 { get; set; }
    }
}
