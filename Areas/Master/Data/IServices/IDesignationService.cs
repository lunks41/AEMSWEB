using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IDesignationService
    {
        public Task<DesignationViewModelCount> GetDesignationListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<DesignationViewModel> GetDesignationByIdAsync(short CompanyId, short UserId, short DesignationId);

        public Task<SqlResponse> SaveDesignationAsync(short CompanyId, short UserId, M_Designation M_Designation);

        public Task<SqlResponse> DeleteDesignationAsync(short CompanyId, short UserId, short designationId);
    }
}