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
    public sealed class AccountSetupCategoryService : IAccountSetupCategoryService
    {
        private readonly IRepository<M_AccountSetupCategory> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public AccountSetupCategoryService(IRepository<M_AccountSetupCategory> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            AccountSetupCategoryViewModelCount countViewModel = new AccountSetupCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_AccountSetupCategory M_AccSetCa  WHERE M_AccSetCa.AccSetupCategoryId<>0 AND  ( M_AccSetCa.AccSetupCategoryName LIKE '%{searchString}%' OR M_AccSetCa.AccSetupCategoryCode LIKE '%{searchString}%' OR M_AccSetCa.Remarks LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<AccountSetupCategoryViewModel>($"SELECT M_AccSetCa.AccSetupCategoryId,M_AccSetCa.AccSetupCategoryCode,M_AccSetCa.AccSetupCategoryName,M_AccSetCa.Remarks,M_AccSetCa.IsActive,M_AccSetCa.CreateById,M_AccSetCa.CreateDate,M_AccSetCa.EditById,M_AccSetCa.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_AccountSetupCategory M_AccSetCa LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_AccSetCa.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_AccSetCa.EditById WHERE M_AccSetCa.AccSetupCategoryId<>0 AND  ( M_AccSetCa.AccSetupCategoryName LIKE '%{searchString}%' OR M_AccSetCa.AccSetupCategoryCode LIKE '%{searchString}%' OR M_AccSetCa.Remarks LIKE '%{searchString}%') AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory})) ORDER BY M_AccSetCa.AccSetupCategoryName");

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
                    TransactionId = (short)E_Master.AccountSetupCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_AccountSetupCategory> GetAccountSetupCategoryByIdAsync(short CompanyId, short UserId, short AccSetupCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_AccountSetupCategory>($"SELECT AccSetupCategoryId,AccSetupCategoryCode,AccSetupCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId={AccSetupCategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetupCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveAccountSetupCategoryAsync(short CompanyId, short UserId, M_AccountSetupCategory m_AccountSetupCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_AccountSetupCategory.AccSetupCategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId<>@AccSetupCategoryId AND AccSetupCategoryCode=@AccSetupCategoryCode", new { m_AccountSetupCategory.AccSetupCategoryId, m_AccountSetupCategory.AccSetupCategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "AccSetupCategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId<>@AccSetupCategoryId AND AccSetupCategoryName=@AccSetupCategoryName", new { m_AccountSetupCategory.AccSetupCategoryId, m_AccountSetupCategory.AccSetupCategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "AccSetupCategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId=@AccSetupCategoryId", new { m_AccountSetupCategory.AccSetupCategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_AccountSetupCategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "AccountSetupCategory Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (AccSetupCategoryId + 1) FROM dbo.M_AccountSetupCategory WHERE (AccSetupCategoryId + 1) NOT IN (SELECT AccSetupCategoryId FROM dbo.M_AccountSetupCategory)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_AccountSetupCategory.AccSetupCategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_AccountSetupCategory);
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
                            TransactionId = (short)E_Master.AccountSetupCategory,
                            DocumentId = m_AccountSetupCategory.AccSetupCategoryId,
                            DocumentNo = m_AccountSetupCategory.AccSetupCategoryCode,
                            TblName = "M_AccountSetupCategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "AccountSetupCategory Save Successfully",
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
                        TransactionId = (short)E_Master.AccountSetupCategory,
                        DocumentId = m_AccountSetupCategory.AccSetupCategoryId,
                        DocumentNo = m_AccountSetupCategory.AccSetupCategoryCode,
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

        public async Task<SqlResponse> DeleteAccountSetupCategoryAsync(short CompanyId, short UserId, M_AccountSetupCategory accountSetupCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (accountSetupCategory.AccSetupCategoryId > 0)
                    {
                        var accountSetupCategoryToRemove = _context.M_AccountSetupCategory.Where(x => x.AccSetupCategoryId == accountSetupCategory.AccSetupCategoryId).ExecuteDelete();

                        if (accountSetupCategoryToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.AccountSetupCategory,
                                DocumentId = accountSetupCategory.AccSetupCategoryId,
                                DocumentNo = accountSetupCategory.AccSetupCategoryCode,
                                TblName = "M_AccountSetupCategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "AccountSetupCategory Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "AccSetupCategoryId Should be zero" };
                    }
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
                        TransactionId = (short)E_Master.AccountSetupCategory,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_AccountSetupCategory",
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