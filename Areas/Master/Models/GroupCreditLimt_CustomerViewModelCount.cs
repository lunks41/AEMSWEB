namespace AEMSWEB.Models.Masters
{
    public class GroupCreditLimit_CustomerViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<GroupCreditLimit_CustomerViewModel> data { get; set; }
    }
}