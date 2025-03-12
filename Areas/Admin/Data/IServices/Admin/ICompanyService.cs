using AEMSWEB.Models;
using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Masters
{
    public interface ICompanyService
    {
        public Task<CompanyViewModelCount> GetCompanyListAsync(int pageSize, int pageNumber, string searchString);

        public Task<CompanyViewModel> GetCompanyByIdAsync(short CompanyId);

        public Task<SqlResponse> SaveCompanyAsync(short UserId, AdmCompany m_Company);

        //public Task<SqlResponse> DeleteCompanyAsync(short UserId, CompanyViewModel m_Company);
    }
}