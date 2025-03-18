using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IGstService
    {
        public Task<GstViewModelCount> GetGstListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_Gst> GetGstByIdAsync(short CompanyId, short UserId, short GstId);

        public Task<SqlResponse> SaveGstAsync(short CompanyId, short UserId, M_Gst m_GstDt);

        public Task<SqlResponse> DeleteGstAsync(short CompanyId, short UserId, M_Gst m_GstDt);

        public Task<GstDtViewModelCount> GetGstDtListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<GstDtViewModel> GetGstDtByIdAsync(short CompanyId, short UserId, short GstDtId, DateTime ValidFrom);

        public Task<SqlResponse> SaveGstDtAsync(short CompanyId, short UserId, M_GstDt m_GstDt);

        public Task<SqlResponse> DeleteGstDtAsync(short CompanyId, short UserId, GstDtViewModel m_GstDt);

        public Task<GstCategoryViewModelCount> GetGstCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_GstCategory> GetGstCategoryByIdAsync(short CompanyId, short UserId, int GstCategoryId);

        public Task<SqlResponse> SaveGstCategoryAsync(short CompanyId, short UserId, M_GstCategory m_GstCategory);

        public Task<SqlResponse> DeleteGstCategoryAsync(short CompanyId, short UserId, M_GstCategory m_GstCategory);
    }
}