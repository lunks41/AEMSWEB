using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICountryService
    {
        public Task<CountryViewModelCount> GetCountryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<M_Country> GetCountryByIdAsync(short CompanyId, short UserId, short CountryId);

        public Task<SqlResponse> SaveCountryAsync(short CompanyId, short UserId, M_Country M_Country);

        public Task<SqlResponse> DeleteCountryAsync(short CompanyId, short UserId, M_Country M_Country);
    }
}