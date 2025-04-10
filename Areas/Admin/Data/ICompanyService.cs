using AMESWEB.Models;
using AMESWEB.Models.Admin;

namespace AMESWEB.Areas.Admin.Data
{
    public interface ICompanyService
    {
        public Task<CompanyViewModelCount> GetCompanyListAsync(int pageSize, int pageNumber, string searchString);

        public Task<CompanyViewModel> GetCompanyByIdAsync(short CompanyId);

        public Task<SqlResponce> SaveCompanyAsync(short UserId, AdmCompany m_Company);

        //public Task<SqlResponce> DeleteCompanyAsync(short UserId, CompanyViewModel m_Company);
    }
}