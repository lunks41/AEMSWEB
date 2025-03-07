using AEMSWEB.Enums;

namespace AEMSWEB.IServices
{
    public interface ILogService
    {
        Task SaveAuditLogAsync(Int16 CompanyId, E_Modules moduleId, E_Master transactionId, Int64 DocumentId, string DocumentNo, string TblName, E_Mode mode, string Remarks, Int16 UserId);

        Task LogErrorAsync<T>(Exception ex, Int16 CompanyId, E_Modules moduleId, T transactionId, Int64 DocumentId, string DocumentNo, string TblName, E_Mode mode, string errorType, Int16 UserId);
    }
}