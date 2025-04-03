using AMESWEB.Areas.Account.Models.AP;
using AMESWEB.Entities.Accounts.AP;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPCreditNoteService
    {
        public Task<APCreditNoteViewModelCount> GetAPCreditNoteListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APCreditNoteViewModel> GetAPCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId);

        public Task<SqlResponse> SaveAPCreditNoteAsync(short CompanyId, ApCreditNoteHd APCreditNoteHd, List<ApCreditNoteDt> APCreditNoteDt, short UserId);

        public Task<SqlResponse> DeleteAPCreditNoteAsync(short CompanyId, long CreditNoteId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APCreditNoteViewModel>> GetHistoryAPCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId);
    }
}