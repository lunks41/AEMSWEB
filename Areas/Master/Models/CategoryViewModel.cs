namespace AEMSWEB.Models.Masters
{
    public class CategoryViewModel
    {
        public Int16 CategoryId { get; set; }
        public Int16 CompanyId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveCategoryViewModel
    {
        public CategoryViewModel Category { get; set; }
        public string CompanyId { get; set; }
    }

    public class CategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<CategoryViewModel> data { get; set; }
    }

    public class SubCategoryViewModel
    {
        public Int16 SubCategoryId { get; set; }
        public Int16 CompanyId { get; set; }
        public string SubCategoryCode { get; set; }
        public string SubCategoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SubCategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<SubCategoryViewModel> data { get; set; }
    }
}