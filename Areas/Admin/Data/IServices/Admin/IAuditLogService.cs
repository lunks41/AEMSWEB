using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Masters
{
    public interface IAuditLogService
    {
        public Task<IEnumerable<AuditLogViewModel>> GetAuditLogListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);
    }
}