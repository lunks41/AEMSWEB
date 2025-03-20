namespace AEMSWEB.Models.Masters
{
    public class SaveSupplierViewModel
    {
        public SupplierViewModel customer { get; set; }
        public string companyId { get; set; }
    }

    public class SaveSupplierAddressViewModel
    {
        public SupplierAddressViewModel customerAddress { get; set; }
        public string companyId { get; set; }
    }

    public class SaveSupplierContactViewModel
    {
        public SupplierContactViewModel customerContact { get; set; }
        public string companyId { get; set; }
    }

    public class SupplierViewModel
    {
        public Int32 SupplierId { get; set; }
        public Int16 CompanyId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierOtherName { get; set; }
        public string SupplierShortName { get; set; }
        public string SupplierRegNo { get; set; }
        public Int16 CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public Int16 CreditTermId { get; set; }
        public string CreditTermCode { get; set; }
        public string CreditTermName { get; set; }
        public Int32 ParentSupplierId { get; set; }
        public Int16 AccSetupId { get; set; }
        public string AccSetupCode { get; set; }
        public string AccSetupName { get; set; }
        public Int32 CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsVendor { get; set; }
        public bool IsTrader { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SupplierAddressViewModel
    {
        public Int32 SupplierId { get; set; }
        public Int16 AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PinCode { get; set; }
        public Int16 CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string EmailAdd { get; set; }
        public string WebUrl { get; set; }
        public bool IsDefaultAdd { get; set; }
        public bool IsDeliveryAdd { get; set; }
        public bool IsFinAdd { get; set; }
        public bool IsSalesAdd { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SupplierContactViewModel
    {
        public Int16 ContactId { get; set; }
        public Int32 SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ContactName { get; set; }
        public string OtherName { get; set; }
        public string MobileNo { get; set; }
        public string OffNo { get; set; }
        public string FaxNo { get; set; }
        public string EmailAdd { get; set; }
        public string MessId { get; set; }
        public string ContactMessType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsFinance { get; set; }
        public bool IsSales { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SupplierViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<SupplierViewModel> data { get; set; }
    }

    public class SupplierAddressViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<SupplierAddressViewModel> data { get; set; }
    }

    public class SupplierContactViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<SupplierContactViewModel> data { get; set; }
    }
}