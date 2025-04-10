﻿using AMESWEB.Areas.Master.Data.IServices;
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
    public sealed class AccountSetupService : IAccountSetupService
    {
        private readonly IRepository<M_AccountSetup> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public AccountSetupService(IRepository<M_AccountSetup> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        #region Header

        public async Task<AccountSetupViewModelCount> GetAccountSetupListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            AccountSetupViewModelCount countViewModel = new AccountSetupViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_AccountSetup M_ACC INNER JOIN dbo.M_AccountSetupCategory M_Accsc ON M_Accsc.AccSetupCategoryId = M_ACC.AccSetupCategoryId WHERE (M_ACC.AccSetupName LIKE '%{searchString}%' OR M_ACC.AccSetupCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%' OR M_Accsc.AccSetupCategoryName LIKE '%{searchString}%' OR M_Accsc.AccSetupCategoryCode LIKE '%{searchString}%') AND M_ACC.AccSetupId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetup}))");

                var result = await _repository.GetQueryAsync<AccountSetupViewModel>($"SELECT M_ACC.AccSetupId,M_ACC.AccSetupCode,M_ACC.AccSetupName,M_ACC.CompanyId,M_ACC.AccSetupCategoryId,M_Accsc.AccSetupCategoryCode,M_Accsc.AccSetupCategoryName,M_ACC.Remarks,M_ACC.IsActive,M_ACC.CreateById,M_ACC.CreateDate,M_ACC.EditById,M_ACC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_AccountSetup M_ACC  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById INNER JOIN dbo.M_AccountSetupCategory M_Accsc ON M_Accsc.AccSetupCategoryId = M_ACC.AccSetupCategoryId  WHERE (M_ACC.AccSetupName LIKE '%{searchString}%' OR M_ACC.AccSetupCode LIKE '%{searchString}%' OR M_ACC.Remarks LIKE '%{searchString}%' OR M_Accsc.AccSetupCategoryName LIKE '%{searchString}%' OR M_Accsc.AccSetupCategoryCode LIKE '%{searchString}%') AND M_ACC.AccSetupId<>0 AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetup})) ORDER BY M_ACC.AccSetupName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<AccountSetupViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetup",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<AccountSetupViewModel> GetAccountSetupByIdAsync(short CompanyId, short UserId, short AccSetupId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AccountSetupViewModel>($"SELECT M_ACC.AccSetupId,M_ACC.AccSetupCode,M_ACC.AccSetupName,M_ACC.CompanyId,M_ACC.AccSetupCategoryId,M_Accsc.AccSetupCategoryCode,M_Accsc.AccSetupCategoryName,M_ACC.Remarks,M_ACC.IsActive,M_ACC.CreateById,M_ACC.CreateDate,M_ACC.EditById,M_ACC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_AccountSetup M_ACC  LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACC.EditById INNER JOIN dbo.M_AccountSetupCategory M_Accsc ON M_Accsc.AccSetupCategoryId = M_ACC.AccSetupCategoryId WHERE M_ACC.AccSetupId={AccSetupId} AND M_ACC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetup}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetup",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveAccountSetupAsync(short CompanyId, short UserId, M_AccountSetup m_AccountSetup)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_AccountSetup.AccSetupId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_AccountSetup WHERE AccSetupId<>@AccSetupId AND AccSetupCode=@AccSetupCode",
                        new { m_AccountSetup.AccSetupId, m_AccountSetup.AccSetupCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "AccSetup Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_AccountSetup WHERE AccSetupId<>@AccSetupId AND AccSetupName=@AccSetupName",
                        new { m_AccountSetup.AccSetupId, m_AccountSetup.AccSetupName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "AccSetup Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_AccountSetup WHERE AccSetupId=@AccSetupId",
                            new { m_AccountSetup.AccSetupId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_AccountSetup);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "AccountSetup Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT ISNULL((SELECT TOP 1 (AccSetupId + 1) FROM dbo.M_AccountSetup WHERE (AccSetupId + 1) NOT IN (SELECT AccSetupId FROM dbo.M_AccountSetup)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_AccountSetup.AccSetupId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_AccountSetup);
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
                            TransactionId = (short)E_Master.AccountSetup,
                            DocumentId = m_AccountSetup.AccSetupId,
                            DocumentNo = m_AccountSetup.AccSetupCode,
                            TblName = "M_AccountSetup",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "AccountSetup Save Successfully",
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
                        TransactionId = (short)E_Master.AccountSetup,
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
                        TransactionId = (short)E_Master.AccountSetup,
                        DocumentId = m_AccountSetup.AccSetupId,
                        DocumentNo = m_AccountSetup.AccSetupCode,
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

        public async Task<SqlResponce> DeleteAccountSetupAsync(short CompanyId, short UserId, short accSetupId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (accSetupId > 0)
                    {
                        var AccountSetupToRemove = _context.M_AccountSetup.Where(x => x.AccSetupId == accSetupId).ExecuteDelete();

                        if (AccountSetupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.AccountSetup,
                                DocumentId = accSetupId,
                                DocumentNo = "",
                                TblName = "M_AccountSetup",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "AccountSetup Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccSetupId Should be zero" };
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
                        TransactionId = (short)E_Master.AccountSetup,
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
                        TransactionId = (short)E_Master.AccountSetup,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_AccountSetup",
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

        #endregion Header

        #region Details

        public async Task<AccountSetupDtViewModelCount> GetAccountSetupDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            AccountSetupDtViewModelCount countViewModel = new AccountSetupDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_AccountSetupDt M_ACCdt INNER JOIN dbo.M_AccountSetup M_Acc ON M_Acc.AccSetupId = M_ACCdt.AccSetupId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_ACCdt.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chacc ON M_Chacc.GLId = M_ACCdt.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACCdt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACCdt.EditById WHERE (M_Acc.AccSetupCode LIKE '%{searchString}%' OR M_ACC.AccSetupName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Chacc.GLCode LIKE '%{searchString}%'  OR M_Chacc.GLName LIKE '%{searchString}%') AND M_ACCdt.AccSetupId<>0 AND M_ACCdt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupDt}))");

                var result = await _repository.GetQueryAsync<AccountSetupDtViewModel>($"SELECT M_ACCdt.CompanyId,M_ACCdt.AccSetupId,M_ACC.AccSetupCode,M_ACC.AccSetupName,M_ACCdt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_ACCdt.GLId,M_Chacc.GLCode,M_Chacc.GLName,M_ACCdt.CreateById,M_ACCdt.CreateDate,M_ACCdt.EditById,M_ACCdt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_AccountSetupDt M_ACCdt INNER JOIN dbo.M_AccountSetup M_Acc ON M_Acc.AccSetupId = M_ACCdt.AccSetupId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_ACCdt.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chacc ON M_Chacc.GLId = M_ACCdt.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACCdt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACCdt.EditById WHERE (M_Acc.AccSetupCode LIKE '%{searchString}%' OR M_ACC.AccSetupName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Chacc.GLCode LIKE '%{searchString}%'  OR M_Chacc.GLName LIKE '%{searchString}%') AND M_ACCdt.AccSetupId<>0 AND M_ACCdt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupDt})) ORDER BY M_ACC.AccSetupName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<AccountSetupDtViewModel>(0);

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetupDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<AccountSetupDtViewModel> GetAccountSetupDtByIdAsync(short CompanyId, short UserId, short accSetupId, short currencyId, short glId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AccountSetupDtViewModel>($"SELECT M_ACCdt.CompanyId,M_ACCdt.AccSetupId,M_ACC.AccSetupCode,M_ACC.AccSetupName,M_ACCdt.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_ACCdt.GLId,M_Chacc.GLCode,M_Chacc.GLName,M_ACCdt.CreateById,M_ACCdt.CreateDate,M_ACCdt.EditById,M_ACCdt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_AccountSetupDt M_ACCdt INNER JOIN dbo.M_AccountSetup M_Acc ON M_Acc.AccSetupId = M_ACCdt.AccSetupId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_ACCdt.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chacc ON M_Chacc.GLId = M_ACCdt.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_ACCdt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_ACCdt.EditById WHERE M_ACCdt.AccSetupId={accSetupId} AND M_ACCdt.CurrencyId={currencyId} AND M_ACCdt.GLId={glId} AND M_ACCdt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupDt}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetupDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveAccountSetupDtAsync(short CompanyId, short UserId, M_AccountSetupDt m_AccountSetupDt)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_AccountSetupDt.AccSetupId > 0 && m_AccountSetupDt.CurrencyId > 0 && m_AccountSetupDt.GLId > 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_AccountSetupDt WHERE CompanyId={CompanyId} AND AccSetupId={m_AccountSetupDt.AccSetupId} AND CurrencyId={m_AccountSetupDt.CurrencyId} AND GLId={m_AccountSetupDt.GLId}");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_AccountSetupDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_AccountSetupDt);
                            entityHead.Property(b => b.EditDate).IsModified = false;
                            entityHead.Property(b => b.EditById).IsModified = false;
                        }
                    }

                    var AccountDtDtToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (AccountDtDtToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.AccountSetupDt,
                            DocumentId = m_AccountSetupDt.AccSetupId,
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
                        TransactionId = (short)E_Master.AccountSetupDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_AccountSetupDt",
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

        public async Task<SqlResponce> DeleteAccountSetupDtAsync(short CompanyId, short UserId, short accSetupId, short currencyId, short glId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (accSetupId > 0)
                    {
                        var AccountSetupDtToRemove = _context.M_AccountSetupDt.Where(x => x.AccSetupId == accSetupId && x.CurrencyId == currencyId && x.GLId == glId).ExecuteDelete();

                        if (AccountSetupDtToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.AccountSetupDt,
                                DocumentId = accSetupId,
                                DocumentNo = "",
                                TblName = "M_AccountSetupDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "AccountSetupDt Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "AccountSetupDtId Should be zero" };
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
                        TransactionId = (short)E_Master.AccountSetupDt,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_AccountSetupDt",
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

        #region

        public async Task<AccountSetupCategoryViewModelCount> GetAccountSetupCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            AccountSetupCategoryViewModelCount countViewModel = new AccountSetupCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_AccountSetupCategory M_AccSetCa  WHERE M_AccSetCa.AccSetupCategoryId<>0 AND  ( M_AccSetCa.AccSetupCategoryName LIKE '%{searchString}%' OR M_AccSetCa.AccSetupCategoryCode LIKE '%{searchString}%' OR M_AccSetCa.Remarks LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<AccountSetupCategoryViewModel>($"SELECT M_AccSetCa.AccSetupCategoryId,M_AccSetCa.AccSetupCategoryCode,M_AccSetCa.AccSetupCategoryName,M_AccSetCa.Remarks,M_AccSetCa.IsActive,M_AccSetCa.CreateById,M_AccSetCa.CreateDate,M_AccSetCa.EditById,M_AccSetCa.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_AccountSetupCategory M_AccSetCa LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_AccSetCa.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_AccSetCa.EditById WHERE M_AccSetCa.AccSetupCategoryId<>0 AND  ( M_AccSetCa.AccSetupCategoryName LIKE '%{searchString}%' OR M_AccSetCa.AccSetupCategoryCode LIKE '%{searchString}%' OR M_AccSetCa.Remarks LIKE '%{searchString}%') AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory})) ORDER BY M_AccSetCa.AccSetupCategoryName");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<AccountSetupCategoryViewModel>();

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

        public async Task<AccountSetupCategoryViewModel> GetAccountSetupCategoryByIdAsync(short CompanyId, short UserId, short AccSetupCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AccountSetupCategoryViewModel>($"SELECT AccSetupCategoryId,AccSetupCategoryCode,AccSetupCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId={AccSetupCategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory}))");

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

        public async Task<SqlResponce> SaveAccountSetupCategoryAsync(short CompanyId, short UserId, M_AccountSetupCategory m_AccountSetupCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_AccountSetupCategory.AccSetupCategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId<>@AccSetupCategoryId AND AccSetupCategoryCode=@AccSetupCategoryCode", new { m_AccountSetupCategory.AccSetupCategoryId, m_AccountSetupCategory.AccSetupCategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "AccSetupCategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId<>@AccSetupCategoryId AND AccSetupCategoryName=@AccSetupCategoryName", new { m_AccountSetupCategory.AccSetupCategoryId, m_AccountSetupCategory.AccSetupCategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "AccSetupCategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId=@AccSetupCategoryId", new { m_AccountSetupCategory.AccSetupCategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_AccountSetupCategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "AccountSetupCategory Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT ISNULL((SELECT TOP 1 (AccSetupCategoryId + 1) FROM dbo.M_AccountSetupCategory WHERE (AccSetupCategoryId + 1) NOT IN (SELECT AccSetupCategoryId FROM dbo.M_AccountSetupCategory)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_AccountSetupCategory.AccSetupCategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_AccountSetupCategory);
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

        public async Task<SqlResponce> DeleteAccountSetupCategoryAsync(short CompanyId, short UserId, short accSetupCategoryId)
        {
            string accSetupCategoryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    accSetupCategoryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT AccSetupCategoryCode FROM dbo.M_AccountSetupCategory WHERE AccSetupCategoryId={accSetupCategoryId}");

                    if (accSetupCategoryId > 0)
                    {
                        var accountGroupToRemove = _context.M_AccountSetupCategory
                            .Where(x => x.AccSetupCategoryId == accSetupCategoryId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.AccountSetupCategory,
                                DocumentId = accSetupCategoryId,
                                DocumentNo = accSetupCategoryNo,
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
                        return new SqlResponce { Result = -1, Message = "AccSetupCategoryId Should be zero" };
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
                    TransactionId = (short)E_Master.AccountSetupCategory,
                    DocumentId = accSetupCategoryId,
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
                    TransactionId = (short)E_Master.AccountSetupCategory,
                    DocumentId = accSetupCategoryId,
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

        #endregion
    }
}