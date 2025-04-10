using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ICustomerGroupCreditLimitService
    {
        public Task<CustomerGroupCreditLimitViewModelCount> GetCustomerGroupCreditLimitListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CustomerGroupCreditLimitViewModel> GetCustomerGroupCreditLimitByIdAsync(short CompanyId, short UserId, short GroupCreditLimitId);

        public Task<SqlResponce> SaveCustomerGroupCreditLimitAsync(short CompanyId, short UserId, M_CustomerGroupCreditLimit m_CustomerGroupCreditLimit);

        public Task<SqlResponce> DeleteCustomerGroupCreditLimitAsync(short CompanyId, short UserId, M_CustomerGroupCreditLimit M_CustomerGroupCreditLimit);
    }
}