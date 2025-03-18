using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IDesignationService
    {
        public Task<DesignationViewModelCount> GetDesignationListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<M_Designation> GetDesignationByIdAsync(short CompanyId, short UserId, short DesignationId);

        public Task<SqlResponse> SaveDesignationAsync(short CompanyId, short UserId, M_Designation M_Designation);

        public Task<SqlResponse> DeleteDesignationAsync(short CompanyId, short UserId, M_Designation M_Designation);
    }
}