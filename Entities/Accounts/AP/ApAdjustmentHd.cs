using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.AP
{
    public class ApAdjustmentHd
    {
        [ForeignKey(nameof(CompanyId))]
        public Int16 CompanyId { get; set; }

        [Key]
        public Int64 AdjustmentId { get; set; }

        public string AdjustmentNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime TrnDate { get; set; }
        public DateTime AccountDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DueDate { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Int32 SupplierId { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Int16 CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal CtyExhRate { get; set; }

        [ForeignKey(nameof(CreditTermId))]
        public Int16 CreditTermId { get; set; }

        [ForeignKey(nameof(BankId))]
        public Int16 BankId { get; set; }

        public bool IsDebit { get; set; }

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

        [NotMapped]
        [Column(TypeName = "decimal(18,4)")]
        public decimal BalAmt { get; set; }

        [NotMapped]
        [Column(TypeName = "decimal(18,4)")]
        public decimal BalLocalAmt { get; set; }

        [NotMapped]
        [Column(TypeName = "decimal(18,4)")]
        public decimal PayAmt { get; set; }

        [NotMapped]
        [Column(TypeName = "decimal(18,4)")]
        public decimal PayLocalAmt { get; set; }

        [NotMapped]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ExGainLoss { get; set; }

        public Int64 PurchaseOrderId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public Int64 OperationId { get; set; }
        public string OperationNo { get; set; }
        public string Remarks { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PinCode { get; set; }
        public Int16 CountryId { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string ContactName { get; set; }
        public string MobileNo { get; set; }
        public string EmailAdd { get; set; }
        public string ModuleFrom { get; set; }
        public string CustomerName { get; set; }
        public string SuppAdjustmentNo { get; set; }
        public Int64 ArAdjustmentId { get; set; }
        public string ArAdjustmentNo { get; set; }
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