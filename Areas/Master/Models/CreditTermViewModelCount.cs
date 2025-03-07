namespace AEMSWEB.Models.Masters
{
    public class CreditTermViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CreditTermViewModel> data { get; set; }
    }
}