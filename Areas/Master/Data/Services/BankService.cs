using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Helpers;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class BankService : IBankService
    {
        private readonly IRepository<M_Bank> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public BankService(IRepository<M_Bank> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<BankViewModelCount> GetBankListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            BankViewModelCount countViewModel = new BankViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM dbo.M_Bank M_Ban INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.AccountNo LIKE '%{searchString}%' OR M_Ban.SwiftCode LIKE '%{searchString}%' OR M_Ban.Remarks1 LIKE '%{searchString}%' OR M_Ban.Remarks2 LIKE '%{searchString}%' OR M_Chr.GLName LIKE '%{searchString}%' OR M_Chr.GLCode LIKE '%{searchString}%') AND M_Ban.BankId<>0 AND M_Ban.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<BankViewModel>($"SELECT M_Ban.BankId,M_Ban.CompanyId,M_Ban.BankCode,M_Ban.BankName,M_Cur.CurrencyId,M_Cur.CurrencyName,M_Cur.CurrencyCode,M_Ban.AccountNo,M_Ban.SwiftCode,M_Ban.Remarks1,M_Ban.Remarks2,M_Ban.GLId,M_Chr.GLCode,M_Chr.GLName,M_Ban.IsActive,M_Ban.IsOwnBank,M_Ban.CreateById,M_Ban.CreateDate,M_Ban.EditById,M_Ban.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy  FROM dbo.M_Bank M_Ban INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Ban.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Ban.EditById WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Ban.BankName LIKE '%{searchString}%' OR M_Ban.BankCode LIKE '%{searchString}%' OR M_Ban.AccountNo LIKE '%{searchString}%' OR M_Ban.SwiftCode LIKE '%{searchString}%' OR M_Ban.Remarks1 LIKE '%{searchString}%' OR M_Ban.Remarks2 LIKE '%{searchString}%' OR M_Chr.GLName LIKE '%{searchString}%' OR M_Chr.GLCode LIKE '%{searchString}%') AND M_Ban.BankId<>0 AND M_Ban.CompanyId={CompanyId} ORDER BY M_Ban.BankName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<BankViewModel>();

                return countViewModel;
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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BankViewModel> GetBankByIdAsync(short CompanyId, short UserId, short BankId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<BankViewModel>($"SELECT BankId,CompanyId,BankCode,BankName,CurrencyId,AccountNo,SwiftCode,Remarks1,Remarks2,GLId,IsActive,IsOwnBank,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Bank WHERE BankId={BankId} AND CompanyId ={CompanyId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BankViewModel> GetBankAsync(short CompanyId, short UserId, int BankId, string BankCode, string BankName)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<BankViewModel>($"SELECT\r\nM_Ban.BankId,M_Ban.CompanyId,M_Ban.BankCode,M_Ban.BankName,M_Ban.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Ban.AccountNo,M_Ban.SwiftCode,M_Ban.Remarks1,M_Ban.Remarks2,M_Ban.IsActive,M_Ban.IsOwnBank,M_Ban.GLId,M_Chr.GLName,M_Chr.GLCode,M_Ban.CreateById,M_Ban.CreateDate,M_Ban.EditById,M_Ban.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Bank M_Ban INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Ban.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Ban.EditById WHERE (M_Ban.BankId={BankId} OR {BankId}=0) AND (M_Ban.BankCode='{BankCode}' OR '{BankCode}'='{string.Empty}') AND (M_Ban.BankName='{BankName}' OR '{BankName}'='{string.Empty}') AND M_Ban.CompanyId={CompanyId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveBankAsync(short CompanyId, short UserId, M_Bank m_Bank)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Bank.BankId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Bank WHERE BankId<>@BankId AND BankCode=@BankCode",
                        new { m_Bank.BankId, m_Bank.BankCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Bank Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Bank WHERE BankId<>@BankId AND BankName=@BankName",
                        new { m_Bank.BankId, m_Bank.BankName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Bank Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Bank WHERE BankId=@BankId",
                            new { m_Bank.BankId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Bank);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Bank Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (BankId + 1) FROM dbo.M_Bank WHERE (BankId + 1) NOT IN (SELECT BankId FROM dbo.M_Bank)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Bank.BankId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Bank);
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Bank,
                            DocumentId = m_Bank.BankId,
                            DocumentNo = m_Bank.BankCode,
                            TblName = "M_Bank",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Bank Save Successfully",
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
                catch (SqlException sqlEx)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.COACategory1,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_COACategory1",
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
                        TransactionId = (short)E_Master.Bank,
                        DocumentId = m_Bank.BankId,
                        DocumentNo = m_Bank.BankCode,
                        TblName = "AdmUser",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException?.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw;
                }
            }
        }

        public async Task<SqlResponse> DeleteBankAsync(short CompanyId, short UserId, short BankId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (BankId > 0)
                    {
                        var BankToRemove = _context.M_Bank.Where(x => x.BankId == BankId).ExecuteDelete();

                        if (BankToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Bank,
                                DocumentId = BankId,
                                DocumentNo = "",
                                TblName = "M_Bank",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Bank Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "BankId Should be zero" };
                    }
                    return new SqlResponse();
                }
                catch (SqlException sqlEx)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.COACategory1,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_COACategory1",
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
                        TransactionId = (short)E_Master.Bank,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_Bank",
                        ModeId = (short)E_Mode.Delete,
                        Remarks = ex.Message + ex.InnerException?.Message,
                        CreateById = UserId,
                    };

                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }
        public async Task<IEnumerable<BankAddressViewModel>> GetBankAddressByBankIdAsync(short CompanyId, short UserId, int BankId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BankAddressViewModel>($"SELECT  M_BanAdd.BankId,M_BanAdd.AddressId,M_BanAdd.Address1,M_BanAdd.Address2,M_BanAdd.Address3,M_BanAdd.Address4,M_BanAdd.PhoneNo,M_BanAdd.EmailAdd,M_BanAdd.IsDefaultAdd,M_BanAdd.IsDeliveryAdd,M_BanAdd.IsFinAdd,M_BanAdd.IsSalesAdd,M_BanAdd.IsActive,M_Ban.BankCode,M_Ban.BankName,M_BanAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_BanAdd.CreateById,M_BanAdd.CreateDate,M_BanAdd.EditById,M_BanAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_BankAddress M_BanAdd INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = M_BanAdd.BankId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_BanAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_BanAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_BanAdd.EditById WHERE M_BanAdd.BankId = {BankId} AND M_BanAdd.AddressId <>0 ");

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Bank, BankId, "", "M_BankAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        //Bank Address one record by using addressId
        public async Task<BankAddressViewModel> GetBankAddressByIdAsync(short CompanyId, short UserId, int BankId, short AddressId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<BankAddressViewModel>($"SELECT  M_BanAdd.BankId,M_BanAdd.AddressId,M_BanAdd.Address1,M_BanAdd.Address2,M_BanAdd.Address3,M_BanAdd.Address4,M_BanAdd.PinCode,M_BanAdd.PhoneNo,M_BanAdd.FaxNo,M_BanAdd.WebUrl,M_BanAdd.EmailAdd,M_BanAdd.IsDefaultAdd,M_BanAdd.IsDeliveryAdd,M_BanAdd.IsFinAdd,M_BanAdd.IsSalesAdd,M_BanAdd.IsActive,M_Ban.BankCode,M_Ban.BankName,M_BanAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_BanAdd.CreateById,M_BanAdd.CreateDate,M_BanAdd.EditById,M_BanAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_BankAddress M_BanAdd INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = M_BanAdd.BankId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_BanAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_BanAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_BanAdd.EditById WHERE M_BanAdd.BankId = {BankId} And M_BanAdd.AddressId={AddressId}");

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Bank, AddressId, "", "M_BankAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveBankAddressAsync(short CompanyId, short UserId, M_BankAddress m_BankAddress)
        {
            bool IsEdit = m_BankAddress.BankId != 0 && m_BankAddress.AddressId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_BankAddress where BankId = {m_BankAddress.BankId} And Address1 = '{m_BankAddress.Address1}' And AddressId<>{m_BankAddress.AddressId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponse { Result = -1, Message = "Bank Address Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_BankAddress);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.BankId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT ISNULL((SELECT TOP 1(AddressId + 1) FROM dbo.M_BankAddress WHERE BankId = {m_BankAddress.BankId} AND (AddressId + 1) NOT IN(SELECT AddressId FROM dbo.M_BankAddress where BankId= {m_BankAddress.BankId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_BankAddress.AddressId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_BankAddress.EditDate = null;
                            m_BankAddress.EditById = null;
                            _context.Add(m_BankAddress);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Id Should not be zero" };
                    }

                    var BankToSave = _context.SaveChanges();

                    if (BankToSave > 0)
                    {
                        await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.Bank, m_BankAddress.AddressId, m_BankAddress.Address1, "M_BankAddress", IsEdit ? E_Mode.Update : E_Mode.Create, "BankAddress Save Successfully", UserId);
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Bank, m_BankAddress.AddressId, "", "M_BankAddress", E_Mode.Delete, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Bank, m_BankAddress.AddressId, "", "M_BankAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteBankAddressAsync(short CompanyId, short UserId, int BankId, short AddressId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (AddressId > 0 && BankId > 0)
                    {
                        var BankAddressToRemove = _context.M_BankAddress.Where(x => x.AddressId == AddressId && x.BankId == BankId).ExecuteDelete();

                        if (BankAddressToRemove > 0)
                        {
                            await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.Bank, AddressId, "", "M_BankAddress", E_Mode.Delete, "BankAddress Delete Successfully", UserId);
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
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Bank, AddressId, "", "M_BankAddress", E_Mode.Delete, "SQL", UserId);
                return new SqlResponse { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Bank, AddressId, "", "M_BankAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }
        public async Task<IEnumerable<BankContactViewModel>> GetBankContactByBankIdAsync(short CompanyId, short UserId, int BankId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BankContactViewModel>($"SELECT M_BanCon.BankId,M_BanCon.ContactId,M_BanCon.ContactName,M_BanCon.OtherName,M_BanCon.MobileNo,M_BanCon.OffNo,M_BanCon.FaxNo,M_BanCon.EmailAdd,M_BanCon.MessId,M_BanCon.ContactMessType,M_BanCon.IsDefault,M_BanCon.IsFinance,M_BanCon.IsSales,M_BanCon.IsActive,M_Ban.BankCode,M_Ban.BankName,M_BanCon.CreateById,M_BanCon.CreateDate,M_BanCon.EditById,M_BanCon.EditDate FROM dbo.M_BankContact M_BanCon INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = M_BanCon.BankId WHERE M_BanCon.BankId = {BankId}");

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
                    TblName = "M_BankContact",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BankContactViewModel> GetBankContactByIdAsync(short CompanyId, short UserId, int BankId, short ContactId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<BankContactViewModel>($"SELECT M_BanCon.BankId,M_BanCon.ContactId,M_BanCon.ContactName,M_BanCon.OtherName,M_BanCon.MobileNo,M_BanCon.OffNo,M_BanCon.FaxNo,M_BanCon.EmailAdd,M_BanCon.MessId,M_BanCon.ContactMessType,M_BanCon.IsDefault,M_BanCon.IsFinance,M_BanCon.IsSales,M_BanCon.IsActive,M_Ban.BankCode,M_Ban.BankName,M_BanCon.CreateById,M_BanCon.CreateDate,M_BanCon.EditById,M_BanCon.EditDate FROM dbo.M_BankContact M_BanCon INNER JOIN dbo.M_Bank M_Ban ON M_Ban.BankId = M_BanCon.BankId WHERE M_BanCon.BankId = {BankId} AND ContactId={ContactId}");

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
                    TblName = "M_BankContact",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveBankContactAsync(short CompanyId, short UserId, M_BankContact m_BankContact)
        {
            bool IsEdit = false;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (m_BankContact.BankId != 0 && m_BankContact.ContactId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_BankContact where BankId = {m_BankContact.BankId} And ContactName = '{m_BankContact.ContactName}' And ContactId<>{m_BankContact.ContactId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponse { Result = -1, Message = "Bank Contact Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_BankContact);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.BankId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT ISNULL((SELECT TOP 1(ContactId + 1) FROM dbo.M_BankContact WHERE BankId = {m_BankContact.BankId} AND (ContactId + 1) NOT IN(SELECT ContactId FROM dbo.M_BankContact where BankId= {m_BankContact.BankId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_BankContact.ContactId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_BankContact.EditDate = null;
                            m_BankContact.EditById = null;
                            _context.Add(m_BankContact);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Id Should not be zero" };
                    }

                    var BankToSave = _context.SaveChanges();

                    #region Save AuditLog

                    if (BankToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Bank,
                            DocumentId = m_BankContact.ContactId,
                            DocumentNo = "",
                            TblName = "M_Bank",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Bank Save Successfully",
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
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",
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
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> DeleteBankContactAsync(short CompanyId, short UserId, int BankId, short ContactId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (BankId > 0 && ContactId > 0)
                    {
                        var BankContactToRemove = _context.M_BankContact.Where(x => x.BankId == BankId && x.ContactId == ContactId).ExecuteDelete();

                        if (BankContactToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Bank,
                                DocumentId = BankId,
                                DocumentNo = "",
                                TblName = "M_BankContact",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "BankContact Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "BankId Should be zero" };
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
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",

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
                    TransactionId = (short)E_Master.Bank,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_BankContact",
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