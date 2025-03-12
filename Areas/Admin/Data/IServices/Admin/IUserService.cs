using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Admin
{
    public interface IUserService
    {
        public Task<UserViewModelCount> GetUserListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AdmUser> GetUserByIdAsync(short CompanyId, short UserId);

        public Task<SqlResponse> DeleteUserAsync(short CompanyId, short UserId, AdmUser admUser);

        public Task<UserGroupViewModelCount> GetUserGroupListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AdmUserGroup> GetUserGroupByIdAsync(short CompanyId, short UserId, Int16 UserGroupId);

        public Task<SqlResponse> SaveUserGroupAsync(short CompanyId, short UserId, AdmUserGroup admUserGroup);

        public Task<SqlResponse> DeleteUserGroupAsync(short CompanyId, short UserId, AdmUserGroup admUserGroup);

        public Task<IEnumerable<UserRightsViewModel>> GetUserRightsByIdAsync(short CompanyId, short UserId, int SelectedUserId);

        public Task<SqlResponse> SaveUserRightsAsync(short CompanyId, short UserId, List<AdmUserRights> admUserRights, Int16 SelectedUserId);

        public Task<IEnumerable<UserGroupRightsViewModel>> GetUserGroupRightsByIdAsync(short CompanyId, short UserId, int SelectedUserId, int SelectedGroupId);

        public Task<SqlResponse> SaveUserGroupRightsAsync(short CompanyId, short UserId, List<AdmUserGroupRights> admUserGroupRights, Int16 UserGroupId);
    }
}