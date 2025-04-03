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
    public sealed class DepartmentService : IDepartmentService
    {
        private readonly IRepository<M_Department> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public DepartmentService(IRepository<M_Department> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<DepartmentViewModelCount> GetDepartmentListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            DepartmentViewModelCount countViewModel = new DepartmentViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Department M_Dep WHERE (M_Dep.DepartmentName LIKE '%{searchString}%' OR M_Dep.DepartmentCode LIKE '%{searchString}%' OR M_Dep.Remarks LIKE '%{searchString}%') AND M_Dep.DepartmentId<>0 AND M_Dep.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Department}))");

                var result = await _repository.GetQueryAsync<DepartmentViewModel>($"SELECT M_Dep.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,M_Dep.CompanyId,M_Dep.Remarks,M_Dep.IsActive,M_Dep.CreateById,M_Dep.CreateDate,M_Dep.EditById,M_Dep.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Department M_Dep LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Dep.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Dep.EditById WHERE (M_Dep.DepartmentName LIKE '%{searchString}%' OR M_Dep.DepartmentCode LIKE '%{searchString}%' OR M_Dep.Remarks LIKE '%{searchString}%') AND M_Dep.DepartmentId<>0 AND M_Dep.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Department})) ORDER BY M_Dep.DepartmentName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<DepartmentViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Department,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Department",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<DepartmentViewModel> GetDepartmentByIdAsync(short CompanyId, short UserId, short DepartmentId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<DepartmentViewModel>($"SELECT DepartmentId,DepartmentCode,DepartmentName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Department WHERE DepartmentId={DepartmentId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Department}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Department,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Department",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveDepartmentAsync(short CompanyId, short UserId, M_Department m_Department)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Department.DepartmentId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Department WHERE DepartmentId<>@DepartmentId AND DepartmentCode=@DepartmentCode",
                        new { m_Department.DepartmentId, m_Department.DepartmentCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Department Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Department WHERE DepartmentId<>@DepartmentId AND DepartmentName=@DepartmentName",
                        new { m_Department.DepartmentId, m_Department.DepartmentName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Department Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Department WHERE DepartmentId=@DepartmentId",
                            new { m_Department.DepartmentId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Department);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Department Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (DepartmentId + 1) FROM dbo.M_Department WHERE (DepartmentId + 1) NOT IN (SELECT DepartmentId FROM dbo.M_Department)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Department.DepartmentId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Department);
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
                            TransactionId = (short)E_Master.Department,
                            DocumentId = m_Department.DepartmentId,
                            DocumentNo = m_Department.DepartmentCode,
                            TblName = "M_Department",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Department Save Successfully",
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
                        TransactionId = (short)E_Master.Department,
                        DocumentId = m_Department.DepartmentId,
                        DocumentNo = m_Department.DepartmentCode,
                        TblName = "M_Department",
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

        public async Task<SqlResponse> DeleteDepartmentAsync(short CompanyId, short UserId, short departmentId)
        {
            string departmentNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    departmentNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT DepartmentCode FROM dbo.M_Department WHERE DepartmentId={departmentId}");

                    if (departmentId > 0)
                    {
                        var accountGroupToRemove = _context.M_Department
                            .Where(x => x.DepartmentId == departmentId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Department,
                                DocumentId = departmentId,
                                DocumentNo = departmentNo,
                                TblName = "M_Department",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Department Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "DepartmentId Should be zero" };
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
                    TransactionId = (short)E_Master.Department,
                    DocumentId = departmentId,
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
                    TransactionId = (short)E_Master.Department,
                    DocumentId = departmentId,
                    DocumentNo = "",
                    TblName = "M_Department",
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