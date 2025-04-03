namespace AMESWEB.Areas.Account.Models.CB
{
    public class CBBankTransferViewModelList
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CBBankTransferViewModel> data { get; set; }
    }
}