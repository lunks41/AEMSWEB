namespace AEMSWEB.Areas.Setting.Models
{
    public class UserGridViewModel
    {
        public short CompanyId { get; set; }
        public short UserId { get; set; }
        public short ModuleId { get; set; }
        public short TransactionId { get; set; }
        public string? GrdName { get; set; }
        public string? GrdKey { get; set; }
        public string? GrdColVisible { get; set; }
        public string? GrdColOrder { get; set; }
        public string? GrdColSize { get; set; }
        public string? GrdSort { get; set; }
        public string? GrdString { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }
}