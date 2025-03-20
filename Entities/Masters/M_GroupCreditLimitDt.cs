using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(GroupCreditLimitId), nameof(CompanyId), nameof(EffectFrom))]
    public class M_GroupCreditLimitDt
    {
        public Int16 CompanyId { get; set; }
        public Int16 GroupCreditLimitId { get; set; }
        public DateOnly EffectFrom { get; set; }
        public DateOnly EffectUntil { get; set; }
        public bool IsExpires { get; set; }
        public string? Remarks { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CreditLimitAmt { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}