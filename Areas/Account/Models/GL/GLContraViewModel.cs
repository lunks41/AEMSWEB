namespace AEMSWEB.Areas.Account.Models.GL
{
    public class GLContraViewModel
    {
        public short responseCode { get; set; }
        public string? responseMessage { get; set; }
        public long totalRecords { get; set; }
        public List<GLContraHdViewModel> data { get; set; }
    }
}