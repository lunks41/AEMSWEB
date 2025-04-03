using AMESWEB.Entities.Admin;

namespace AMESWEB.ModelsServices
{
    public interface IAuditLogServices
    {
        public Task AddAuditLogAsync(AdmAuditLog auditLog);
    }
}