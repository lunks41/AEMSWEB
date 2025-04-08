using AMESWEB.Areas.Project.Models;
using AMESWEB.Entities.Project;
using AMESWEB.Models;

namespace AMESWEB.Areas.Project.Data.IServices
{
    public interface ITariffService
    {
        public Task<TariffViewModelCount> GetTariffPortExpensesListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffLaunchServicesListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffEquipmentsUsedListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffCrewSignOnListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffCrewSignOffListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffCrewMiscellaneousListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffMedicalAssistanceListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffConsignmentImportListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffConsignmentExportListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffThirdPartySupplyListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffFreshWaterSupplyListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffTechniciansSurveyorsListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffLandingItemsListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffOtherServiceListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TariffViewModelCount> GetTariffAgencyRemunerationListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId);

        public Task<TaskCountsViewModel> GetTariffStatusCountsAsync(short companyId, short userId, string searchString, int customerId, int portId);

        public Task<TariffViewModel> GetTariffByIdAsync(short CompanyId, short UserId, int tariffId, int taskId, int customerId, int portId, int chargeId);

        public Task<SqlResponse> SaveTariffAsync(short CompanyId, short UserId, Ser_Tariff ser_Tariff);

        public Task<SqlResponse> DeleteTariffAsync(short CompanyId, short UserId, int tariffId, int taskId, int customerId, int portId, int chargeId);

        public Task<SqlResponse> CopyCustomerTariffAsync(short CompanyId, short UserId, int fromCustomerId, int fromPortId, int fromTaskId, int toCustomerId, int toPortId, int toTaskId, bool overwriteExisting, bool deleteExisting);

        public Task<SqlResponse> CopyCompanyToCustomerTariffAsync(short CompanyId, short UserId, int fromCompanyId, int fromCustomerId, int fromPortId, int fromTaskId, int toCompanyId, int toCustomerId, int toPortId, int toTaskId, bool overwriteExisting, bool deleteExisting);
    }
}