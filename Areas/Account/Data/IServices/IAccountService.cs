namespace AEMSWEB.Areas.Account.Data.IServices
{
    public interface IAccountService
    {
        public Task<long> GenrateDocumentId(short ModuleId, short TransactionId);

        public Task<string> GenrateDocumentNumber(short CompanyId, short ModuleId, short TransactionId, DateTime AccountDate);

        public Task<dynamic> GetARAPPaymentHistoryListAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId);

        public Task<bool> GetGlPeriodCloseAsync(short CompanyId, short ModuleId, short TransactionId, string PrevAccountDate, string AccountDate);

        //public Task<dynamic> GetHistoryVersionListAsync( Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId);

        public Task<dynamic> GetGLPostingHistoryListAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId);

        public Task<dynamic> GetCustomerInvoiceListAsyn(short CompanyId, int CustomerId, int CurrencyId);

        public Task<dynamic> GetCustomerInvoiceAsyn(short CompanyId, int CustomerId, int CurrencyId, string InvoiceNo);
    }
}