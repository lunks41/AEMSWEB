namespace AEMSWEB.Areas.Account.Models.CB
{
    public class CBBankReconViewModel
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<CBBankReconHdViewModel> data { get; set; }
    }
}