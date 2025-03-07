namespace AEMSWEB.IServices.Setting
{
    public interface IBaseSettingsService
    {
        public Task<decimal> GetExchangeRateAsync(Int16 CompanyId, Int16 CurrencyId, string TrnsDate, Int16 UserId);

        public Task<decimal> GetExchangeRateLocalAsync(Int16 CompanyId, Int16 CurrencyId, string TrnsDate, Int16 UserId);

        public Task<bool> GetCheckPeriodClosedAsync(Int16 CompanyId, Int16 ModuleId, string TrnsDate, Int16 UserId);

        public Task<decimal> GetGstPercentageAsync(Int16 CompanyId, Int16 GstId, string TrnsDate, Int16 UserId);

        public Task<decimal> GetCreditTermDayAsync(Int16 CompanyId, Int16 CreditTermId, string TrnsDate, Int16 UserId);
    }
}