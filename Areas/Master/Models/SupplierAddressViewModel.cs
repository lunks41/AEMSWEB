namespace AEMSWEB.Models.Masters
{
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

    public class SupplierAddressViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<SupplierAddressViewModel> data { get; set; }
    }
}