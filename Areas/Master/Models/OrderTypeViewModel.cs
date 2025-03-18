namespace AEMSWEB.Models.Masters
{
    public class OrderTypeViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 OrderTypeId { get; set; }
        public string OrderTypeCode { get; set; }
        public string OrderTypeName { get; set; }
        public Int16 OrderTypeCategoryId { get; set; }
        public string OrderTypeCategoryCode { get; set; }
        public string OrderTypeCategoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveOrderTypeViewModel
    {
        public OrderTypeViewModel orderType { get; set; }
        public string companyId { get; set; }
    }

    public class OrderTypeCategoryViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 OrderTypeCategoryId { get; set; }
        public string OrderTypeCategoryCode { get; set; }
        public string OrderTypeCategoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveOrderTypeCategoryViewModel
    {
        public OrderTypeCategoryViewModel orderTypeCategory { get; set; }
        public string companyId { get; set; }
    }

    public class OrderTypeViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<OrderTypeViewModel> data { get; set; }
    }

    public class OrderTypeCategoryViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<OrderTypeCategoryViewModel> data { get; set; }
    }
}