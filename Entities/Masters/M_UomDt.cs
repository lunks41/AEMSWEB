using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(UomId), nameof(PackUomId))]
    public class M_UomDt
    {
        public Int16 CompanyId { get; set; }
        public Int16 UomId { get; set; }
        public Int16 PackUomId { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal UomFactor { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}