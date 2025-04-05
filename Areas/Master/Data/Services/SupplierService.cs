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
    public sealed class SupplierService : ISupplierService
    {
        private readonly IRepository<M_Supplier> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public SupplierService(IRepository<M_Supplier> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        #region CUSTOMER

        public async Task<SupplierViewModelCount> GetSupplierListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            SupplierViewModelCount countViewModel = new SupplierViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Supplier M_Sup LEFT JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_Sup.CustomerId LEFT JOIN dbo.M_AccountSetup M_Set ON M_Set.AccSetupId = M_Sup.AccSetupId LEFT JOIN dbo.M_Bank M_Bak ON M_Bak.BankId = M_Sup.BankId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sup.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sup.EditById WHERE (M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Sup.SupplierRegNo LIKE '%{searchString}%' OR M_Sup.SupplierOtherName LIKE '%{searchString}%' OR M_Sup.SupplierShortName LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.Remarks LIKE '%{searchString}%') AND M_Sup.SupplierId<>0 AND M_Sup.CompanyId ={CompanyId}");

                var result = await _repository.GetQueryAsync<SupplierViewModel>($"SELECT M_Sup.SupplierId,M_Sup.CompanyId,M_Sup.SupplierCode,M_Sup.SupplierName,M_Sup.SupplierOtherName,M_Sup.SupplierShortName,M_Sup.SupplierRegNo,M_Sup.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Sup.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_Sup.ParentSupplierId,M_Sup.AccSetupId,M_Set.AccSetupCode,M_Set.AccSetupName,M_Sup.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Sup.BankId,M_Bak.BankCode,M_Bak.BankName,M_Sup.IsSupplier,M_Sup.IsVendor,M_Sup.IsTrader,M_Sup.IsSupplier,M_Sup.Remarks,M_Sup.IsActive,M_Sup.CreateById,M_Sup.CreateDate,M_Sup.EditById,M_Sup.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Supplier M_Sup LEFT JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_Sup.CustomerId LEFT JOIN dbo.M_AccountSetup M_Set ON M_Set.AccSetupId = M_Sup.AccSetupId LEFT JOIN dbo.M_Bank M_Bak ON M_Bak.BankId = M_Sup.BankId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sup.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sup.EditById WHERE (M_Crd.CreditTermName LIKE '%{searchString}%' OR M_Crd.CreditTermCode LIKE '%{searchString}%' OR M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Sup.SupplierRegNo LIKE '%{searchString}%' OR M_Sup.SupplierOtherName LIKE '%{searchString}%' OR M_Sup.SupplierShortName LIKE '%{searchString}%' OR M_Sup.SupplierName LIKE '%{searchString}%' OR M_Sup.SupplierCode LIKE '%{searchString}%' OR M_Sup.Remarks LIKE '%{searchString}%') AND M_Sup.SupplierId<>0 AND M_Sup.CompanyId ={CompanyId} ORDER BY M_Sup.SupplierName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<SupplierViewModel>();

                return countViewModel;
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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SupplierViewModel> GetSupplierByIdAsync(short CompanyId, short UserId, int SupplierId, string SupplierCode, string SupplierName)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<SupplierViewModel>($"SELECT M_Sup.SupplierId,M_Sup.CompanyId,M_Sup.SupplierCode,M_Sup.SupplierName,M_Sup.SupplierOtherName,M_Sup.SupplierShortName,M_Sup.SupplierRegNo,M_Sup.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Sup.CreditTermId,M_Crd.CreditTermCode,M_Crd.CreditTermName,M_Sup.ParentSupplierId,M_Sup.AccSetupId,M_Set.AccSetupCode,M_Set.AccSetupName,M_Sup.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Sup.BankId,M_Bak.BankCode,M_Bak.BankName,M_Sup.IsSupplier,M_Sup.IsVendor,M_Sup.IsTrader,M_Sup.IsSupplier,M_Sup.Remarks,M_Sup.IsActive,M_Sup.CreateById,M_Sup.CreateDate,M_Sup.EditById,M_Sup.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Supplier M_Sup LEFT JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_Sup.CustomerId LEFT JOIN dbo.M_AccountSetup M_Set ON M_Set.AccSetupId = M_Sup.AccSetupId LEFT JOIN dbo.M_Bank M_Bak ON M_Bak.BankId = M_Sup.BankId INNER JOIN dbo.M_CreditTerm M_Crd ON M_Crd.CreditTermId = M_Sup.CreditTermId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Sup.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Sup.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Sup.EditById WHERE (M_Sup.SupplierId={SupplierId} OR {SupplierId}=0) AND (M_Sup.SupplierCode='{SupplierCode}' OR '{SupplierCode}'='{string.Empty}') AND (M_Sup.SupplierName='{SupplierName}' OR '{SupplierName}'='{string.Empty}') AND M_Sup.CompanyId ={CompanyId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveSupplierAsync(short CompanyId, short UserId, M_Supplier m_Supplier)
        {
            bool IsEdit = m_Supplier.SupplierId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Supplier WHERE SupplierId <>{m_Supplier.SupplierId} AND SupplierCode='{m_Supplier.SupplierCode}' AND CompanyId ={CompanyId}");

                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Supplier Code exists" };

                    //var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>( $"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Supplier WHERE SupplierId <>{m_Supplier.SupplierId} AND SupplierName='{m_Supplier.SupplierName}' AND CompanyId ={CompanyId}");

                    //if ((nameExist?.IsExist ?? 0) > 0)
                    //    return new SqlResponse { Result = -1, Message = "Supplier Name exists" };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Supplier WHERE SupplierId<>0 AND SupplierId={m_Supplier.SupplierId} AND CompanyId ={CompanyId}");

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Supplier);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Supplier Not Found" };
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>("SELECT ISNULL((SELECT TOP 1 (SupplierId + 1) FROM dbo.M_Supplier WHERE (SupplierId + 1) NOT IN (SELECT SupplierId FROM dbo.M_Supplier)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Supplier.SupplierId = Convert.ToInt32(sqlMissingResponse.NextId);

                            m_Supplier.EditDate = null;
                            m_Supplier.EditById = null;
                            _context.Add(m_Supplier);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "SupplierId Should not be zero" };
                    }

                    var SupplierToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (SupplierToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Supplier,
                            DocumentId = m_Supplier.SupplierId,
                            DocumentNo = m_Supplier.SupplierCode,
                            TblName = "M_Supplier",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Supplier Save Successfully",
                            CreateById = UserId,
                            CreateDate = DateTime.Now
                        };

                        _context.Add(auditLog);
                        var auditLogSave = _context.SaveChanges();

                        if (auditLogSave > 0)
                        {
                            TScope.Complete();
                            return new SqlResponse { Result = m_Supplier.SupplierId, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponse { Result = -1, Message = "Save Failed" };
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
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = m_Supplier.SupplierCode,
                    TblName = "M_Supplier",
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
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = m_Supplier.SupplierCode,
                    TblName = "M_Supplier",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteSupplierAsync(short CompanyId, short UserId, int SupplierId)
        {
            string SupplierCode = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    SupplierCode = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT SupplierCode FROM dbo.M_Supplier WHERE SupplierId={SupplierId}");

                    if (SupplierId > 0 && SupplierCode != null)
                    {
                        var SupplierToRemove = _context.M_Supplier.Where(x => x.SupplierId == SupplierId).ExecuteDelete();

                        if (SupplierToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Supplier,
                                DocumentId = SupplierId,
                                DocumentNo = SupplierCode,
                                TblName = "M_Supplier",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Supplier Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "SupplierId Should be zero" };
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
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = SupplierId,
                    DocumentNo = SupplierCode,
                    TblName = "M_Supplier",
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
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = SupplierId,
                    DocumentNo = SupplierCode,
                    TblName = "M_Supplier",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion CUSTOMER

        #region ADDRESS

        public async Task<IEnumerable<SupplierAddressViewModel>> GetSupplierAddressBySupplierIdAsync(short CompanyId, short UserId, int SupplierId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierAddressViewModel>($"SELECT  M_SupAdd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,M_SupAdd.AddressId,M_SupAdd.Address1,M_SupAdd.Address2,M_SupAdd.Address3,M_SupAdd.Address4,M_SupAdd.PinCode,M_SupAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName  ,M_SupAdd.PhoneNo,M_SupAdd.FaxNo,M_SupAdd.EmailAdd,M_SupAdd.WebUrl,M_SupAdd.IsDefaultAdd,M_SupAdd.IsDeliveryAdd,M_SupAdd.IsFinAdd,M_SupAdd.IsSalesAdd,M_SupAdd.IsActive,M_SupAdd.CreateById,M_SupAdd.CreateDate,M_SupAdd.EditById,M_SupAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_SupplierAddress M_SupAdd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = M_SupAdd.SupplierId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_SupAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_SupAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_SupAdd.EditById WHERE M_SupAdd.SupplierId = {SupplierId} AND M_SupAdd.AddressId <>0 ");

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Supplier, SupplierId, "", "M_SupplierAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        //Supplier Address one record by using addressId
        public async Task<SupplierAddressViewModel> GetSupplierAddressByIdAsync(short CompanyId, short UserId, int SupplierId, short AddressId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<SupplierAddressViewModel>($"SELECT  M_SupAdd.SupplierId,M_Cus.SupplierCode,M_Cus.SupplierName,M_SupAdd.AddressId,M_SupAdd.Address1,M_SupAdd.Address2,M_SupAdd.Address3,M_SupAdd.Address4,M_SupAdd.PinCode,M_SupAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName  ,M_SupAdd.PhoneNo,M_SupAdd.FaxNo,M_SupAdd.EmailAdd,M_SupAdd.WebUrl,M_SupAdd.IsDefaultAdd,M_SupAdd.IsDeliveryAdd,M_SupAdd.IsFinAdd,M_SupAdd.IsSalesAdd,M_SupAdd.IsActive,M_SupAdd.CreateById,M_SupAdd.CreateDate,M_SupAdd.EditById,M_SupAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_SupplierAddress M_SupAdd INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = M_SupAdd.SupplierId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_SupAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_SupAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_SupAdd.EditById WHERE M_SupAdd.SupplierId = {SupplierId} And M_SupAdd.AddressId={AddressId}");

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Supplier, AddressId, "", "M_SupplierAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveSupplierAddressAsync(short CompanyId, short UserId, M_SupplierAddress m_SupplierAddress)
        {
            bool IsEdit = m_SupplierAddress.SupplierId != 0 && m_SupplierAddress.AddressId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_SupplierAddress where SupplierId = {m_SupplierAddress.SupplierId} And Address1 = '{m_SupplierAddress.Address1}' And AddressId<>{m_SupplierAddress.AddressId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponse { Result = -1, Message = "Supplier Address Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_SupplierAddress);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.SupplierId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT ISNULL((SELECT TOP 1(AddressId + 1) FROM dbo.M_SupplierAddress WHERE SupplierId = {m_SupplierAddress.SupplierId} AND (AddressId + 1) NOT IN(SELECT AddressId FROM dbo.M_SupplierAddress where SupplierId= {m_SupplierAddress.SupplierId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_SupplierAddress.AddressId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_SupplierAddress.EditDate = null;
                            m_SupplierAddress.EditById = null;
                            _context.Add(m_SupplierAddress);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Id Should not be zero" };
                    }

                    var SupplierToSave = _context.SaveChanges();

                    if (SupplierToSave > 0)
                    {
                        await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.Supplier, m_SupplierAddress.AddressId, m_SupplierAddress.Address1, "M_SupplierAddress", IsEdit ? E_Mode.Update : E_Mode.Create, "SupplierAddress Save Successfully", UserId);
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Supplier, m_SupplierAddress.AddressId, "", "M_SupplierAddress", E_Mode.Delete, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Supplier, m_SupplierAddress.AddressId, "", "M_SupplierAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteSupplierAddressAsync(short CompanyId, short UserId, int SupplierId, short AddressId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (AddressId > 0 && SupplierId > 0)
                    {
                        var SupplierAddressToRemove = _context.M_SupplierAddress.Where(x => x.AddressId == AddressId && x.SupplierId == SupplierId).ExecuteDelete();

                        if (SupplierAddressToRemove > 0)
                        {
                            await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.Supplier, AddressId, "", "M_SupplierAddress", E_Mode.Delete, "SupplierAddress Delete Successfully", UserId);
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Supplier, AddressId, "", "M_SupplierAddress", E_Mode.Delete, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Supplier, AddressId, "", "M_SupplierAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        #endregion ADDRESS

        #region CONTACT

        public async Task<IEnumerable<SupplierContactViewModel>> GetSupplierContactBySupplierIdAsync(short CompanyId, short UserId, int SupplierId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierContactViewModel>($"SELECT M_SupCon.SupplierId,M_SupCon.ContactId,M_SupCon.ContactName,M_SupCon.OtherName,M_SupCon.MobileNo,M_SupCon.OffNo,M_SupCon.FaxNo,M_SupCon.EmailAdd,M_SupCon.MessId,M_SupCon.ContactMessType,M_SupCon.IsDefault,M_SupCon.IsFinance,M_SupCon.IsSales,M_SupCon.IsActive,M_Cus.SupplierCode,M_Cus.SupplierName,M_SupCon.CreateById,M_SupCon.CreateDate,M_SupCon.EditById,M_SupCon.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_SupplierContact M_SupCon INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = M_SupCon.SupplierId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_SupCon.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_SupCon.EditById WHERE M_SupCon.SupplierId = {SupplierId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SupplierContactViewModel> GetSupplierContactByIdAsync(short CompanyId, short UserId, int SupplierId, short ContactId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<SupplierContactViewModel>($"SELECT M_SupCon.SupplierId,M_SupCon.ContactId,M_SupCon.ContactName,M_SupCon.OtherName,M_SupCon.MobileNo,M_SupCon.OffNo,M_SupCon.FaxNo,M_SupCon.EmailAdd,M_SupCon.MessId,M_SupCon.ContactMessType,M_SupCon.IsDefault,M_SupCon.IsFinance,M_SupCon.IsSales,M_SupCon.IsActive,M_Cus.SupplierCode,M_Cus.SupplierName,M_SupCon.CreateById,M_SupCon.CreateDate,M_SupCon.EditById,M_SupCon.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_SupplierContact M_SupCon INNER JOIN dbo.M_Supplier M_Cus ON M_Cus.SupplierId = M_SupCon.SupplierId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_SupCon.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_SupCon.EditById WHERE M_SupCon.SupplierId = {SupplierId} AND ContactId={ContactId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveSupplierContactAsync(short CompanyId, short UserId, M_SupplierContact m_SupplierContact)
        {
            bool IsEdit = false;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (m_SupplierContact.SupplierId != 0 && m_SupplierContact.ContactId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_SupplierContact where SupplierId = {m_SupplierContact.SupplierId} And ContactName = '{m_SupplierContact.ContactName}' And ContactId<>{m_SupplierContact.ContactId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponse { Result = -1, Message = "Supplier Contact Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_SupplierContact);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.SupplierId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT ISNULL((SELECT TOP 1(ContactId + 1) FROM dbo.M_SupplierContact WHERE SupplierId = {m_SupplierContact.SupplierId} AND (ContactId + 1) NOT IN(SELECT ContactId FROM dbo.M_SupplierContact where SupplierId= {m_SupplierContact.SupplierId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_SupplierContact.ContactId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_SupplierContact.EditDate = null;
                            m_SupplierContact.EditById = null;
                            _context.Add(m_SupplierContact);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Id Should not be zero" };
                    }

                    var SupplierToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (SupplierToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Supplier,
                            DocumentId = m_SupplierContact.ContactId,
                            DocumentNo = "",
                            TblName = "M_Supplier",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Supplier Save Successfully",
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
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierContact",
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
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierContact",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteSupplierContactAsync(short CompanyId, short UserId, int SupplierId, short ContactId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (SupplierId > 0 && ContactId > 0)
                    {
                        var SupplierContactToRemove = _context.M_SupplierContact.Where(x => x.SupplierId == SupplierId && x.ContactId == ContactId).ExecuteDelete();

                        if (SupplierContactToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Supplier,
                                DocumentId = SupplierId,
                                DocumentNo = "",
                                TblName = "M_SupplierContact",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "SupplierContact Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "SupplierId Should be zero" };
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
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierContact",

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
                    TransactionId = (short)E_Master.Supplier,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_SupplierContact",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion CONTACT
    }
}