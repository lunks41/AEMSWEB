﻿using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface ICreditTermService
    {
        public Task<CreditTermViewModelCount> GetCreditTermListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CreditTermViewModel> GetCreditTermByIdAsync(short CompanyId, short UserId, short CreditTermId);

        public Task<SqlResponse> SaveCreditTermAsync(short CompanyId, short UserId, M_CreditTerm m_CreditTerm);

        public Task<SqlResponse> DeleteCreditTermAsync(short CompanyId, short UserId, short CreditTermId);

        public Task<CreditTermDtViewModelCount> GetCreditTermDtListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<CreditTermDtViewModel> GetCreditTermDtByIdAsync(short CompanyId, short UserId, short CreditTermId, short FromDay);

        public Task<SqlResponse> SaveCreditTermDtAsync(short CompanyId, short UserId, M_CreditTermDt m_CreditTermDt);

        public Task<SqlResponse> DeleteCreditTermDtAsync(short CompanyId, short UserId, short CreditTermId, short FromDay);
    }
}