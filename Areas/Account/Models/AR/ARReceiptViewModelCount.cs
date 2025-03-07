namespace AEMSWEB.Areas.Account.Models.AR
{
    public class ARReceiptViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ARReceiptViewModel> data { get; set; }
    }
}