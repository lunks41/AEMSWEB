using AEMSWEB.Areas.Account.Models.AP;
using AEMSWEB.Entities.Accounts.AP;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AP
{
    public interface IAPDebitNoteService
    {
        public Task<APDebitNoteViewModelCount> GetAPDebitNoteListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<APDebitNoteViewModel> GetAPDebitNoteByIdAsync(Int16 CompanyId, Int64 DebitNoteId, string DebitNoteNo, Int16 UserId);

        public Task<SqlResponse> SaveAPDebitNoteAsync(Int16 CompanyId, ApDebitNoteHd APDebitNoteHd, List<ApDebitNoteDt> APDebitNoteDt, Int16 UserId);

        public Task<SqlResponse> DeleteAPDebitNoteAsync(Int16 CompanyId, Int64 DebitNoteId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<APDebitNoteViewModel>> GetHistoryAPDebitNoteByIdAsync(Int16 CompanyId, Int64 DebitNoteId, string DebitNoteNo, Int16 UserId);
    }
}