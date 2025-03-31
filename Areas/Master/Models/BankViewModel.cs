namespace AEMSWEB.Areas.Master.Models
{
    public class SaveBankViewModel
    {
        public BankViewModel bank { get; set; }
        public string? companyId { get; set; }
    }

    public class SaveBankAddressViewModel
    {
        public BankAddressViewModel bankAddress { get; set; }
        public string? companyId { get; set; }
    }

    public class SaveBankContactViewModel
    {
        public BankContactViewModel bankContact { get; set; }
        public string? companyId { get; set; }
    }

    public class BankViewModel
    {
        public Int16 BankId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
        public Int16 CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public string? AccountNo { get; set; }
        public string? SwiftCode { get; set; }
        public string? Remarks1 { get; set; }
        public string? Remarks2 { get; set; }
        public Int16 GLId { get; set; }
        public string? GLCode { get; set; }
        public string? GLName { get; set; }
        public bool IsActive { get; set; }
        public bool IsOwnBank { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class BankContactViewModel
    {
        public Int16 ContactId { get; set; }
        public Int32 BankId { get; set; }

        public string? BankCode { get; set; }

        public string? BankName { get; set; }
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

    public class BankAddressViewModel
    {
        public Int32 BankId { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
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

    public class BankContactViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<BankContactViewModel> data { get; set; }
    }

    public class BankViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<BankViewModel> data { get; set; }
    }

    public class BankAddressViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<BankAddressViewModel> data { get; set; }
    }
}