namespace AEMSWEB.Models.Masters
{
    public class CustomerContactLookupModel
    {
        public Int16 ContactId { get; set; }
        public string? ContactName { get; set; }
        public string? OtherName { get; set; }
        public string? MobileNo { get; set; }
        public string? OffNo { get; set; }
        public string? FaxNo { get; set; }
        public string? EmailAdd { get; set; }
        public string? MessId { get; set; }
        public string? ContactMessType { get; set; }
    }
}