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
    public sealed class CustomerAddressService : ICustomerAddressService
    {
        private readonly IRepository<M_CustomerAddress> _repository;
        private ApplicationDbContext _context;
        private readonly ILogService _logService;

        public CustomerAddressService(IRepository<M_CustomerAddress> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context;
            _logService = logService;
        }

        //Customer Address List
        public async Task<IEnumerable<CustomerAddressViewModel>> GetCustomerAddressByCustomerIdAsync(short CompanyId, short UserId, int CustomerId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerAddressViewModel>($"SELECT  M_CusAdd.CustomerId,M_CusAdd.AddressId,M_CusAdd.Address1,M_CusAdd.Address2,M_CusAdd.Address3,M_CusAdd.Address4,M_CusAdd.PhoneNo,M_CusAdd.EmailAdd,M_CusAdd.IsDefaultAdd,M_CusAdd.IsDeliveryAdd,M_CusAdd.IsFinAdd,M_CusAdd.IsSalesAdd,M_CusAdd.IsActive,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_CusAdd.CreateById,M_CusAdd.CreateDate,M_CusAdd.EditById,M_CusAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CustomerAddress M_CusAdd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusAdd.CustomerId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_CusAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_CusAdd.EditById WHERE M_CusAdd.CustomerId = {CustomerId} AND M_CusAdd.AddressId <>0 ");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<CustomerAddressViewModel>($"SELECT  M_CusAdd.CustomerId,M_CusAdd.AddressId,M_CusAdd.Address1,M_CusAdd.Address2,M_CusAdd.Address3,M_CusAdd.Address4,M_CusAdd.PinCode,M_CusAdd.PhoneNo,M_CusAdd.FaxNo,M_CusAdd.WebUrl,M_CusAdd.EmailAdd,M_CusAdd.IsDefaultAdd,M_CusAdd.IsDeliveryAdd,M_CusAdd.IsFinAdd,M_CusAdd.IsSalesAdd,M_CusAdd.IsActive,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusAdd.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_CusAdd.CreateById,M_CusAdd.CreateDate,M_CusAdd.EditById,M_CusAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_CustomerAddress M_CusAdd INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusAdd.CustomerId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_CusAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_CusAdd.EditById WHERE M_CusAdd.CustomerId = {CustomerId} And M_CusAdd.AddressId={AddressId}");

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
    }
}