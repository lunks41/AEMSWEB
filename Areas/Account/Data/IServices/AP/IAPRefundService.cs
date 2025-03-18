using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPRefundService
    {
        public Task<APRefundViewModelCount> GetAPRefundListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APRefundViewModel> GetAPRefundByIdAsync(short CompanyId, long RefundId, string RefundNo, short UserId);

        public Task<SqlResponse> SaveAPRefundAsync(short CompanyId, ApRefundHd APRefundHd, List<ApRefundDt> APRefundDt, short UserId);

        public Task<SqlResponse> DeleteAPRefundAsync(short CompanyId, long RefundId, string RefundNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APRefundViewModel>> GetHistoryAPRefundByIdAsync(short CompanyId, long RefundId, string RefundNo, short UserId);
    }
}