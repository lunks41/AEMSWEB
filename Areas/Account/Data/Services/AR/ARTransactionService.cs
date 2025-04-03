using AMESWEB.Areas.Account.Data.IServices;
using AMESWEB.Areas.Account.Data.IServices.AR;
using AMESWEB.Areas.Account.Models;
using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Repository;

namespace AMESWEB.Areas.Account.Data.Services.AR
{
    public sealed class ARTransactionService : IARTransactionService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public ARTransactionService(IRepository<dynamic> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<IEnumerable<GetOutstandTransactionViewModel>> GetAROutstandTransactionListAsync(short CompanyId, GetTransactionViewModel getTransactionViewModel, short UserId)
        {
            try
            {
                var productDetails = await _repository.GetQueryAsync<GetOutstandTransactionViewModel>($"exec FIN_AR_GetOutstandTransactions {CompanyId},{getTransactionViewModel.CustomerId},{getTransactionViewModel.CurrencyId},'{getTransactionViewModel.DocumentId}',{getTransactionViewModel.IsRefund},{UserId}");

                return productDetails;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Receipt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "ARTransaction",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}