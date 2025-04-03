using AMESWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Areas.Account.Models.AR
{
    public class ARRefundViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;
        private DateTime? _chequeDate;

        public short CompanyId { get; set; }
        public string? RefundId { get; set; }
        public string? RefundNo { get; set; }
        public string? ReferenceNo { get; set; }

        public string? TrnDate
        {
            get { return DateHelperStatic.FormatDate(_trnDate); }
            set { _trnDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string? AccountDate
        {
            get { return DateHelperStatic.FormatDate(_accountDate); }
            set { _accountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public short BankId { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
        public short PaymentTypeId { get; set; }
        public string? PaymentTypeCode { get; set; }
        public string? PaymentTypeName { get; set; }
        public string? ChequeNo { get; set; }

        public string? ChequeDate
        {
            get { return _chequeDate.HasValue ? DateHelperStatic.FormatDate(_chequeDate.Value) : ""; }
            set { _chequeDate = string.IsNullOrEmpty(value) ? null : DateHelperStatic.ParseDBDate(value); }
        }

        public int CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public short CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        public short RecCurrencyId { get; set; }
        public string? RecCurrencyCode { get; set; }
        public string? RecCurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal RecExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal RecTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal RecTotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public int BankChargeGLId { get; set; }
        public string? BankChargeGLCode { get; set; }
        public string? BankChargeGLName { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankChargesAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BankChargesLocalAmt { get; set; }

        public string? Remarks { get; set; }
        public string? ModuleFrom { get; set; }
        public string? CreateBy { get; set; }
        public short CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public short? EditById { get; set; }
        public string? EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public short? CancelById { get; set; }
        public string? CancelBy { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelRemarks { get; set; }
        public byte EditVersion { get; set; }
        public List<ARRefundDtViewModel> data_details { get; set; }
    }
}