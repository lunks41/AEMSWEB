using AEMSWEB.Areas.Project.Models;

namespace AEMSWEB.Areas.Project.Data.IServices
{
    public interface ITariffService
    {
        public Task<TariffViewModelCount> GetTariffFreshWaterListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);
        public Task<TariffViewModelCount> GetTariffListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, DateTime? fromDate, DateTime? toDate, string status);

        Task<StatusCountsViewModel> GetJobStatusCountsAsync(short companyId, short userId, string searchString, int customerId, DateTime? fromDate, DateTime? toDate);

        public Task<TariffViewModelCount> GetTariffListAsyncV1(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, string customerId, DateTime? fromDate, DateTime? toDate, string status);

        public Task<TariffViewModel> GetTariffByIdAsync(short CompanyId, short UserId, int TariffId, string TariffCode, string TariffName);
    }
}