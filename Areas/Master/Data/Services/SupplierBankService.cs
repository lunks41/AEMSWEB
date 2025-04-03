using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.Models;
using AMESWEB.Models.Masters;
using AMESWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AMESWEB.Areas.Master.Data.Services
{
    public sealed class SupplierBankService : ISupplierBankService
    {
        private readonly IRepository<M_SupplierBank> _repository;
        private ApplicationDbContext _context;

        public SupplierBankService(IRepository<M_SupplierBank> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SupplierBankViewModel>> GetSupplierBankBySupplierIdAsync(short CompanyId, short UserId, int SupplierId)
        {
            try
            {
                var result = await _repository.GetQueryAsync<SupplierBankViewModel>($"SELECT M_SupBank.SupplierId,M_SupBank.SupplierBankId,M_SupBank.BranchName,M_SupBank.AccountNo,M_SupBank.SwiftCode,M_SupBank.OtherCode,M_SupBank.Address1,M_SupBank.Address2,M_SupBank.Address3,M_SupBank.Address4,M_SupBank.PinCode,M_SupBank.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_SupBank.Remarks1,M_SupBank.Remarks2,M_SupBank.IsDefault,M_Sup.SupplierCode,M_Sup.SupplierName,M_SupBank.CreateById,M_SupBank.CreateDate,M_SupBank.EditById,M_SupBank.EditDate FROM dbo.M_SupplierBank M_SupBank INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = M_SupBank.SupplierId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId= M_SupBank.CountryId WHERE M_SupCon.SupplierId = {SupplierId}");

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
                    TblName = "M_SupplierBank",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SupplierBankViewModel> GetSupplierBankByIdAsync(short CompanyId, short UserId, int SupplierId, short SupplierBankId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<SupplierBankViewModel>($"SELECT M_SupBank.SupplierId,M_SupBank.SupplierBankId,M_SupBank.BranchName,M_SupBank.AccountNo,M_SupBank.SwiftCode,M_SupBank.OtherCode,M_SupBank.Address1,M_SupBank.Address2,M_SupBank.Address3,M_SupBank.Address4,M_SupBank.PinCode,M_SupBank.CountryId,M_Cou.CountryCode,M_Cou.CountryName,M_SupBank.Remarks1,M_SupBank.Remarks2,M_SupBank.IsDefault,M_Sup.SupplierCode,M_Sup.SupplierName,M_SupBank.CreateById,M_SupBank.CreateDate,M_SupBank.EditById,M_SupBank.EditDate FROM dbo.M_SupplierBank M_SupBank INNER JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = M_SupBank.SupplierId INNER JOIN dbo.M_Country M_Cou ON M_Cou.CountryId= M_SupBank.CountryId WHERE M_SupCon.SupplierId = {SupplierId} AND M_SupCon.SupplierBankId={SupplierBankId}");

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
                    TblName = "M_SupplierBank",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveSupplierBankAsync(short CompanyId, short UserId, M_SupplierBank m_SupplierBank)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_SupplierBank.SupplierId != 0 && m_SupplierBank.SupplierBankId != 0)
                        IsEdit = true;
                    if (IsEdit)
                    {
                        var entityHead = _context.Update(m_SupplierBank);
                        entityHead.Property(b => b.CreateById).IsModified = false;
                        entityHead.Property(b => b.SupplierId).IsModified = false;
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT ISNULL((SELECT TOP 1(SupplierBankId + 1) FROM dbo.M_SupplierBank WHERE SupplierId = {m_SupplierBank.SupplierId} AND (SupplierBankId + 1) NOT IN(SELECT SupplierBankId FROM dbo.M_SupplierBank where SupplierId={m_SupplierBank.SupplierId})),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_SupplierBank.SupplierBankId = Convert.ToInt16(sqlMissingResponse.NextId);

                            m_SupplierBank.EditDate = null;
                            m_SupplierBank.EditById = null;
                            _context.Add(m_SupplierBank);
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
                            DocumentId = m_SupplierBank.SupplierBankId,
                            DocumentNo = m_SupplierBank.BranchName,
                            TblName = "M_SupplierBank",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Supplier Bank Save Successfully",
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
                        TblName = "M_SupplierBank",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException?.Message,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw new Exception(ex.ToString());
                }
            }
        }

        public async Task<SqlResponse> DeleteSupplierBankAsync(short CompanyId, short UserId, int SupplierId, short SupplierBankId)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (SupplierId > 0 && SupplierBankId > 0)
                    {
                        var SupplierBankToRemove = _context.M_SupplierBank.Where(x => x.SupplierId == SupplierId && x.SupplierBankId == SupplierBankId).ExecuteDelete();

                        if (SupplierBankToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Supplier,
                                DocumentId = SupplierId,
                                DocumentNo = SupplierBankId.ToString(),
                                TblName = "M_SupplierBank",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Supplier Bank Delete Successfully",
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
                        TblName = "M_SupplierBank",
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
    }
}