using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPInvoiceService
    {
        public Task<APInvoiceViewModelCount> GetAPInvoiceListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APInvoiceViewModel> GetAPInvoiceByIdNoAsync(short CompanyId, long InvoiceId, string InvoiceNo, short UserId);

        public Task<SqlResponse> SaveAPInvoiceAsync(short CompanyId, ApInvoiceHd APInvoiceHd, List<ApInvoiceDt> APInvoiceDt, short UserId);

        public Task<SqlResponse> DeleteAPInvoiceAsync(short CompanyId, long InvoiceId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APInvoiceViewModel>> GetHistoryAPInvoiceByIdAsync(short CompanyId, long InvoiceId, string InvoiceNo, short UserId);
    }
}