using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AP
{
    public interface IAPAdjustmentService
    {
        public Task<APAdjustmentViewModelCount> GetAPAdjustmentListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APAdjustmentViewModel> GetAPAdjustmentByIdAsync(Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponse> SaveAPAdjustmentAsync(Int16 CompanyId, ApAdjustmentHd APAdjustmentHd, List<ApAdjustmentDt> APAdjustmentDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPAdjustmentAsync(Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APAdjustmentViewModel>> GetHistoryAPAdjustmentByIdAsync(Int16 CompanyId, Int64 AdjustmentId, string AdjustmentNo, Int16 UserId);
    }
}