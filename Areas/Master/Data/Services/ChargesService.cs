using AEMSWEB.Areas.Master.Data.IServices;
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
    public sealed class ChargesService : IChargesService
    {
        private readonly IRepository<M_Charges> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public ChargesService(IRepository<M_Charges> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<ChargesViewModelCount> GetChargesListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            ChargesViewModelCount countViewModel = new ChargesViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_Charges M_Dep WHERE (M_Dep.ChargeName LIKE '%{searchString}%' OR M_Dep.ChargeCode LIKE '%{searchString}%' OR M_Dep.Remarks LIKE '%{searchString}%') AND M_Dep.ChargeId<>0 AND M_Dep.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Charges}))");

                var result = await _repository.GetQueryAsync<ChargesViewModel>($"SELECT M_Dep.ChargeId,M_Dep.ChargeCode,M_Dep.ChargeName,M_Dep.CompanyId,M_Dep.Remarks,M_Dep.IsActive,M_Dep.CreateById,M_Dep.CreateDate,M_Dep.EditById,M_Dep.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Charges M_Dep LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Dep.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Dep.EditById WHERE (M_Dep.ChargeName LIKE '%{searchString}%' OR M_Dep.ChargeCode LIKE '%{searchString}%' OR M_Dep.Remarks LIKE '%{searchString}%') AND M_Dep.ChargeId<>0 AND M_Dep.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Charges})) ORDER BY M_Dep.ChargeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<ChargesViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Charges,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Charges",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ChargesViewModel> GetChargesByIdAsync(short CompanyId, short UserId, short ChargeId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<ChargesViewModel>($"SELECT ChargeId,ChargeCode,ChargeName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_Charges WHERE ChargeId={ChargeId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.Charges}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.Charges,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_Charges",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveChargesAsync(short CompanyId, short UserId, M_Charges m_Charges)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_Charges.ChargeId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Charges WHERE ChargeId<>@ChargeId AND ChargeCode=@ChargeCode",
                        new { m_Charges.ChargeId, m_Charges.ChargeCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Charges Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_Charges WHERE ChargeId<>@ChargeId AND ChargeName=@ChargeName",
                        new { m_Charges.ChargeId, m_Charges.ChargeName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Charges Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_Charges WHERE ChargeId=@ChargeId",
                            new { m_Charges.ChargeId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_Charges);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Charges Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (ChargeId + 1) FROM dbo.M_Charges WHERE (ChargeId + 1) NOT IN (SELECT ChargeId FROM dbo.M_Charges)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_Charges.ChargeId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_Charges);
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
                            TransactionId = (short)E_Master.Charges,
                            DocumentId = m_Charges.ChargeId,
                            DocumentNo = m_Charges.ChargeCode,
                            TblName = "M_Charges",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Charges Save Successfully",
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
                        TransactionId = (short)E_Master.Charges,
                        DocumentId = m_Charges.ChargeId,
                        DocumentNo = m_Charges.ChargeCode,
                        TblName = "M_Charges",
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

        public async Task<SqlResponse> DeleteChargesAsync(short CompanyId, short UserId, short chargeId)
        {
            string chargeNo = string.Empty;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    chargeNo = await _repository.GetQuerySingleOrDefaultAsync<string>($"SELECT ChargeCode FROM dbo.M_Charges WHERE ChargeId={chargeId}");

                    if (chargeId > 0)
                    {
                        var accountGroupToRemove = _context.M_Charges
                            .Where(x => x.ChargeId == chargeId)
                            .ExecuteDelete();


                        if (accountGroupToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.Charges,
                                DocumentId = chargeId,
                                DocumentNo = chargeNo,
                                TblName = "M_Charges",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Charges Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "ChargesId Should be zero" };
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
                    TransactionId = (short)E_Master.Charges,
                    DocumentId = chargeId,
                    DocumentNo = "",
                    TblName = "AdmUser",
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
                    TransactionId = (short)E_Master.Charges,
                    DocumentId = chargeId,
                    DocumentNo = "",
                    TblName = "M_Charges",
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