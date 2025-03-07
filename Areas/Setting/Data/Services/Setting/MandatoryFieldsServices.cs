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
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AEMSWEB.Services.Setting
{
    public sealed class MandatoryFieldsServices : IMandatoryFieldsServices
    {
        private readonly IRepository<S_MandatoryFields> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public MandatoryFieldsServices(IRepository<S_MandatoryFields> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<MandatoryFieldsViewModel> GetMandatoryFieldsByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId)
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

        public async Task<IEnumerable<MandatoryFieldsViewModel>> GetMandatoryFieldsByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 UserId)
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

        public async Task<SqlResponse> SaveMandatoryFieldsAsync(Int16 CompanyId, List<S_MandatoryFields> s_MandatoryFields, short UserId)
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

                    //// Remove existing records
                    //var existingEntities = await _context.S_MandatoryFields
                    //    .Where(x => x.ModuleId == Convert.ToByte(s_MandatoryFields[0].ModuleId) && x.CompanyId == Convert.ToByte(CompanyId))
                    //    .ToListAsync();

                    //if (existingEntities.Any())
                    //{
                    //    _context.S_MandatoryFields.RemoveRange(existingEntities);
                    //    await _context.SaveChangesAsync();
                    //}

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
    }
}