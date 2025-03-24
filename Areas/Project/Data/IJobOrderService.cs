using AEMSWEB.Areas.Project.Models;

namespace AEMSWEB.Areas.Project.Data.IServices
{
    public interface IJobOrderService
    {
        public Task<JobOrderViewModelCount> GetJobOrderListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString);

        public Task<JobOrderHdViewModel> GetJobOrderByIdAsync(short CompanyId, short UserId, int JobOrderId, string JobOrderCode, string JobOrderName);

    }
}