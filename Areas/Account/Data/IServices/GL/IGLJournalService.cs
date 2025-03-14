using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Entities.Accounts.GL;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.GL
{
    public interface IGLJournalService
    {
        public Task<GLJournalViewModel> GetGLJournalListAsync(short CompanyId, short pageSize, short pageNumber, string searchString, string fromDate, string toDate, short UserId);

        public Task<GLJournalHdViewModel> GetGLJournalByIdNoAsync(short CompanyId, long JournalId, string JournalNo, short UserId);

        public Task<SqlResponse> SaveGLJournalAsync(short CompanyId, GLJournalHd GLJournalHd, List<GLJournalDt> GLJournalDts, short UserId);

        public Task<SqlResponse> DeleteGLJournalAsync(short CompanyId, long JournalId, string CanacelRemarks, short UserId);

        public Task<IEnumerable<GLJournalHdViewModel>> GetHistoryGLJournalByIdAsync(short CompanyId, long JournalId, string JournalNo, short UserId);
    }
}