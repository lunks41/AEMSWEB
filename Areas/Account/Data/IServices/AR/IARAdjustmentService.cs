using AMESWEB.Areas.Account.Models.AR;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AR
{
    public interface IARAdjustmentService
    {
        public Task<ARAdjustmentViewModelCount> GetARAdjustmentListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARAdjustmentViewModel> GetARAdjustmentByIdAsync(short CompanyId, long InvoiceId, string InvoiceNo, short UserId);

        public Task<SqlResponse> SaveARAdjustmentAsync(short CompanyId, ArAdjustmentHd ARAdjustmentHd, List<ArAdjustmentDt> ARAdjustmentDt, short UserId);

        public Task<SqlResponse> DeleteARAdjustmentAsync(short CompanyId, long InvoiceId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARAdjustmentViewModel>> GetHistoryARAdjustmentByIdAsync(short CompanyId, long AdjustmentId, string AdjustmentNo, short UserId);
    }
}