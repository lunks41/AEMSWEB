using AEMSWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Areas.Account.Models.AP
{
    public class APPaymentViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;
        private DateTime? _chequeDate;

        public short CompanyId { get; set; }
        public string PaymentId { get; set; }
        public string PaymentNo { get; set; }
        public string ReferenceNo { get; set; }

        public string TrnDate
        {
            get { return DateHelperStatic.FormatDate(_trnDate); }
            set { _trnDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string AccountDate
        {
            get { return DateHelperStatic.FormatDate(_accountDate); }
            set { _accountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public short BankId { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public short PaymentTypeId { get; set; }
        public string PaymentTypeCode { get; set; }
        public string PaymentTypeName { get; set; }
        public string ChequeNo { get; set; }

        public string ChequeDate
        {
            get { return _chequeDate.HasValue ? DateHelperStatic.FormatDate(_chequeDate.Value) : ""; }
            set { _chequeDate = string.IsNullOrEmpty(value) ? null : DateHelperStatic.ParseDBDate(value); }
        }

        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public short CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal UnAllocTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal UnAllocTotLocalAmt { get; set; }

        public short RecCurrencyId { get; set; }
        public string RecCurrencyCode { get; set; }
        public string RecCurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal RecExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal RecTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal RecTotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocTotLocalAmt { get; set; }

        public int BankChargeGLId { get; set; }
        public string BankChargeGLCode { get; set; }
        public string BankChargeGLName { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankChargesAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankChargesLocalAmt { get; set; }

        public string Remarks { get; set; }
        public string ModuleFrom { get; set; }
        public string CreateBy { get; set; }
        public short CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public short? EditById { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public short? CancelById { get; set; }
        public string CancelBy { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
        public List<APPaymentDtViewModel> data_details { get; set; }
    }
}