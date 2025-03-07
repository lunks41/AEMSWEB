using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Entities.Accounts.GL;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.GL
{
    public interface IGLJournalService
    {
        public Task<GLJournalViewModel> GetGLJournalListAsync(Int16 CompanyId, Int16 pageSize, Int16 pageNumber, string searchString, string fromDate, string toDate, Int16 UserId);

        public Task<GLJournalHdViewModel> GetGLJournalByIdNoAsync(Int16 CompanyId, Int64 JournalId, string JournalNo, Int16 UserId);

        public Task<SqlResponse> SaveGLJournalAsync(Int16 CompanyId, GLJournalHd GLJournalHd, List<GLJournalDt> GLJournalDts, Int16 UserId);

        public Task<SqlResponse> DeleteGLJournalAsync(Int16 CompanyId, Int64 JournalId, string CanacelRemarks, Int16 UserId);

        public Task<IEnumerable<GLJournalHdViewModel>> GetHistoryGLJournalByIdAsync(Int16 CompanyId, Int64 JournalId, string JournalNo, Int16 UserId);
    }
}