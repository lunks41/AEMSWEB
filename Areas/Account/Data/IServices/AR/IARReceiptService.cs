using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AR
{
    public interface IARReceiptService
    {
        public Task<ARReceiptViewModelCount> GetARReceiptListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARReceiptViewModel> GetARReceiptByIdAsync(short CompanyId, long ReceiptId, string ReceiptNo, short UserId);

        public Task<SqlResponse> SaveARReceiptAsync(short CompanyId, ArReceiptHd ARReceiptHd, List<ArReceiptDt> ARReceiptDt, short UserId);

        public Task<SqlResponse> DeleteARReceiptAsync(short CompanyId, long ReceiptId, string ReceiptNo, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARReceiptViewModel>> GetHistoryARReceiptByIdAsync(short CompanyId, long ReceiptId, string ReceiptNo, short UserId);
    }
}