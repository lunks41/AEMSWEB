using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Admin
{
    public interface IUserService
    {
        public Task<UserViewModelCount> GetUserListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, Int16 UserId);

        public Task<AdmUser> GetUserByIdAsync(Int16 CompanyId, Int16 Userid, Int16 UserId);


        public Task<SqlResponse> DeleteUserAsync(Int16 CompanyId, AdmUser admUser, Int16 UserId);

    }
}