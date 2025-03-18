using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AP
{
    public interface IAPDebitNoteService
    {
        public Task<APDebitNoteViewModelCount> GetAPDebitNoteListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<APDebitNoteViewModel> GetAPDebitNoteByIdAsync(short CompanyId, long DebitNoteId, string DebitNoteNo, short UserId);

        public Task<SqlResponse> SaveAPDebitNoteAsync(short CompanyId, ApDebitNoteHd APDebitNoteHd, List<ApDebitNoteDt> APDebitNoteDt, short UserId);

        public Task<SqlResponse> DeleteAPDebitNoteAsync(short CompanyId, long DebitNoteId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<APDebitNoteViewModel>> GetHistoryAPDebitNoteByIdAsync(short CompanyId, long DebitNoteId, string DebitNoteNo, short UserId);
    }
}