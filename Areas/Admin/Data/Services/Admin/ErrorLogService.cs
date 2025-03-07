using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices.Masters;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;

namespace AEMSWEB.Services.Admin
{
    public sealed class ErrorLogService : IErrorLogService
    {
        private readonly IRepository<AdmErrorLog> _repository;
        private ApplicationDbContext _context;

        public ErrorLogService(IRepository<AdmErrorLog> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ErrorLogViewModel>> GetErrorLogListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<ErrorLogViewModel>($"SELECT ErrorLogId,ErrorLogName FROM AdmErrorLog ");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetErrorLogListAsync",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}