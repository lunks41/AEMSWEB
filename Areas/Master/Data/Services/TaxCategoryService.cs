using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class TaxCategoryService : ITaxCategoryService
    {
        private readonly IRepository<M_TaxCategory> _repository;
        private ApplicationDbContext _context;

        public TaxCategoryService(IRepository<M_TaxCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<TaxCategoryViewModelCount> GetTaxCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            TaxCategoryViewModelCount countViewModel = new TaxCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_TaxCategory M_Txc WHERE (M_Txc.TaxCategoryName LIKE '%{searchString}%' OR M_Txc.TaxCategoryCode LIKE '%{searchString}%' OR M_Txc.Remarks LIKE '%{searchString}%' ) AND M_Txc.TaxCategoryId<>0 AND M_Txc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory}))");

                var result = await _repository.GetQueryAsync<TaxCategoryViewModel>($"SELECT M_Txc.TaxCategoryId,M_Txc.TaxCategoryCode,M_Txc.TaxCategoryName,M_Txc.CompanyId,M_Txc.Remarks,M_Txc.IsActive,M_Txc.CreateById,M_Txc.CreateDate,M_Txc.EditById,M_Txc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_TaxCategory M_Txc LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Txc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Txc.EditById WHERE (M_Txc.TaxCategoryName LIKE '%{searchString}%' OR M_Txc.TaxCategoryCode LIKE '%{searchString}%' OR M_Txc.Remarks LIKE '%{searchString}%' ) AND M_Txc.TaxCategoryId<>0 AND M_Txc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory})) ORDER BY M_Txc.TaxCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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

        public async Task<M_TaxCategory> GetTaxCategoryByIdAsync(short CompanyId, short UserId, short TaxCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_TaxCategory>($"SELECT TaxCategoryId,TaxCategoryCode,TaxCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_TaxCategory WHERE TaxCategoryId={TaxCategoryId}");

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

        public async Task<SqlResponse> DeleteTaxCategoryAsync(short CompanyId, short UserId, M_TaxCategory TaxCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (TaxCategory.TaxCategoryId > 0)
                    {
                        var TaxCategoryToRemove = _context.M_TaxCategory.Where(x => x.TaxCategoryId == TaxCategory.TaxCategoryId).ExecuteDelete();

                        if (TaxCategoryToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.TaxCategory,
                                DocumentId = TaxCategory.TaxCategoryId,
                                DocumentNo = TaxCategory.TaxCategoryCode,
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.TaxCategory,
                        DocumentId = 0,
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
}