using AMESWEB.Models;
using AMESWEB.Models.Admin;

namespace AMESWEB.IServices
{
    public interface ILookupService
    {
        public Task<IEnumerable<CompanyViewModel>> GetCompanyLookupAsync(Int16 UserId);

        public Task<IEnumerable<UserLookupModel>> GetUserLookupAsync();

        public Task<IEnumerable<UserGroupLookupModel>> GetUserGroupLookupAsync();

        public Task<IEnumerable<ModuleLookupModel>> GetModuleLookupAsync(bool IsVisible, bool IsMandatory);

        public Task<IEnumerable<TransCategoryLookupModel>> GetModuleTransCategoryLookupAsync(bool IsVisible, bool IsMandatory);

        public Task<IEnumerable<TransactionLookupModel>> GetTransactionLookupAsync(Int16 ModuleId);

        public Task<IEnumerable<AccountSetupCategoryLookupModel>> GetAccountSetupCategoryLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<AccountSetupLookupModel>> GetAccountSetupLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<AccountGroupLookupModel>> GetAccountGroupLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<OrderTypeLookupModel>> GetOrderTypeLookupAsync(Int16 CompanyId, Int16 UserId, Int16 categoryId);

        public Task<IEnumerable<AccountTypeLookupModel>> GetAccountTypeLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BankLookupModel>> GetBankLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BankLookupModel>> GetBankLookup_SuppAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CategoryLookupModel>> GetCategoryLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<ChartOfAccountLookupModel>> GetChartOfAccountLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory1LookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory2LookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory3LookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CountryLookupModel>> GetCountryLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CurrencyLookupModel>> GetCurrencyLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerCodeLookupModel>> GetCustomerCodeLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerGroupCreditLimitLookupModel>> GetCustomerGroupCreditLimitLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount);

        public Task<IEnumerable<DepartmentLookupModel>> GetDepartmentLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<DesignationLookupModel>> GetDesignationLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<EmployeeLookupModel>> GetEmployeeLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GroupCreditLimitLookupModel>> GetGroupCreditLimitLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GstLookupModel>> GetGstLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GstCategoryLookupModel>> GetGstCategoryLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<OrderTypeCategoryLookupModel>> GetOrderTypeCategoryLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<OrderTypeLookupModel>> GetOrderTypeLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PaymentTypeLookupModel>> GetPaymentTypeLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PortLookupModel>> GetPortLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PortRegionLookupModel>> GetPortRegionLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<ProductLookupModel>> GetProductLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<TaskLookupModel>> GetTaskLookupAsync(Int16 CompanyId, Int16 UserId);
        public Task<IEnumerable<TaskLookupModel>> GetTaskByJobOrderLookupAsync(Int16 CompanyId, Int16 UserId, long jobOrderId);

        public Task<IEnumerable<ChargeLookupModel>> GetChargeLookupAsync(Int16 CompanyId, Int16 UserId, int taskId);

        public Task<IEnumerable<ProductLookupModel>> GetProductLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount);

        public Task<IEnumerable<SubCategoryLookupModel>> GetSubCategoryLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount);

        public Task<IEnumerable<UomLookupModel>> GetUomLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount);

        public Task<IEnumerable<BargeLookupModel>> GetBargeLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BargeLookupModel>> GetBargeLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount);

        public Task<IEnumerable<CreditTermLookupModel>> GetCreditTermLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VesselLookupModel>> GetVesselLookupAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VesselLookupModel>> GetVesselLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount);

        public Task<IEnumerable<CustomerAddressLookupModel>> GetCustomerAddressLookup_FinAsync(Int16 CompanyId, Int16 UserId, int CustomerId);

        public Task<IEnumerable<CustomerContactLookupModel>> GetCustomerContactLookup_FinAsync(Int16 CompanyId, Int16 UserId, int CustomerId);

        public Task<IEnumerable<SupplierAddressLookupModel>> GetSupplierAddressLookup_FinAsync(Int16 CompanyId, Int16 UserId, Int16 SupplierId);

        public Task<IEnumerable<SupplierContactLookupModel>> GetSupplierContactLookup_FinAsync(Int16 CompanyId, Int16 UserId, Int16 SupplierId);

        public Task<IEnumerable<YearLookupModel>> GetPeriodCloseYearLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<YearLookupModel>> GetPeriodCloseNextYearLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<DocumentTypeLookupModel>> GetDocumentTypeLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<JobOrderLookupModel>> GetJobOrderLookupAsync(Int16 CompanyId, Int16 UserId);
    }
}