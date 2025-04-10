using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ICOACategoryService
    {
        public Task<COACategoryViewModelCount> GetCOACategory1ListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<COACategoryViewModel> GetCOACategory1ByIdAsync(short CompanyId, short UserId, short coaCategoryId);

        public Task<SqlResponce> SaveCOACategory1Async(short CompanyId, short UserId, M_COACategory1 m_COACategory1);

        public Task<SqlResponce> DeleteCOACategory1Async(short CompanyId, short UserId, short coaCategoryId);

        public Task<COACategoryViewModelCount> GetCOACategory2ListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<COACategoryViewModel> GetCOACategory2ByIdAsync(short CompanyId, short UserId, short coaCategoryId);

        public Task<SqlResponce> SaveCOACategory2Async(short CompanyId, short UserId, M_COACategory2 M_COACategory2);

        public Task<SqlResponce> DeleteCOACategory2Async(short CompanyId, short UserId, short coaCategoryId);

        public Task<COACategoryViewModelCount> GetCOACategory3ListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<COACategoryViewModel> GetCOACategory3ByIdAsync(short CompanyId, short UserId, short coaCategoryId);

        public Task<SqlResponce> SaveCOACategory3Async(short CompanyId, short UserId, M_COACategory3 M_COACategory3);

        public Task<SqlResponce> DeleteCOACategory3Async(short CompanyId, short UserId, short coaCategoryId);
    }
}