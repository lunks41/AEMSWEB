using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.AP
{
    public class ApOutstandTransactions
    {
        public Int16 CompanyId { get; set; }
        public Int16 TransactionId { get; set; }
        public Int64 DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public string? ReferenceNo { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DueDate { get; set; }
        public Int32 SupplierId { get; set; }
        public Int16 CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BalLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public string? Remarks { get; set; }

        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}