namespace AEMSWEB.Models.Masters
{
    public class COACategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<COACategoryViewModel> data { get; set; }
    }
}