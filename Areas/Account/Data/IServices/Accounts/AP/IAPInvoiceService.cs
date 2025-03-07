using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AP
{
    public interface IAPInvoiceService
    {
        public Task<APInvoiceViewModelCount> GetAPInvoiceListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APInvoiceViewModel> GetAPInvoiceByIdNoAsync(Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponse> SaveAPInvoiceAsync(Int16 CompanyId, ApInvoiceHd APInvoiceHd, List<ApInvoiceDt> APInvoiceDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPInvoiceAsync(Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APInvoiceViewModel>> GetHistoryAPInvoiceByIdAsync(Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);
    }
}