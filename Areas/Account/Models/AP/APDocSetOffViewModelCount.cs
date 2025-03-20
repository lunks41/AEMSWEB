namespace AEMSWEB.Areas.Account.Models.AP
{
    public class APDocSetOffViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<APDocSetOffViewModel> data { get; set; }
    }
}