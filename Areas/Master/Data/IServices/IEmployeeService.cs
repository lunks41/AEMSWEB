using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IEmployeeService
    {
        public Task<EmployeeViewModelCount> GetEmployeeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<EmployeeViewModel> GetEmployeeByIdAsync(short CompanyId, short UserId, short EmployeeId);

        public Task<SqlResponce> SaveEmployeeAsync(short CompanyId, short UserId, M_Employee M_Employee);

        public Task<SqlResponce> DeleteEmployeeAsync(short CompanyId, short UserId, short EmployeeId);
    }
}