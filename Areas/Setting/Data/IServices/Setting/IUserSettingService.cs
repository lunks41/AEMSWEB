using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface IUserSettingService
    {
        public Task<UserSettingViewModel> GetUserSettingAsync(Int16 CompanyId, Int16 UserId);

        public Task<SqlResponse> SaveUserSettingAsync(Int16 CompanyId, S_UserSettings S_UserSettings, Int16 UserId);
    }
}