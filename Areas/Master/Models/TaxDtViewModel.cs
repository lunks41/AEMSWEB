using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Models.Masters
{
    public class TaxDtViewModel
    {
        public Int16 TaxId { get; set; }
        public string TaxCode { get; set; }
        public string TaxName { get; set; }
        public Int16 CompanyId { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal TaxPercentage { get; set; }

        public DateTime ValidFrom { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}