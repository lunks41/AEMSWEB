using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ITaxService
    {
        public Task<TaxViewModelCount> GetTaxListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_Tax> GetTaxByIdAsync(short CompanyId, short UserId, short TaxId);

        public Task<SqlResponse> SaveTaxAsync(short CompanyId, short UserId, M_Tax m_Tax);

        public Task<SqlResponse> DeleteTaxAsync(short CompanyId, short UserId, M_Tax m_Tax);

        public Task<TaxDtViewModelCount> GetTaxDtListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<TaxDtViewModel> GetTaxDtByIdAsync(short CompanyId, short UserId, short TaxId, DateTime ValidFrom);

        public Task<SqlResponse> SaveTaxDtAsync(short CompanyId, short UserId, M_TaxDt m_TaxDt);

        public Task<SqlResponse> DeleteTaxDtAsync(short CompanyId, short UserId, TaxDtViewModel m_TaxDt);
    }
}