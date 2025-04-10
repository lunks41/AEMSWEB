using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IVoyageService
    {
        public Task<VoyageViewModelCount> GetVoyageListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<VoyageViewModel> GetVoyageByIdAsync(short CompanyId, short UserId, short voyageId);

        public Task<SqlResponce> SaveVoyageAsync(short CompanyId, short UserId, M_Voyage m_Voyage);

        public Task<SqlResponce> DeleteVoyageAsync(short CompanyId, short UserId, short voyageId);
    }
}