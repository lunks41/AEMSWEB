using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.CB
{
    [PrimaryKey(nameof(ReceiptId))]
    public class CBGenReceiptHd
    {
        [ForeignKey(nameof(CompanyId))]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int64 ReceiptId { get; set; }

        public string ReceiptNo { get; set; }
        public string ReferenceNo { get; set; }

        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Int16 CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal CtyExhRate { get; set; }

        public Int16 PaymentTypeId { get; set; }

        [ForeignKey(nameof(BankId))]
        public Int16 BankId { get; set; }

        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankChgAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankChgLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public DateTime GstClaimDate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstCtyAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmtAftGst { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmtAftGst { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmtAftGst { get; set; }

        public string PayeeTo { get; set; }

        public string Remarks { get; set; }
        public string ModuleFrom { get; set; }

        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public Int16? CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
    }
}