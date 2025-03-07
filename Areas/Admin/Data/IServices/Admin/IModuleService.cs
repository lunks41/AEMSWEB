using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Masters
{
    public interface IModuleService
    {
        public Task<IEnumerable<UserModuleViewModel>> GetUsersModulesAsync(Int16 CompanyId, Int16 UserId);
    }
}