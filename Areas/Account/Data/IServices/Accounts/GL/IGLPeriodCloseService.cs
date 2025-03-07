using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.GL
{
    public interface IGLPeriodCloseService
    {
        public Task<IEnumerable<GLPeriodCloseViewModel>> GetGLPeriodCloseListAsync(Int16 CompanyId, Int32 FinYear, Int16 UserId);

        public Task<SqlResponse> SaveGLPeriodCloseAsync(Int16 CompanyId, PeriodCloseViewModel periodCloseViewModel, Int16 UserId);
    }
}