using AEMSWEB.Entities.Admin;
using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Admin
{
    public interface IUserRightsService
    {
        public Task<IEnumerable<UserRightsViewModel>> GetUserRightsByIdAsync(Int16 CompanyId, Int16 SelectedUserId, Int16 UserId);

        public Task<SqlResponse> SaveUserRightsAsync(Int16 CompanyId, List<AdmUserRights> admUserRights, Int16 SelectedUserId, Int16 UserId);
    }
}