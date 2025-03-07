using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICOACategory1Service
    {
        public Task<COACategoryViewModelCount> GetCOACategory1ListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_COACategory1> GetCOACategory1ByIdAsync(short CompanyId, short UserId, short COACategoryId);

        public Task<SqlResponse> SaveCOACategory1Async(short CompanyId, short UserId, M_COACategory1 m_COACategory1);

        public Task<SqlResponse> DeleteCOACategory1Async(short CompanyId, short UserId, short COACategoryId);
    }
}