namespace AMESWEB.Areas.Setting.Models
{
    public class MandatoryFieldsViewModel
    {
        public short ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public short TransactionId { get; set; }
        public string? TransactionCode { get; set; }
        public string? TransactionName { get; set; }
        public bool M_ProductId { get; set; }
        public bool M_GLId { get; set; }
        public bool M_QTY { get; set; }
        public bool M_UomId { get; set; }
        public bool M_UnitPrice { get; set; }
        public bool M_TotAmt { get; set; }
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
        public bool M_ReferenceNo { get; set; }
        public bool M_SuppInvoiceNo { get; set; }
        public bool M_BankId { get; set; }
        public bool M_Remarks_Hd { get; set; }
        public bool M_Address1 { get; set; }
        public bool M_Address2 { get; set; }
        public bool M_Address3 { get; set; }
        public bool M_Address4 { get; set; }
        public bool M_PinCode { get; set; }
        public bool M_CountryId { get; set; }
        public bool M_PhoneNo { get; set; }
        public bool M_ContactName { get; set; }
        public bool M_MobileNo { get; set; }
        public bool M_EmailAdd { get; set; }
    }

    public class SaveMandatoryFieldsViewModel
    {
        public List<MandatoryFieldsViewModel> MandatoryFields { get; set; }
        public byte CompanyId { get; set; }
    }
}