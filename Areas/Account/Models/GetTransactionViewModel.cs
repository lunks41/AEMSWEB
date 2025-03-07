namespace AEMSWEB.Areas.Account.Models
{
    public class GetTransactionViewModel
    {
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }
        public short CurrencyId { get; set; }
        public short CompanyId { get; set; }
        public string DocumentId { get; set; }
        public bool IsRefund { get; set; }
    }
}