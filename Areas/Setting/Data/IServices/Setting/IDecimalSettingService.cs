using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface IDecimalSettingService
    {
        public Task<DecimalSettingViewModel> GetDecSettingAsync(Int16 CompanyId, Int16 UserId);

        public Task<SqlResponse> SaveDecSettingAsync(Int16 CompanyId, S_DecSettings s_DecSettings, Int16 UserId);
    }
}