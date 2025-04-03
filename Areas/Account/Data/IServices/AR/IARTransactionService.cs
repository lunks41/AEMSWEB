using AMESWEB.Areas.Account.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AR
{
    public interface IARTransactionService
    {
        public Task<IEnumerable<GetOutstandTransactionViewModel>> GetAROutstandTransactionListAsync(short CompanyId, GetTransactionViewModel getTransactionViewModel, short UserId);
    }
}