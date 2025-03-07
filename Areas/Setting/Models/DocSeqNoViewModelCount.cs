namespace AEMSWEB.Areas.Setting.Models
{
    public class DocSeqNoViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public int totalRecords { get; set; }
        public List<DynamicLookupViewModel> data { get; set; }
    }
}