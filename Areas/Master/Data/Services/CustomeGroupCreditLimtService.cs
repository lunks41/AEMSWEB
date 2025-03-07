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
    public sealed class CustomerGroupCreditLimitService : ICustomerGroupCreditLimitService
    {
        private readonly IRepository<M_CustomerGroupCreditLimit> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public CustomerGroupCreditLimitService(IRepository<M_CustomerGroupCreditLimit> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<CustomerGroupCreditLimitViewModelCount> GetCustomerGroupCreditLimitListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            CustomerGroupCreditLimitViewModelCount countViewModel = new CustomerGroupCreditLimitViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_CustomerGroupCreditLimit M_CusGrp WHERE (M_CusGrp.GroupCreditLimitName LIKE '%{searchString}%' OR M_CusGrp.GroupCreditLimitCode LIKE '%{searchString}%' OR M_CusGrp.Remarks LIKE '%{searchString}%') AND M_CusGrp.GroupCreditLimitId<>0 AND M_CusGrp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CustomerGroupCreditLimit}))");

                var result = await _repository.GetQueryAsync<CustomerGroupCreditLimitViewModel>($"SELECT M_CusGrp.GroupCreditLimitId,M_CusGrp.GroupCreditLimitCode,M_CusGrp.GroupCreditLimitName,M_CusGrp.CompanyId,M_CusGrp.Remarks,M_CusGrp.IsActive,M_CusGrp.CreateById,M_CusGrp.CreateDate,M_CusGrp.EditById,M_CusGrp.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_CustomerGroupCreditLimit M_CusGrp LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_CusGrp.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_CusGrp.EditById WHERE (M_CusGrp.GroupCreditLimitName LIKE '%{searchString}%' OR M_CusGrp.GroupCreditLimitCode LIKE '%{searchString}%' OR M_CusGrp.Remarks LIKE '%{searchString}%') AND M_CusGrp.GroupCreditLimitId<>0 AND M_CusGrp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CustomerGroupCreditLimit})) ORDER BY M_CusGrp.GroupCreditLimitName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result == null ? null : result.ToList();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CustomerGroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerGroupCreditLimit",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_CustomerGroupCreditLimit> GetCustomerGroupCreditLimitByIdAsync(short CompanyId, short UserId, short GroupCreditLimitId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_CustomerGroupCreditLimit>($"SELECT GroupCreditLimitId,GroupCreditLimitCode,GroupCreditLimitName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_CustomerGroupCreditLimit WHERE GroupCreditLimitId={GroupCreditLimitId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CustomerGroupCreditLimit}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CustomerGroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerGroupCreditLimit",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveCustomerGroupCreditLimitAsync(short CompanyId, short UserId, M_CustomerGroupCreditLimit m_CustomerGroupCreditLimit)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_CustomerGroupCreditLimit.GroupCreditLimitId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit WHERE GroupCreditLimitId<>@GroupCreditLimitId AND GroupCreditLimitCode=@GroupCreditLimitCode",
                        new { m_CustomerGroupCreditLimit.GroupCreditLimitId, m_CustomerGroupCreditLimit.GroupCreditLimitCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Group Credit Limit Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit WHERE GroupCreditLimitId<>@GroupCreditLimitId AND GroupCreditLimitName=@GroupCreditLimitName",
                        new { m_CustomerGroupCreditLimit.GroupCreditLimitId, m_CustomerGroupCreditLimit.GroupCreditLimitName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "Group Credit Limit Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit WHERE GroupCreditLimitId=@GroupCreditLimitId",
                            new { m_CustomerGroupCreditLimit.GroupCreditLimitId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_CustomerGroupCreditLimit);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "Group Credit Limit Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (GroupCreditLimitId + 1) FROM dbo.M_GroupCreditLimit WHERE (GroupCreditLimitId + 1) NOT IN (SELECT GroupCreditLimitId FROM dbo.M_GroupCreditLimit)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_CustomerGroupCreditLimit.GroupCreditLimitId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_CustomerGroupCreditLimit);
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
                            TransactionId = (short)E_Master.GroupCreditLimit,
                            DocumentId = m_CustomerGroupCreditLimit.GroupCreditLimitId,
                            DocumentNo = m_CustomerGroupCreditLimit.GroupCreditLimitCode,
                            TblName = "M_GroupCreditLimit",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Group Credit Limit Save Successfully",
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
                        TransactionId = (short)E_Master.CustomerGroupCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CustomerGroupCreditLimit",
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
                        TransactionId = (short)E_Master.GroupCreditLimit,
                        DocumentId = m_CustomerGroupCreditLimit.GroupCreditLimitId,
                        DocumentNo = m_CustomerGroupCreditLimit.GroupCreditLimitCode,
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

        public async Task<SqlResponse> DeleteCustomerGroupCreditLimitAsync(short CompanyId, short UserId, M_CustomerGroupCreditLimit CustomerGroupCreditLimit)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (CustomerGroupCreditLimit.GroupCreditLimitId > 0)
                    {
                        var CustomerGroupCreditLimitToRemove = await _context.M_CustomerGroupCreditLimit.Where(x => x.GroupCreditLimitId == CustomerGroupCreditLimit.GroupCreditLimitId).ExecuteDeleteAsync();

                        if (CustomerGroupCreditLimitToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CustomerGroupCreditLimit,
                                DocumentId = CustomerGroupCreditLimit.GroupCreditLimitId,
                                DocumentNo = CustomerGroupCreditLimit.GroupCreditLimitCode,
                                TblName = "M_CustomerGroupCreditLimit",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CustomerGroupCreditLimit Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "GroupCreditLimitId Should be zero" };
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
                        TransactionId = (short)E_Master.CustomerGroupCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        ModeId = (short)E_Mode.Delete,
                        TblName = "M_CustomerGroupCreditLimit",
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
                        TransactionId = (short)E_Master.CustomerGroupCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CustomerGroupCreditLimit",
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