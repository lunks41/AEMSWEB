using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IChargesService
    {
        public Task<ChargesViewModelCount> GetChargesListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<ChargesViewModel> GetChargesByIdAsync(short CompanyId, short UserId, short ChargeId);

        public Task<SqlResponce> SaveChargesAsync(short CompanyId, short UserId, M_Charges M_Charges);

        public Task<SqlResponce> DeleteChargesAsync(short CompanyId, short UserId, short ChargeId);
    }
}