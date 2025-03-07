using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Models.Masters
{
    public class CustomerCreditLimitViewModel
    {
        public Int32 CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public Int16 CompanyId { get; set; }
        public DateTime EffectFrom { get; set; }
        public DateTime EffectUntil { get; set; }
        public bool IsExpires { get; set; }
        public string Remarks { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CreditLimitAmt { get; set; }

        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}