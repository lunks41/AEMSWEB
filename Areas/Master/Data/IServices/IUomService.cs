using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IUomService
    {
        public Task<UomViewModelCount> GetUomListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<UomViewModel> GetUomByIdAsync(short CompanyId, short UserId, short UomId);

        public Task<SqlResponce> SaveUomAsync(short CompanyId, short UserId, M_Uom m_Uom);

        public Task<SqlResponce> DeleteUomAsync(short CompanyId, short UserId, short UomId);

        public Task<UomDtViewModelCount> GetUomDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<UomDtViewModel> GetUomDtByIdAsync(short CompanyId, short UserId, short UomId, short PackUomId);

        public Task<SqlResponce> SaveUomDtAsync(short CompanyId, short UserId, M_UomDt m_UomDt);

        public Task<SqlResponce> DeleteUomDtAsync(short CompanyId, short UserId, short UomId, short PackUomId);
    }
}