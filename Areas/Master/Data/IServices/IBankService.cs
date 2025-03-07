using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IBankService
    {
        public Task<BankViewModelCount> GetBankListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<BankViewModel> GetBankByIdAsync(short CompanyId, short UserId, short BankId);

        public Task<BankViewModel> GetBankAsync(short CompanyId, short UserId, int BankId, string BankCode, string BankName);

        public Task<SqlResponse> SaveBankAsync(short CompanyId, short UserId, M_Bank M_Bank);

        public Task<SqlResponse> DeleteBankAsync(short CompanyId, short UserId, short BankId);
    }
}