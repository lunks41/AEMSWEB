using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPCreditNoteService
    {
        public Task<APCreditNoteViewModelCount> GetAPCreditNoteListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APCreditNoteViewModel> GetAPCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId);

        public Task<SqlResponse> SaveAPCreditNoteAsync(short CompanyId, ApCreditNoteHd APCreditNoteHd, List<ApCreditNoteDt> APCreditNoteDt, short UserId);

        public Task<SqlResponse> DeleteAPCreditNoteAsync(short CompanyId, long CreditNoteId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APCreditNoteViewModel>> GetHistoryAPCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId);
    }
}