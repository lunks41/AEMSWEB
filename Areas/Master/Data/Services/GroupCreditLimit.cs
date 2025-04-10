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
    public sealed class GroupCreditLimitService : IGroupCreditLimitService
    {
        private readonly IRepository<M_GroupCreditLimit> _repository;
        private ApplicationDbContext _context;

        public GroupCreditLimitService(IRepository<M_GroupCreditLimit> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<GroupCreditLimitViewModelCount> GetGroupCreditLimitListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            GroupCreditLimitViewModelCount countViewModel = new GroupCreditLimitViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM M_GroupCreditLimit M_Grp WHERE (M_Grp.GroupCreditLimitName LIKE '%{searchString}%' OR M_Grp.Remarks LIKE '%{searchString}%') AND M_Grp.GroupCreditLimitId<>0 AND M_Grp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit}))");

                var result = await _repository.GetQueryAsync<GroupCreditLimitViewModel>($"SELECT M_Grp.GroupCreditLimitId,M_Grp.GroupCreditLimitCode,M_Grp.GroupCreditLimitName,M_Grp.CompanyId,M_Grp.Remarks,M_Grp.IsActive,M_Grp.CreateById,M_Grp.CreateDate,M_Grp.EditById,M_Grp.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_GroupCreditLimit M_Grp LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Grp.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Grp.EditById WHERE (M_Grp.GroupCreditLimitName LIKE '%{searchString}%' OR M_Grp.Remarks LIKE '%{searchString}%') AND M_Grp.GroupCreditLimitId<>0 AND M_Grp.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit})) ORDER BY M_Grp.GroupCreditLimitName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<GroupCreditLimitViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimit",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<GroupCreditLimitViewModel> GetGroupCreditLimitByIdAsync(short CompanyId, short UserId, short GroupCreditLimitId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<GroupCreditLimitViewModel>($"SELECT GroupCreditLimitId,GroupCreditLimitCode,GroupCreditLimitName,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_GroupCreditLimit WHERE GroupCreditLimitId={GroupCreditLimitId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.GroupCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_GroupCreditLimit",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveGroupCreditLimitAsync(short CompanyId, short UserId, M_GroupCreditLimit m_GroupCreditLimit)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_GroupCreditLimit.GroupCreditLimitId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit WHERE GroupCreditLimitId<>@GroupCreditLimitId AND GroupCreditLimitCode=@GroupCreditLimitCode",
                        new { m_GroupCreditLimit.GroupCreditLimitId, m_GroupCreditLimit.GroupCreditLimitCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimit Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit WHERE GroupCreditLimitId<>@GroupCreditLimitId AND GroupCreditLimitName=@GroupCreditLimitName",
                        new { m_GroupCreditLimit.GroupCreditLimitId, m_GroupCreditLimit.GroupCreditLimitName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimit Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit WHERE GroupCreditLimitId=@GroupCreditLimitId",
                            new { m_GroupCreditLimit.GroupCreditLimitId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_GroupCreditLimit);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "GroupCreditLimit Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT ISNULL((SELECT TOP 1 (GroupCreditLimitId + 1) FROM dbo.M_GroupCreditLimit WHERE (GroupCreditLimitId + 1) NOT IN (SELECT GroupCreditLimitId FROM dbo.M_GroupCreditLimit)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_GroupCreditLimit.GroupCreditLimitId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_GroupCreditLimit);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
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
                            DocumentId = m_GroupCreditLimit.GroupCreditLimitId,
                            DocumentNo = m_GroupCreditLimit.GroupCreditLimitCode,
                            TblName = "M_GroupCreditLimit",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "GroupCreditLimit Save Successfully",
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.GroupCreditLimit,
                        DocumentId = m_GroupCreditLimit.GroupCreditLimitId,
                        DocumentNo = m_GroupCreditLimit.GroupCreditLimitCode,
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

        public async Task<SqlResponce> DeleteGroupCreditLimitAsync(short CompanyId, short UserId, M_GroupCreditLimit GroupCreditLimit)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (GroupCreditLimit.GroupCreditLimitId > 0)
                    {
                        var GroupCreditLimitToRemove = _context.M_GroupCreditLimit.Where(x => x.GroupCreditLimitId == GroupCreditLimit.GroupCreditLimitId).ExecuteDelete();

                        if (GroupCreditLimitToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GroupCreditLimit,
                                DocumentId = GroupCreditLimit.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimit",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "GroupCreditLimit Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "GroupCreditLimitId Should be zero" };
                    }
                    return new SqlResponce();
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.GroupCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_GroupCreditLimit",
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