using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class SupplierAddressService : ISupplierAddressService
    {
        private readonly IRepository<M_SupplierAddress> _repository;
        private ApplicationDbContext _context;

        public SupplierAddressService(IRepository<M_SupplierAddress> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SupplierAddressViewModel>> GetSupplierAddressBySupplierIdAsync(short CompanyId, short UserId, int SupplierId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierAddressViewModel>($"SELECT M_SupAdd.SupplierId,M_SupAdd.AddressId,M_SupAdd.Address1,M_SupAdd.Address2,M_SupAdd.Address3,M_SupAdd.Address4,M_SupAdd.PhoneNo,M_SupAdd.FaxNo,M_SupAdd.PinCode,M_SupAdd.EmailAdd,M_SupAdd.WebUrl,M_SupAdd.IsDefaultAdd,M_SupAdd.IsDeliveryAdd,M_SupAdd.IsFinAdd,M_SupAdd.IsSalesAdd,M_SupAdd.IsActive,M_Sup.SupplierCode,M_Sup.SupplierName,M_Cou.CountryCode,M_Cou.CountryName,M_SupAdd.CreateById,M_SupAdd.CreateDate,M_SupAdd.EditById,M_SupAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_SupplierAddress M_SupAdd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = M_SupAdd.SupplierId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_SupAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_SupAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_SupAdd.EditById WHERE M_SupAdd.SupplierId = {SupplierId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        //Supplier Address one record by using addressId
        public async Task<SupplierAddressViewModel> GetSupplierAddressByIdAsync(short CompanyId, short UserId, int SupplierId, short AddressId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<SupplierAddressViewModel>($"SELECT M_SupAdd.SupplierId,M_SupAdd.AddressId,M_SupAdd.Address1,M_SupAdd.Address2,M_SupAdd.Address3,M_SupAdd.Address4,M_SupAdd.PhoneNo,M_SupAdd.FaxNo,M_SupAdd.PinCode,M_SupAdd.EmailAdd,M_SupAdd.WebUrl,M_SupAdd.IsDefaultAdd,M_SupAdd.IsDeliveryAdd,M_SupAdd.IsFinAdd,M_SupAdd.IsSalesAdd,M_SupAdd.IsActive,M_Sup.SupplierCode,M_Sup.SupplierName,M_Cou.CountryCode,M_Cou.CountryName,M_SupAdd.CreateById,M_SupAdd.CreateDate,M_SupAdd.EditById,M_SupAdd.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_SupplierAddress M_SupAdd INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = M_SupAdd.SupplierId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId = M_SupAdd.CountryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_SupAdd.CreateById LEFT JOIN AdmUser Usr1 ON Usr1.UserId = M_SupAdd.EditById WHERE M_SupAdd.SupplierId = {SupplierId} And M_SupAdd.AddressId={AddressId}");

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
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveSupplierAddressAsync(short CompanyId, short UserId, M_SupplierAddress m_SupplierAddress)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_SupplierAddress.SupplierId != 0 && m_SupplierAddress.AddressId != 0)
                    {
                        IsEdit = true;
                    }
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

                    #region Save AuditLog

                    if (SupplierToSave > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.Supplier,
                            DocumentId = m_SupplierAddress.AddressId,
                            DocumentNo = m_SupplierAddress.Address1,
                            TblName = "M_SupplierAddress",
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
                        TblName = "M_SupplierAddress",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponse> DeleteSupplierAddressAsync(short CompanyId, short UserId, int SupplierId, short AddressId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (AddressId > 0 && SupplierId > 0)
                    {
                        var SupplierAddressToRemove = _context.M_SupplierAddress.Where(x => x.AddressId == AddressId && x.SupplierId == SupplierId).ExecuteDelete();

                        if (SupplierAddressToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Supplier,
                                DocumentId = AddressId,
                                DocumentNo = "",
                                TblName = "M_SupplierAddress",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "SupplierAddress Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "AddressId Should be zero" };
                    }
                    return new SqlResponse();
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
                        TblName = "M_SupplierAddress",
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
}