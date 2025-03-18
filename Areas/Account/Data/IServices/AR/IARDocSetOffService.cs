using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AR
{
    public interface IARDocSetOffService
    {
        public Task<ARDocSetOffViewModelCount> GetARDocSetOffListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARDocSetOffViewModel> GetARDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId);

        public Task<SqlResponse> SaveARDocSetOffAsync(short CompanyId, ArDocSetOffHd ARDocSetOffHd, List<ArDocSetOffDt> ARDocSetOffDt, short UserId);

        public Task<SqlResponse> DeleteARDocSetOffAsync(short CompanyId, long SetoffId, string SetoffNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARDocSetOffViewModel>> GetHistoryARDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId);
    }
}