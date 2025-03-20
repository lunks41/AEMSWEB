namespace AEMSWEB.Areas.Account.Models.AR
{
    public class ARCreditNoteViewModelCount
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<ARCreditNoteViewModel> data { get; set; }
    }
}