using AEMSWEB.Areas.Account.Data.IServices;
using AEMSWEB.Areas.Account.Data.IServices.AP;
using AEMSWEB.Areas.Account.Data.IServices.AR;
using AEMSWEB.Areas.Account.Data.IServices.CB;
using AEMSWEB.Areas.Account.Data.IServices.GL;
using AEMSWEB.Areas.Account.Data.Services;
using AEMSWEB.Areas.Account.Data.Services.AP;
using AEMSWEB.Areas.Account.Data.Services.AR;
using AEMSWEB.Areas.Account.Data.Services.CB;
using AEMSWEB.Areas.Account.Data.Services.GL;
using AEMSWEB.Areas.Admin.Data;
using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Data.Services;
using AEMSWEB.Areas.Setting.Data;
using AEMSWEB.IServices;
using AEMSWEB.Repository;
using AEMSWEB.Services;
using AEMSWEB.Services.IServices;
using AEMSWEB.Services.Masters;

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
        services.AddScoped<IAccountSetupService, AccountSetupService>();
        services.AddScoped<IAccountTypeService, AccountTypeService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IBankAddressService, BankAddressService>();
        services.AddScoped<IBankContactService, BankContactService>();
        services.AddScoped<IBargeService, BargeService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IChartOfAccountService, ChartOfAccountService>();
        services.AddScoped<ICOACategoryService, COACategoryService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<ICustomerGroupCreditLimitService, CustomerGroupCreditLimitService>();
        services.AddScoped<ICustomerCreditLimitService, CustomerCreditLimitService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDesignationService, DesignationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IGroupCreditLimitService, GroupCreditLimitService>();
        services.AddScoped<IGroupCreditLimit_CustomerService, GroupCreditLimit_CustomerService>();
        services.AddScoped<IGstService, GstService>();
        services.AddScoped<IOrderTypeService, OrderTypeService>();
        services.AddScoped<IPaymentTypeService, PaymentTypeService>();
        services.AddScoped<IPortRegionService, PortRegionService>();
        services.AddScoped<IPortService, PortService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISupplierService, SupplierService>();
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

        services.AddScoped<ISettingService, SettingServices>();

        #endregion Setting

        #region System

        #endregion System

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}