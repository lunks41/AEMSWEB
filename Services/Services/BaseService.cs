using AEMSWEB.Data;
using AEMSWEB.IServices;
using AEMSWEB.Models.Admin;
using AEMSWEB.Repository;

namespace AEMSWEB.Services
{
    public class BaseService : IBaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<UserGroupRightsViewModel> _repository;

        public BaseService(IRepository<UserGroupRightsViewModel> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<UserGroupRightsViewModel> ValidateScreen(Int16 companyId, Int32 userId, Int16 ModuleId, Int16 TransactionId)
        {
            try
            {
                var userGroupRightsViewModels = _repository.GetQuerySingleOrDefaultAsync<UserGroupRightsViewModel>($"select GroupRights.ModuleId,GroupRights.TransactionId,GroupRights.IsRead,GroupRights.IsCreate,GroupRights.IsEdit,GroupRights.IsDelete,GroupRights.IsExport,GroupRights.IsPrint from AdmUserGroupRights GroupRights INNER Join AdmUser Auser on GroupRights.UserGroupId=Auser.UserGroupId inner join AdmUserRights UserRights on UserRights.UserId=AUser.UserId where UserRights.CompanyId={companyId} And UserRights.UserId= {userId}And GroupRights.ModuleId={ModuleId} And GroupRights.TransactionId={TransactionId}");

                var userGroupRightsViewModels1 = _repository.GetQuerySingleOrDefaultAsync<dynamic>($"select GroupRights.ModuleId,GroupRights.TransactionId,GroupRights.IsRead,GroupRights.IsCreate,GroupRights.IsEdit,GroupRights.IsDelete,GroupRights.IsExport,GroupRights.IsPrint from AdmUserGroupRights GroupRights INNER Join AdmUser Auser on GroupRights.UserGroupId=Auser.UserGroupId inner join AdmUserRights UserRights on UserRights.UserId=AUser.UserId where UserRights.CompanyId={companyId} And UserRights.UserId= {userId}And GroupRights.ModuleId={ModuleId} And GroupRights.TransactionId={TransactionId}");

                return userGroupRightsViewModels.Result;
            }
            catch
            {
                throw;
            }
        }

        //public async Task<bool> HasPermission(string username, string module, string permissionType)
        //{
        //    // Example implementation - adjust based on your permission structure
        //    return await _context.UserPermissions
        //        .AnyAsync(up =>
        //            up.User.Username == username &&
        //            up.Module == module &&
        //            up.PermissionType == permissionType);
        //}
    }
}