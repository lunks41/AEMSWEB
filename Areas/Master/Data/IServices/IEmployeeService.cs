﻿using AEMSWEB.Entities.Masters;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.Areas.Master.Data.IServices
{
    public interface IEmployeeService
    {
        public Task<EmployeeViewModelCount> GetEmployeeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<EmployeeViewModel> GetEmployeeByIdAsync(short CompanyId, short UserId, short EmployeeId);

        public Task<SqlResponse> SaveEmployeeAsync(short CompanyId, short UserId, M_Employee M_Employee);

        public Task<SqlResponse> DeleteEmployeeAsync(short CompanyId, short UserId, short EmployeeId);
    }
}