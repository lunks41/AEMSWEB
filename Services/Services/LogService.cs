using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.IServices;

namespace AMESWEB.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAuditLogAsync(Int16 CompanyId, E_Modules moduleId, E_Master transactionId, Int64 DocumentId, string DocumentNo, string TblName, E_Mode mode, string Remarks, Int16 UserId)
        {
            var auditLog = new AdmAuditLog
            {
                CompanyId = CompanyId,
                ModuleId = (short)moduleId,
                TransactionId = (short)transactionId,
                DocumentId = DocumentId,
                DocumentNo = DocumentNo,
                TblName = TblName,
                ModeId = (short)mode,
                Remarks = Remarks,
                CreateById = UserId,
                CreateDate = DateTime.Now
            };

            _context.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogErrorAsync<T>(Exception ex, Int16 CompanyId, E_Modules moduleId, T transactionId, Int64 DocumentId, string DocumentNo, string TblName, E_Mode mode, string errorType, Int16 UserId)
        {
            _context.ChangeTracker.Clear();

            var errorLog = new AdmErrorLog
            {
                CompanyId = CompanyId,
                ModuleId = (short)moduleId,
                //TransactionId = (short)transactionId,
                TransactionId = Convert.ToInt16(transactionId),
                DocumentId = DocumentId,
                DocumentNo = DocumentNo,
                TblName = TblName,
                ModeId = (short)mode,
                Remarks = errorType == "SQL" ? ex.Message + ex.InnerException?.Message : ex.Message,
                CreateById = UserId,
                CreateDate = DateTime.Now
            };

            _context.Add(errorLog);
            await _context.SaveChangesAsync();
        }
    }
}