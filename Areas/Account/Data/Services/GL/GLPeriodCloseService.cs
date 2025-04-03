using AMESWEB.Areas.Account.Data.IServices;
using AMESWEB.Areas.Account.Data.IServices.GL;
using AMESWEB.Areas.Account.Models.GL;
using AMESWEB.Data;
using AMESWEB.Entities.Accounts.GL;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Account.Data.Services.GL
{
    public sealed class GLPeriodCloseService : IGLPeriodCloseService
    {
        private readonly IRepository<GLPeriodClose> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;
        private readonly IAccountService _accountService;

        public GLPeriodCloseService(IRepository<GLPeriodClose> repository, ApplicationDbContext context, ILogService logService, IAccountService accountService)
        {
            _repository = repository;
            _context = context; _logService = logService;
            _accountService = accountService;
        }

        public async Task<IEnumerable<GLPeriodCloseViewModel>> GetGLPeriodCloseListAsync(short CompanyId, int FinYear, short UserId)
        {
            try
            {
                return await _repository.GetQueryAsync<GLPeriodCloseViewModel>($"SELECT GLprcls.CompanyId,GLprcls.FinYear,GLprcls.FinMonth,GLprcls.StartDate,GLprcls.EndDate,GLprcls.IsArClose,GLprcls.ArCloseById,GLprcls.ArCloseDate,GLprcls.IsApClose,GLprcls.ApCloseById,GLprcls.ApCloseDate,GLprcls.IsCbClose,GLprcls.CbCloseById,GLprcls.CbCloseDate,GLprcls.IsGlClose,GLprcls.GlCloseById,GLprcls.GlCloseDate,UsrAr.UserName ArCloseBy,UsrAp.UserName ApCloseBy,UsrCb.UserName CbCloseBy,UsrGl.UserName GlCloseBy,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.GLPeriodClose GLprcls LEFT JOIN dbo.AdmUser UsrAr ON UsrAr.UserId = GLprcls.ArCloseById LEFT JOIN dbo.AdmUser UsrAp ON UsrAp.UserId = GLprcls.ApCloseById LEFT JOIN dbo.AdmUser UsrCb ON UsrCb.UserId = GLprcls.CbCloseById LEFT JOIN dbo.AdmUser UsrGl ON UsrGl.UserId = GLprcls.GlCloseById LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = GLprcls.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = GLprcls.EditById WHERE GLprcls.FinYear={FinYear}");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.PeriodClose,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GLPeriodClose",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveGLPeriodCloseAsync(short CompanyId, PeriodCloseViewModel periodCloseViewModel, short UserId)
        {
            bool IsEdit = true;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var SaveDetails = 0;

                    if (periodCloseViewModel.FieldName == "isArClose")
                        SaveDetails = await _context.GLPeriodClose.Where(x => x.FinYear == periodCloseViewModel.FinYear && x.FinMonth == periodCloseViewModel.FinMonth).ExecuteUpdateAsync(setPropertyCalls: setters => setters.SetProperty(b => b.IsArClose, periodCloseViewModel.IsValue).SetProperty(b => b.ArCloseById, UserId).SetProperty(b => b.ArCloseDate, DateTime.Now).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));
                    else if (periodCloseViewModel.FieldName == "isApClose")
                        SaveDetails = await _context.GLPeriodClose.Where(x => x.FinYear == periodCloseViewModel.FinYear && x.FinMonth == periodCloseViewModel.FinMonth).ExecuteUpdateAsync(setPropertyCalls: setters => setters.SetProperty(b => b.IsApClose, periodCloseViewModel.IsValue).SetProperty(b => b.ApCloseById, UserId).SetProperty(b => b.ApCloseDate, DateTime.Now).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));
                    else if (periodCloseViewModel.FieldName == "isCbClose")
                        SaveDetails = await _context.GLPeriodClose.Where(x => x.FinYear == periodCloseViewModel.FinYear && x.FinMonth == periodCloseViewModel.FinMonth).ExecuteUpdateAsync(setPropertyCalls: setters => setters.SetProperty(b => b.IsCbClose, periodCloseViewModel.IsValue).SetProperty(b => b.CbCloseById, UserId).SetProperty(b => b.CbCloseDate, DateTime.Now).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));
                    else if (periodCloseViewModel.FieldName == "isGlClose")
                        SaveDetails = await _context.GLPeriodClose.Where(x => x.FinYear == periodCloseViewModel.FinYear && x.FinMonth == periodCloseViewModel.FinMonth).ExecuteUpdateAsync(setPropertyCalls: setters => setters.SetProperty(b => b.IsGlClose, periodCloseViewModel.IsValue).SetProperty(b => b.GlCloseById, UserId).SetProperty(b => b.GlCloseDate, DateTime.Now).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));

                    #region Save AuditLog

                    if (SaveDetails > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.GL,
                            TransactionId = (short)E_GL.PeriodClose,
                            DocumentId = periodCloseViewModel.FinYear,
                            DocumentNo = periodCloseViewModel.FinMonth.ToString(),
                            TblName = "GLPeriodClose",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.PeriodClose,
                    DocumentId = periodCloseViewModel.FinYear,
                    DocumentNo = periodCloseViewModel.FinMonth.ToString(),
                    TblName = "GLPeriodClose",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }

        public async Task<SqlResponse> GenrateGLPeriodCloseYearlyAsync(short CompanyId, PeriodCloseViewModel periodCloseViewModel, short UserId)
        {
            bool IsEdit = true;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var SaveDetails = 0;

                    if (periodCloseViewModel.FieldName == "isArClose")
                        SaveDetails = await _context.GLPeriodClose.Where(x => x.FinYear == periodCloseViewModel.FinYear && x.FinMonth == periodCloseViewModel.FinMonth).ExecuteUpdateAsync(setPropertyCalls: setters => setters.SetProperty(b => b.IsArClose, periodCloseViewModel.IsValue).SetProperty(b => b.ArCloseById, UserId).SetProperty(b => b.ArCloseDate, DateTime.Now).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));
                    else if (periodCloseViewModel.FieldName == "isApClose")
                        SaveDetails = await _context.GLPeriodClose.Where(x => x.FinYear == periodCloseViewModel.FinYear && x.FinMonth == periodCloseViewModel.FinMonth).ExecuteUpdateAsync(setPropertyCalls: setters => setters.SetProperty(b => b.IsApClose, periodCloseViewModel.IsValue).SetProperty(b => b.ApCloseById, UserId).SetProperty(b => b.ApCloseDate, DateTime.Now).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));
                    else if (periodCloseViewModel.FieldName == "isCbClose")
                        SaveDetails = await _context.GLPeriodClose.Where(x => x.FinYear == periodCloseViewModel.FinYear && x.FinMonth == periodCloseViewModel.FinMonth).ExecuteUpdateAsync(setPropertyCalls: setters => setters.SetProperty(b => b.IsCbClose, periodCloseViewModel.IsValue).SetProperty(b => b.CbCloseById, UserId).SetProperty(b => b.CbCloseDate, DateTime.Now).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));
                    else if (periodCloseViewModel.FieldName == "isGlClose")
                        SaveDetails = await _context.GLPeriodClose.Where(x => x.FinYear == periodCloseViewModel.FinYear && x.FinMonth == periodCloseViewModel.FinMonth).ExecuteUpdateAsync(setPropertyCalls: setters => setters.SetProperty(b => b.IsGlClose, periodCloseViewModel.IsValue).SetProperty(b => b.GlCloseById, UserId).SetProperty(b => b.GlCloseDate, DateTime.Now).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));

                    #region Save AuditLog

                    if (SaveDetails > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.GL,
                            TransactionId = (short)E_GL.PeriodClose,
                            DocumentId = periodCloseViewModel.FinYear,
                            DocumentNo = periodCloseViewModel.FinMonth.ToString(),
                            TblName = "GLPeriodClose",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.PeriodClose,
                    DocumentId = periodCloseViewModel.FinYear,
                    DocumentNo = periodCloseViewModel.FinMonth.ToString(),
                    TblName = "GLPeriodClose",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();
                throw;
            }
        }
    }
}