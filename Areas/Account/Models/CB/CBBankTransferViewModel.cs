using AEMSWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Areas.Account.Models.CB
{
    public class CBBankTransferViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;
        private DateTime _chequeDate;
        public short CompanyId { get; set; }
        public string TransferId { get; set; }
        public string TransferNo { get; set; }
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

        public short FromBankId { get; set; }
        public string FromBankCode { get; set; }
        public string FromBankName { get; set; }
        public short FromCurrencyId { get; set; }
        public string FromCurrencyCode { get; set; }
        public string FromCurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal FromExhRate { get; set; }

        public short PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }

        public string ChequeDate
        {
            get { return DateHelperStatic.FormatDate(_chequeDate); }
            set { _chequeDate = DateHelperStatic.ParseDBDate(value); }
        }

        [Column(TypeName = "decimal(18,4)")]
        public decimal FromBankChgAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal FromBankChgLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal FromTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal FromTotLocalAmt { get; set; }

        public short ToBankId { get; set; }
        public string ToBankCode { get; set; }
        public string ToBankName { get; set; }
        public short ToCurrencyId { get; set; }
        public string ToCurrencyCode { get; set; }
        public string ToCurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
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

        public string Remarks { get; set; }
        public string PayeeTo { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public string ModuleFrom { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public bool IsCancel { get; set; }
        public short? CancelById { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelRemarks { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
        public string CancelBy { get; set; }
        public byte EditVersion { get; set; }
    }
}