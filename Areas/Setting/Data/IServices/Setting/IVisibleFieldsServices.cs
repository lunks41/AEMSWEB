using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface IVisibleFieldsServices
    {
        public Task<VisibleFieldsViewModel> GetVisibleFieldsByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId);

        public Task<IEnumerable<VisibleFieldsViewModel>> GetVisibleFieldsByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 UserId);

        public Task<SqlResponse> SaveVisibleFieldsAsync(Int16 CompanyId, List<S_VisibleFields> s_VisibleFields, Int16 UserId);
    }
}