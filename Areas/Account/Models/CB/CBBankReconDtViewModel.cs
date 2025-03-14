using AEMSWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Areas.Account.Models.CB
{
    public class CBBankReconDtViewModel
    {
        private DateTime _accountDate;
        private DateTime _chequeDate;
        public string ReconId { get; set; }
        public string ReconNo { get; set; }
        public short ItemNo { get; set; }
        public bool IsSel { get; set; }
        public byte ModuleId { get; set; }
        public byte TransactionId { get; set; }
        public string DocumentId { get; set; }
        public string DocumentNo { get; set; }
        public string DocReferenceNo { get; set; }

        public string AccountDate
        {
            get { return DateHelperStatic.FormatDate(_accountDate); }
            set { _accountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public short PaymentTypeId { get; set; }
        public string ChequeNo { get; set; }

        public string ChequeDate
        {
            get { return DateHelperStatic.FormatDate(_chequeDate); }
            set { _chequeDate = DateHelperStatic.ParseDBDate(value); }
        }

        public int CustomerId { get; set; }
        public int SupplierId { get; set; }

        [ForeignKey(nameof(GLId))]
        public short GLId { get; set; }

        public string GLCode { get; set; }
        public string GLName { get; set; }

        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        public string PaymentFromTo { get; set; }
        public string Remarks { get; set; }

        public byte EditVersion { get; set; }
    }
}