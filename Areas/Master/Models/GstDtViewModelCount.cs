namespace AEMSWEB.Models.Masters
{
    public class GstDtViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<GstDtViewModel> data { get; set; }
    }
}