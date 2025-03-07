using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Enums;
using AEMSWEB.Helper;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Setting;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace AEMSWEB.Services.Setting
{
    public sealed class UserSettingServices : IUserSettingService
    {
        private readonly IRepository<S_UserSettings> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public UserSettingServices(IRepository<S_UserSettings> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        // add the number id
        public async Task<UserSettingViewModel> GetUserSettingAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<UserSettingViewModel>($"SELECT CompanyId,UserId,Trn_Grd_TotRec,M_Grd_TotRec,Ar_IN_GLId,Ar_CN_GLId,Ar_DN_GLId,Ap_IN_GLId,Ap_CN_GLId,Ap_DN_GLId FROM dbo.S_UserSettings WHERE CompanyId={CompanyId} AND UserId={UserId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Setting.UserSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_UserSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveUserSettingAsync(Int16 CompanyId, S_UserSettings S_UserSettings, Int16 UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.S_UserSettings WHERE CompanyId = {S_UserSettings.CompanyId} AND UserId={UserId}");

                    if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(S_UserSettings);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        S_UserSettings.EditById = null;
                        S_UserSettings.EditDate = null;
                        _context.Add(S_UserSettings);
                    }

                    var UserSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (UserSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Setting.UserSetting,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_UserSettings",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "UserSettings Save Successfully",
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
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.UserSetting, 0, "", "S_UserSettings", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.UserSetting, 0, "", "S_UserSettings", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
    }
}