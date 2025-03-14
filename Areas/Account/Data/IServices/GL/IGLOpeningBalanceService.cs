using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Account.Data.IServices.GL
{
    public interface IGLOpeningBalanceService
    {
        public Task<IEnumerable<GLOpeningBalanceViewModel>> GetGLOpeningBalanceListAsync(short CompanyId, int DocumentId, short UserId);

        public Task<SqlResponse> SaveGLOpeningBalanceAsync(short CompanyId, GLOpeningBalanceViewModel gLOpeningBalanceViewModel, short UserId);

        public Task<IEnumerable<GLOpeningBalanceViewModel>> GenerateGLYearEndProcessAsyn(short CompanyId, int FinYear, short UserId);
    }
}