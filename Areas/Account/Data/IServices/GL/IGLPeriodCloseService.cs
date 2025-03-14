using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.GL
{
    public interface IGLPeriodCloseService
    {
        public Task<IEnumerable<GLPeriodCloseViewModel>> GetGLPeriodCloseListAsync(short CompanyId, int FinYear, short UserId);

        public Task<SqlResponse> SaveGLPeriodCloseAsync(short CompanyId, PeriodCloseViewModel periodCloseViewModel, short UserId);
    }
}