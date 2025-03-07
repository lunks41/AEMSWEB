namespace AEMSWEB.Areas.Account.Models.AP
{
    public class APAdjustmentViewModelCount
    {
        public short responseCode { get; set; }
        public string responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<APAdjustmentViewModel> data { get; set; }
    }
}