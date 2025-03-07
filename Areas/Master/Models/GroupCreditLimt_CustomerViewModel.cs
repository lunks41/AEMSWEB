namespace AEMSWEB.Models.Masters
{
    public class GroupCreditLimit_CustomerViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 GroupCreditLimitId { get; set; }
        public string GroupCreditLimitCode { get; set; }
        public string GroupCreditLimitName { get; set; }
        public Int32 CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}