namespace AMESWEB.Areas.Account.Models.AR
{
    public class ArInvoiceViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ArInvoiceViewModel> data { get; set; }
    }
}