﻿using AMESWEB.Entities.Masters;
using AMESWEB.Models;
using AMESWEB.Models.Masters;

namespace AMESWEB.Areas.Master.Data.IServices
{
    public interface IAccountTypeService
    {
        public Task<AccountTypeViewModelCount> GetAccountTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<AccountTypeViewModel> GetAccountTypeByIdAsync(short CompanyId, short UserId, short accTypeId);

        public Task<SqlResponse> SaveAccountTypeAsync(short CompanyId, short UserId, M_AccountType M_AccountType);

        public Task<SqlResponse> DeleteAccountTypeAsync(short CompanyId, short UserId, short accTypeId);
    }
}