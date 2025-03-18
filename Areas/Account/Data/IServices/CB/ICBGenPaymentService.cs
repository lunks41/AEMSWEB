using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBGenPaymentService
    {
        public Task<CBGenPaymentViewModel> GetCBGenPaymentListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<CBGenPaymentHdViewModel> GetCBGenPaymentByIdNoAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);

        public Task<SqlResponse> SaveCBGenPaymentAsync(short CompanyId, CBGenPaymentHd cBGenPaymentHd, List<CBGenPaymentDt> cBGenPaymentDts, short UserId);

        public Task<SqlResponse> DeleteCBGenPaymentAsync(short CompanyId, long PaymentId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBGenPaymentHdViewModel>> GetHistoryCBGenPaymentByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);
    }
}