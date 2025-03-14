using AEMSWEB.Areas.Account.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AR
{
    public interface IARTransactionService
    {
        public Task<IEnumerable<GetOutstandTransactionViewModel>> GetAROutstandTransactionListAsync(short CompanyId, GetTransactionViewModel getTransactionViewModel, short UserId);
    }
}