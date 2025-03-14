using AEMSWEB.Models.Admin;

namespace AEMSWEB.Areas.Admin.Data
{
    public interface IModuleService
    {
        public Task<IEnumerable<UserModuleViewModel>> GetUsersModulesAsync(short CompanyId, short UserId);
    }
}