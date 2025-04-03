namespace AMESWEB.Models.Masters
{
    public class CustomerViewModel
    {
        public Int32 CustomerId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerOtherName { get; set; }
        public string? CustomerShortName { get; set; }
        public string? CustomerRegNo { get; set; }
        public Int16 CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public Int16 CreditTermId { get; set; }
        public string? CreditTermCode { get; set; }
        public string? CreditTermName { get; set; }
        public Int32 ParentCustomerId { get; set; }
        public Int16 AccSetupId { get; set; }
        public string? AccSetupCode { get; set; }
        public string? AccSetupName { get; set; }
        public Int32 SupplierId { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }
        public Int16 BankId { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsVendor { get; set; }
        public bool IsTrader { get; set; }
        public bool IsSupplier { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class CustomerAddressViewModel
    {
        public Int32 CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public Int16 AddressId { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? PinCode { get; set; }
        public Int16 CountryId { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? PhoneNo { get; set; }
        public string? FaxNo { get; set; }
        public string? EmailAdd { get; set; }
        public string? WebUrl { get; set; }
        public bool IsDefaultAdd { get; set; }
        public bool IsDeliveryAdd { get; set; }
        public bool IsFinAdd { get; set; }
        public bool IsSalesAdd { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class CustomerContactViewModel
    {
        public Int16 ContactId { get; set; }
        public Int32 CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? ContactName { get; set; }
        public string? OtherName { get; set; }
        public string? MobileNo { get; set; }
        public string? OffNo { get; set; }
        public string? FaxNo { get; set; }
        public string? EmailAdd { get; set; }
        public string? MessId { get; set; }
        public string? ContactMessType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsFinance { get; set; }
        public bool IsSales { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveCustomerContactViewModel
    {
        public CustomerContactViewModel customerContact { get; set; }
        public string? companyId { get; set; }
    }

    public class SaveCustomerAddressViewModel
    {
        public CustomerAddressViewModel customerAddress { get; set; }
        public string? companyId { get; set; }
    }

    public class SaveCustomerViewModel
    {
        public CustomerViewModel customer { get; set; }
        public string? companyId { get; set; }
    }

    public class CustomerViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CustomerViewModel> data { get; set; }
    }

    public class CustomerAddressViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CustomerAddressViewModel> data { get; set; }
    }

    public class CustomerContactViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CustomerContactViewModel> data { get; set; }
    }
}