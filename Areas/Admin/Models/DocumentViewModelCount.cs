namespace AEMSWEB.Models.Admin
{
    public class DocumentViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<DocumentViewModel> data { get; set; }
    }
}