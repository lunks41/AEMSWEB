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
    public sealed class UserGridServices : IUserGridServices
    {
        private readonly IRepository<S_UserGrdFormat> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public UserGridServices(IRepository<S_UserGrdFormat> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<UserGridViewModelCount> GetUserGridAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId)
        {
            UserGridViewModelCount countViewModel = new UserGridViewModelCount();
            try
            {
                //var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>( $"SELECT COUNT(*) AS CountId FROM AdmTransaction AdmTrn INNER JOIN AdmModule AdmMod on AdmMod.ModuleId=AdmTrn.ModuleId where AdmMod.IsActive=1 And AdmTrn.IsActive=1 And AdmTrn.IsNumber=1  ORDER BY AdmMod.SeqNo,AdmTrn.SeqNo");

                var result = await _repository.GetQueryAsync<UserGridViewModel>($"SELECT S_usr.CompanyId,S_usr.UserId,S_usr.ModuleId,S_usr.TransactionId,S_usr.GrdName,S_usr.GrdKey,S_usr.GrdColVisible,S_usr.GrdColOrder,S_usr.GrdColSize,S_usr.GrdSort,S_usr.GrdString,S_usr.CreateById,S_usr.CreateDate,S_usr.EditById,S_usr.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM S_UserGrdFormat S_usr LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = S_usr.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = S_usr.EditById WHERE S_usr.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.GridSetting})) and S_usr.UserId={UserId} and S_usr.ModuleId={ModuleId} and S_usr.TransactionId={TransactionId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";

                countViewModel.totalRecords = 0;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.GridSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_UserGrdFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<UserGridViewModel> GetUserGridByIdAsync(Int16 CompanyId, UserGridViewModel userGridViewModel, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<UserGridViewModel>($"SELECT CompanyId,UserId,ModuleId,TransactionId,GrdName,GrdKey,GrdColVisible,GrdColOrder,GrdColSize,GrdSort,GrdString,CreateById,CreateDate,EditById,EditDate FROM dbo.S_UserGrdFormat WHERE UserId={UserId} and ModuleId={userGridViewModel.ModuleId} and TransactionId={userGridViewModel.TransactionId} And CompanyId={CompanyId}");
                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.GridSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_UserGrdFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<UserGridViewModel>> GetUserGridByUserIdAsync(Int16 CompanyId, Int16 SelectedUserId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserGridViewModel>($"SELECT CompanyId,UserId,ModuleId,TransactionId,GrdName,GrdKey,GrdColVisible,GrdColOrder,GrdColSize,GrdSort,GrdString,CreateById,CreateDate,EditById,EditDate FROM dbo.S_UserGrdFormat WHERE UserId={SelectedUserId} And CompanyId={CompanyId}");
                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.GridSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_UserGrdFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveUserGridAsync(Int16 CompanyId, S_UserGrdFormat s_UserGrdFormat, Int16 UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.S_UserGrdFormat WHERE CompanyId = {CompanyId} and UserId={UserId} and ModuleId={s_UserGrdFormat.ModuleId} and TransactionId={s_UserGrdFormat.TransactionId} and GrdName='{s_UserGrdFormat.GrdName}'");

                    if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_UserGrdFormat);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        s_UserGrdFormat.EditById = null;
                        s_UserGrdFormat.EditDate = null;
                        var entity = _context.Add(s_UserGrdFormat);
                    }

                    var DocumentNosToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (DocumentNosToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.GridSetting,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_UserGrdFormat",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "User Grid Settings Save Successfully",
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.GridSetting, 0, "", "S_UserGrdFormat", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.GridSetting, 0, "", "S_UserGrdFormat", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> CloneUserGridSettingAsync(Int16 CompanyId, Int16 FromUserId, Int16 ToUserId, Int16 UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var CreateResult = await _repository.GetRowExecuteAsync($"Create FROM dbo.S_UserGrdFormat WHERE UserId = {ToUserId} and CompanyId={CompanyId}");

                    var insertResult = await _repository.GetRowExecuteAsync(
                         $@"
                        INSERT INTO dbo.S_UserGrdFormat
                        (
                            CompanyId,
                            UserId,
                            ModuleId,
                            TransactionId,
                            GrdName,
                            GrdKey,
                            GrdColVisible,
                            GrdColOrder,
                            GrdColSize,
                            GrdSort,
                            GrdString,
                            CreateById,
                            CreateDate
                        )
                        SELECT
                            CompanyId,
                            {ToUserId} AS UserId, -- Assign target UserId
                            ModuleId,
                            TransactionId,
                            GrdName,
                            GrdKey,
                            GrdColVisible,
                            GrdColOrder,
                            GrdColSize,
                            GrdSort,
                            GrdString,
                            {UserId} AS CreateById,
                            GETDATE() AS CreateDate
                        FROM dbo.S_UserGrdFormat
                        WHERE UserId = {FromUserId} AND CompanyId = {CompanyId}"
                     );

                    if (insertResult == 0) // If no rows were inserted
                        return new SqlResponse { Result = 0, Message = "No records cloned for the target user." };

                    #region Save AuditLog

                    if (insertResult > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.GridSetting,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_UserGrdFormat",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "User Grid Settings Clone Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Clone Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Clone Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.GridSetting, 0, "", "S_UserGrdFormat", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.GridSetting, 0, "", "S_UserGrdFormat", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
    }
}