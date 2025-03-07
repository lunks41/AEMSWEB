using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ISupplierService
    {
        public Task<SupplierViewModelCount> GetSupplierListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<SupplierViewModel> GetSupplierAsync(short CompanyId, short UserId, int SupplierId, string SupplierCode, string SupplierName);

        public Task<SqlResponse> SaveSupplierAsync(short CompanyId, short UserId, M_Supplier M_Supplier);

        public Task<SqlResponse> DeleteSupplierAsync(short CompanyId, short UserId, int SupplierId);
    }
}