namespace AEMSWEB.Models.Masters
{
    public class CustomerAddressLookupModel
    {
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
    }
}