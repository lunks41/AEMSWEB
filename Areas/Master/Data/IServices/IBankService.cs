using AMESWEB.Areas.Master.Models;
using AMESWEB.Entities.Masters;
using AMESWEB.Models;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IBankService
    {
        public Task<BankViewModelCount> GetBankListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<BankViewModel> GetBankByIdAsync(short CompanyId, short UserId, int BankId, string BankCode, string BankName);

        public Task<SqlResponse> SaveBankAsync(short CompanyId, short UserId, M_Bank M_Bank);

        public Task<SqlResponse> DeleteBankAsync(short CompanyId, short UserId, short BankId);

        public Task<IEnumerable<BankContactViewModel>> GetBankContactByBankIdAsync(short CompanyId, short UserId, int BankId);

        public Task<BankContactViewModel> GetBankContactByIdAsync(short CompanyId, short UserId, int BankId, short ContactId);

        public Task<SqlResponse> SaveBankContactAsync(short CompanyId, short UserId, M_BankContact m_BankContact);

        public Task<SqlResponse> DeleteBankContactAsync(short CompanyId, short UserId, int BankId, short ContactId);

        public Task<IEnumerable<BankAddressViewModel>> GetBankAddressByBankIdAsync(short CompanyId, short UserId, int BankId);

        public Task<BankAddressViewModel> GetBankAddressByIdAsync(short CompanyId, short UserId, int BankId, short AddressId);

        public Task<SqlResponse> SaveBankAddressAsync(short CompanyId, short UserId, M_BankAddress m_BankAddress);

        public Task<SqlResponse> DeleteBankAddressAsync(short CompanyId, short UserId, int BankId, short AddressId);
    }
}