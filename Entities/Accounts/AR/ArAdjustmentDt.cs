using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Accounts.AR
{
    [PrimaryKey(nameof(AdjustmentId), nameof(ItemNo))]
    public class ArAdjustmentDt
    {
        public Int64 AdjustmentId { get; set; }
        public string AdjustmentNo { get; set; }
        public Int16 ItemNo { get; set; }
        public Int16 SeqNo { get; set; }
        public Int16 DocItemNo { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Int16 ProductId { get; set; }

        [ForeignKey(nameof(GLId))]
        public Int16 GLId { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal QTY { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal BillQTY { get; set; }

        [ForeignKey(nameof(UomId))]
        public Int16 UomId { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal UnitPrice { get; set; }

        public bool IsDebit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotCtyAmt { get; set; }

        public string Remarks { get; set; }

        [ForeignKey(nameof(UomId))]
        public byte GstId { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal GstPercentage { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstLocalAmt { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal GstCtyAmt { get; set; }

        public DateTime DeliveryDate { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Int16 DepartmentId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Int16 EmployeeId { get; set; }

        [ForeignKey(nameof(PortId))]
        public Int16 PortId { get; set; }

        [ForeignKey(nameof(VesselId))]
        public Int32 VesselId { get; set; }

        [ForeignKey(nameof(BargeId))]
        public Int16 BargeId { get; set; }

        [ForeignKey(nameof(VoyageId))]
        public Int16 VoyageId { get; set; }

        public Int64 OperationId { get; set; }
        public string OperationNo { get; set; }
        public string OPRefNo { get; set; }
        public Int64 SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
        public DateTime SupplyDate { get; set; }
        public string SupplierName { get; set; }
        public string SuppAdjustmentNo { get; set; }

        public byte EditVersion { get; set; }
    }
}