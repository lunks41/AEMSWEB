using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ISubCategoryService
    {
        public Task<SubCategoryViewModelCount> GetSubCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_SubCategory> GetSubCategoryByIdAsync(short CompanyId, short UserId, short SubCategoryId);

        public Task<SqlResponse> SaveSubCategoryAsync(short CompanyId, short UserId, M_SubCategory m_SubCategory);

        public Task<SqlResponse> DeleteSubCategoryAsync(short CompanyId, short UserId, M_SubCategory m_SubCategory);
    }
}