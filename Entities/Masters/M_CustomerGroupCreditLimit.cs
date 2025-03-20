using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    public class M_CustomerGroupCreditLimit
    {
        public Int16 CompanyId { get; set; }

        [Key]
        public Int16 GroupCreditLimitId { get; set; }

        public string? GroupCreditLimitCode { get; set; }
        public string? GroupCreditLimitName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}