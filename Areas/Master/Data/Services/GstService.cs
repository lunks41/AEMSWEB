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
    public sealed class GstService : IGstService
    {
        private readonly IRepository<M_Gst> _repository;
        private ApplicationDbContext _context;

        public GstService(IRepository<M_Gst> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        #region GST_HD

        public async Task<GstViewModelCount> GetGstListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            GstViewModelCount countViewModel = new GstViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Gst M_Gt INNER JOIN dbo.M_GstCategory M_gtc ON M_gtc.GstCategoryId = M_Gt.GstCategoryId WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%' OR M_Gt.Remarks LIKE '%{searchString}%' OR M_gtc.GstCategoryName LIKE '%{searchString}%' OR M_gtc.GstCategoryCode LIKE '%{searchString}%') AND M_Gt.GstId<>0 AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory}))");

                var result = await _repository.GetQueryAsync<GstViewModel>($"SELECT M_Gt.GstId,M_Gt.GstCode,M_Gt.GstName,M_Gt.CompanyId,M_Gt.Remarks,M_Gt.IsActive,M_Gt.GstCategoryId,M_gtc.GstCategoryCode,M_gtc.GstCategoryName,M_Gt.CreateById,M_Gt.CreateDate,M_Gt.EditById,M_Gt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Gst M_Gt INNER JOIN dbo.M_GstCategory M_gtc ON M_gtc.GstCategoryId = M_Gt.GstCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gt.EditById WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%' OR M_Gt.Remarks LIKE '%{searchString}%' OR M_gtc.GstCategoryName LIKE '%{searchString}%' OR M_gtc.GstCategoryCode LIKE '%{searchString}%') AND M_Gt.GstId<>0 AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Gst})) ORDER BY M_Gt.GstName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<GstViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Gst",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<GstViewModel> GetGstByIdAsync(short CompanyId, short UserId, short GstId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<GstViewModel>($"SELECT M_Gt.GstId,M_Gt.GstCode,M_Gt.GstName,M_Gt.CompanyId,M_Gt.Remarks,M_Gt.IsActive,M_Gt.GstCategoryId,M_gtc.GstCategoryCode,M_gtc.GstCategoryName,M_Gt.CreateById,M_Gt.CreateDate,M_Gt.EditById,M_Gt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Gst M_Gt INNER JOIN dbo.M_GstCategory M_gtc ON M_gtc.GstCategoryId = M_Gt.GstCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gt.EditById  WHERE M_Gt.GstId={GstId} AND M_Gt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Gst}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Gst",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveGstAsync(short companyId, short userId, M_Gst gst)
        {
            bool isEdit = gst.GstId != 0;
            try
            {
                using (var tScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Combined existence check for both code and name
                    var existenceCheck = await _repository.GetQuerySingleOrDefaultAsync<ExistenceResult>(
                        @"SELECT
                    CASE WHEN EXISTS (
                        SELECT 1
                        FROM dbo.M_Gst
                        WHERE GstId <> @GstId AND GstCode = @GstCode
                        AND CompanyId IN (SELECT CompanyId FROM dbo.Fn_Adm_GetShareCompany(@CompanyId, @ModuleId, @MasterId))
                    ) THEN 1 ELSE 0 END AS CodeExists,
                    CASE WHEN EXISTS (
                        SELECT 1
                        FROM dbo.M_Gst
                        WHERE GstId <> @GstId AND GstName = @GstName
                        AND CompanyId IN (SELECT CompanyId FROM dbo.Fn_Adm_GetShareCompany(@CompanyId, @ModuleId, @MasterId))
                    ) THEN 1 ELSE 0 END AS NameExists",
                        new
                        {
                            gst.GstId,
                            gst.GstCode,
                            gst.GstName,
                            companyId,
                            ModuleId = (short)E_Modules.Master,
                            MasterId = (short)E_Master.Gst
                        });

                    if (existenceCheck?.CodeExists == 1)
                        return new SqlResponse { Result = -1, Message = "GST Code already exists." };

                    if (existenceCheck?.NameExists == 1)
                        return new SqlResponse { Result = -2, Message = "GST Name already exists." };

                    // Generate GstId for new records
                    if (!isEdit)
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            @"SELECT ISNULL((SELECT TOP 1 (GstId + 1)
                            FROM dbo.M_Gst WITH (UPDLOCK, SERIALIZABLE)
                            WHERE (GstId + 1) NOT IN (SELECT GstId FROM dbo.M_Gst)
                            ORDER BY GstId), 1) AS NextId");

                        if (sqlMissingResponse == null || sqlMissingResponse.NextId <= 0)
                            return new SqlResponse { Result = -1, Message = "Failed to generate GST ID." };

                        if (sqlMissingResponse.NextId > short.MaxValue)
                            return new SqlResponse { Result = -1, Message = "GST ID exceeds maximum allowed value." };

                        gst.GstId = Convert.ToInt16(sqlMissingResponse.NextId);

                        // Ensure Edit fields are null for new records
                        gst.EditById = null;
                        gst.EditDate = null;
                    }
                    else
                    {
                        // Prevent modification of created fields
                        _context.Entry(gst).Property(x => x.CreateById).IsModified = false;
                    }

                    // Save main entity
                    var entity = isEdit ? _context.Update(gst) : _context.Add(gst);

                    var gstSaveResult = await _context.SaveChangesAsync();

                    if (gstSaveResult <= 0)
                        return new SqlResponse { Result = -1, Message = "Save operation failed." };

                    // Audit logging
                    var auditLog = new AdmAuditLog
                    {
                        CompanyId = companyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.Gst,
                        DocumentId = gst.GstId,
                        DocumentNo = gst.GstCode,
                        TblName = "M_Gst",
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
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = gst.GstId,
                    DocumentNo = gst.GstCode,
                    TblName = "M_Gst",
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

        public async Task<SqlResponse> DeleteGstAsync(short CompanyId, short UserId, short gstId)
        {
            string gstNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    gstNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT GstCode FROM dbo.M_Gst WHERE GstId={gstId}");

                    if (gstId > 0)
                    {
                        var accountGroupToRemove = _context.M_Gst
                            .Where(x => x.GstId == gstId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Gst,
                                DocumentId = gstId,
                                DocumentNo = gstNo,
                                TblName = "M_Gst",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Gst Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "GstId Should be zero" };
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
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = gstId,
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
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = gstId,
                    DocumentNo = "",
                    TblName = "M_Gst",
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

        public async Task<GstDtViewModelCount> GetGstDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            GstDtViewModelCount GstDtViewModelCount = new GstDtViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_GstDt M_GsDt INNER JOIN dbo.M_Gst M_Gt ON M_Gt.GstId = M_GsDt.GstId WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%') AND M_GsDt.GstId<>0 AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstDt}))");

                var result = await _repository.GetQueryAsync<GstDtViewModel>($"SELECT M_GsDt.GstId,M_Gt.GstCode,M_Gt.GstName,M_GsDt.GstPercentage,M_GsDt.CompanyId,M_GsDt.ValidFrom,M_GsDt.CreateById,M_GsDt.CreateDate,M_GsDt.EditById,M_GsDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditB FROM dbo.M_GstDt M_GsDt INNER JOIN dbo.M_Gst M_Gt ON M_Gt.GstId = M_GsDt.GstId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_GsDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_GsDt.EditById WHERE (M_Gt.GstName LIKE '%{searchString}%' OR M_Gt.GstCode LIKE '%{searchString}%') AND M_GsDt.GstId<>0 AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstDt})) ORDER BY M_Gt.GstName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                GstDtViewModelCount.responseCode = 200;
                GstDtViewModelCount.responseMessage = "success";
                GstDtViewModelCount.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                GstDtViewModelCount.data = result == null ? null : result.ToList();

                return GstDtViewModelCount;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GstDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<GstDtViewModel> GetGstDtByIdAsync(short CompanyId, short UserId, short GstId, DateTime ValidFrom)
        {
            string validFrom = ValidFrom.ToString("yyyy-MM-dd");
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<GstDtViewModel>($"SELECT M_GsDt.GstId,M_Gt.GstCode,M_Gt.GstName,M_GsDt.GstPercentage,M_GsDt.CompanyId,M_GsDt.ValidFrom,M_GsDt.CreateById,M_GsDt.CreateDate,M_GsDt.EditById,M_GsDt.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditB FROM dbo.M_GstDt M_GsDt INNER JOIN dbo.M_Gst M_Gt ON M_Gt.GstId = M_GsDt.GstId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_GsDt.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_GsDt.EditById WHERE M_GsDt.GstId={GstId} AND M_GsDt.ValidFrom = '{validFrom}' AND M_GsDt.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstDt}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GstDt,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstDt",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveGstDtAsync(short CompanyId, short UserId, M_GstDt m_GstDt)
        {
            string validFrom = m_GstDt.ValidFrom.ToString("yyyy-MM-dd");

            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_GstDt.GstId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_GstDt WHERE CompanyId={CompanyId} AND GstId={m_GstDt.GstId} AND ValidFrom='{validFrom}'");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_GstDt);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            var entityHead = _context.Add(m_GstDt);
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
                            TransactionId = (short)E_Master.GstDt,
                            DocumentId = m_GstDt.GstId,
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
                        TransactionId = (short)E_Master.GstDt,
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

        public async Task<SqlResponse> DeleteGstDtAsync(short CompanyId, short UserId, short gstId, DateTime validFrom)
        {
            string gstNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    gstNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT GstCode FROM dbo.M_Gst WHERE GstId={gstId}");

                    if (gstId > 0)
                    {
                        var accountGroupToRemove = _context.M_GstDt
                            .Where(x => x.GstId == gstId && x.ValidFrom == validFrom)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GstDt,
                                DocumentId = gstId,
                                DocumentNo = gstNo,
                                TblName = "M_GstDt",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Gst Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "GstId Should be zero" };
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
                    TransactionId = (short)E_Master.GstDt,
                    DocumentId = gstId,
                    DocumentNo = "",
                    TblName = "M_GstDt",
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
                    TransactionId = (short)E_Master.GstDt,
                    DocumentId = gstId,
                    DocumentNo = "",
                    TblName = "M_GstDt",
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

        public async Task<GstCategoryViewModelCount> GetGstCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            GstCategoryViewModelCount countViewModel = new GstCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_GstCategory M_Gstc WHERE (M_Gstc.GstCategoryName LIKE '%{searchString}%' OR M_Gstc.GstCategoryCode LIKE '%{searchString}%' OR M_Gstc.Remarks LIKE '%{searchString}%') AND M_Gstc.GstCategoryId<>0 AND M_Gstc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory}))");

                var result = await _repository.GetQueryAsync<GstCategoryViewModel>($"SELECT M_Gstc.GstCategoryId,M_Gstc.GstCategoryCode,M_Gstc.GstCategoryName,M_Gstc.CompanyId,M_Gstc.Remarks,M_Gstc.IsActive,M_Gstc.CreateById,M_Gstc.CreateDate,M_Gstc.EditById,M_Gstc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_GstCategory M_Gstc LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gstc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gstc.EditById WHERE (M_Gstc.GstCategoryName LIKE '%{searchString}%' OR M_Gstc.GstCategoryCode LIKE '%{searchString}%' OR M_Gstc.Remarks LIKE '%{searchString}%') AND M_Gstc.GstCategoryId<>0 AND M_Gstc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory})) ORDER BY M_Gstc.GstCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<GstCategoryViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GstCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<GstCategoryViewModel> GetGstCategoryByIdAsync(short CompanyId, short UserId, int GstCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<GstCategoryViewModel>($"SELECT GstCategoryId,GstCategoryCode,GstCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_GstCategory WHERE GstCategoryId={GstCategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GstCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveGstCategoryAsync(short CompanyId, short UserId, M_GstCategory m_GstCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_GstCategory.GstCategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_GstCategory WHERE GstCategoryId<>@GstCategoryId AND GstCategoryCode=@GstCategoryCode",
                        new { m_GstCategory.GstCategoryId, m_GstCategory.GstCategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "GstCategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_GstCategory WHERE GstCategoryId<>@GstCategoryId AND GstCategoryName=@GstCategoryName",
                        new { m_GstCategory.GstCategoryId, m_GstCategory.GstCategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "GstCategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_GstCategory WHERE GstCategoryId=@GstCategoryId",
                            new { m_GstCategory.GstCategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_GstCategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "GstCategory Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (GstCategoryId + 1) FROM dbo.M_GstCategory WHERE (GstCategoryId + 1) NOT IN (SELECT GstCategoryId FROM dbo.M_GstCategory)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_GstCategory.GstCategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_GstCategory);
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
                            TransactionId = (short)E_Master.GstCategory,
                            DocumentId = m_GstCategory.GstCategoryId,
                            DocumentNo = m_GstCategory.GstCategoryCode,
                            TblName = "M_GstCategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "GstCategory Save Successfully",
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
                        TransactionId = (short)E_Master.GstCategory,
                        DocumentId = m_GstCategory.GstCategoryId,
                        DocumentNo = m_GstCategory.GstCategoryCode,
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

        public async Task<SqlResponse> DeleteGstCategoryAsync(short CompanyId, short UserId, short gstCategoryId)
        {
            string gstCategoryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    gstCategoryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT GstCategoryCode FROM dbo.M_GstCategory WHERE GstCategoryId={gstCategoryId}");

                    if (gstCategoryId > 0)
                    {
                        var accountGroupToRemove = _context.M_GstCategory
                            .Where(x => x.GstCategoryId == gstCategoryId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GstCategory,
                                DocumentId = gstCategoryId,
                                DocumentNo = gstCategoryNo,
                                TblName = "M_GstCategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "GstCategory Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "GstId Should be zero" };
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
                    TransactionId = (short)E_Master.GstCategory,
                    DocumentId = gstCategoryId,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
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
                    TransactionId = (short)E_Master.GstCategory,
                    DocumentId = gstCategoryId,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
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