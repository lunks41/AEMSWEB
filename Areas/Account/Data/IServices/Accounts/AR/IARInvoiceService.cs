using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AR
{
    public interface IARInvoiceService
    {
        public Task<ARInvoiceViewModelCount> GetARInvoiceListAsync(Int16 CompanyId, Int16 UserId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate);

        public Task<ARInvoiceViewModel> GetARInvoiceByIdNoAsync(Int16 CompanyId, Int16 UserId, Int64 InvoiceId, string InvoiceNo);

        public Task<SqlResponse> SaveARInvoiceAsync(Int16 CompanyId, Int16 UserId, ArInvoiceHd arInvoiceHd, List<ArInvoiceDt> arInvoiceDt);

        public Task<SqlResponse> DeleteARInvoiceAsync(Int16 CompanyId, Int16 UserId, Int64 InvoiceId, string CanacelRemarks);

        public Task<IEnumerable<ARInvoiceViewModel>> GetHistoryARInvoiceByIdAsync(Int16 CompanyId, Int16 UserId, Int64 InvoiceId, string InvoiceNo);
    }
}