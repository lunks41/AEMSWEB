using AMESWEB.Areas.Account.Models.CB;
using AMESWEB.Entities.Accounts.CB;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBPettyCashService
    {
        public Task<CBPettyCashViewModel> GetCBPettyCashListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<CBPettyCashHdViewModel> GetCBPettyCashByIdNoAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);

        public Task<SqlResponce> SaveCBPettyCashAsync(short CompanyId, CBPettyCashHd cBPettyCashHd, List<CBPettyCashDt> cBPettyCashDts, short UserId);

        public Task<SqlResponce> DeleteCBPettyCashAsync(short CompanyId, long PaymentId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBPettyCashViewModel>> GetHistoryCBPettyCashByIdAsync(short CompanyId, long PaymentId, string PaymentNo, short UserId);
    }
}