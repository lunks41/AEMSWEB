namespace AMESWEB.Areas.Account.Models.AR
{
    public class ARDebitNoteViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ARDebitNoteViewModel> data { get; set; }
    }
}