namespace AEMSWEB.Areas.Setting.Models
{
    public class DynamicLookupViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public int totalRecords { get; set; }
        public List<DynamicLookupViewModel> data { get; set; }
    }
}