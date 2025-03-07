using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices.Masters;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;

namespace AEMSWEB.Services.Admin
{
    public sealed class AuditLogService : IAuditLogService
    {
        private readonly IRepository<AdmAuditLog> _repository;
        private ApplicationDbContext _context;

        public AuditLogService(IRepository<AdmAuditLog> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<AuditLogViewModel>> GetAuditLogListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<AuditLogViewModel>($"SELECT AuditLogId,AuditLogName FROM AdmAuditLog ");
            }
            catch (Exception ex)
            {
                var AuditLog = new AdmAuditLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetAuditLogListAsync",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(AuditLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}