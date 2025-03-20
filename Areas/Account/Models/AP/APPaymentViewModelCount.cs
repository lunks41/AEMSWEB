namespace AEMSWEB.Areas.Account.Models.AP
{
    public class APPaymentViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<APPaymentViewModel> data { get; set; }
    }
}