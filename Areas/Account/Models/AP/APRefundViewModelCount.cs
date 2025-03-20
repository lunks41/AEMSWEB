namespace AEMSWEB.Areas.Account.Models.AP
{
    public class APRefundViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<APRefundViewModel> data { get; set; }
    }
}