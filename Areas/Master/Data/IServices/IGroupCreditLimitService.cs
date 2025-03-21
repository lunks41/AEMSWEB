using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IGroupCreditLimitService
    {
        public Task<GroupCreditLimitViewModelCount> GetGroupCreditLimitListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<GroupCreditLimitViewModel> GetGroupCreditLimitByIdAsync(short CompanyId, short UserId, short COACategoryId);

        public Task<SqlResponse> SaveGroupCreditLimitAsync(short CompanyId, short UserId, M_GroupCreditLimit M_GroupCreditLimit);

        public Task<SqlResponse> DeleteGroupCreditLimitAsync(short CompanyId, short UserId, M_GroupCreditLimit M_GroupCreditLimit);
    }
}