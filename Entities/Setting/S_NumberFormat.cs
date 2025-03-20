using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Setting
{
    public class S_NumberFormat
    {
        [Key]
        public Int32 NumberId { get; set; }

        public Int16 CompanyId { get; set; }
        public Int16 ModuleId { get; set; }
        public Int16 TransactionId { get; set; }
        public string? Prefix { get; set; }
        public Int16 PrefixSeq { get; set; }
        public string? PrefixDelimiter { get; set; }
        public bool IncludeYear { get; set; }
        public Int16 YearSeq { get; set; }
        public string? YearFormat { get; set; }
        public string? YearDelimiter { get; set; }
        public bool IncludeMonth { get; set; }
        public Int16 MonthSeq { get; set; }
        public string? MonthFormat { get; set; }
        public string? MonthDelimiter { get; set; }
        public Int16 NoDIgits { get; set; }
        public Int16 DIgitSeq { get; set; }
        public bool ResetYearly { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}