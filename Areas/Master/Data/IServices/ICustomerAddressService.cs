using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICustomerAddressService
    {
        public Task<IEnumerable<CustomerAddressViewModel>> GetCustomerAddressByCustomerIdAsync(short CompanyId, short UserId, int CustomerId);

        public Task<CustomerAddressViewModel> GetCustomerAddressByIdAsync(short CompanyId, short UserId, int CustomerId, short AddressId);

        public Task<SqlResponse> SaveCustomerAddressAsync(short CompanyId, short UserId, M_CustomerAddress m_CustomerAddress);

        public Task<SqlResponse> DeleteCustomerAddressAsync(short CompanyId, short UserId, int CustomerId, short AddressId);
    }
}