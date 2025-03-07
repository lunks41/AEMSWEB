﻿using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface ICurrencyService
    {
        public Task<CurrencyViewModelCount> GetCurrencyListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<CurrencyViewModel> GetCurrencyByIdAsync(short CompanyId, short UserId, int CurrencyId);

        public Task<SqlResponse> SaveCurrencyAsync(short CompanyId, short UserId, M_Currency M_Currency);

        public Task<SqlResponse> DeleteCurrencyAsync(short CompanyId, short UserId, int CurrencyId);

        public Task<CurrencyDtViewModelCount> GetCurrencyDtListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<CurrencyDtViewModel> GetCurrencyDtByIdAsync(short CompanyId, short UserId, int CurrencyId, DateTime ValidFrom);

        public Task<SqlResponse> SaveCurrencyDtAsync(short CompanyId, short UserId, M_CurrencyDt M_CurrencyDt);

        public Task<SqlResponse> DeleteCurrencyDtAsync(short CompanyId, short UserId, CurrencyDtViewModel M_CurrencyDt);

        public Task<CurrencyLocalDtViewModelCount> GetCurrencyLocalDtListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString);

        public Task<CurrencyLocalDtViewModel> GetCurrencyLocalDtByIdAsync(short CompanyId, short UserId, int CurrencyId, DateTime ValidFrom);

        public Task<SqlResponse> SaveCurrencyLocalDtAsync(short CompanyId, short UserId, M_CurrencyLocalDt M_CurrencyLocalDt);

        public Task<SqlResponse> DeleteCurrencyLocalDtAsync(short CompanyId, short UserId, CurrencyLocalDtViewModel currencyLocalDtViewModel);
    }
}