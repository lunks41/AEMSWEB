using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IBargeService
    {
        public Task<BargeViewModelCount> GetBargeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<BargeViewModel> GetBargeByIdAsync(short CompanyId, short UserId, short bargeId);

        public Task<SqlResponse> SaveBargeAsync(short CompanyId, short UserId, M_Barge M_Barge);

        public Task<SqlResponse> DeleteBargeAsync(short CompanyId, short UserId, short bargeId);
    }
}