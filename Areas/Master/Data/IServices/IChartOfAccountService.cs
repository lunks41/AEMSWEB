using AMESWEB.Areas.Master.Models;
using AMESWEB.Entities.Masters;
using AMESWEB.Models;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IChartOfAccountService
    {
        public Task<ChartOfAccountViewModelCount> GetChartOfAccountListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<ChartOfAccountViewModel> GetChartOfAccountByIdAsync(short CompanyId, short UserId, short GlId);

        public Task<SqlResponce> SaveChartOfAccountAsync(short CompanyId, short UserId, M_ChartOfAccount M_ChartOfAccount);

        public Task<SqlResponce> DeleteChartOfAccountAsync(short CompanyId, short UserId, short GlId);
    }
}