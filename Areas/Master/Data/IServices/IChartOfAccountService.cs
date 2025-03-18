using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IChartOfAccountService
    {
        public Task<ChartOfAccountViewModelCount> GetChartOfAccountListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<ChartOfAccountViewModel> GetChartOfAccountByIdAsync(short CompanyId, short UserId, short GlId);

        public Task<SqlResponse> SaveChartOfAccountAsync(short CompanyId, short UserId, M_ChartOfAccount M_ChartOfAccount);

        public Task<SqlResponse> DeleteChartOfAccountAsync(short CompanyId, short UserId, short GlId);
    }
}