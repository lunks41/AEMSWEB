using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_Tax
    {
        [Key]
        public Int16 TaxId { get; set; }

        public Int16 CompanyId { get; set; }

        [ForeignKey(nameof(TaxCategoryId))]
        public Int16 TaxCategoryId { get; set; }

        public string? TaxCode { get; set; }
        public string? TaxName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}