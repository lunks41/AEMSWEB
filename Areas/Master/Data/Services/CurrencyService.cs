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
    public sealed class CurrencyService : ICurrencyService
    {
        private readonly IRepository<M_Currency> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public CurrencyService(IRepository<M_Currency> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        #region Headers

        public async Task<CurrencyViewModelCount> GetCurrencyListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CurrencyViewModelCount countViewModel = new CurrencyViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Currency M_Cur WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.Remarks LIKE '%{searchString}%') AND M_Cur.CurrencyId<>0 AND M_Cur.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency}))");

                var result = await _repository.GetQueryAsync<CurrencyViewModel>($"SELECT M_Cur.CurrencyId,M_Cur.CompanyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Cur.IsMultiply,M_Cur.Remarks,M_Cur.IsActive,M_Cur.CreateById,M_Cur.CreateDate,M_Cur.EditById,M_Cur.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_Currency M_Cur LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cur.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cur.EditById WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.Remarks LIKE '%{searchString}%') AND M_Cur.CurrencyId<>0 AND M_Cur.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency})) ORDER BY M_Cur.CurrencyName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.Currency,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Currency",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CurrencyViewModel> GetCurrencyByIdAsync(short CompanyId, short UserId, int CurrencyId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CurrencyViewModel>($"SELECT M_Cur.CurrencyId,M_Cur.CompanyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Cur.IsMultiply,M_Cur.Remarks,M_Cur.IsActive,M_Cur.CreateById,M_Cur.CreateDate,M_Cur.EditById,M_Cur.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_Currency M_Cur LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cur.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cur.EditById WHERE CurrencyId={CurrencyId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Currency}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Currency,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Currency",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCurrencyAsync(short CompanyId, short UserId, M_Currency m_Currency)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Currency.CurrencyId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Currency WHERE CurrencyId<>@CurrencyId AND CurrencyCode=@CurrencyCode AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                        new { m_Currency.CurrencyId, m_Currency.CurrencyCode, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.Currency });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Currency Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Currency WHERE CurrencyId<>@CurrencyId AND CurrencyName=@CurrencyName AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany (@CompanyId, @ModuleId, @MasterId))",
                        new { m_Currency.CurrencyId, m_Currency.CurrencyName, CompanyId, ModuleId = (short)E_Modules.Master, MasterId = (short)E_Master.Currency });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -2, Message = "Currency Name already exists." };

                    if (!IsEdit)
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT ISNULL((SELECT TOP 1 (CurrencyId + 1) FROM dbo.M_Currency WHERE (CurrencyId + 1) NOT IN (SELECT CurrencyId FROM dbo.M_Currency)),1) AS NextId");
                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Currency.CurrencyId = Convert.ToInt16(sqlMissingResponse.NextId);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "CurrencyId Should not be zero" };
                        }
                    }

                    #region Saving Currency

                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_Currency);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CompanyId).IsModified = false;
                    }
                    else
                    {
                        var entityHead = _context.Add(m_Currency);
                        entityHead.Property(b => b.EditDate).IsModified = false;
                        entityHead.Property(b => b.EditById).IsModified = false;
                    }

                    var CurrencyToSave = _context.SaveChanges();

                    #endregion Saving Currency

                    #region Save AuditLog

                    if (CurrencyToSave > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Currency,
                            DocumentId = m_Currency.CurrencyId,
                            DocumentNo = m_Currency.CurrencyCode,
                            TblName = "M_Currency",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Currency Save Successfully",
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
                        TransactionId = (short)E_Master.Currency,
                        DocumentId = m_Currency.CurrencyId,
                        DocumentNo = m_Currency.CurrencyCode,
                        TblName = "M_Currency",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException?.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponce> DeleteCurrencyAsync(short CompanyId, short UserId, short CurrencyId)
        {
            string CurrencyCode = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    //Get Invoice Number
                    CurrencyCode = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT CurrencyCode FROM dbo.M_Currency WHERE CurrencyId={CurrencyId}");

                    if (CurrencyId > 0)
                    {
                        var CurrencyToRemove = _context.M_Currency.Where(x => x.CurrencyId == CurrencyId).ExecuteDelete();

                        if (CurrencyToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Currency,
                                DocumentId = CurrencyId,
                                DocumentNo = CurrencyCode,
                                TblName = "M_Currency",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Currency Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should be zero" };
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
                    TransactionId = (short)E_Master.Currency,
                    DocumentId = CurrencyId,
                    DocumentNo = CurrencyCode,
                    TblName = "M_Currency",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Headers

        #region Details

        public async Task<CurrencyDtViewModelCount> GetCurrencyDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CurrencyDtViewModelCount countViewModel = new CurrencyDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_CurrencyDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND M_CurDt.CurrencyId<>0 AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyDt}))");

                var result = await _repository.GetQueryAsync<CurrencyDtViewModel>($"SELECT M_CurDt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_CurDt.CompanyId,M_CurDt.ExhRate,M_CurDt.ValidFrom,M_CurDt.CreateById,M_CurDt.CreateDate,M_CurDt.EditById,M_CurDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CurrencyDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CurDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CurDt.EditById WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND M_CurDt.CurrencyId<>0 AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyDt})) ORDER BY M_Cur.CurrencyName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.CurrencyDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CurrencyDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CurrencyDtViewModel> GetCurrencyDtByIdAsync(short CompanyId, short UserId, int CurrencyId, DateTime ValidFrom)
        {
            string validFrom = ValidFrom.ToString("yyyy-MM-dd");
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CurrencyDtViewModel>($"SELECT M_CurDt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_CurDt.CompanyId,M_CurDt.ExhRate,M_CurDt.ValidFrom,M_CurDt.CreateById,M_CurDt.CreateDate,M_CurDt.EditById,M_CurDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CurrencyDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CurDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CurDt.EditById WHERE M_CurDt.CurrencyId={CurrencyId} AND M_CurDt.ValidFrom = '{validFrom}' AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyDt}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CurrencyDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CurrencyDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCurrencyDtAsync(short CompanyId, short UserId, M_CurrencyDt m_CurrencyDt)
        {
            //string validFrom = m_CurrencyDt.ValidFrom.ToString("yyyy-MM-dd");
            string validFrom = m_CurrencyDt.ValidFrom.ToString("dd/MMM/yyyy");
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_CurrencyDt.CurrencyId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_CurrencyDt WHERE CompanyId={CompanyId} AND CurrencyId={m_CurrencyDt.CurrencyId} AND ValidFrom ='{validFrom}'");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_CurrencyDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_CurrencyDt);
                            entityHead.Property(b => b.EditDate).IsModified = false;
                            entityHead.Property(b => b.EditById).IsModified = false;
                        }
                    }

                    var CurrencyDtToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CurrencyDtToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.CurrencyDt,
                            DocumentId = m_CurrencyDt.CurrencyId,
                            DocumentNo = "",
                            TblName = "M_CurrencyDt",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CurrencyDt Save Successfully",
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
                        TransactionId = (short)E_Master.CurrencyDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyDt",
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

        public async Task<SqlResponce> DeleteCurrencyDtAsync(short CompanyId, short UserId, CurrencyDtViewModel currencyDtViewModel)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (currencyDtViewModel.CurrencyId > 0)
                    {
                        var CurrencyDtToRemove = await _context.M_CurrencyDt.Where(x => x.CurrencyId == currencyDtViewModel.CurrencyId && x.ValidFrom == currencyDtViewModel.ValidFrom && x.CompanyId == CompanyId).ExecuteDeleteAsync();

                        if (CurrencyDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CurrencyDt,
                                DocumentId = currencyDtViewModel.CurrencyId,
                                DocumentNo = "",
                                TblName = "M_CurrencyDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CurrencyDt Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should be zero" };
                    }
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
                        TransactionId = (short)E_Master.CurrencyDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyDt",
                        ModeId = (short)E_Mode.Delete,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId,
                    };

                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        #endregion Details

        #region Local Details

        public async Task<CurrencyLocalDtViewModelCount> GetCurrencyLocalDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CurrencyLocalDtViewModelCount countViewModel = new CurrencyLocalDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_CurrencyLocalDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND M_CurDt.CurrencyId<>0 AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyLocalDt}))");

                var result = await _repository.GetQueryAsync<CurrencyLocalDtViewModel>($"SELECT M_CurDt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_CurDt.CompanyId,M_CurDt.ExhRate,M_CurDt.ValidFrom,M_CurDt.CreateById,M_CurDt.CreateDate,M_CurDt.EditById,M_CurDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CurrencyLocalDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CurDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CurDt.EditById WHERE (M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%') AND M_CurDt.CurrencyId<>0 AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyLocalDt})) ORDER BY M_Cur.CurrencyName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.CurrencyLocalDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CurrencyLocalDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CurrencyLocalDtViewModel> GetCurrencyLocalDtByIdAsync(short CompanyId, short UserId, int CurrencyId, DateTime ValidFrom)
        {
            string validFrom = ValidFrom.ToString("yyyy-MM-dd");
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CurrencyLocalDtViewModel>($"SELECT M_CurDt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_CurDt.CompanyId,M_CurDt.ExhRate,M_CurDt.ValidFrom,M_CurDt.CreateById,M_CurDt.CreateDate,M_CurDt.EditById,M_CurDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CurrencyLocalDt M_CurDt INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_CurDt.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CurDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CurDt.EditById WHERE M_CurDt.CurrencyId={CurrencyId} AND M_CurDt.ValidFrom = '{validFrom}' AND M_CurDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CurrencyLocalDt}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CurrencyLocalDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CurrencyLocalDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCurrencyLocalDtAsync(short CompanyId, short UserId, M_CurrencyLocalDt m_CurrencyLocalDt)
        {
            string validFrom = m_CurrencyLocalDt.ValidFrom.ToString("yyyy-MM-dd");
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_CurrencyLocalDt.CurrencyId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_CurrencyLocalDt WHERE CompanyId={CompanyId} AND CurrencyId={m_CurrencyLocalDt.CurrencyId} AND ValidFrom ='{validFrom}'");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_CurrencyLocalDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_CurrencyLocalDt);
                            entityHead.Property(b => b.EditDate).IsModified = false;
                            entityHead.Property(b => b.EditById).IsModified = false;
                        }
                    }

                    var CurrencyLocalDtToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CurrencyLocalDtToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.CurrencyLocalDt,
                            DocumentId = m_CurrencyLocalDt.CurrencyId,
                            DocumentNo = "",
                            TblName = "M_CurrencyLocalDt",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CurrencyLocalDt Save Successfully",
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
                        TransactionId = (short)E_Master.CurrencyLocalDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyLocalDt",
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

        public async Task<SqlResponce> DeleteCurrencyLocalDtAsync(short CompanyId, short UserId, CurrencyLocalDtViewModel currencyLocalDtViewModel)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (currencyLocalDtViewModel.CurrencyId > 0)
                    {
                        var CurrencyLocalDtToRemove = _context.M_CurrencyLocalDt.Where(x => x.CurrencyId == currencyLocalDtViewModel.CurrencyId).ExecuteDelete();

                        if (CurrencyLocalDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CurrencyLocalDt,
                                DocumentId = currencyLocalDtViewModel.CurrencyId,
                                DocumentNo = "",
                                TblName = "M_CurrencyLocalDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CurrencyLocalDt Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CurrencyId Should be zero" };
                    }
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
                        TransactionId = (short)E_Master.CurrencyLocalDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CurrencyLocalDt",
                        ModeId = (short)E_Mode.Delete,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId,
                    };

                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        #endregion Local Details
    }
}