using AEMSWEB.Areas.Account.Models;

namespace AEMSWEB.IServices.Accounts.AP
{
    public interface IAPTransactionService
    {
        public Task<IEnumerable<GetOutstandTransactionViewModel>> GetAPOutstandTransactionListAsync(Int16 CompanyId, GetTransactionViewModel getTransactionViewModel, Int16 UserId);
    }
}