using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.CB
{
    public interface ICBPettyCashService
    {
        public Task<CBPettyCashViewModel> GetCBPettyCashListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<CBPettyCashHdViewModel> GetCBPettyCashByIdNoAsync(Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);

        public Task<SqlResponse> SaveCBPettyCashAsync(Int16 CompanyId, CBPettyCashHd cBPettyCashHd, List<CBPettyCashDt> cBPettyCashDts, Int16 UserId);

        public Task<SqlResponse> DeleteCBPettyCashAsync(Int16 CompanyId, Int64 PaymentId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<CBPettyCashViewModel>> GetHistoryCBPettyCashByIdAsync(Int16 CompanyId, Int64 PaymentId, string PaymentNo, Int16 UserId);
    }
}