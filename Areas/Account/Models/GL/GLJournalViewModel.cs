namespace AEMSWEB.Areas.Account.Models.GL
{
    public class GLJournalViewModel
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<GLJournalHdViewModel> data { get; set; }
    }
}