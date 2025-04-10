using AMESWEB.Areas.Account.Models.GL;
using AMESWEB.Models;

namespace AMESWEB.Areas.Account.Data.IServices.GL
{
    public interface IGLPeriodCloseService
    {
        public Task<IEnumerable<GLPeriodCloseViewModel>> GetGLPeriodCloseListAsync(short CompanyId, int FinYear, short UserId);

        public Task<SqlResponce> SaveGLPeriodCloseAsync(short CompanyId, PeriodCloseViewModel periodCloseViewModel, short UserId);
    }
}