using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Enums;
using AEMSWEB.Helpers;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AEMSWEB.Areas.Setting.Data
{
    public sealed class SettingServices : ISettingService
    {
        private readonly IRepository<S_DecSettings> _repository;
        private ApplicationDbContext _context;
        private readonly ILogService _logService;

        public SettingServices(IRepository<S_DecSettings> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        #region

        public async Task<DecimalSettingViewModel> GetDecSettingAsync(short CompanyId, short UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DecimalSettingViewModel>($"SELECT AmtDec,LocAmtDec,CtyAmtDec,PriceDec,QtyDec,ExhRateDec,DateFormat,LongDateFormat,S_dec.CreateById,Usr.UserCode CreateBy, S_dec.CreateDate, S_dec.EditById,Usr1.UserCode EditBy,S_dec.EditDate FROM dbo.S_DecSettings S_dec Left Join AdmUser Usr on Usr.UserId=S_dec.CreatebyId Left Join AdmUser Usr1 on Usr1.UserId=S_dec.CreatebyId WHERE CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DecSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveDecSettingAsync(short CompanyId, short UserId, S_DecSettings s_DecSettings)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.S_DecSettings WHERE CompanyId = {s_DecSettings.CompanyId}");

                    if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_DecSettings);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        s_DecSettings.EditById = null;
                        s_DecSettings.EditDate = null;
                        _context.Add(s_DecSettings);
                    }

                    var FinSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (FinSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DecSetting,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_DecSettings",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "FinSettings Save Successfully",
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.DecSetting, 0, "", "S_DecSettings", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.DecSetting, 0, "", "S_DecSettings", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        #endregion

        #region

        public async Task<FinanceSettingViewModel> GetFinSettingAsync(short CompanyId, short UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<FinanceSettingViewModel>($"SELECT Base_CurrencyId,Local_CurrencyId,ExhGainLoss_GlId,BankCharge_GlId,ProfitLoss_GlId,RetEarning_GlId,SaleGst_GlId,PurGst_GlId,SaleDef_GlId,PurDef_GlId,S_Fin.CreateById,Usr.UserCode CreateBy, S_Fin.CreateDate, S_Fin.EditById,Usr1.UserCode EditBy,S_Fin.EditDate FROM dbo.S_FinSettings S_Fin Left Join AdmUser Usr on Usr.UserId=S_Fin.CreatebyId Left Join AdmUser Usr1 on Usr1.UserId=S_Fin.CreatebyId WHERE CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Setting.FinSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_FinSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveFinSettingAsync(short CompanyId, short UserId, S_FinSettings s_FinSettings)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.S_FinSettings WHERE CompanyId = {s_FinSettings.CompanyId}");

                    if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_FinSettings);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        s_FinSettings.EditById = null;
                        s_FinSettings.EditDate = null;
                        _context.Add(s_FinSettings);
                    }

                    var FinSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (FinSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Setting.FinSetting,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_FinSettings",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "FinSettings Save Successfully",
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.FinSetting, 0, "", "S_FinSetting", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.FinSetting, 0, "", "S_FinSetting", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        #endregion

        #region

        public async Task<MandatoryFieldsViewModel> GetMandatoryFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<MandatoryFieldsViewModel>($"SELECT CompanyId,ModuleId,TransactionId,M_ProductId,M_GLId,M_QTY,M_UomId,M_UnitPrice,M_TotAmt,M_Remarks,M_GstId,M_DeliveryDate,M_DepartmentId,M_EmployeeId,M_PortId,M_VesselId,M_BargeId,M_VoyageId,M_SupplyDate,M_ReferenceNo,M_SuppInvoiceNo,M_BankId,M_Remarks_Hd,M_Address1,M_Address2,M_Address3,M_Address4,M_PinCode,M_CountryId,M_PhoneNo,M_ContactName,M_MobileNo,M_EmailAdd FROM dbo.S_MandatoryFields S_Man LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = S_Man.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = S_Man.EditById WHERE ModuleId={ModuleId} AND CompanyId={CompanyId} AND TransactionId={TransactionId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.MandatoryFields,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_MandatoryFields",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<MandatoryFieldsViewModel>> GetMandatoryFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<MandatoryFieldsViewModel>(
                     $"SELECT CompanyId,S_Vis.ModuleId,AdmMod.ModuleName,AdmTrs.TransactionId,AdmTrs.TransactionName,M_ProductId,M_QTY,M_UomId,M_UnitPrice,M_Remarks,M_GstId,M_DeliveryDate,M_DepartmentId,M_EmployeeId,M_PortId,M_VesselId,M_BargeId,M_VoyageId,M_SupplyDate,M_BankId,S_Vis.CreateById,S_Vis.CreateDate,S_Vis.EditById,S_Vis.EditDate FROM S_MandatoryFields S_Vis INNER JOIN dbo.AdmModule AdmMod ON AdmMod.ModuleId = S_Vis.ModuleId INNER JOIN dbo.AdmTransaction AdmTrs ON AdmTrs.TransactionId = S_Vis.TransactionId AND AdmTrs.ModuleId= S_Vis.ModuleId WHERE S_Vis.ModuleId = {ModuleId} and S_Vis.CompanyId={CompanyId}" +
                     $" UNION all " +
                     $" SELECT {CompanyId} CompanyId,AdmTrs.ModuleId,AdmMod.ModuleName,AdmTrs.TransactionId,AdmTrs.TransactionName,0 M_ProductId,0 M_QTY,0 M_UomId,0 M_UnitPrice,0 M_Remarks,0 M_GstId,0 M_DeliveryDate,0 M_DepartmentId,0 M_EmployeeId,0 M_PortId,0 M_VesselId,0 M_BargeId,0 M_VoyageId,0 M_SupplyDate,0 M_BankId,0 CreateById,Null CreateDate,0 EditById,Null EditDate FROM AdmTransaction AdmTrs INNER JOIN dbo.AdmModule AdmMod ON AdmMod.ModuleId = AdmTrs.ModuleId WHERE AdmTrs.IsVisible=1 And AdmTrs.ModuleId={ModuleId} And AdmTrs.TransactionId not in (SELECT TransactionId FROM dbo.S_MandatoryFields where CompanyId={CompanyId} And ModuleId={ModuleId})");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.MandatoryFields,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_MandatoryFields",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveMandatoryFieldsAsync(short CompanyId, short UserId, List<S_MandatoryFields> s_MandatoryFields)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Clear existing tracked entities
                    var trackedEntities = _context.ChangeTracker.Entries<S_MandatoryFields>().ToList();
                    foreach (var entry in trackedEntities)
                    {
                        entry.State = EntityState.Detached;
                    }

                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"Create FROM dbo.S_MandatoryFields WHERE CompanyId={CompanyId} AND ModuleId={s_MandatoryFields[0].ModuleId}");

                    // Add new records
                    _context.S_MandatoryFields.AddRange(s_MandatoryFields);
                    var saveResult = await _context.SaveChangesAsync();

                    #region Audit Log

                    if (saveResult > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.MandatoryFields,
                            TblName = "S_MandatoryFields",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "MandatoryFields Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.AdmAuditLog.Add(auditLog);
                        await _context.SaveChangesAsync();

                        TScope.Complete();
                        return new SqlResponse { Result = 1, Message = "Save Successfully" };
                    }

                    #endregion Audit Log

                    return new SqlResponse { Result = 0, Message = "Save Failed" };
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.MandatoryFields, 0, "", "S_MandatoryFields", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.MandatoryFields, 0, "", "S_MandatoryFields", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveMandatoryFieldsAsyncV1(short CompanyId, List<S_MandatoryFields> s_MandatoryFields, short UserId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //var dataExist = await _repository.GetQueryAsync<SqlResponseIds>(
                    //$"Create dbo.S_MandatoryFields WHERE ModuleId={s_MandatoryFields[0].ModuleId} AND CompanyId={CompanyId}");

                    _context.AddRange(s_MandatoryFields);

                    var mandatoryToSave = await _context.SaveChangesAsync();

                    #region Save AuditLog

                    if (mandatoryToSave > 0)
                    {
                        // Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.MandatoryFields,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_MandatoryFields",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "MandatoryFields Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = await _context.SaveChangesAsync();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 0, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse { Result = -1, Message = "Save Failed" };
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.MandatoryFields, 0, "", "S_MandatoryFields", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.MandatoryFields, 0, "", "S_MandatoryFields", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        #endregion

        #region

        public async Task<VisibleFieldsViewModel> GetVisibleFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<VisibleFieldsViewModel>($"SELECT  S_Vis.M_ProductId,S_Vis.M_QTY,S_Vis.M_BillQTY,S_Vis.M_UomId,S_Vis.M_UnitPrice,S_Vis.M_Remarks,S_Vis.M_GstId,S_Vis.M_DeliveryDate,S_Vis.M_DepartmentId,S_Vis.M_EmployeeId,S_Vis.M_PortId,S_Vis.M_VesselId,S_Vis.M_BargeId,S_Vis.M_VoyageId,S_Vis.M_SupplyDate,S_Vis.M_BankId,S_Vis.M_CtyCurr FROM S_VisibleFields S_Vis WHERE ModuleId={ModuleId} AND CompanyId={CompanyId} AND TransactionId={TransactionId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.VisibleFields,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_VisibleFields",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VisibleFieldsViewModel>> GetVisibleFieldsByIdAsync(short CompanyId, short UserId, Int16 ModuleId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<VisibleFieldsViewModel>(
                    $"SELECT CompanyId,S_Vis.ModuleId,AdmMod.ModuleName,AdmTrs.TransactionId,AdmTrs.TransactionName,M_ProductId,M_QTY,M_BillQTY,M_UomId,M_UnitPrice,M_Remarks,M_GstId,M_DeliveryDate,M_DepartmentId,M_EmployeeId,M_PortId,M_VesselId,M_BargeId,M_VoyageId,M_SupplyDate,M_BankId,M_CtyCurr,S_Vis.CreateById,S_Vis.CreateDate,S_Vis.EditById,S_Vis.EditDate FROM S_VisibleFields S_Vis INNER JOIN dbo.AdmModule AdmMod ON AdmMod.ModuleId = S_Vis.ModuleId INNER JOIN dbo.AdmTransaction AdmTrs ON AdmTrs.TransactionId = S_Vis.TransactionId AND AdmTrs.ModuleId= S_Vis.ModuleId WHERE S_Vis.ModuleId = {ModuleId} and S_Vis.CompanyId={CompanyId}" +
                    $" UNION all " +
                    $" SELECT {CompanyId} CompanyId,AdmTrs.ModuleId,AdmMod.ModuleName,AdmTrs.TransactionId,AdmTrs.TransactionName,0 M_ProductId,0 M_QTY,0 M_BillQTY,0 M_UomId,0 M_UnitPrice,0 M_Remarks,0 M_GstId,0 M_DeliveryDate,0 M_DepartmentId,0 M_EmployeeId,0 M_PortId,0 M_VesselId,0 M_BargeId,0 M_VoyageId,0 M_SupplyDate,0 M_BankId,0 M_CtyCurr,0 CreateById,Null CreateDate,0 EditById,Null EditDate FROM AdmTransaction AdmTrs INNER JOIN dbo.AdmModule AdmMod ON AdmMod.ModuleId = AdmTrs.ModuleId AND AdmMod.ModuleId={ModuleId} WHERE AdmTrs.IsVisible=1 And AdmTrs.ModuleId={ModuleId} And AdmTrs.TransactionId not in (SELECT TransactionId FROM S_VisibleFields where CompanyId={CompanyId} And ModuleId={ModuleId})");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.VisibleFields,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_VisibleFields",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveVisibleFieldsAsync(short CompanyId, short UserId, List<S_VisibleFields> s_VisibleFields)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Clear existing tracked entities
                    var trackedEntities = _context.ChangeTracker.Entries<S_VisibleFields>().ToList();
                    foreach (var entry in trackedEntities)
                    {
                        entry.State = EntityState.Detached;
                    }

                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"Create FROM dbo.S_VisibleFields WHERE CompanyId={CompanyId} AND ModuleId={s_VisibleFields[0].ModuleId}");

                    // Add new records
                    _context.S_VisibleFields.AddRange(s_VisibleFields);
                    var saveResult = await _context.SaveChangesAsync();

                    #region Audit Log

                    if (saveResult > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.VisibleFields,
                            TblName = "S_VisibleFields",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "VisibleFields Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.AdmAuditLog.Add(auditLog);
                        await _context.SaveChangesAsync();

                        //TScope.Complete();
                        TScope.Complete();
                        return new SqlResponse { Result = 1, Message = "Save Successfully" };
                    }

                    #endregion Audit Log

                    return new SqlResponse { Result = 0, Message = "Save Failed" };
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.VisibleFields, 0, "", "S_VisibleFieldss", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.VisibleFields, 0, "", "S_VisibleFieldss", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        #endregion

        #region

        public async Task<ModelNameViewModelCount> GetNumberFormatListAsync(short CompanyId, short UserId)
        {
            ModelNameViewModelCount countViewModel = new ModelNameViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM AdmTransaction AdmTrn INNER JOIN AdmModule AdmMod on AdmMod.ModuleId=AdmTrn.ModuleId where AdmMod.IsActive=1 And AdmTrn.IsActive=1 And AdmTrn.IsNumber=1");

                var result = await _repository.GetQueryAsync<ModelNameViewModel>($"SELECT AdmMod.ModuleId,AdmMod.ModuleName,AdmTrn.TransactionId,AdmTrn.TransactionName FROM AdmTransaction AdmTrn INNER JOIN AdmModule AdmMod on AdmMod.ModuleId=AdmTrn.ModuleId where AdmMod.IsActive=1 And AdmTrn.IsActive=1 And AdmTrn.IsNumber=1  ORDER BY AdmMod.SeqNo,AdmTrn.SeqNo");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<ModelNameViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<NumberSettingViewModel> GetNumberFormatByIdAsync(short CompanyId, short UserId, Int32 ModuleId, Int32 TransactionId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<NumberSettingViewModel>($"SELECT S_No.NumberId,S_No.CompanyId,S_No.ModuleId,S_No.TransactionId,S_No.Prefix,S_No.PrefixSeq,S_No.PrefixDelimiter,S_No.IncludeYear,S_No.YearSeq,S_No.YearFormat,S_No.YearDelimiter,S_No.IncludeMonth,S_No.MonthSeq,S_No.MonthFormat,S_No.MonthDelimiter,S_No.NoDIgits,S_No.DIgitSeq,S_No.ResetYearly,S_No.CreateById,S_No.CreateDate,S_No.EditById,S_No.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.S_NumberFormat S_No LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = S_No.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = S_No.EditById WHERE S_No.ModuleId={ModuleId} AND S_No.TransactionId={TransactionId} AND S_No.CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormat",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<NumberSettingDtViewModel> GetNumberFormatByYearAsync(short CompanyId, short UserId, Int32 NumberId, Int32 NumYear)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<NumberSettingDtViewModel>($"SELECT NumberId,NumYear,Month1,Month2,Month3,Month4,Month5,Month6,Month7,Month8,Month9,Month10,Month11,Month12,LastNumber FROM dbo.S_NumberFormatDt WHERE NumberId={NumberId} AND NumYear={NumYear}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocumentNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_NumberFormatDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveNumberFormatAsync(short CompanyId, short UserId, S_NumberFormat s_NumberFormat)
        {
            bool IsEdit = s_NumberFormat.NumberId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.S_NumberFormat WHERE ModuleId<>{s_NumberFormat.ModuleId} AND TransactionId<>{s_NumberFormat.TransactionId} AND CompanyId={CompanyId} AND Prefix = '{s_NumberFormat.Prefix}'");

                        if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                            return new SqlResponse { Result = -1, Message = "Invoice Not Exist" };
                    }

                    if (IsEdit)
                    {
                        var entityHead = _context.Update(s_NumberFormat);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>("SELECT ISNULL((SELECT TOP 1 (NumberId + 1) FROM dbo.S_NumberFormat WHERE (NumberId + 1) NOT IN (SELECT NumberId FROM dbo.S_NumberFormat)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            s_NumberFormat.NumberId = Convert.ToInt32(sqlMissingResponse.NextId);

                            s_NumberFormat.EditById = null;
                            s_NumberFormat.EditDate = null;
                            var entity = _context.Add(s_NumberFormat);
                        }
                    }

                    var NumberFormatToSave = _context.SaveChanges();

                    if (NumberFormatToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DocumentNo,
                            DocumentId = s_NumberFormat.NumberId,
                            DocumentNo = "",
                            TblName = "S_NumberFormat",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "NumberFormat Save Successfully",
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

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.DocumentNo, 0, "", "S_NumberFormat", IsEdit ? E_Mode.Update : E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.DocumentNo, 0, "", "S_NumberFormat", IsEdit ? E_Mode.Update : E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        #endregion

        #region

        public async Task<UserGridViewModelCount> GetUserGridAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId)
        {
            UserGridViewModelCount countViewModel = new UserGridViewModelCount();
            try
            {
                //var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>( $"SELECT COUNT(*) AS CountId FROM AdmTransaction AdmTrn INNER JOIN AdmModule AdmMod on AdmMod.ModuleId=AdmTrn.ModuleId where AdmMod.IsActive=1 And AdmTrn.IsActive=1 And AdmTrn.IsNumber=1  ORDER BY AdmMod.SeqNo,AdmTrn.SeqNo");

                var result = await _repository.GetQueryAsync<UserGridViewModel>($"SELECT S_usr.CompanyId,S_usr.UserId,S_usr.ModuleId,S_usr.TransactionId,S_usr.GrdName,S_usr.GrdKey,S_usr.GrdColVisible,S_usr.GrdColOrder,S_usr.GrdColSize,S_usr.GrdSort,S_usr.GrdString,S_usr.CreateById,S_usr.CreateDate,S_usr.EditById,S_usr.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM S_UserGrdFormat S_usr LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = S_usr.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = S_usr.EditById WHERE S_usr.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.GridSetting})) and S_usr.UserId={UserId} and S_usr.ModuleId={ModuleId} and S_usr.TransactionId={TransactionId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";

                countViewModel.totalRecords = 0;
                countViewModel.data = result?.ToList() ?? new List<UserGridViewModel>();

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

        public async Task<UserGridViewModel> GetUserGridByIdAsync(short CompanyId, short UserId, UserGridViewModel userGridViewModel)
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

        public async Task<IEnumerable<UserGridViewModel>> GetUserGridByUserIdAsync(short CompanyId, short UserId, Int16 SelectedUserId)
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

        public async Task<SqlResponse> SaveUserGridAsync(short CompanyId, short UserId, S_UserGrdFormat s_UserGrdFormat)
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

        public async Task<SqlResponse> CloneUserGridSettingAsync(short CompanyId, short UserId, Int16 FromUserId, Int16 ToUserId)
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

        public async Task<UserSettingViewModel> GetUserSettingAsync(short CompanyId, short UserId)
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

        public async Task<SqlResponse> SaveUserSettingAsync(short CompanyId, short UserId, S_UserSettings S_UserSettings)
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

        #endregion

        public async Task<decimal> GetExchangeRateAsync(short CompanyId, short UserId, Int16 CurrencyId, DateTime? TrnsDate)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>($"select dbo.GetExchangeRate ({CompanyId},{CurrencyId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetExchangeRateLocalAsync(short CompanyId, short UserId, Int16 CurrencyId, string TrnsDate)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>($"select dbo.GetExchangeRate_Local ({CompanyId},{CurrencyId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<bool> GetCheckPeriodClosedAsync(short CompanyId, short UserId, Int16 ModuleId, string TrnsDate)
        {
            try
            {
                bool IsResult = await _repository.GetQuerySingleOrDefaultAsync<bool>($"select  dbo.CheckPeriodClosed ({CompanyId},{ModuleId},'{TrnsDate}')");

                return IsResult;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetGstPercentageAsync(short CompanyId, short UserId, Int16 GstId, string TrnsDate)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>($"select dbo.GetGSTPercentage ({CompanyId},{GstId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<decimal> GetCreditTermDayAsync(short CompanyId, short UserId, Int16 CreditTermId, string TrnsDate)
        {
            try
            {
                return await _repository.GetQuerySingleOrDefaultAsync<decimal>($"select dbo.GetCreditTermDays ({CompanyId},{CreditTermId},'{TrnsDate}')");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DecSetting,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_BaseSettings",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<DocSeqNoViewModel> GetDocSeqNoByTransactionAsync(short CompanyId, short UserId, Int16 ModuleId, Int16 TransactionId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DocSeqNoViewModel>($"SELECT CompanyId,ModuleId,TransactionId,H_ReferenceNo,H_TrnDate,H_AccountDate,H_DeliveryDate,H_DueDate,H_CustomerId,H_CurrencyId,H_ExhRate,H_CtyExhRate,H_CreditTermId,H_DocSeqNoId,H_InvoiceNo,H_TotAmt,H_TotLocalAmt,H_TotCtyAmt,H_GstClaimDate,H_GstAmt,H_GstLocalAmt,H_GstCtyAmt,H_TotAmtAftGst,H_TotLocalAmtAftGst,H_TotCtyAmtAftGst,H_SalesOrderNo,H_OperationNo,H_Remarks,H_Address1,H_Address2,H_Address3,H_Address4,H_PinCode,H_CountryId,H_PhoneNo,H_FaxNo,H_ContactName,H_MobileNo,H_EmailAdd,H_SupplierName,H_SuppInvoiceNo,H_APInvoiceNo,D_SeqNo,D_ProductId,D_GLId,D_QTY,D_BillQTY,D_UomId,D_UnitPrice,D_TotAmt,D_TotLocalAmt,D_TotCtyAmt,D_Remarks,D_GstId,D_GstPercentage,D_GstAmt,D_GstLocalAmt,D_GstCtyAmt,D_DeliveryDate,D_DepartmentId,D_EmployeeId,D_PortId,D_VesselId,D_BargeId,D_VoyageId,D_OperationNo,D_OPRefNo,D_SalesOrderNo,D_SupplyDate,D_SupplierName,D_SuppInvoiceNo,D_APInvoiceNo,CreateById,CreateDate,EditById,EditDate FROM S_DocSeqNo where ModuleId={ModuleId} And TransactionId={TransactionId} And CompanyId in (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.DocSeqNo}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocSeqNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DocSeqNo",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<DocSeqNoViewModel> GetDocSeqNoAsync(short CompanyId, short UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DocSeqNoViewModel>($"SELECT DocSeq.CompanyId,DocSeq.ModuleId,DocSeq.TransactionId,H_ReferenceNo,H_TrnDate,H_AccountDate,H_DeliveryDate,H_DueDate,H_CustomerId,H_CurrencyId,H_ExhRate,H_CtyExhRate,H_CreditTermId,H_DocSeqNoId,H_InvoiceNo,H_TotAmt,H_TotLocalAmt,H_TotCtyAmt,H_GstClaimDate,H_GstAmt,H_GstLocalAmt,H_GstCtyAmt,H_TotAmtAftGst,H_TotLocalAmtAftGst,H_TotCtyAmtAftGst,H_SalesOrderNo,H_OperationNo,H_Remarks,H_Address1,H_Address2,H_Address3,H_Address4,H_PinCode,H_CountryId,H_PhoneNo,H_FaxNo,H_ContactName,H_MobileNo,H_EmailAdd,H_SupplierName,H_SuppInvoiceNo,H_APInvoiceNo,D_SeqNo,D_ProductId,D_GLId,D_QTY,D_BillQTY,D_UomId,D_UnitPrice,D_TotAmt,D_TotLocalAmt,D_TotCtyAmt,D_Remarks,D_GstId,D_GstPercentage,D_GstAmt,D_GstLocalAmt,D_GstCtyAmt,D_DeliveryDate,D_DepartmentId,D_EmployeeId,D_PortId,D_VesselId,D_BargeId,D_VoyageId,D_OperationNo,D_OPRefNo,D_SalesOrderNo,D_SupplyDate,D_SupplierName,D_SuppInvoiceNo,D_APInvoiceNo,DocSeq.CreateById,Usr.UserCode CreateBy, DocSeq.CreateDate, DocSeq.EditById,Usr1.UserCode EditBy,DocSeq.EditDate FROM S_DocSeqNo DocSeq Left Join AdmUser Usr on Usr.UserId=DocSeq.CreatebyId Left Join AdmUser Usr1 on Usr1.UserId=DocSeq.CreatebyId where CompanyId in (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.DocSeqNo}))");
                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DocSeqNo,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DocSeqNo",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> UpsertDocSeqNoAsync(short CompanyId, short UserId, S_DocSeqNo s_DocSeqNo)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM S_DocSeqNo WHERE ModuleId={s_DocSeqNo.ModuleId} And TransactionId={s_DocSeqNo.TransactionId} And CompanyId in (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Setting},{(short)E_Setting.DocSeqNo}))");

                    if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_DocSeqNo);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        s_DocSeqNo.EditById = null;
                        s_DocSeqNo.EditDate = null;
                        _context.Add(s_DocSeqNo);
                    }

                    var FinSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (FinSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DocSeqNo,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_DocSeqNo",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "Document Seqence Settings Save Successfully",
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.DocSeqNo, 0, "", "S_DocSeqNo", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.DocSeqNo, 0, "", "S_DocSeqNo", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<DynamicLookupViewModel> GetDynamicLookupAsync(short CompanyId, short UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DynamicLookupViewModel>($"SELECT CompanyId,IsBarge,IsVessel,IsVoyage,IsCustomer,IsSupplier,IsProduct,CreateById,CreateDate,EditById,EditDate FROM S_DynamicLookup WHERE CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Setting,
                    TransactionId = (short)E_Setting.DynamicLookup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "S_DynamicLookup",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveDynamicLookupAsync(short CompanyId, short UserId, S_DynamicLookup s_DynamicLookup)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM S_DynamicLookup WHERE CompanyId = {s_DynamicLookup.CompanyId}");

                    if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                    {
                        var entity = _context.Update(s_DynamicLookup);
                        entity.Property(b => b.CreateById).IsModified = false;
                        entity.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        s_DynamicLookup.EditById = null;
                        s_DynamicLookup.EditDate = null;
                        _context.Add(s_DynamicLookup);
                    }

                    var FinSettingsToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (FinSettingsToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Setting,
                            TransactionId = (short)E_Setting.DynamicLookup,
                            DocumentId = 0,
                            DocumentNo = "",
                            TblName = "S_DynamicLookup",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "Dynamic Lookup Settings Save Successfully",
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Setting, E_Setting.DynamicLookup, 0, "", "S_DynamicLookup", E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Setting, E_Setting.DynamicLookup, 0, "", "S_DynamicLookup", E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
    }
}