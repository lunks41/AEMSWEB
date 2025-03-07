using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AR
{
    public interface IARDocSetOffService
    {
        public Task<ARDocSetOffViewModelCount> GetARDocSetOffListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<ARDocSetOffViewModel> GetARDocSetOffByIdAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId);

        public Task<SqlResponse> SaveARDocSetOffAsync(Int16 CompanyId, ArDocSetOffHd ARDocSetOffHd, List<ArDocSetOffDt> ARDocSetOffDt, Int16 UserId);

        public Task<SqlResponse> DeleteARDocSetOffAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARDocSetOffViewModel>> GetHistoryARDocSetOffByIdAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId);
    }
}