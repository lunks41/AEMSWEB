namespace AEMSWEB.Models.Admin
{
    public class TransactionViewModel
    {
        public Int16 TransactionId { get; set; }
        public string? TransactionCode { get; set; }
        public string? TransactionName { get; set; }
        public Int16 ModuleId { get; set; }
        public string? ModuleCode { get; set; }
        public string? ModuleName { get; set; }
        public Int32 TransCategoryId { get; set; }
        public string? TransCategoryCode { get; set; }
        public string? TransCategoryName { get; set; }
        public Int16 SeqNo { get; set; }
        public Int32 TransCatSeqNo { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
    }

    public class GroupViewModelCount
    {
        public List<GroupViewModel> group { get; set; }
    }

    public class GroupViewModel
    {
        public string? id { get; set; }
        public string? groupLabel { get; set; }
        public List<MenuViewModel> menus { get; set; }
    }

    public class MenuViewModel
    {
        public string? id { get; set; }
        public string? href { get; set; }
        public string? label { get; set; }
        public bool active { get; set; }
        public string? icon { get; set; }
        public List<SubMenuViewModel> submenus { get; set; }
    }

    public class SubMenuViewModel
    {
        public string? href { get; set; }
        public string? label { get; set; }
        public bool active { get; set; }
        public string? icon { get; set; }
    }
}