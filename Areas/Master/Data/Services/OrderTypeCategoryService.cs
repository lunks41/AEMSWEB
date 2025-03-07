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
    public sealed class OrderTypeCategoryService : IOrderTypeCategoryService
    {
        private readonly IRepository<M_OrderTypeCategory> _repository;
        private ApplicationDbContext _context;

        public OrderTypeCategoryService(IRepository<M_OrderTypeCategory> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(short CompanyId, short UserId, short pageSize, short pageNumber, string searchString)
        {
            OrderTypeCategoryViewModelCount countViewModel = new OrderTypeCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_OrderTypeCategory M_OrdC WHERE (M_OrdC.OrderTypeCategoryName LIKE '%{searchString}%' OR M_OrdC.OrderTypeCategoryCode LIKE '%{searchString}%' OR M_OrdC.Remarks LIKE '%{searchString}%') AND M_OrdC.OrderTypeCategoryId<>0 AND M_OrdC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderTypeCategory}))");

                var result = await _repository.GetQueryAsync<OrderTypeCategoryViewModel>($"SELECT M_OrdC.CompanyId,M_OrdC.OrderTypeCategoryId,M_OrdC.OrderTypeCategoryCode,M_OrdC.OrderTypeCategoryName,M_OrdC.Remarks,M_OrdC.IsActive,M_OrdC.CreateById,M_OrdC.CreateDate,M_OrdC.EditById,M_OrdC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_OrderTypeCategory M_OrdC LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_OrdC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_OrdC.EditById WHERE (M_OrdC.OrderTypeCategoryName LIKE '%{searchString}%' OR M_OrdC.OrderTypeCategoryCode LIKE '%{searchString}%' OR M_OrdC.Remarks LIKE '%{searchString}%') AND M_OrdC.OrderTypeCategoryId<>0 AND M_OrdC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderTypeCategory})) ORDER BY M_OrdC.OrderTypeCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

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
                    TransactionId = (short)E_Master.OrderTypeCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderTypeCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_OrderTypeCategory> GetOrderTypeCategoryByIdAsync(short CompanyId, short UserId, int OrderTypeCategoryId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_OrderTypeCategory>($"SELECT CompanyId,OrderTypeCategoryId,OrderTypeCategoryCode,OrderTypeCategoryName,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_OrderTypeCategory WHERE OrderTypeCategoryId={OrderTypeCategoryId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderTypeCategory}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.OrderTypeCategory,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderTypeCategory",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveOrderTypeCategoryAsync(short CompanyId, short UserId, M_OrderTypeCategory m_OrderTypeCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_OrderTypeCategory.OrderTypeCategoryId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_OrderTypeCategory WHERE OrderTypeCategoryId<>@OrderTypeCategoryId AND OrderTypeCategoryCode=@OrderTypeCategoryCode",
                        new { m_OrderTypeCategory.OrderTypeCategoryId, m_OrderTypeCategory.OrderTypeCategoryCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "OrderTypeCategory Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_OrderTypeCategory WHERE OrderTypeCategoryId<>@OrderTypeCategoryId AND OrderTypeCategoryName=@OrderTypeCategoryName",
                        new { m_OrderTypeCategory.OrderTypeCategoryId, m_OrderTypeCategory.OrderTypeCategoryName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "OrderTypeCategory Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_OrderTypeCategory WHERE OrderTypeCategoryId=@OrderTypeCategoryId",
                            new { m_OrderTypeCategory.OrderTypeCategoryId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_OrderTypeCategory);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "OrderTypeCategory Not Found" };
                        }
                    }
                    else
                    {
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (OrderTypeCategoryId + 1) FROM dbo.M_OrderTypeCategory WHERE (OrderTypeCategoryId + 1) NOT IN (SELECT OrderTypeCategoryId FROM dbo.M_OrderTypeCategory)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_OrderTypeCategory.OrderTypeCategoryId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_OrderTypeCategory);
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
                            TransactionId = (short)E_Master.OrderTypeCategory,
                            DocumentId = m_OrderTypeCategory.OrderTypeCategoryId,
                            DocumentNo = m_OrderTypeCategory.OrderTypeCategoryCode,
                            TblName = "M_OrderTypeCategory",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "OrderTypeCategory Save Successfully",
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
                        TransactionId = (short)E_Master.OrderTypeCategory,
                        DocumentId = m_OrderTypeCategory.OrderTypeCategoryId,
                        DocumentNo = m_OrderTypeCategory.OrderTypeCategoryCode,
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

        public async Task<SqlResponse> DeleteOrderTypeCategoryAsync(short CompanyId, short UserId, M_OrderTypeCategory OrderTypeCategory)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (OrderTypeCategory.OrderTypeCategoryId > 0)
                    {
                        var OrderTypeCategoryToRemove = _context.M_OrderTypeCategory.Where(x => x.OrderTypeCategoryId == OrderTypeCategory.OrderTypeCategoryId).ExecuteDelete();

                        if (OrderTypeCategoryToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.OrderTypeCategory,
                                DocumentId = OrderTypeCategory.OrderTypeCategoryId,
                                DocumentNo = OrderTypeCategory.OrderTypeCategoryCode,
                                TblName = "M_OrderTypeCategory",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "OrderTypeCategory Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "OrderTypeCategoryId Should be zero" };
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
                        TransactionId = (short)E_Master.OrderTypeCategory,
                        DocumentId = 0,
                        DocumentNo = "",
                        TblName = "M_OrderTypeCategory",
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