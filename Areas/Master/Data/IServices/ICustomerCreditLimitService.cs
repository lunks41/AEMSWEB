using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ICustomerCreditLimitService
    {
        public Task<CustomerCreditLimitViewModelCount> GetCustomerCreditLimitListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CustomerCreditLimitViewModel> GetCustomerCreditLimitByIdAsync(short CompanyId, short UserId, int CustomerId);

        //public Task<SqlResponce> SaveCustomerCreditLimitAsync( Int16 CompanyId, M_CustomerCreditLimit M_CustomerCreditLimit, Int16 UserId);

        public Task<SqlResponce> DeleteCustomerCreditLimitAsync(short CompanyId, short UserId, M_CustomerCreditLimit M_CustomerCreditLimit);
    }
}