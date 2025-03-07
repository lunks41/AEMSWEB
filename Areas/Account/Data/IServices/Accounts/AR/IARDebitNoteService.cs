using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.AR
{
    public interface IARDebitNoteService
    {
        public Task<ARDebitNoteViewModelCount> GetARDebitNoteListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<ARDebitNoteViewModel> GetARDebitNoteByIdAsync(Int16 CompanyId, Int64 DebitNoteId, string DebitNoteNo, Int16 UserId);

        public Task<SqlResponse> SaveARDebitNoteAsync(Int16 CompanyId, ArDebitNoteHd ARDebitNoteHd, List<ArDebitNoteDt> ARDebitNoteDt, Int16 UserId);

        public Task<SqlResponse> DeleteARDebitNoteAsync(Int16 CompanyId, Int64 DebitNoteId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<ARDebitNoteViewModel>> GetHistoryARDebitNoteByIdAsync(Int16 CompanyId, Int64 DebitNoteId, string DebitNoteNo, Int16 UserId);
    }
}