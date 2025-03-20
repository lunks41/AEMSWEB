namespace AEMSWEB.Models.Admin
{
    public class UserLogViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<UserLogViewModel> data { get; set; }
    }
}