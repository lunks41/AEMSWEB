using AEMSWEB.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Areas.Account.Models.GL
{
    public class GLOpeningBalanceViewModel
    {
        private DateTime? _accountDate;
        public short CompanyId { get; set; }
        public int DocumentId { get; set; }
        public int ItemNo { get; set; }
        public int GLId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }
        public string DocumentNo { get; set; }

        public string AccountDate
        {
            get { return _accountDate.HasValue ? DateHelperStatic.FormatDate(_accountDate.Value) : ""; }
            set { _accountDate = string.IsNullOrEmpty(value) ? null : DateHelperStatic.ParseDBDate(value); }
        }

        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public short CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }

        [Column(TypeName = "decimal(18,10)")]
        public decimal ExhRate { get; set; }

        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int PortId { get; set; }
        public string PortCode { get; set; }
        public string PortName { get; set; }
        public int VesselId { get; set; }
        public string VesselCode { get; set; }
        public string VesselName { get; set; }
        public short BargeId { get; set; }
        public string BargeCode { get; set; }
        public string BargeName { get; set; }
        public int VoyageId { get; set; }
        public string VoyageNo { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
        public byte EditVersion { get; set; }
    }
}