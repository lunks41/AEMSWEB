using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICustomerCreditLimitService
    {
        public Task<CustomerCreditLimitViewModelCount> GetCustomerCreditLimitListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_CustomerCreditLimit> GetCustomerCreditLimitByIdAsync(short CompanyId, short UserId, int CustomerId);

        //public Task<SqlResponse> SaveCustomerCreditLimitAsync( Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int16 UserId);

        public Task<SqlResponse> DeleteCustomerCreditLimitAsync(short CompanyId, short UserId, M_CustomerCreditLimit M_CustomerCreditLimit);
    }
}