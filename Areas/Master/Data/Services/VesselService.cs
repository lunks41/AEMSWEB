using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Helpers;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class VesselService : IVesselService
    {
        private readonly IRepository<M_Vessel> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public VesselService(IRepository<M_Vessel> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<VesselViewModelCount> GetVesselListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            VesselViewModelCount countViewModel = new VesselViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Vessel M_Vess WHERE (M_Vess.VesselName LIKE '%{searchString}%' OR M_Vess.VesselCode LIKE '%{searchString}%' OR M_Vess.VesselType LIKE '%{searchString}%') AND M_Vess.VesselId <>0 AND M_Vess.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Vessel}))");

                var result = await _repository.GetQueryAsync<VesselViewModel>($"SELECT M_Vess.VesselId,M_Vess.CompanyId,M_Vess.VesselCode,M_Vess.VesselName,M_Vess.CallSign,M_Vess.IMOCode,M_Vess.GRT,M_Vess.LicenseNo,M_Vess.VesselType,M_Vess.Flag,M_Vess.Remarks,M_Vess.IsActive,M_Vess.CreateById,M_Vess.CreateDate,M_Vess.EditById,M_Vess.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_Vessel M_Vess LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Vess.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Vess.EditById WHERE (M_Vess.VesselName LIKE '%{searchString}%' OR M_Vess.VesselCode LIKE '%{searchString}%' OR M_Vess.VesselType LIKE '%{searchString}%') AND M_Vess.VesselId <>0 AND M_Vess.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Vessel})) ORDER BY M_Vess.VesselName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
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
                    TransactionId = (short)E_Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Vessel> GetVesselByIdAsync(short CompanyId, short UserId, int VesselId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Vessel>($"SELECT VesselId,CompanyId,VesselCode,VesselName,CallSign,IMOCode,GRT,LicenseNo,VesselType,Flag,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Vessel WHERE VesselId={VesselId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Vessel}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveVesselAsync(short CompanyId, short UserId, M_Vessel m_Vessel)
        {
            bool IsEdit = m_Vessel.VesselId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Vessel WHERE VesselId<>@VesselId AND VesselCode=@VesselCode",
                        new { m_Vessel.VesselId, m_Vessel.VesselCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Vessel Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Vessel WHERE VesselId<>@VesselId AND VesselName=@VesselName",
                        new { m_Vessel.VesselId, m_Vessel.VesselName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Vessel Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Vessel WHERE VesselId=@VesselId",
                            new { m_Vessel.VesselId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Vessel);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Vessel Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (VesselId + 1) FROM dbo.M_Vessel WHERE (VesselId + 1) NOT IN (SELECT VesselId FROM dbo.M_Vessel)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Vessel.VesselId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Vessel);
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Vessel,
                            DocumentId = m_Vessel.VesselId,
                            DocumentNo = m_Vessel.VesselCode,
                            TblName = "M_Vessel",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Vessel Save Successfully",
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Vessel, m_Vessel.VesselId, m_Vessel.VesselCode, "M_Vessel", IsEdit ? E_Mode.Update : E_Mode.Create, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Vessel, m_Vessel.VesselId, m_Vessel.VesselCode, "M_Vessel", IsEdit ? E_Mode.Update : E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteVesselAsync(short CompanyId, short UserId, M_Vessel Vessel)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (Vessel.VesselId > 0)
                    {
                        var VesselToRemove = _context.M_Vessel.Where(x => x.VesselId == Vessel.VesselId).ExecuteDelete();

                        if (VesselToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Vessel,
                                DocumentId = Vessel.VesselId,
                                DocumentNo = Vessel.VesselCode,
                                TblName = "M_Vessel",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Vessel Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "VesselId Should be zero" };
                    }
                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Vessel, Vessel.VesselId, "", "M_Vessel", E_Mode.Delete, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Vessel, Vessel.VesselId, "", "M_Vessel", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
    }
}