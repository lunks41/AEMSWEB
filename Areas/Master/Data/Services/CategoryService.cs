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
    public sealed class CategoryService : ICategoryService
    {
        private readonly IRepository<M_Category> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public CategoryService(IRepository<M_Category> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<CategoryViewModelCount> GetCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CategoryViewModelCount countViewModel = new CategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Category M_Cat WHERE (M_Cat.CategoryName LIKE '%{searchString}%' OR M_Cat.CategoryCode LIKE '%{searchString}%' OR M_Cat.Remarks LIKE '%{searchString}%') AND M_Cat.CategoryId<>0 AND M_Cat.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Category}))");

                var result = await _repository.GetQueryAsync<CategoryViewModel>($"SELECT M_Cat.CategoryId,M_Cat.CompanyId,M_Cat.CategoryCode,M_Cat.CategoryName,M_Cat.Remarks,M_Cat.IsActive,M_Cat.CreateById,M_Cat.CreateDate,M_Cat.EditById,M_Cat.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Category M_Cat LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cat.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cat.EditById WHERE (M_Cat.CategoryName LIKE '%{searchString}%' OR M_Cat.CategoryCode LIKE '%{searchString}%' OR M_Cat.Remarks LIKE '%{searchString}%') AND M_Cat.CategoryId<>0 AND M_Cat.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Category})) ORDER BY M_Cat.CategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<CategoryViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Category,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Category",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CategoryViewModel> GetCategoryByIdAsync(short CompanyId, short UserId, short CategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CategoryViewModel>($"SELECT CategoryId,CompanyId,CategoryCode,CategoryName,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Category WHERE CategoryId={CategoryId} AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Category}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Category,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Category",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCategoryAsync(short CompanyId, short UserId, M_Category m_Category)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Category.CategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Category WHERE CategoryId<>@CategoryId AND CategoryCode=@CategoryCode",
                        new { m_Category.CategoryId, m_Category.CategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Category Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Category WHERE CategoryId<>@CategoryId AND CategoryName=@CategoryName",
                        new { m_Category.CategoryId, m_Category.CategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Category Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Category WHERE CategoryId=@CategoryId",
                            new { m_Category.CategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Category);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Category Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (CategoryId + 1) FROM dbo.M_Category WHERE (CategoryId + 1) NOT IN (SELECT CategoryId FROM dbo.M_Category)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Category.CategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Category);
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
                            TransactionId = (short)E_Master.Category,
                            DocumentId = m_Category.CategoryId,
                            DocumentNo = m_Category.CategoryCode,
                            TblName = "M_Category",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Category Save Successfully",
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
                        TransactionId = (short)E_Master.Category,
                        DocumentId = m_Category.CategoryId,
                        DocumentNo = m_Category.CategoryCode,
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

        public async Task<SqlResponse> DeleteCategoryAsync(short CompanyId, short UserId, short categoryId)
        {
            string categoryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    categoryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT CategoryCode FROM dbo.M_Category WHERE CategoryId={categoryId}");

                    if (categoryId > 0)
                    {
                        var accountGroupToRemove = _context.M_Category
                            .Where(x => x.CategoryId == categoryId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Category,
                                DocumentId = categoryId,
                                DocumentNo = categoryNo,
                                TblName = "M_Category",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Category Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "CategoryId Should be zero" };
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
                    TransactionId = (short)E_Master.Category,
                    DocumentId = categoryId,
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
                    TransactionId = (short)E_Master.Category,
                    DocumentId = categoryId,
                    DocumentNo = "",
                    TblName = "M_Category",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SubCategoryViewModelCount> GetSubCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            SubCategoryViewModelCount countViewModel = new SubCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_SubCategory M_Sub WHERE (M_Sub.SubCategoryName LIKE '%{searchString}%' OR M_Sub.SubCategoryCode LIKE '%{searchString}%' OR M_Sub.Remarks LIKE '%{searchString}%') AND M_Sub.SubCategoryId<>0 AND M_Sub.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory}))");

                var result = await _repository.GetQueryAsync<SubCategoryViewModel>($"SELECT M_Sub.SubCategoryId,M_Sub.SubCategoryCode,M_Sub.SubCategoryName,M_Sub.CompanyId,M_Sub.Remarks,M_Sub.IsActive,M_Sub.CreateById,M_Sub.CreateDate,M_Sub.EditById,M_Sub.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_SubCategory M_Sub LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sub.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sub.EditById WHERE (M_Sub.SubCategoryName LIKE '%{searchString}%' OR M_Sub.SubCategoryCode LIKE '%{searchString}%' OR M_Sub.Remarks LIKE '%{searchString}%') AND M_Sub.SubCategoryId<>0 AND M_Sub.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory})) ORDER BY M_Sub.SubCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<SubCategoryViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.SubCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SubCategoryViewModel> GetSubCategoryByIdAsync(short CompanyId, short UserId, short SubCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<SubCategoryViewModel>($"SELECT SubCategoryId,SubCategoryCode,SubCategoryName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_SubCategory WHERE SubCategoryId={SubCategoryId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.SubCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveSubCategoryAsync(short CompanyId, short UserId, M_SubCategory m_SubCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_SubCategory.SubCategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_SubCategory WHERE SubCategoryId<>@SubCategoryId AND SubCategoryCode=@SubCategoryCode",
                        new { m_SubCategory.SubCategoryId, m_SubCategory.SubCategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "SubCategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_SubCategory WHERE SubCategoryId<>@SubCategoryId AND SubCategoryName=@SubCategoryName",
                        new { m_SubCategory.SubCategoryId, m_SubCategory.SubCategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "SubCategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_SubCategory WHERE SubCategoryId=@SubCategoryId",
                            new { m_SubCategory.SubCategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_SubCategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "SubCategory Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (SubCategoryId + 1) FROM dbo.M_SubCategory WHERE (SubCategoryId + 1) NOT IN (SELECT SubCategoryId FROM dbo.M_SubCategory)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_SubCategory.SubCategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_SubCategory);
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
                            TransactionId = (short)E_Master.SubCategory,
                            DocumentId = m_SubCategory.SubCategoryId,
                            DocumentNo = m_SubCategory.SubCategoryCode,
                            TblName = "M_SubCategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "SubCategory Save Successfully",
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
                        TransactionId = (short)E_Master.SubCategory,
                        DocumentId = m_SubCategory.SubCategoryId,
                        DocumentNo = m_SubCategory.SubCategoryCode,
                        TblName = "M_SubCategory",
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

        public async Task<SqlResponse> DeleteSubCategoryAsync(short CompanyId, short UserId, short subCategoryId)
        {
            string subCategoryNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    subCategoryNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT SubCategoryCode FROM dbo.M_SubCategory WHERE SubCategoryId={subCategoryId}");

                    if (subCategoryId > 0)
                    {
                        var accountGroupToRemove = _context.M_SubCategory
                            .Where(x => x.SubCategoryId == subCategoryId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.SubCategory,
                                DocumentId = subCategoryId,
                                DocumentNo = subCategoryNo,
                                TblName = "M_SubCategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "SubCategory Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "SubCategoryId Should be zero" };
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
                    TransactionId = (short)E_Master.SubCategory,
                    DocumentId = subCategoryId,
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
                    TransactionId = (short)E_Master.SubCategory,
                    DocumentId = subCategoryId,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
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