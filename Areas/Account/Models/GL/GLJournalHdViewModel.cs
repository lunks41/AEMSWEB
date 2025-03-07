using AEMSWEB.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Areas.Account.Models.GL
{
    public class GLJournalHdViewModel
    {
        private DateTime _trnDate;
        private DateTime _accountDate;
        private DateTime _gstClaimDate;
        private DateTime _revDate;
        private DateTime _revcurrenceUntilDate;
        public short CompanyId { get; set; }

        public string JournalId { get; set; }

        public string JournalNo { get; set; }
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

        [ForeignKey(nameof(CurrencyId))]
        public short CurrencyId { get; set; }

        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal CtyExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public string GstClaimDate
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

        public string Remarks { get; set; }
        public bool IsReverse { get; set; }
        public bool IsRecurrency { get; set; }

        public string RevDate
        {
            get { return DateHelperStatic.FormatDate(_revDate); }
            set { _revDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string RecurrenceUntil
        {
            get { return DateHelperStatic.FormatDate(_revcurrenceUntilDate); }
            set { _revcurrenceUntilDate = DateHelperStatic.ParseDBDate(value); }
        }

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
        public List<GLJournalDtViewModel> data_details { get; set; }
    }
}