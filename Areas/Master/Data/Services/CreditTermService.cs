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
    public sealed class CreditTermService : ICreditTermService
    {
        private readonly IRepository<M_CreditTerm> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public CreditTermService(IRepository<M_CreditTerm> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        #region Header

        public async Task<CreditTermViewModelCount> GetCreditTermListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CreditTermViewModelCount countViewModel = new CreditTermViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_CreditTerm M_Crd WHERE (M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.Remarks LIKE '%{searchString}%') AND M_Crd.CreditTermId<>0 AND M_Crd.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTerm}))");

                var result = await _repository.GetQueryAsync<CreditTermViewModel>($"SELECT M_Crd.CreditTermId,M_Crd.CreditTermCode,M_Crd.CompanyId,M_Crd.CreditTermName,M_Crd.NoDays,M_Crd.Remarks,M_Crd.IsActive,M_Crd.CreateById,M_Crd.CreateDate,M_Crd.EditById,M_Crd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CreditTerm M_Crd LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Crd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Crd.EditById WHERE (M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.Remarks LIKE '%{searchString}%') AND M_Crd.CreditTermId<>0 AND M_Crd.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTerm})) ORDER BY M_Crd.CreditTermName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CreditTerm,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CreditTermViewModel> GetCreditTermByIdAsync(short CompanyId, short UserId, short CreditTermId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CreditTermViewModel>($"SELECT CreditTermId,CreditTermCode,CreditTermName,CompanyId,NoDays,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_CreditTerm WHERE CreditTermId={CreditTermId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTerm}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CreditTerm,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCreditTermAsync(short CompanyId, short UserId, M_CreditTerm m_CreditTerm)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_CreditTerm.CreditTermId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_CreditTerm WHERE CreditTermId<>@CreditTermId AND CreditTermCode=@CreditTermCode AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                        new { m_CreditTerm.CreditTermId, m_CreditTerm.CreditTermCode, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.CreditTerm });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "CreditTerm Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_CreditTerm WHERE CreditTermId<>@CreditTermId AND CreditTermName=@CreditTermName AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                        new { m_CreditTerm.CreditTermId, m_CreditTerm.CreditTermName, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.CreditTerm });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -2, Message = "CreditTerm Name already exists." };

                    if (!IsEdit)
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (CreditTermId + 1) FROM dbo.M_CreditTerm WHERE (CreditTermId + 1) NOT IN (SELECT CreditTermId FROM dbo.M_CreditTerm)),1) AS NextId");
                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_CreditTerm.CreditTermId = Convert.ToInt16(sqlMissingResponse.NextId);
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "CreditTermId Should not be zero" };
                        }
                    }

                    var entity = IsEdit ? _context.Update(m_CreditTerm) : _context.Add(m_CreditTerm);
                    entity.Property(b => b.EditDate).IsModified = false;
                    entity.Property(b => b.EditById).IsModified = false;

                    var CreditTermToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CreditTermToSave > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.CreditTerm,
                            DocumentId = m_CreditTerm.CreditTermId,
                            DocumentNo = m_CreditTerm.CreditTermCode,
                            TblName = "M_CreditTerm",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CreditTerm Save Successfully",
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
                        TransactionId = (short)E_Master.CreditTerm,
                        DocumentId = m_CreditTerm.CreditTermId,
                        DocumentNo = m_CreditTerm.CreditTermCode,
                        TblName = "M_CreditTerm",
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

        public async Task<SqlResponse> DeleteCreditTermAsync(short CompanyId, short UserId, short creditTermId)
        {
            string creditTermNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    creditTermNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT CreditTermCode FROM dbo.M_CreditTerm WHERE CreditTermId={creditTermId}");

                    if (creditTermId > 0)
                    {
                        var accountGroupToRemove = _context.M_CreditTerm
                            .Where(x => x.CreditTermId == creditTermId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CreditTerm,
                                DocumentId = creditTermId,
                                DocumentNo = creditTermNo,
                                TblName = "M_CreditTerm",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CreditTerm Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "CreditTermId Should be zero" };
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
                    TransactionId = (short)E_Master.CreditTerm,
                    DocumentId = creditTermId,
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
                    TransactionId = (short)E_Master.CreditTerm,
                    DocumentId = creditTermId,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Header

        #region Details

        public async Task<CreditTermDtViewModelCount> GetCreditTermDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CreditTermDtViewModelCount countViewModel = new CreditTermDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_CreditTermDt M_CrdDt INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_CrdDt.CreditTermId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Crd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Crd.EditById WHERE (M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%') AND M_CrdDt.CreditTermId<>0 AND M_CrdDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTermDt}))");

                var result = await _repository.GetQueryAsync<CreditTermDtViewModel>($"SELECT M_CrdDt.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_CrdDt.CompanyId,M_CrdDt.FromDay,M_CrdDt.ToDay,M_CrdDt.IsEndOfMonth,M_CrdDt.DueDay,M_CrdDt.NoMonth,M_CrdDt.CreateById,M_CrdDt.CreateDate,M_CrdDt.EditById,M_CrdDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CreditTermDt M_CrdDt INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_CrdDt.CreditTermId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Crd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Crd.EditById WHERE (M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Crd.CreditTermName LIKE '%{searchString}%') AND M_CrdDt.CreditTermId<>0 AND M_CrdDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTermDt})) ORDER BY M_Crd.CreditTermName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CreditTermDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTermDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CreditTermDtViewModel> GetCreditTermDtByIdAsync(short CompanyId, short UserId, short CreditTermId, short FromDay)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CreditTermDtViewModel>($"SELECT M_CrdDt.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_CrdDt.CompanyId,M_CrdDt.FromDay,M_CrdDt.ToDay,M_CrdDt.IsEndOfMonth,M_CrdDt.DueDay,M_CrdDt.NoMonth,M_CrdDt.CreateById,M_CrdDt.CreateDate,M_CrdDt.EditById,M_CrdDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CreditTermDt M_CrdDt INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_CrdDt.CreditTermId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Crd.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Crd.EditById WHERE M_CrdDt.CreditTermId={CreditTermId} AND M_CrdDt.CompanyId={CompanyId} AND FromDay={FromDay}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CreditTermDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTermDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCreditTermDtAsync(short CompanyId, short UserId, M_CreditTermDt m_CreditTermDt)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_CreditTermDt.CreditTermId != 0)
                    {
                        IsEdit = true;
                    }

                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_CreditTermDt WHERE CreditTermId={m_CreditTermDt.CreditTermId} AND CompanyId={CompanyId} AND FromDay={m_CreditTermDt.FromDay}");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_CreditTermDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_CreditTermDt);
                            entityHead.Property(b => b.EditDate).IsModified = false;
                            entityHead.Property(b => b.EditById).IsModified = false;
                        }
                    }

                    var CreditTermDtToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CreditTermDtToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.CreditTermDt,
                            DocumentId = m_CreditTermDt.CreditTermId,
                            DocumentNo = "",
                            TblName = "M_CreditTermDt",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CreditTermDt Save Successfully",
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
                        TransactionId = (short)E_Master.CreditTermDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CreditTermDt",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponse> DeleteCreditTermDtAsync(short CompanyId, short UserId, short CreditTermId, short FromDay)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (CreditTermId > 0)
                    {
                        var CreditTermDtToRemove = await _context.M_CreditTermDt.Where(x => x.CreditTermId == CreditTermId && x.FromDay == FromDay && x.CompanyId == CompanyId).ExecuteDeleteAsync();

                        if (CreditTermDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CreditTermDt,
                                DocumentId = CreditTermId,
                                DocumentNo = FromDay.ToString(),
                                TblName = "M_CreditTermDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CreditTermDt Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();

                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Cancel Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Cancel Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "CreditTermDt Not exists" };
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
                    ModuleId = (short)E_Modules.AR,
                    TransactionId = (short)E_AR.Invoice,
                    DocumentId = CreditTermId,
                    DocumentNo = FromDay.ToString(),
                    TblName = "M_CreditTermDt",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Details
    }
}