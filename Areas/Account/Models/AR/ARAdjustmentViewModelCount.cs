namespace AEMSWEB.Areas.Account.Models.AR
{
    public class ARAdjustmentViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ARAdjustmentViewModel> data { get; set; }
    }
}