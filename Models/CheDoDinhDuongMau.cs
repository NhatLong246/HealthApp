using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    [Table("CheDoDinhDuongMau")]
    public class CheDoDinhDuongMau
    {
        [Key]
        [StringLength(20)]
        public string CheDoID { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiMucTieu { get; set; }

        public int Calo { get; set; }

        public int Protein { get; set; }

        public int Carbs { get; set; }

        public int Fat { get; set; }

        public int Fiber { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }
    }
}

