using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ISupplierAddressService
    {
        public Task<IEnumerable<SupplierAddressViewModel>> GetSupplierAddressBySupplierIdAsync(short CompanyId, short UserId, int SupplierId);

        public Task<SupplierAddressViewModel> GetSupplierAddressByIdAsync(short CompanyId, short UserId, int SupplierId, short AddressId);

        public Task<SqlResponse> SaveSupplierAddressAsync(short CompanyId, short UserId, M_SupplierAddress m_SupplierAddress);

        public Task<SqlResponse> DeleteSupplierAddressAsync(short CompanyId, short UserId, int SupplierId, short AddressId);
    }
}