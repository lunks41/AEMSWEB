using AMESWEB.Areas.Account.Models.AR;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AR
{
    public interface IARDocSetOffService
    {
        public Task<ARDocSetOffViewModelCount> GetARDocSetOffListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARDocSetOffViewModel> GetARDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId);

        public Task<SqlResponce> SaveARDocSetOffAsync(short CompanyId, ArDocSetOffHd ARDocSetOffHd, List<ArDocSetOffDt> ARDocSetOffDt, short UserId);

        public Task<SqlResponce> DeleteARDocSetOffAsync(short CompanyId, long SetoffId, string SetoffNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARDocSetOffViewModel>> GetHistoryARDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId);
    }
}