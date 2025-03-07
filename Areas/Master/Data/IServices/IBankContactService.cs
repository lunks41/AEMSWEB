using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IBankContactService
    {
        public Task<IEnumerable<BankContactViewModel>> GetBankContactByBankIdAsync(short CompanyId, short UserId, int BankId);

        public Task<BankContactViewModel> GetBankContactByIdAsync(short CompanyId, short UserId, int BankId, short ContactId);

        public Task<SqlResponse> SaveBankContactAsync(short CompanyId, short UserId, M_BankContact m_BankContact);

        public Task<SqlResponse> DeleteBankContactAsync(short CompanyId, short UserId, int BankId, short ContactId);
    }
}