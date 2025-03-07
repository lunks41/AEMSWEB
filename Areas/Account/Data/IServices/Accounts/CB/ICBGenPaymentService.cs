using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.CB
{
    public interface ICBGenPaymentService
    {
        public Task<CBGenPaymentViewModel> GetCBGenPaymentListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<CBGenPaymentHdViewModel> GetCBGenPaymentByIdNoAsync(Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);

        public Task<SqlResponse> SaveCBGenPaymentAsync(Int16 CompanyId, CBGenPaymentHd cBGenPaymentHd, List<CBGenPaymentDt> cBGenPaymentDts, Int16 UserId);

        public Task<SqlResponse> DeleteCBGenPaymentAsync(Int16 CompanyId, Int64 PaymentId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<CBGenPaymentHdViewModel>> GetHistoryCBGenPaymentByIdAsync(Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);
    }
}