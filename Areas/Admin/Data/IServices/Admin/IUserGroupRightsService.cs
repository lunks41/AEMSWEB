using AEMSWEB.Entities.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Admin
{
    public interface IUserGroupRightsService
    {
        public Task<IEnumerable<UserGroupRightsViewModel>> GetUserGroupRightsByIdAsync(Int16 CompanyId, Int16 UserGroupId, Int16 UserId);

        public Task<SqlResponse> SaveUserGroupRightsAsync(Int16 CompanyId, List<AdmUserGroupRights> admUserGroupRights, Int16 UserGroupId, Int16 UserId);
    }
}