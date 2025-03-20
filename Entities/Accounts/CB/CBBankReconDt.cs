using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.CB
{
    [PrimaryKey(nameof(ReconId), nameof(ItemNo))]
    public class CBBankReconDt
    {
        public Int64 ReconId { get; set; }
        public string? ReconNo { get; set; }
        public Int16 ItemNo { get; set; }
        public bool IsSel { get; set; }
        public byte ModuleId { get; set; }
        public byte TransactionId { get; set; }
        public Int64 DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public string? DocReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public Int16 PaymentTypeId { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public Int32 CustomerId { get; set; }
        public Int32 SupplierId { get; set; }

        [ForeignKey(nameof(GLId))]
        public Int16 GLId { get; set; }

        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        public string? PaymentFromTo { get; set; }
        public string? Remarks { get; set; }

        public byte EditVersion { get; set; }
    }
}