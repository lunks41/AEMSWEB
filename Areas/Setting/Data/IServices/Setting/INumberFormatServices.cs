using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface INumberFormatServices
    {
        public Task<ModelNameViewModelCount> GetNumberFormatListAsync(Int16 CompanyId, Int16 UserId);

        public Task<NumberSettingViewModel> GetNumberFormatByIdAsync(Int16 CompanyId, Int32 ModuleId, Int32 TransactionId, Int16 UserId);

        public Task<NumberSettingDtViewModel> GetNumberFormatByYearAsync(Int16 CompanyId, Int32 NumberId, Int32 NumYear, Int16 UserId);

        public Task<SqlResponse> SaveNumberFormatAsync(Int16 CompanyId, S_NumberFormat s_NumberFormat, Int16 UserId);
    }
}