namespace AEMSWEB.Models
{
    public class BargeLookupModel
    {
        public short BargeId { get; set; }
        public string? BargeCode { get; set; }
        public string? BargeName { get; set; }
    }

    public class BankLookupModel
    {
        public short BankId { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
    }

    public class AccountGroupLookupModel
    {
        public short AccGroupId { get; set; }
        public string? AccGroupCode { get; set; }
        public string? AccGroupName { get; set; }
    }

    public class AccountSetupCategoryLookupModel
    {
        public short AccSetupCategoryId { get; set; }
        public string? AccSetupCategoryCode { get; set; }
        public string? AccSetupCategoryName { get; set; }
    }

    public class AccountSetupLookupModel
    {
        public short AccSetupId { get; set; }
        public string? AccSetupCode { get; set; }
        public string? AccSetupName { get; set; }
    }

    public class AccountTypeLookupModel
    {
        public short AccTypeId { get; set; }
        public string? AccTypeCode { get; set; }
        public string? AccTypeName { get; set; }
    }

    public class CategoryLookupModel
    {
        public short CategoryId { get; set; }
        public string? CategoryCode { get; set; }
        public string? CategoryName { get; set; }
    }
    public class ChartOfAccountLookupModel
    {
        public short GLId { get; set; }
        public string? GLCode { get; set; }
        public string? GLName { get; set; }
    }
    public class COACategoryLookupModel
    {
        public short COACategoryId { get; set; }
        public string? COACategoryCode { get; set; }
        public string? COACategoryName { get; set; }
    }
    public class CompanyLookupModel
    {
        public short CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }
    public class CountryLookupModel
    {
        public short CountryId { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
    }
    public class CreditTermLookupModel
    {
        public short CreditTermId { get; set; }
        public string? CreditTermCode { get; set; }
        public string? CreditTermName { get; set; }
    }
    public class CurrencyLookupModel
    {
        public int CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public bool IsMultiply { get; set; }
    }
    public class CustomerAddressLookupModel
    {
        public short AddressId { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? PinCode { get; set; }
        public short CountryId { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? PhoneNo { get; set; }
        public string? FaxNo { get; set; }
        public string? EmailAdd { get; set; }
        public string? WebUrl { get; set; }
    }
    public class CustomerContactLookupModel
    {
        public short ContactId { get; set; }
        public string? ContactName { get; set; }
        public string? OtherName { get; set; }
        public string? MobileNo { get; set; }
        public string? OffNo { get; set; }
        public string? FaxNo { get; set; }
        public string? EmailAdd { get; set; }
        public string? MessId { get; set; }
        public string? ContactMessType { get; set; }
    }
    public class CustomerGroupCreditLimitLookupModel
    {
        public short GroupCreditLimitId { get; set; }
        public string? GroupCreditLimitCode { get; set; }
        public string? GroupCreditLimitName { get; set; }
    }
    public class CustomerLookupModel
    {
        public int CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public short CurrencyId { get; set; }
        public short CreditTermId { get; set; }
        public short BankId { get; set; }
    }
    public class DepartmentLookupModel
    {
        public short DepartmentId { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
    }
    public class DesignationLookupModel
    {
        public short DesignationId { get; set; }
        public string? DesignationCode { get; set; }
        public string? DesignationName { get; set; }
    }
    public class DocumentTypeLookupModel
    {
        public short DocTypeId { get; set; }
        public string? DocTypeCode { get; set; }
        public string? DocTypeName { get; set; }
    }
    public class EmployeeLookupModel
    {
        public short EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
    }
    public class GroupCreditLimitLookupModel
    {
        public short GroupCreditLimitId { get; set; }
        public string? GroupCreditLimitCode { get; set; }
        public string? GroupCreditLimitName { get; set; }
    }
    public class GstCategoryLookupModel
    {
        public short GstCategoryId { get; set; }
        public string? GstCategoryCode { get; set; }
        public string? GstCategoryName { get; set; }
    }
    public class GstLookupModel
    {
        public short GstId { get; set; }
        public string? GstCode { get; set; }
        public string? GstName { get; set; }
    }
    public class ModuleLookupModel
    {
        public short ModuleId { get; set; }
        public string? ModuleCode { get; set; }
        public string? ModuleName { get; set; }
    }
    public class OrderTypeCategoryLookupModel
    {
        public short OrderTypeCategoryId { get; set; }
        public string? OrderTypeCategoryCode { get; set; }
        public string? OrderTypeCategoryName { get; set; }
    }
    public class OrderTypeLookupModel
    {
        public short CompanyId { get; set; }
        public string? OrderTypeCode { get; set; }
        public string? OrderTypeName { get; set; }
    }
    public class PaymentTypeLookupModel
    {
        public short PaymentTypeId { get; set; }
        public string? PaymentTypeCode { get; set; }
        public string? PaymentTypeName { get; set; }
    }
    public class PortLookupModel
    {
        public short PortId { get; set; }
        public string? PortCode { get; set; }
        public string? PortName { get; set; }
    }
    public class PortRegionLookupModel
    {
        public short PortRegionId { get; set; }
        public string? PortRegionCode { get; set; }
        public string? PortRegionName { get; set; }
    }
    public class ProductLookupModel
    {
        public short ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
    }
    public class SubCategoryLookupModel
    {
        public short SubCategoryId { get; set; }
        public string? SubCategoryCode { get; set; }
        public string? SubCategoryName { get; set; }
    }
    public class SupplierAddressLookupModel
    {
        public short AddressId { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
    }
    public class SupplierContactLookupModel
    {
        public short ContactId { get; set; }
        public string? ContactName { get; set; }
    }
    public class SupplierLookupModel
    {
        public int SupplierId { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }
        public short CurrencyId { get; set; }
        public short CreditTermId { get; set; }
    }
    public class TaxCategoryLookupModel
    {
        public short TaxCategoryId { get; set; }
        public string? TaxCategoryCode { get; set; }
        public string? TaxCategoryName { get; set; }
    }
    public class TaxLookupModel
    {
        public short TaxId { get; set; }
        public string? TaxCode { get; set; }
        public string? TaxName { get; set; }
    }
    public class TransactionLookupModel
    {
        public short TransactionId { get; set; }
        public string? TransactionCode { get; set; }
        public string? TransactionName { get; set; }
    }
    public class TransCategoryLookupModel
    {
        public short TransCategoryId { get; set; }
        public string? TransCategoryCode { get; set; }
        public string? TransCategoryName { get; set; }
    }
    public class UomLookupModel
    {
        public short UomId { get; set; }
        public string? UomCode { get; set; }
        public string? UomName { get; set; }
    }
    public class UserGroupLookupModel
    {
        public short UserGroupId { get; set; }
        public string? UserGroupCode { get; set; }
        public string? UserGroupName { get; set; }
    }
    public class UserLookupModel
    {
        public short UserId { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
    public class VesselLookupModel
    {
        public int VesselId { get; set; }
        public string? VesselCode { get; set; }
        public string? VesselName { get; set; }
    }
    public class VoyageLookupModel
    {
        public short VoyageId { get; set; }
        public string? VoyageNo { get; set; }
        public string? ReferenceNo { get; set; }
    }
    public class YearLookupModel
    {
        public short YearId { get; set; }
        public string? YearCode { get; set; }
        public string? YearName { get; set; }
    }

    public class ChargeLookupModel
    {
        public short TaskId { get; set; }
        public short ChargeId { get; set; }
        public string? ChargeCode { get; set; }
        public string? ChargeName { get; set; }
    }

    public class TaskLookupModel
    {
        public short TaskId { get; set; }
        public string? TaskCode { get; set; }
        public string? TaskName { get; set; }
    }
}