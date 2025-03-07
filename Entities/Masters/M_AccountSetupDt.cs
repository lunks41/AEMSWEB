using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Masters
{
    [PrimaryKey(nameof(CompanyId), nameof(AccSetupId), nameof(CurrencyId), nameof(GLId))]
    public class M_AccountSetupDt
    {
        public Int16 CompanyId { get; set; }
        public Int16 AccSetupId { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int16 GLId { get; set; }
        public bool ApplyAllCurr { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}