namespace AEMSWEB.Models.Admin
{
    public class CompanyViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CompanyViewModel> data { get; set; }
    }
}