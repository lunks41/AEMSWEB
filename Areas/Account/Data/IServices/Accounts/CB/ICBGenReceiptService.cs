using AEMSWEB.Areas.Account.Models.CB;
using AEMSWEB.Entities.Accounts.CB;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.CB
{
    public interface ICBGenReceiptService
    {
        public Task<CBGenReceiptViewModel> GetCBGenReceiptListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<CBGenReceiptHdViewModel> GetCBGenReceiptByIdNoAsync(Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);

        public Task<SqlResponse> SaveCBGenReceiptAsync(Int16 CompanyId, CBGenReceiptHd CBGenReceiptHd, List<CBGenReceiptDt> CBGenReceiptDt, Int16 UserId);

        public Task<SqlResponse> DeleteCBGenReceiptAsync(Int16 CompanyId, Int64 InvoiceId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<CBGenReceiptViewModel>> GetHistoryCBGenReceiptByIdAsync(Int16 CompanyId, Int64 InvoiceId, string InvoiceNo, Int16 UserId);
    }
}