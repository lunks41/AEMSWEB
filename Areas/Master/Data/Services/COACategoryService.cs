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
    public sealed class COACategoryService : ICOACategoryService
    {
        private readonly IRepository<M_COACategory1> _repository;
        private readonly ApplicationDbContext _context; private readonly ILogService _logService;

        public COACategoryService(IRepository<M_COACategory1> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<COACategoryViewModelCount> GetCOACategory1ListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            COACategoryViewModelCount countViewModel = new COACategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_COACategory1 M_Catg WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1}))");

                var result = await _repository.GetQueryAsync<COACategoryViewModel>($"SELECT M_Catg.COACategoryId,M_Catg.COACategoryCode,M_Catg.COACategoryName,M_Catg.seqNo,M_Catg.CompanyId,M_Catg.Remarks,M_Catg.IsActive,M_Catg.CreateById,M_Catg.CreateDate,M_Catg.EditById,M_Catg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_COACategory1 M_Catg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Catg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Catg.EditById WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1})) ORDER BY M_Catg.COACategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<COACategoryViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory1",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<COACategoryViewModel> GetCOACategory1ByIdAsync(short CompanyId, short UserId, short COACategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<COACategoryViewModel>($"SELECT COACategoryId,COACategoryCode,COACategoryName,seqNo,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_COACategory1 WHERE COACategoryId={COACategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory1",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCOACategory1Async(short CompanyId, short UserId, M_COACategory1 m_COACategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_COACategory.COACategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_COACategory1 WHERE COACategoryId<>@COACategoryId AND COACategoryCode=@COACategoryCode",
                        new { m_COACategory.COACategoryId, m_COACategory.COACategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "COACategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_COACategory1 WHERE COACategoryId<>@COACategoryId AND COACategoryName=@COACategoryName",
                        new { m_COACategory.COACategoryId, m_COACategory.COACategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "COACategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_COACategory1 WHERE COACategoryId=@COACategoryId",
                            new { m_COACategory.COACategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_COACategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "COACategory Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (COACategoryId + 1) FROM dbo.M_COACategory1 WHERE (COACategoryId + 1) NOT IN (SELECT COACategoryId FROM dbo.M_COACategory1)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_COACategory.COACategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_COACategory);
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
                            TransactionId = (short)E_Master.COACategory1,
                            DocumentId = m_COACategory.COACategoryId,
                            DocumentNo = m_COACategory.COACategoryCode,
                            TblName = "M_COACategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "COACategory Save Successfully",
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
                        DocumentId = m_COACategory.COACategoryId,
                        DocumentNo = m_COACategory.COACategoryCode,
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
                        TransactionId = (short)E_Master.COACategory1,
                        DocumentId = m_COACategory.COACategoryId,
                        DocumentNo = m_COACategory.COACategoryCode,
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

        public async Task<SqlResponse> DeleteCOACategory1Async(short CompanyId, short UserId, short coaCategoryId)
        {
            string coaCategoryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    coaCategoryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT COACategoryCode FROM dbo.M_COACategory1 WHERE COACategoryId={coaCategoryId}");

                    if (coaCategoryId > 0)
                    {
                        var accountGroupToRemove = _context.M_COACategory1
                            .Where(x => x.COACategoryId == coaCategoryId)
                            .ExecuteDelete();


                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory1,
                                DocumentId = coaCategoryId,
                                DocumentNo = coaCategoryNo,
                                TblName = "M_COACategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "COACategory Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "COACategoryId Should be zero" };
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
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = coaCategoryId,
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
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = coaCategoryId,
                    DocumentNo = "",
                    TblName = "M_COACategory",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<COACategoryViewModelCount> GetCOACategory2ListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            COACategoryViewModelCount countViewModel = new COACategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_COACategory2 M_Catg WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2}))");

                var result = await _repository.GetQueryAsync<COACategoryViewModel>($"SELECT M_Catg.COACategoryId,M_Catg.COACategoryCode,M_Catg.COACategoryName,M_Catg.seqNo,M_Catg.CompanyId,M_Catg.Remarks,M_Catg.IsActive,M_Catg.CreateById,M_Catg.CreateDate,M_Catg.EditById,M_Catg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_COACategory2 M_Catg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Catg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Catg.EditById WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2})) ORDER BY M_Catg.COACategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<COACategoryViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<COACategoryViewModel> GetCOACategory2ByIdAsync(short CompanyId, short UserId, short COACategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<COACategoryViewModel>($"SELECT COACategoryId,COACategoryCode,COACategoryName,seqNo,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_COACategory2 WHERE COACategoryId={COACategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCOACategory2Async(short CompanyId, short UserId, M_COACategory2 m_COACategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_COACategory.COACategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_COACategory2 WHERE COACategoryId<>@COACategoryId AND COACategoryCode=@COACategoryCode",
                        new { m_COACategory.COACategoryId, m_COACategory.COACategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "COACategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_COACategory2 WHERE COACategoryId<>@COACategoryId AND COACategoryName=@COACategoryName",
                        new { m_COACategory.COACategoryId, m_COACategory.COACategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "COACategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_COACategory2 WHERE COACategoryId=@COACategoryId",
                            new { m_COACategory.COACategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_COACategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "COACategory Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (COACategoryId + 1) FROM dbo.M_COACategory2 WHERE (COACategoryId + 1) NOT IN (SELECT COACategoryId FROM dbo.M_COACategory2)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_COACategory.COACategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_COACategory);
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
                            TransactionId = (short)E_Master.COACategory2,
                            DocumentId = m_COACategory.COACategoryId,
                            DocumentNo = m_COACategory.COACategoryCode,
                            TblName = "M_COACategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "COACategory Save Successfully",
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
                        TransactionId = (short)E_Master.COACategory2,
                        DocumentId = m_COACategory.COACategoryId,
                        DocumentNo = m_COACategory.COACategoryCode,
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

        public async Task<SqlResponse> DeleteCOACategory2Async(short CompanyId, short UserId, short coaCategoryId)
        {
            string coaCategoryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    coaCategoryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT COACategoryCode FROM dbo.M_COACategory2 WHERE COACategoryId={coaCategoryId}");

                    if (coaCategoryId > 0)
                    {
                        var accountGroupToRemove = _context.M_COACategory2
                            .Where(x => x.COACategoryId == coaCategoryId)
                            .ExecuteDelete();


                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory2,
                                DocumentId = coaCategoryId,
                                DocumentNo = coaCategoryNo,
                                TblName = "M_COACategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "COACategory Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "COACategoryId Should be zero" };
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
                    TransactionId = (short)E_Master.COACategory2,
                    DocumentId = coaCategoryId,
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
                    TransactionId = (short)E_Master.COACategory2,
                    DocumentId = coaCategoryId,
                    DocumentNo = "",
                    TblName = "M_COACategory",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<COACategoryViewModelCount> GetCOACategory3ListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            COACategoryViewModelCount countViewModel = new COACategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_COACategory3 M_Catg WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory3}))");

                var result = await _repository.GetQueryAsync<COACategoryViewModel>($"SELECT M_Catg.COACategoryId,M_Catg.COACategoryCode,M_Catg.COACategoryName,M_Catg.seqNo,M_Catg.CompanyId,M_Catg.Remarks,M_Catg.IsActive,M_Catg.CreateById,M_Catg.CreateDate,M_Catg.EditById,M_Catg.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_COACategory3 M_Catg LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Catg.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Catg.EditById WHERE (M_Catg.COACategoryName LIKE '%{searchString}%' OR M_Catg.COACategoryCode LIKE '%{searchString}%' OR M_Catg.Remarks LIKE '%{searchString}%') AND M_Catg.COACategoryId<>0 AND M_Catg.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory3})) ORDER BY M_Catg.COACategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<COACategoryViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory3,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory3",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<COACategoryViewModel> GetCOACategory3ByIdAsync(short CompanyId, short UserId, short COACategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<COACategoryViewModel>($"SELECT COACategoryId,COACategoryCode,COACategoryName,seqNo,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_COACategory3 WHERE COACategoryId={COACategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory3}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory3,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory3",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCOACategory3Async(short CompanyId, short UserId, M_COACategory3 m_COACategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_COACategory.COACategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_COACategory3 WHERE COACategoryId<>@COACategoryId AND COACategoryCode=@COACategoryCode",
                        new { m_COACategory.COACategoryId, m_COACategory.COACategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "COACategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_COACategory3 WHERE COACategoryId<>@COACategoryId AND COACategoryName=@COACategoryName",
                        new { m_COACategory.COACategoryId, m_COACategory.COACategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "COACategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_COACategory3 WHERE COACategoryId=@COACategoryId",
                            new { m_COACategory.COACategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_COACategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "COACategory Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (COACategoryId + 1) FROM dbo.M_COACategory3 WHERE (COACategoryId + 1) NOT IN (SELECT COACategoryId FROM dbo.M_COACategory3)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_COACategory.COACategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_COACategory);
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
                            TransactionId = (short)E_Master.COACategory3,
                            DocumentId = m_COACategory.COACategoryId,
                            DocumentNo = m_COACategory.COACategoryCode,
                            TblName = "M_COACategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "COACategory Save Successfully",
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
                        TransactionId = (short)E_Master.COACategory3,
                        DocumentId = m_COACategory.COACategoryId,
                        DocumentNo = m_COACategory.COACategoryCode,
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

        public async Task<SqlResponse> DeleteCOACategory3Async(short CompanyId, short UserId, short coaCategoryId)
        {
            string coaCategoryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    coaCategoryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT COACategoryCode FROM dbo.M_COACategory3 WHERE COACategoryId={coaCategoryId}");

                    if (coaCategoryId > 0)
                    {
                        var accountGroupToRemove = _context.M_COACategory3
                            .Where(x => x.COACategoryId == coaCategoryId)
                            .ExecuteDelete();


                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.COACategory3,
                                DocumentId = coaCategoryId,
                                DocumentNo = coaCategoryNo,
                                TblName = "M_COACategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "COACategory Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "COACategoryId Should be zero" };
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
                    TransactionId = (short)E_Master.COACategory3,
                    DocumentId = coaCategoryId,
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
                    TransactionId = (short)E_Master.COACategory3,
                    DocumentId = coaCategoryId,
                    DocumentNo = "",
                    TblName = "M_COACategory",
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