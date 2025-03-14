using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPDocSetOffService
    {
        public Task<APDocSetOffViewModelCount> GetAPDocSetOffListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APDocSetOffViewModel> GetAPDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId);

        public Task<SqlResponse> SaveAPDocSetOffAsync(short CompanyId, ApDocSetOffHd APDocSetOffHd, List<ApDocSetOffDt> APDocSetOffDt, short UserId);

        public Task<SqlResponse> DeleteAPDocSetOffAsync(short CompanyId, long SetoffId, string SetoffNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APDocSetOffViewModel>> GetHistoryAPDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId);
    }
}