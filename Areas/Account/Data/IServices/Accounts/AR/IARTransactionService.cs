using AEMSWEB.Areas.Account.Models;

namespace AEMSWEB.IServices.Accounts.AR
{
    public interface IARTransactionService
    {
        public Task<IEnumerable<GetOutstandTransactionViewModel>> GetAROutstandTransactionListAsync(Int16 CompanyId, GetTransactionViewModel getTransactionViewModel, Int16 UserId);
    }
}