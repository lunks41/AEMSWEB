using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.Helper;
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
    public sealed class CustomerContactService : ICustomerContactService
    {
        private readonly IRepository<M_CustomerContact> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public CustomerContactService(IRepository<M_CustomerContact> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<IEnumerable<CustomerContactViewModel>> GetCustomerContactByCustomerIdAsync(short CompanyId, short UserId, int CustomerId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<CustomerContactViewModel>($"SELECT M_CusCon.CustomerId,M_CusCon.ContactId,M_CusCon.ContactName,M_CusCon.OtherName,M_CusCon.MobileNo,M_CusCon.OffNo,M_CusCon.FaxNo,M_CusCon.EmailAdd,M_CusCon.MessId,M_CusCon.ContactMessType,M_CusCon.IsDefault,M_CusCon.IsFinance,M_CusCon.IsSales,M_CusCon.IsActive,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusCon.CreateById,M_CusCon.CreateDate,M_CusCon.EditById,M_CusCon.EditDate FROM dbo.M_CustomerContact M_CusCon INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusCon.CustomerId WHERE M_CusCon.CustomerId = {CustomerId}");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<CustomerContactViewModel>($"SELECT M_CusCon.CustomerId,M_CusCon.ContactId,M_CusCon.ContactName,M_CusCon.OtherName,M_CusCon.MobileNo,M_CusCon.OffNo,M_CusCon.FaxNo,M_CusCon.EmailAdd,M_CusCon.MessId,M_CusCon.ContactMessType,M_CusCon.IsDefault,M_CusCon.IsFinance,M_CusCon.IsSales,M_CusCon.IsActive,M_Cus.CustomerCode,M_Cus.CustomerName,M_CusCon.CreateById,M_CusCon.CreateDate,M_CusCon.EditById,M_CusCon.EditDate FROM dbo.M_CustomerContact M_CusCon INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_CusCon.CustomerId WHERE M_CusCon.CustomerId = {CustomerId} AND ContactId={ContactId}");

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