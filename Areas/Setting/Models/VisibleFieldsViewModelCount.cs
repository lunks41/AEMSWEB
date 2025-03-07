namespace AEMSWEB.Areas.Setting.Models
{
    public class VisibleFieldsViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public int totalRecords { get; set; }
        public List<VisibleFieldsViewModel> data { get; set; }
    }
}