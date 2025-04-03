using AMESWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Areas.Account.Models.AP
{
    public class APPaymentDtViewModel
    {
        private DateTime _docaccountDate;
        private DateTime _docdueDate;

        public short CompanyId { get; set; }
        public string? PaymentId { get; set; }
        public string? PaymentNo { get; set; }
        public short ItemNo { get; set; }
        public short TransactionId { get; set; }
        public string? DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public string? ReferenceNo { get; set; }
        public short DocCurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal DocExhRate { get; set; }

        public string? DocAccountDate
        {
            get { return DateHelperStatic.FormatDate(_docaccountDate); }
            set { _docaccountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string? DocDueDate
        {
            get { return DateHelperStatic.FormatDate(_docdueDate); }
            set { _docdueDate = DateHelperStatic.ParseDBDate(value); }
        }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocTotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocTotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocBalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocBalLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AllocLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocAllocAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal DocAllocLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CentDiff { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ExhGainLoss { get; set; }

        public byte EditVersion { get; set; }
    }
}