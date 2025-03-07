namespace AEMSWEB.Models.Admin
{
    public class UserViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<UserViewModel> data { get; set; }
    }
}