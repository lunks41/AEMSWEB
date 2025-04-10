using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ICurrencyService
    {
        public Task<CurrencyViewModelCount> GetCurrencyListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CurrencyViewModel> GetCurrencyByIdAsync(short CompanyId, short UserId, int CurrencyId);

        public Task<SqlResponce> SaveCurrencyAsync(short CompanyId, short UserId, M_Currency M_Currency);

        public Task<SqlResponce> DeleteCurrencyAsync(short CompanyId, short UserId, short CurrencyId);

        public Task<CurrencyDtViewModelCount> GetCurrencyDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CurrencyDtViewModel> GetCurrencyDtByIdAsync(short CompanyId, short UserId, int CurrencyId, DateTime ValidFrom);

        public Task<SqlResponce> SaveCurrencyDtAsync(short CompanyId, short UserId, M_CurrencyDt M_CurrencyDt);

        public Task<SqlResponce> DeleteCurrencyDtAsync(short CompanyId, short UserId, CurrencyDtViewModel M_CurrencyDt);

        public Task<CurrencyLocalDtViewModelCount> GetCurrencyLocalDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CurrencyLocalDtViewModel> GetCurrencyLocalDtByIdAsync(short CompanyId, short UserId, int CurrencyId, DateTime ValidFrom);

        public Task<SqlResponce> SaveCurrencyLocalDtAsync(short CompanyId, short UserId, M_CurrencyLocalDt M_CurrencyLocalDt);

        public Task<SqlResponce> DeleteCurrencyLocalDtAsync(short CompanyId, short UserId, CurrencyLocalDtViewModel currencyLocalDtViewModel);
    }
}