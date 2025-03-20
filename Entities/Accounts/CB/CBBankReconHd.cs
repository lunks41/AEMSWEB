using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.CB
{
    public class CBBankReconHd
    {
        [ForeignKey(nameof(CompanyId))]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int64 ReconId { get; set; }

        public string? ReconNo { get; set; }
        public Int64 PrevReconId { get; set; }

        public string? PrevReconNo { get; set; }
        public string? ReferenceNo { get; set; }

        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }

        [ForeignKey(nameof(BankId))]
        public Int16 BankId { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Int16 CurrencyId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Remarks { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal OPBalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CLBalAmt { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int16? CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
    }
}