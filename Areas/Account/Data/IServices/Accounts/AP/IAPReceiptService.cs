using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AP
{
    public interface IAPPaymentService
    {
        public Task<APPaymentViewModelCount> GetAPPaymentListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APPaymentViewModel> GetAPPaymentByIdAsync(Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);

        public Task<SqlResponse> SaveAPPaymentAsync(Int16 CompanyId, ApPaymentHd APPaymentHd, List<ApPaymentDt> APPaymentDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPPaymentAsync(Int16 CompanyId, Int64 PaymentId, string PaymentNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APPaymentViewModel>> GetHistoryAPPaymentByIdAsync(Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);
    }
}