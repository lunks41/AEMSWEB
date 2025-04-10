using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Areas.Master.Models;
using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.Helpers;
using AMESWEB.Models;
using AMESWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Master.Data.Services
{
    public sealed class PortService : IPortService
    {
        private readonly IRepository<M_Port> _repository;
        private ApplicationDbContext _context;

        public PortService(IRepository<M_Port> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PortViewModelCount> GetPortListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            PortViewModelCount countViewModel = new PortViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Port M_Pro INNER JOIN dbo.M_PortRegion M_Prg ON M_Prg.PortRegionId = M_Pro.PortRegionId WHERE (M_Pro.PortName LIKE '%{searchString}%' OR M_Pro.PortCode LIKE '%{searchString}%' OR M_Pro.Remarks LIKE '%{searchString}%'OR M_Prg.PortRegionCode LIKE '%{searchString}%' OR M_Prg.PortRegionName LIKE '%{searchString}%') AND M_Pro.PortId<>0 AND M_Pro.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port}))");

                var result = await _repository.GetQueryAsync<PortViewModel>($"SELECT M_Pro.PortId,M_Pro.CompanyId,M_Pro.PortRegionId,M_Prg.PortRegionCode,M_Prg.PortRegionName,M_Pro.PortCode,M_Pro.PortName,M_Pro.Remarks,M_Pro.IsActive,M_Pro.CreateById,M_Pro.CreateDate,M_Pro.EditById,M_Pro.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Port M_Pro INNER JOIN dbo.M_PortRegion M_Prg ON M_Prg.PortRegionId = M_Pro.PortRegionId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Pro.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Pro.EditById WHERE (M_Pro.PortName LIKE '%{searchString}%' OR M_Pro.PortCode LIKE '%{searchString}%' OR M_Pro.Remarks LIKE '%{searchString}%'OR M_Prg.PortRegionCode LIKE '%{searchString}%' OR M_Prg.PortRegionName LIKE '%{searchString}%') AND M_Pro.PortId<>0 AND M_Pro.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port})) ORDER BY M_Pro.PortName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<PortViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Port,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<PortViewModel> GetPortByIdAsync(short CompanyId, short UserId, short PortId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<PortViewModel>($"SELECT M_Pro.PortId,M_Pro.CompanyId,M_Pro.PortRegionId,M_Prg.PortRegionCode,M_Prg.PortRegionName,M_Pro.PortCode,M_Pro.PortName,M_Pro.Remarks,M_Pro.IsActive,M_Pro.CreateById,M_Pro.CreateDate,M_Pro.EditById,M_Pro.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Port M_Pro INNER JOIN M_PortRegion M_Prg ON M_Prg.PortRegionId = M_Pro.PortRegionId LEFT JOIN AdmUser Usr ON Usr.UserId = M_Pro.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_Pro.EditById WHERE M_Pro.PortId={PortId} AND M_Pro.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Port,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SavePortAsync(short CompanyId, short UserId, M_Port m_Port)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Port.PortId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Port WHERE PortId<>@PortId AND PortCode=@PortCode",
                        new { m_Port.PortId, m_Port.PortCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Port Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Port WHERE PortId<>@PortId AND PortName=@PortName",
                        new { m_Port.PortId, m_Port.PortName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Port Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Port WHERE PortId=@PortId",
                            new { m_Port.PortId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Port);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Port Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT ISNULL((SELECT TOP 1 (PortId + 1) FROM M_Port WHERE (PortId + 1) NOT IN (SELECT PortId FROM M_Port)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Port.PortId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Port);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
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
                            TransactionId = (short)E_Master.Port,
                            DocumentId = m_Port.PortId,
                            DocumentNo = m_Port.PortCode,
                            TblName = "M_Port",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Port Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponce { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Port,
                        DocumentId = m_Port.PortId,
                        DocumentNo = m_Port.PortCode,
                        TblName = "M_Port",
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

        public async Task<SqlResponce> DeletePortAsync(short CompanyId, short UserId, short portId)
        {
            string portNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    portNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT PortCode FROM dbo.M_Port WHERE PortId={portId}");

                    if (portId > 0)
                    {
                        var accountGroupToRemove = _context.M_Port
                            .Where(x => x.PortId == portId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Port,
                                DocumentId = portId,
                                DocumentNo = portNo,
                                TblName = "M_Port",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Port Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "PortId Should be zero" };
                    }
                    return new SqlResponce();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Port,
                    DocumentId = portId,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Port,
                    DocumentId = portId,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)E_Mode.Delete,
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