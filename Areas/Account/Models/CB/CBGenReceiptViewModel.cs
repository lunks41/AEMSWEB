namespace AMESWEB.Areas.Account.Models.CB
{
    public class CBGenReceiptViewModel
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CBGenReceiptHdViewModel> data { get; set; }
    }
}