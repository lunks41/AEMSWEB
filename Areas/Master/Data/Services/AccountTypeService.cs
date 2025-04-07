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
    public sealed class AccountTypeService : IAccountTypeService
    {
        private readonly IRepository<M_AccountType> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public AccountTypeService(IRepository<M_AccountType> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<AccountTypeViewModelCount> GetAccountTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            AccountTypeViewModelCount countViewModel = new AccountTypeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_AccountType  M_ACC LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById WHERE (M_ACC.AccTypeName LIKE '%{searchString}%' OR M_ACC.AccTypeCode LIKE '%{searchString}%' OR M_ACC.AccGroupName LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%') AND M_ACC.AccTypeId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountType}))");

                var result = await _repository.GetQueryAsync<AccountTypeViewModel>($"SELECT M_ACC.CompanyId,M_ACC.AccTypeId,M_ACC.AccTypeCode,M_ACC.AccTypeName,M_ACC.CodeStart,M_ACC.CodeEnd,M_ACC.SeqNo,M_ACC.AccGroupName,M_ACC.Remarks,M_ACC.IsActive,M_ACC.CreateById,M_ACC.CreateDate,M_ACC.EditById,M_ACC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_AccountType M_ACC LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById WHERE (M_ACC.AccTypeName LIKE '%{searchString}%' OR M_ACC.AccTypeCode LIKE '%{searchString}%' OR M_ACC.AccGroupName LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%') AND M_ACC.AccTypeId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountType})) ORDER BY M_ACC.AccTypeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<AccountTypeViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountType",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<AccountTypeViewModel> GetAccountTypeByIdAsync(short CompanyId, short UserId, short AccTypeId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AccountTypeViewModel>($"SELECT AccTypeId,CompanyId,AccTypeCode,AccTypeName,CodeStart,CodeEnd,SeqNo,AccGroupName,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountType WHERE AccTypeId={AccTypeId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountType}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountType",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveAccountTypeAsync(short CompanyId, short UserId, M_AccountType m_AccountType)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_AccountType.AccTypeId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_AccountType WHERE AccTypeId<>@AccTypeId AND AccTypeCode=@AccTypeCode",
                        new { m_AccountType.AccTypeId, m_AccountType.AccTypeCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "AccountType Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_AccountType WHERE AccTypeId<>@AccTypeId AND AccTypeName=@AccTypeName",
                        new { m_AccountType.AccTypeId, m_AccountType.AccTypeName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "AccountType Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_AccountType WHERE AccTypeId=@AccTypeId",
                            new { m_AccountType.AccTypeId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_AccountType);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "AccountType Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (AccTypeId + 1) FROM dbo.M_AccountType WHERE (AccTypeId + 1) NOT IN (SELECT AccTypeId FROM dbo.M_AccountType)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_AccountType.AccTypeId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_AccountType);
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
                            TransactionId = (short)E_Master.AccountType,
                            DocumentId = m_AccountType.AccTypeId,
                            DocumentNo = m_AccountType.AccTypeCode,
                            TblName = "M_AccountType",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "AccountType Save Successfully",
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
                        TransactionId = (short)E_Master.AccountType,
                        DocumentId = m_AccountType.AccTypeId,
                        DocumentNo = m_AccountType.AccTypeCode,
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

        public async Task<SqlResponse> DeleteAccountTypeAsync(short CompanyId, short UserId, short accTypeId)
        {
            string accTypeNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    accTypeNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT AccTypeCode FROM dbo.M_AccountType WHERE AccTypeId={accTypeId}");

                    if (accTypeId > 0)
                    {
                        var accountTypeToRemove = _context.M_AccountType
                            .Where(x => x.AccTypeId == accTypeId)
                            .ExecuteDelete();

                        if (accountTypeToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.AccountType,
                                DocumentId = accTypeId,
                                DocumentNo = accTypeNo,
                                TblName = "M_AccountType",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "AccountType Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "AccTypeId Should be zero" };
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
                    TransactionId = (short)E_Master.AccountType,
                    DocumentId = accTypeId,
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
                    TransactionId = (short)E_Master.AccountType,
                    DocumentId = accTypeId,
                    DocumentNo = "",
                    TblName = "M_AccountType",
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