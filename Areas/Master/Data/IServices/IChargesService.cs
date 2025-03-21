using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IChargesService
    {
        public Task<ChargesViewModelCount> GetChargesListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<ChargesViewModel> GetChargesByIdAsync(short CompanyId, short UserId, short ChargeId);

        public Task<SqlResponse> SaveChargesAsync(short CompanyId, short UserId, M_Charges M_Charges);

        public Task<SqlResponse> DeleteChargesAsync(short CompanyId, short UserId, short ChargeId);
    }
}