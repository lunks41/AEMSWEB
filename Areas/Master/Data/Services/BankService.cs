using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Areas.Master.Models;
using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.Helpers;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Master.Data.Services
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

        #region CUSTOMER

        public async Task<BankViewModelCount> GetBankListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            BankViewModelCount countViewModel = new BankViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_Bank M_Bnk INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Bnk.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Bnk.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Bnk.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Bnk.EditById WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Bnk.BankName LIKE '%{searchString}%' OR M_Bnk.BankCode LIKE '%{searchString}%' OR M_Bnk.Remarks1 LIKE '%{searchString}%') AND M_Bnk.BankId<>0 AND M_Bnk.CompanyId ={CompanyId}");

                var result = await _repository.GetQueryAsync<BankViewModel>($"SELECT M_Bnk.BankId,M_Bnk.CompanyId,M_Bnk.BankCode,M_Bnk.BankName,M_Bnk.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Bnk.AccountNo,M_Bnk.SwiftCode,M_Bnk.Remarks1,M_Bnk.Remarks2,M_Bnk.Remarks3,M_Bnk.GLId,M_Chr.GLCode,M_Chr.GLName,M_Bnk.IsActive,M_Bnk.IsOwnBank,M_Bnk.CreateById,M_Bnk.CreateDate,M_Bnk.EditById,M_Bnk.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Bank M_Bnk INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Bnk.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Bnk.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Bnk.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Bnk.EditById WHERE (M_Cur.CurrencyName LIKE '%{searchString}%' OR M_Cur.CurrencyCode LIKE '%{searchString}%' OR M_Bnk.BankName LIKE '%{searchString}%' OR M_Bnk.BankCode LIKE '%{searchString}%' OR M_Bnk.Remarks1 LIKE '%{searchString}%') AND M_Bnk.BankId<>0 AND M_Bnk.CompanyId ={CompanyId} ORDER BY M_Bnk.BankName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
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
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<BankViewModel> GetBankByIdAsync(short CompanyId, short UserId, int BankId, string BankCode, string BankName)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<BankViewModel>($"SELECT M_Bnk.BankId,M_Bnk.CompanyId,M_Bnk.BankCode,M_Bnk.BankName,M_Bnk.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Bnk.AccountNo,M_Bnk.SwiftCode,M_Bnk.Remarks1,M_Bnk.Remarks2,M_Bnk.Remarks3,M_Bnk.GLId,M_Chr.GLCode,M_Chr.GLName,M_Bnk.IsActive,M_Bnk.IsOwnBank,M_Bnk.CreateById,M_Bnk.CreateDate,M_Bnk.EditById,M_Bnk.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Bank M_Bnk INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Bnk.CurrencyId INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Bnk.GLId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Bnk.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Bnk.EditById WHERE (M_Bnk.BankId={BankId} OR {BankId}=0) AND (M_Bnk.BankCode='{BankCode}' OR '{BankCode}'='{string.Empty}') AND (M_Bnk.BankName='{BankName}' OR '{BankName}'='{string.Empty}') AND M_Bnk.CompanyId ={CompanyId}");

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

        public async Task<SqlResponce> SaveBankAsync(short CompanyId, short UserId, M_Bank m_Bank)
        {
            bool IsEdit = m_Bank.BankId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Bank WHERE BankId <>{m_Bank.BankId} AND BankCode='{m_Bank.BankCode}' AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Bank}))");

                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "Bank Code exists" };

                    //var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>( $"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Bank WHERE BankId <>{m_Bank.BankId} AND BankName='{m_Bank.BankName}' AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Bank}))");

                    //if ((nameExist?.IsExist ?? 0) > 0)
                    //    return new SqlResponce { Result = -1, Message = "Bank Name exists" };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT TOP (1) 1 AS IsExist FROM dbo.M_Bank WHERE BankId<>0 AND BankId={m_Bank.BankId} AND CompanyId IN (SELECT DISTINCT CompanyId FROM dbo.Fn_Adm_GetShareCompany ({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Bank}))");

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Bank);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Bank Not Found" };
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>("SELECT ISNULL((SELECT TOP 1 (BankId + 1) FROM dbo.M_Bank WHERE (BankId + 1) NOT IN (SELECT BankId FROM dbo.M_Bank)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Bank.BankId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_Bank.EditDate = null;
                            m_Bank.EditById = null;
                            _context.Add(m_Bank);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "BankId Should not be zero" };
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
                            return new SqlResponce { Result = m_Bank.BankId, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponce();
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
                    DocumentNo = m_Bank.BankCode,
                    TblName = "M_Bank",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentNo = m_Bank.BankCode,
                    TblName = "M_Bank",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> DeleteBankAsync(short CompanyId, short UserId, int BankId)
        {
            string BankCode = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    BankCode = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT BankCode FROM dbo.M_Bank WHERE BankId={BankId}");

                    if (BankId > 0 && BankCode != null)
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
                                DocumentNo = BankCode,
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
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "BankId Should be zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = BankId,
                    DocumentNo = BankCode,
                    TblName = "M_Bank",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = BankId,
                    DocumentNo = BankCode,
                    TblName = "M_Bank",
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

        public async Task<IEnumerable<BankAddressViewModel>> GetBankAddressByBankIdAsync(short CompanyId, short UserId, int BankId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BankAddressViewModel>($"SELECT  M_CusAdd.BankId,M_Bnk.BankCode,M_Bnk.BankName,M_CusAdd.AddressId,M_CusAdd.Address1,M_CusAdd.Address2,M_CusAdd.Address3,M_CusAdd.Address4,M_CusAdd.PinCode,M_CusAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName  ,M_CusAdd.PhoneNo,M_CusAdd.FaxNo,M_CusAdd.EmailAdd,M_CusAdd.WebUrl,M_CusAdd.IsDefaultAdd,M_CusAdd.IsDeliveryAdd,M_CusAdd.IsFinAdd,M_CusAdd.IsSalesAdd,M_CusAdd.IsActive,M_CusAdd.CreateById,M_CusAdd.CreateDate,M_CusAdd.EditById,M_CusAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_BankAddress M_CusAdd INNER JOIN dbo.M_Bank M_Cus ON M_Bnk.BankId = M_CusAdd.BankId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_CusAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_CusAdd.EditById WHERE M_CusAdd.BankId = {BankId} AND M_CusAdd.AddressId <>0 ");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<BankAddressViewModel>($"SELECT  M_CusAdd.BankId,M_Bnk.BankCode,M_Bnk.BankName,M_CusAdd.AddressId,M_CusAdd.Address1,M_CusAdd.Address2,M_CusAdd.Address3,M_CusAdd.Address4,M_CusAdd.PinCode,M_CusAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName  ,M_CusAdd.PhoneNo,M_CusAdd.FaxNo,M_CusAdd.EmailAdd,M_CusAdd.WebUrl,M_CusAdd.IsDefaultAdd,M_CusAdd.IsDeliveryAdd,M_CusAdd.IsFinAdd,M_CusAdd.IsSalesAdd,M_CusAdd.IsActive,M_CusAdd.CreateById,M_CusAdd.CreateDate,M_CusAdd.EditById,M_CusAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_BankAddress M_CusAdd INNER JOIN dbo.M_Bank M_Cus ON M_Bnk.BankId = M_CusAdd.BankId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_CusAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_CusAdd.EditById WHERE M_CusAdd.BankId = {BankId} And M_CusAdd.AddressId={AddressId}");

                return result;
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Bank, AddressId, "", "M_BankAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveBankAddressAsync(short CompanyId, short UserId, M_BankAddress m_BankAddress)
        {
            bool IsEdit = m_BankAddress.BankId != 0 && m_BankAddress.AddressId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_BankAddress where BankId = {m_BankAddress.BankId} And Address1 = '{m_BankAddress.Address1}' And AddressId<>{m_BankAddress.AddressId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponce { Result = -1, Message = "Bank Address Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_BankAddress);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.BankId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT ISNULL((SELECT TOP 1(AddressId + 1) FROM dbo.M_BankAddress WHERE BankId = {m_BankAddress.BankId} AND (AddressId + 1) NOT IN(SELECT AddressId FROM dbo.M_BankAddress where BankId= {m_BankAddress.BankId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_BankAddress.AddressId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_BankAddress.EditDate = null;
                            m_BankAddress.EditById = null;
                            _context.Add(m_BankAddress);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Id Should not be zero" };
                    }

                    var BankToSave = _context.SaveChanges();

                    if (BankToSave > 0)
                    {
                        await _logService.SaveAuditLogAsync(CompanyId, E_Modules.Master, E_Master.Bank, m_BankAddress.AddressId, m_BankAddress.Address1, "M_BankAddress", IsEdit ? E_Mode.Update : E_Mode.Create, "BankAddress Save Successfully", UserId);
                        TScope.Complete();
                        return new SqlResponce { Result = 1, Message = "Save Successfully" };
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "Save Failed" };
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Bank, m_BankAddress.AddressId, "", "M_BankAddress", E_Mode.Delete, "SQL", UserId);
                return new SqlResponce { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Bank, m_BankAddress.AddressId, "", "M_BankAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> DeleteBankAddressAsync(short CompanyId, short UserId, int BankId, short AddressId)
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
                            return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "AddressId Should be zero" };
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                await _logService.LogErrorAsync(sqlEx, CompanyId, E_Modules.Master, E_Master.Bank, AddressId, "", "M_BankAddress", E_Mode.Delete, "SQL", UserId);
                return new SqlResponce { Result = -1, Message = SqlErrorHelper.GetErrorMessage(sqlEx.Number) };
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(ex, CompanyId, E_Modules.Master, E_Master.Bank, AddressId, "", "M_BankAddress", E_Mode.Delete, "General", UserId);
                throw new Exception(ex.ToString());
            }
        }

        #endregion ADDRESS

        #region CONTACT

        public async Task<IEnumerable<BankContactViewModel>> GetBankContactByBankIdAsync(short CompanyId, short UserId, int BankId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<BankContactViewModel>($"SELECT M_CusCon.BankId,M_CusCon.ContactId,M_CusCon.ContactName,M_CusCon.OtherName,M_CusCon.MobileNo,M_CusCon.OffNo,M_CusCon.FaxNo,M_CusCon.EmailAdd,M_CusCon.MessId,M_CusCon.ContactMessType,M_CusCon.IsDefault,M_CusCon.IsFinance,M_CusCon.IsSales,M_CusCon.IsActive,M_Bnk.BankCode,M_Bnk.BankName,M_CusCon.CreateById,M_CusCon.CreateDate,M_CusCon.EditById,M_CusCon.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_BankContact M_CusCon INNER JOIN dbo.M_Bank M_Cus ON M_Bnk.BankId = M_CusCon.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusCon.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CusCon.EditById WHERE M_CusCon.BankId = {BankId}");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<BankContactViewModel>($"SELECT M_CusCon.BankId,M_CusCon.ContactId,M_CusCon.ContactName,M_CusCon.OtherName,M_CusCon.MobileNo,M_CusCon.OffNo,M_CusCon.FaxNo,M_CusCon.EmailAdd,M_CusCon.MessId,M_CusCon.ContactMessType,M_CusCon.IsDefault,M_CusCon.IsFinance,M_CusCon.IsSales,M_CusCon.IsActive,M_Bnk.BankCode,M_Bnk.BankName,M_CusCon.CreateById,M_CusCon.CreateDate,M_CusCon.EditById,M_CusCon.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_BankContact M_CusCon INNER JOIN dbo.M_Bank M_Cus ON M_Bnk.BankId = M_CusCon.BankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusCon.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CusCon.EditById WHERE M_CusCon.BankId = {BankId} AND ContactId={ContactId}");

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

        public async Task<SqlResponce> SaveBankContactAsync(short CompanyId, short UserId, M_BankContact m_BankContact)
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
                        var DataExist = await _repository.GetQueryAsync<SqlResponceIds>($"SELECT 1 AS IsExist FROM dbo.M_BankContact where BankId = {m_BankContact.BankId} And ContactName = '{m_BankContact.ContactName}' And ContactId<>{m_BankContact.ContactId}");

                        if (DataExist.Count() > 0 && (DataExist.ToList()[0].IsExist == 1 || DataExist.ToList()[0].IsExist == 2))
                            return new SqlResponce { Result = -1, Message = "Bank Contact Name Exist" };
                    }
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_BankContact);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.BankId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT ISNULL((SELECT TOP 1(ContactId + 1) FROM dbo.M_BankContact WHERE BankId = {m_BankContact.BankId} AND (ContactId + 1) NOT IN(SELECT ContactId FROM dbo.M_BankContact where BankId= {m_BankContact.BankId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_BankContact.ContactId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_BankContact.EditDate = null;
                            m_BankContact.EditById = null;
                            _context.Add(m_BankContact);
                        }
                        else
                            return new SqlResponce { Result = -1, Message = "Id Should not be zero" };
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
                            return new SqlResponce { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = 1, Message = "Save Failed" };
                    }

                    #endregion Save AuditLog

                    return new SqlResponce();
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

                return new SqlResponce
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

        public async Task<SqlResponce> DeleteBankContactAsync(short CompanyId, short UserId, int BankId, short ContactId)
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
                                return new SqlResponce { Result = 1, Message = "Delete Successfully" };
                            }
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Delete Failed" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = -1, Message = "BankId Should be zero" };
                    }
                    return new SqlResponce();
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

                return new SqlResponce
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

        #endregion CONTACT
    }
}