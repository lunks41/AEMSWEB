using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AR
{
    public interface IARCreditNoteService
    {
        public Task<ARCreditNoteViewModelCount> GetARCreditNoteListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<ARCreditNoteViewModel> GetARCreditNoteByIdAsync(Int16 CompanyId, Int64 CreditNoteId, string CreditNoteNo, Int16 UserId);

        public Task<SqlResponse> SaveARCreditNoteAsync(Int16 CompanyId, ArCreditNoteHd ARCreditNoteHd, List<ArCreditNoteDt> ARCreditNoteDt, Int16 UserId);

        public Task<SqlResponse> DeleteARCreditNoteAsync(Int16 CompanyId, Int64 CreditNoteId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARCreditNoteViewModel>> GetHistoryARCreditNoteByIdAsync(Int16 CompanyId, Int64 CreditNoteId, string CreditNoteNo, Int16 UserId);
    }
}