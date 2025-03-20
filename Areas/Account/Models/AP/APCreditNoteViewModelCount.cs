namespace AEMSWEB.Areas.Account.Models.AP
{
    public class APCreditNoteViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<APCreditNoteViewModel> data { get; set; }
    }
}