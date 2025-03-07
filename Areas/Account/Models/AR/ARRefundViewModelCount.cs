namespace AEMSWEB.Areas.Account.Models.AR
{
    public class ARRefundViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ARRefundViewModel> data { get; set; }
    }
}