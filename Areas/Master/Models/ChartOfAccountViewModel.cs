namespace AEMSWEB.Areas.Master.Models
{
    public class ChartOfAccountViewModel
    {
        public Int16 GLId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? GLCode { get; set; }
        public string? GLName { get; set; }
        public Int16 AccTypeId { get; set; }
        public string? AccTypeCode { get; set; }
        public string? AccTypeName { get; set; }
        public Int16 AccGroupId { get; set; }
        public string? AccGroupCode { get; set; }
        public string? AccGroupName { get; set; }
        public Int16 COACategoryId1 { get; set; }
        public string? COACategoryCode1 { get; set; }
        public string? COACategoryName1 { get; set; }
        public Int16 COACategoryId2 { get; set; }
        public string? COACategoryCode2 { get; set; }
        public string? COACategoryName2 { get; set; }
        public Int16 COACategoryId3 { get; set; }
        public string? COACategoryCode3 { get; set; }
        public string? COACategoryName3 { get; set; }
        public bool IsSysControl { get; set; }
        public Int16 SeqNo { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime? CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveChartOfAccountViewModel
    {
        public ChartOfAccountViewModel chartOfAccount { get; set; }
        public string? companyId { get; set; }
    }

    public class ChartOfAccountViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<ChartOfAccountViewModel> data { get; set; }
    }
}