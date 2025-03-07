using AEMSWEB.Models.Admin;

namespace AEMSWEB.IServices.Masters
{
    public interface ICompanyService
    {
        public Task<IEnumerable<CompanyViewModel>> GetUserCompanyListAsync(Int16 UserId);
    }
}