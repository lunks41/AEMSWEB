using AMESWEB.Areas.Account.Models.AR;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AR
{
    public interface IArInvoiceService
    {
        public Task<ArInvoiceViewModelCount> GetArInvoiceListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, string fromDate, string toDate, bool isShowAll);

        public Task<ArInvoiceViewModel> GetArInvoiceByIdNoAsync(short CompanyId, short UserId, long InvoiceId, string InvoiceNo);

        public Task<SqlResponce> SaveArInvoiceAsync(short CompanyId, short UserId, ArInvoiceHd arInvoiceHd, List<ArInvoiceDt> arInvoiceDt);

        public Task<SqlResponce> DeleteArInvoiceAsync(short CompanyId, short UserId, long InvoiceId, string CanacelRemarks);

        public Task<IEnumerable<ArInvoiceViewModel>> GetHistoryArInvoiceByIdAsync(short CompanyId, short UserId, long InvoiceId, string InvoiceNo);
    }
}