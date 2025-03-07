using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.GL
{
    [PrimaryKey(nameof(JournalId), nameof(ItemNo))]
    public class GLJournalDt
    {
        public Int64 JournalId { get; set; }
        public string JournalNo { get; set; }
        public Int16 ItemNo { get; set; }
        public Int16 SeqNo { get; set; }

        [ForeignKey(nameof(GLId))]
        public Int16 GLId { get; set; }

        public string Remarks { get; set; }
        public Int16 ProductId { get; set; }
        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public byte GstId { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal GstPercentage { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstCtyAmt { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Int16 DepartmentId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Int16 EmployeeId { get; set; }

        [ForeignKey(nameof(PortId))]
        public Int16 PortId { get; set; }

        [ForeignKey(nameof(VesselId))]
        public Int32 VesselId { get; set; }

        [ForeignKey(nameof(BargeId))]
        public Int16 BargeId { get; set; }

        [ForeignKey(nameof(VoyageId))]
        public Int16 VoyageId { get; set; }

        public byte EditVersion { get; set; }
    }
}