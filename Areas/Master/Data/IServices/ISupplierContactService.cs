using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ISupplierContactService
    {
        public Task<IEnumerable<SupplierContactViewModel>> GetSupplierContactBySupplierIdAsync(short CompanyId, short UserId, int SupplierId);

        public Task<SupplierContactViewModel> GetSupplierContactByIdAsync(short CompanyId, short UserId, int SupplierId, short ContactId);

        public Task<SqlResponse> SaveSupplierContactAsync(short CompanyId, short UserId, M_SupplierContact M_SupplierContact);

        public Task<SqlResponse> DeleteSupplierContactAsync(short CompanyId, short UserId, int SupplierId, short ContactId);
    }
}