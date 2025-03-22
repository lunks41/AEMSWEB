using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Helpers;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class ChartOfAccountService : IChartOfAccountService
    {
        private readonly IRepository<M_ChartOfAccount> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public ChartOfAccountService(IRepository<M_ChartOfAccount> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<ChartOfAccountViewModelCount> GetChartOfAccountListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            ChartOfAccountViewModelCount countViewModel = new ChartOfAccountViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_ChartOfAccount M_ChtAcc INNER JOIN dbo.M_AccountType M_AccTy ON M_AccTy.AccTypeId = M_ChtAcc.AccTypeId INNER JOIN dbo.M_AccountGroup M_AccGrp ON M_AccGrp.AccGroupId = M_ChtAcc.AccGroupId LEFT JOIN dbo.M_COACategory1 M_Coa1 ON M_Coa1.COACategoryId = M_ChtAcc.COACategoryId1 LEFT JOIN dbo.M_COACategory2 M_Coa2 ON M_Coa2.COACategoryId = M_ChtAcc.COACategoryId2 LEFT JOIN dbo.M_COACategory3 M_Coa3 ON M_Coa3.COACategoryId = M_ChtAcc.COACategoryId3 WHERE (M_ChtAcc.GLName LIKE '%{searchString}%' OR M_ChtAcc.GLCode LIKE '%{searchString}%' OR M_ChtAcc.Remarks LIKE '%{searchString}%' OR M_AccTy.AccTypeCode LIKE '%{searchString}%'OR M_AccTy.AccTypeName LIKE '%{searchString}%' OR M_AccGrp.AccGroupCode LIKE '%{searchString}%'OR M_AccGrp.AccGroupName LIKE '%{searchString}%' OR M_Coa1.COACategoryName LIKE '%{searchString}%' OR M_Coa2.COACategoryCode LIKE '%{searchString}%' OR M_Coa2.COACategoryName LIKE '%{searchString}%' OR M_Coa3.COACategoryCode LIKE '%{searchString}%' OR M_Coa3.COACategoryName LIKE '%{searchString}%') AND M_ChtAcc.GLId<>0 AND M_ChtAcc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.ChartOfAccount}))");

                var result = await _repository.GetQueryAsync<ChartOfAccountViewModel>($"SELECT M_ChtAcc.GLId,M_ChtAcc.CompanyId,M_ChtAcc.GLCode,M_ChtAcc.GLName,M_ChtAcc.AccTypeId,M_AccTy.AccTypeCode,M_AccTy.AccTypeName,M_ChtAcc.AccGroupId,M_AccGrp.AccGroupCode,M_AccGrp.AccGroupName,M_ChtAcc.COACategoryId1,M_Coa1.COACategoryCode AS COACategoryCode1,M_Coa1.COACategoryName AS COACategoryName1,M_ChtAcc.COACategoryId2,M_Coa2.COACategoryCode AS COACategoryCode2,M_Coa2.COACategoryName AS COACategoryName2,M_ChtAcc.COACategoryId3,M_Coa3.COACategoryCode AS COACategoryCode3,M_Coa3.COACategoryName AS COACategoryName3,M_ChtAcc.IsSysControl,M_ChtAcc.seqNo,M_ChtAcc.Remarks,M_ChtAcc.IsActive,M_ChtAcc.CreateById,M_ChtAcc.CreateDate,M_ChtAcc.EditById,M_ChtAcc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_ChartOfAccount M_ChtAcc INNER JOIN dbo.M_AccountType M_AccTy ON M_AccTy.AccTypeId = M_ChtAcc.AccTypeId INNER JOIN dbo.M_AccountGroup M_AccGrp ON M_AccGrp.AccGroupId = M_ChtAcc.AccGroupId LEFT JOIN dbo.M_COACategory1 M_Coa1 ON M_Coa1.COACategoryId = M_ChtAcc.COACategoryId1 LEFT JOIN dbo.M_COACategory2 M_Coa2 ON M_Coa2.COACategoryId = M_ChtAcc.COACategoryId2 LEFT JOIN dbo.M_COACategory3 M_Coa3 ON M_Coa3.COACategoryId = M_ChtAcc.COACategoryId3 LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ChtAcc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ChtAcc.EditById WHERE (M_ChtAcc.GLName LIKE '%{searchString}%' OR M_ChtAcc.GLCode LIKE '%{searchString}%' OR M_ChtAcc.Remarks LIKE '%{searchString}%' OR M_AccTy.AccTypeCode LIKE '%{searchString}%'OR M_AccTy.AccTypeName LIKE '%{searchString}%' OR M_AccGrp.AccGroupCode LIKE '%{searchString}%'OR M_AccGrp.AccGroupName LIKE '%{searchString}%' OR M_Coa1.COACategoryName LIKE '%{searchString}%' OR M_Coa2.COACategoryCode LIKE '%{searchString}%' OR M_Coa2.COACategoryName LIKE '%{searchString}%' OR M_Coa3.COACategoryCode LIKE '%{searchString}%' OR M_Coa3.COACategoryName LIKE '%{searchString}%') AND M_ChtAcc.GLId<>0 AND M_ChtAcc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.ChartOfAccount})) ORDER BY M_ChtAcc.GLName ");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<ChartOfAccountViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.ChartOfAccount,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_ChartOfAccount",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ChartOfAccountViewModel> GetChartOfAccountByIdAsync(short CompanyId, short UserId, short GLId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<ChartOfAccountViewModel>($"SELECT M_ChtAcc.GLId,M_ChtAcc.CompanyId,M_ChtAcc.GLCode,M_ChtAcc.GLName,M_ChtAcc.AccTypeId,M_ChtAcc.AccGroupId,M_ChtAcc.COACategoryId1,M_ChtAcc.COACategoryId2,M_ChtAcc.COACategoryId3,M_ChtAcc.IsSysControl,M_ChtAcc.seqNo,M_ChtAcc.Remarks,M_ChtAcc.IsActive,M_ChtAcc.CreateById,M_ChtAcc.CreateDate,M_ChtAcc.EditById,M_ChtAcc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_ChartOfAccount M_ChtAcc LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ChtAcc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ChtAcc.EditById WHERE M_ChtAcc.GLId={GLId} AND M_ChtAcc.CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.ChartOfAccount}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.ChartOfAccount,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_ChartOfAccount",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveChartOfAccountAsync(short CompanyId, short UserId, M_ChartOfAccount m_ChartOfAccount)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_ChartOfAccount.GLId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT TOP (1) 1 AS IsExist FROM dbo.M_ChartOfAccount WHERE GLId<>@GLId AND GLCode=@GLCode AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                        new { m_ChartOfAccount.GLId, m_ChartOfAccount.GLCode, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.ChartOfAccount });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "GL Code exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT TOP (1) 1 AS IsExist FROM dbo.M_ChartOfAccount WHERE GLId<>@GLId AND GLName=@GLName AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                        new { m_ChartOfAccount.GLId, m_ChartOfAccount.GLName, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.ChartOfAccount });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "GL Name exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT TOP (1) 1 AS IsExist FROM dbo.M_ChartOfAccount WHERE GLId=@GLId AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                            new { m_ChartOfAccount.GLId, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.ChartOfAccount });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_ChartOfAccount);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "GL Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (GLId + 1) FROM dbo.M_ChartOfAccount WHERE (GLId + 1) NOT IN (SELECT GLId FROM dbo.M_ChartOfAccount)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_ChartOfAccount.GLId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_ChartOfAccount);
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
                            TransactionId = (short)E_Master.ChartOfAccount,
                            DocumentId = m_ChartOfAccount.GLId,
                            DocumentNo = m_ChartOfAccount.GLCode,
                            TblName = "M_ChartOfAccount",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "ChartOfAccount Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            var dataExist = await _repository.GetQuerySingleOrDefaultAsync<M_ChartOfAccount>(
                                $"SELECT * FROM dbo.M_ChartOfAccount WHERE GLId=@GLId AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                                new { m_ChartOfAccount.GLId, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.ChartOfAccount });

                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully", Data = dataExist };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
                catch (SqlException sqlEx)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.ChartOfAccount,
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

                    return new SqlResponse
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
                        TransactionId = (short)E_Master.ChartOfAccount,
                        DocumentId = m_ChartOfAccount.GLId,
                        DocumentNo = m_ChartOfAccount.GLCode,
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

        public async Task<SqlResponse> DeleteChartOfAccountAsync(short CompanyId, short UserId, short glIdId)
        {
            string glIdNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    glIdNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT GLCode FROM dbo.M_ChartOfAccount WHERE GLId={glIdId}");

                    if (glIdId > 0)
                    {
                        var accountGroupToRemove = _context.M_ChartOfAccount
                            .Where(x => x.GLId == glIdId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.ChartOfAccount,
                                DocumentId = glIdId,
                                DocumentNo = glIdNo,
                                TblName = "M_ChartOfAccount",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "ChartOfAccount Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "ChartOfAccountId Should be zero" };
                    }
                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.ChartOfAccount,
                    DocumentId = glIdId,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
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
                    TransactionId = (short)E_Master.ChartOfAccount,
                    DocumentId = glIdId,
                    DocumentNo = "",
                    TblName = "M_ChartOfAccount",
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