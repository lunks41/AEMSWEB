﻿using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Helpers;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class TaxService : ITaxService
    {
        private readonly IRepository<M_Tax> _repository;
        private ApplicationDbContext _context;

        public TaxService(IRepository<M_Tax> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        #region HEaders

        public async Task<TaxViewModelCount> GetTaxListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            TaxViewModelCount countViewModel = new TaxViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Tax M_Tx INNER JOIN dbo.M_TaxCategory M_Txc ON M_Txc.TaxCategoryId = M_Tx.TaxCategoryId  WHERE (M_Tx.TaxName LIKE '%{searchString}%' OR M_Tx.TaxName LIKE '%{searchString}%' OR M_Tx.Remarks LIKE '%{searchString}%' OR M_Txc.TaxCategoryCode LIKE '%{searchString}%' OR M_Txc.TaxCategoryName LIKE '%{searchString}%' ) AND M_Tx.TaxId<>0 AND M_Tx.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Tax}))");

                var result = await _repository.GetQueryAsync<TaxViewModel>($"SELECT M_Tx.TaxId,M_Tx.TaxName,M_Tx.TaxCode,M_Tx.CompanyId,M_Tx.Remarks,M_Tx.IsActive,M_Tx.TaxCategoryId,M_Txc.TaxCategoryCode,M_Txc.TaxCategoryName,M_Tx.CreateById,M_Tx.CreateDate,M_Tx.EditById,M_Tx.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Tax M_Tx INNER JOIN dbo.M_TaxCategory M_Txc ON M_Txc.TaxCategoryId = M_Tx.TaxCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Tx.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Tx.EditById WHERE (M_Tx.TaxName LIKE '%{searchString}%' OR M_Tx.TaxName LIKE '%{searchString}%' OR M_Tx.Remarks LIKE '%{searchString}%' OR M_Txc.TaxCategoryCode LIKE '%{searchString}%' OR M_Txc.TaxCategoryName LIKE '%{searchString}%' ) AND M_Tx.TaxId<>0 AND M_Tx.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Tax})) ORDER BY M_Tx.TaxName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TaxViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Tax,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Tax",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<TaxViewModel> GetTaxByIdAsync(short CompanyId, short UserId, short TaxId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<TaxViewModel>($"SELECT TaxId,TaxCode,TaxName,TaxCategoryId,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Tax WHERE TaxId={TaxId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Tax,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Tax",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveTaxAsync(short CompanyId, short UserId, M_Tax m_Tax)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Tax.TaxId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Tax WHERE TaxId<>@TaxId AND TaxCode=@TaxCode",
                        new { m_Tax.TaxId, m_Tax.TaxCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Tax Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Tax WHERE TaxId<>@TaxId AND TaxName=@TaxName",
                        new { m_Tax.TaxId, m_Tax.TaxName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Tax Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Tax WHERE TaxId=@TaxId",
                            new { m_Tax.TaxId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Tax);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Tax Not Found" };
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (TaxId + 1) FROM dbo.M_Tax WHERE (TaxId + 1) NOT IN (SELECT TaxId FROM dbo.M_Tax)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Tax.TaxId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Tax);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Tax,
                            DocumentId = m_Tax.TaxId,
                            DocumentNo = m_Tax.TaxCode,
                            TblName = "M_Tax",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Tax Save Successfully",
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
                        TransactionId = (short)E_Master.Tax,
                        DocumentId = m_Tax.TaxId,
                        DocumentNo = m_Tax.TaxCode,
                        TblName = "M_Taz",
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

        public async Task<SqlResponse> DeleteTaxAsync(short CompanyId, short UserId, short taxId)
        {
            string taxNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    taxNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT TaxCode FROM dbo.M_Tax WHERE TaxId={taxId}");

                    if (taxId > 0)
                    {
                        var accountGroupToRemove = _context.M_Tax
                            .Where(x => x.TaxId == taxId)
                            .ExecuteDelete();


                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Tax,
                                DocumentId = taxId,
                                DocumentNo = taxNo,
                                TblName = "M_Tax",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Tax Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "TaxId Should be zero" };
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
                    TransactionId = (short)E_Master.Tax,
                    DocumentId = taxId,
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
                    TransactionId = (short)E_Master.Tax,
                    DocumentId = taxId,
                    DocumentNo = "",
                    TblName = "M_Tax",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion HEaders

        #region Details

        public async Task<TaxDtViewModelCount> GetTaxDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            TaxDtViewModelCount countViewModel = new TaxDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_TaxDt M_TxDt INNER JOIN dbo.M_Tax M_Tx ON M_Tx.TaxId = M_TxDt.TaxId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_TxDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_TxDt.EditById WHERE (M_Tx.TaxName LIKE '%{searchString}%' OR M_Tx.TaxCode LIKE '%{searchString}%') AND M_TxDt.TaxId<>0 AND M_TxDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxDt}))");

                var result = await _repository.GetQueryAsync<TaxDtViewModel>($"SELECT M_TxDt.TaxId,M_Tx.TaxCode,M_Tx.TaxName,M_TxDt.CompanyId,M_TxDt.TaxPercentage,M_TxDt.ValidFrom,M_TxDt.CreateById,M_TxDt.CreateDate,M_TxDt.EditById,M_TxDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_TaxDt M_TxDt INNER JOIN dbo.M_Tax M_Tx ON M_Tx.TaxId = M_TxDt.TaxId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_TxDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_TxDt.EditById WHERE (M_Tx.TaxName LIKE '%{searchString}%' OR M_Tx.TaxCode LIKE '%{searchString}%') AND M_TxDt.TaxId<>0 AND M_TxDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxDt})) ORDER BY M_Tx.TaxName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TaxDtViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.TaxDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_TaxDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<TaxDtViewModel> GetTaxDtByIdAsync(short CompanyId, short UserId, short TaxId, DateTime ValidFrom)
        {
            string validFrom = ValidFrom.ToString("yyyy-MM-dd");
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<TaxDtViewModel>($"SELECT M_TxDt.TaxId,M_Tx.TaxCode,M_Tx.TaxName,M_TxDt.CompanyId,M_TxDt.TaxPercentage,M_TxDt.ValidFrom,M_TxDt.CreateById,M_TxDt.CreateDate,M_TxDt.EditById,M_TxDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_TaxDt M_TxDt INNER JOIN dbo.M_Tax M_Tx ON M_Tx.TaxId = M_TxDt.TaxId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_TxDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_TxDt.EditById WHERE M_TxDt.TaxId={TaxId} AND M_TxDt.ValidFrom='{validFrom}'");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.TaxDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_TaxDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveTaxDtAsync(short CompanyId, short UserId, M_TaxDt m_TaxDt)
        {
            string validFrom = m_TaxDt.ValidFrom.ToString("yyyy-MM-dd");
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_TaxDt.TaxId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_TaxDt WHERE CompanyId={CompanyId} AND TaxId={m_TaxDt.TaxId} AND ValidFrom='{validFrom}'");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_TaxDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_TaxDt);
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
                            TransactionId = (short)E_Master.TaxDt,
                            DocumentId = m_TaxDt.TaxId,
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
                        TransactionId = (short)E_Master.TaxDt,
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

        public async Task<SqlResponse> DeleteTaxDtAsync(short CompanyId, short UserId, short taxId, DateTime Validfrom)
        {
            string taxNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    taxNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT TaxCode FROM dbo.M_Tax WHERE TaxId={taxId}");

                    if (taxId > 0)
                    {
                        var accountGroupToRemove = _context.M_TaxDt
                            .Where(x => x.TaxId == taxId && x.ValidFrom == Validfrom)
                            .ExecuteDelete();


                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.TaxDt,
                                DocumentId = taxId,
                                DocumentNo = taxNo,
                                TblName = "M_TaxDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Tax Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "TaxId Should be zero" };
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
                    TransactionId = (short)E_Master.TaxDt,
                    DocumentId = taxId,
                    DocumentNo = "",
                    TblName = "TaxDt",
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
                    TransactionId = (short)E_Master.TaxDt,
                    DocumentId = taxId,
                    DocumentNo = "",
                    TblName = "TaxDt",
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

        public async Task<TaxCategoryViewModelCount> GetTaxCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            TaxCategoryViewModelCount countViewModel = new TaxCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_TaxCategory M_Txc WHERE (M_Txc.TaxCategoryName LIKE '%{searchString}%' OR M_Txc.TaxCategoryCode LIKE '%{searchString}%' OR M_Txc.Remarks LIKE '%{searchString}%' ) AND M_Txc.TaxCategoryId<>0 AND M_Txc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory}))");

                var result = await _repository.GetQueryAsync<TaxCategoryViewModel>($"SELECT M_Txc.TaxCategoryId,M_Txc.TaxCategoryCode,M_Txc.TaxCategoryName,M_Txc.CompanyId,M_Txc.Remarks,M_Txc.IsActive,M_Txc.CreateById,M_Txc.CreateDate,M_Txc.EditById,M_Txc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_TaxCategory M_Txc LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Txc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Txc.EditById WHERE (M_Txc.TaxCategoryName LIKE '%{searchString}%' OR M_Txc.TaxCategoryCode LIKE '%{searchString}%' OR M_Txc.Remarks LIKE '%{searchString}%' ) AND M_Txc.TaxCategoryId<>0 AND M_Txc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory})) ORDER BY M_Txc.TaxCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TaxCategoryViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.TaxCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_TaxCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<TaxCategoryViewModel> GetTaxCategoryByIdAsync(short CompanyId, short UserId, short TaxCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<TaxCategoryViewModel>($"SELECT TaxCategoryId,TaxCategoryCode,TaxCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_TaxCategory WHERE TaxCategoryId={TaxCategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.TaxCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_TaxCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveTaxCategoryAsync(short CompanyId, short UserId, M_TaxCategory m_TaxCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_TaxCategory.TaxCategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_TaxCategory WHERE TaxCategoryId<>@TaxCategoryId AND TaxCategoryCode=@TaxCategoryCode",
                        new { m_TaxCategory.TaxCategoryId, m_TaxCategory.TaxCategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "TaxCategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_TaxCategory WHERE TaxCategoryId<>@TaxCategoryId AND TaxCategoryName=@TaxCategoryName",
                        new { m_TaxCategory.TaxCategoryId, m_TaxCategory.TaxCategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "TaxCategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_TaxCategory WHERE TaxCategoryId=@TaxCategoryId",
                            new { m_TaxCategory.TaxCategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_TaxCategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "TaxCategory Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (TaxCategoryId + 1) FROM dbo.M_TaxCategory WHERE (TaxCategoryId + 1) NOT IN (SELECT TaxCategoryId FROM dbo.M_TaxCategory)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_TaxCategory.TaxCategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_TaxCategory);
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
                            TransactionId = (short)E_Master.TaxCategory,
                            DocumentId = m_TaxCategory.TaxCategoryId,
                            DocumentNo = m_TaxCategory.TaxCategoryCode,
                            TblName = "M_TaxCategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "TaxCategory Save Successfully",
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
                        TransactionId = (short)E_Master.TaxCategory,
                        DocumentId = m_TaxCategory.TaxCategoryId,
                        DocumentNo = m_TaxCategory.TaxCategoryCode,
                        TblName = "M_TaxCategory",
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

        public async Task<SqlResponse> DeleteTaxCategoryAsync(short CompanyId, short UserId, short taxCategoryId)
        {
            string taxCategoryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    taxCategoryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT TaxCategoryCode FROM dbo.M_TaxCategory WHERE TaxCategoryId={taxCategoryId}");

                    if (taxCategoryId > 0)
                    {
                        var accountGroupToRemove = _context.M_TaxCategory
                            .Where(x => x.TaxCategoryId == taxCategoryId)
                            .ExecuteDelete();


                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.TaxCategory,
                                DocumentId = taxCategoryId,
                                DocumentNo = taxCategoryNo,
                                TblName = "M_TaxCategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "TaxCategory Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "TaxCategoryId Should be zero" };
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
                    TransactionId = (short)E_Master.TaxCategory,
                    DocumentId = taxCategoryId,
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
                    TransactionId = (short)E_Master.TaxCategory,
                    DocumentId = taxCategoryId,
                    DocumentNo = "",
                    TblName = "M_TaxCategory",
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