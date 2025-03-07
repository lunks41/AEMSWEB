using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(CustomerId), nameof(CompanyId), nameof(EffectFrom))]
    public class M_CustomerCreditLimit
    {
        public Int32 CustomerId { get; set; }
        public Int16 CompanyId { get; set; }
        public DateTime EffectFrom { get; set; }
        public DateTime? EffectUntil { get; set; }
        public bool IsExpires { get; set; }
        public string Remarks { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CreditLimitAmt { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}