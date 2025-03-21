using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IVoyageService
    {
        public Task<VoyageViewModelCount> GetVoyageListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<VoyageViewModel> GetVoyageByIdAsync(short CompanyId, short UserId, short voyageId);

        public Task<SqlResponse> SaveVoyageAsync(short CompanyId, short UserId, M_Voyage m_Voyage);

        public Task<SqlResponse> DeleteVoyageAsync(short CompanyId, short UserId, short voyageId);
    }
}