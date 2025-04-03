namespace AMESWEB.Models.Admin
{
    public class ErrorLogViewModel
    {
        public Int64 ErrId { get; set; }
        public Int16 CompanyId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public Int64 DocumentId { get; set; }
        public string? DocumentNo { get; set; }
        public string? TblName { get; set; }
        public Int16 ModeId { get; set; }
        public string? Remarks { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
    }
}