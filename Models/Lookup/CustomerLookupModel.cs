namespace AEMSWEB.Models.Masters
{
    public class CustomerLookupModel
    {
        public Int32 CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public Int16 CurrencyId { get; set; }
        public Int16 CreditTermId { get; set; }
        public Int16 BankId { get; set; }
    }
}