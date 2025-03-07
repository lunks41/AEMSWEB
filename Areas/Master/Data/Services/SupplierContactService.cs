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
    public sealed class SupplierContactService : ISupplierContactService
    {
        private readonly IRepository<M_SupplierContact> _repository;
        private ApplicationDbContext _context;

        public SupplierContactService(IRepository<M_SupplierContact> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SupplierContactViewModel>> GetSupplierContactBySupplierIdAsync(short CompanyId, short UserId, int SupplierId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierContactViewModel>($"SELECT M_SupCon.ContactId,M_SupCon.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,M_SupCon.ContactName,M_SupCon.OtherName,M_SupCon.MobileNo,M_SupCon.OffNo,M_SupCon.FaxNo,M_SupCon.EmailAdd,M_SupCon.MessId,M_SupCon.ContactMessType,M_SupCon.IsDefault,M_SupCon.IsFinance,M_SupCon.IsSales,M_SupCon.IsActive,M_SupCon.CreateById,M_SupCon.CreateDate,M_SupCon.EditById,M_SupCon.EditDate FROM dbo.M_SupplierContact M_SupCon INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = M_SupCon.SupplierId WHERE M_SupCon.SupplierId = {SupplierId}");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<SupplierContactViewModel>($"SELECT M_SupCon.ContactId,M_SupCon.SupplierId,M_Sup.SupplierCode,M_Sup.SupplierName,M_SupCon.ContactName,M_SupCon.OtherName,M_SupCon.MobileNo,M_SupCon.OffNo,M_SupCon.FaxNo,M_SupCon.EmailAdd,M_SupCon.MessId,M_SupCon.ContactMessType,M_SupCon.IsDefault,M_SupCon.IsFinance,M_SupCon.IsSales,M_SupCon.IsActive,M_SupCon.CreateById,M_SupCon.CreateDate,M_SupCon.EditById,M_SupCon.EditDate FROM dbo.M_SupplierContact M_SupCon INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = M_SupCon.SupplierId WHERE M_SupCon.SupplierId = {SupplierId} AND ContactId={ContactId}");

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
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
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
        }

        public async Task<SqlResponse> DeleteSupplierContactAsync(short CompanyId, short UserId, int SupplierId, short ContactId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
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
        }
    }
}