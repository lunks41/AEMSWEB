using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AP
{
    public interface IAPDocSetOffService
    {
        public Task<APDocSetOffViewModelCount> GetAPDocSetOffListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APDocSetOffViewModel> GetAPDocSetOffByIdAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId);

        public Task<SqlResponse> SaveAPDocSetOffAsync(Int16 CompanyId, ApDocSetOffHd APDocSetOffHd, List<ApDocSetOffDt> APDocSetOffDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPDocSetOffAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APDocSetOffViewModel>> GetHistoryAPDocSetOffByIdAsync(Int16 CompanyId, Int64 SetoffId, string SetoffNo, Int16 UserId);
    }
}