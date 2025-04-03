using AMESWEB.Models.Admin;

namespace AMESWEB.IServices
{
    public interface IBaseService
    {
        Task<UserGroupRightsViewModel> ValidateScreen(Int16 companyId, Int32 userId, Int16 ModuleId, Int16 TransactionId);

        //<bool> HasPermission(string username, string module, string permissionType);
    }
}