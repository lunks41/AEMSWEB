using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(TaxId), nameof(CompanyId), nameof(ValidFrom))]
    public class M_TaxDt
    {
        public Int16 TaxId { get; set; }
        public Int16 CompanyId { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal TaxPercentage { get; set; }

        public DateTime ValidFrom { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}