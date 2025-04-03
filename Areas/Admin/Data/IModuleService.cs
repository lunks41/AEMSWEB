using AMESWEB.Models.Admin;

namespace AMESWEB.Areas.Admin.Data
{
    public interface IModuleService
    {
        public Task<IEnumerable<UserModuleViewModel>> GetUsersModulesAsync(short CompanyId, short UserId);
    }
}