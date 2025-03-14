using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBPettyCashService
    {
        public Task<CBPettyCashViewModel> GetCBPettyCashListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<CBPettyCashHdViewModel> GetCBPettyCashByIdNoAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);

        public Task<SqlResponse> SaveCBPettyCashAsync(short CompanyId, CBPettyCashHd cBPettyCashHd, List<CBPettyCashDt> cBPettyCashDts, short UserId);

        public Task<SqlResponse> DeleteCBPettyCashAsync(short CompanyId, long PaymentId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBPettyCashViewModel>> GetHistoryCBPettyCashByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);
    }
}