﻿using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Models.Admin;
using AMESWEB.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using BC = BCrypt.Net.BCrypt;

namespace AMESWEB.Areas.Admin.Data
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

        #region User

        public async Task<UserViewModelCount> GetUserListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            UserViewModelCount countViewModel = new UserViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.AdmUser A_Usr INNER JOIN dbo.AdmUserGroup A_UsrG ON A_UsrG.UserGroupId = A_Usr.UserGroupId WHERE A_Usr.UserId<>0 AND (A_Usr.UserName LIKE '%{searchString}%' OR A_Usr.UserCode LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%' OR A_UsrG.UserGroupName LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<UserViewModel>($"SELECT A_Usr.UserId,A_Usr.UserCode,A_Usr.FullName,A_Usr.UserName,A_Usr.UserEmail,A_Usr.Remarks,A_Usr.IsActive,A_Usr.UserGroupId,A_UsrG.UserGroupCode,A_UsrG.UserGroupName,A_Usr.CreateById,A_Usr.CreateDate,A_Usr.EditById,A_Usr.EditDate ,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmUser A_Usr INNER JOIN dbo.AdmUserGroup A_UsrG ON A_UsrG.UserGroupId = A_Usr.UserGroupId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = A_Usr.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = A_Usr.EditById WHERE A_Usr.UserId<>0 AND (A_Usr.UserName LIKE '%{searchString}%' OR A_Usr.UserCode LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%' OR A_UsrG.UserGroupName LIKE '%{searchString}%') ORDER BY A_Usr.UserName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<UserViewModel>();

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

        public async Task<UserViewModel> GetUserByIdAsync(short CompanyId, short UserId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<UserViewModel>($"SELECT A_Usr.UserId,A_Usr.UserCode,A_Usr.FullName,A_Usr.UserName,A_Usr.UserEmail,A_Usr.Remarks,A_Usr.IsActive,A_Usr.UserGroupId,A_UsrG.UserGroupCode,A_UsrG.UserGroupName,A_Usr.CreateById,A_Usr.CreateDate,A_Usr.EditById,A_Usr.EditDate ,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmUser A_Usr INNER JOIN dbo.AdmUserGroup A_UsrG ON A_UsrG.UserGroupId = A_Usr.UserGroupId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = A_Usr.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = A_Usr.EditById WHERE A_Usr.UserId={UserId}");

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

        public async Task<SqlResponce> SaveUserAsync(short UserId, AdmUser admUser, string password)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (admUser.Id != 0)
                    {
                        IsEdit = true;
                    }

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.AdmUser WHERE Id<>0 AND UserId={admUser.Id} ");

                        if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(admUser);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "User Not Found" };
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (Id + 1) FROM dbo.AdmUser WHERE (Id + 1) NOT IN (SELECT Id FROM dbo.AdmUser)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            admUser.Id = Convert.ToInt16(sqlMissingResponse.NextId);
                            admUser.EditById = null;
                            admUser.EditDate = null;

                            // Generate PasswordHash, SecurityStamp, and ConcurrencyStamp
                            var passwordHasher = new PasswordHasher<AdmUser>();
                            admUser.PasswordHash = passwordHasher.HashPassword(admUser, password);
                            admUser.SecurityStamp = Guid.NewGuid().ToString();
                            admUser.ConcurrencyStamp = Guid.NewGuid().ToString();

                            _context.Add(admUser);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        // Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = 0,
                            ModuleId = (short)E_Modules.Admin,
                            TransactionId = (short)E_Admin.User,
                            DocumentId = admUser.Id,
                            DocumentNo = admUser.UserCode,
                            TblName = "AdmUser",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "User Save Successfully",
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = 0,
                        ModuleId = (short)E_Modules.Admin,
                        TransactionId = (short)E_Admin.User,
                        DocumentId = admUser.Id,
                        DocumentNo = admUser.UserCode,
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

        public async Task<SqlResponce> DeleteUserAsync(short CompanyId, short UserId, UserViewModel admUser)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (admUser.UserId > 0)
                    {
                        var UserToRemove = _context.AdmUser.Where(x => x.Id == admUser.UserId).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsActive, false).SetProperty(b => b.EditById, UserId).SetProperty(b => b.EditDate, DateTime.Now));

                        if (UserToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Admin.User,
                                DocumentId = admUser.UserId,
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
                        return new SqlResponce { Result = -1, Message = "Id Should be zero" };
                    }
                    return new SqlResponce();
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

        #endregion User

        #region User Group

        public async Task<UserGroupViewModelCount> GetUserGroupListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            UserGroupViewModelCount countViewModel = new UserGroupViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.AdmUserGroup A_UsrG WHERE (A_UsrG.UserGroupName LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%') AND A_UsrG.UserGroupId<>0");

                var result = await _repository.GetQueryAsync<UserGroupViewModel>($"SELECT A_UsrG.UserGroupId,A_UsrG.UserGroupCode,A_UsrG.UserGroupName,A_UsrG.Remarks,A_UsrG.IsActive,A_UsrG.CreateById,A_UsrG.CreateDate,A_UsrG.EditById,A_UsrG.EditDate ,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmUserGroup A_UsrG LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = A_UsrG.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = A_UsrG.EditById WHERE (A_UsrG.UserGroupName LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%') AND A_UsrG.UserGroupId<>0 ORDER BY A_UsrG.UserGroupName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<UserGroupViewModel>();

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

        public async Task<UserGroupViewModel> GetUserGroupByIdAsync(short CompanyId, short UserId, short Id)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<UserGroupViewModel>($"SELECT A_UsrG.UserGroupId,A_UsrG.UserGroupCode,A_UsrG.UserGroupName,A_UsrG.Remarks,A_UsrG.IsActive,A_UsrG.CreateById,A_UsrG.CreateDate,A_UsrG.EditById,A_UsrG.EditDate ,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmUserGroup A_UsrG LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = A_UsrG.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = A_UsrG.EditById WHERE A_UsrG.UserGroupId={Id}");

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

        public async Task<SqlResponce> SaveUserGroupAsync(short UserId, AdmUserGroup admUserGroup)
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
                        var dataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.AdmUserGroup WHERE Id<>0 AND UserId={admUserGroup.Id} ");

                        if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(admUserGroup);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "UserGroup Not Found" };
                    }
                    else
                    {
                        //Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (Id + 1) FROM dbo.AdmUserGroup WHERE (Id + 1) NOT IN (SELECT Id FROM dbo.AdmUserGroup)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            admUserGroup.Id = Convert.ToInt16(sqlMissingResponse.NextId);
                            admUserGroup.EditById = null;
                            admUserGroup.EditDate = null;
                            _context.Add(admUserGroup);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = 0,
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = 0,
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

        public async Task<SqlResponce> DeleteUserGroupAsync(short CompanyId, short UserId, UserGroupViewModel UserGroup)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (UserGroup.UserGroupId > 0)
                    {
                        var UserGroupToRemove = _context.AdmUserGroup.Where(x => x.Id == UserGroup.UserGroupId).ExecuteDelete();

                        if (UserGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Admin.UserGroup,
                                DocumentId = UserGroup.UserGroupId,
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
                        return new SqlResponce { Result = -1, Message = "Id Should be zero" };
                    }
                    return new SqlResponce();
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

        #endregion User Group

        #region User Rights

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

        public async Task<SqlResponce> SaveUserRightsAsync(short CompanyId, short UserId, List<AdmUserRights> admUserRights, short SelectedUserId)
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

                    await _repository.GetQueryAsync<SqlResponceIds>($"DELETE FROM dbo.AdmUserRights WHERE UserId = {SelectedUserId}");

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
                        return new SqlResponce { Result = 1, Message = "Save Successfully" };
                    }

                    #endregion Audit Log

                    return new SqlResponce { Result = -1, Message = "Save Failed" };
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

        #endregion User Rights

        #region User Group Rights

        public async Task<IEnumerable<UserGroupRightsViewModel>> GetUserGroupRightsByIdAsync(short CompanyId, short UserId, int SelectedGroupId)
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

        public async Task<SqlResponce> SaveUserGroupRightsAsync(short CompanyId, short UserId, List<AdmUserGroupRights> admUserGroupRights, short UserGroupId)
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

                    await _repository.GetQueryAsync<SqlResponceIds>($"DELETE FROM dbo.AdmUserGroupRights WHERE  UserGroupUserId={UserGroupId}");

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
                        return new SqlResponce { Result = 1, Message = "Save Successfully" };
                    }

                    #endregion Audit Log

                    return new SqlResponce { Result = 0, Message = "Save Failed" };
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

        #endregion User Group Rights
    }
}