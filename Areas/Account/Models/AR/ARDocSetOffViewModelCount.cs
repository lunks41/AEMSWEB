namespace AMESWEB.Areas.Account.Models.AR
{
    public class ARDocSetOffViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ARDocSetOffViewModel> data { get; set; }
    }
}