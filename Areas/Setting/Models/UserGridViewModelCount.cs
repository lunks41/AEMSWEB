namespace AEMSWEB.Areas.Setting.Models
{
    public class UserGridViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<UserGridViewModel> data { get; set; }
    }
}