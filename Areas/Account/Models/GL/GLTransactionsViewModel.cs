using AMESWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Areas.Account.Models.GL
{
    public class GLTransactionsViewModel
    {
        private DateTime _accountDate;
        private DateTime _gstClaimDate;
        public short CompanyId { get; set; }
        public string? DocumentId { get; set; }
        public int ItemNo { get; set; }
        public short ModuleId { get; set; }
        public short TransactionId { get; set; }
        public string? DocumentNo { get; set; }
        public string? ReferenceNo { get; set; }

        public string? AccountDate
        {
            get { return DateHelperStatic.FormatDate(_accountDate); }
            set { _accountDate = DateHelperStatic.ParseDBDate(value); }
        }

        public int CustomerId { get; set; }
        public short CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal CtyExhRate { get; set; }

        public short BankId { get; set; }
        public short GLId { get; set; }
        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public short GstId { get; set; }

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

        public string? Remarks { get; set; }
        public short DepartmentId { get; set; }
        public short EmployeeId { get; set; }
        public short PortId { get; set; }
        public int VesselId { get; set; }
        public short BargeId { get; set; }
        public string? PaymentFromTo { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentNo { get; set; }
        public bool IsSystem { get; set; }
        public bool IsMaster { get; set; }
        public string? ModuleFrom { get; set; }
        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}