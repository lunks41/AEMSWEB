using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices
{
    public interface IBaseService
    {
        UserGroupRightsViewModel ValidateScreen(Int16 companyId, Int16 ModuleId, Int16 TransactionId, Int32 userId);
    }
}