using AMESWEB.Areas.Account.Models.AR;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AR
{
    public interface IARDebitNoteService
    {
        public Task<ARDebitNoteViewModelCount> GetARDebitNoteListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARDebitNoteViewModel> GetARDebitNoteByIdAsync(short CompanyId, long DebitNoteId, string DebitNoteNo, short UserId);

        public Task<SqlResponse> SaveARDebitNoteAsync(short CompanyId, ArDebitNoteHd ARDebitNoteHd, List<ArDebitNoteDt> ARDebitNoteDt, short UserId);

        public Task<SqlResponse> DeleteARDebitNoteAsync(short CompanyId, long DebitNoteId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARDebitNoteViewModel>> GetHistoryARDebitNoteByIdAsync(short CompanyId, long DebitNoteId, string DebitNoteNo, short UserId);
    }
}