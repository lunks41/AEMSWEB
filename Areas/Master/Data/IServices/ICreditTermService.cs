using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICreditTermService
    {
        public Task<CreditTermViewModelCount> GetCreditTermListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<M_CreditTerm> GetCreditTermByIdAsync(short CompanyId, short UserId, short CreditTermId);

        public Task<SqlResponse> SaveCreditTermAsync(short CompanyId, short UserId, M_CreditTerm m_CreditTerm);

        public Task<SqlResponse> DeleteCreditTermAsync(short CompanyId, short UserId, M_CreditTerm m_CreditTerm);

        public Task<CreditTermDtViewModelCount> GetCreditTermDtListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<CreditTermDtViewModel> GetCreditTermDtByIdAsync(short CompanyId, short UserId, short CreditTermId, short FromDay);

        public Task<SqlResponse> SaveCreditTermDtAsync(short CompanyId, short UserId, M_CreditTermDt m_CreditTermDt);

        public Task<SqlResponse> DeleteCreditTermDtAsync(short CompanyId, short UserId, short CreditTermId, short FromDay);
    }
}