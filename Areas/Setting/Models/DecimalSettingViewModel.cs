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
    }
}