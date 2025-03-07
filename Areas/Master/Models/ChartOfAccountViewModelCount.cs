using AEMSWEB.Areas.Master.Models;

namespace AEMSWEB.Models.Masters
{
    public class ChartOfAccountViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ChartOfAccountViewModel> data { get; set; }
    }
}