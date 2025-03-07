namespace AEMSWEB.Areas.Setting.Models
{
    public class MandatoryFieldsViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<MandatoryFieldsViewModel> data { get; set; }
    }
}