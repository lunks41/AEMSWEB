namespace AMESWEB.Models.Masters
{
    public class DocumentTypeViewModel
    {
        public Int16 DocTypeId { get; set; }
        public Int16 CompanyId { get; set; }
        public string? DocTypeCode { get; set; }
        public string? DocTypeName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public Int16? CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }

    public class DocumentTypeViewModelCount
    {
        public Int16 responseCode { get; set; }
        public string? responseMessage { get; set; }
        public Int64 totalRecords { get; set; }
        public List<DocumentTypeViewModel> data { get; set; }
    }
}