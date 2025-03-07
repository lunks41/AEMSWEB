namespace AEMSWEB.Models.Masters
{
    public class GroupCreditLimitViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 GroupCreditLimitId { get; set; }
        public string GroupCreditLimitCode { get; set; }
        public string GroupCreditLimitName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}