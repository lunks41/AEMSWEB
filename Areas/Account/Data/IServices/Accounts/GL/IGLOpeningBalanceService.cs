using AEMSWEB.Areas.Account.Models.GL;
using AEMSWEB.Models;

namespace AEMSWEB.IServices.Accounts.GL
{
    public interface IGLOpeningBalanceService
    {
        public Task<IEnumerable<GLOpeningBalanceViewModel>> GetGLOpeningBalanceListAsync(Int16 CompanyId, Int32 DocumentId, Int16 UserId);

        public Task<SqlResponse> SaveGLOpeningBalanceAsync(Int16 CompanyId, GLOpeningBalanceViewModel gLOpeningBalanceViewModel, Int16 UserId);

        public Task<IEnumerable<GLOpeningBalanceViewModel>> GenerateGLYearEndProcessAsyn(Int16 CompanyId, Int32 FinYear, Int16 UserId);
    }
}