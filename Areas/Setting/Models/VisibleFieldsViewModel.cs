namespace AMESWEB.Areas.Setting.Models
{
    public class VisibleFieldsViewModel
    {
        public short ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public short TransactionId { get; set; }
        public string? TransactionCode { get; set; }
        public string? TransactionName { get; set; }
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
    }
}