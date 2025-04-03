namespace AMESWEB.Areas.Account.Models.AP
{
    public class APInvoiceViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<APInvoiceViewModel> data { get; set; }
    }
}