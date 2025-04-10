using AMESWEB.Areas.Project.Models;
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

        #region DebitNote

        #endregion DebitNote

        public Task<SqlResponce> SaveTaskForwardAsync(short CompanyId, short UserId, Int64 JobOrderId, string jobOrderNo, Int64 prevJobOrderId, int taskId, string MultipleId);

        public Task<TaskCountsViewModel> GetTaskJobOrderCountsAsync(short companyId, short userId, string searchString, Int64 jobOrderId);

        public Task<IEnumerable<dynamic>> GetPurchaseJobOrderAsync(short companyId, short userId, Int64 jobOrderId, int taskId);
    }
}