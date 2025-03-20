using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class PortRegionService : IPortRegionService
    {
        private readonly IRepository<M_PortRegion> _repository;
        private ApplicationDbContext _context;

        public PortRegionService(IRepository<M_PortRegion> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PortRegionViewModelCount> GetPortRegionListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            var parameters = new DynamicParameters();
            PortRegionViewModelCount countViewModel = new PortRegionViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_PortRegion M_PortRg WHERE M_PortRg.PortRegionId<>0 AND  ( M_PortRg.PortRegionName LIKE '%{searchString}%' OR M_PortRg.PortRegionCode LIKE '%{searchString}%' OR M_PortRg.Remarks LIKE '%{searchString}%') AND  M_PortRg.CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PortRegion}))");

                var result = await _repository.GetQueryAsync<PortRegionViewModel>($"SELECT M_PortRg.PortRegionId,M_PortRg.CompanyId,M_PortRg.PortRegionCode,M_PortRg.PortRegionName,M_PortRg.Remarks,M_PortRg.IsActive,M_PortRg.CreateById,M_PortRg.CreateDate,M_PortRg.EditById,M_PortRg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_PortRegion M_PortRg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_PortRg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_PortRg.EditById WHERE M_PortRg.PortRegionId<>0 AND  ( M_PortRg.PortRegionName LIKE '%{searchString}%' OR M_PortRg.PortRegionCode LIKE '%{searchString}%' OR M_PortRg.Remarks LIKE '%{searchString}%') AND  M_PortRg.CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PortRegion})) ORDER BY M_PortRg.PortRegionName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.PortRegion,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<PortRegionViewModel> GetPortRegionByIdAsync(short CompanyId, short UserId, short PortRegionId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<PortRegionViewModel>($"SELECT M_PortRg.PortRegionId,M_PortRg.CompanyId,M_PortRg.PortRegionCode,M_PortRg.PortRegionName,M_PortRg.Remarks,M_PortRg.IsActive,M_PortRg.CreateById,M_PortRg.CreateDate,M_PortRg.EditById,M_PortRg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_PortRegion M_PortRg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_PortRg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_PortRg.EditById WHERE PortRegionId={PortRegionId} AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PortRegion}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.PortRegion,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SavePortRegionAsync(short CompanyId, short UserId, M_PortRegion m_PortRegion)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_PortRegion.PortRegionId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_PortRegion WHERE PortRegionId<>@PortRegionId AND PortRegionCode=@PortRegionCode",
                        new { m_PortRegion.PortRegionId, m_PortRegion.PortRegionCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Port Region Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_PortRegion WHERE PortRegionId<>@PortRegionId AND PortRegionName=@PortRegionName",
                        new { m_PortRegion.PortRegionId, m_PortRegion.PortRegionName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Port Region Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_PortRegion WHERE PortRegionId=@PortRegionId",
                            new { m_PortRegion.PortRegionId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_PortRegion);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Port Region Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (PortRegionId + 1) FROM dbo.M_PortRegion WHERE (PortRegionId + 1) NOT IN (SELECT PortRegionId FROM dbo.M_PortRegion)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_PortRegion.PortRegionId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_PortRegion);
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
                            TransactionId = (short)E_Master.PortRegion,
                            DocumentId = m_PortRegion.PortRegionId,
                            DocumentNo = m_PortRegion.PortRegionCode,
                            TblName = "M_PortRegion",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Port Region Save Successfully",
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.PortRegion,
                        DocumentId = m_PortRegion.PortRegionId,
                        DocumentNo = m_PortRegion.PortRegionCode,
                        TblName = "M_PortRegion",
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

        public async Task<SqlResponse> DeletePortRegionAsync(short CompanyId, short UserId, PortRegionViewModel PortRegion)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (PortRegion.PortRegionId > 0)
                    {
                        var PortRegionToRemove = _context.M_PortRegion.Where(x => x.PortRegionId == PortRegion.PortRegionId).ExecuteDelete();

                        if (PortRegionToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.PortRegion,
                                DocumentId = PortRegion.PortRegionId,
                                DocumentNo = PortRegion.PortRegionCode,
                                TblName = "M_PortRegion",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "PortRegion Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "PortRegionId Should be zero" };
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
                        TransactionId = (short)E_Master.PortRegion,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_PortRegion",
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
}