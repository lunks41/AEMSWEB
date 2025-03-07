using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Data.Services;
using AEMSWEB.IServices;
using AEMSWEB.IServices.Accounts;
using AEMSWEB.IServices.Accounts.AP;
using AEMSWEB.IServices.Accounts.AR;
using AEMSWEB.IServices.Accounts.CB;
using AEMSWEB.IServices.Accounts.GL;
using AEMSWEB.IServices.Admin;
using AEMSWEB.IServices.Masters;
using AEMSWEB.IServices.Setting;
using AEMSWEB.Repository;
using AEMSWEB.Services;
using AEMSWEB.Services.Accounts;
using AEMSWEB.Services.Accounts.AP;
using AEMSWEB.Services.Accounts.AR;
using AEMSWEB.Services.Accounts.CB;
using AEMSWEB.Services.Accounts.GL;
using AEMSWEB.Services.Admin;
using AEMSWEB.Services.Masters;
using AEMSWEB.Services.Setting;

namespace AEMSWEB.Extensions;

public static class InfraServices
{
    public static IServiceCollection RegisterService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<IBaseService, BaseService>();

        #region Master

        services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        services.AddScoped<IAccountGroupService, AccountGroupService>();
        services.AddScoped<IAccountSetupCategoryService, AccountSetupCategoryService>();
        services.AddScoped<IAccountSetupService, AccountSetupService>();
        services.AddScoped<IAccountTypeService, AccountTypeService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IBankAddressService, BankAddressService>();
        services.AddScoped<IBankContactService, BankContactService>();
        services.AddScoped<IBargeService, BargeService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IChartOfAccountService, ChartOfAccountService>();
        services.AddScoped<ICOACategory1Service, COACategory1Service>();
        services.AddScoped<ICOACategory2Service, COACategory2Service>();
        services.AddScoped<ICOACategory3Service, COACategory3Service>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICreditTermService, CreditTermService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<ICustomerGroupCreditLimitService, CustomerGroupCreditLimitService>();
        services.AddScoped<ICustomerAddressService, CustomerAddressService>();
        services.AddScoped<ICustomerContactService, CustomerContactService>();
        services.AddScoped<ICustomerCreditLimitService, CustomerCreditLimitService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDesignationService, DesignationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IGroupCreditLimitService, GroupCreditLimitService>();
        services.AddScoped<IGroupCreditLimit_CustomerService, GroupCreditLimit_CustomerService>();
        services.AddScoped<IGstCategoryService, GstCategoryService>();
        services.AddScoped<IGstService, GstService>();
        services.AddScoped<IOrderTypeCategoryService, OrderTypeCategoryService>();
        services.AddScoped<IOrderTypeService, OrderTypeService>();
        services.AddScoped<IPaymentTypeService, PaymentTypeService>();
        services.AddScoped<IPortRegionService, PortRegionService>();
        services.AddScoped<IPortService, PortService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();
        services.AddScoped<ISupplierAddressService, SupplierAddressService>();
        services.AddScoped<ISupplierContactService, SupplierContactService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ITaxCategoryService, TaxCategoryService>();
        services.AddScoped<ITaxService, TaxService>();
        services.AddScoped<IUomService, UomService>();
        services.AddScoped<IVesselService, VesselService>();
        services.AddScoped<IVoyageService, VoyageService>();

        #endregion Master

        #region Admin

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IUserGroupService, UserGroupService>();
        services.AddScoped<IUserGroupRightsService, UserGroupRightsService>();
        services.AddScoped<IUserRightsService, UserRightsService>();
        services.AddScoped<IDocumentService, DocumentService>();

        #endregion Admin

        #region LookUp

        services.AddScoped<IMasterLookupService, MasterLookupService>();

        #endregion LookUp

        #region Account

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ILogService, LogService>();

        services.AddScoped<IARInvoiceService, ARInvoiceService>();
        services.AddScoped<IARCreditNoteService, ARCreditNoteService>();
        services.AddScoped<IARDebitNoteService, ARDebitNoteService>();
        services.AddScoped<IARReceiptService, ARReceiptService>();
        services.AddScoped<IARTransactionService, ARTransactionService>();
        services.AddScoped<IARAdjustmentService, ARAdjustmentService>();
        services.AddScoped<IARRefundService, ARRefundService>();
        services.AddScoped<IARDocSetOffService, ARDocSetOffService>();

        services.AddScoped<IAPInvoiceService, APInvoiceService>();
        services.AddScoped<IAPCreditNoteService, APCreditNoteService>();
        services.AddScoped<IAPDebitNoteService, APDebitNoteService>();
        services.AddScoped<IAPAdjustmentService, APAdjustmentService>();
        services.AddScoped<IAPTransactionService, APTransactionService>();
        services.AddScoped<IAPPaymentService, APPaymentService>();
        services.AddScoped<IAPRefundService, APRefundService>();
        services.AddScoped<IAPDocSetOffService, APDocSetOffService>();

        services.AddScoped<ICBBankTransferService, CBBankTransferService>();
        services.AddScoped<ICBGenPaymentService, CBGenPaymentService>();
        services.AddScoped<ICBGenReceiptService, CBGenReceiptService>();
        services.AddScoped<ICBPettyCashService, CBPettyCashService>();

        services.AddScoped<IGLPeriodCloseService, GLPeriodCloseService>();
        services.AddScoped<IGLOpeningBalanceService, GLOpeningBalanceService>();
        services.AddScoped<IGLJournalService, GLJournalService>();
        services.AddScoped<IGLContraService, GLContraService>();

        #endregion Account

        #region Setting

        services.AddScoped<IDecimalSettingService, DecimalSettingServices>();
        services.AddScoped<IFinanceSettingService, FinanceSettingServices>();
        services.AddScoped<IUserSettingService, UserSettingServices>();
        services.AddScoped<INumberFormatServices, NumberFormatServices>();
        services.AddScoped<IUserGridServices, UserGridServices>();
        services.AddScoped<IBaseSettingsService, BaseSettingsServices>();
        services.AddScoped<IMandatoryFieldsServices, MandatoryFieldsServices>();
        services.AddScoped<IVisibleFieldsServices, VisibleFieldsServices>();
        services.AddScoped<IDynamicLookupService, DynamicLookupServices>();
        services.AddScoped<IDocSeqNoService, DocSeqNoServices>();

        #endregion Setting

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}