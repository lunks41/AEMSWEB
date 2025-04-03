namespace AMESWEB.Areas.Setting.Models
{
    public class NumberSettingViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<NumberSettingViewModel> data { get; set; }
    }
}