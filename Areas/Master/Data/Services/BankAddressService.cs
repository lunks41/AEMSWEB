using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
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
    public sealed class BankAddressService : IBankAddressService
    {
        private readonly IRepository<M_BankAddress> _repository;
        private ApplicationDbContext _context;
        private readonly ILogService _logService;

        public BankAddressService(IRepository<M_BankAddress> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context;
            _logService = logService;
        }

        //Bank Address List
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
    }
}