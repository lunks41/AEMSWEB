using AEMSWEB.Data;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Accounts;
using AEMSWEB.Repository;
using Dapper;
using System.Data;

namespace AEMSWEB.Services.Accounts
{
    public sealed class AccountService : IAccountService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public AccountService(IRepository<dynamic> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        //Document Id Generation
        public async Task<Int64> GenrateDocumentId(Int16 ModuleId, Int16 TransactionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@inModuleId", ModuleId, DbType.Int16);
            parameters.Add("@inTransactionId", TransactionId, DbType.Int16);
            parameters.Add("@OUTPUT_DOC_ID", dbType: DbType.Int64, direction: ParameterDirection.Output);
            return await _repository.ExecuteStoredProcedureAsync<Int64>("S_GENERATE_NUMBER_ID", parameters, "@OUTPUT_DOC_ID");
        }

        //Document Number Generation
        public async Task<string> GenrateDocumentNumber(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, DateTime AccountDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@inCompanyId", CompanyId, DbType.Int16);
            parameters.Add("@inModuleId", ModuleId, DbType.Int16);
            parameters.Add("@inTransactionId", TransactionId, DbType.Int16);
            parameters.Add("@inTrxDate", AccountDate, DbType.Date);
            parameters.Add("@OUTPUT_DOC_NO", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);
            return await _repository.ExecuteStoredProcedureAsync<string>("S_GENERATE_NUMBER", parameters, "@OUTPUT_DOC_NO");
        }

        //Upsert Transaction
        public async Task<string> CreateARStatement(Int16 CompanyId, Int16 UserId, Int64 DocumentId, Int16 TransactionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@inCompanyId", CompanyId, DbType.Int16);
            parameters.Add("@inUserId", UserId, DbType.Int32);
            parameters.Add("@inDocumentId", DocumentId, DbType.Int64);
            parameters.Add("@inTransactionId", TransactionId, DbType.Int16);
            return await _repository.ExecuteStoredProcedureAsync<string>("FIN_AR_CreateStatement", parameters, "");
        }

        public async Task<dynamic> GetARAPPaymentHistoryListAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId)
        {
            return await _repository.GetQueryAsync<dynamic>($"exec FIN_AR_AP_PaymentHistory {CompanyId},{ModuleId},{TransactionId},{DocumentId}");
        }

        public async Task<dynamic> GetGLPostingHistoryListAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int64 DocumentId)
        {
            return await _repository.GetQueryAsync<dynamic>($"exec FIN_GL_PostingHistory {CompanyId},{ModuleId},{TransactionId},{DocumentId}");
        }

        public async Task<dynamic> GetCustomerInvoiceListAsyn(Int16 CompanyId, Int32 CustomerId, Int32 CurrencyId)
        {
            return await _repository.GetQueryAsync<dynamic>($"SELECT InvoiceId,InvoiceNo,ReferenceNo,AccountDate,CurrencyId,ExhRate,TotAmt,TotLocalAmt,TotCtyAmt,GstAmt,GstLocalAmt,GstCtyAmt,TotAmtAftGst,TotLocalAmtAftGst FROM dbo.ArInvoiceHd WHERE CustomerId={CustomerId} AND CurrencyId={CurrencyId} AND CompanyId={CompanyId}");
        }

        public async Task<dynamic> GetCustomerInvoiceAsyn(Int16 CompanyId, Int32 CustomerId, Int32 CurrencyId, string InvoiceNo)
        {
            return await _repository.GetQuerySingleOrDefaultAsync<dynamic>($"SELECT InvoiceId,InvoiceNo,ReferenceNo,AccountDate,CurrencyId,ExhRate,TotAmt,TotLocalAmt,TotCtyAmt,GstAmt,GstLocalAmt,GstCtyAmt,TotAmtAftGst,TotLocalAmtAftGst FROM dbo.ArInvoiceHd WHERE CustomerId={CustomerId} AND CurrencyId={CurrencyId} AND CompanyId={CompanyId} AND InvoiceNo='{InvoiceNo}'");
        }

        public async Task<bool> GetGlPeriodCloseAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, string PrevAccountDate, string AccountDate)
        {
            bool IsPeriodClosed = false;

            if (PrevAccountDate != AccountDate)
            {
                IsPeriodClosed = await _repository.GetQuerySingleOrDefaultAsync<bool>($"SELECT dbo.CheckPeriodClosed({CompanyId},{ModuleId},'{PrevAccountDate}') as IsExist");
                if (!IsPeriodClosed)
                {
                    IsPeriodClosed = await _repository.GetQuerySingleOrDefaultAsync<bool>($"SELECT dbo.CheckPeriodClosed({CompanyId},{ModuleId},'{AccountDate}') as IsExist");
                }
            }
            else
            {
                IsPeriodClosed = await _repository.GetQuerySingleOrDefaultAsync<bool>($"SELECT dbo.CheckPeriodClosed({CompanyId},{ModuleId},'{AccountDate}') as IsExist");
            }

            return IsPeriodClosed;
        }
    }
}