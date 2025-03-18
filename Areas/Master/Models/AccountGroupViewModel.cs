namespace AEMSWEB.Models.Masters
{
    public class AccountGroupViewModel
    {
        public Int16 AccGroupId { get; set; }

        public Int16 CompanyId { get; set; }
        public string AccGroupCode { get; set; }
        public string AccGroupName { get; set; }
        public Int16 SeqNo { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveAccountGroupViewModel
    {
        public AccountGroupViewModel accountGroup { get; set; }
        public string CompanyId { get; set; }
    }

    public class AccountGroupViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<AccountGroupViewModel> data { get; set; }
    }
}