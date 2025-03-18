using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICustomerService
    {
        #region Customer

        public Task<CustomerViewModelCount> GetCustomerListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CustomerViewModel> GetCustomerByIdAsync(short CompanyId, short UserId, int CustomerId, string CustomerCode, string CustomerName);

        public Task<SqlResponse> SaveCustomerAsync(short CompanyId, short UserId, M_Customer M_Customer);

        public Task<SqlResponse> DeleteCustomerAsync(short CompanyId, short UserId, int CustomerId);

        #endregion Customer

        #region Customer Address

        public Task<IEnumerable<CustomerAddressViewModel>> GetCustomerAddressByCustomerIdAsync(short CompanyId, short UserId, int CustomerId);

        public Task<CustomerAddressViewModel> GetCustomerAddressByIdAsync(short CompanyId, short UserId, int CustomerId, short AddressId);

        public Task<SqlResponse> SaveCustomerAddressAsync(short CompanyId, short UserId, M_CustomerAddress m_CustomerAddress);

        public Task<SqlResponse> DeleteCustomerAddressAsync(short CompanyId, short UserId, int CustomerId, short AddressId);

        #endregion Customer Address

        #region Customer Contact

        public Task<IEnumerable<CustomerContactViewModel>> GetCustomerContactByCustomerIdAsync(short CompanyId, short UserId, int CustomerId);

        public Task<CustomerContactViewModel> GetCustomerContactByIdAsync(short CompanyId, short UserId, int CustomerId, short ContactId);

        public Task<SqlResponse> SaveCustomerContactAsync(short CompanyId, short UserId, M_CustomerContact m_CustomerContact);

        public Task<SqlResponse> DeleteCustomerContactAsync(short CompanyId, short UserId, int CustomerId, short ContactId);

        #endregion Customer Contact
    }
}