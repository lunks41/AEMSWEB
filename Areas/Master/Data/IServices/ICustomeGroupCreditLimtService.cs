using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICustomerGroupCreditLimitService
    {
        public Task<CustomerGroupCreditLimitViewModelCount> GetCustomerGroupCreditLimitListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<M_CustomerGroupCreditLimit> GetCustomerGroupCreditLimitByIdAsync(short CompanyId, short UserId, short GroupCreditLimitId);

        public Task<SqlResponse> SaveCustomerGroupCreditLimitAsync(short CompanyId, short UserId, M_CustomerGroupCreditLimit m_CustomerGroupCreditLimit);

        public Task<SqlResponse> DeleteCustomerGroupCreditLimitAsync(short CompanyId, short UserId, M_CustomerGroupCreditLimit M_CustomerGroupCreditLimit);
    }
}