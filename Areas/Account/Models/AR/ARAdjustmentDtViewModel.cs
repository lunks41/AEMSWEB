using AEMSWEB.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Areas.Account.Models.AR
{
    public class ARAdjustmentDtViewModel
    {
        //Get & Set the formate

        private DateTime _supplyDate;
        private DateTime _deliveryDate;

        //actual Model
        public string AdjustmentId { get; set; }

        public string AdjustmentNo { get; set; }
        public short ItemNo { get; set; }
        public short SeqNo { get; set; }
        public short DocItemNo { get; set; }
        public short ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public short GLId { get; set; }
        public string GLCode { get; set; }
        public string GLName { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal QTY { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal BillQTY { get; set; }

        public short UomId { get; set; }
        public string UomCode { get; set; }
        public string UomName { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public string Remarks { get; set; }
        public bool IsDebit { get; set; }
        public byte GstId { get; set; }
        public string GstCode { get; set; }
        public string GstName { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal GstPercentage { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstCtyAmt { get; set; }

        public string DeliveryDate
        {
            get { return DateHelperStatic.FormatDate(_deliveryDate); }
            set { _deliveryDate = DateHelperStatic.ParseDBDate(value); }
        }

        public short DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public short EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public short PortId { get; set; }
        public string PortCode { get; set; }
        public string PortName { get; set; }
        public int VesselId { get; set; }
        public string VesselCode { get; set; }
        public string VesselName { get; set; }
        public short BargeId { get; set; }
        public string BargeCode { get; set; }
        public string BargeName { get; set; }
        public short VoyageId { get; set; }
        public string VoyageNo { get; set; }
        public string VoyageReferenceNo { get; set; }
        public string OperationId { get; set; }
        public string OperationNo { get; set; }
        public string OPRefNo { get; set; }
        public string SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }

        public string SupplyDate
        {
            get { return DateHelperStatic.FormatDate(_supplyDate); }
            set { _supplyDate = DateHelperStatic.ParseDBDate(value); }
        }

        public string SupplierName { get; set; }
        public string SuppAdjustmentNo { get; set; }
        public byte EditVersion { get; set; }
    }
}