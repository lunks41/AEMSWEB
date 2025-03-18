using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICustomerService
    {
        public Task<CustomerViewModelCount> GetCustomerListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CustomerViewModel> GetCustomerAsync(short CompanyId, short UserId, int CustomerId, string CustomerCode, string CustomerName);

        public Task<SqlResponse> SaveCustomerAsync(short CompanyId, short UserId, M_Customer M_Customer);

        public Task<SqlResponse> DeleteCustomerAsync(short CompanyId, short UserId, int CustomerId);
    }
}