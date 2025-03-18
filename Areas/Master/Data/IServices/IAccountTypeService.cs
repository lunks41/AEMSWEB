using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IAccountTypeService
    {
        public Task<AccountTypeViewModelCount> GetAccountTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<M_AccountType> GetAccountTypeByIdAsync(short CompanyId, short UserId, short AccTypeId);

        public Task<SqlResponse> SaveAccountTypeAsync(short CompanyId, short UserId, M_AccountType M_AccountType);

        public Task<SqlResponse> DeleteAccountTypeAsync(short CompanyId, short UserId, M_AccountType M_AccountType);
    }
}