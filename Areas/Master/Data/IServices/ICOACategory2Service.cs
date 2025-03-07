using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICOACategory2Service
    {
        public Task<COACategoryViewModelCount> GetCOACategory2ListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_COACategory2> GetCOACategory2ByIdAsync(short CompanyId, short UserId, short COACategoryId);

        public Task<SqlResponse> SaveCOACategory2Async(short CompanyId, short UserId, M_COACategory2 M_COACategory2);

        public Task<SqlResponse> DeleteCOACategory2Async(short CompanyId, short UserId, M_COACategory2 M_COACategory2);
    }
}