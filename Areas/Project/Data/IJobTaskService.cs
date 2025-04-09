using AMESWEB.Areas.Project.Models;
using AMESWEB.Entities.Project;
using AMESWEB.Models;

namespace AMESWEB.Areas.Project.Data.IServices
{
    public interface IJobTaskService
    {
        #region Port Expenses

        public Task<PortExpensesViewModelCount> GetPortExpensesListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<PortExpensesViewModel> GetPortExpensesByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId);

        public Task<SqlResponse> SavePortExpensesAsync(short CompanyId, short UserId, Ser_PortExpenses ser_PortExpenses);

        public Task<SqlResponse> DeletePortExpensesAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId);

        #endregion Port Expenses

        #region Launch Services

        public Task<LaunchServicesViewModelCount> GetLaunchServicesListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<LaunchServicesViewModel> GetLaunchServicesByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId);

        public Task<SqlResponse> SaveLaunchServicesAsync(short CompanyId, short UserId, Ser_LaunchServices ser_LaunchServices);

        public Task<SqlResponse> DeleteLaunchServicesAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId);

        #endregion Launch Services
    }
}