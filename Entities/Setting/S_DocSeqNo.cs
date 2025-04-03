using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMESWEB.Entities.Setting
{
    [PrimaryKey(nameof(CompanyId), nameof(ModuleId), nameof(TransactionId))]
    public class S_DocSeqNo
    {
        public Int16 CompanyId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }

        public Byte H_ReferenceNo { get; set; }
        public Byte H_TrnDate { get; set; }
        public Byte H_AccountDate { get; set; }
        public Byte H_DeliveryDate { get; set; }
        public Byte H_DueDate { get; set; }
        public Byte H_CustomerId { get; set; }
        public Byte H_CurrencyId { get; set; }
        public Byte H_ExhRate { get; set; }
        public Byte H_CtyExhRate { get; set; }
        public Byte H_CreditTermId { get; set; }
        public Byte H_BankId { get; set; }
        public Byte H_InvoiceNo { get; set; }
        public Byte H_TotAmt { get; set; }
        public Byte H_TotLocalAmt { get; set; }
        public Byte H_TotCtyAmt { get; set; }
        public Byte H_GstClaimDate { get; set; }
        public Byte H_GstAmt { get; set; }
        public Byte H_GstLocalAmt { get; set; }
        public Byte H_GstCtyAmt { get; set; }
        public Byte H_TotAmtAftGst { get; set; }
        public Byte H_TotLocalAmtAftGst { get; set; }
        public Byte H_TotCtyAmtAftGst { get; set; }
        public Byte H_SalesOrderNo { get; set; }
        public Byte H_OperationNo { get; set; }
        public Byte H_Remarks { get; set; }
        public Byte H_Address1 { get; set; }
        public Byte H_Address2 { get; set; }
        public Byte H_Address3 { get; set; }
        public Byte H_Address4 { get; set; }
        public Byte H_PinCode { get; set; }
        public Byte H_CountryId { get; set; }
        public Byte H_PhoneNo { get; set; }
        public Byte H_FaxNo { get; set; }
        public Byte H_ContactName { get; set; }
        public Byte H_MobileNo { get; set; }
        public Byte H_EmailAdd { get; set; }
        public Byte H_SupplierName { get; set; }
        public Byte H_SuppInvoiceNo { get; set; }
        public Byte H_APInvoiceNo { get; set; }

        public Byte D_SeqNo { get; set; }
        public Byte D_ProductId { get; set; }
        public Byte D_GLId { get; set; }
        public Byte D_QTY { get; set; }
        public Byte D_BillQTY { get; set; }
        public Byte D_UomId { get; set; }
        public Byte D_UnitPrice { get; set; }
        public Byte D_TotAmt { get; set; }
        public Byte D_TotLocalAmt { get; set; }
        public Byte D_TotCtyAmt { get; set; }
        public Byte D_Remarks { get; set; }
        public Byte D_GstId { get; set; }
        public Byte D_GstPercentage { get; set; }
        public Byte D_GstAmt { get; set; }
        public Byte D_GstLocalAmt { get; set; }
        public Byte D_GstCtyAmt { get; set; }
        public Byte D_DeliveryDate { get; set; }
        public Byte D_DepartmentId { get; set; }
        public Byte D_EmployeeId { get; set; }
        public Byte D_PortId { get; set; }
        public Byte D_VesselId { get; set; }
        public Byte D_BargeId { get; set; }
        public Byte D_VoyageId { get; set; }
        public Byte D_OperationNo { get; set; }
        public Byte D_OPRefNo { get; set; }
        public Byte D_SalesOrderNo { get; set; }
        public Byte D_SupplyDate { get; set; }
        public Byte D_SupplierName { get; set; }
        public Byte D_SuppInvoiceNo { get; set; }
        public Byte D_APInvoiceNo { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}