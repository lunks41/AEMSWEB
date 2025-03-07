using AEMSWEB.Models.Admin;
using AEMSWEB.Models.Masters;

namespace AEMSWEB.IServices
{
    public interface IMasterLookupService
    {
        public Task<IEnumerable<CompanyViewModel>> GetCompanyLookupListAsync(Int16 UserId);

        public Task<IEnumerable<AccountSetupCategoryLookupModel>> GetAccountSetupCategoryLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<AccountSetupLookupModel>> GetAccountSetupLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<AccountGroupLookupModel>> GetAccountGroupLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<AccountTypeLookupModel>> GetAccountTypeLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BankLookupModel>> GetBankLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BankLookupModel>> GetBankLookup_SuppListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CategoryLookupModel>> GetCategoryLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<ChartOfAccountLookupModel>> GetChartOfAccountLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory1LookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory2LookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<COACategoryLookupModel>> GetCOACategory3LookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CountryLookupModel>> GetCountryLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CurrencyLookupModel>> GetCurrencyLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerGroupCreditLimitLookupModel>> GetCustomerGroupCreditLimitLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupListAsync_V1(Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<DepartmentLookupModel>> GetDepartmentLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<DesignationLookupModel>> GetDesignationLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<EmployeeLookupModel>> GetEmployeeLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GroupCreditLimitLookupModel>> GetGroupCreditLimitLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GstLookupModel>> GetGstLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<GstCategoryLookupModel>> GetGstCategoryLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<OrderTypeCategoryLookupModel>> GetOrderTypeCategoryLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<OrderTypeLookupModel>> GetOrderTypeLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PaymentTypeLookupModel>> GetPaymentTypeLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PortLookupModel>> GetPortLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<PortRegionLookupModel>> GetPortRegionLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<ProductLookupModel>> GetProductLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<ProductLookupModel>> GetProductLookupListAsync_V1(Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<SubCategoryLookupModel>> GetSubCategoryLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupListAsync_V1(Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<UomLookupModel>> GetUomLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupListAsync_V1(Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<BargeLookupModel>> GetBargeLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<BargeLookupModel>> GetBargeLookupListAsync_V1(Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<CreditTermsLookupModel>> GetCreditTermsLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<TaxLookupModel>> GetTaxLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<TaxCategoryLookupModel>> GetTaxCategoryLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VesselLookupModel>> GetVesselLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<VesselLookupModel>> GetVesselLookupListAsync_V1(Int16 CompanyId, string searchString, Int16 RecordCount, Int16 UserId);

        public Task<IEnumerable<UserGroupLookupModel>> GetUserGroupLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<UserLookupModel>> GetUserLookupListAsync(Int16 CompanyId, Int16 UserId);

        public Task<IEnumerable<CustomerAddressLookupModel>> GetCustomerAddressLookup_FinListAsync(Int16 CompanyId, Int16 UserId, Int16 CustomerId);

        public Task<IEnumerable<CustomerContactLookupModel>> GetCustomerContactLookup_FinListAsync(Int16 CompanyId, Int16 UserId, Int16 CustomerId);

        public Task<IEnumerable<SupplierAddressLookupModel>> GetSupplierAddressLookup_FinListAsync(Int16 CompanyId, Int16 UserId, Int16 SupplierId);

        public Task<IEnumerable<SupplierContactLookupModel>> GetSupplierContactLookup_FinListAsync(Int16 CompanyId, Int16 UserId, Int16 SupplierId);

        public Task<IEnumerable<ModuleLookupModel>> GetModuleLookupAsync(Int16 CompanyId, Int16 UserId, bool IsVisible, bool IsMandatory);

        public Task<IEnumerable<TransactionLookupModel>> GetTransactionLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<YearLookupModel>> GetPeriodCloseYearLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<YearLookupModel>> GetPeriodCloseNextYearLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId);

        public Task<IEnumerable<DocumentTypeLookupModel>> GetDocumentTypeLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId);
    }
}