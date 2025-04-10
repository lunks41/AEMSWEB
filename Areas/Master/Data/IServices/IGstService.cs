using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IGstService
    {
        public Task<GstViewModelCount> GetGstListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<GstViewModel> GetGstByIdAsync(short CompanyId, short UserId, short GstId);

        public Task<SqlResponce> SaveGstAsync(short CompanyId, short UserId, M_Gst Gst);

        public Task<SqlResponce> DeleteGstAsync(short CompanyId, short UserId, short GstId);

        public Task<GstDtViewModelCount> GetGstDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<GstDtViewModel> GetGstDtByIdAsync(short CompanyId, short UserId, short GstDtId, DateTime ValidFrom);

        public Task<SqlResponce> SaveGstDtAsync(short CompanyId, short UserId, M_GstDt m_GstDt);

        public Task<SqlResponce> DeleteGstDtAsync(short CompanyId, short UserId, short GstId, DateTime ValidFrom);

        public Task<GstCategoryViewModelCount> GetGstCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<GstCategoryViewModel> GetGstCategoryByIdAsync(short CompanyId, short UserId, int GstCategoryId);

        public Task<SqlResponce> SaveGstCategoryAsync(short CompanyId, short UserId, M_GstCategory m_GstCategory);

        public Task<SqlResponce> DeleteGstCategoryAsync(short CompanyId, short UserId, short gstCategoryId);
    }
}