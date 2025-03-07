using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(CreditTermId), nameof(CompanyId), nameof(FromDay))]
    public class M_CreditTermDt
    {
        public Int16 CreditTermId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 FromDay { get; set; }
        public Int16 ToDay { get; set; }
        public bool IsEndOfMonth { get; set; }
        public Int16 DueDay { get; set; }
        public Int16 NoMonth { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}