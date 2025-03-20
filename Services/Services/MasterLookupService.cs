using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models.Admin;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;

namespace AEMSWEB.Services.Masters
{
    internal sealed class MasterLookupService : IMasterLookupService
    {
        private readonly IRepository<dynamic> _repository;
        private ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private short recordCount = 0;

        public MasterLookupService(IRepository<dynamic> repository, ApplicationDbContext context, IConfiguration configuration)
        {
            _repository = repository;
            _context = context;
            _configuration = configuration;
            recordCount = Convert.ToInt16(_configuration["LookupDefault:RecordCount"]);
        }

        public async Task<IEnumerable<CompanyViewModel>> GetCompanyLookupAsync(Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CompanyViewModel>($"SELECT CompanyId,CompanyName FROM AdmCompany WHERE IsActive=1 AND CompanyId IN (SELECT CompanyId FROM AdmUserRights WHERE UserId={UserId})");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GetUserLoginCompany",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<UserGroupLookupModel>> GetUserGroupLookupAsync()
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserGroupLookupModel>($"SELECT UserGroupId,UserGroupCode,UserGroupName FROM AdmUserGroup WHERE UserGroupId<>0 And IsActive=1  order by UserGroupName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.UserGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUserGroup",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<UserLookupModel>> GetUserLookupAsync()
        {
            try
            {
                var result = await _repository.GetQueryAsync<UserLookupModel>($"SELECT UserId,UserCode,UserName FROM AdmUser WHERE UserId<>0 And IsActive=1  order by UserName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.User,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmUser",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ModuleLookupModel>> GetModuleLookupAsync(bool IsVisible, bool IsMandatory)
        {
            try
            {
                string sql = "Select ModuleId, ModuleCode, ModuleName from AdmModule where IsActive = 1";
                if (IsMandatory)
                {
                    sql += " And IsMandatory = 1";
                }
                if (IsVisible)
                {
                    sql += " And IsVisible = 1";
                }
                sql += " Order by ModuleName";

                return await _repository.GetQueryAsync<ModuleLookupModel>(sql);

                //return await _repository.GetQueryAsync<ModuleLookupModel>( $"Select ModuleId,ModuleCode,ModuleName from AdmModule where IsActive=1 and Order by ModuleName");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Modules,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmModule",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TransCategoryLookupModel>> GetModuleTransCategoryLookupAsync(bool IsVisible, bool IsMandatory)
        {
            try
            {
                string sql =
                    "SELECT TrnCat.TransCategoryId,TrnCat.TransCategoryName,       TrnCat.TransCategoryCode FROM dbo.AdmModule AS Mod " +
                    " INNER JOIN dbo.AdmTransaction AS Trn ON Trn.ModuleId = Mod.ModuleId" +
                    " INNER JOIN dbo.AdmTransactionCategory AS TrnCat ON TrnCat.TransCategoryId = Trn.TransCategoryId " +
                    "where Mod.IsActive = 1 ";
                if (IsMandatory)
                {
                    sql += " And Trn.IsMandatory = 1";
                }
                if (IsVisible)
                {
                    sql += " And Trn.IsVisible = 1";
                }
                sql += " GROUP BY TrnCat.TransCategoryId,TrnCat.TransCategoryName,        TrnCat.TransCategoryCode ORDER BY TrnCat.TransCategoryName";

                return await _repository.GetQueryAsync<TransCategoryLookupModel>(sql);
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Modules,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmModule",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TransactionLookupModel>> GetTransactionLookupAsync(Int16 ModuleId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<TransactionLookupModel>($"Select TransactionId,TransactionCode,TransactionName from AdmTransaction where IsActive=1 And ModuleId={ModuleId} Order by TransactionName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = 0,
                    ModuleId = (short)E_Modules.Admin,
                    TransactionId = (short)E_Admin.Transaction,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "AdmTransaction",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = 0
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<AccountSetupCategoryLookupModel>> GetAccountSetupCategoryLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<AccountSetupCategoryLookupModel>($"select AccSetupCategoryId,AccSetupCategoryCode,AccSetupCategoryName from M_AccountSetupCategory where AccSetupCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetupCategory})) order by AccSetupCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetupCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetupCategory",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<AccountSetupLookupModel>> GetAccountSetupLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<AccountSetupLookupModel>($"select AccSetupId,AccSetupCode,AccSetupName from M_AccountSetup where AccSetupId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountSetup})) order by AccSetupName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountSetup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountSetup",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<AccountTypeLookupModel>> GetAccountTypeLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<AccountTypeLookupModel>($"select AccTypeId,AccTypeCode,AccTypeName from M_AccountType where AccTypeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountType})) order by AccTypeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountType",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<AccountGroupLookupModel>> GetAccountGroupLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<AccountGroupLookupModel>($"select AccGroupId,AccGroupCode,AccGroupName from M_AccountGroup where AccGroupId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.AccountGroup})) order by AccGroupName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.AccountGroup,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_AccountGroup",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<BankLookupModel>> GetBankLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BankLookupModel>($"select BankId,BankCode,BankName from M_Bank where BankId<>0 And IsActive=1 And IsOwnBank=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Bank})) order by BankName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Bank",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<BankLookupModel>> GetBankLookup_SuppAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BankLookupModel>($"select BankId,BankCode,BankName from M_Bank where BankId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Bank})) order by BankName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Bank",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CategoryLookupModel>> GetCategoryLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CategoryLookupModel>($"select CategoryId,CategoryCode,CategoryName from M_Category where CategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Category})) order by CategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Category,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Category",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ChartOfAccountLookupModel>> GetChartOfAccountLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<ChartOfAccountLookupModel>($"select GLId,GLCode,GLName from M_ChartOfAccount where GLId<>0 And IsActive=1 And CompanyId ={CompanyId} order by GLName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.ChartOfAccount,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_ChartOfAccount",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<COACategoryLookupModel>> GetCOACategory1LookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<COACategoryLookupModel>($"select COACategoryId,COACategoryCode,COACategoryName from M_COACategory1 where COACategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory1})) order by COACategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory1,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory1",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<COACategoryLookupModel>> GetCOACategory2LookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<COACategoryLookupModel>($"select COACategoryId,COACategoryCode,COACategoryName from M_COACategory2 where COACategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory2})) order by COACategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory2,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory2",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<COACategoryLookupModel>> GetCOACategory3LookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<COACategoryLookupModel>($"select COACategoryId,COACategoryCode,COACategoryName from M_COACategory3 where COACategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.COACategory3})) order by COACategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.COACategory3,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_COACategory3",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CountryLookupModel>> GetCountryLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CountryLookupModel>($"SELECT CountryId,CountryCode,CountryName FROM M_Country WHERE CountryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Country})) order by CountryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Country,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Country",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CurrencyLookupModel>> GetCurrencyLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CurrencyLookupModel>($"SELECT CurrencyId,CurrencyCode,CurrencyName,IsMultiply FROM M_Currency WHERE CurrencyId<>0 And IsActive=1 And CompanyId ={CompanyId} order by CurrencyName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Currency,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Currency",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerGroupCreditLimitLookupModel>> GetCustomerGroupCreditLimitLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerGroupCreditLimitLookupModel>($"select GroupCreditLimitId,GroupCreditLimitCode,GroupCreditLimitName from M_CustomerGroupCreditLimit WHERE GroupCreditLimitId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CustomerGroupCreditLimit})) order by GroupCreditLimitName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CustomerGroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerGroupCreditLimit",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<DepartmentLookupModel>> GetDepartmentLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<DepartmentLookupModel>($"select DepartmentId,DepartmentCode,DepartmentName from M_Department WHERE DepartmentId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Department})) order by DepartmentName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Department,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Department",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<DesignationLookupModel>> GetDesignationLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<DesignationLookupModel>($"select DesignationId,DesignationCode,DesignationName from M_Designation WHERE DesignationId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Designation})) order by DesignationName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Designation,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Designation",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<EmployeeLookupModel>> GetEmployeeLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<EmployeeLookupModel>($"select EmployeeId,EmployeeCode,EmployeeName from M_Employee WHERE EmployeeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Employee})) order by EmployeeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Employee,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Employee",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<GroupCreditLimitLookupModel>> GetGroupCreditLimitLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<GroupCreditLimitLookupModel>($"select GroupCreditLimitId,GroupCreditLimitCode,GroupCreditLimitName from M_GroupCreditLimit WHERE GroupCreditLimitId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit})) order by GroupCreditLimitName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimit",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<GstLookupModel>> GetGstLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<GstLookupModel>($"select GstId,GstCode,GstName from M_Gst WHERE GstId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Gst})) order by GstName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Gst,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Gst",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<GstCategoryLookupModel>> GetGstCategoryLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<GstCategoryLookupModel>($"select GstCategoryId,GstCategoryCode,GstCategoryName from M_GstCategory WHERE GstCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GstCategory})) order by GstCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GstCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GstCategory",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<OrderTypeCategoryLookupModel>> GetOrderTypeCategoryLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<OrderTypeCategoryLookupModel>($"select OrderTypeCategoryId,OrderTypeCategoryCode,OrderTypeCategoryName from M_OrderTypeCategory WHERE OrderTypeCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderTypeCategory})) order by OrderTypeCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.OrderTypeCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderTypeCategory",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<OrderTypeLookupModel>> GetOrderTypeLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<OrderTypeLookupModel>($"select OrderTypeId,OrderTypeCode,OrderTypeName from M_OrderType WHERE OrderTypeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderType})) order by OrderTypeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.OrderType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderType",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<PaymentTypeLookupModel>> GetPaymentTypeLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<PaymentTypeLookupModel>($"select PaymentTypeId,PaymentTypeCode,PaymentTypeName from M_PaymentType WHERE PaymentTypeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PaymentType})) order by PaymentTypeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.PaymentType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PaymentType",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<PortLookupModel>> GetPortLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<PortLookupModel>($"select PortId,PortCode,PortName from M_Port WHERE PortId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Port})) order by PortName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Port,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Port",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<PortRegionLookupModel>> GetPortRegionLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<PortRegionLookupModel>($"select PortRegionId,PortRegionCode,PortRegionName from M_PortRegion WHERE PortRegionId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.PortRegion})) order by PortRegionName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.PortRegion,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_PortRegion",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<SubCategoryLookupModel>> GetSubCategoryLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SubCategoryLookupModel>($"select SubCategoryId,SubCategoryCode,SubCategoryName from M_SubCategory WHERE SubCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.SubCategory})) order by SubCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.SubCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SubCategory",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<UomLookupModel>> GetUomLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<UomLookupModel>($"select UomId,UomCode,UomName from M_Uom WHERE UomId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Uom})) order by UomName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Uom,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Uom",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CreditTermsLookupModel>> GetCreditTermsLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CreditTermsLookupModel>($"SELECT CreditTermId,CreditTermCode,CreditTermName FROM dbo.M_CreditTerm WHERE CreditTermId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CreditTerms})) order by CreditTermName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CreditTerms,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TaxLookupModel>> GetTaxLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<TaxLookupModel>($"SELECT TaxId,TaxCode,TaxName FROM dbo.M_Tax WHERE TaxId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Tax})) order by TaxName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Tax,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<BargeLookupModel>> GetBargeLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BargeLookupModel>($"SELECT BargeId,BargeCode,BargeName FROM M_Barge WHERE BargeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Barge})) order by BargeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<BargeLookupModel>> GetBargeLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount)
        {
            RecordCount = RecordCount == 0 ? recordCount : RecordCount;
            try
            {
                var result = await _repository.GetQueryAsync<BargeLookupModel>($"SELECT TOP {RecordCount} BargeId,BargeCode,BargeName FROM M_Barge WHERE BargeId<>0 And IsActive=1 And (BargeName like '%{searchString}%' OR BargeCode like '%{searchString}%') And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Barge})) order by BargeName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Barge,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Barge",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ProductLookupModel>> GetProductLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<ProductLookupModel>($"select ProductId,ProductCode,ProductName from M_Product WHERE ProductId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Product})) order by ProductName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Product,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Product",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<ProductLookupModel>> GetProductLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount)
        {
            RecordCount = RecordCount == 0 ? recordCount : RecordCount;
            try
            {
                var result = await _repository.GetQueryAsync<ProductLookupModel>($"select TOP {RecordCount} ProductId,ProductCode,ProductName from M_Product WHERE ProductId<>0 And IsActive=1 And (ProductCode like '%{searchString}%' OR ProductName like '%{searchString}%') And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Product})) order by ProductName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Product,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Product",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerLookupModel>($"select CustomerId,CustomerCode,CustomerName,CurrencyId,CreditTermId,BankId from M_Customer WHERE CustomerId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer})) order by CustomerName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Customer",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerLookupModel>> GetCustomerLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount)
        {
            RecordCount = RecordCount == 0 ? recordCount : RecordCount;
            try
            {
                var result = await _repository.GetQueryAsync<CustomerLookupModel>($"select TOP {RecordCount} CustomerId,CustomerCode,CustomerName,CurrencyId,CreditTermId,BankId from M_Customer WHERE CustomerId<>0 And IsActive=1 And (CustomerCode like '%{searchString}%' Or CustomerName like '%{searchString}%') And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer})) order by CustomerName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Customer",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierLookupModel>($"select SupplierId,SupplierCode,SupplierName,CurrencyId,CreditTermId from M_Supplier WHERE SupplierId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier})) order by SupplierName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<SupplierLookupModel>> GetSupplierLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount)
        {
            RecordCount = RecordCount == 0 ? recordCount : RecordCount;
            try
            {
                var result = await _repository.GetQueryAsync<SupplierLookupModel>($"select TOP {RecordCount} SupplierId,SupplierCode,SupplierName,CurrencyId,CreditTermId from M_Supplier WHERE SupplierId<>0 And IsActive=1 And (SupplierCode like '%{searchString}%' Or SupplierName like '%{searchString}%') And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Supplier})) order by SupplierName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Supplier",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VesselLookupModel>> GetVesselLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<VesselLookupModel>($"SELECT VesselId,VesselCode,VesselName FROM M_Vessel WHERE VesselId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Vessel})) order by VesselName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VesselLookupModel>> GetVesselLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount)
        {
            RecordCount = RecordCount == 0 ? recordCount : RecordCount;
            try
            {
                var result = await _repository.GetQueryAsync<VesselLookupModel>($"SELECT TOP {RecordCount} VesselId,VesselCode,VesselName FROM M_Vessel WHERE VesselId<>0 And IsActive=1 And (VesselName like '%{searchString}%' OR VesselCode like '%{searchString}%') And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Vessel})) order by VesselName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Vessel,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Vessel",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<VoyageLookupModel>($"select VoyageId,VoyageNo,ReferenceNo from M_Voyage WHERE VoyageId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Voyage})) order by VoyageNo");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Voyage,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<VoyageLookupModel>> GetVoyageLookupAsync_V1(Int16 CompanyId, Int16 UserId, string searchString, Int16 RecordCount)
        {
            RecordCount = RecordCount == 0 ? recordCount : RecordCount;
            try
            {
                var result = await _repository.GetQueryAsync<VoyageLookupModel>($"select TOP {RecordCount} VoyageId,VoyageNo,ReferenceNo from M_Voyage WHERE VoyageId<>0 And IsActive=1 And (VoyageNo like '%{searchString}%' OR ReferenceNo like '%{searchString}%') And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Voyage})) order by VoyageNo");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Voyage,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Voyage",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerAddressLookupModel>> GetCustomerAddressLookup_FinAsync(Int16 CompanyId, Int16 UserId, Int16 CustomerId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerAddressLookupModel>($"select AddressId, Address1, Address2, Address3, Address4, PinCode, CountryId, PhoneNo, FaxNo, EmailAdd, WebUrl from M_CustomerAddress where IsActive=1 And CustomerId={CustomerId} order by IsFinAdd Desc,IsDefaultAdd desc");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerAddress",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerContactLookupModel>> GetCustomerContactLookup_FinAsync(Int16 CompanyId, Int16 UserId, Int16 CustomerId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerContactLookupModel>($"Select ContactId, ContactName, OtherName, MobileNo, OffNo, FaxNo, EmailAdd, MessId, ContactMessType from M_CustomerContact where IsActive=1 And CustomerId={CustomerId} Order by IsFinance Desc,IsDefault desc");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<SupplierAddressLookupModel>> GetSupplierAddressLookup_FinAsync(Int16 CompanyId, Int16 UserId, Int16 SupplierId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierAddressLookupModel>($"select AddressId, Address1, Address2, Address3, Address4, PinCode, CountryId, PhoneNo, FaxNo, EmailAdd, WebUrl from M_SupplierAddress where IsActive=1 And SupplierId={SupplierId} order by IsFinAdd Desc,IsDefaultAdd desc");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierAddress",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<SupplierContactLookupModel>> GetSupplierContactLookup_FinAsync(Int16 CompanyId, Int16 UserId, Int16 SupplierId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierContactLookupModel>($"Select ContactId, ContactName, OtherName, MobileNo, OffNo, FaxNo, EmailAdd, MessId, ContactMessType from M_SupplierContact where IsActive=1 And SupplierId={SupplierId} Order by IsFinance Desc,IsDefault desc");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierContact",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<TaxCategoryLookupModel>> GetTaxCategoryLookupAsync(Int16 CompanyId, Int16 UserId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<TaxCategoryLookupModel>($"SELECT TaxCategoryId,TaxCategoryCode,TaxCategoryName FROM dbo.M_TaxCategory WHERE TaxCategoryId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.TaxCategory})) order by TaxCategoryName");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.TaxCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CreditTerm",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<YearLookupModel>> GetPeriodCloseYearLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<YearLookupModel>($"SELECT  FinYear AS YearId,FinYear YearCode,FinYear YearName FROM dbo.GLPeriodClose GROUP BY FinYear ORDER BY FinYear");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.PeriodClose,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GLPeriodClose : Year",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<YearLookupModel>> GetPeriodCloseNextYearLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<YearLookupModel>(
                    $"SELECT ROW_NUMBER() OVER (ORDER BY FinYear) AS YearId,FinYear AS YearCode,FinYear AS YearName" +
                    $"FROM (SELECT DISTINCT FinYear FROM dbo.GLPeriodClose) AS distinctYears" +
                    $"UNION ALL" +
                    $"SELECT (SELECT COUNT(DISTINCT FinYear) FROM dbo.GLPeriodClose) + 1 AS YearId, MAX(FinYear) + 1 AS YearCode,MAX(FinYear) + 1 AS YearName" +
                    $"FROM dbo.GLPeriodClose");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.GL,
                    TransactionId = (short)E_GL.PeriodClose,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "GLPeriodClose : Year",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<DocumentTypeLookupModel>> GetDocumentTypeLookupAsync(Int16 CompanyId, Int16 UserId, Int16 ModuleId)
        {
            try
            {
                return await _repository.GetQueryAsync<DocumentTypeLookupModel>(
                    $"SELECT DocTypeId,DocTypeCode,DocTypeName FROM dbo.M_DocumentType WHERE DocTypeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.DocumentType})) order by DocTypeName");
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.DocumentType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_DocumentType",
                    ModeId = (short)E_Mode.Lookup,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        //public async Task<IEnumerable<BargeLookupModel>> GetBargeLookupAsync( Int16 CompanyId, Int16 UserId)
        //{
        //    try
        //    {
        //        var result = await _repository.GetQueryAsync<BargeLookupModel>( $"SELECT BargeId,BargeCode,BargeName FROM M_Barge WHERE BargeId<>0 And IsActive=1 And CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Master.Barge},{(short)E_Modules.Master})) order by BargeName");

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorLog = new AdmErrorLog
        //        {
        //            CompanyId = CompanyId,
        //            ModuleId = (short)E_Modules.Master,
        //            TransactionId = (short)E_Master.Barge,
        //            DocumentId = 0,
        //            DocumentNo = "",
        //            TblName = "M_Barge",
        //            ModeId = (short)E_Mode.Lookup,
        //            Remarks = ex.Message + ex.InnerException?.Message,
        //            CreateById = UserId
        //        };

        //        _context.Add(errorLog);
        //        _context.SaveChanges();

        //        throw new Exception(ex.ToString());
        //    }
        //}
    }
}