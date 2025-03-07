using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(CompanyId), nameof(GroupCreditLimitId), nameof(CustomerId))]
    public class M_GroupCreditLimit_Customer
    {
        public Int16 CompanyId { get; set; }
        public Int16 GroupCreditLimitId { get; set; }
        public Int32 CustomerId { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}