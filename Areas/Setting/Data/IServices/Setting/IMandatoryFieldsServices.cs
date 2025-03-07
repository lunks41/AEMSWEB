using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface IMandatoryFieldsServices
    {
        public Task<MandatoryFieldsViewModel> GetMandatoryFieldsByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId);

        public Task<IEnumerable<MandatoryFieldsViewModel>> GetMandatoryFieldsByIdAsync(Int16 CompanyId, Int16 ModuleId, Int16 UserId);

        public Task<SqlResponse> SaveMandatoryFieldsAsync(Int16 CompanyId, List<S_MandatoryFields> s_MandatoryFields, Int16 UserId);
    }
}