using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IAccountSetupService
    {
        #region Header

        public Task<AccountSetupViewModelCount> GetAccountSetupListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AccountSetupViewModel> GetAccountSetupByIdAsync(short CompanyId, short UserId, short AccSetupId);

        public Task<SqlResponce> SaveAccountSetupAsync(short CompanyId, short UserId, M_AccountSetup M_AccountSetup);

        public Task<SqlResponce> DeleteAccountSetupAsync(short CompanyId, short UserId, short accSetupId);

        #endregion Header

        #region Details

        public Task<AccountSetupDtViewModelCount> GetAccountSetupDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AccountSetupDtViewModel> GetAccountSetupDtByIdAsync(short CompanyId, short UserId, short accSetupId, short currencyId, short glId);

        public Task<SqlResponce> SaveAccountSetupDtAsync(short CompanyId, short UserId, M_AccountSetupDt m_AccountSetupDt);

        public Task<SqlResponce> DeleteAccountSetupDtAsync(short CompanyId, short UserId, short accSetupId, short currencyId, short glId);

        #endregion Details

        #region AccountSetupCategory

        public Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AccountSetupCategoryViewModel> GetAccountSetupCategoryByIdAsync(short CompanyId, short UserId, short AccSetupCategoryId);

        public Task<SqlResponce> SaveAccountSetupCategoryAsync(short CompanyId, short UserId, M_AccountSetupCategory m_AccountSetupCategory);

        public Task<SqlResponce> DeleteAccountSetupCategoryAsync(short CompanyId, short UserId, short AccSetupCategoryId);

        #endregion AccountSetupCategory
    }
}