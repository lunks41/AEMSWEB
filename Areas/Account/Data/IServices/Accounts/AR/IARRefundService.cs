using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AR
{
    public interface IARRefundService
    {
        public Task<ARRefundViewModelCount> GetARRefundListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<ARRefundViewModel> GetARRefundByIdAsync(Int16 CompanyId, Int64 RefundId, string RefundNo, Int16 UserId);

        public Task<SqlResponse> SaveARRefundAsync(Int16 CompanyId, ArRefundHd ARRefundHd, List<ArRefundDt> ARRefundDt, Int16 UserId);

        public Task<SqlResponse> DeleteARRefundAsync(Int16 CompanyId, Int64 RefundId, string RefundNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARRefundViewModel>> GetHistoryARRefundByIdAsync(Int16 CompanyId, Int64 RefundId, string RefundNo, Int16 UserId);
    }
}