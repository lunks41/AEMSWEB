using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IAccountGroupService
    {
        public Task<AccountGroupViewModelCount> GetAccountGroupListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AccountGroupViewModel> GetAccountGroupByIdAsync(short CompanyId, short UserId, short AccGroupId);

        public Task<SqlResponse> SaveAccountGroupAsync(short CompanyId, short UserId, M_AccountGroup m_AccountGroup);

        public Task<SqlResponse> DeleteAccountGroupAsync(short CompanyId, short UserId, short accGroupId);
    }
}