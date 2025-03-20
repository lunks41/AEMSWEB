namespace AEMSWEB.Areas.Setting.Models
{
    public class DocSeqNoViewModel
    {
        public short CompanyId { get; set; }
        public short ModuleId { get; set; }
        public short TransactionId { get; set; }

        public byte H_ReferenceNo { get; set; }
        public byte H_TrnDate { get; set; }
        public byte H_AccountDate { get; set; }
        public byte H_DeliveryDate { get; set; }
        public byte H_DueDate { get; set; }
        public byte H_CustomerId { get; set; }
        public byte H_CurrencyId { get; set; }
        public byte H_ExhRate { get; set; }
        public byte H_CtyExhRate { get; set; }
        public byte H_CreditTermId { get; set; }
        public byte H_BankId { get; set; }
        public byte H_InvoiceNo { get; set; }
        public byte H_TotAmt { get; set; }
        public byte H_TotLocalAmt { get; set; }
        public byte H_TotCtyAmt { get; set; }
        public byte H_GstClaimDate { get; set; }
        public byte H_GstAmt { get; set; }
        public byte H_GstLocalAmt { get; set; }
        public byte H_GstCtyAmt { get; set; }
        public byte H_TotAmtAftGst { get; set; }
        public byte H_TotLocalAmtAftGst { get; set; }
        public byte H_TotCtyAmtAftGst { get; set; }
        public byte H_SalesOrderNo { get; set; }
        public byte H_OperationNo { get; set; }
        public byte H_Remarks { get; set; }
        public byte H_Address1 { get; set; }
        public byte H_Address2 { get; set; }
        public byte H_Address3 { get; set; }
        public byte H_Address4 { get; set; }
        public byte H_PinCode { get; set; }
        public byte H_CountryId { get; set; }
        public byte H_PhoneNo { get; set; }
        public byte H_FaxNo { get; set; }
        public byte H_ContactName { get; set; }
        public byte H_MobileNo { get; set; }
        public byte H_EmailAdd { get; set; }
        public byte H_SupplierName { get; set; }
        public byte H_SuppInvoiceNo { get; set; }
        public byte H_APInvoiceNo { get; set; }

        public byte D_SeqNo { get; set; }
        public byte D_ProductId { get; set; }
        public byte D_GLId { get; set; }
        public byte D_QTY { get; set; }
        public byte D_BillQTY { get; set; }
        public byte D_UomId { get; set; }
        public byte D_UnitPrice { get; set; }
        public byte D_TotAmt { get; set; }
        public byte D_TotLocalAmt { get; set; }
        public byte D_TotCtyAmt { get; set; }
        public byte D_Remarks { get; set; }
        public byte D_GstId { get; set; }
        public byte D_GstPercentage { get; set; }
        public byte D_GstAmt { get; set; }
        public byte D_GstLocalAmt { get; set; }
        public byte D_GstCtyAmt { get; set; }
        public byte D_DeliveryDate { get; set; }
        public byte D_DepartmentId { get; set; }
        public byte D_EmployeeId { get; set; }
        public byte D_PortId { get; set; }
        public byte D_VesselId { get; set; }
        public byte D_BargeId { get; set; }
        public byte D_VoyageId { get; set; }
        public byte D_OperationNo { get; set; }
        public byte D_OPRefNo { get; set; }
        public byte D_SalesOrderNo { get; set; }
        public byte D_SupplyDate { get; set; }
        public byte D_SupplierName { get; set; }
        public byte D_SuppInvoiceNo { get; set; }
        public byte D_APInvoiceNo { get; set; }

        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }
}