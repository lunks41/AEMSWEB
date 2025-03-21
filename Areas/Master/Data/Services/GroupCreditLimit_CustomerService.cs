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
    public sealed class GroupCreditLimit_CustomerService : IGroupCreditLimit_CustomerService
    {
        private readonly IRepository<M_GroupCreditLimit_Customer> _repository;
        private ApplicationDbContext _context;

        public GroupCreditLimit_CustomerService(IRepository<M_GroupCreditLimit_Customer> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<GroupCreditLimit_CustomerViewModelCount> GetGroupCreditLimit_CustomerListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            GroupCreditLimit_CustomerViewModelCount countViewModel = new GroupCreditLimit_CustomerViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_GroupCreditLimit_Customer M_Grc INNER JOIN M_GroupCreditLimit M_Grp ON M_Grp.GroupCreditLimitId = M_Grc.GroupCreditLimitId INNER JOIN M_Customer M_Cus ON M_Cus.CustomerId = M_Grc.CustomerId WHERE (M_Grp.GroupCreditLimitName LIKE '%{searchString}%' OR M_Grp.GroupCreditLimitCode LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%') AND M_Grc.GroupCreditLimitId<>0 AND M_Grc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit_Customer}))");

                var result = await _repository.GetQueryAsync<GroupCreditLimit_CustomerViewModel>($"SELECT M_Grc.GroupCreditLimitId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Grc.CompanyId,M_Grp.Remarks,M_Grp.GroupCreditLimitName,M_Grp.GroupCreditLimitCode,M_Grc.CreateById,M_Grc.CreateDate,M_Grc.EditById,M_Grc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_GroupCreditLimit_Customer M_Grc INNER JOIN M_GroupCreditLimit M_Grp ON M_Grp.GroupCreditLimitId = M_Grc.GroupCreditLimitId INNER JOIN M_Customer M_Cus ON M_Cus.CustomerId = M_Grc.CustomerId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Grc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Grc.EditById WHERE (M_Grp.GroupCreditLimitName LIKE '%{searchString}%' OR M_Grp.GroupCreditLimitCode LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%' OR M_Cus.CustomerName LIKE '%{searchString}%') AND M_Grc.GroupCreditLimitId<>0 AND M_Grc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit_Customer})) ORDER BY M_Grp.GroupCreditLimitName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<GroupCreditLimit_CustomerViewModel>();

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
                    TblName = "M_GroupCreditLimit_Customer",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<GroupCreditLimit_CustomerViewModel> GetGroupCreditLimit_CustomerByIdAsync(short CompanyId, short UserId, short GroupCreditLimitId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<GroupCreditLimit_CustomerViewModel>($"SELECT GroupCreditLimitId,CustomerId,CompanyId,CreateById,CreateDate,EditById,EditDate FROM dbo.M_GroupCreditLimit_Customer WHERE GroupCreditLimitId={GroupCreditLimitId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.GroupCreditLimit_Customer}))");

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
                    TblName = "M_GroupCreditLimit_Customer",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveGroupCreditLimit_CustomerAsync(short CompanyId, short UserId, M_GroupCreditLimit_Customer m_GroupCreditLimit_Customer)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = false;
                try
                {
                    if (m_GroupCreditLimit_Customer.GroupCreditLimitId != 0)
                    {
                        IsEdit = true;
                    }
                    if (IsEdit)
                    {
                        var DataExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit_Customer WHERE GroupCreditLimitId<>0 AND GroupCreditLimitId={m_GroupCreditLimit_Customer.GroupCreditLimitId} ");

                        if (DataExist.Count() > 0 && DataExist.ToList()[0].IsExist == 1)
                        {
                            var entityHead = _context.Update(m_GroupCreditLimit_Customer);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "User Not Found" };
                    }
                    else
                    {
                        var codeExist = await _repository.GetQueryAsync<SqlResponseIds>($"SELECT 1 AS IsExist FROM dbo.M_GroupCreditLimit_Customer WHERE GroupCreditLimitId<>0 ");

                        if (codeExist.Count() > 0 && codeExist.ToList()[0].IsExist == 1)
                            return new SqlResponse { Result = -1, Message = "GroupCreditLimit_Customer Code Same" };

                        //Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>("SELECT ISNULL((SELECT TOP 1 (GroupCreditLimitId + 1) FROM dbo.M_GroupCreditLimit_Customer WHERE (GroupCreditLimitId + 1) NOT IN (SELECT GroupCreditLimitId FROM dbo.M_GroupCreditLimit_Customer)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_GroupCreditLimit_Customer.GroupCreditLimitId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_GroupCreditLimit_Customer);
                        }
                        else
                            return new SqlResponse { Result = -1, Message = "Internal Server Error" };
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    #region Save AuditLog

                    if (saveChangeRecord > 0)
                    {
                        //Saving Audit log
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Master,
                            TransactionId = (short)E_Master.GroupCreditLimit_Customer,
                            DocumentId = m_GroupCreditLimit_Customer.GroupCreditLimitId,
                            DocumentNo = "",
                            TblName = "M_GroupCreditLimit_Customer",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "GroupCreditLimit_Customer Save Successfully",
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
                        TransactionId = (short)E_Master.GroupCreditLimit_Customer,
                        DocumentId = m_GroupCreditLimit_Customer.GroupCreditLimitId,
                        DocumentNo = "",
                        TblName = "AdmUser",
                        ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                        Remarks = ex.Message + ex.InnerException,
                        CreateById = UserId
                    };
                    _context.Add(errorLog);
                    _context.SaveChanges();

                    throw;
                }
            }
        }

        public async Task<SqlResponse> DeleteGroupCreditLimit_CustomerAsync(short CompanyId, short UserId, M_GroupCreditLimit_Customer GroupCreditLimit_Customer)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (GroupCreditLimit_Customer.GroupCreditLimitId > 0)
                    {
                        var GroupCreditLimit_CustomerToRemove = _context.M_GroupCreditLimit_Customer.Where(x => x.GroupCreditLimitId == GroupCreditLimit_Customer.GroupCreditLimitId).ExecuteDelete();

                        if (GroupCreditLimit_CustomerToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.GroupCreditLimit,
                                DocumentId = GroupCreditLimit_Customer.GroupCreditLimitId,
                                DocumentNo = "",
                                TblName = "M_GroupCreditLimit_Customer",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "GroupCreditLimit_Customer Delete Successfully",
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
                        TblName = "M_GroupCreditLimit_Customer",
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