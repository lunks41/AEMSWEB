using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IGstService
    {
        public Task<GstViewModelCount> GetGstListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<GstViewModel> GetGstByIdAsync(short CompanyId, short UserId, short GstId);

        public Task<SqlResponse> SaveGstAsync(short CompanyId, short UserId, M_Gst m_GstDt);

        public Task<SqlResponse> DeleteGstAsync(short CompanyId, short UserId, short GstId);

        public Task<GstDtViewModelCount> GetGstDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<GstDtViewModel> GetGstDtByIdAsync(short CompanyId, short UserId, short GstDtId, DateTime ValidFrom);

        public Task<SqlResponse> SaveGstDtAsync(short CompanyId, short UserId, M_GstDt m_GstDt);

        public Task<SqlResponse> DeleteGstDtAsync(short CompanyId, short UserId, short GstId, DateTime validate);

        public Task<GstCategoryViewModelCount> GetGstCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<GstCategoryViewModel> GetGstCategoryByIdAsync(short CompanyId, short UserId, int GstCategoryId);

        public Task<SqlResponse> SaveGstCategoryAsync(short CompanyId, short UserId, M_GstCategory m_GstCategory);

        public Task<SqlResponse> DeleteGstCategoryAsync(short CompanyId, short UserId, short gstCategoryId);
    }
}