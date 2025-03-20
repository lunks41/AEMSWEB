namespace AEMSWEB.Models.Admin
{
    public class DocumentViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 TransactionId { get; set; }
        public Int16 ModuleId { get; set; }
        public string? DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public Int32 ItemNo { get; set; }
        public Int16 DocTypeId { get; set; }
        public string? DocTypeCode { get; set; }
        public string? DocTypeName { get; set; }
        public string? DocPath { get; set; }
        public string? Remarks { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public string? CreateBy { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? EditBy { get; set; }
    }
}