using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IBankAddressService
    {
        public Task<IEnumerable<BankAddressViewModel>> GetBankAddressByBankIdAsync(short CompanyId, short UserId, int BankId);

        public Task<BankAddressViewModel> GetBankAddressByIdAsync(short CompanyId, short UserId, int BankId, short AddressId);

        public Task<SqlResponse> SaveBankAddressAsync(short CompanyId, short UserId, M_BankAddress m_BankAddress);

        public Task<SqlResponse> DeleteBankAddressAsync(short CompanyId, short UserId, int BankId, short AddressId);
    }
}