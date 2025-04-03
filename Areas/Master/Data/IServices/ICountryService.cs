using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ICountryService
    {
        public Task<CountryViewModelCount> GetCountryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CountryViewModel> GetCountryByIdAsync(short CompanyId, short UserId, short CountryId);

        public Task<SqlResponse> SaveCountryAsync(short CompanyId, short UserId, M_Country M_Country);

        public Task<SqlResponse> DeleteCountryAsync(short CompanyId, short UserId, short CountryId);
    }
}