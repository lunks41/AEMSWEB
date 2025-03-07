using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Services.Admin
{
    public sealed class UserGroupService : IUserGroupService
    {
        private readonly IRepository<AdmUserGroup> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public UserGroupService(IRepository<AdmUserGroup> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<UserGroupViewModelCount> GetUserGroupListAsync(Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 UserId)
        {
            UserGroupViewModelCount countViewModel = new UserGroupViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.AdmUserGroup A_UsrG WHERE (A_UsrG.UserGroupName LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%') AND A_UsrG.Id<>0");

                var result = await _repository.GetQueryAsync<UserGroupViewModel>($"SELECT A_UsrG.Id,A_UsrG.UserGroupCode,A_UsrG.UserGroupName,A_UsrG.Remarks,A_UsrG.IsActive,A_UsrG.CreateById,A_UsrG.CreateDate,A_UsrG.EditById,A_UsrG.EditDate ,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmUserGroup A_UsrG LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = A_UsrG.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = A_UsrG.EditById WHERE (A_UsrG.UserGroupName LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%') AND A_UsrG.Id<>0 ORDER BY A_UsrG.UserGroupName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
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
                    TransactionId = (short)E_Admin.UserGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUserGroup",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<AdmUserGroup> GetUserGroupByIdAsync(Int16 CompanyId, Int16 Id, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AdmUserGroup>($"SELECT Id,UserGroupCode,UserGroupName,IsActive,Remarks,CreateById,CreateDate,EditById,EditDate FROM dbo.AdmUserGroup WHERE Id={Id}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Admin.UserGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUserGroup",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveUserGroupAsync(Int16 CompanyId, AdmUserGroup admUserGroup, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (admUserGroup.Id != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.AdmUserGroup WHERE Id<>0 AND Id={admUserGroup.Id} ");

                        if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(admUserGroup);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "UserGroup Not Found" };
                    }
                    else
                    {
                        //Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>("SELECT ISNULL((SELECT TOP 1 (Id + 1) FROM dbo.AdmUserGroup WHERE (Id + 1) NOT IN (SELECT Id FROM dbo.AdmUserGroup)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            admUserGroup.Id = Convert.ToInt16(sqlMissingResponse.NextId);
                            admUserGroup.EditById = null;
                            admUserGroup.EditDate = null;
                            _context.Add(admUserGroup);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.UserGroup,
                            DocumentId = admUserGroup.Id,
                            DocumentNo = admUserGroup.UserGroupCode,
                            TblName = "AdmUserGroup",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "User Group Save Successfully",
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
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.UserGroup,
                        DocumentId = admUserGroup.Id,
                        DocumentNo = admUserGroup.UserGroupCode,
                        TblName = "AdmUserGroup",
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

        public async Task<SqlResponse> DeleteUserGroupAsync(Int16 CompanyId, AdmUserGroup UserGroup, Int16 UserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (UserGroup.Id > 0)
                    {
                        var UserGroupToRemove = _context.AdmUserGroup.Where(x => x.Id == UserGroup.Id).ExecuteDelete();

                        if (UserGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Admin.UserGroup,
                                DocumentId = UserGroup.Id,
                                DocumentNo = UserGroup.UserGroupCode,
                                TblName = "AdmUserGroup",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "UserGroup Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "Id Should be zero" };
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
                        TransactionId = (short)E_Admin.UserGroup,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "AdmUserGroup",
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