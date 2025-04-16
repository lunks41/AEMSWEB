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

        public Task<SqlResponce> SavePortExpensesAsync(short CompanyId, short UserId, Ser_PortExpenses ser_PortExpenses);

        public Task<SqlResponce> DeletePortExpensesAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId);

        #endregion Port Expenses

        #region Launch Services

        public Task<LaunchServicesViewModelCount> GetLaunchServicesListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<LaunchServicesViewModel> GetLaunchServicesByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId);

        public Task<SqlResponce> SaveLaunchServicesAsync(short CompanyId, short UserId, Ser_LaunchServices ser_LaunchServices);

        public Task<SqlResponce> DeleteLaunchServicesAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 LaunchServiceId);

        #endregion Launch Services

        #region Equipment Used

        public Task<EquipmentsUsedViewModelCount> GetEquipmentsUsedListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<EquipmentsUsedViewModel> GetEquipmentsUsedByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 EquipmentsUsedId);

        public Task<SqlResponce> SaveEquipmentsUsedAsync(short CompanyId, short UserId, Ser_EquipmentsUsed ser_EquipmentsUsed);

        public Task<SqlResponce> DeleteEquipmentsUsedAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 equipmentsUsedId);

        #endregion Equipment Used

        #region Crew Sign On

        public Task<CrewSignOnViewModelCount> GetCrewSignOnListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<CrewSignOnViewModel> GetCrewSignOnByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 CrewSignOnId);

        public Task<SqlResponce> SaveCrewSignOnAsync(short CompanyId, short UserId, Ser_CrewSignOn ser_CrewSignOn);

        public Task<SqlResponce> DeleteCrewSignOnAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 crewSignOnId);

        #endregion Crew Sign On

        #region Crew Sign Off

        public Task<CrewSignOffViewModelCount> GetCrewSignOffListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<CrewSignOffViewModel> GetCrewSignOffByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 CrewSignOffId);

        public Task<SqlResponce> SaveCrewSignOffAsync(short CompanyId, short UserId, Ser_CrewSignOff ser_CrewSignOff);

        public Task<SqlResponce> DeleteCrewSignOffAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 crewSignOffId);

        #endregion Crew Sign Off

        #region Crew Miscellaneous

        public Task<CrewMiscellaneousViewModelCount> GetCrewMiscellaneousListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<CrewMiscellaneousViewModel> GetCrewMiscellaneousByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 CrewMiscellaneousId);

        public Task<SqlResponce> SaveCrewMiscellaneousAsync(short CompanyId, short UserId, Ser_CrewMiscellaneous ser_CrewMiscellaneous);

        public Task<SqlResponce> DeleteCrewMiscellaneousAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 crewMiscellaneousId);

        #endregion Crew Miscellaneous

        #region Medical Assistance

        public Task<MedicalAssistanceViewModelCount> GetMedicalAssistanceListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<MedicalAssistanceViewModel> GetMedicalAssistanceByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 MedicalAssistanceId);

        public Task<SqlResponce> SaveMedicalAssistanceAsync(short CompanyId, short UserId, Ser_MedicalAssistance ser_MedicalAssistance);

        public Task<SqlResponce> DeleteMedicalAssistanceAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 medicalAssistanceId);

        #endregion Medical Assistance

        #region Consignment Import

        public Task<ConsignmentImportViewModelCount> GetConsignmentImportListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<ConsignmentImportViewModel> GetConsignmentImportByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 ConsignmentImportId);

        public Task<SqlResponce> SaveConsignmentImportAsync(short CompanyId, short UserId, Ser_ConsignmentImport ser_ConsignmentImport);

        public Task<SqlResponce> DeleteConsignmentImportAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 consignmentImportId);

        #endregion Consignment Import

        #region Consignment Export

        public Task<ConsignmentExportViewModelCount> GetConsignmentExportListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<ConsignmentExportViewModel> GetConsignmentExportByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 ConsignmentExportId);

        public Task<SqlResponce> SaveConsignmentExportAsync(short CompanyId, short UserId, Ser_ConsignmentExport ser_ConsignmentExport);

        public Task<SqlResponce> DeleteConsignmentExportAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 consignmentExportId);

        #endregion Consignment Export

        #region Third Party

        public Task<ThirdPartyViewModelCount> GetThirdPartyListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<ThirdPartyViewModel> GetThirdPartyByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 ThirdPartyId);

        public Task<SqlResponce> SaveThirdPartyAsync(short CompanyId, short UserId, Ser_ThirdParty ser_ThirdParty);

        public Task<SqlResponce> DeleteThirdPartyAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 thirdPartyId);

        #endregion Third Party

        #region Fresh Water

        public Task<FreshWaterViewModelCount> GetFreshWaterListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<FreshWaterViewModel> GetFreshWaterByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 FreshWaterId);

        public Task<SqlResponce> SaveFreshWaterAsync(short CompanyId, short UserId, Ser_FreshWater ser_FreshWater);

        public Task<SqlResponce> DeleteFreshWaterAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 freshWaterId);

        #endregion Fresh Water

        #region Technicians Surveyors

        public Task<TechniciansSurveyorsViewModelCount> GetTechniciansSurveyorsListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<TechniciansSurveyorsViewModel> GetTechniciansSurveyorsByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 TechniciansSurveyorsId);

        public Task<SqlResponce> SaveTechniciansSurveyorsAsync(short CompanyId, short UserId, Ser_TechniciansSurveyors ser_TechniciansSurveyors);

        public Task<SqlResponce> DeleteTechniciansSurveyorsAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 techniciansSurveyorsId);

        #endregion Technicians Surveyors

        #region Landing Items

        public Task<LandingItemsViewModelCount> GetLandingItemsListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<LandingItemsViewModel> GetLandingItemsByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 LandingItemId);

        public Task<SqlResponce> SaveLandingItemsAsync(short CompanyId, short UserId, Ser_LandingItems ser_LandingItems);

        public Task<SqlResponce> DeleteLandingItemsAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 landingItemId);

        #endregion Landing Items

        #region Other Service

        public Task<OtherServiceViewModelCount> GetOtherServiceListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<OtherServiceViewModel> GetOtherServiceByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 OtherServiceId);

        public Task<SqlResponce> SaveOtherServiceAsync(short CompanyId, short UserId, Ser_OtherService ser_OtherService);

        public Task<SqlResponce> DeleteOtherServiceAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 otherServiceId);

        #endregion Other Service

        #region Agency Remuneration

        public Task<AgencyRemunerationViewModelCount> GetAgencyRemunerationListAsync(short CompanyId, short UserId, Int64 JobOrderId);

        public Task<AgencyRemunerationViewModel> GetAgencyRemunerationByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 AgencyRemunerationId);

        public Task<SqlResponce> SaveAgencyRemunerationAsync(short CompanyId, short UserId, Ser_AgencyRemuneration ser_AgencyRemuneration);

        public Task<SqlResponce> DeleteAgencyRemunerationAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 agencyRemunerationId);

        #endregion Agency Remuneration
    }
}