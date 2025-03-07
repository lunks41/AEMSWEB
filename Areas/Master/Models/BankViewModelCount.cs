using AEMSWEB.Areas.Master.Models;

namespace AEMSWEB.Models.Masters
{
    public class BankViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<BankViewModel> data { get; set; }
    }
}