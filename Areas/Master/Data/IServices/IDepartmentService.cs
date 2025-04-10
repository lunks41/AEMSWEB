using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IDepartmentService
    {
        public Task<DepartmentViewModelCount> GetDepartmentListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<DepartmentViewModel> GetDepartmentByIdAsync(short CompanyId, short UserId, short departmentId);

        public Task<SqlResponce> SaveDepartmentAsync(short CompanyId, short UserId, M_Department M_Department);

        public Task<SqlResponce> DeleteDepartmentAsync(short CompanyId, short UserId, short departmentId);
    }
}