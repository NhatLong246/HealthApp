namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TapTin")]
    public partial class TapTin
    {
        [StringLength(20)]
        public string TapTinID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        [Required]
        [StringLength(255)]
        public string TenTapTin { get; set; }

        [Required]
        [StringLength(255)]
        public string TenLuuTrenServer { get; set; }

        [Required]
        [StringLength(500)]
        public string DuongDan { get; set; }

        public long? KichThuoc { get; set; }

        [StringLength(100)]
        public string MimeType { get; set; }

        [StringLength(50)]
        public string LoaiFile { get; set; }

        [StringLength(50)]
        public string MucDich { get; set; }

        public DateTime? NgayUpload { get; set; }

        public bool? DaXoa { get; set; }

        public DateTime? NgayXoa { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public virtual Users Users { get; set; }
    }
}
