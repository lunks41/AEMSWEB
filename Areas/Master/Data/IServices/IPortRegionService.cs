using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IPortRegionService
    {
        public Task<PortRegionViewModelCount> GetPortRegionListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<PortRegionViewModel> GetPortRegionByIdAsync(short CompanyId, short UserId, short PortRegionId);

        public Task<SqlResponse> SavePortRegionAsync(short CompanyId, short UserId, M_PortRegion M_PortRegion);

        public Task<SqlResponse> DeletePortRegionAsync(short CompanyId, short UserId, short PortRegionId);
    }
}