using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AR
{
    public interface IARRefundService
    {
        public Task<ARRefundViewModelCount> GetARRefundListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARRefundViewModel> GetARRefundByIdAsync(short CompanyId, long RefundId, string RefundNo, short UserId);

        public Task<SqlResponse> SaveARRefundAsync(short CompanyId, ArRefundHd ARRefundHd, List<ArRefundDt> ARRefundDt, short UserId);

        public Task<SqlResponse> DeleteARRefundAsync(short CompanyId, long RefundId, string RefundNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARRefundViewModel>> GetHistoryARRefundByIdAsync(short CompanyId, long RefundId, string RefundNo, short UserId);
    }
}