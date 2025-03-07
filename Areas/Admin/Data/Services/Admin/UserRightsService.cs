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
    public sealed class UserRightsService : IUserRightsService
    {
        private readonly IRepository<AdmUserRights> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public UserRightsService(IRepository<AdmUserRights> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<IEnumerable<UserRightsViewModel>> GetUserRightsByIdAsync(Int16 CompanyId, Int16 SelectedUserId, Int16 UserId)
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

        public async Task<SqlResponse> SaveUserRightsAsync(Int16 CompanyId, List<AdmUserRights> admUserRights, Int16 SelectedUserId, Int16 UserId)
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
    }
}