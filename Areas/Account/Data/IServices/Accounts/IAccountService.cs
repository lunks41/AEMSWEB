namespace AEMSWEB.IServices.Accounts
{
    public interface IAccountService
    {
        public Task<Int64> GenrateDocumentId(Int16 ModuleId, Int16 TransactionId);

        public Task<string> GenrateDocumentNumber(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, DateTime AccountDate);

        public Task<dynamic> GetARAPPaymentHistoryListAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId);

        public Task<bool> GetGlPeriodCloseAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, string PrevAccountDate, string AccountDate);

        //public Task<dynamic> GetHistoryVersionListAsync( Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId);

        public Task<dynamic> GetGLPostingHistoryListAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId);

        public Task<dynamic> GetCustomerInvoiceListAsyn(Int16 CompanyId, Int32 CustomerId, Int32 CurrencyId);

        public Task<dynamic> GetCustomerInvoiceAsyn(Int16 CompanyId, Int32 CustomerId, Int32 CurrencyId, string InvoiceNo);
    }
}