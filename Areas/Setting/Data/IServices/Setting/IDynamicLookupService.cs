using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface IDynamicLookupService
    {
        public Task<DynamicLookupViewModel> GetDynamicLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<SqlResponse> SaveDynamicLookupAsync(Int16 CompanyId, S_DynamicLookup s_DynamicLookup, Int16 UserId);
    }
}