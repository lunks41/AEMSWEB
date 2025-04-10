using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IGroupCreditLimit_CustomerService
    {
        public Task<GroupCreditLimit_CustomerViewModelCount> GetGroupCreditLimit_CustomerListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<GroupCreditLimit_CustomerViewModel> GetGroupCreditLimit_CustomerByIdAsync(short CompanyId, short UserId, short GroupCreditLimitId);

        public Task<SqlResponce> SaveGroupCreditLimit_CustomerAsync(short CompanyId, short UserId, M_GroupCreditLimit_Customer M_GroupCreditLimit_Customer);

        public Task<SqlResponce> DeleteGroupCreditLimit_CustomerAsync(short CompanyId, short UserId, M_GroupCreditLimit_Customer M_GroupCreditLimit_Customer);
    }
}