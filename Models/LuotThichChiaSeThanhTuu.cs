namespace HealthApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LuotThichChiaSeThanhTuu")]
    public partial class LuotThichChiaSeThanhTuu
    {
        [Key]
        [StringLength(20)]
        public string ThichID { get; set; }

        [Required]
        [StringLength(20)]
        public string ChiaSeID { get; set; }

        [Required]
        [StringLength(20)]
        public string UserID { get; set; }

        public DateTime? NgayThich { get; set; }

        public virtual ChiaSeThanhTuu ChiaSeThanhTuu { get; set; }

        public virtual Users Users { get; set; }
    }
}
