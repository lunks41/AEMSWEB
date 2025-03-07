namespace AEMSWEB.Areas.Setting.Models
{
    public class ModelNameViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ModelNameViewModel> data { get; set; }
    }
}