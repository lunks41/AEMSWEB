using AMESWEB.Areas.Account.Models.AP;
using AMESWEB.Entities.Accounts.AP;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPDocSetOffService
    {
        public Task<APDocSetOffViewModelCount> GetAPDocSetOffListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APDocSetOffViewModel> GetAPDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId);

        public Task<SqlResponce> SaveAPDocSetOffAsync(short CompanyId, ApDocSetOffHd APDocSetOffHd, List<ApDocSetOffDt> APDocSetOffDt, short UserId);

        public Task<SqlResponce> DeleteAPDocSetOffAsync(short CompanyId, long SetoffId, string SetoffNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APDocSetOffViewModel>> GetHistoryAPDocSetOffByIdAsync(short CompanyId, long SetoffId, string SetoffNo, short UserId);
    }
}