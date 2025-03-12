using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;
using System.Transactions;

namespace AEMSWEB.Services.Admin
{
    public sealed class AllLogService : IAllLogService
    {
        private readonly IRepository<AdmUserLog> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public AllLogService(IRepository<AdmUserLog> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
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

        public async Task<UserLogViewModelCount> GetUserLogListAsync(Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 UserId)
        {
            UserLogViewModelCount countViewModel = new UserLogViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.AdmUserLog A_UsrLog INNER JOIN dbo.AdmUser A_Usr ON A_Usr.UserId = A_UsrLog.UserId WHERE (A_Usr.UserCode LIKE '%{searchString}%' OR A_Usr.UserName LIKE '%{searchString}%' OR A_UsrLog.Remarks LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<UserLogViewModel>($"SELECT A_UsrLog.UserId,A_Usr.UserCode,A_Usr.UserName,A_UsrLog.IsLogin,A_UsrLog.LoginDate,A_UsrLog.Remarks FROM dbo.AdmUserLog A_UsrLog INNER JOIN dbo.AdmUser A_Usr ON A_Usr.UserId = A_UsrLog.UserId WHERE (A_Usr.UserCode LIKE '%{searchString}%' OR A_Usr.UserName LIKE '%{searchString}%' OR A_UsrLog.Remarks LIKE '%{searchString}%') ORDER BY A_Usr.UserName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveUserLog(Int16 CompanyId, AdmUserLog admUserLog, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (admUserLog.UserId > 0)
                    {
                        #region Update UserLogRights

                        var entity = _context.Add(admUserLog);

                        var counToUpdate = _context.SaveChanges();

                        #endregion Update UserLogRights

                        if (counToUpdate > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Admin.User,
                                DocumentId = 0,
                                DocumentNo = "",
                                TblName = "AdmUserLog",
                                ModeId = (short)E_Mode.Update,
                                Remarks = "UserLogRights Update Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Upset Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Upset Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "UserLogRights Should not be zero" };
                    }
                    return new SqlResponse();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Admin.User,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "AdmUserLog",
                        ModeId = (short)E_Mode.Update,
                        Remarks = ex.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
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