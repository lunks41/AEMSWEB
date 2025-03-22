using AEMSWEB.Areas.Master.Data.IServices;
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

        #region GST_HD

        public async Task<TaxViewModelCount> GetTaxListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            TaxViewModelCount countViewModel = new TaxViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Tax M_Gt INNER JOIN dbo.M_TaxCategory M_gtc ON M_gtc.TaxCategoryId = M_Gt.TaxCategoryId WHERE (M_Gt.TaxName LIKE '%{searchString}%' OR M_Gt.TaxCode LIKE '%{searchString}%' OR M_Gt.Remarks LIKE '%{searchString}%' OR M_gtc.TaxCategoryName LIKE '%{searchString}%' OR M_gtc.TaxCategoryCode LIKE '%{searchString}%') AND M_Gt.TaxId<>0 AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory}))");

                var result = await _repository.GetQueryAsync<TaxViewModel>($"SELECT M_Gt.TaxId,M_Gt.TaxCode,M_Gt.TaxName,M_Gt.CompanyId,M_Gt.Remarks,M_Gt.IsActive,M_Gt.TaxCategoryId,M_gtc.TaxCategoryCode,M_gtc.TaxCategoryName,M_Gt.CreateById,M_Gt.CreateDate,M_Gt.EditById,M_Gt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Tax M_Gt INNER JOIN dbo.M_TaxCategory M_gtc ON M_gtc.TaxCategoryId = M_Gt.TaxCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gt.EditById WHERE (M_Gt.TaxName LIKE '%{searchString}%' OR M_Gt.TaxCode LIKE '%{searchString}%' OR M_Gt.Remarks LIKE '%{searchString}%' OR M_gtc.TaxCategoryName LIKE '%{searchString}%' OR M_gtc.TaxCategoryCode LIKE '%{searchString}%') AND M_Gt.TaxId<>0 AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Tax})) ORDER BY M_Gt.TaxName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<TaxViewModel>($"SELECT M_Gt.TaxId,M_Gt.TaxCode,M_Gt.TaxName,M_Gt.CompanyId,M_Gt.Remarks,M_Gt.IsActive,M_Gt.TaxCategoryId,M_gtc.TaxCategoryCode,M_gtc.TaxCategoryName,M_Gt.CreateById,M_Gt.CreateDate,M_Gt.EditById,M_Gt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Tax M_Gt INNER JOIN dbo.M_TaxCategory M_gtc ON M_gtc.TaxCategoryId = M_Gt.TaxCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gt.EditById  WHERE M_Gt.TaxId={TaxId} AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Tax}))");

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

        public async Task<SqlResponse> SaveTaxAsync(short companyId, short userId, M_Tax tax)
        {
            bool isEdit = tax.TaxId != 0;
            try
            {
                using (var tScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Combined existence check for both code and name
                    var existenceCheck = await _repository.GetQuerySingleOrDefaultAsync<ExistenceResult>(
                        @"SELECT
                    CASE WHEN EXISTS (
                        SELECT 1
                        FROM dbo.M_Tax
                        WHERE TaxId <> @TaxId AND TaxCode = @TaxCode
                        AND CompanyId IN (SELECT CompanyId FROM dbo.Fn_Adm_GetShareCompany(@CompanyId, @ModuleId, @MasterId))
                    ) THEN 1 ELSE 0 END AS CodeExists,
                    CASE WHEN EXISTS (
                        SELECT 1
                        FROM dbo.M_Tax
                        WHERE TaxId <> @TaxId AND TaxName = @TaxName
                        AND CompanyId IN (SELECT CompanyId FROM dbo.Fn_Adm_GetShareCompany(@CompanyId, @ModuleId, @MasterId))
                    ) THEN 1 ELSE 0 END AS NameExists",
                        new
                        {
                            tax.TaxId,
                            tax.TaxCode,
                            tax.TaxName,
                            companyId,
                            ModuleId = (short)E_Modules.Master,
                            MasterId = (short)E_Master.Tax
                        });

                    if (existenceCheck?.CodeExists == 1)
                        return new SqlResponse { Result = -1, Message = "GST Code already exists." };

                    if (existenceCheck?.NameExists == 1)
                        return new SqlResponse { Result = -2, Message = "GST Name already exists." };

                    // Generate TaxId for new records
                    if (!isEdit)
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            @"SELECT ISNULL((SELECT TOP 1 (TaxId + 1)
                            FROM dbo.M_Tax WITH (UPDLOCK, SERIALIZABLE)
                            WHERE (TaxId + 1) NOT IN (SELECT TaxId FROM dbo.M_Tax)
                            ORDER BY TaxId), 1) AS NextId");

                        if (sqlMissingResponse == null || sqlMissingResponse.NextId <= 0)
                            return new SqlResponse { Result = -1, Message = "Failed to generate GST ID." };

                        if (sqlMissingResponse.NextId > short.MaxValue)
                            return new SqlResponse { Result = -1, Message = "GST ID exceeds maximum allowed value." };

                        tax.TaxId = Convert.ToInt16(sqlMissingResponse.NextId);

                        // Ensure Edit fields are null for new records
                        tax.EditById = null;
                        tax.EditDate = null;
                    }
                    else
                    {
                        // Prevent modification of created fields
                        _context.Entry(tax).Property(x => x.CreateById).IsModified = false;
                    }

                    // Save main entity
                    var entity = isEdit ? _context.Update(tax) : _context.Add(tax);

                    var taxSaveResult = await _context.SaveChangesAsync();

                    if (taxSaveResult <= 0)
                        return new SqlResponse { Result = -1, Message = "Save operation failed." };

                    // Audit logging
                    var auditLog = new AdmAuditLog
                    {
                        CompanyId = companyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Tax,
                        DocumentId = tax.TaxId,
                        DocumentNo = tax.TaxCode,
                        TblName = "M_Tax",
                        ModeId = isEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = "GST saved successfully",
                        CreateById = userId,
                        CreateDate = DateTime.Now
                    };

                    _context.Add(auditLog);
                    var auditSaveResult = await _context.SaveChangesAsync();

                    if (auditSaveResult <= 0)
                        return new SqlResponse { Result = -1, Message = "Audit log save failed." };

                    tScope.Complete();
                    return new SqlResponse { Result = 1, Message = "Saved successfully" };
                }
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = companyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Tax,
                    DocumentId = tax.TaxId,
                    DocumentNo = tax.TaxCode,
                    TblName = "M_Tax",
                    ModeId = isEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = $"{ex.Message} {ex.InnerException?.Message}",
                    CreateById = userId,
                    CreateDate = DateTime.Now
                };

                _context.Add(errorLog);
                await _context.SaveChangesAsync();

                return new SqlResponse
                {
                    Result = -99,
                    Message = "System error occurred. Check error logs.",
                    ErrorDetails = ex.Message
                };
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

        #endregion GST_HD

        #region GST_DT

        public async Task<TaxDtViewModelCount> GetTaxDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            TaxDtViewModelCount TaxDtViewModelCount = new TaxDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_TaxDt M_GsDt INNER JOIN dbo.M_Tax M_Gt ON M_Gt.TaxId = M_GsDt.TaxId WHERE (M_Gt.TaxName LIKE '%{searchString}%' OR M_Gt.TaxCode LIKE '%{searchString}%') AND M_GsDt.TaxId<>0 AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxDt}))");

                var result = await _repository.GetQueryAsync<TaxDtViewModel>($"SELECT M_GsDt.TaxId,M_Gt.TaxCode,M_Gt.TaxName,M_GsDt.TaxPercentage,M_GsDt.CompanyId,M_GsDt.ValidFrom,M_GsDt.CreateById,M_GsDt.CreateDate,M_GsDt.EditById,M_GsDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditB FROM dbo.M_TaxDt M_GsDt INNER JOIN dbo.M_Tax M_Gt ON M_Gt.TaxId = M_GsDt.TaxId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_GsDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_GsDt.EditById WHERE (M_Gt.TaxName LIKE '%{searchString}%' OR M_Gt.TaxCode LIKE '%{searchString}%') AND M_GsDt.TaxId<>0 AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxDt})) ORDER BY M_Gt.TaxName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                TaxDtViewModelCount.responseCode = 200;
                TaxDtViewModelCount.responseMessage = "success";
                TaxDtViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                TaxDtViewModelCount.data = result == null ? null : result.ToList();

                return TaxDtViewModelCount;
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
                var result = await _repository.GetQuerySingleOrDefaultAsync<TaxDtViewModel>($"SELECT M_GsDt.TaxId,M_Gt.TaxCode,M_Gt.TaxName,M_GsDt.TaxPercentage,M_GsDt.CompanyId,M_GsDt.ValidFrom,M_GsDt.CreateById,M_GsDt.CreateDate,M_GsDt.EditById,M_GsDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditB FROM dbo.M_TaxDt M_GsDt INNER JOIN dbo.M_Tax M_Gt ON M_Gt.TaxId = M_GsDt.TaxId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_GsDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_GsDt.EditById WHERE M_GsDt.TaxId={TaxId} AND M_GsDt.ValidFrom = '{validFrom}' AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxDt}))");

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

        public async Task<SqlResponse> DeleteTaxDtAsync(short CompanyId, short UserId, short taxId, DateTime validFrom)
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
                            .Where(x => x.TaxId == taxId && x.ValidFrom == validFrom)
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
                    TblName = "M_TaxDt",
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
                    TblName = "M_TaxDt",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion GST_DT

        public async Task<TaxCategoryViewModelCount> GetTaxCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            TaxCategoryViewModelCount countViewModel = new TaxCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_TaxCategory M_Taxc WHERE (M_Taxc.TaxCategoryName LIKE '%{searchString}%' OR M_Taxc.TaxCategoryCode LIKE '%{searchString}%' OR M_Taxc.Remarks LIKE '%{searchString}%') AND M_Taxc.TaxCategoryId<>0 AND M_Taxc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory}))");

                var result = await _repository.GetQueryAsync<TaxCategoryViewModel>($"SELECT M_Taxc.TaxCategoryId,M_Taxc.TaxCategoryCode,M_Taxc.TaxCategoryName,M_Taxc.CompanyId,M_Taxc.Remarks,M_Taxc.IsActive,M_Taxc.CreateById,M_Taxc.CreateDate,M_Taxc.EditById,M_Taxc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_TaxCategory M_Taxc LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Taxc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Taxc.EditById WHERE (M_Taxc.TaxCategoryName LIKE '%{searchString}%' OR M_Taxc.TaxCategoryCode LIKE '%{searchString}%' OR M_Taxc.Remarks LIKE '%{searchString}%') AND M_Taxc.TaxCategoryId<>0 AND M_Taxc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory})) ORDER BY M_Taxc.TaxCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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

        public async Task<TaxCategoryViewModel> GetTaxCategoryByIdAsync(short CompanyId, short UserId, int TaxCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<TaxCategoryViewModel>($"SELECT TaxCategoryId,TaxCategoryCode,TaxCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_TaxCategory WHERE TaxCategoryId={TaxCategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory}))");

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
                    TransactionId = (short)E_Master.TaxCategory,
                    DocumentId = taxCategoryId,
                    DocumentNo = "",
                    TblName = "M_TaxCategory",
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