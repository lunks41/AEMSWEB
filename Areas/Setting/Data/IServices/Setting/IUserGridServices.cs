using AEMSWEB.Areas.Setting.Models;
using AEMSWEB.Entities.Setting;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Setting
{
    public interface IUserGridServices
    {
        public Task<UserGridViewModelCount> GetUserGridAsync(Int16 CompanyId, Int16 ModuleId, Int16 TransactionId, Int16 UserId);

        public Task<UserGridViewModel> GetUserGridByIdAsync(Int16 CompanyId, UserGridViewModel userGridViewModel, Int16 UserId);

        public Task<IEnumerable<UserGridViewModel>> GetUserGridByUserIdAsync(Int16 CompanyId, Int16 SelectedUserId, Int16 UserId);

        public Task<SqlResponse> SaveUserGridAsync(Int16 CompanyId, S_UserGrdFormat s_UserGrdFormat, Int16 UserId);

        public Task<SqlResponse> CloneUserGridSettingAsync(Int16 CompanyId, Int16 FromUserId, Int16 ToUserId, Int16 UserId);
    }
}