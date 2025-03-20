using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IAccountSetupService
    {
        #region Header

        public Task<AccountSetupViewModelCount> GetAccountSetupListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AccountSetupViewModel> GetAccountSetupByIdAsync(short CompanyId, short UserId, short AccSetupId);

        public Task<SqlResponse> SaveAccountSetupAsync(short CompanyId, short UserId, M_AccountSetup M_AccountSetup);

        public Task<SqlResponse> DeleteAccountSetupAsync(short CompanyId, short UserId, short accSetupId);

        #endregion Header

        #region Details

        public Task<AccountSetupDtViewModelCount> GetAccountSetupDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AccountSetupDtViewModel> GetAccountSetupDtByIdAsync(short CompanyId, short UserId, short accSetupId, short currencyId, short gLId);

        public Task<SqlResponse> SaveAccountSetupDtAsync(short CompanyId, short UserId, M_AccountSetupDt m_AccountSetupDt);

        public Task<SqlResponse> DeleteAccountSetupDtAsync(short CompanyId, short UserId, short accSetupId, short currencyId, short gLId);

        #endregion Details

        #region AccountSetupCategory

        public Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<M_AccountSetupCategory> GetAccountSetupCategoryByIdAsync(short CompanyId, short UserId, short AccSetupCategoryId);

        public Task<SqlResponse> SaveAccountSetupCategoryAsync(short CompanyId, short UserId, M_AccountSetupCategory m_AccountSetupCategory);

        public Task<SqlResponse> DeleteAccountSetupCategoryAsync(short CompanyId, short UserId, M_AccountSetupCategory m_AccountSetupCategory);

        #endregion AccountSetupCategory
    }
}