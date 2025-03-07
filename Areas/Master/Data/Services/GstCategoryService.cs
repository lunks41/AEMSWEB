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
    public sealed class GstCategoryService : IGstCategoryService
    {
        private readonly IRepository<M_GstCategory> _repository;
        private ApplicationDbContext _context;

        public GstCategoryService(IRepository<M_GstCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<GstCategoryViewModelCount> GetGstCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            GstCategoryViewModelCount countViewModel = new GstCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_GstCategory M_Gstc WHERE (M_Gstc.GstCategoryName LIKE '%{searchString}%' OR M_Gstc.GstCategoryCode LIKE '%{searchString}%' OR M_Gstc.Remarks LIKE '%{searchString}%') AND M_Gstc.GstCategoryId<>0 AND M_Gstc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory}))");

                var result = await _repository.GetQueryAsync<GstCategoryViewModel>($"SELECT M_Gstc.GstCategoryId,M_Gstc.GstCategoryCode,M_Gstc.GstCategoryName,M_Gstc.CompanyId,M_Gstc.Remarks,M_Gstc.IsActive,M_Gstc.CreateById,M_Gstc.CreateDate,M_Gstc.EditById,M_Gstc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_GstCategory M_Gstc LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Gstc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Gstc.EditById WHERE (M_Gstc.GstCategoryName LIKE '%{searchString}%' OR M_Gstc.GstCategoryCode LIKE '%{searchString}%' OR M_Gstc.Remarks LIKE '%{searchString}%') AND M_Gstc.GstCategoryId<>0 AND M_Gstc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory})) ORDER BY M_Gstc.GstCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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

        public async Task<M_GstCategory> GetGstCategoryByIdAsync(short CompanyId, short UserId, int GstCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_GstCategory>($"SELECT GstCategoryId,GstCategoryCode,GstCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_GstCategory WHERE GstCategoryId={GstCategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory}))");

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

        public async Task<SqlResponse> DeleteGstCategoryAsync(short CompanyId, short UserId, M_GstCategory GstCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (GstCategory.GstCategoryId > 0)
                    {
                        var GstCategoryToRemove = _context.M_GstCategory.Where(x => x.GstCategoryId == GstCategory.GstCategoryId).ExecuteDelete();

                        if (GstCategoryToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GstCategory,
                                DocumentId = GstCategory.GstCategoryId,
                                DocumentNo = GstCategory.GstCategoryCode,
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
                        return new SqlResponse { Result = -1, Message = "GstCategoryId Should be zero" };
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
                        TransactionId = (short)E_Master.GstCategory,
                        DocumentId = 0,
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
}