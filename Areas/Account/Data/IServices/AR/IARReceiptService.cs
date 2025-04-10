using AMESWEB.Areas.Account.Models.AR;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AR
{
    public interface IARReceiptService
    {
        public Task<ARReceiptViewModelCount> GetARReceiptListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARReceiptViewModel> GetARReceiptByIdAsync(short CompanyId, long ReceiptId, string ReceiptNo, short UserId);

        public Task<SqlResponce> SaveARReceiptAsync(short CompanyId, ArReceiptHd ARReceiptHd, List<ArReceiptDt> ARReceiptDt, short UserId);

        public Task<SqlResponce> DeleteARReceiptAsync(short CompanyId, long ReceiptId, string ReceiptNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARReceiptViewModel>> GetHistoryARReceiptByIdAsync(short CompanyId, long ReceiptId, string ReceiptNo, short UserId);
    }
}