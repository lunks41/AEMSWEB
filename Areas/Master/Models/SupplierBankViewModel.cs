namespace AMESWEB.Models.Masters
{
    public class SupplierBankViewModel
    {
        public Int16 SupplierBankId { get; set; }
        public Int32 SupplierId { get; set; }
        public Int16 BankId { get; set; }
        public string? BranchName { get; set; }
        public string? AccountNo { get; set; }
        public string? SwiftCode { get; set; }
        public string? OtherCode { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? PinCode { get; set; }
        public Int16 CountryId { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? Remarks1 { get; set; }
        public string? Remarks2 { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SupplierBankViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<SupplierBankViewModel> data { get; set; }
    }
}