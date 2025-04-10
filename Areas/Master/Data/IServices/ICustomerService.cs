using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ICustomerService
    {
        #region Customer

        public Task<CustomerViewModelCount> GetCustomerListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CustomerViewModel> GetCustomerByIdAsync(short CompanyId, short UserId, int CustomerId, string CustomerCode, string CustomerName);

        public Task<SqlResponce> SaveCustomerAsync(short CompanyId, short UserId, M_Customer M_Customer);

        public Task<SqlResponce> DeleteCustomerAsync(short CompanyId, short UserId, int CustomerId);

        #endregion Customer

        #region Customer Address

        public Task<IEnumerable<CustomerAddressViewModel>> GetCustomerAddressByCustomerIdAsync(short CompanyId, short UserId, int CustomerId);

        public Task<CustomerAddressViewModel> GetCustomerAddressByIdAsync(short CompanyId, short UserId, int CustomerId, short AddressId);

        public Task<SqlResponce> SaveCustomerAddressAsync(short CompanyId, short UserId, M_CustomerAddress m_CustomerAddress);

        public Task<SqlResponce> DeleteCustomerAddressAsync(short CompanyId, short UserId, int CustomerId, short AddressId);

        #endregion Customer Address

        #region Customer Contact

        public Task<IEnumerable<CustomerContactViewModel>> GetCustomerContactByCustomerIdAsync(short CompanyId, short UserId, int CustomerId);

        public Task<CustomerContactViewModel> GetCustomerContactByIdAsync(short CompanyId, short UserId, int CustomerId, short ContactId);

        public Task<SqlResponce> SaveCustomerContactAsync(short CompanyId, short UserId, M_CustomerContact m_CustomerContact);

        public Task<SqlResponce> DeleteCustomerContactAsync(short CompanyId, short UserId, int CustomerId, short ContactId);

        #endregion Customer Contact
    }
}