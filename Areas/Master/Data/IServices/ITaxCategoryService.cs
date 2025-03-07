using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ITaxCategoryService
    {
        public Task<TaxCategoryViewModelCount> GetTaxCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_TaxCategory> GetTaxCategoryByIdAsync(short CompanyId, short UserId, short TaxCategoryId);

        public Task<SqlResponse> SaveTaxCategoryAsync(short CompanyId, short UserId, M_TaxCategory m_TaxCategory);

        public Task<SqlResponse> DeleteTaxCategoryAsync(short CompanyId, short UserId, M_TaxCategory m_TaxCategory);
    }
}