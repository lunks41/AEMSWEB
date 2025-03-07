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

namespace AEMSWEB.Services.Admin
{
    public sealed class UserGroupRightsService : IUserGroupRightsService
    {
        private readonly IRepository<AdmUserGroupRights> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public UserGroupRightsService(IRepository<AdmUserGroupRights> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<IEnumerable<UserGroupRightsViewModel>> GetUserGroupRightsByIdAsync(Int16 CompanyId, Int16 UserGroupId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserGroupRightsViewModel>($"exec Adm_GetGroupAccessRights {CompanyId},{UserGroupId}");

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

        public async Task<SqlResponse> SaveUserGroupRightsAsync(Int16 CompanyId, List<AdmUserGroupRights> admUserGroupRights, Int16 UserGroupId, Int16 UserId)
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