using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IGstCategoryService
    {
        public Task<GstCategoryViewModelCount> GetGstCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_GstCategory> GetGstCategoryByIdAsync(short CompanyId, short UserId, int GstCategoryId);

        public Task<SqlResponse> SaveGstCategoryAsync(short CompanyId, short UserId, M_GstCategory m_GstCategory);

        public Task<SqlResponse> DeleteGstCategoryAsync(short CompanyId, short UserId, M_GstCategory m_GstCategory);
    }
}