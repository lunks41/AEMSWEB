namespace AEMSWEB.Areas.Account.Models.AR
{
    public class ARInvoiceViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ARInvoiceViewModel> data { get; set; }
    }
}