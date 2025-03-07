using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AR
{
    public interface IARAdjustmentService
    {
        public Task<ARAdjustmentViewModelCount> GetARAdjustmentListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<ARAdjustmentViewModel> GetARAdjustmentByIdAsync(Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponse> SaveARAdjustmentAsync(Int16 CompanyId, ArAdjustmentHd ARAdjustmentHd, List<ArAdjustmentDt> ARAdjustmentDt, Int16 UserId);

        public Task<SqlResponse> DeleteARAdjustmentAsync(Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARAdjustmentViewModel>> GetHistoryARAdjustmentByIdAsync(Int16 CompanyId, Int64 AdjustmentId, string AdjustmentNo, Int16 UserId);
    }
}