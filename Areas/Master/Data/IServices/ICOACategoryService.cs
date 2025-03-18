using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICOACategoryService
    {
        public Task<COACategoryViewModelCount> GetCOACategory1ListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_COACategory1> GetCOACategory1ByIdAsync(short CompanyId, short UserId, short COACategoryId);

        public Task<SqlResponse> SaveCOACategory1Async(short CompanyId, short UserId, M_COACategory1 m_COACategory1);

        public Task<SqlResponse> DeleteCOACategory1Async(short CompanyId, short UserId, short COACategoryId);

        public Task<COACategoryViewModelCount> GetCOACategory2ListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_COACategory2> GetCOACategory2ByIdAsync(short CompanyId, short UserId, short COACategoryId);

        public Task<SqlResponse> SaveCOACategory2Async(short CompanyId, short UserId, M_COACategory2 M_COACategory2);

        public Task<SqlResponse> DeleteCOACategory2Async(short CompanyId, short UserId, M_COACategory2 M_COACategory2);

        public Task<COACategoryViewModelCount> GetCOACategory3ListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_COACategory3> GetCOACategory3ByIdAsync(short CompanyId, short UserId, short COACategoryId);

        public Task<SqlResponse> SaveCOACategory3Async(short CompanyId, short UserId, M_COACategory3 M_COACategory3);

        public Task<SqlResponse> DeleteCOACategory3Async(short CompanyId, short UserId, M_COACategory3 M_COACategory3);
    }
}