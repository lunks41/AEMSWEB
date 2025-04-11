using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Project
{
    public class Ser_AgencyRemuneration
    {
        public byte CompanyId { get; set; }

        [Key]
        public long AgencyRemunerationId { get; set; }

        public DateTime RemunerationDate { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; }
        public short TaskId { get; set; }
        public short GLId { get; set; }
        public short ChargeId { get; set; }
        public long? DebitNoteId { get; set; }
        public string DebitNoteNo { get; set; }
        public decimal TotAmt { get; set; } = 0m;
        public decimal GstAmt { get; set; } = 0m;
        public decimal TotAmtAftGst { get; set; } = 0m;
        public decimal Amount { get; set; } = 0m;
        public string AgencyName { get; set; }
        public int? Qty { get; set; }
        public DateTime CrewHandlingDateGmt { get; set; }
        public short? UomId { get; set; }
        public short StatusId { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public short CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public byte EditVersion { get; set; }
    }
}