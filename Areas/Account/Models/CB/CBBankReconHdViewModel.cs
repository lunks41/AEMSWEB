using AMESWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Areas.Account.Models.CB
{
    public class CBBankReconHdViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;
        private DateTime _fromDate;
        private DateTime _toDate;

        public short CompanyId { get; set; }
        public string? ReconId { get; set; }
        public string? ReconNo { get; set; }
        public string? PrevReconId { get; set; }
        public string? PrevReconNo { get; set; }
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
        public short CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        public string? FromDate
        {
            get { return DateHelperStatic.FormatDate(_fromDate); }
            set { _fromDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string? ToDate
        {
            get { return DateHelperStatic.FormatDate(_toDate); }
            set { _toDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string? Remarks { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal OPBalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CLBalAmt { get; set; }

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
        public List<CBBankReconDtViewModel> data_details { get; set; }
    }
}