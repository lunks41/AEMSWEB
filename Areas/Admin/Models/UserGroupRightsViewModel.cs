namespace AEMSWEB.Models.Admin
{
    public class UserGroupRightsViewModel
    {
        public Int16 UserGroupId { get; set; }
        public Int16 ModuleId { get; set; }
        public string ModuleName { get; set; }
        public Int16 TransactionId { get; set; }
        public string TransactionName { get; set; }
        public string TransCategoryName { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
        public Int16 ModSeqNo { get; set; }
        public Int16 TrnCatSeqNo { get; set; }
        public Int16 TrnSeqNo { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
    }
}