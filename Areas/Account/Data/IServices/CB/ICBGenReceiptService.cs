using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.CB
{
    public interface ICBGenReceiptService
    {
        public Task<CBGenReceiptViewModel> GetCBGenReceiptListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<CBGenReceiptHdViewModel> GetCBGenReceiptByIdNoAsync(short CompanyId, long InvoiceId, string InvoiceNo, short UserId);

        public Task<SqlResponse> SaveCBGenReceiptAsync(short CompanyId, CBGenReceiptHd CBGenReceiptHd, List<CBGenReceiptDt> CBGenReceiptDt, short UserId);

        public Task<SqlResponse> DeleteCBGenReceiptAsync(short CompanyId, long InvoiceId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<CBGenReceiptViewModel>> GetHistoryCBGenReceiptByIdAsync(short CompanyId, long InvoiceId, string InvoiceNo, short UserId);
    }
}