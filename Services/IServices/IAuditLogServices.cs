using AEMSWEB.Entities.Admin;

namespace AEMSWEB.ModelsServices
{
    public interface IAuditLogServices
    {
        public Task AddAuditLogAsync(AdmAuditLog auditLog);
    }
}