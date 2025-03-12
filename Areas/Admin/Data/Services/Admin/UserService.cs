using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using BC = BCrypt.Net.BCrypt;

namespace AEMSWEB.Services.Admin
{
    public sealed class UserService : IUserService
    {
        private readonly IRepository<AdmUser> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public UserService(IRepository<AdmUser> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<UserViewModelCount> GetUserListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            UserViewModelCount countViewModel = new UserViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.AdmUser A_Usr INNER JOIN dbo.AdmUserGroup A_UsrG ON A_UsrG.UserGroupId = A_Usr.UserGroupId WHERE A_Usr.UserId<>0 AND (A_Usr.UserName LIKE '%{searchString}%' OR A_Usr.UserCode LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%' OR A_UsrG.UserGroupName LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<UserViewModel>($"SELECT A_Usr.UserId,A_Usr.UserCode,A_Usr.UserName,A_Usr.UserEmail,A_Usr.Remarks,A_Usr.IsActive,A_Usr.UserGroupId,A_UsrG.UserGroupCode,A_UsrG.UserGroupName,A_Usr.CreateById,A_Usr.CreateDate,A_Usr.EditById,A_Usr.EditDate ,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmUser A_Usr INNER JOIN dbo.AdmUserGroup A_UsrG ON A_UsrG.UserGroupId = A_Usr.UserGroupId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = A_Usr.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = A_Usr.EditById WHERE A_Usr.UserId<>0 AND (A_Usr.UserName LIKE '%{searchString}%' OR A_Usr.UserCode LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%' OR A_UsrG.UserGroupName LIKE '%{searchString}%') ORDER BY A_Usr.UserName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<AdmUser> GetUserByIdAsync(short CompanyId, short UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AdmUser>($"SELECT Id,UserCode,UserName,UserEmail,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.AdmUser WHERE Id={UserId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteUserAsync(short CompanyId, short UserId, AdmUser admUser)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (admUser.Id > 0)
                    {
                        var UserToRemove = _context.AdmUser.Where(x => x.Id == admUser.Id).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsActive, false).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));

                        if (UserToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Admin.User,
                                DocumentId = admUser.Id,
                                DocumentNo = admUser.UserCode,
                                TblName = "AdmUser",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "User Delete Successfully",
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
                        TransactionId = (short)E_Admin.User,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "AdmUser",
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

        public static string GenratePassword(string userCode, string password)
        {
            return BC.HashPassword(userCode.ToLower().Trim() + password.Trim());
        }

        #region

        public async Task<UserGroupViewModelCount> GetUserGroupListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
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

        public async Task<AdmUserGroup> GetUserGroupByIdAsync(short CompanyId, short UserId, Int16 Id)
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

        public async Task<SqlResponse> SaveUserGroupAsync(short CompanyId, short UserId, AdmUserGroup admUserGroup)
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

        public async Task<SqlResponse> DeleteUserGroupAsync(short CompanyId, short UserId, AdmUserGroup UserGroup)
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

        #endregion

        public async Task<IEnumerable<UserRightsViewModel>> GetUserRightsByIdAsync(short CompanyId, short UserId, int SelectedUserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserRightsViewModel>($"exec Adm_GetUserAccessRights {CompanyId},{SelectedUserId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                await _context.SaveChangesAsync();

                throw;
            }
        }

        public async Task<SqlResponse> SaveUserRightsAsync(short CompanyId, short UserId, List<AdmUserRights> admUserRights, Int16 SelectedUserId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Detach tracked entities to avoid conflicts
                    var trackedEntities = _context.ChangeTracker.Entries<AdmUserRights>().ToList();
                    foreach (var entry in trackedEntities)
                    {
                        entry.State = EntityState.Detached;
                    }

                    await _repository.GetQueryAsync<SqlResponseIds>($"DELETE FROM dbo.AdmUserRights WHERE UserId = {SelectedUserId}");

                    // Add filtered entities
                    _context.AdmUserRights.AddRange(admUserRights);
                    var saveResult = await _context.SaveChangesAsync();

                    #region Audit Log

                    if (saveResult > 0 || admUserRights == null)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.UserRights,
                            TblName = "AdmUserRights",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "User Group Rights Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.AdmAuditLog.Add(auditLog);
                        await _context.SaveChangesAsync();

                        TScope.Complete();
                        return new SqlResponse { Result = 1, Message = "Save Successfully" };
                    }

                    #endregion Audit Log

                    return new SqlResponse { Result = -1, Message = "Save Failed" };
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.UserRights,
                        TblName = "AdmUserRights",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty),
                        CreateById = UserId,
                        CreateDate = DateTime.Now
                    };

                    _context.AdmErrorLog.Add(errorLog);
                    await _context.SaveChangesAsync();

                    throw;
                }
            }
        }

        public async Task<IEnumerable<UserGroupRightsViewModel>> GetUserGroupRightsByIdAsync(short CompanyId, short UserId, int SelectedUserId, int SelectedGroupId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserGroupRightsViewModel>($"exec Adm_GetGroupAccessRights {CompanyId},{SelectedGroupId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.UserGroupRights,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUserGroupRights",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveUserGroupRightsAsync(short CompanyId, short UserId, List<AdmUserGroupRights> admUserGroupRights, Int16 UserGroupId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var trackedEntities = _context.ChangeTracker.Entries<AdmUserGroupRights>().ToList();
                    foreach (var entry in trackedEntities)
                    {
                        entry.State = EntityState.Detached;
                    }

                    await _repository.GetQueryAsync<SqlResponseIds>($"DELETE FROM dbo.AdmUserGroupRights WHERE  UserGroupId={UserGroupId}");

                    _context.AdmUserGroupRights.AddRange(admUserGroupRights);
                    var saveResult = await _context.SaveChangesAsync();

                    #region Audit Log

                    if (saveResult > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.UserGroupRights,
                            TblName = "AdmUserGroupRights",
                            ModeId = (short)E_Mode.Create,
                            Remarks = "User Group Rights Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.AdmAuditLog.Add(auditLog);
                        await _context.SaveChangesAsync();

                        TScope.Complete();
                        return new SqlResponse { Result = 1, Message = "Save Successfully" };
                    }

                    #endregion Audit Log

                    return new SqlResponse { Result = 0, Message = "Save Failed" };
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.UserGroupRights,
                        TblName = "AdmUserGroupRights",
                        ModeId = (short)E_Mode.Create,
                        Remarks = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : string.Empty),
                        CreateById = UserId,
                        CreateDate = DateTime.Now
                    };

                    _context.AdmErrorLog.Add(errorLog);
                    await _context.SaveChangesAsync();

                    throw;
                }
            }
        }
    }
}