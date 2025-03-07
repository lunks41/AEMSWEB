using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICategoryService
    {
        public Task<CategoryViewModelCount> GetCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_Category> GetCategoryByIdAsync(short CompanyId, short UserId, short COACategoryId);

        public Task<SqlResponse> SaveCategoryAsync(short CompanyId, short UserId, M_Category M_Category);

        public Task<SqlResponse> DeleteCategoryAsync(short CompanyId, short UserId, M_Category M_Category);
    }
}