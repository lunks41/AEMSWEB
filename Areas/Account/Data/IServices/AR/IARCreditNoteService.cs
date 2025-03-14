using AEMSWEB.Areas.Account.Models.AR;
using AEMSWEB.Entities.Accounts.AR;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.AR
{
    public interface IARCreditNoteService
    {
        public Task<ARCreditNoteViewModelCount> GetARCreditNoteListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARCreditNoteViewModel> GetARCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId);

        public Task<SqlResponse> SaveARCreditNoteAsync(short CompanyId, ArCreditNoteHd ARCreditNoteHd, List<ArCreditNoteDt> ARCreditNoteDt, short UserId);

        public Task<SqlResponse> DeleteARCreditNoteAsync(short CompanyId, long CreditNoteId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARCreditNoteViewModel>> GetHistoryARCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId);
    }
}