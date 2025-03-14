using AEMSWEB.Areas.Account.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPTransactionService
    {
        public Task<IEnumerable<GetOutstandTransactionViewModel>> GetAPOutstandTransactionListAsync(short CompanyId, GetTransactionViewModel getTransactionViewModel, short UserId);
    }
}