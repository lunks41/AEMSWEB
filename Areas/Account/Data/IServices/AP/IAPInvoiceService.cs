using AMESWEB.Areas.Account.Models.AP;
using AMESWEB.Entities.Accounts.AP;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AP
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