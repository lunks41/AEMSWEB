using AEMSWEB.Entities.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Admin
{
    public interface IUserLogService
    {
        public Task<UserLogViewModelCount> GetUserLogListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<SqlResponse> SaveUserLog(Int16 CompanyId, AdmUserLog admUserLog, Int16 UserId);
    }
}