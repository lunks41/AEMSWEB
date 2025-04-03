using AMESWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Areas.Account.Models.CB
{
    public class CBGenReceiptHdViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;
        private DateTime _chequeDate;
        private DateTime _gstClaimDate;
        public short CompanyId { get; set; }

        public string? ReceiptId { get; set; }

        public string? ReceiptNo { get; set; }
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

        [ForeignKey(nameof(CurrencyId))]
        public short CurrencyId { get; set; }

        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal CtyExhRate { get; set; }

        public short PaymentTypeId { get; set; }

        [ForeignKey(nameof(BankId))]
        public short BankId { get; set; }

        public string? BankCode { get; set; }
        public string? BankName { get; set; }

        public string? ChequeNo { get; set; }

        public string? ChequeDate
        {
            get { return DateHelperStatic.FormatDate(_chequeDate); }
            set { _chequeDate = DateHelperStatic.ParseDBDate(value); }
        }

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

        public string? GstClaimDate
        {
            get { return DateHelperStatic.FormatDate(_gstClaimDate); }
            set { _gstClaimDate = DateHelperStatic.ParseDBDate(value); }
        }

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

        public string? Remarks { get; set; }
        public string? PayeeTo { get; set; }
        public string? ModuleFrom { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public short? CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? CancelRemarks { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
        public string? CancelBy { get; set; }
        public byte EditVersion { get; set; }
        public List<CBGenReceiptDtViewModel> data_details { get; set; }
    }
}