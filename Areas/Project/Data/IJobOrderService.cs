using AEMSWEB.Areas.Project.Models;

namespace AEMSWEB.Areas.Project.Data.IServices
{
    public interface IJobOrderService
    {
        public Task<JobOrderViewModelCount> GetJobOrderListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, DateTime? fromDate, DateTime? toDate, string status);

        Task<StatusCountsViewModel> GetJobStatusCountsAsync(short companyId, short userId, string searchString, int customerId, DateTime? fromDate, DateTime? toDate);

        public Task<JobOrderViewModelCount> GetJobOrderListAsyncV1(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, string customerId, DateTime? fromDate, DateTime? toDate, string status);

        public Task<JobOrderHdViewModel> GetJobOrderByIdAsync(short CompanyId, short UserId, int JobOrderId, string JobOrderCode, string JobOrderName);
    }
}