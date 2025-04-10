using AMESWEB.Areas.Account.Models.AR;
using AMESWEB.Entities.Accounts.AR;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.AR
{
    public interface IARCreditNoteService
    {
        public Task<ARCreditNoteViewModelCount> GetARCreditNoteListAsync(short CompanyId, int pageSize, int pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<ARCreditNoteViewModel> GetARCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId);

        public Task<SqlResponce> SaveARCreditNoteAsync(short CompanyId, ArCreditNoteHd ARCreditNoteHd, List<ArCreditNoteDt> ARCreditNoteDt, short UserId);

        public Task<SqlResponce> DeleteARCreditNoteAsync(short CompanyId, long CreditNoteId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<ARCreditNoteViewModel>> GetHistoryARCreditNoteByIdAsync(short CompanyId, long CreditNoteId, string CreditNoteNo, short UserId);
    }
}