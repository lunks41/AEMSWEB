using System.ComponentModel.DataAnnotations;

namespace AMESWEB.Entities.Project
{
    public class Ser_PortExpenses
    {
        [Key]
        public long PortExpenseId { get; set; }

        public byte CompanyId { get; set; }
        public long JobOrderId { get; set; }
        public string JobOrderNo { get; set; } = string.Empty;
        public short TaskId { get; set; } = 1;
        public decimal Quantity { get; set; } = 1M;
        public int SupplierId { get; set; } = 0;
        public short ChargeId { get; set; }
        public short StatusId { get; set; }
        public short UomId { get; set; }
        public DateTime? DeliverDate { get; set; }
        public short GLId { get; set; }
        public long? DebitNoteId { get; set; }
        public string? DebitNoteNo { get; set; }
        public decimal TotAmt { get; set; } = 0M;
        public decimal GstAmt { get; set; } = 0M;
        public decimal TotAmtAftGst { get; set; } = 0M;
        public string Remarks { get; set; } = string.Empty;
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public byte EditVersion { get; set; } = 0;
    }
}