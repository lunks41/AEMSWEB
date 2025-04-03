using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Accounts.CB
{
    public class CBBankTransfer
    {
        [ForeignKey(nameof(CompanyId))]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int64 TransferId { get; set; }

        public string? TransferNo { get; set; }
        public string? ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public Int16 FromBankId { get; set; }
        public Int16 FromCurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal FromExhRate { get; set; }

        public Int16 PaymentTypeId { get; set; }
        public string? ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal FromBankChgAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal FromBankChgLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal FromTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal FromTotLocalAmt { get; set; }

        public Int16 ToBankId { get; set; }
        public Int16 ToCurrencyId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ToExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ToBankChgAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ToBankChgLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ToTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ToTotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal BankExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankTotLocalAmt { get; set; }

        public string? Remarks { get; set; }
        public string? PayeeTo { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public string? ModuleFrom { get; set; }

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