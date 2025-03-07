using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Areas.Account.Models.CB
{
    public class CBGenReceiptDtViewModel
    {
        public string ReceiptId { get; set; }
        public string ReceiptNo { get; set; }
        public short ItemNo { get; set; }
        public short SeqNo { get; set; }
        public short GLId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public string Remarks { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public byte GstId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstPercentage { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstCtyAmt { get; set; }

        public short BargeId { get; set; }
        public short DepartmentId { get; set; }
        public short EmployeeId { get; set; }
        public short PortId { get; set; }
        public int VesselId { get; set; }
        public short VoyageId { get; set; }
        public byte EditVersion { get; set; }
    }
}