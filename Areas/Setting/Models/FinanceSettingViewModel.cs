namespace AEMSWEB.Areas.Setting.Models
{
    public class FinanceSettingViewModel
    {
        public short Base_CurrencyId { get; set; }
        public short Local_CurrencyId { get; set; }
        public int ExhGainLoss_GlId { get; set; }
        public int BankCharge_GlId { get; set; }
        public int ProfitLoss_GlId { get; set; }
        public int RetEarning_GlId { get; set; }
        public int SaleGst_GlId { get; set; }
        public int PurGst_GlId { get; set; }
        public int SaleDef_GlId { get; set; }
        public int PurDef_GlId { get; set; }
    }
}