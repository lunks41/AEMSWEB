namespace AMESWEB.Areas.Account.Models.AP
{
    public class APDebitNoteViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<APDebitNoteViewModel> data { get; set; }
    }
}