using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AR
{
    public interface IARInvoiceService
    {
        public Task<ARInvoiceViewModelCount> GetARInvoiceListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate);

        public Task<ARInvoiceViewModel> GetARInvoiceByIdNoAsync(short CompanyId, short UserId, long InvoiceId, string InvoiceNo);

        public Task<SqlResponse> SaveARInvoiceAsync(short CompanyId, short UserId, ArInvoiceHd arInvoiceHd, List<ArInvoiceDt> arInvoiceDt);

        public Task<SqlResponse> DeleteARInvoiceAsync(short CompanyId, short UserId, long InvoiceId, string CanacelRemarks);

        public Task<IEnumerable<ARInvoiceViewModel>> GetHistoryARInvoiceByIdAsync(short CompanyId, short UserId, long InvoiceId, string InvoiceNo);
    }
}