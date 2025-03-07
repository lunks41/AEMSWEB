using AEMSWEB.Entities.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Admin
{
    public interface IUserGroupService
    {
        public Task<UserGroupViewModelCount> GetUserGroupListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<AdmUserGroup> GetUserGroupByIdAsync(Int16 CompanyId, Int16 UserGroupId, Int16 UserId);

        public Task<SqlResponse> SaveUserGroupAsync(Int16 CompanyId, AdmUserGroup admUserGroup, Int16 UserId);

        public Task<SqlResponse> DeleteUserGroupAsync(Int16 CompanyId, AdmUserGroup admUserGroup, Int16 UserId);
    }
}