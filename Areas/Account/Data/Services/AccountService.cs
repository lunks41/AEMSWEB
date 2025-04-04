﻿using AMESWEB.Areas.Account.Data.IServices;
using AMESWEB.Data;
using AMESWEB.IServices;
using AMESWEB.Repository;
using Dapper;
using System.Data;

namespace AMESWEB.Areas.Account.Data.Services
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
        public async Task<long> GenrateDocumentId(short ModuleId, short TransactionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@inModuleId", ModuleId, DbType.Int16);
            parameters.Add("@inTransactionId", TransactionId, DbType.Int16);
            parameters.Add("@OUTPUT_DOC_ID", dbType: DbType.Int64, direction: ParameterDirection.Output);
            return await _repository.ExecuteStoredProcedureAsync<long>("S_GENERATE_NUMBER_ID", parameters, "@OUTPUT_DOC_ID");
        }

        //Document Number Generation
        public async Task<string> GenrateDocumentNumber(short CompanyId, short ModuleId, short TransactionId, DateTime AccountDate)
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
        public async Task<string> CreateARStatement(short CompanyId, short UserId, long DocumentId, short TransactionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@inCompanyId", CompanyId, DbType.Int16);
            parameters.Add("@inUserId", UserId, DbType.Int32);
            parameters.Add("@inDocumentId", DocumentId, DbType.Int64);
            parameters.Add("@inTransactionId", TransactionId, DbType.Int16);
            return await _repository.ExecuteStoredProcedureAsync<string>("FIN_AR_CreateStatement", parameters, "");
        }

        public async Task<dynamic> GetARAPPaymentHistoryListAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId)
        {
            return await _repository.GetQueryAsync<dynamic>($"exec FIN_AR_AP_PaymentHistory {CompanyId},{ModuleId},{TransactionId},{DocumentId}");
        }

        public async Task<dynamic> GetGLPostingHistoryListAsync(short CompanyId, short ModuleId, short TransactionId, long DocumentId)
        {
            return await _repository.GetQueryAsync<dynamic>($"exec FIN_GL_PostingHistory {CompanyId},{ModuleId},{TransactionId},{DocumentId}");
        }

        public async Task<dynamic> GetCustomerInvoiceListAsyn(short CompanyId, int CustomerId, int CurrencyId)
        {
            return await _repository.GetQueryAsync<dynamic>($"SELECT InvoiceId,InvoiceNo,ReferenceNo,AccountDate,CurrencyId,ExhRate,TotAmt,TotLocalAmt,TotCtyAmt,GstAmt,GstLocalAmt,GstCtyAmt,TotAmtAftGst,TotLocalAmtAftGst FROM dbo.ArInvoiceHd WHERE CustomerId={CustomerId} AND CurrencyId={CurrencyId} AND CompanyId={CompanyId}");
        }

        public async Task<dynamic> GetCustomerInvoiceAsyn(short CompanyId, int CustomerId, int CurrencyId, string InvoiceNo)
        {
            return await _repository.GetQuerySingleOrDefaultAsync<dynamic>($"SELECT InvoiceId,InvoiceNo,ReferenceNo,AccountDate,CurrencyId,ExhRate,TotAmt,TotLocalAmt,TotCtyAmt,GstAmt,GstLocalAmt,GstCtyAmt,TotAmtAftGst,TotLocalAmtAftGst FROM dbo.ArInvoiceHd WHERE CustomerId={CustomerId} AND CurrencyId={CurrencyId} AND CompanyId={CompanyId} AND InvoiceNo='{InvoiceNo}'");
        }

        public async Task<bool> GetGlPeriodCloseAsync(short CompanyId, short ModuleId, short TransactionId, string PrevAccountDate, string AccountDate)
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