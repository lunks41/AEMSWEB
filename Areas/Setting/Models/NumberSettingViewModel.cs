namespace AMESWEB.Areas.Setting.Models
{
    public class NumberSettingViewModel
    {
        public int NumberId { get; set; }
        public short CompanyId { get; set; }
        public short ModuleId { get; set; }
        public short TransactionId { get; set; }
        public string? Prefix { get; set; }
        public short PrefixSeq { get; set; }
        public string? PrefixDelimiter { get; set; }
        public bool IncludeYear { get; set; }
        public short YearSeq { get; set; }
        public string? YearFormat { get; set; }
        public string? YearDelimiter { get; set; }
        public bool IncludeMonth { get; set; }
        public short MonthSeq { get; set; }
        public string? MonthFormat { get; set; }
        public string? MonthDelimiter { get; set; }
        public short NoDIgits { get; set; }
        public short DIgitSeq { get; set; }
        public bool ResetYearly { get; set; }
        public short CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public short? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string? CreateBy { get; set; }
        public string? EditBy { get; set; }
    }
}