using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AP
{
    public interface IAPCreditNoteService
    {
        public Task<APCreditNoteViewModelCount> GetAPCreditNoteListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APCreditNoteViewModel> GetAPCreditNoteByIdAsync(Int16 CompanyId, Int64 CreditNoteId, string CreditNoteNo, Int16 UserId);

        public Task<SqlResponse> SaveAPCreditNoteAsync(Int16 CompanyId, ApCreditNoteHd APCreditNoteHd, List<ApCreditNoteDt> APCreditNoteDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPCreditNoteAsync(Int16 CompanyId, Int64 CreditNoteId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APCreditNoteViewModel>> GetHistoryAPCreditNoteByIdAsync(Int16 CompanyId, Int64 CreditNoteId, string CreditNoteNo, Int16 UserId);
    }
}