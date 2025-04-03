using AMESWEB.Models.Admin;

namespace AMESWEB.Areas.Admin.Data
{
    public interface ITransactionService
    {
        public Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAsync(short CompanyId, short ModuleId, short UserId);

        public Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAllAsync(short CompanyId, short UserId);

        public Task<List<GroupViewModel>> GetMenuListAsync(short CompanyId, short UserId);
    }
}