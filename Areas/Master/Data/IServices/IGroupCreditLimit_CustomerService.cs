using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IGroupCreditLimit_CustomerService
    {
        public Task<GroupCreditLimit_CustomerViewModelCount> GetGroupCreditLimit_CustomerListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_GroupCreditLimit_Customer> GetGroupCreditLimit_CustomerByIdAsync(short CompanyId, short UserId, short GroupCreditLimitId);

        public Task<SqlResponse> SaveGroupCreditLimit_CustomerAsync(short CompanyId, short UserId, M_GroupCreditLimit_Customer M_GroupCreditLimit_Customer);

        public Task<SqlResponse> DeleteGroupCreditLimit_CustomerAsync(short CompanyId, short UserId, M_GroupCreditLimit_Customer M_GroupCreditLimit_Customer);
    }
}