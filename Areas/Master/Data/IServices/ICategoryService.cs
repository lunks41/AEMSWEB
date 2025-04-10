using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ICategoryService
    {
        public Task<CategoryViewModelCount> GetCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CategoryViewModel> GetCategoryByIdAsync(short CompanyId, short UserId, short categoryId);

        public Task<SqlResponce> SaveCategoryAsync(short CompanyId, short UserId, M_Category M_Category);

        public Task<SqlResponce> DeleteCategoryAsync(short CompanyId, short UserId, short categoryId);

        public Task<SubCategoryViewModelCount> GetSubCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<SubCategoryViewModel> GetSubCategoryByIdAsync(short CompanyId, short UserId, short SubCategoryId);

        public Task<SqlResponce> SaveSubCategoryAsync(short CompanyId, short UserId, M_SubCategory m_SubCategory);

        public Task<SqlResponce> DeleteSubCategoryAsync(short CompanyId, short UserId, short SubCategoryId);
    }
}