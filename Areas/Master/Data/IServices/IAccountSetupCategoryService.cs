using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IAccountSetupCategoryService
    {
        public Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_AccountSetupCategory> GetAccountSetupCategoryByIdAsync(short CompanyId, short UserId, short AccSetupCategoryId);

        public Task<SqlResponse> SaveAccountSetupCategoryAsync(short CompanyId, short UserId, M_AccountSetupCategory m_AccountSetupCategory);

        public Task<SqlResponse> DeleteAccountSetupCategoryAsync(short CompanyId, short UserId, M_AccountSetupCategory m_AccountSetupCategory);
    }
}