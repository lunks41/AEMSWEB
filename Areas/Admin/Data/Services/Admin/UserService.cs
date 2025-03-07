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

        public async Task<UserViewModelCount> GetUserListAsync(Int16 CompanyId, short pageSize, short pageNumber, string searchString, Int16 Id)
        {
            UserViewModelCount countViewModel = new UserViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.AdmUser A_Usr INNER JOIN dbo.AdmUserGroup A_UsrG ON A_UsrG.UserGroupId = A_Usr.UserGroupId WHERE A_Usr.Id<>0 AND (A_Usr.UserName LIKE '%{searchString}%' OR A_Usr.UserCode LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%' OR A_UsrG.UserGroupName LIKE '%{searchString}%')");

                var result = await _repository.GetQueryAsync<UserViewModel>($"SELECT A_Usr.Id,A_Usr.UserCode,A_Usr.UserName,A_Usr.UserEmail,A_Usr.Remarks,A_Usr.IsActive,A_Usr.UserGroupId,A_UsrG.UserGroupCode,A_UsrG.UserGroupName,A_Usr.CreateById,A_Usr.CreateDate,A_Usr.EditById,A_Usr.EditDate ,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.AdmUser A_Usr INNER JOIN dbo.AdmUserGroup A_UsrG ON A_UsrG.UserGroupId = A_Usr.UserGroupId LEFT JOIN dbo.AdmUser Usr ON Usr.Id = A_Usr.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.Id = A_Usr.EditById WHERE A_Usr.Id<>0 AND (A_Usr.UserName LIKE '%{searchString}%' OR A_Usr.UserCode LIKE '%{searchString}%' OR A_UsrG.UserGroupCode LIKE '%{searchString}%' OR A_UsrG.UserGroupName LIKE '%{searchString}%') ORDER BY A_Usr.UserName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    CreateById = Id
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<AdmUser> GetUserByIdAsync(Int16 CompanyId, Int16 Userid, Int16 Id)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AdmUser>($"SELECT Id,UserCode,UserName,UserEmail,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.AdmUser WHERE Id={Userid}");

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
                    CreateById = Id,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }


        public async Task<SqlResponse> DeleteUserAsync(Int16 CompanyId, AdmUser admUser, Int16 Id)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (admUser.Id > 0)
                    {
                        var UserToRemove = _context.AdmUser.Where(x => x.Id == admUser.Id).ExecuteUpdate(setPropertyCalls: setters => setters.SetProperty(b => b.IsActive, false).SetProperty(b => b.EditById, Id).SetProperty(b => b.EditDate, DateTime.Now));

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
                                CreateById = Id
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
                        CreateById = Id,
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
    }
}