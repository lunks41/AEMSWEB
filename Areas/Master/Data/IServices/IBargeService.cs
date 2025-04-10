using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IBargeService
    {
        public Task<BargeViewModelCount> GetBargeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<BargeViewModel> GetBargeByIdAsync(short CompanyId, short UserId, short bargeId);

        public Task<SqlResponce> SaveBargeAsync(short CompanyId, short UserId, M_Barge M_Barge);

        public Task<SqlResponce> DeleteBargeAsync(short CompanyId, short UserId, short bargeId);
    }
}