using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ITaxService
    {
        public Task<TaxViewModelCount> GetTaxListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<TaxViewModel> GetTaxByIdAsync(short CompanyId, short UserId, short TaxId);

        public Task<SqlResponse> SaveTaxAsync(short CompanyId, short UserId, M_Tax Tax);

        public Task<SqlResponse> DeleteTaxAsync(short CompanyId, short UserId, short TaxId);

        public Task<TaxDtViewModelCount> GetTaxDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<TaxDtViewModel> GetTaxDtByIdAsync(short CompanyId, short UserId, short TaxDtId, DateTime ValidFrom);

        public Task<SqlResponse> SaveTaxDtAsync(short CompanyId, short UserId, M_TaxDt m_TaxDt);

        public Task<SqlResponse> DeleteTaxDtAsync(short CompanyId, short UserId, short TaxId, DateTime ValidFrom);

        public Task<TaxCategoryViewModelCount> GetTaxCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<TaxCategoryViewModel> GetTaxCategoryByIdAsync(short CompanyId, short UserId, int TaxCategoryId);

        public Task<SqlResponse> SaveTaxCategoryAsync(short CompanyId, short UserId, M_TaxCategory m_TaxCategory);

        public Task<SqlResponse> DeleteTaxCategoryAsync(short CompanyId, short UserId, short taxCategoryId);
    }
}