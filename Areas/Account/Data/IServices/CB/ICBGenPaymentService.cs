using AMESWEB.Areas.Account.Models.CB;
using AMESWEB.Entities.Accounts.CB;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBGenPaymentService
    {
        public Task<CBGenPaymentViewModel> GetCBGenPaymentListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<CBGenPaymentHdViewModel> GetCBGenPaymentByIdNoAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);

        public Task<SqlResponce> SaveCBGenPaymentAsync(short CompanyId, CBGenPaymentHd cBGenPaymentHd, List<CBGenPaymentDt> cBGenPaymentDts, short UserId);

        public Task<SqlResponce> DeleteCBGenPaymentAsync(short CompanyId, long PaymentId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBGenPaymentHdViewModel>> GetHistoryCBGenPaymentByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);
    }
}