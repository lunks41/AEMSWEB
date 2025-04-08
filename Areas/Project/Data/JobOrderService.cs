using AMESWEB.Areas.Project.Data.IServices;
using AMESWEB.Areas.Project.Models;
using AMESWEB.Data;
using AMESWEB.Entities.Admin;
using AMESWEB.Entities.Project;
using AMESWEB.Enums;
using AMESWEB.Helpers;
using AMESWEB.IServices;
using AMESWEB.Models;
using AMESWEB.Repository;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace AMESWEB.Areas.Project.Data.Services
{
    public sealed class JoborderService : IJobOrderService
    {
        private readonly IRepository<Ser_JobOrderHd> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public JoborderService(IRepository<Ser_JobOrderHd> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        #region Job Order

        public async Task<JobOrderViewModelCount> GetJobOrderListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, DateTime? fromDate, DateTime? toDate, string statusName)
        {
            int statusId = 0;
            JobOrderViewModelCount countViewModel = new JobOrderViewModelCount();
            try
            {
                if (statusName.ToLower() != "all")
                {
                    var statusExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                 "SELECT TOP 1 OrderTypeId AS IsExist FROM dbo.M_OrderType " +
                 "WHERE OrderTypeCode LIKE '%' + @OrderTypeCode + '%' AND OrderTypeCategoryId = 4",
                 new { OrderTypeCode = statusName }); // Using LIKE to support partial matches

                    if ((statusExist?.IsExist ?? 0) > 0)
                    {
                        statusId = statusExist.IsExist;
                    }
                }
                else
                {
                    statusId = 0;
                }

                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                 $"SELECT COUNT(*) AS CountId FROM dbo.Ser_JobOrderHd Hd " +
                 $"LEFT JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                 $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                 $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                 $"OR Hd.JobOrderNo LIKE '%{searchString}%' " +
                 $"OR Hd.Remark1 LIKE '%{searchString}%') " +
                 $"AND Hd.JobOrderId <> 0 " +
                 $"AND Hd.CompanyId = {CompanyId} " +
                 $"AND Hd.JobOrderDate BETWEEN '{fromDate:yyyy-MM-dd}' AND '{toDate:yyyy-MM-dd}' " +
                 $"AND (Hd.CustomerId = {customerId} OR {customerId} = 0) " +
                 $"AND (Hd.StatusId = {statusId}  OR {statusId} = 0) ");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<JobOrderHdViewModel>(
                    $"SELECT Hd.JobOrderId, Hd.CompanyId, Hd.JobOrderNo, Hd.JobOrderDate, Hd.CustomerId, Cur.CurrencyId, Cut.CustomerCode, Cut.CustomerName, Hd.IMONo, " +
                    $"Cur.CurrencyName, Cur.CurrencyCode, Hd.TotalAmt, Hd.TotalLocalAmt, Hd.Remark1, Hd.IsActive, Hd.IsClose, " +
                    $"Usr.UserName AS CreateBy, Usr1.UserName AS EditBy, " +
                    $"Ord.OrderTypeName AS Status " + // Added OrderTypeName as Status
                    $"FROM dbo.Ser_JobOrderHd Hd " +
                    $"INNER JOIN dbo.M_Customer Cut ON Cut.CustomerId = Hd.CustomerId " +
                    $"INNER JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                    $"LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Hd.CreateById " +
                    $"LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Hd.EditById " +
                    $"LEFT JOIN dbo.M_OrderType Ord ON Ord.OrderTypeId = Hd.StatusId " + // Join for OrderType
                    $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                    $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                    $"OR Hd.JobOrderNo LIKE '%{searchString}%' " +
                    $"OR Hd.Remark1 LIKE '%{searchString}%') " +
                    $"AND Hd.JobOrderId <> 0 " +
                    $"AND Hd.CompanyId = {CompanyId} " +
                    $"AND Hd.JobOrderDate BETWEEN '{fromDate:yyyy-MM-dd}' AND '{toDate:yyyy-MM-dd}' " +
                    $"AND (Hd.CustomerId = {customerId} OR {customerId} = 0) " +
                    $"AND (Hd.StatusId = {statusId}  OR {statusId} = 0) " +
                    $"ORDER BY Hd.JobOrderNo " +
                    $"OFFSET {pageSize} * ({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                // Build the result
                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<JobOrderHdViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "Ser_JobOrderHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<StatusCountsViewModel> GetJobStatusCountsAsync(short companyId, short userId, string searchString, int customerId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var countsResult = await _repository.GetQueryAsync<StatusCountViewModel>(
                    $"SELECT Hd.StatusId,COUNT(*) AS CountId FROM  dbo.Ser_JobOrderHd Hd " +
                    $"INNER JOIN dbo.M_Customer Cut ON Cut.CustomerId = Hd.CustomerId " +
                    $"INNER JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                    $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                    $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                    $"OR Hd.JobOrderNo LIKE '%{searchString}%' " +
                    $"OR Hd.Remark1 LIKE '%{searchString}%') " +
                    $"AND Hd.JobOrderId <> 0 " +
                    $"AND Hd.CompanyId = {companyId} " +
                    $"AND Hd.JobOrderDate BETWEEN '{fromDate:yyyy-MM-dd}' AND '{toDate:yyyy-MM-dd}' " +
                    $"AND (Hd.CustomerId = {customerId} OR {customerId} = 0) " +
                    $"GROUP BY Hd.StatusId");

                // Aggregate counts for each status
                int countAll = countsResult.Sum(c => c.CountId);
                int countPending = countsResult.FirstOrDefault(c => c.StatusId == 400)?.CountId ?? 0;
                int countConfirm = countsResult.FirstOrDefault(c => c.StatusId == 401)?.CountId ?? 0;
                int countCompleted = countsResult.FirstOrDefault(c => c.StatusId == 405)?.CountId ?? 0;
                int countCancel = countsResult.FirstOrDefault(c => c.StatusId == 402)?.CountId ?? 0;
                int countPost = countsResult.FirstOrDefault(c => c.StatusId == 407)?.CountId ?? 0;
                int countCancelwithservices = countsResult.FirstOrDefault(c => c.StatusId == 406)?.CountId ?? 0;

                // Build and return result
                return new StatusCountsViewModel
                {
                    All = countAll,
                    Pending = countPending,
                    Confirm = countConfirm,
                    Completed = countCompleted,
                    Cancel = countCancel,
                    Post = countPost,
                    CancelWithService = countCancelwithservices
                };
            }
            catch (Exception ex)
            {
                // Log exception (if applicable) before re-throwing
                throw new Exception($"Error in GetJobStatusCountsAsync: {ex.Message}", ex);
            }
        }

        public async Task<JobOrderHdViewModel> GetJobOrderByIdAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<JobOrderHdViewModel>($"SELECT Hd.CompanyId,Hd.JobOrderId,Hd.JobOrderNo,Hd.JobOrderDate,Hd.CustomerId,M_Cus.CustomerName,Hd.CurrencyId,M_Cur.CurrencyName,Hd.ExhRate,Hd.VesselId,M_Ves.VesselName,Hd.IMONo,Hd.VesselDistance,Hd.PortId,M_Pot.PortName,Hd.LastPortId,M_Pot1.PortName AS LastPortName,Hd.NextPortId,M_Pot2.PortName AS NextPortName,Hd.VoyageId,M_Voy.VoyageNo,Hd.NatureOfCall,Hd.ISPS,Hd.EtaDate,Hd.EtdDate,Hd.OwnerName,Hd.OwnerAgent,Hd.MasterName,Hd.Charters,Hd.ChartersAgent,Hd.IsTaxable FROM dbo.Ser_JobOrderHd Hd  INNER JOIN dbo.M_Customer M_Cus ON M_Cus.CustomerId = Hd.CustomerId  INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = M_Cus.CurrencyId  INNER JOIN dbo.M_Vessel M_Ves ON M_Ves.VesselId = Hd.VesselId  INNER JOIN dbo.M_Port M_Pot ON M_Pot.PortId = Hd.PortId  INNER JOIN dbo.M_Port M_Pot1 ON M_Pot1.PortId = Hd.PortId  INNER JOIN dbo.M_Port M_Pot2 ON M_Pot2.PortId = Hd.PortId  INNER JOIN dbo.M_Voyage M_Voy ON M_Voy.VoyageId = Hd.VoyageId  WHERE Hd.JobOrderId={JobOrderId} AND Hd.CompanyId={CompanyId}");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "M_JobOrderHd",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Job Order

        #region Task

        #region Port Expenses

        public async Task<PortExpensesViewModelCount> GetPortExpensesListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            PortExpensesViewModelCount countViewModel = new PortExpensesViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId  FROM dbo.Ser_PortExpenses Ser_Port INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_Port.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Port.ChargeId AND M_Chr.TaskId = Ser_Port.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Port.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Port.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Port.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Port.EditById WHERE Ser_Port.JobOrderId={JobOrderId} AND Ser_Port.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<PortExpensesViewModel>($"SELECT Ser_Port.PortExpenseId,Ser_Port.CompanyId,Ser_Port.JobOrderId,Ser_Port.JobOrderNo,Ser_Port.TaskId,Ser_Port.Quantity,Ser_Port.SupplierId,M_Ser.SupplierName,Ser_Port.ChargeId,M_Chr.ChargeName,Ser_Port.StatusId,M_Or.OrderTypeName As StatusName,Ser_Port.UomId,M_Uo.UomName,Ser_Port.DeliverDate,Ser_Port.Description,Ser_Port.GLId,Ser_Port.DebitNoteId,Ser_Port.DebitNoteNo,Ser_Port.Remarks,Ser_Port.CreateById,Ser_Port.CreateDate,Ser_Port.EditById,Ser_Port.EditDate,Ser_Port.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_PortExpenses Ser_Port INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_Port.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Port.ChargeId AND M_Chr.TaskId = Ser_Port.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Port.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Port.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Port.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Port.EditById WHERE Ser_Port.JobOrderId={JobOrderId} AND Ser_Port.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<PortExpensesViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "Ser_PortExpenses",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<PortExpensesViewModel> GetPortExpensesByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 PortExpenseId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<PortExpensesViewModel>($"SELECT Ser_Port.PortExpenseId,Ser_Port.CompanyId,Ser_Port.JobOrderId,Ser_Port.JobOrderNo,Ser_Port.TaskId,Ser_Port.Quantity,Ser_Port.SupplierId,M_Ser.SupplierName,Ser_Port.ChargeId,M_Chr.ChargeName,Ser_Port.StatusId,M_Or.OrderTypeName As StatusName,Ser_Port.UomId,M_Uo.UomName,Ser_Port.DeliverDate,Ser_Port.Description,Ser_Port.GLId,Ser_Port.DebitNoteId,Ser_Port.DebitNoteNo,Ser_Port.Remarks,Ser_Port.CreateById,Ser_Port.CreateDate,Ser_Port.EditById,Ser_Port.EditDate,Ser_Port.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_PortExpenses Ser_Port INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_Port.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Port.ChargeId AND M_Chr.TaskId = Ser_Port.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Port.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Port.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Port.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Port.EditById WHERE Ser_Port.JobOrderId={JobOrderId} AND Ser_Port.PortExpenseId={PortExpenseId} AND Ser_Port.CompanyId={CompanyId} ");

                return result;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "Ser_PortExpenses",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SavePortExpensesAsync(short CompanyId, short UserId, Ser_PortExpenses ser_PortExpenses)
        {
            bool IsEdit = ser_PortExpenses.PortExpenseId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_PortExpenses WHERE PortExpenseId=@PortExpenseId",
                     new { ser_PortExpenses.PortExpenseId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_PortExpenses);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "PortExpenses Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                     "SELECT ISNULL((SELECT TOP 1 (PortExpenseId + 1) FROM dbo.Ser_PortExpenses WHERE (PortExpenseId + 1) NOT IN (SELECT PortExpenseId FROM dbo.Ser_PortExpenses)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_PortExpenses.PortExpenseId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_PortExpenses.EditById = null;
                            ser_PortExpenses.EditDate = null;
                            _context.Add(ser_PortExpenses);
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
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_PortExpenses  set EditVersion=EditVersion+1 WHERE   PortExpenseId={ser_PortExpenses.PortExpenseId} AND CompanyId={CompanyId} ");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_PortExpenses.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_PortExpenses.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_PortExpenses.JobOrderId} AND TaskId = {ser_PortExpenses.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_PortExpenses.JobOrderId} AND TaskId = {ser_PortExpenses.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_PortExpenses.JobOrderId},'{ser_PortExpenses.JobOrderNo}',@ItemNo,{ser_PortExpenses.TaskId},@TaskItemNo,{ser_PortExpenses.PortExpenseId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_PortExpenses.PortExpenseId,
                            DocumentNo = "",
                            TblName = "Ser_PortExpenses",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "PortExpenses Save Successfully",
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
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "Ser_PortExpenses",
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
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = ser_PortExpenses.PortExpenseId,
                    DocumentNo = "",
                    TblName = "Ser_PortExpenses",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponse> DeletePortExpensesAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 portExpenseId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (portExpenseId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND  ServiceId={portExpenseId} ");

                        var deleteportExpensesResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_PortExpenses WHERE CompanyId={CompanyId} AND PortExpenseId={portExpenseId}");

                        if (deletejobOrderResult > 0 && deleteportExpensesResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = portExpenseId,
                                DocumentNo = "",
                                TblName = "Ser_PortExpenses",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "PortExpenses Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "PortExpenseId Should be zero" };
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
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = portExpenseId,
                    DocumentNo = "",
                    TblName = "Ser_PortExpenses",
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
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = portExpenseId,
                    DocumentNo = "",
                    TblName = "Ser_PortExpenses",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Port Expenses

        #endregion Task

        public async Task<SqlResponse> SaveTaskForwardAsync(short companyId, short userId, Int64 jobOrderId, string jobOrderNo, Int64 prevJobOrderId, int taskId, string multipleId)
        {
            try
            {
                using (var tScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Build common query to update JobOrderId on Ser_JobOrderDt.
                    string commonQuery = $@"UPDATE dbo.Ser_JobOrderDt SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND ServiceId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected;";

                    int rowsAffected = await _repository.GetQuerySingleOrDefaultAsync<int>(commonQuery);

                    if (rowsAffected == 0)
                    {
                        return new SqlResponse { Result = -1, Message = "Data Not there" };
                    }

                    // Build the task-specific query based on taskId.
                    string specificQuery = taskId switch
                    {
                        1 => $"UPDATE dbo.Ser_PortExpenses SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND PortExpenseId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        2 => $"UPDATE dbo.Ser_LaunchServices SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND LaunchServiceId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        3 => $"UPDATE dbo.Ser_EquipmentsUsed SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND EquipmentsUsedId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        4 => $"UPDATE dbo.Ser_CrewSignOn SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND CrewSignOnId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        5 => $"UPDATE dbo.Ser_CrewSignOff SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND CrewSignOffId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        6 => $"UPDATE dbo.Ser_CrewMiscellaneous SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND CrewMiscellaneousId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        7 => $"UPDATE dbo.Ser_MedicalAssistance SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND MedicalAssistanceId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        8 => $"UPDATE dbo.Ser_ConsignmentImport SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND ConsignmentImportId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        9 => $"UPDATE dbo.Ser_ConsignmentExport SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND ConsignmentExporId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        10 => $"UPDATE dbo.Ser_ThirdPartySupply SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND ThirdPartySupplyId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        11 => $"UPDATE dbo.Ser_FreshWater SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND FreshWaterId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        12 => $"UPDATE dbo.Ser_TechniciansSurveyors SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND TechniciansSurveyorsId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        13 => $"UPDATE dbo.Ser_LandingItems SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND LandingItemId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        14 => $"UPDATE dbo.Ser_OtherService SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND OtherServiceId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        15 => $"UPDATE dbo.Ser_AgencyRemuneration SET JobOrderId = {jobOrderId},JobOrderNo='{jobOrderNo}' WHERE TaskId = {taskId} AND JobOrderId = {prevJobOrderId} AND AgencyRemunerationId IN ({multipleId});  SELECT @@ROWCOUNT AS RowsAffected; ",
                        _ => null
                    };

                    if (string.IsNullOrEmpty(specificQuery))
                    {
                        return new SqlResponse { Result = -1, Message = "Invalid task specified." };
                    }

                    int rowsAffectedSpecific = await _repository.GetQuerySingleOrDefaultAsync<int>(specificQuery);

                    if (rowsAffectedSpecific == 0)
                    {
                        return new SqlResponse { Result = -1, Message = "Data Not there" };
                    }
                    else
                    {
                        tScope.Complete();
                        return new SqlResponse { Result = 1, Message = "Successfully task forward" };
                    }
                }
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = companyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Job,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "Ser_PortExpenses",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + (ex.InnerException?.Message ?? string.Empty),
                    CreateById = userId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw; // Rethrow the exception for upper layers to handle if needed.
            }
        }

        #region Task Count

        public async Task<TaskCountsViewModel> GetTaskJobOrderCountsAsync(short companyId, short userId, string searchString, Int64 jobOrderId)
        {
            try
            {
                var countsResult = await _repository.GetQueryAsync<TaskCountViewModel>(
                    $"SELECT Ser_Jobdt.CompanyId,Ser_Jobdt.TaskId,COUNT(*) AS CountId FROM dbo.Ser_JobOrderDt Ser_Jobdt WHERE Ser_Jobdt.CompanyId={companyId} AND Ser_Jobdt.JobOrderId={jobOrderId} GROUP BY Ser_Jobdt.CompanyId,Ser_Jobdt.TaskId");

                // Aggregate counts for each status
                int countPortExpenses = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.PortExpenses)?.CountId ?? 0;
                int countLaunchServices = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.LaunchServices)?.CountId ?? 0;
                int countEquipmentsUsed = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.EquipmentsUsed)?.CountId ?? 0;
                int countCrewSignOn = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.CrewSignOn)?.CountId ?? 0;
                int countCrewSignOff = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.CrewSignOff)?.CountId ?? 0;
                int countCrewMiscellaneous = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.CrewMiscellaneous)?.CountId ?? 0;
                int countMedicalAssistance = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.MedicalAssistance)?.CountId ?? 0;
                int countConsignmentImport = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.ConsignmentImport)?.CountId ?? 0;
                int countConsignmentExport = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.ConsignmentExport)?.CountId ?? 0;
                int countThirdPartySupply = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.ThirdPartySupply)?.CountId ?? 0;
                int countFreshWaterSupply = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.FreshWaterSupply)?.CountId ?? 0;
                int countTechniciansSurveyors = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.TechniciansSurveyors)?.CountId ?? 0;
                int countLandingItems = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.LandingItems)?.CountId ?? 0;
                int countOtherService = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.OtherService)?.CountId ?? 0;
                int countAgencyRemuneration = countsResult.FirstOrDefault(c => c.TaskId == (short)E_Task.AgencyRemuneration)?.CountId ?? 0;
                int countVisa = 0;

                // Build and return result
                return new TaskCountsViewModel
                {
                    PortExpense = countPortExpenses,
                    LaunchServices = countLaunchServices,
                    EquipmentsUsed = countEquipmentsUsed,
                    CrewSignOn = countCrewSignOn,
                    CrewSignOff = countCrewSignOff,
                    CrewMiscellaneous = countCrewMiscellaneous,
                    MedicalAssistance = countMedicalAssistance,
                    ConsignmentImport = countConsignmentImport,
                    ConsignmentExport = countConsignmentExport,
                    ThirdPartySupply = countThirdPartySupply,
                    FreshWaterSupply = countFreshWaterSupply,
                    TechniciansSurveyors = countTechniciansSurveyors,
                    LandingItems = countLandingItems,
                    OtherService = countOtherService,
                    AgencyRemuneration = countAgencyRemuneration,
                    Visa = countVisa
                };
            }
            catch (Exception ex)
            {
                // Log exception (if applicable) before re-throwing
                throw new Exception($"Error in GetTaskJobOrderCountsAsync: {ex.Message}", ex);
            }
        }

        #endregion Task Count
    }
}