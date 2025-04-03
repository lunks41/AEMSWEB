using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ISupplierBankService
    {
        public Task<IEnumerable<SupplierBankViewModel>> GetSupplierBankBySupplierIdAsync(short CompanyId, short UserId, int SupplierId);

        public Task<SupplierBankViewModel> GetSupplierBankByIdAsync(short CompanyId, short UserId, int SupplierId, short SupplierBankId);

        public Task<SqlResponse> SaveSupplierBankAsync(short CompanyId, short UserId, M_SupplierBank m_SupplierBank);

        public Task<SqlResponse> DeleteSupplierBankAsync(short CompanyId, short UserId, int SupplierId, short BaSupplierBankIdnkId);
    }
}