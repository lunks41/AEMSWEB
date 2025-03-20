using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ISupplierService
    {
        public Task<SupplierViewModelCount> GetSupplierListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<SupplierViewModel> GetSupplierByIdAsync(short CompanyId, short UserId, int SupplierId, string SupplierCode, string SupplierName);

        public Task<SqlResponse> SaveSupplierAsync(short CompanyId, short UserId, M_Supplier M_Supplier);

        public Task<SqlResponse> DeleteSupplierAsync(short CompanyId, short UserId, int SupplierId);

        public Task<IEnumerable<SupplierAddressViewModel>> GetSupplierAddressBySupplierIdAsync(short CompanyId, short UserId, int SupplierId);

        public Task<SupplierAddressViewModel> GetSupplierAddressByIdAsync(short CompanyId, short UserId, int SupplierId, short AddressId);

        public Task<SqlResponse> SaveSupplierAddressAsync(short CompanyId, short UserId, M_SupplierAddress m_SupplierAddress);

        public Task<SqlResponse> DeleteSupplierAddressAsync(short CompanyId, short UserId, int SupplierId, short AddressId);

        public Task<IEnumerable<SupplierContactViewModel>> GetSupplierContactBySupplierIdAsync(short CompanyId, short UserId, int SupplierId);

        public Task<SupplierContactViewModel> GetSupplierContactByIdAsync(short CompanyId, short UserId, int SupplierId, short ContactId);

        public Task<SqlResponse> SaveSupplierContactAsync(short CompanyId, short UserId, M_SupplierContact M_SupplierContact);

        public Task<SqlResponse> DeleteSupplierContactAsync(short CompanyId, short UserId, int SupplierId, short ContactId);
    }
}