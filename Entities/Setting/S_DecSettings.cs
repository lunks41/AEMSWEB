using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Entities.Setting
{
    public class S_DecSettings
    {
        [Key]
        public Int16 CompanyId { get; set; }

        public Int16 AmtDec { get; set; }
        public Int16 LocAmtDec { get; set; }
        public Int16 CtyAmtDec { get; set; }
        public Int16 PriceDec { get; set; }
        public Int16 QtyDec { get; set; }
        public Int16 ExhRateDec { get; set; }
        public string DateFormat { get; set; }
        public string LongDateFormat { get; set; }
        public Int16 CreateById { get; set; }

        [NotMapped]
        public DateTime CreateDate { get; set; }

        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
    }
}