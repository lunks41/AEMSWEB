using AEMSWEB.Models.Masters;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMSWEB.Areas.Setting.Models
{
    public class DecimalSettingViewModel
    {
        public short AmtDec { get; set; }
        public short LocAmtDec { get; set; }
        public short CtyAmtDec { get; set; }
        public short PriceDec { get; set; }
        public short QtyDec { get; set; }
        public short ExhRateDec { get; set; }
        public string DateFormat { get; set; }
        public string LongDateFormat { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }

    public class SaveDecimalSettingViewModel
    {
        public DecimalSettingViewModel DecimalSetting { get; set; }
        public string CompanyId { get; set; }
    }
}