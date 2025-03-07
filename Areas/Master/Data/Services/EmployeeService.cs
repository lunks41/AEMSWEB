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
    public sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepository<M_Employee> _repository;
        private ApplicationDbContext _context;

        public EmployeeService(IRepository<M_Employee> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<EmployeeViewModelCount> GetEmployeeListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            EmployeeViewModelCount countViewModel = new EmployeeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Employee M_Emp INNER JOIN M_Department M_Dep ON M_Dep.DepartmentId = M_Emp.DepartmentId WHERE (M_Emp.EmployeeName LIKE '%{searchString}%' OR M_Emp.EmployeeCode LIKE '%{searchString}%' OR M_Emp.Remarks LIKE '%{searchString}%' OR M_Dep.DepartmentName LIKE '%{searchString}%' OR M_Dep.DepartmentCode LIKE '%{searchString}%') AND M_Emp.EmployeeId<>0 AND M_Emp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Employee}))");

                var result = await _repository.GetQueryAsync<EmployeeViewModel>($"SELECT M_Emp.EmployeeId,M_Emp.EmployeeCode,M_Emp.CompanyId,M_Emp.EmployeeName,M_Emp.EmployeeOtherName,M_Emp.EmployeePhoto,M_Emp.EmployeeSignature,M_Emp.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,M_Emp.EmployeeSex,M_Emp.MartialStatus,M_Emp.EmployeeDOB,M_Emp.EmployeeJoinDate,M_Emp.EmployeeOffEmailAdd,M_Emp.EmployeeOtherEmailAdd,M_Emp.Remarks,M_Emp.IsActive,M_Emp.CreateById,M_Emp.CreateDate,M_Emp.EditById,M_Emp.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Employee M_Emp INNER JOIN M_Department M_Dep ON M_Dep.DepartmentId = M_Emp.DepartmentId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Emp.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Emp.EditById WHERE (M_Emp.EmployeeName LIKE '%{searchString}%' OR M_Emp.EmployeeCode LIKE '%{searchString}%' OR M_Emp.Remarks LIKE '%{searchString}%' OR M_Dep.DepartmentName LIKE '%{searchString}%' OR M_Dep.DepartmentCode LIKE '%{searchString}%') AND M_Emp.EmployeeId<>0 AND M_Emp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Employee})) ORDER BY M_Emp.EmployeeName");

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
                    TransactionId = (short)E_Master.Employee,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Employee",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_Employee> GetEmployeeByIdAsync(short CompanyId, short UserId, short EmployeeId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_Employee>($"SELECT M_Emp.EmployeeId,M_Emp.EmployeeCode,M_Emp.CompanyId,M_Emp.EmployeeName,M_Emp.EmployeeOtherName,M_Emp.EmployeePhoto,M_Emp.EmployeeSignature,M_Emp.DepartmentId,M_Dep.DepartmentCode,M_Dep.DepartmentName,M_Emp.EmployeeSex,M_Emp.MartialStatus,M_Emp.EmployeeDOB,M_Emp.EmployeeJoinDate,M_Emp.EmployeeOffEmailAdd,M_Emp.EmployeeOtherEmailAdd,M_Emp.Remarks,M_Emp.IsActive,M_Emp.CreateById,M_Emp.CreateDate,M_Emp.EditById,M_Emp.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Employee M_Emp INNER JOIN M_Department M_Dep ON M_Dep.DepartmentId = M_Emp.DepartmentId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Emp.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Emp.EditById M_Emp.EmployeeId={EmployeeId} AND M_Emp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Employee}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Employee,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Employee",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveEmployeeAsync(short CompanyId, short UserId, M_Employee m_Employee)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Employee.EmployeeId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Employee WHERE EmployeeId<>@EmployeeId AND EmployeeCode=@EmployeeCode",
                        new { m_Employee.EmployeeId, m_Employee.EmployeeCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Employee Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Employee WHERE EmployeeId<>@EmployeeId AND EmployeeName=@EmployeeName",
                        new { m_Employee.EmployeeId, m_Employee.EmployeeName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Employee Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Employee WHERE EmployeeId=@EmployeeId",
                            new { m_Employee.EmployeeId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Employee);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Employee Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (EmployeeId + 1) FROM dbo.M_Employee WHERE (EmployeeId + 1) NOT IN (SELECT EmployeeId FROM dbo.M_Employee)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Employee.EmployeeId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Employee);
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
                            TransactionId = (short)E_Master.Employee,
                            DocumentId = m_Employee.EmployeeId,
                            DocumentNo = m_Employee.EmployeeCode,
                            TblName = "M_Employee",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Employee Save Successfully",
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
                        TransactionId = (short)E_Master.Employee,
                        DocumentId = m_Employee.EmployeeId,
                        DocumentNo = m_Employee.EmployeeCode,
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

        public async Task<SqlResponse> DeleteEmployeeAsync(short CompanyId, short UserId, M_Employee Employee)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (Employee.EmployeeId > 0)
                    {
                        var EmployeeToRemove = _context.M_Employee.Where(x => x.EmployeeId == Employee.EmployeeId).ExecuteDelete();

                        if (EmployeeToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Employee,
                                DocumentId = Employee.EmployeeId,
                                DocumentNo = Employee.EmployeeCode,
                                TblName = "M_Employee",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Employee Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "EmployeeId Should be zero" };
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
                        TransactionId = (short)E_Master.Employee,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Employee",
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