using AMESWEB.Models;
using AMESWEB.Models.Admin;

namespace AMESWEB.Areas.Admin.Data
{
    public interface IUserService
    {
        #region User

        public Task<UserViewModelCount> GetUserListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<UserViewModel> GetUserByIdAsync(short CompanyId, short UserId);

        public Task<SqlResponse> SaveUserAsync(short UserId, AdmUser admUser, string password);

        public Task<SqlResponse> DeleteUserAsync(short CompanyId, short UserId, UserViewModel admUser);

        #endregion User

        #region User Group

        public Task<UserGroupViewModelCount> GetUserGroupListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<UserGroupViewModel> GetUserGroupByIdAsync(short CompanyId, short UserId, short UserGroupId);

        public Task<SqlResponse> SaveUserGroupAsync(short UserId, AdmUserGroup admUserGroup);

        public Task<SqlResponse> DeleteUserGroupAsync(short CompanyId, short UserId, UserGroupViewModel admUserGroup);

        #endregion User Group

        #region User Rights

        public Task<IEnumerable<UserRightsViewModel>> GetUserRightsByIdAsync(short CompanyId, short UserId, int SelectedUserId);

        public Task<SqlResponse> SaveUserRightsAsync(short CompanyId, short UserId, List<AdmUserRights> admUserRights, short SelectedUserId);

        #endregion User Rights

        #region User Group Rights

        public Task<IEnumerable<UserGroupRightsViewModel>> GetUserGroupRightsByIdAsync(short CompanyId, short UserId, int SelectedGroupId);

        public Task<SqlResponse> SaveUserGroupRightsAsync(short CompanyId, short UserId, List<AdmUserGroupRights> admUserGroupRights, short UserGroupId);

        #endregion User Group Rights
    }
}