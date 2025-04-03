using AMESWEB.Areas.Account.Models.AP;
using AMESWEB.Entities.Accounts.AP;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPPaymentService
    {
        public Task<APPaymentViewModelCount> GetAPPaymentListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APPaymentViewModel> GetAPPaymentByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);

        public Task<SqlResponse> SaveAPPaymentAsync(short CompanyId, ApPaymentHd APPaymentHd, List<ApPaymentDt> APPaymentDt, short UserId);

        public Task<SqlResponse> DeleteAPPaymentAsync(short CompanyId, long PaymentId, string PaymentNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APPaymentViewModel>> GetHistoryAPPaymentByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);
    }
}