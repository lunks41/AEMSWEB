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
    public sealed class TaskService : ITaskService
    {
        private readonly IRepository<M_Task> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public TaskService(IRepository<M_Task> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<TaskViewModelCount> GetTaskListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            TaskViewModelCount countViewModel = new TaskViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Task M_Vess WHERE (M_Vess.TaskName LIKE '%{searchString}%' OR M_Vess.TaskCode LIKE '%{searchString}%' OR M_Vess.TaskType LIKE '%{searchString}%') AND M_Vess.TaskId <>0 AND M_Vess.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Task}))");

                var result = await _repository.GetQueryAsync<TaskViewModel>($"SELECT M_Vess.TaskId,M_Vess.CompanyId,M_Vess.TaskCode,M_Vess.TaskName,M_Vess.CallSign,M_Vess.IMOCode,M_Vess.GRT,M_Vess.LicenseNo,M_Vess.TaskType,M_Vess.Flag,M_Vess.Remarks,M_Vess.IsActive,M_Vess.CreateById,M_Vess.CreateDate,M_Vess.EditById,M_Vess.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_Task M_Vess LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Vess.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Vess.EditById WHERE (M_Vess.TaskName LIKE '%{searchString}%' OR M_Vess.TaskCode LIKE '%{searchString}%' OR M_Vess.TaskType LIKE '%{searchString}%') AND M_Vess.TaskId <>0 AND M_Vess.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Task})) ORDER BY M_Vess.TaskName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TaskViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Task,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Task",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<TaskViewModel> GetTaskByIdAsync(short CompanyId, short UserId, short TaskId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<TaskViewModel>($"SELECT TaskId,CompanyId,TaskCode,TaskName,CallSign,IMOCode,GRT,LicenseNo,TaskType,Flag,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Task WHERE TaskId={TaskId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Task}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Task,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Task",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveTaskAsync(short CompanyId, short UserId, M_Task m_Task)
        {
            bool IsEdit = m_Task.TaskId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Task WHERE TaskId<>@TaskId AND TaskCode=@TaskCode",
                        new { m_Task.TaskId, m_Task.TaskCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Task Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Task WHERE TaskId<>@TaskId AND TaskName=@TaskName",
                        new { m_Task.TaskId, m_Task.TaskName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Task Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Task WHERE TaskId=@TaskId",
                            new { m_Task.TaskId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Task);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Task Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT ISNULL((SELECT TOP 1 (TaskId + 1) FROM dbo.M_Task WHERE (TaskId + 1) NOT IN (SELECT TaskId FROM dbo.M_Task)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Task.TaskId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Task);
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
                            TransactionId = (short)E_Master.Task,
                            DocumentId = m_Task.TaskId,
                            DocumentNo = m_Task.TaskCode,
                            TblName = "M_Task",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Task Save Successfully",
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
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Task, m_Task.TaskId, m_Task.TaskCode ?? "", "M_Task", IsEdit ? E_Mode.Update : E_Mode.Create, "SQL", UserId);
                return new SqlResponce { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Task, m_Task.TaskId, m_Task.TaskCode ?? "", "M_Task", IsEdit ? E_Mode.Update : E_Mode.Create, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> DeleteTaskAsync(short CompanyId, short UserId, short taskId)
        {
            string taskNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    taskNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT TaskCode FROM dbo.M_Task WHERE TaskId={taskId}");

                    if (taskId > 0)
                    {
                        var accountGroupToRemove = _context.M_Task
                            .Where(x => x.TaskId == taskId)
                            .ExecuteDelete();

                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Task,
                                DocumentId = taskId,
                                DocumentNo = taskNo,
                                TblName = "M_Task",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Task Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "TaskId Should be zero" };
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
                    TransactionId = (short)E_Master.Task,
                    DocumentId = taskId,
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
                    TransactionId = (short)E_Master.Task,
                    DocumentId = taskId,
                    DocumentNo = "",
                    TblName = "M_Task",
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