using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IDepartmentService
    {
        public Task<DepartmentViewModelCount> GetDepartmentListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<DepartmentViewModel> GetDepartmentByIdAsync(short CompanyId, short UserId, short departmentId);

        public Task<SqlResponse> SaveDepartmentAsync(short CompanyId, short UserId, M_Department M_Department);

        public Task<SqlResponse> DeleteDepartmentAsync(short CompanyId, short UserId, short departmentId);
    }
}