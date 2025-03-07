using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AP
{
    public interface IAPRefundService
    {
        public Task<APRefundViewModelCount> GetAPRefundListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APRefundViewModel> GetAPRefundByIdAsync(Int16 CompanyId, Int64 RefundId, string RefundNo, Int16 UserId);

        public Task<SqlResponse> SaveAPRefundAsync(Int16 CompanyId, ApRefundHd APRefundHd, List<ApRefundDt> APRefundDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPRefundAsync(Int16 CompanyId, Int64 RefundId, string RefundNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APRefundViewModel>> GetHistoryAPRefundByIdAsync(Int16 CompanyId, Int64 RefundId, string RefundNo, Int16 UserId);
    }
}