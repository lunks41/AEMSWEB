using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICOACategory3Service
    {
        public Task<COACategoryViewModelCount> GetCOACategory3ListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_COACategory3> GetCOACategory3ByIdAsync(short CompanyId, short UserId, short COACategoryId);

        public Task<SqlResponse> SaveCOACategory3Async(short CompanyId, short UserId, M_COACategory3 M_COACategory3);

        public Task<SqlResponse> DeleteCOACategory3Async(short CompanyId, short UserId, M_COACategory3 M_COACategory3);
    }
}