using AMESWEB.Entities.Admin;
using AMESWEB.Models;
using AMESWEB.Models.Admin;

namespace AMESWEB.Areas.Admin.Data
{
    public interface IAllLogService
    {
        public Task<IEnumerable<AuditLogViewModel>> GetAuditLogListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, short UserId);

        public Task<UserLogViewModelCount> GetUserLogListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, short UserId);

        public Task<SqlResponce> SaveUserLog(short CompanyId, AdmUserLog admUserLog, short UserId);

        public Task<IEnumerable<ErrorLogViewModel>> GetErrorLogListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, short UserId);
    }
}