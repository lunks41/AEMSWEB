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
    public sealed class VisibleFieldsServices : IVisibleFieldsServices
    {
        private readonly IRepository<S_VisibleFields> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public VisibleFieldsServices(IRepository<S_VisibleFields> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<VisibleFieldsViewModel> GetVisibleFieldsByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId)
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

        public async Task<IEnumerable<VisibleFieldsViewModel>> GetVisibleFieldsByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 UserId)
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

        public async Task<SqlResponse> SaveVisibleFieldsAsync(short CompanyId, List<S_VisibleFields> s_VisibleFields, short UserId)
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

                    //// Remove existing records
                    //var existingEntities = await _context.S_VisibleFields
                    //    .Where(x => x.ModuleId == s_VisibleFields[0].ModuleId && x.CompanyId == CompanyId)
                    //    .ToListAsync();

                    //if (existingEntities.Any())
                    //{
                    //    _context.S_VisibleFields.RemoveRange(existingEntities);
                    //    await _context.SaveChangesAsync();
                    //}

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
    }
}