using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface IFinanceSettingService
    {
        public Task<FinanceSettingViewModel> GetFinSettingAsync(Int16 CompanyId, Int16 UserId);

        public Task<SqlResponse> SaveFinSettingAsync(Int16 CompanyId, S_FinSettings s_FinSettings, Int16 UserId);
    }
}