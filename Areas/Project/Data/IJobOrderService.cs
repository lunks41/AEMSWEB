using AMESWEB.Areas.Project.Models;
using AMESWEB.Entities.Project;
using AMESWEB.Models;

namespace AMESWEB.Areas.Project.Data.IServices
{
    public interface IJobOrderService
    {
        #region Job Order

        public Task<JobOrderViewModelCount> GetJobOrderListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, DateTime? fromDate, DateTime? toDate, string status);

        Task<StatusCountsViewModel> GetJobStatusCountsAsync(short companyId, short userId, string searchString, int customerId, DateTime? fromDate, DateTime? toDate);

        public Task<JobOrderHdViewModel> GetJobOrderByIdAsync(short CompanyId, short UserId, Int64 JobOrderId);

        #endregion Job Order

        #region Task

        #region Port Expenses

        public Task<PortExpensesViewModelCount> GetPortExpensesListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<PortExpensesViewModel> GetPortExpensesByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId);

        public Task<SqlResponse> SavePortExpensesAsync(short CompanyId, short UserId, Ser_PortExpenses ser_PortExpenses);

        public Task<SqlResponse> DeletePortExpensesAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId);

        #endregion Port Expenses

        #region Launch Services

        #endregion Launch Services

        #region Equipment Used

        #endregion Equipment Used

        #region Crew Sign On

        #endregion Crew Sign On

        #region Crew Sign Off

        #endregion Crew Sign Off

        #region Crew Miscellaneous

        #endregion Crew Miscellaneous

        #region Medical Assistance

        #endregion Medical Assistance

        #region Consignment Import

        #endregion Consignment Import

        #region Consignment Export

        #endregion Consignment Export

        #region Third Party Supply

        #endregion Third Party Supply

        #region Fresh Water Supply

        #endregion Fresh Water Supply

        #region Technicians Surveyors

        #endregion Technicians Surveyors

        #region Landing Items

        #endregion Landing Items

        #region Other Service

        #endregion Other Service

        #region Agency Remuneration

        #endregion Agency Remuneration

        #endregion Task

        #region DebitNote

        #endregion DebitNote

        public Task<SqlResponse> SaveTaskForwardAsync(short CompanyId, short UserId, Int64 JobOrderId, string jobOrderNo, Int64 prevJobOrderId, int taskId, string MultipleId);

        public Task<TaskCountsViewModel> GetTaskJobOrderCountsAsync(short companyId, short userId, string searchString, Int64 jobOrderId);

        public Task<IEnumerable<dynamic>> GetPurchaseJobOrderAsync(short companyId, short userId, Int64 jobOrderId,int taskId);
    }
}