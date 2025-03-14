using AEMSWEB.Entities.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.Areas.Admin.Data
{
    public interface IAllLogService
    {
        public Task<IEnumerable<AuditLogViewModel>> GetAuditLogListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, short UserId);

        public Task<UserLogViewModelCount> GetUserLogListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, short UserId);

        public Task<SqlResponse> SaveUserLog(short CompanyId, AdmUserLog admUserLog, short UserId);

        public Task<IEnumerable<ErrorLogViewModel>> GetErrorLogListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, short UserId);
    }
}