using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Models.Masters;
using AEMSWEB.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AEMSWEB.Areas.Master.Data.Services
{
    public sealed class CustomerCreditLimitService : ICustomerCreditLimitService
    {
        private readonly IRepository<M_CustomerCreditLimit> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public CustomerCreditLimitService(IRepository<M_CustomerCreditLimit> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<CustomerCreditLimitViewModelCount> GetCustomerCreditLimitListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            CustomerCreditLimitViewModelCount countViewModel = new CustomerCreditLimitViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_CustomerCreditLimit M_Cusc INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_Cusc.CustomerId WHERE (M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%') AND M_Cusc.CustomerId<>0 AND M_Cusc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CustomerCreditLimit}))");

                var result = await _repository.GetQueryAsync<CustomerCreditLimitViewModel>($"SELECT M_Cusc.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Cusc.EffectFrom,M_Cusc.EffectUntil,M_Cusc.IsExpires,M_Cusc.Remarks,M_Cusc.CreditLimitAmt,M_Cusc.CreateById,M_Cusc.CreateDate,M_Cusc.EditById,M_Cusc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_CustomerCreditLimit M_Cusc INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = M_Cusc.CustomerId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Cusc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Cusc.EditById WHERE (M_Cus.CustomerName LIKE '%{searchString}%' OR M_Cus.CustomerCode LIKE '%{searchString}%') AND M_Cusc.CustomerId<>0 AND M_Cusc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CustomerCreditLimit})) ORDER BY M_Cus.CustomerName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<CustomerCreditLimitViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CustomerCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerCreditLimit",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_CustomerCreditLimit> GetCustomerCreditLimitByIdAsync(short CompanyId, short UserId, int CustomerId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_CustomerCreditLimit>($"SELECT M_Cusc.CustomerId,M_Cus.CustomerCode,M_Cus.CustomerName,M_Cusc.EffectFrom,M_Cusc.EffectUntil,M_Cusc.IsExpires,M_Cusc.Remarks,M_Cusc.CreditLimitAmt,M_Cusc.CreateById,M_Cusc.CreateDate,M_Cusc.EditById,M_Cusc.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_CustomerCreditLimit M_Cusc Inner Join M_Customer M_Cus ON M_Cus.CustomerId = M_Cusc.CustomerId left Join AdmUser Usr ON Usr.UserId = M_Cusc.CreateById left Join AdmUser Usr1 ON Usr1.UserId = M_Cusc.EditById WHERE M_Cusc.CustomerId={CustomerId} AND M_Cusc.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.CustomerCreditLimit}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.CustomerCreditLimit,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_CustomerCreditLimit",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        //public async Task<SqlResponse> SaveCustomerCreditLimitAsync( Int16 CompanyId, M_CustomerCreditLimit m_CustomerCreditLimit, Int16 UserId)
        //{
        //    using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        bool IsEdit = false;
        //        try
        //        {
        //            if (m_CustomerCreditLimit.CustomerId != 0)
        //            {
        //                IsEdit = true;
        //            }
        //            if (IsEdit)
        //            {
        //                var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>( $"SELECT 1 AS IsExist FROM dbo.M_CustomerCreditLimit WHERE CustomerId<>0 AND CustomerId={m_CustomerCreditLimit.CustomerId} ");

        //                if (dataExist.Count() > 0 && dataExist.ToList()[0].IsExist == 1)
        //                {
        //                    var entityHead = _context.Update(m_CustomerCreditLimit);
        //                    entityHead.Property(b => b.CreateById).IsModified = false;
        //                    entityHead.Property(b => b.CompanyId).IsModified = false;
        //                }
        //                else
        //                    return new SqlResponse { Result = -1, Message = "User Not Found" };
        //            }
        //            else
        //            {
        //                var codeExist = await _repository.GetQueryAsync<SqlResponseIds>( $"SELECT 1 AS IsExist FROM dbo.M_CustomerCreditLimit WHERE GroupCreditLimitId<>0 AND GroupCreditLimitCode={m_CustomerCreditLimit.GroupCreditLimitCode} AND GroupCreditLimitName={m_CustomerCreditLimit.GroupCreditLimitName} ");

        //                if (codeExist.Count() > 0 && codeExist.ToList()[0].IsExist == 1)
        //                    return new SqlResponse { Result = -1, Message = "GroupCreditLimit Code already exists. " };

        //                //Take the Next Id From SQL
        //                var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>( "SELECT ISNULL((SELECT TOP 1 (GroupCreditLimitId + 1) FROM dbo.M_CustomerCreditLimit WHERE (GroupCreditLimitId + 1) NOT IN (SELECT GroupCreditLimitId FROM dbo.M_CustomerCreditLimit)),1) AS NextId");

        //                if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
        //                {
        //                    m_CustomerCreditLimit.GroupCreditLimitId = Convert.ToInt16(sqlMissingResponse.NextId);
        //                    _context.Add(m_CustomerCreditLimit);
        //                }
        //                else
        //                    return new SqlResponse { Result = -1, Message = "Internal Server Error" };
        //            }

        //            var saveChangeRecord = _context.SaveChanges();

        //            #region Save AuditLog

        //            if (saveChangeRecord > 0)
        //            {
        //                //Saving Audit log
        //                var auditLog = new AdmAuditLog
        //                {
        //                    CompanyId = CompanyId,
        //                    ModuleId = (short)E_Modules.Master,
        //                    TransactionId = (short)E_Master.GroupCreditLimit,
        //                    DocumentId = m_CustomerCreditLimit.GroupCreditLimitId,
        //                    DocumentNo = m_CustomerCreditLimit.GroupCreditLimitCode,
        //                    TblName = "M_CustomerCreditLimit",
        //                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
        //                    Remarks = "GroupCreditLimit Save Successfully",
        //                    CreateById = UserId,
        //                    CreateDate = DateTime.Now
        //                };

        //                _context.Add(auditLog);
        //                var auditLogSave = _context.SaveChanges();

        //                if (auditLogSave > 0)
        //                {
        //                    TScope.Complete();
        //                    return new SqlResponse { Result = 1, Message = "Save Successfully" };
        //                }
        //            }
        //            else
        //            {
        //                return new SqlResponse { Result = 1, Message = "Save Failed" };
        //            }

        //            #endregion Save AuditLog

        //            return new SqlResponse();
        //        }
        //        catch (Exception ex)
        //        {
        //            _context.ChangeTracker.Clear();

        //            var errorLog = new AdmErrorLog
        //            {
        //                CompanyId = CompanyId,
        //                ModuleId = (short)E_Modules.Master,
        //                TransactionId = (short)E_Master.GroupCreditLimit,
        //                DocumentId = m_CustomerCreditLimit.CreditLimitAmt,
        //                DocumentNo = m_CustomerCreditLimit.GroupCreditLimitCode,
        //                TblName = "AdmUser",
        //                ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
        //                Remarks = ex.Message + ex.InnerException?.Message,
        //                CreateById = UserId
        //            };
        //            _context.Add(errorLog);
        //            _context.SaveChanges();

        //            throw;
        //        }
        //    }
        //}

        public async Task<SqlResponse> DeleteCustomerCreditLimitAsync(short CompanyId, short UserId, M_CustomerCreditLimit CustomerCreditLimit)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (CustomerCreditLimit.CustomerId > 0)
                    {
                        var CustomerCreditLimitToRemove = _context.M_CustomerCreditLimit.Where(x => x.CustomerId == CustomerCreditLimit.CustomerId).ExecuteDelete();

                        if (CustomerCreditLimitToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.CustomerCreditLimit,
                                DocumentId = CustomerCreditLimit.CustomerId,
                                DocumentNo = "",
                                TblName = "M_CustomerCreditLimit",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CustomerCreditLimit Delete Successfully",
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
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();

                    var errorLog = new AdmErrorLog
                    {
                        CompanyId = CompanyId,
                        ModuleId = (short)E_Modules.Master,
                        TransactionId = (short)E_Master.CustomerCreditLimit,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_CustomerCreditLimit",
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