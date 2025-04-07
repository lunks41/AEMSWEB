namespace AMESWEB.Models.Masters
{
    public class AccountTypeViewModel
    {
        public Int16 AccTypeId { get; set; }

        public Int16 CompanyId { get; set; }
        public string? AccTypeCode { get; set; }
        public string? AccTypeName { get; set; }
        public Int32 CodeStart { get; set; }
        public Int32 CodeEnd { get; set; }
        public Int16 SeqNo { get; set; }
        public string? AccGroupName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class SaveAccountTypeViewModel
    {
        public AccountTypeViewModel accountType { get; set; }
        public string? companyId { get; set; }
    }

    public class AccountTypeViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<AccountTypeViewModel> data { get; set; }
    }
}