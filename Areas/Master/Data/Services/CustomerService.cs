using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.Helpers;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Models.Masters;
using AMESWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Master.Data.Services
{
    public sealed class CustomerService : ICustomerService
    {
        private readonly IRepository<M_Customer> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public CustomerService(IRepository<M_Customer> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        #region
        #endregion
        #region
        #endregion
        #region
        #endregion

        public async Task<CustomerViewModelCount> GetCustomerListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CustomerViewModelCount countViewModel = new CustomerViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Customer M_Cus INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Cus.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Cus.CurrencyId WHERE (M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cus.CustomerRegNo LIKE '%{searchString}%' OR M_Cus.CustomerOtherName LIKE '%{searchString}%' OR M_Cus.CustomerShortName LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.Remarks LIKE '%{searchString}%') AND M_Cus.CustomerId<>0 AND M_Cus.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer}))");

                var result = await _repository.GetQueryAsync<CustomerViewModel>($"SELECT M_Cus.CustomerId,M_Cus.CompanyId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Cus.CustomerOtherName,M_Cus.CustomerShortName,M_Cus.CustomerRegNo,M_Cus.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Cus.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_Cus.ParentCustomerId,M_Cus.AccSetupId,M_Set.AccSetupCode,M_Set.AccSetupName,M_Cus.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,M_Cus.BankId,M_Bak.BankCode,M_Bak.BankName,M_Cus.IsCustomer,M_Cus.IsVendor,M_Cus.IsTrader,M_Cus.IsSupplier,M_Cus.Remarks,M_Cus.IsActive,M_Cus.CreateById,M_Cus.CreateDate,M_Cus.EditById,M_Cus.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Customer M_Cus LEFT JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = M_Cus.SupplierId LEFT JOIN dbo.M_AccountSetup M_Set ON M_Set.AccSetupId = M_Cus.AccSetupId LEFT JOIN dbo.M_Bank M_Bak ON M_Bak.BankId = M_Cus.BankId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Cus.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Cus.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cus.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cus.EditById WHERE (M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Cus.CustomerRegNo LIKE '%{searchString}%' OR M_Cus.CustomerOtherName LIKE '%{searchString}%' OR M_Cus.CustomerShortName LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.Remarks LIKE '%{searchString}%') AND M_Cus.CustomerId<>0 AND M_Cus.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer})) ORDER BY M_Cus.CustomerName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<CustomerViewModel>();

                return countViewModel;
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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CustomerViewModel> GetCustomerByIdAsync(short CompanyId, short UserId, int CustomerId, string CustomerCode, string CustomerName)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CustomerViewModel>($"SELECT TOP (1) M_Cus.CustomerId,M_Cus.CompanyId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Cus.CustomerOtherName,M_Cus.CustomerShortName,M_Cus.CustomerRegNo,M_Cus.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Cus.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_Cus.ParentCustomerId,M_Cus.AccSetupId,M_Set.AccSetupCode,M_Set.AccSetupName,M_Cus.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,M_Cus.BankId,M_Bak.BankCode,M_Bak.BankName,M_Cus.IsCustomer,M_Cus.IsVendor,M_Cus.IsTrader,M_Cus.IsSupplier,M_Cus.Remarks,M_Cus.IsActive,M_Cus.CreateById,M_Cus.CreateDate,M_Cus.EditById,M_Cus.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Customer M_Cus LEFT JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = M_Cus.SupplierId LEFT JOIN dbo.M_AccountSetup M_Set ON M_Set.AccSetupId = M_Cus.AccSetupId LEFT JOIN dbo.M_Bank M_Bak ON M_Bak.BankId = M_Cus.BankId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Cus.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Cus.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cus.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cus.EditById WHERE (M_Cus.CustomerId={CustomerId} OR {CustomerId}=0) AND (M_Cus.CustomerCode='{CustomerCode}' OR '{CustomerCode}'='{string.Empty}') AND (M_Cus.CustomerName='{CustomerName}' OR '{CustomerName}'='{string.Empty}') AND M_Cus.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer}))");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCustomerAsync(short CompanyId, short UserId, M_Customer m_Customer)
        {
            bool IsEdit = m_Customer.CustomerId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Customer WHERE CustomerId <>{m_Customer.CustomerId} AND CustomerCode='{m_Customer.CustomerCode}' AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer}))");

                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Customer Code exists" };

                    //var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>( $"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Customer WHERE CustomerId <>{m_Customer.CustomerId} AND CustomerName='{m_Customer.CustomerName}' AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer}))");

                    //if ((nameExist?.IsExist ?? 0) > 0)
                    //    return new SqlResponse { Result = -1, Message = "Customer Name exists" };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Customer WHERE CustomerId<>0 AND CustomerId={m_Customer.CustomerId} AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Customer}))");

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Customer);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Customer Not Found" };
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>("SELECT ISNULL((SELECT TOP 1 (CustomerId + 1) FROM dbo.M_Customer WHERE (CustomerId + 1) NOT IN (SELECT CustomerId FROM dbo.M_Customer)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Customer.CustomerId = Convert.ToInt32(sqlMissingResponse.NextId);

                            m_Customer.EditDate = null;
                            m_Customer.EditById = null;
                            _context.Add(m_Customer);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "CustomerId Should not be zero" };
                    }

                    var CustomerToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CustomerToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Customer,
                            DocumentId = m_Customer.CustomerId,
                            DocumentNo = m_Customer.CustomerCode,
                            TblName = "M_Customer",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Customer Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = m_Customer.CustomerCode,
                    TblName = "M_Customer",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = m_Customer.CustomerCode,
                    TblName = "M_Customer",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteCustomerAsync(short CompanyId, short UserId, int CustomerId)
        {
            string CustomerCode = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    CustomerCode = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT CustomerCode FROM dbo.M_Customer WHERE CustomerId={CustomerId}");

                    if (CustomerId > 0 && CustomerCode != null)
                    {
                        var CustomerToRemove = _context.M_Customer.Where(x => x.CustomerId == CustomerId).ExecuteDelete();

                        if (CustomerToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Customer,
                                DocumentId = CustomerId,
                                DocumentNo = CustomerCode,
                                TblName = "M_Customer",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Customer Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "CustomerId Should be zero" };
                    }
                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = CustomerId,
                    DocumentNo = CustomerCode,
                    TblName = "M_Customer",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = CustomerId,
                    DocumentNo = CustomerCode,
                    TblName = "M_Customer",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        //Customer Address List
        public async Task<IEnumerable<CustomerAddressViewModel>> GetCustomerAddressByCustomerIdAsync(short CompanyId, short UserId, int CustomerId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerAddressViewModel>($"SELECT  M_CusAdd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusAdd.AddressId,M_CusAdd.Address1,M_CusAdd.Address2,M_CusAdd.Address3,M_CusAdd.Address4,M_CusAdd.PinCode,M_CusAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName  ,M_CusAdd.PhoneNo,M_CusAdd.FaxNo,M_CusAdd.EmailAdd,M_CusAdd.WebUrl,M_CusAdd.IsDefaultAdd,M_CusAdd.IsDeliveryAdd,M_CusAdd.IsFinAdd,M_CusAdd.IsSalesAdd,M_CusAdd.IsActive,M_CusAdd.CreateById,M_CusAdd.CreateDate,M_CusAdd.EditById,M_CusAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CustomerAddress M_CusAdd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusAdd.CustomerId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_CusAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_CusAdd.EditById WHERE M_CusAdd.CustomerId = {CustomerId} AND M_CusAdd.AddressId <>0 ");

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Customer, CustomerId, "", "M_CustomerAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        //Customer Address one record by using addressId
        public async Task<CustomerAddressViewModel> GetCustomerAddressByIdAsync(short CompanyId, short UserId, int CustomerId, short AddressId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CustomerAddressViewModel>($"SELECT  M_CusAdd.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusAdd.AddressId,M_CusAdd.Address1,M_CusAdd.Address2,M_CusAdd.Address3,M_CusAdd.Address4,M_CusAdd.PinCode,M_CusAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName  ,M_CusAdd.PhoneNo,M_CusAdd.FaxNo,M_CusAdd.EmailAdd,M_CusAdd.WebUrl,M_CusAdd.IsDefaultAdd,M_CusAdd.IsDeliveryAdd,M_CusAdd.IsFinAdd,M_CusAdd.IsSalesAdd,M_CusAdd.IsActive,M_CusAdd.CreateById,M_CusAdd.CreateDate,M_CusAdd.EditById,M_CusAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CustomerAddress M_CusAdd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusAdd.CustomerId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_CusAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_CusAdd.EditById WHERE M_CusAdd.CustomerId = {CustomerId} And M_CusAdd.AddressId={AddressId}");

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Customer, AddressId, "", "M_CustomerAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCustomerAddressAsync(short CompanyId, short UserId, M_CustomerAddress m_CustomerAddress)
        {
            bool IsEdit = m_CustomerAddress.CustomerId != 0 && m_CustomerAddress.AddressId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_CustomerAddress where CustomerId = {m_CustomerAddress.CustomerId} And Address1 = '{m_CustomerAddress.Address1}' And AddressId<>{m_CustomerAddress.AddressId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponse { Result = -1, Message = "Customer Address Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_CustomerAddress);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CustomerId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT ISNULL((SELECT TOP 1(AddressId + 1) FROM dbo.M_CustomerAddress WHERE CustomerId = {m_CustomerAddress.CustomerId} AND (AddressId + 1) NOT IN(SELECT AddressId FROM dbo.M_CustomerAddress where CustomerId= {m_CustomerAddress.CustomerId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_CustomerAddress.AddressId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_CustomerAddress.EditDate = null;
                            m_CustomerAddress.EditById = null;
                            _context.Add(m_CustomerAddress);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Id Should not be zero" };
                    }

                    var CustomerToSave = _context.SaveChanges();

                    if (CustomerToSave > 0)
                    {
                        await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.Customer, m_CustomerAddress.AddressId, m_CustomerAddress.Address1, "M_CustomerAddress", IsEdit ? E_Mode.Update : E_Mode.Create, "CustomerAddress Save Successfully", UserId);
                        TScope.Complete();
                        return new SqlResponse { Result = 1, Message = "Save Successfully" };
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "Save Failed" };
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Customer, m_CustomerAddress.AddressId, "", "M_CustomerAddress", E_Mode.Delete, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Customer, m_CustomerAddress.AddressId, "", "M_CustomerAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteCustomerAddressAsync(short CompanyId, short UserId, int CustomerId, short AddressId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (AddressId > 0 && CustomerId > 0)
                    {
                        var CustomerAddressToRemove = _context.M_CustomerAddress.Where(x => x.AddressId == AddressId && x.CustomerId == CustomerId).ExecuteDelete();

                        if (CustomerAddressToRemove > 0)
                        {
                            await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.Customer, AddressId, "", "M_CustomerAddress", E_Mode.Delete, "CustomerAddress Delete Successfully", UserId);
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "AddressId Should be zero" };
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Customer, AddressId, "", "M_CustomerAddress", E_Mode.Delete, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Customer, AddressId, "", "M_CustomerAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<IEnumerable<CustomerContactViewModel>> GetCustomerContactByCustomerIdAsync(short CompanyId, short UserId, int CustomerId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerContactViewModel>($"SELECT M_CusCon.CustomerId,M_CusCon.ContactId,M_CusCon.ContactName,M_CusCon.OtherName,M_CusCon.MobileNo,M_CusCon.OffNo,M_CusCon.FaxNo,M_CusCon.EmailAdd,M_CusCon.MessId,M_CusCon.ContactMessType,M_CusCon.IsDefault,M_CusCon.IsFinance,M_CusCon.IsSales,M_CusCon.IsActive,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusCon.CreateById,M_CusCon.CreateDate,M_CusCon.EditById,M_CusCon.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CustomerContact M_CusCon INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusCon.CustomerId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusCon.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CusCon.EditById WHERE M_CusCon.CustomerId = {CustomerId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CustomerContactViewModel> GetCustomerContactByIdAsync(short CompanyId, short UserId, int CustomerId, short ContactId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CustomerContactViewModel>($"SELECT M_CusCon.CustomerId,M_CusCon.ContactId,M_CusCon.ContactName,M_CusCon.OtherName,M_CusCon.MobileNo,M_CusCon.OffNo,M_CusCon.FaxNo,M_CusCon.EmailAdd,M_CusCon.MessId,M_CusCon.ContactMessType,M_CusCon.IsDefault,M_CusCon.IsFinance,M_CusCon.IsSales,M_CusCon.IsActive,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusCon.CreateById,M_CusCon.CreateDate,M_CusCon.EditById,M_CusCon.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CustomerContact M_CusCon INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusCon.CustomerId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusCon.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CusCon.EditById WHERE M_CusCon.CustomerId = {CustomerId} AND ContactId={ContactId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCustomerContactAsync(short CompanyId, short UserId, M_CustomerContact m_CustomerContact)
        {
            bool IsEdit = false;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (m_CustomerContact.CustomerId != 0 && m_CustomerContact.ContactId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_CustomerContact where CustomerId = {m_CustomerContact.CustomerId} And ContactName = '{m_CustomerContact.ContactName}' And ContactId<>{m_CustomerContact.ContactId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponse { Result = -1, Message = "Customer Contact Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_CustomerContact);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.CustomerId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT ISNULL((SELECT TOP 1(ContactId + 1) FROM dbo.M_CustomerContact WHERE CustomerId = {m_CustomerContact.CustomerId} AND (ContactId + 1) NOT IN(SELECT ContactId FROM dbo.M_CustomerContact where CustomerId= {m_CustomerContact.CustomerId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_CustomerContact.ContactId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_CustomerContact.EditDate = null;
                            m_CustomerContact.EditById = null;
                            _context.Add(m_CustomerContact);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Id Should not be zero" };
                    }

                    var CustomerToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (CustomerToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Customer,
                            DocumentId = m_CustomerContact.ContactId,
                            DocumentNo = "",
                            TblName = "M_Customer",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Customer Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteCustomerContactAsync(short CompanyId, short UserId, int CustomerId, short ContactId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (CustomerId > 0 && ContactId > 0)
                    {
                        var CustomerContactToRemove = _context.M_CustomerContact.Where(x => x.CustomerId == CustomerId && x.ContactId == ContactId).ExecuteDelete();

                        if (CustomerContactToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Customer,
                                DocumentId = CustomerId,
                                DocumentNo = "",
                                TblName = "M_CustomerContact",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CustomerContact Delete Successfully",
                                CreateById = UserId
                            };
                            _context.Add(auditLog);
                            var auditLogSave = await _context.SaveChangesAsync();
                            if (auditLogSave > 0)
                            {
                                TScope.Complete();
                                return new SqlResponse { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "CustomerId Should be zero" };
                    }
                    return new SqlResponse();
                }
            }
            catch (SqlException sqlEx)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",

                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponse
                {
                    Result = -1,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();

                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Customer,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerContact",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }
    }
}