using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.Helpers;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Models.Masters;
using AMESWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Master.Data.Services
{
    public sealed class BargeService : IBargeService
    {
        private readonly IRepository<M_Barge> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public BargeService(IRepository<M_Barge> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<BargeViewModelCount> GetBargeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            BargeViewModelCount countViewModel = new BargeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Barge M_Brg WHERE (M_Brg.BargeName LIKE '%{searchString}%' OR M_Brg.BargeCode LIKE '%{searchString}%' OR M_Brg.Remarks LIKE '%{searchString}%') AND M_Brg.BargeId<>0 AND M_Brg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Barge}))");

                var result = await _repository.GetQueryAsync<BargeViewModel>($"SELECT M_Brg.BargeId,M_Brg.CompanyId,M_Brg.BargeCode,M_Brg.BargeName,M_Brg.CallSign,M_Brg.IMOCode,M_Brg.GRT,M_Brg.LicenseNo,M_Brg.BargeType,M_Brg.Flag,M_Brg.Remarks,M_Brg.IsActive,M_Brg.CreateById,M_Brg.CreateDate,M_Brg.EditById,M_Brg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Barge M_Brg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Brg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Brg.EditById WHERE (M_Brg.BargeName LIKE '%{searchString}%' OR M_Brg.BargeCode LIKE '%{searchString}%' OR M_Brg.Remarks LIKE '%{searchString}%') AND M_Brg.BargeId<>0 AND M_Brg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Barge})) ORDER BY M_Brg.BargeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<BargeViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BargeViewModel> GetBargeByIdAsync(short CompanyId, short UserId, short BargeId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<BargeViewModel>($"SELECT M_Brg.BargeId,M_Brg.CompanyId,M_Brg.BargeCode,M_Brg.BargeName,M_Brg.CallSign,M_Brg.IMOCode,M_Brg.GRT,M_Brg.LicenseNo,M_Brg.BargeType,M_Brg.Flag,M_Brg.Remarks,M_Brg.IsActive,M_Brg.CreateById,M_Brg.CreateDate,M_Brg.EditById,M_Brg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Barge M_Brg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Brg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Brg.EditById WHERE BargeId={BargeId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Barge}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveBargeAsync(short CompanyId, short UserId, M_Barge m_Barge)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Barge.BargeId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Barge WHERE BargeId<>@BargeId AND BargeCode=@BargeCode",
                        new { m_Barge.BargeId, m_Barge.BargeCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Barge Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Barge WHERE BargeId<>@BargeId AND BargeName=@BargeName",
                        new { m_Barge.BargeId, m_Barge.BargeName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Barge Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Barge WHERE BargeId=@BargeId",
                            new { m_Barge.BargeId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Barge);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Barge Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT ISNULL((SELECT TOP 1 (BargeId + 1) FROM dbo.M_Barge WHERE (BargeId + 1) NOT IN (SELECT BargeId FROM dbo.M_Barge)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Barge.BargeId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Barge);
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
                            TransactionId = (short)E_Master.Barge,
                            DocumentId = m_Barge.BargeId,
                            DocumentNo = m_Barge.BargeCode,
                            TblName = "M_Barge",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Barge Save Successfully",
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
                catch (SqlException sqlEx)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.COACategory1,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_COACategory1",
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
                        TransactionId = (short)E_Master.Barge,
                        DocumentId = m_Barge.BargeId,
                        DocumentNo = m_Barge.BargeCode,
                        TblName = "AdmUser",
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

        public async Task<SqlResponce> DeleteBargeAsync(short CompanyId, short UserId, short bargeId)
        {
            string bargeNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    bargeNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT BargeCode FROM dbo.M_Barge WHERE BargeId={bargeId}");

                    if (bargeId > 0)
                    {
                        var accountGroupToRemove = _context.M_Barge
                            .Where(x => x.BargeId == bargeId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Barge,
                                DocumentId = bargeId,
                                DocumentNo = bargeNo,
                                TblName = "M_Barge",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Barge Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "BargeId Should be zero" };
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
                    TransactionId = (short)E_Master.Barge,
                    DocumentId = bargeId,
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
                    TransactionId = (short)E_Master.Barge,
                    DocumentId = bargeId,
                    DocumentNo = "",
                    TblName = "M_Barge",
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