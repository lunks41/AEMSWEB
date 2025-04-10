using AMESWEB.Areas.Master.Models;
using AMESWEB.Entities.Masters;
using AMESWEB.Models;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IBankService
    {
        #region Bank

        public Task<BankViewModelCount> GetBankListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<BankViewModel> GetBankByIdAsync(short CompanyId, short UserId, int BankId, string BankCode, string BankName);

        public Task<SqlResponce> SaveBankAsync(short CompanyId, short UserId, M_Bank M_Bank);

        public Task<SqlResponce> DeleteBankAsync(short CompanyId, short UserId, int BankId);

        #endregion Bank

        #region Bank Address

        public Task<IEnumerable<BankAddressViewModel>> GetBankAddressByBankIdAsync(short CompanyId, short UserId, int BankId);

        public Task<BankAddressViewModel> GetBankAddressByIdAsync(short CompanyId, short UserId, int BankId, short AddressId);

        public Task<SqlResponce> SaveBankAddressAsync(short CompanyId, short UserId, M_BankAddress m_BankAddress);

        public Task<SqlResponce> DeleteBankAddressAsync(short CompanyId, short UserId, int BankId, short AddressId);

        #endregion Bank Address

        #region Bank Contact

        public Task<IEnumerable<BankContactViewModel>> GetBankContactByBankIdAsync(short CompanyId, short UserId, int BankId);

        public Task<BankContactViewModel> GetBankContactByIdAsync(short CompanyId, short UserId, int BankId, short ContactId);

        public Task<SqlResponce> SaveBankContactAsync(short CompanyId, short UserId, M_BankContact m_BankContact);

        public Task<SqlResponce> DeleteBankContactAsync(short CompanyId, short UserId, int BankId, short ContactId);

        #endregion Bank Contact
    }
}