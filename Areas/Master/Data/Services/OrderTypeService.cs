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
    public sealed class OrderTypeService : IOrderTypeService
    {
        private readonly IRepository<M_OrderType> _repository;
        private ApplicationDbContext _context;

        public OrderTypeService(IRepository<M_OrderType> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<OrderTypeViewModelCount> GetOrderTypeListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            OrderTypeViewModelCount countViewModel = new OrderTypeViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_OrderType M_Ord WHERE (M_Ord.OrderTypeName LIKE '%{searchString}%' OR M_Ord.OrderTypeCode LIKE '%{searchString}%' OR M_Ord.Remarks LIKE '%{searchString}%') AND M_Ord.OrderTypeId<>0 AND M_Ord.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderType}))");

                var result = await _repository.GetQueryAsync<OrderTypeViewModel>($"SELECT M_Ord.CompanyId,M_Ord.OrderTypeId,M_Ord.OrderTypeCode,M_Ord.OrderTypeName,M_Ord.OrderTypeCategoryId,M_Ordc.OrderTypeCategoryCode,M_Ordc.OrderTypeCategoryName,M_Ord.Remarks,M_Ord.IsActive,M_Ord.CreateById,M_Ord.CreateDate,M_Ord.EditById,M_Ord.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_OrderType M_Ord INNER JOIN dbo.M_OrderTypeCategory M_Ordc ON M_Ordc.OrderTypeCategoryId = M_Ord.OrderTypeCategoryId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Ord.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Ord.EditById WHERE (M_Ord.OrderTypeName LIKE '%{searchString}%' OR M_Ord.OrderTypeCode LIKE '%{searchString}%' OR M_Ordc.OrderTypeCategoryCode LIKE '%{searchString}%' OR M_Ordc.OrderTypeCategoryName LIKE '%{searchString}%' OR M_Ord.Remarks LIKE '%{searchString}%') AND M_Ord.OrderTypeId<>0 AND M_Ord.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderType})) ORDER BY M_Ord.OrderTypeName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<OrderTypeViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.OrderType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderType",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<M_OrderType> GetOrderTypeByIdAsync(short CompanyId, short UserId, short OrderTypeId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<M_OrderType>($"SELECT OrderTypeId,OrderTypeCode,OrderTypeName,OrderTypeCategoryId,CompanyId,Remarks,IsActive,CreateById,CreateDate,EditById,EditDate FROM dbo.M_OrderType WHERE OrderTypeId={OrderTypeId} AND CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderType}))");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Master,
                    TransactionId = (short)E_Master.OrderType,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_OrderType",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveOrderTypeAsync(short CompanyId, short UserId, M_OrderType m_OrderType)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool IsEdit = m_OrderType.OrderTypeId != 0;
                try
                {
                    var codeExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_OrderType WHERE OrderTypeId<>@OrderTypeId AND OrderTypeCode=@OrderTypeCode",
                        new { m_OrderType.OrderTypeId, m_OrderType.OrderTypeCode });
                    if ((codeExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "OrderType Code already exists." };

                    var nameExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT 1 AS IsExist FROM dbo.M_OrderType WHERE OrderTypeId<>@OrderTypeId AND OrderTypeName=@OrderTypeName",
                        new { m_OrderType.OrderTypeId, m_OrderType.OrderTypeName });
                    if ((nameExist?.IsExist ?? 0) > 0)
                        return new SqlResponse { Result = -1, Message = "OrderType Name already exists." };

                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            $"SELECT 1 AS IsExist FROM dbo.M_OrderType WHERE OrderTypeId=@OrderTypeId",
                            new { m_OrderType.OrderTypeId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(m_OrderType);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "OrderType Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "SELECT ISNULL((SELECT TOP 1 (OrderTypeId + 1) FROM dbo.M_OrderType WHERE (OrderTypeId + 1) NOT IN (SELECT OrderTypeId FROM dbo.M_OrderType)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            m_OrderType.OrderTypeId = Convert.ToInt16(sqlMissingResponse.NextId);
                            _context.Add(m_OrderType);
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
                            TransactionId = (short)E_Master.OrderType,
                            DocumentId = m_OrderType.OrderTypeId,
                            DocumentNo = m_OrderType.OrderTypeCode,
                            TblName = "M_OrderType",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "OrderType Save Successfully",
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
                        TransactionId = (short)E_Master.OrderType,
                        DocumentId = m_OrderType.OrderTypeId,
                        DocumentNo = m_OrderType.OrderTypeCode,
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

        public async Task<SqlResponse> DeleteOrderTypeAsync(short CompanyId, short UserId, M_OrderType OrderType)
        {
            using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (OrderType.OrderTypeId > 0)
                    {
                        var OrderTypeToRemove = _context.M_OrderType.Where(x => x.OrderTypeId == OrderType.OrderTypeId).ExecuteDelete();

                        if (OrderTypeToRemove > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Master,
                                TransactionId = (short)E_Master.OrderType,
                                DocumentId = OrderType.OrderTypeId,
                                DocumentNo = OrderType.OrderTypeCode,
                                TblName = "M_OrderType",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "OrderType Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "OrderTypeId Should be zero" };
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
                        TransactionId = (short)E_Master.OrderType,
                        DocumentId = OrderType.OrderTypeId,
                        DocumentNo = OrderType.OrderTypeCode,
                        TblName = "M_OrderType",
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

        public async Task<OrderTypeCategoryViewModelCount> GetOrderTypeCategoryListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString)
        {
            OrderTypeCategoryViewModelCount countViewModel = new OrderTypeCategoryViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId FROM M_OrderTypeCategory M_OrdC WHERE (M_OrdC.OrderTypeCategoryName LIKE '%{searchString}%' OR M_OrdC.OrderTypeCategoryCode LIKE '%{searchString}%' OR M_OrdC.Remarks LIKE '%{searchString}%') AND M_OrdC.OrderTypeCategoryId<>0 AND M_OrdC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderTypeCategory}))");

                var result = await _repository.GetQueryAsync<OrderTypeCategoryViewModel>($"SELECT M_OrdC.CompanyId,M_OrdC.OrderTypeCategoryId,M_OrdC.OrderTypeCategoryCode,M_OrdC.OrderTypeCategoryName,M_OrdC.Remarks,M_OrdC.IsActive,M_OrdC.CreateById,M_OrdC.CreateDate,M_OrdC.EditById,M_OrdC.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM dbo.M_OrderTypeCategory M_OrdC LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_OrdC.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_OrdC.EditById WHERE (M_OrdC.OrderTypeCategoryName LIKE '%{searchString}%' OR M_OrdC.OrderTypeCategoryCode LIKE '%{searchString}%' OR M_OrdC.Remarks LIKE '%{searchString}%') AND M_OrdC.OrderTypeCategoryId<>0 AND M_OrdC.CompanyId IN (SELECT distinct CompanyId FROM Fn_Adm_GetShareCompany({CompanyId},{(short)E_Modules.Master},{(short)E_Master.OrderTypeCategory})) ORDER BY M_OrdC.OrderTypeCategoryName OFFSET {pageSize}*({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<OrderTypeCategoryViewModel>(0);

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