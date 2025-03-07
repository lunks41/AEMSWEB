namespace AEMSWEB.Models
{
    public class ModuleView
    {
        public int ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public List<TransCategoryView> TransCategorys { get; set; }
    }

    public class TransCategoryView
    {
        public Int32 TransCategoryId { get; set; }
        public string TransCategoryCode { get; set; }
        public string TransCategoryName { get; set; }
        public List<TransactionView> Transactions { get; set; }
    }

    public class TransactionView
    {
        public int TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
    }

    public class TransactionViewModel
    {
        public Int16 TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionName { get; set; }
        public Int16 ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public Int32 TransCategoryId { get; set; }
        public string TransCategoryCode { get; set; }
        public string TransCategoryName { get; set; }
        public Int16 SeqNo { get; set; }
        public Int32 TransCatSeqNo { get; set; }
        public bool IsRead { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsExport { get; set; }
        public bool IsPrint { get; set; }
    }
}