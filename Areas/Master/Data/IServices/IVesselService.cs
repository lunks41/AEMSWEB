using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IVesselService
    {
        public Task<VesselViewModelCount> GetVesselListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<VesselViewModel> GetVesselByIdAsync(short CompanyId, short UserId, int VesselId);

        public Task<SqlResponse> SaveVesselAsync(short CompanyId, short UserId, M_Vessel m_Vessel);

        public Task<SqlResponse> DeleteVesselAsync(short CompanyId, short UserId, int VesselId);
    }
}