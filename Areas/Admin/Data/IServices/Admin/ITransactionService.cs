using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Masters
{
    public interface ITransactionService
    {
        public Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAsync(Int16 CompanyId, Int16 ModuleId, Int16 UserId);

        public Task<IEnumerable<TransactionViewModel>> GetUsersTransactionsAllAsync(Int16 CompanyId, Int16 UserId);

        public Task<List<GroupViewModel>> GetMenuListAsync(Int16 CompanyId, Int16 UserId);
    }
}