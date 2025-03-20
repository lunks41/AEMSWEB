using Microsoft.EntityFrameworkCore;

namespace AEMSWEB.Models
{
    [PrimaryKey(nameof(UserGroupId), nameof(ModuleId), nameof(TransactionId))]
    public class AdmUserGroupRights
    {
        public Int16 UserGroupId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }

    public class UserTransactionRights
    {
        public short ModuleId { get; set; }
        public string? ModuleCode { get; set; }
        public string? ModuleName { get; set; }
        public short TransactionId { get; set; }
        public string? TransactionCode { get; set; }
        public string? TransactionName { get; set; }
        public int TransCategoryId { get; set; }
        public string? TransCategoryCode { get; set; }
        public string? TransCategoryName { get; set; }
        public short SeqNo { get; set; }
        public short TransCatSeqNo { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
    }
}