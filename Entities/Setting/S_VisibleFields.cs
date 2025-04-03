using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Setting
{
    [PrimaryKey(nameof(CompanyId), nameof(ModuleId), nameof(TransactionId))]
    public class S_VisibleFields
    {
        public Int16 CompanyId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public bool M_ProductId { get; set; }
        public bool M_QTY { get; set; }
        public bool M_BillQTY { get; set; }
        public bool M_UomId { get; set; }
        public bool M_UnitPrice { get; set; }
        public bool M_Remarks { get; set; }
        public bool M_GstId { get; set; }
        public bool M_DeliveryDate { get; set; }
        public bool M_DepartmentId { get; set; }
        public bool M_EmployeeId { get; set; }
        public bool M_PortId { get; set; }
        public bool M_VesselId { get; set; }
        public bool M_BargeId { get; set; }
        public bool M_VoyageId { get; set; }
        public bool M_SupplyDate { get; set; }
        public bool M_BankId { get; set; }
        public bool M_CtyCurr { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}