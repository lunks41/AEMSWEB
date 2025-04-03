namespace AMESWEB.Areas.Account.Models.CB
{
    public class CBGenPaymentViewModel
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CBGenPaymentHdViewModel> data { get; set; }
    }
}