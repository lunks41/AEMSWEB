using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICustomerContactService
    {
        public Task<IEnumerable<CustomerContactViewModel>> GetCustomerContactByCustomerIdAsync(short CompanyId, short UserId, int CustomerId);

        public Task<CustomerContactViewModel> GetCustomerContactByIdAsync(short CompanyId, short UserId, int CustomerId, short ContactId);

        public Task<SqlResponse> SaveCustomerContactAsync(short CompanyId, short UserId, M_CustomerContact m_CustomerContact);

        public Task<SqlResponse> DeleteCustomerContactAsync(short CompanyId, short UserId, int CustomerId, short ContactId);
    }
}