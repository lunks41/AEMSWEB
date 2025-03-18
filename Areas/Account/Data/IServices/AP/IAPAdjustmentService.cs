using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPAdjustmentService
    {
        public Task<APAdjustmentViewModelCount> GetAPAdjustmentListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APAdjustmentViewModel> GetAPAdjustmentByIdAsync(short CompanyId, long InvoiceId, string InvoiceNo, short UserId);

        public Task<SqlResponse> SaveAPAdjustmentAsync(short CompanyId, ApAdjustmentHd APAdjustmentHd, List<ApAdjustmentDt> APAdjustmentDt, short UserId);

        public Task<SqlResponse> DeleteAPAdjustmentAsync(short CompanyId, long InvoiceId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APAdjustmentViewModel>> GetHistoryAPAdjustmentByIdAsync(short CompanyId, long AdjustmentId, string AdjustmentNo, short UserId);
    }
}