using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AR
{
    public interface IARReceiptService
    {
        public Task<ARReceiptViewModelCount> GetARReceiptListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<ARReceiptViewModel> GetARReceiptByIdAsync(Int16 CompanyId, Int64 ReceiptId, string ReceiptNo, Int16 UserId);

        public Task<SqlResponse> SaveARReceiptAsync(Int16 CompanyId, ArReceiptHd ARReceiptHd, List<ArReceiptDt> ARReceiptDt, Int16 UserId);

        public Task<SqlResponse> DeleteARReceiptAsync(Int16 CompanyId, Int64 ReceiptId, string ReceiptNo, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARReceiptViewModel>> GetHistoryARReceiptByIdAsync(Int16 CompanyId, Int64 ReceiptId, string ReceiptNo, Int16 UserId);
    }
}