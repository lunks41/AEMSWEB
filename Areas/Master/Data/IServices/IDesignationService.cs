using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IDesignationService
    {
        public Task<DesignationViewModelCount> GetDesignationListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<DesignationViewModel> GetDesignationByIdAsync(short CompanyId, short UserId, short DesignationId);

        public Task<SqlResponce> SaveDesignationAsync(short CompanyId, short UserId, M_Designation M_Designation);

        public Task<SqlResponce> DeleteDesignationAsync(short CompanyId, short UserId, short designationId);
    }
}