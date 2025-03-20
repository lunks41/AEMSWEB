namespace AEMSWEB.Models.Masters
{
    public class CurrencyLookupModel
    {
        public Int32 CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public bool IsMultiply { get; set; }
    }
}