namespace AEMSWEB.Models.Masters
{
    public class SupplierLookupModel
    {
        public Int32 SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int16 CreditTermId { get; set; }
    }
}