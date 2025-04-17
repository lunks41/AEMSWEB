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
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace AMESWEB.Areas.Project.Data.Services
{
    public sealed class JobTaskService : IJobTaskService
    {
        private readonly IRepository<Ser_PortExpenses> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public JobTaskService(IRepository<Ser_PortExpenses> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        #region Port Expenses

        public async Task<PortExpensesViewModelCount> GetPortExpensesListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            PortExpensesViewModelCount countViewModel = new PortExpensesViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId  FROM dbo.Ser_PortExpenses Ser_Port INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_Port.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Port.ChargeId AND M_Chr.TaskId = Ser_Port.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Port.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Port.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Port.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Port.EditById WHERE Ser_Port.JobOrderId={JobOrderId} AND Ser_Port.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<PortExpensesViewModel>($"SELECT Ser_Port.PortExpenseId,Ser_Port.CompanyId,Ser_Port.JobOrderId,Ser_Port.JobOrderNo,Ser_Port.TaskId,Ser_Port.Quantity,Ser_Port.SupplierId,M_Ser.SupplierName,Ser_Port.ChargeId,M_Chr.ChargeName,Ser_Port.StatusId,M_Or.OrderTypeName As StatusName,Ser_Port.UomId,M_Uo.UomName,Ser_Port.DeliverDate,Ser_Port.GLId,Ser_Port.DebitNoteId,Ser_Port.DebitNoteNo,Ser_Port.Remarks,Ser_Port.CreateById,Ser_Port.CreateDate,Ser_Port.EditById,Ser_Port.EditDate,Ser_Port.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_PortExpenses Ser_Port INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_Port.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Port.ChargeId AND M_Chr.TaskId = Ser_Port.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Port.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Port.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Port.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Port.EditById WHERE Ser_Port.JobOrderId={JobOrderId} AND Ser_Port.CompanyId={CompanyId}");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<PortExpensesViewModel>($"SELECT Ser_Port.PortExpenseId,Ser_Port.CompanyId,Ser_Port.JobOrderId,Ser_Port.JobOrderNo,Ser_Port.TaskId,Ser_Port.Quantity,Ser_Port.SupplierId,M_Ser.SupplierName,Ser_Port.ChargeId,M_Chr.ChargeName,Ser_Port.StatusId,M_Or.OrderTypeName As StatusName,Ser_Port.UomId,M_Uo.UomName,Ser_Port.DeliverDate,Ser_Port.GLId,Ser_Port.DebitNoteId,Ser_Port.DebitNoteNo,Ser_Port.Remarks,Ser_Port.CreateById,Ser_Port.CreateDate,Ser_Port.EditById,Ser_Port.EditDate,Ser_Port.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_PortExpenses Ser_Port INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_Port.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Port.ChargeId AND M_Chr.TaskId = Ser_Port.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Port.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Port.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Port.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Port.EditById WHERE Ser_Port.JobOrderId={JobOrderId} AND Ser_Port.PortExpenseId={PortExpenseId} AND Ser_Port.CompanyId={CompanyId} ");

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

        public async Task<SqlResponce> SavePortExpensesAsync(short CompanyId, short UserId, Ser_PortExpenses ser_PortExpenses)
        {
            bool IsEdit = ser_PortExpenses.PortExpenseId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
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
                            return new SqlResponce { Result = -1, Message = "PortExpenses Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
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
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_PortExpenses  set EditVersion=EditVersion+1 WHERE   PortExpenseId={ser_PortExpenses.PortExpenseId} AND CompanyId={CompanyId} ");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
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
                            return new SqlResponce { Result = 1, Message = "Save Successfully" };
                        }
                    }
                    else
                    {
                        return new SqlResponce { Result = 1, Message = "Save Failed" };
                    }

                    return new SqlResponce();
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

                return new SqlResponce
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

        public async Task<SqlResponce> DeletePortExpensesAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 portExpenseId)
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
                        return new SqlResponce { Result = -1, Message = "PortExpenseId Should be zero" };
                    }
                    return new SqlResponce();
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

                return new SqlResponce
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

        #region Launch Services

        public async Task<LaunchServiceViewModelCount> GetLaunchServiceListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            LaunchServiceViewModelCount countViewModel = new LaunchServiceViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId  FROM dbo.Ser_LaunchService Ser_Launch INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Launch.ChargeId AND M_Chr.TaskId = Ser_Launch.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Launch.UomId LEFT JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = Ser_Launch.BargeId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Launch.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Launch.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Launch.EditById WHERE Ser_Launch.JobOrderId={JobOrderId} AND Ser_Launch.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<LaunchServiceViewModel>($"SELECT Ser_Launch.LaunchServiceId,Ser_Launch.LaunchServiceDate,Ser_Launch.CompanyId,Ser_Launch.JobOrderId,Ser_Launch.JobOrderNo,Ser_Launch.TaskId,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Launch.LoadingTime,Ser_Launch.LeftJetty,Ser_Launch.AlongsideVessel,Ser_Launch.DepartedFromVessel,Ser_Launch.ArrivedAtJetty,Ser_Launch.DistanceFromJetty,Ser_Launch.UomId,M_Uo.UomName,Ser_Launch.AmeTally,Ser_Launch.BoatopTally,Ser_Launch.DistanceFromJettyToVessel,Ser_Launch.WeightOfCargoDelivered,Ser_Launch.WeightOfCargoLanded,Ser_Launch.BoatOperator,Ser_Launch.InvoiceNo,Ser_Launch.Annexure,Ser_Launch.TimeDiff,Ser_Launch.LaunchWaitingTime,Ser_Launch.BargeId,Ser_Launch.BargeName,Ser_Launch.StatusId,M_Or.OrderTypeName As StatusName,Ser_Launch.GLId,Ser_Launch.DebitNoteId,Ser_Launch.DebitNoteNo,Ser_Launch.Remarks,Ser_Launch.CreateById,Ser_Launch.CreateDate,Ser_Launch.EditById,Ser_Launch.EditDate,Ser_Launch.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_LaunchService Ser_Launch INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Launch.ChargeId AND M_Chr.TaskId = Ser_Launch.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Launch.UomId LEFT JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = Ser_Launch.BargeId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Launch.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Launch.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Launch.EditById WHERE Ser_Launch.JobOrderId={JobOrderId} AND Ser_Launch.CompanyId={CompanyId} ");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<LaunchServiceViewModel>();

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
                    TblName = "Ser_LaunchService",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<LaunchServiceViewModel> GetLaunchServiceByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 LaunchServiceId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<LaunchServiceViewModel>($"SELECT Ser_Launch.LaunchServiceId,Ser_Launch.LaunchServiceDate,Ser_Launch.CompanyId,Ser_Launch.JobOrderId,Ser_Launch.JobOrderNo,Ser_Launch.TaskId,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Launch.LoadingTime,Ser_Launch.LeftJetty,Ser_Launch.AlongsideVessel,Ser_Launch.DepartedFromVessel,Ser_Launch.ArrivedAtJetty,Ser_Launch.DistanceFromJetty,Ser_Launch.UomId,M_Uo.UomName,Ser_Launch.AmeTally,Ser_Launch.BoatopTally,Ser_Launch.DistanceFromJettyToVessel,Ser_Launch.WeightOfCargoDelivered,Ser_Launch.WeightOfCargoLanded,Ser_Launch.BoatOperator,Ser_Launch.InvoiceNo,Ser_Launch.Annexure,Ser_Launch.TimeDiff,Ser_Launch.LaunchWaitingTime,Ser_Launch.BargeId,Ser_Launch.BargeName,Ser_Launch.StatusId,M_Or.OrderTypeName As StatusName,Ser_Launch.GLId,Ser_Launch.DebitNoteId,Ser_Launch.DebitNoteNo,Ser_Launch.Remarks,Ser_Launch.CreateById,Ser_Launch.CreateDate,Ser_Launch.EditById,Ser_Launch.EditDate,Ser_Launch.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_LaunchService Ser_Launch INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Launch.ChargeId AND M_Chr.TaskId = Ser_Launch.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Launch.UomId LEFT JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = Ser_Launch.BargeId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Launch.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Launch.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Launch.EditById WHERE Ser_Launch.JobOrderId={JobOrderId} AND Ser_Launch.CompanyId={CompanyId} AND AND Ser_Launch.LaunchServiceId={LaunchServiceId} ");

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
                    TblName = "Ser_LaunchService",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveLaunchServiceAsync(short CompanyId, short UserId, Ser_LaunchService ser_LaunchService)
        {
            bool IsEdit = ser_LaunchService.LaunchServiceId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_LaunchService WHERE LaunchServiceId=@LaunchServiceId",
                     new { ser_LaunchService.LaunchServiceId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_LaunchService);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "LaunchService Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (LaunchServiceId + 1) FROM dbo.Ser_LaunchService WHERE (LaunchServiceId + 1) NOT IN (SELECT LaunchServiceId FROM dbo.Ser_LaunchService)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_LaunchService.LaunchServiceId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_LaunchService.EditById = null;
                            ser_LaunchService.EditDate = null;
                            _context.Add(ser_LaunchService);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_LaunchService  set EditVersion=EditVersion+1 WHERE   LaunchServiceId={ser_LaunchService.LaunchServiceId} AND CompanyId={CompanyId} ");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LaunchService.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LaunchService.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LaunchService.JobOrderId} AND TaskId = {ser_LaunchService.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LaunchService.JobOrderId} AND TaskId = {ser_LaunchService.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_LaunchService.JobOrderId},'{ser_LaunchService.JobOrderNo}',@ItemNo,{ser_LaunchService.TaskId},@TaskItemNo,{ser_LaunchService.LaunchServiceId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_LaunchService.LaunchServiceId,
                            DocumentNo = "",
                            TblName = "Ser_LaunchService",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "LaunchService Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_LaunchService",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_LaunchService.LaunchServiceId,
                    DocumentNo = "",
                    TblName = "Ser_LaunchService",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteLaunchServiceAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 portExpenseId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (portExpenseId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND  ServiceId={portExpenseId} ");

                        var deleteportExpensesResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_LaunchService WHERE CompanyId={CompanyId} AND LaunchServiceId={portExpenseId}");

                        if (deletejobOrderResult > 0 && deleteportExpensesResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = portExpenseId,
                                DocumentNo = "",
                                TblName = "Ser_LaunchService",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "LaunchService Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "LaunchServiceId Should be zero" };
                    }
                    return new SqlResponce();
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
                    TblName = "Ser_LaunchService",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    TblName = "Ser_LaunchService",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Launch Services

        #region Equipments Used

        public async Task<EquipmentsUsedViewModelCount> GetEquipmentsUsedListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            EquipmentsUsedViewModelCount countViewModel = new EquipmentsUsedViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_EquipmentsUsed Ser_Eq LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Eq.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Eq.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Eq.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Eq.ChargeId AND M_Chr.TaskId = Ser_Eq.TaskId LEFT JOIN dbo.M_Charge M_Chr1 ON M_Chr.ChargeId = Ser_Eq.ForkliftChargeId LEFT JOIN dbo.M_Charge M_Chr2 ON M_Chr.ChargeId = Ser_Eq.CraneChargeId LEFT JOIN dbo.M_Charge M_Chr3 ON M_Chr.ChargeId = Ser_Eq.StevedorChargeId  LEFT JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Eq.UomId WHERE Ser_Eq.JobOrderId={JobOrderId} AND Ser_Eq.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<EquipmentsUsedViewModel>($"SELECT Ser_Eq.EquipmentsUsedId,Ser_Eq.CompanyId,Ser_Eq.JobOrderId,Ser_Eq.JobOrderNo,Ser_Eq.TaskId,Ser_Eq.Quantity,Ser_Eq.ChargeId,M_Chr.ChargeName,Ser_Eq.UomId,M_Uo.UomName,Ser_Eq.GLId,Ser_Eq.Mafi,Ser_Eq.Others,Ser_Eq.ForkliftChargeId,M_Chr1.ChargeName AS ForkliftChargeName,Ser_Eq.CraneChargeId,M_Chr2.ChargeName AS CraneChargeName,Ser_Eq.StevedorChargeId,M_Chr3.ChargeName AS StevedorChargeName,Ser_Eq.LoadingRefNo,Ser_Eq.Craneloading,Ser_Eq.Forkliftloading,Ser_Eq.Stevedorloading,Ser_Eq.OffloadingRefNo,Ser_Eq.CraneOffloading,Ser_Eq.ForkliftOffloading,Ser_Eq.StevedorOffloading,Ser_Eq.MorningTimeIn,Ser_Eq.MorningTimeOut,Ser_Eq.MorningTotalHours,Ser_Eq.EveningTimeIn,Ser_Eq.EveningTimeOut,Ser_Eq.EveningTotalHours,Ser_Eq.TotalRegularHours,Ser_Eq.TotalOvertimeHours,Ser_Eq.DriverName,Ser_Eq.VehicleName,Ser_Eq.Remarks,Ser_Eq.StatusId,M_Or.OrderTypeName AS StatusName,Ser_Eq.IsEquimentFooter,Ser_Eq.EquimentFooter,Ser_Eq.DebitNoteId,Ser_Eq.DebitNoteNo,Ser_Eq.CreateById,Ser_Eq.CreateDate,Ser_Eq.EditById,Ser_Eq.EditDate,Ser_Eq.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_EquipmentsUsed Ser_Eq LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Eq.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Eq.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Eq.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Eq.ChargeId AND M_Chr.TaskId = Ser_Eq.TaskId LEFT JOIN dbo.M_Charge M_Chr1 ON M_Chr.ChargeId = Ser_Eq.ForkliftChargeId LEFT JOIN dbo.M_Charge M_Chr2 ON M_Chr.ChargeId = Ser_Eq.CraneChargeId LEFT JOIN dbo.M_Charge M_Chr3 ON M_Chr.ChargeId = Ser_Eq.StevedorChargeId  LEFT JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Eq.UomId WHERE Ser_Eq.JobOrderId={JobOrderId} AND Ser_Eq.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<EquipmentsUsedViewModel>();

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
                    TblName = "Ser_EquipmentsUsed",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<EquipmentsUsedViewModel> GetEquipmentsUsedByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 EquipmentsUsedId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<EquipmentsUsedViewModel>($"SELECT Ser_Eq.EquipmentsUsedId,Ser_Eq.CompanyId,Ser_Eq.JobOrderId,Ser_Eq.JobOrderNo,Ser_Eq.TaskId,Ser_Eq.Quantity,Ser_Eq.ChargeId,M_Chr.ChargeName,Ser_Eq.UomId,M_Uo.UomName,Ser_Eq.GLId,Ser_Eq.Mafi,Ser_Eq.Others,Ser_Eq.ForkliftChargeId,M_Chr1.ChargeName AS ForkliftChargeName,Ser_Eq.CraneChargeId,M_Chr2.ChargeName AS CraneChargeName,Ser_Eq.StevedorChargeId,M_Chr3.ChargeName AS StevedorChargeName,Ser_Eq.LoadingRefNo,Ser_Eq.Craneloading,Ser_Eq.Forkliftloading,Ser_Eq.Stevedorloading,Ser_Eq.OffloadingRefNo,Ser_Eq.CraneOffloading,Ser_Eq.ForkliftOffloading,Ser_Eq.StevedorOffloading,Ser_Eq.MorningTimeIn,Ser_Eq.MorningTimeOut,Ser_Eq.MorningTotalHours,Ser_Eq.EveningTimeIn,Ser_Eq.EveningTimeOut,Ser_Eq.EveningTotalHours,Ser_Eq.TotalRegularHours,Ser_Eq.TotalOvertimeHours,Ser_Eq.DriverName,Ser_Eq.VehicleName,Ser_Eq.Remarks,Ser_Eq.StatusId,M_Or.OrderTypeName AS StatusName,Ser_Eq.IsEquimentFooter,Ser_Eq.EquimentFooter,Ser_Eq.DebitNoteId,Ser_Eq.DebitNoteNo,Ser_Eq.CreateById,Ser_Eq.CreateDate,Ser_Eq.EditById,Ser_Eq.EditDate,Ser_Eq.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_EquipmentsUsed Ser_Eq LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Eq.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Eq.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Eq.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Eq.ChargeId AND M_Chr.TaskId = Ser_Eq.TaskId LEFT JOIN dbo.M_Charge M_Chr1 ON M_Chr.ChargeId = Ser_Eq.ForkliftChargeId LEFT JOIN dbo.M_Charge M_Chr2 ON M_Chr.ChargeId = Ser_Eq.CraneChargeId LEFT JOIN dbo.M_Charge M_Chr3 ON M_Chr.ChargeId = Ser_Eq.StevedorChargeId  LEFT JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Eq.UomId WHERE Ser_Eq.JobOrderId={JobOrderId} AND Ser_Eq.EquipmentsUsedId={EquipmentsUsedId} AND Ser_Eq.CompanyId={CompanyId}");

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
                    TblName = "Ser_EquipmentsUsed",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveEquipmentsUsedAsync(short CompanyId, short UserId, Ser_EquipmentsUsed ser_EquipmentsUsed)
        {
            bool IsEdit = ser_EquipmentsUsed.EquipmentsUsedId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_EquipmentsUsed WHERE EquipmentsUsedId=@EquipmentsUsedId",
                     new { ser_EquipmentsUsed.EquipmentsUsedId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_EquipmentsUsed);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "EquipmentsUsed Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (EquipmentsUsedId + 1) FROM dbo.Ser_EquipmentsUsed WHERE (EquipmentsUsedId + 1) NOT IN (SELECT EquipmentsUsedId FROM dbo.Ser_EquipmentsUsed)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_EquipmentsUsed.EquipmentsUsedId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_EquipmentsUsed.EditById = null;
                            ser_EquipmentsUsed.EditDate = null;
                            _context.Add(ser_EquipmentsUsed);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_EquipmentsUsed set EditVersion=EditVersion+1 WHERE EquipmentsUsedId={ser_EquipmentsUsed.EquipmentsUsedId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_EquipmentsUsed.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_EquipmentsUsed.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_EquipmentsUsed.JobOrderId} AND TaskId = {ser_EquipmentsUsed.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_EquipmentsUsed.JobOrderId} AND TaskId = {ser_EquipmentsUsed.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_EquipmentsUsed.JobOrderId},'{ser_EquipmentsUsed.JobOrderNo}',@ItemNo,{ser_EquipmentsUsed.TaskId},@TaskItemNo,{ser_EquipmentsUsed.EquipmentsUsedId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_EquipmentsUsed.EquipmentsUsedId,
                            DocumentNo = "",
                            TblName = "Ser_EquipmentsUsed",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "EquipmentsUsed Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_EquipmentsUsed",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_EquipmentsUsed.EquipmentsUsedId,
                    DocumentNo = "",
                    TblName = "Ser_EquipmentsUsed",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteEquipmentsUsedAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 equipmentUsedId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (equipmentUsedId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={equipmentUsedId}");

                        var deleteEquipmentsUsedResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_EquipmentsUsed WHERE CompanyId={CompanyId} AND EquipmentsUsedId={equipmentUsedId}");

                        if (deletejobOrderResult > 0 && deleteEquipmentsUsedResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = equipmentUsedId,
                                DocumentNo = "",
                                TblName = "Ser_EquipmentsUsed",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "EquipmentsUsed Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "EquipmentsUsedId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = equipmentUsedId,
                    DocumentNo = "",
                    TblName = "Ser_EquipmentsUsed",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = equipmentUsedId,
                    DocumentNo = "",
                    TblName = "Ser_EquipmentsUsed",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Equipments Used

        #region Crew SignOn

        public async Task<CrewSignOnViewModelCount> GetCrewSignOnListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            CrewSignOnViewModelCount countViewModel = new CrewSignOnViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_CrewSignOn Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.M_OrderType M_Or1 ON M_Or1.OrderTypeId = Ser_Sign.VisaTypeId LEFT JOIN dbo.M_OrderType M_Or2 ON M_Or2.OrderTypeId = Ser_Sign.RankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<CrewSignOnViewModel>($"SELECT Ser_Sign.CrewSignOnId,Ser_Sign.Date,Ser_Sign.CompanyId,Ser_Sign.JobOrderId,Ser_Sign.JobOrderNo,Ser_Sign.TaskId,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Sign.GLId,Ser_Sign.VisaTypeId,M_Or1.OrderTypeName As VisaName,Ser_Sign.CrewName,Ser_Sign.Nationality,Ser_Sign.RankId,Ser_Sign.FlightDetails,Ser_Sign.HotelName,Ser_Sign.TicketNo,Ser_Sign.TransportName,Ser_Sign.Clearing   ,Ser_Sign.StatusId,M_Or.OrderTypeName As StatusName,Ser_Sign.DebitNoteId,Ser_Sign.DebitNoteNo,Ser_Sign.TotAmt,Ser_Sign.GstAmt,Ser_Sign.TotAmtAftGst,Ser_Sign.Remarks,Ser_Sign.CreateById,Ser_Sign.CreateDate,Ser_Sign.EditById,Ser_Sign.EditDate,Ser_Sign.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewSignOn Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.M_OrderType M_Or1 ON M_Or1.OrderTypeId = Ser_Sign.VisaTypeId LEFT JOIN dbo.M_OrderType M_Or2 ON M_Or2.OrderTypeId = Ser_Sign.RankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<CrewSignOnViewModel>();

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
                    TblName = "Ser_CrewSignOn",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CrewSignOnViewModel> GetCrewSignOnByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 CrewSignOnId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CrewSignOnViewModel>($"SELECT Ser_Sign.CrewSignOnId,Ser_Sign.Date,Ser_Sign.CompanyId,Ser_Sign.JobOrderId,Ser_Sign.JobOrderNo,Ser_Sign.TaskId,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Sign.GLId,Ser_Sign.VisaTypeId,M_Or1.OrderTypeName As VisaName,Ser_Sign.CrewName,Ser_Sign.Nationality,Ser_Sign.RankId,Ser_Sign.FlightDetails,Ser_Sign.HotelName,Ser_Sign.TicketNo,Ser_Sign.TransportName,Ser_Sign.Clearing   ,Ser_Sign.StatusId,M_Or.OrderTypeName As StatusName,Ser_Sign.DebitNoteId,Ser_Sign.DebitNoteNo,Ser_Sign.TotAmt,Ser_Sign.GstAmt,Ser_Sign.TotAmtAftGst,Ser_Sign.Remarks,Ser_Sign.CreateById,Ser_Sign.CreateDate,Ser_Sign.EditById,Ser_Sign.EditDate,Ser_Sign.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewSignOn Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.M_OrderType M_Or1 ON M_Or1.OrderTypeId = Ser_Sign.VisaTypeId LEFT JOIN dbo.M_OrderType M_Or2 ON M_Or2.OrderTypeId = Ser_Sign.RankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CrewSignOnId={CrewSignOnId} AND Ser_Sign.CompanyId={CompanyId}");

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
                    TblName = "Ser_CrewSignOn",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCrewSignOnAsync(short CompanyId, short UserId, Ser_CrewSignOn ser_CrewSignOn)
        {
            bool IsEdit = ser_CrewSignOn.CrewSignOnId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_CrewSignOn WHERE CrewSignOnId=@CrewSignOnId",
                     new { ser_CrewSignOn.CrewSignOnId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_CrewSignOn);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "CrewSignOn Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (CrewSignOnId + 1) FROM dbo.Ser_CrewSignOn WHERE (CrewSignOnId + 1) NOT IN (SELECT CrewSignOnId FROM dbo.Ser_CrewSignOn)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_CrewSignOn.CrewSignOnId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_CrewSignOn.EditById = null;
                            ser_CrewSignOn.EditDate = null;
                            _context.Add(ser_CrewSignOn);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_CrewSignOn set EditVersion=EditVersion+1 WHERE CrewSignOnId={ser_CrewSignOn.CrewSignOnId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewSignOn.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewSignOn.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewSignOn.JobOrderId} AND TaskId = {ser_CrewSignOn.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewSignOn.JobOrderId} AND TaskId = {ser_CrewSignOn.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_CrewSignOn.JobOrderId},'{ser_CrewSignOn.JobOrderNo}',@ItemNo,{ser_CrewSignOn.TaskId},@TaskItemNo,{ser_CrewSignOn.CrewSignOnId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_CrewSignOn.CrewSignOnId,
                            DocumentNo = "",
                            TblName = "Ser_CrewSignOn",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CrewSignOn Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_CrewSignOn",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_CrewSignOn.CrewSignOnId,
                    DocumentNo = "",
                    TblName = "Ser_CrewSignOn",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteCrewSignOnAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 crewSignOnId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (crewSignOnId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={crewSignOnId}");

                        var deleteCrewSignOnResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_CrewSignOn WHERE CompanyId={CompanyId} AND CrewSignOnId={crewSignOnId}");

                        if (deletejobOrderResult > 0 && deleteCrewSignOnResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = crewSignOnId,
                                DocumentNo = "",
                                TblName = "Ser_CrewSignOn",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CrewSignOn Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CrewSignOnId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = crewSignOnId,
                    DocumentNo = "",
                    TblName = "Ser_CrewSignOn",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = crewSignOnId,
                    DocumentNo = "",
                    TblName = "Ser_CrewSignOn",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Crew SignOn

        #region Crew SignOff

        public async Task<CrewSignOffViewModelCount> GetCrewSignOffListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            CrewSignOffViewModelCount countViewModel = new CrewSignOffViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_CrewSignOff Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.M_OrderType M_Or1 ON M_Or1.OrderTypeId = Ser_Sign.VisaTypeId LEFT JOIN dbo.M_OrderType M_Or2 ON M_Or2.OrderTypeId = Ser_Sign.RankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<CrewSignOffViewModel>($"SELECT Ser_Sign.CrewSignOffId,Ser_Sign.Date,Ser_Sign.CompanyId,Ser_Sign.JobOrderId,Ser_Sign.JobOrderNo,Ser_Sign.TaskId,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Sign.GLId,Ser_Sign.VisaTypeId,M_Or1.OrderTypeName As VisaName,Ser_Sign.CrewName,Ser_Sign.Nationality,Ser_Sign.RankId,Ser_Sign.FlightDetails,Ser_Sign.HotelName,Ser_Sign.TicketNo,Ser_Sign.TransportName,Ser_Sign.Clearing   ,Ser_Sign.StatusId,M_Or.OrderTypeName As StatusName,Ser_Sign.DebitNoteId,Ser_Sign.DebitNoteNo,Ser_Sign.TotAmt,Ser_Sign.GstAmt,Ser_Sign.TotAmtAftGst,Ser_Sign.Remarks,Ser_Sign.CreateById,Ser_Sign.CreateDate,Ser_Sign.EditById,Ser_Sign.EditDate,Ser_Sign.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewSignOff Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.M_OrderType M_Or1 ON M_Or1.OrderTypeId = Ser_Sign.VisaTypeId LEFT JOIN dbo.M_OrderType M_Or2 ON M_Or2.OrderTypeId = Ser_Sign.RankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<CrewSignOffViewModel>();

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
                    TblName = "Ser_CrewSignOff",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CrewSignOffViewModel> GetCrewSignOffByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 CrewSignOffId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CrewSignOffViewModel>($"SELECT Ser_Sign.CrewSignOffId,Ser_Sign.Date,Ser_Sign.CompanyId,Ser_Sign.JobOrderId,Ser_Sign.JobOrderNo,Ser_Sign.TaskId,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Sign.GLId,Ser_Sign.VisaTypeId,M_Or1.OrderTypeName As VisaName,Ser_Sign.CrewName,Ser_Sign.Nationality,Ser_Sign.RankId,Ser_Sign.FlightDetails,Ser_Sign.HotelName,Ser_Sign.TicketNo,Ser_Sign.TransportName,Ser_Sign.Clearing   ,Ser_Sign.StatusId,M_Or.OrderTypeName As StatusName,Ser_Sign.DebitNoteId,Ser_Sign.DebitNoteNo,Ser_Sign.TotAmt,Ser_Sign.GstAmt,Ser_Sign.TotAmtAftGst,Ser_Sign.Remarks,Ser_Sign.CreateById,Ser_Sign.CreateDate,Ser_Sign.EditById,Ser_Sign.EditDate,Ser_Sign.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewSignOff Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.M_OrderType M_Or1 ON M_Or1.OrderTypeId = Ser_Sign.VisaTypeId LEFT JOIN dbo.M_OrderType M_Or2 ON M_Or2.OrderTypeId = Ser_Sign.RankId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CrewSignOffId={CrewSignOffId} AND Ser_Sign.CompanyId={CompanyId}");

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
                    TblName = "Ser_CrewSignOff",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCrewSignOffAsync(short CompanyId, short UserId, Ser_CrewSignOff ser_CrewSignOff)
        {
            bool IsEdit = ser_CrewSignOff.CrewSignOffId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_CrewSignOff WHERE CrewSignOffId=@CrewSignOffId",
                     new { ser_CrewSignOff.CrewSignOffId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_CrewSignOff);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "CrewSignOff Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (CrewSignOffId + 1) FROM dbo.Ser_CrewSignOff WHERE (CrewSignOffId + 1) NOT IN (SELECT CrewSignOffId FROM dbo.Ser_CrewSignOff)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_CrewSignOff.CrewSignOffId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_CrewSignOff.EditById = null;
                            ser_CrewSignOff.EditDate = null;
                            _context.Add(ser_CrewSignOff);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_CrewSignOff set EditVersion=EditVersion+1 WHERE CrewSignOffId={ser_CrewSignOff.CrewSignOffId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewSignOff.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewSignOff.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewSignOff.JobOrderId} AND TaskId = {ser_CrewSignOff.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewSignOff.JobOrderId} AND TaskId = {ser_CrewSignOff.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_CrewSignOff.JobOrderId},'{ser_CrewSignOff.JobOrderNo}',@ItemNo,{ser_CrewSignOff.TaskId},@TaskItemNo,{ser_CrewSignOff.CrewSignOffId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_CrewSignOff.CrewSignOffId,
                            DocumentNo = "",
                            TblName = "Ser_CrewSignOff",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CrewSignOff Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_CrewSignOff",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_CrewSignOff.CrewSignOffId,
                    DocumentNo = "",
                    TblName = "Ser_CrewSignOff",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteCrewSignOffAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 crewSignOffId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (crewSignOffId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={crewSignOffId}");

                        var deleteCrewSignOffResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_CrewSignOff WHERE CompanyId={CompanyId} AND CrewSignOffId={crewSignOffId}");

                        if (deletejobOrderResult > 0 && deleteCrewSignOffResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = crewSignOffId,
                                DocumentNo = "",
                                TblName = "Ser_CrewSignOff",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CrewSignOff Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CrewSignOffId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = crewSignOffId,
                    DocumentNo = "",
                    TblName = "Ser_CrewSignOff",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = crewSignOffId,
                    DocumentNo = "",
                    TblName = "Ser_CrewSignOff",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Crew SignOff

        #region Crew Miscellaneous

        public async Task<CrewMiscellaneousViewModelCount> GetCrewMiscellaneousListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            CrewMiscellaneousViewModelCount countViewModel = new CrewMiscellaneousViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_CrewMiscellaneous Ser_Misc LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Misc.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Misc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Misc.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Misc.ChargeId AND M_Chr.TaskId = Ser_Misc.TaskId WHERE Ser_Misc.JobOrderId={JobOrderId} AND Ser_Misc.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<CrewMiscellaneousViewModel>($"SELECT Ser_Misc.CrewMiscellaneousId,Ser_Misc.CompanyId,Ser_Misc.JobOrderId,Ser_Misc.JobOrderNo,Ser_Misc.TaskId,Ser_Misc.MiscDate,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Misc.GLId,Ser_Misc.StatusId,M_Or.OrderTypeName As StatusName,Ser_Misc.DebitNoteId,Ser_Misc.DebitNoteNo,Ser_Misc.TotAmt,Ser_Misc.GstAmt,Ser_Misc.TotAmtAftGst,Ser_Misc.Remarks,Ser_Misc.CreateById,Ser_Misc.CreateDate,Ser_Misc.EditById,Ser_Misc.EditDate,Ser_Misc.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewMiscellaneous Ser_Misc LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Misc.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Misc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Misc.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Misc.ChargeId AND M_Chr.TaskId = Ser_Misc.TaskId WHERE Ser_Misc.JobOrderId={JobOrderId} AND Ser_Misc.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<CrewMiscellaneousViewModel>();

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
                    TblName = "Ser_CrewMiscellaneous",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<CrewMiscellaneousViewModel> GetCrewMiscellaneousByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 CrewMiscellaneousId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CrewMiscellaneousViewModel>($"SELECT Ser_Misc.CrewMiscellaneousId,Ser_Misc.CompanyId,Ser_Misc.JobOrderId,Ser_Misc.JobOrderNo,Ser_Misc.TaskId,Ser_Misc.MiscDate,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Misc.GLId,Ser_Misc.StatusId,M_Or.OrderTypeName As StatusName,Ser_Misc.DebitNoteId,Ser_Misc.DebitNoteNo,Ser_Misc.TotAmt,Ser_Misc.GstAmt,Ser_Misc.TotAmtAftGst,Ser_Misc.Remarks,Ser_Misc.CreateById,Ser_Misc.CreateDate,Ser_Misc.EditById,Ser_Misc.EditDate,Ser_Misc.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewMiscellaneous Ser_Misc LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Misc.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Misc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Misc.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Misc.ChargeId AND M_Chr.TaskId = Ser_Misc.TaskId WHERE Ser_Misc.JobOrderId={JobOrderId} AND Ser_Misc.CrewMiscellaneousId={CrewMiscellaneousId} AND Ser_Misc.CompanyId={CompanyId}");

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
                    TblName = "Ser_CrewMiscellaneous",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveCrewMiscellaneousAsync(short CompanyId, short UserId, Ser_CrewMiscellaneous ser_CrewMiscellaneous)
        {
            bool IsEdit = ser_CrewMiscellaneous.CrewMiscellaneousId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_CrewMiscellaneous WHERE CrewMiscellaneousId=@CrewMiscellaneousId",
                     new { ser_CrewMiscellaneous.CrewMiscellaneousId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_CrewMiscellaneous);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "CrewMiscellaneous Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (CrewMiscellaneousId + 1) FROM dbo.Ser_CrewMiscellaneous WHERE (CrewMiscellaneousId + 1) NOT IN (SELECT CrewMiscellaneousId FROM dbo.Ser_CrewMiscellaneous)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_CrewMiscellaneous.CrewMiscellaneousId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_CrewMiscellaneous.EditById = null;
                            ser_CrewMiscellaneous.EditDate = null;
                            _context.Add(ser_CrewMiscellaneous);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_CrewMiscellaneous set EditVersion=EditVersion+1 WHERE CrewMiscellaneousId={ser_CrewMiscellaneous.CrewMiscellaneousId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewMiscellaneous.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewMiscellaneous.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewMiscellaneous.JobOrderId} AND TaskId = {ser_CrewMiscellaneous.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewMiscellaneous.JobOrderId} AND TaskId = {ser_CrewMiscellaneous.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_CrewMiscellaneous.JobOrderId},'{ser_CrewMiscellaneous.JobOrderNo}',@ItemNo,{ser_CrewMiscellaneous.TaskId},@TaskItemNo,{ser_CrewMiscellaneous.CrewMiscellaneousId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_CrewMiscellaneous.CrewMiscellaneousId,
                            DocumentNo = "",
                            TblName = "Ser_CrewMiscellaneous",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "CrewMiscellaneous Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_CrewMiscellaneous",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_CrewMiscellaneous.CrewMiscellaneousId,
                    DocumentNo = "",
                    TblName = "Ser_CrewMiscellaneous",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteCrewMiscellaneousAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 crewMiscId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (crewMiscId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={crewMiscId}");

                        var deleteCrewMiscellaneousResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_CrewMiscellaneous WHERE CompanyId={CompanyId} AND CrewMiscellaneousId={crewMiscId}");

                        if (deletejobOrderResult > 0 && deleteCrewMiscellaneousResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = crewMiscId,
                                DocumentNo = "",
                                TblName = "Ser_CrewMiscellaneous",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "CrewMiscellaneous Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "CrewMiscellaneousId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = crewMiscId,
                    DocumentNo = "",
                    TblName = "Ser_CrewMiscellaneous",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = crewMiscId,
                    DocumentNo = "",
                    TblName = "Ser_CrewMiscellaneous",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Crew Miscellaneous

        #region Medical Assistance

        public async Task<MedicalAssistanceViewModelCount> GetMedicalAssistanceListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            MedicalAssistanceViewModelCount countViewModel = new MedicalAssistanceViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_MedicalAssistance Ser_Med LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Med.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Med.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Med.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Med.ChargeId AND M_Chr.TaskId = Ser_Med.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Med.CrewId WHERE Ser_Med.JobOrderId={JobOrderId} AND Ser_Med.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<MedicalAssistanceViewModel>($"SELECT Ser_Med.MedicalAssistanceId,Ser_Med.CompanyId,Ser_Med.JobOrderId,Ser_Med.JobOrderNo,Ser_Med.TaskId,Ser_Med.MedicalDate,Ser_Med.CrewId,M_Cr.CrewName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Med.GLId,Ser_Med.StatusId,M_Or.OrderTypeName As StatusName,Ser_Med.DebitNoteId,Ser_Med.DebitNoteNo,Ser_Med.TotAmt,Ser_Med.GstAmt,Ser_Med.TotAmtAftGst,Ser_Med.Remarks,Ser_Med.CreateById,Ser_Med.CreateDate,Ser_Med.EditById,Ser_Med.EditDate,Ser_Med.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_MedicalAssistance Ser_Med LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Med.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Med.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Med.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Med.ChargeId AND M_Chr.TaskId = Ser_Med.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Med.CrewId WHERE Ser_Med.JobOrderId={JobOrderId} AND Ser_Med.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<MedicalAssistanceViewModel>();

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
                    TblName = "Ser_MedicalAssistance",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<MedicalAssistanceViewModel> GetMedicalAssistanceByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 MedicalAssistanceId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<MedicalAssistanceViewModel>($"SELECT Ser_Med.MedicalAssistanceId,Ser_Med.CompanyId,Ser_Med.JobOrderId,Ser_Med.JobOrderNo,Ser_Med.TaskId,Ser_Med.MedicalDate,Ser_Med.CrewId,M_Cr.CrewName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Med.GLId,Ser_Med.StatusId,M_Or.OrderTypeName As StatusName,Ser_Med.DebitNoteId,Ser_Med.DebitNoteNo,Ser_Med.TotAmt,Ser_Med.GstAmt,Ser_Med.TotAmtAftGst,Ser_Med.Remarks,Ser_Med.CreateById,Ser_Med.CreateDate,Ser_Med.EditById,Ser_Med.EditDate,Ser_Med.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_MedicalAssistance Ser_Med LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Med.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Med.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Med.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Med.ChargeId AND M_Chr.TaskId = Ser_Med.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Med.CrewId WHERE Ser_Med.JobOrderId={JobOrderId} AND Ser_Med.MedicalAssistanceId={MedicalAssistanceId} AND Ser_Med.CompanyId={CompanyId}");

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
                    TblName = "Ser_MedicalAssistance",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveMedicalAssistanceAsync(short CompanyId, short UserId, Ser_MedicalAssistance ser_MedicalAssistance)
        {
            bool IsEdit = ser_MedicalAssistance.MedicalAssistanceId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_MedicalAssistance WHERE MedicalAssistanceId=@MedicalAssistanceId",
                     new { ser_MedicalAssistance.MedicalAssistanceId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_MedicalAssistance);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "MedicalAssistance Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (MedicalAssistanceId + 1) FROM dbo.Ser_MedicalAssistance WHERE (MedicalAssistanceId + 1) NOT IN (SELECT MedicalAssistanceId FROM dbo.Ser_MedicalAssistance)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_MedicalAssistance.MedicalAssistanceId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_MedicalAssistance.EditById = null;
                            ser_MedicalAssistance.EditDate = null;
                            _context.Add(ser_MedicalAssistance);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_MedicalAssistance set EditVersion=EditVersion+1 WHERE MedicalAssistanceId={ser_MedicalAssistance.MedicalAssistanceId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_MedicalAssistance.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_MedicalAssistance.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_MedicalAssistance.JobOrderId} AND TaskId = {ser_MedicalAssistance.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_MedicalAssistance.JobOrderId} AND TaskId = {ser_MedicalAssistance.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_MedicalAssistance.JobOrderId},'{ser_MedicalAssistance.JobOrderNo}',@ItemNo,{ser_MedicalAssistance.TaskId},@TaskItemNo,{ser_MedicalAssistance.MedicalAssistanceId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_MedicalAssistance.MedicalAssistanceId,
                            DocumentNo = "",
                            TblName = "Ser_MedicalAssistance",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "MedicalAssistance Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_MedicalAssistance",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_MedicalAssistance.MedicalAssistanceId,
                    DocumentNo = "",
                    TblName = "Ser_MedicalAssistance",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteMedicalAssistanceAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 medicalAssistId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (medicalAssistId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={medicalAssistId}");

                        var deleteMedicalAssistanceResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_MedicalAssistance WHERE CompanyId={CompanyId} AND MedicalAssistanceId={medicalAssistId}");

                        if (deletejobOrderResult > 0 && deleteMedicalAssistanceResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = medicalAssistId,
                                DocumentNo = "",
                                TblName = "Ser_MedicalAssistance",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "MedicalAssistance Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "MedicalAssistanceId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = medicalAssistId,
                    DocumentNo = "",
                    TblName = "Ser_MedicalAssistance",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = medicalAssistId,
                    DocumentNo = "",
                    TblName = "Ser_MedicalAssistance",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Medical Assistance

        #region Consignment Import

        public async Task<ConsignmentImportViewModelCount> GetConsignmentImportListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            ConsignmentImportViewModelCount countViewModel = new ConsignmentImportViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_ConsignmentImport Ser_Cons LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Cons.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Cons.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Cons.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Cons.ChargeId AND M_Chr.TaskId = Ser_Cons.TaskId WHERE Ser_Cons.JobOrderId={JobOrderId} AND Ser_Cons.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<ConsignmentImportViewModel>($"SELECT Ser_Cons.ConsignmentImportId,Ser_Cons.CompanyId,Ser_Cons.JobOrderId,Ser_Cons.JobOrderNo,Ser_Cons.TaskId,Ser_Cons.ImportDate,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Cons.GLId,Ser_Cons.StatusId,M_Or.OrderTypeName As StatusName,Ser_Cons.DebitNoteId,Ser_Cons.DebitNoteNo,Ser_Cons.TotAmt,Ser_Cons.GstAmt,Ser_Cons.TotAmtAftGst,Ser_Cons.Remarks,Ser_Cons.CreateById,Ser_Cons.CreateDate,Ser_Cons.EditById,Ser_Cons.EditDate,Ser_Cons.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_ConsignmentImport Ser_Cons LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Cons.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Cons.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Cons.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Cons.ChargeId AND M_Chr.TaskId = Ser_Cons.TaskId WHERE Ser_Cons.JobOrderId={JobOrderId} AND Ser_Cons.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<ConsignmentImportViewModel>();

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
                    TblName = "Ser_ConsignmentImport",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ConsignmentImportViewModel> GetConsignmentImportByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 ConsignmentImportId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<ConsignmentImportViewModel>($"SELECT Ser_Cons.ConsignmentImportId,Ser_Cons.CompanyId,Ser_Cons.JobOrderId,Ser_Cons.JobOrderNo,Ser_Cons.TaskId,Ser_Cons.ImportDate,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Cons.GLId,Ser_Cons.StatusId,M_Or.OrderTypeName As StatusName,Ser_Cons.DebitNoteId,Ser_Cons.DebitNoteNo,Ser_Cons.TotAmt,Ser_Cons.GstAmt,Ser_Cons.TotAmtAftGst,Ser_Cons.Remarks,Ser_Cons.CreateById,Ser_Cons.CreateDate,Ser_Cons.EditById,Ser_Cons.EditDate,Ser_Cons.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_ConsignmentImport Ser_Cons LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Cons.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Cons.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Cons.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Cons.ChargeId AND M_Chr.TaskId = Ser_Cons.TaskId WHERE Ser_Cons.JobOrderId={JobOrderId} AND Ser_Cons.ConsignmentImportId={ConsignmentImportId} AND Ser_Cons.CompanyId={CompanyId}");

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
                    TblName = "Ser_ConsignmentImport",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveConsignmentImportAsync(short CompanyId, short UserId, Ser_ConsignmentImport ser_ConsignmentImport)
        {
            bool IsEdit = ser_ConsignmentImport.ConsignmentImportId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_ConsignmentImport WHERE ConsignmentImportId=@ConsignmentImportId",
                     new { ser_ConsignmentImport.ConsignmentImportId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_ConsignmentImport);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "ConsignmentImport Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (ConsignmentImportId + 1) FROM dbo.Ser_ConsignmentImport WHERE (ConsignmentImportId + 1) NOT IN (SELECT ConsignmentImportId FROM dbo.Ser_ConsignmentImport)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_ConsignmentImport.ConsignmentImportId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_ConsignmentImport.EditById = null;
                            ser_ConsignmentImport.EditDate = null;
                            _context.Add(ser_ConsignmentImport);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_ConsignmentImport set EditVersion=EditVersion+1 WHERE ConsignmentImportId={ser_ConsignmentImport.ConsignmentImportId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ConsignmentImport.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ConsignmentImport.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ConsignmentImport.JobOrderId} AND TaskId = {ser_ConsignmentImport.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ConsignmentImport.JobOrderId} AND TaskId = {ser_ConsignmentImport.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_ConsignmentImport.JobOrderId},'{ser_ConsignmentImport.JobOrderNo}',@ItemNo,{ser_ConsignmentImport.TaskId},@TaskItemNo,{ser_ConsignmentImport.ConsignmentImportId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_ConsignmentImport.ConsignmentImportId,
                            DocumentNo = "",
                            TblName = "Ser_ConsignmentImport",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "ConsignmentImport Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_ConsignmentImport",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_ConsignmentImport.ConsignmentImportId,
                    DocumentNo = "",
                    TblName = "Ser_ConsignmentImport",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteConsignmentImportAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 consignmentImportId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (consignmentImportId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={consignmentImportId}");

                        var deleteConsignmentImportResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_ConsignmentImport WHERE CompanyId={CompanyId} AND ConsignmentImportId={consignmentImportId}");

                        if (deletejobOrderResult > 0 && deleteConsignmentImportResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = consignmentImportId,
                                DocumentNo = "",
                                TblName = "Ser_ConsignmentImport",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "ConsignmentImport Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "ConsignmentImportId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = consignmentImportId,
                    DocumentNo = "",
                    TblName = "Ser_ConsignmentImport",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = consignmentImportId,
                    DocumentNo = "",
                    TblName = "Ser_ConsignmentImport",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Consignment Import

        #region Consignment Export

        public async Task<ConsignmentExportViewModelCount> GetConsignmentExportListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            ConsignmentExportViewModelCount countViewModel = new ConsignmentExportViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_ConsignmentExport Ser_Cons LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Cons.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Cons.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Cons.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Cons.ChargeId AND M_Chr.TaskId = Ser_Cons.TaskId WHERE Ser_Cons.JobOrderId={JobOrderId} AND Ser_Cons.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<ConsignmentExportViewModel>($"SELECT Ser_Cons.ConsignmentExportId,Ser_Cons.CompanyId,Ser_Cons.JobOrderId,Ser_Cons.JobOrderNo,Ser_Cons.TaskId,Ser_Cons.ExportDate,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Cons.GLId,Ser_Cons.StatusId,M_Or.OrderTypeName As StatusName,Ser_Cons.DebitNoteId,Ser_Cons.DebitNoteNo,Ser_Cons.TotAmt,Ser_Cons.GstAmt,Ser_Cons.TotAmtAftGst,Ser_Cons.Remarks,Ser_Cons.CreateById,Ser_Cons.CreateDate,Ser_Cons.EditById,Ser_Cons.EditDate,Ser_Cons.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_ConsignmentExport Ser_Cons LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Cons.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Cons.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Cons.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Cons.ChargeId AND M_Chr.TaskId = Ser_Cons.TaskId WHERE Ser_Cons.JobOrderId={JobOrderId} AND Ser_Cons.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<ConsignmentExportViewModel>();

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
                    TblName = "Ser_ConsignmentExport",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ConsignmentExportViewModel> GetConsignmentExportByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 ConsignmentExportId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<ConsignmentExportViewModel>($"SELECT Ser_Cons.ConsignmentExportId,Ser_Cons.CompanyId,Ser_Cons.JobOrderId,Ser_Cons.JobOrderNo,Ser_Cons.TaskId,Ser_Cons.ExportDate,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Cons.GLId,Ser_Cons.StatusId,M_Or.OrderTypeName As StatusName,Ser_Cons.DebitNoteId,Ser_Cons.DebitNoteNo,Ser_Cons.TotAmt,Ser_Cons.GstAmt,Ser_Cons.TotAmtAftGst,Ser_Cons.Remarks,Ser_Cons.CreateById,Ser_Cons.CreateDate,Ser_Cons.EditById,Ser_Cons.EditDate,Ser_Cons.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_ConsignmentExport Ser_Cons LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Cons.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Cons.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Cons.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Cons.ChargeId AND M_Chr.TaskId = Ser_Cons.TaskId WHERE Ser_Cons.JobOrderId={JobOrderId} AND Ser_Cons.ConsignmentExportId={ConsignmentExportId} AND Ser_Cons.CompanyId={CompanyId}");

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
                    TblName = "Ser_ConsignmentExport",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveConsignmentExportAsync(short CompanyId, short UserId, Ser_ConsignmentExport ser_ConsignmentExport)
        {
            bool IsEdit = ser_ConsignmentExport.ConsignmentExportId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_ConsignmentExport WHERE ConsignmentExportId=@ConsignmentExportId",
                     new { ser_ConsignmentExport.ConsignmentExportId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_ConsignmentExport);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "ConsignmentExport Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (ConsignmentExportId + 1) FROM dbo.Ser_ConsignmentExport WHERE (ConsignmentExportId + 1) NOT IN (SELECT ConsignmentExportId FROM dbo.Ser_ConsignmentExport)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_ConsignmentExport.ConsignmentExportId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_ConsignmentExport.EditById = null;
                            ser_ConsignmentExport.EditDate = null;
                            _context.Add(ser_ConsignmentExport);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_ConsignmentExport set EditVersion=EditVersion+1 WHERE ConsignmentExportId={ser_ConsignmentExport.ConsignmentExportId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ConsignmentExport.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ConsignmentExport.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ConsignmentExport.JobOrderId} AND TaskId = {ser_ConsignmentExport.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ConsignmentExport.JobOrderId} AND TaskId = {ser_ConsignmentExport.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_ConsignmentExport.JobOrderId},'{ser_ConsignmentExport.JobOrderNo}',@ItemNo,{ser_ConsignmentExport.TaskId},@TaskItemNo,{ser_ConsignmentExport.ConsignmentExportId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_ConsignmentExport.ConsignmentExportId,
                            DocumentNo = "",
                            TblName = "Ser_ConsignmentExport",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "ConsignmentExport Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_ConsignmentExport",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_ConsignmentExport.ConsignmentExportId,
                    DocumentNo = "",
                    TblName = "Ser_ConsignmentExport",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteConsignmentExportAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 consignmentExportId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (consignmentExportId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={consignmentExportId}");

                        var deleteConsignmentExportResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_ConsignmentExport WHERE CompanyId={CompanyId} AND ConsignmentExportId={consignmentExportId}");

                        if (deletejobOrderResult > 0 && deleteConsignmentExportResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = consignmentExportId,
                                DocumentNo = "",
                                TblName = "Ser_ConsignmentExport",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "ConsignmentExport Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "ConsignmentExportId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = consignmentExportId,
                    DocumentNo = "",
                    TblName = "Ser_ConsignmentExport",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = consignmentExportId,
                    DocumentNo = "",
                    TblName = "Ser_ConsignmentExport",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Consignment Export

        #region Third Party

        public async Task<ThirdPartyViewModelCount> GetThirdPartyListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            ThirdPartyViewModelCount countViewModel = new ThirdPartyViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_ThirdParty Ser_Tps LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tps.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Tps.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Tps.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Tps.ChargeId AND M_Chr.TaskId = Ser_Tps.TaskId LEFT JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Ser_Tps.SupplierId WHERE Ser_Tps.JobOrderId={JobOrderId} AND Ser_Tps.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<ThirdPartyViewModel>($"SELECT Ser_Tps.ThirdPartyId,Ser_Tps.CompanyId,Ser_Tps.JobOrderId,Ser_Tps.JobOrderNo,Ser_Tps.TaskId,Ser_Tps.Date,Ser_Tps.SupplierId,M_Sup.SupplierName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Tps.GLId,Ser_Tps.StatusId,M_Or.OrderTypeName As StatusName,Ser_Tps.DebitNoteId,Ser_Tps.DebitNoteNo,Ser_Tps.TotAmt,Ser_Tps.GstAmt,Ser_Tps.TotAmtAftGst,Ser_Tps.Remarks,Ser_Tps.CreateById,Ser_Tps.CreateDate,Ser_Tps.EditById,Ser_Tps.EditDate,Ser_Tps.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_ThirdParty Ser_Tps LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tps.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Tps.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Tps.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Tps.ChargeId AND M_Chr.TaskId = Ser_Tps.TaskId LEFT JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Ser_Tps.SupplierId WHERE Ser_Tps.JobOrderId={JobOrderId} AND Ser_Tps.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<ThirdPartyViewModel>();

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
                    TblName = "Ser_ThirdParty",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ThirdPartyViewModel> GetThirdPartyByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 ThirdPartyId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<ThirdPartyViewModel>($"SELECT Ser_Tps.ThirdPartyId,Ser_Tps.CompanyId,Ser_Tps.JobOrderId,Ser_Tps.JobOrderNo,Ser_Tps.TaskId,Ser_Tps.Date,Ser_Tps.SupplierId,M_Sup.SupplierName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Tps.GLId,Ser_Tps.StatusId,M_Or.OrderTypeName As StatusName,Ser_Tps.DebitNoteId,Ser_Tps.DebitNoteNo,Ser_Tps.TotAmt,Ser_Tps.GstAmt,Ser_Tps.TotAmtAftGst,Ser_Tps.Remarks,Ser_Tps.CreateById,Ser_Tps.CreateDate,Ser_Tps.EditById,Ser_Tps.EditDate,Ser_Tps.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_ThirdParty Ser_Tps LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tps.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Tps.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Tps.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Tps.ChargeId AND M_Chr.TaskId = Ser_Tps.TaskId LEFT JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Ser_Tps.SupplierId WHERE Ser_Tps.JobOrderId={JobOrderId} AND Ser_Tps.ThirdPartyId={ThirdPartyId} AND Ser_Tps.CompanyId={CompanyId}");

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
                    TblName = "Ser_ThirdParty",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveThirdPartyAsync(short CompanyId, short UserId, Ser_ThirdParty ser_ThirdParty)
        {
            bool IsEdit = ser_ThirdParty.ThirdPartyId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_ThirdParty WHERE ThirdPartyId=@ThirdPartyId",
                     new { ser_ThirdParty.ThirdPartyId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_ThirdParty);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "ThirdParty Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (ThirdPartyId + 1) FROM dbo.Ser_ThirdParty WHERE (ThirdPartyId + 1) NOT IN (SELECT ThirdPartyId FROM dbo.Ser_ThirdParty)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_ThirdParty.ThirdPartyId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_ThirdParty.EditById = null;
                            ser_ThirdParty.EditDate = null;
                            _context.Add(ser_ThirdParty);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_ThirdParty set EditVersion=EditVersion+1 WHERE ThirdPartyId={ser_ThirdParty.ThirdPartyId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ThirdParty.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ThirdParty.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ThirdParty.JobOrderId} AND TaskId = {ser_ThirdParty.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ThirdParty.JobOrderId} AND TaskId = {ser_ThirdParty.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_ThirdParty.JobOrderId},'{ser_ThirdParty.JobOrderNo}',@ItemNo,{ser_ThirdParty.TaskId},@TaskItemNo,{ser_ThirdParty.ThirdPartyId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_ThirdParty.ThirdPartyId,
                            DocumentNo = "",
                            TblName = "Ser_ThirdParty",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "ThirdParty Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_ThirdParty",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_ThirdParty.ThirdPartyId,
                    DocumentNo = "",
                    TblName = "Ser_ThirdParty",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteThirdPartyAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 thirdPartyId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (thirdPartyId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={thirdPartyId}");

                        var deleteThirdPartyResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_ThirdParty WHERE CompanyId={CompanyId} AND ThirdPartyId={thirdPartyId}");

                        if (deletejobOrderResult > 0 && deleteThirdPartyResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = thirdPartyId,
                                DocumentNo = "",
                                TblName = "Ser_ThirdParty",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "ThirdParty Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "ThirdPartyId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = thirdPartyId,
                    DocumentNo = "",
                    TblName = "Ser_ThirdParty",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = thirdPartyId,
                    DocumentNo = "",
                    TblName = "Ser_ThirdParty",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Third Party



        #region FreshWater

        public async Task<FreshWaterViewModelCount> GetFreshWaterListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            FreshWaterViewModelCount countViewModel = new FreshWaterViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_FreshWater Ser_FWS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_FWS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_FWS.ChargeId AND M_Chr.TaskId = Ser_FWS.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_FWS.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_FWS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_FWS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_FWS.EditById WHERE Ser_FWS.JobOrderId={JobOrderId} AND Ser_FWS.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<FreshWaterViewModel>($"SELECT Ser_FWS.FreshWaterId,Ser_FWS.CompanyId,Ser_FWS.JobOrderId,Ser_FWS.JobOrderNo,Ser_FWS.TaskId,Ser_FWS.Quantity,Ser_FWS.SupplierId,M_Ser.SupplierName,Ser_FWS.ChargeId,M_Chr.ChargeName,Ser_FWS.StatusId,M_Or.OrderTypeName As StatusName,Ser_FWS.UomId,M_Uo.UomName,Ser_FWS.DeliverDate,Ser_FWS.GLId,Ser_FWS.DebitNoteId,Ser_FWS.DebitNoteNo,Ser_FWS.Remarks,Ser_FWS.CreateById,Ser_FWS.CreateDate,Ser_FWS.EditById,Ser_FWS.EditDate,Ser_FWS.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_FreshWater Ser_FWS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_FWS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_FWS.ChargeId AND M_Chr.TaskId = Ser_FWS.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_FWS.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_FWS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_FWS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_FWS.EditById WHERE Ser_FWS.JobOrderId={JobOrderId} AND Ser_FWS.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<FreshWaterViewModel>();

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
                    TblName = "Ser_FreshWater",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<FreshWaterViewModel> GetFreshWaterByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 FreshWaterId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<FreshWaterViewModel>($"SELECT Ser_FWS.FreshWaterId,Ser_FWS.CompanyId,Ser_FWS.JobOrderId,Ser_FWS.JobOrderNo,Ser_FWS.TaskId,Ser_FWS.Quantity,Ser_FWS.SupplierId,M_Ser.SupplierName,Ser_FWS.ChargeId,M_Chr.ChargeName,Ser_FWS.StatusId,M_Or.OrderTypeName As StatusName,Ser_FWS.UomId,M_Uo.UomName,Ser_FWS.DeliverDate,Ser_FWS.GLId,Ser_FWS.DebitNoteId,Ser_FWS.DebitNoteNo,Ser_FWS.Remarks,Ser_FWS.CreateById,Ser_FWS.CreateDate,Ser_FWS.EditById,Ser_FWS.EditDate,Ser_FWS.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_FreshWater Ser_FWS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_FWS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_FWS.ChargeId AND M_Chr.TaskId = Ser_FWS.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_FWS.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_FWS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_FWS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_FWS.EditById WHERE Ser_FWS.JobOrderId={JobOrderId} AND Ser_FWS.FreshWaterId={FreshWaterId} AND Ser_FWS.CompanyId={CompanyId}");

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
                    TblName = "Ser_FreshWater",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveFreshWaterAsync(short CompanyId, short UserId, Ser_FreshWater ser_FreshWater)
        {
            bool IsEdit = ser_FreshWater.FreshWaterId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_FreshWater WHERE FreshWaterId=@FreshWaterId",
                     new { ser_FreshWater.FreshWaterId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_FreshWater);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "FreshWater Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (FreshWaterId + 1) FROM dbo.Ser_FreshWater WHERE (FreshWaterId + 1) NOT IN (SELECT FreshWaterId FROM dbo.Ser_FreshWater)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_FreshWater.FreshWaterId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_FreshWater.EditById = null;
                            ser_FreshWater.EditDate = null;
                            _context.Add(ser_FreshWater);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_FreshWater set EditVersion=EditVersion+1 WHERE FreshWaterId={ser_FreshWater.FreshWaterId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1) FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_FreshWater.JobOrderId} AND (ItemNo + 1) NOT IN (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_FreshWater.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1) FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_FreshWater.JobOrderId} AND TaskId = {ser_FreshWater.TaskId} AND (TaskItemNo + 1) NOT IN (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_FreshWater.JobOrderId} AND TaskId = {ser_FreshWater.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId) VALUES ({CompanyId},{ser_FreshWater.JobOrderId},'{ser_FreshWater.JobOrderNo}',@ItemNo,{ser_FreshWater.TaskId},@TaskItemNo,{ser_FreshWater.FreshWaterId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_FreshWater.FreshWaterId,
                            DocumentNo = "",
                            TblName = "Ser_FreshWater",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "FreshWater Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_FreshWater",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_FreshWater.FreshWaterId,
                    DocumentNo = "",
                    TblName = "Ser_FreshWater",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteFreshWaterAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 freshWaterId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (freshWaterId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={freshWaterId}");

                        var deleteResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_FreshWater WHERE CompanyId={CompanyId} AND FreshWaterId={freshWaterId}");

                        if (deletejobOrderResult > 0 && deleteResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = freshWaterId,
                                DocumentNo = "",
                                TblName = "Ser_FreshWater",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "FreshWater Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "FreshWaterId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = freshWaterId,
                    DocumentNo = "",
                    TblName = "Ser_FreshWater",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = freshWaterId,
                    DocumentNo = "",
                    TblName = "Ser_FreshWater",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion FreshWater

        #region TechnicianSurveyor

        public async Task<TechnicianSurveyorViewModelCount> GetTechnicianSurveyorListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            TechnicianSurveyorViewModelCount countViewModel = new TechnicianSurveyorViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_TechnicianSurveyor Ser_TS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_TS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_TS.ChargeId AND M_Chr.TaskId = Ser_TS.TaskId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_TS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_TS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_TS.EditById WHERE Ser_TS.JobOrderId={JobOrderId} AND Ser_TS.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<TechnicianSurveyorViewModel>($"SELECT Ser_TS.TechnicianSurveyorId,Ser_TS.CompanyId,Ser_TS.JobOrderId,Ser_TS.JobOrderNo,Ser_TS.TaskId,Ser_TS.SupplierId,M_Ser.SupplierName,Ser_TS.ChargeId,M_Chr.ChargeName,Ser_TS.StatusId,M_Or.OrderTypeName As StatusName,Ser_TS.ServiceDate,Ser_TS.GLId,Ser_TS.DebitNoteId,Ser_TS.DebitNoteNo,Ser_TS.Remarks,Ser_TS.CreateById,Ser_TS.CreateDate,Ser_TS.EditById,Ser_TS.EditDate,Ser_TS.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_TechnicianSurveyor Ser_TS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_TS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_TS.ChargeId AND M_Chr.TaskId = Ser_TS.TaskId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_TS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_TS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_TS.EditById WHERE Ser_TS.JobOrderId={JobOrderId} AND Ser_TS.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TechnicianSurveyorViewModel>();

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
                    TblName = "Ser_TechnicianSurveyor",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<TechnicianSurveyorViewModel> GetTechnicianSurveyorByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 TechnicianSurveyorId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<TechnicianSurveyorViewModel>($"SELECT Ser_TS.TechnicianSurveyorId,Ser_TS.CompanyId,Ser_TS.JobOrderId,Ser_TS.JobOrderNo,Ser_TS.TaskId,Ser_TS.SupplierId,M_Ser.SupplierName,Ser_TS.ChargeId,M_Chr.ChargeName,Ser_TS.StatusId,M_Or.OrderTypeName As StatusName,Ser_TS.ServiceDate,Ser_TS.GLId,Ser_TS.DebitNoteId,Ser_TS.DebitNoteNo,Ser_TS.Remarks,Ser_TS.CreateById,Ser_TS.CreateDate,Ser_TS.EditById,Ser_TS.EditDate,Ser_TS.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_TechnicianSurveyor Ser_TS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_TS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_TS.ChargeId AND M_Chr.TaskId = Ser_TS.TaskId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_TS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_TS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_TS.EditById WHERE Ser_TS.JobOrderId={JobOrderId} AND Ser_TS.TechnicianSurveyorId={TechnicianSurveyorId} AND Ser_TS.CompanyId={CompanyId}");

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
                    TblName = "Ser_TechnicianSurveyor",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveTechnicianSurveyorAsync(short CompanyId, short UserId, Ser_TechnicianSurveyor ser_TechnicianSurveyor)
        {
            bool IsEdit = ser_TechnicianSurveyor.TechnicianSurveyorId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_TechnicianSurveyor WHERE TechnicianSurveyorId=@TechnicianSurveyorId",
                     new { ser_TechnicianSurveyor.TechnicianSurveyorId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_TechnicianSurveyor);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "TechnicianSurveyor Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (TechnicianSurveyorId + 1) FROM dbo.Ser_TechnicianSurveyor WHERE (TechnicianSurveyorId + 1) NOT IN (SELECT TechnicianSurveyorId FROM dbo.Ser_TechnicianSurveyor)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_TechnicianSurveyor.TechnicianSurveyorId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_TechnicianSurveyor.EditById = null;
                            ser_TechnicianSurveyor.EditDate = null;
                            _context.Add(ser_TechnicianSurveyor);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_TechnicianSurveyor set EditVersion=EditVersion+1 WHERE TechnicianSurveyorId={ser_TechnicianSurveyor.TechnicianSurveyorId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1) FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_TechnicianSurveyor.JobOrderId} AND (ItemNo + 1) NOT IN (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_TechnicianSurveyor.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1) FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_TechnicianSurveyor.JobOrderId} AND TaskId = {ser_TechnicianSurveyor.TaskId} AND (TaskItemNo + 1) NOT IN (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_TechnicianSurveyor.JobOrderId} AND TaskId = {ser_TechnicianSurveyor.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId) VALUES ({CompanyId},{ser_TechnicianSurveyor.JobOrderId},'{ser_TechnicianSurveyor.JobOrderNo}',@ItemNo,{ser_TechnicianSurveyor.TaskId},@TaskItemNo,{ser_TechnicianSurveyor.TechnicianSurveyorId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_TechnicianSurveyor.TechnicianSurveyorId,
                            DocumentNo = "",
                            TblName = "Ser_TechnicianSurveyor",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "TechnicianSurveyor Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_TechnicianSurveyor",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_TechnicianSurveyor.TechnicianSurveyorId,
                    DocumentNo = "",
                    TblName = "Ser_TechnicianSurveyor",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteTechnicianSurveyorAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 TechnicianSurveyorId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (TechnicianSurveyorId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={TechnicianSurveyorId}");

                        var deleteResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_TechnicianSurveyor WHERE CompanyId={CompanyId} AND TechnicianSurveyorId={TechnicianSurveyorId}");

                        if (deletejobOrderResult > 0 && deleteResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = TechnicianSurveyorId,
                                DocumentNo = "",
                                TblName = "Ser_TechnicianSurveyor",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "TechnicianSurveyor Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "TechnicianSurveyorId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = TechnicianSurveyorId,
                    DocumentNo = "",
                    TblName = "Ser_TechnicianSurveyor",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = TechnicianSurveyorId,
                    DocumentNo = "",
                    TblName = "Ser_TechnicianSurveyor",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion TechnicianSurveyor

        #region LandingItems

        public async Task<LandingItemsViewModelCount> GetLandingItemsListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            LandingItemsViewModelCount countViewModel = new LandingItemsViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_LandingItems Ser_LI INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_LI.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_LI.ChargeId AND M_Chr.TaskId = Ser_LI.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_LI.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_LI.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_LI.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_LI.EditById WHERE Ser_LI.JobOrderId={JobOrderId} AND Ser_LI.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<LandingItemsViewModel>($"SELECT Ser_LI.LandingItemId,Ser_LI.CompanyId,Ser_LI.JobOrderId,Ser_LI.JobOrderNo,Ser_LI.TaskId,Ser_LI.Quantity,Ser_LI.SupplierId,M_Ser.SupplierName,Ser_LI.ChargeId,M_Chr.ChargeName,Ser_LI.StatusId,M_Or.OrderTypeName As StatusName,Ser_LI.UomId,M_Uo.UomName,Ser_LI.LandingDate,Ser_LI.GLId,Ser_LI.DebitNoteId,Ser_LI.DebitNoteNo,Ser_LI.Remarks,Ser_LI.CreateById,Ser_LI.CreateDate,Ser_LI.EditById,Ser_LI.EditDate,Ser_LI.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_LandingItems Ser_LI INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_LI.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_LI.ChargeId AND M_Chr.TaskId = Ser_LI.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_LI.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_LI.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_LI.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_LI.EditById WHERE Ser_LI.JobOrderId={JobOrderId} AND Ser_LI.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<LandingItemsViewModel>();

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
                    TblName = "Ser_LandingItems",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<LandingItemsViewModel> GetLandingItemsByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 LandingItemId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<LandingItemsViewModel>($"SELECT Ser_LI.LandingItemId,Ser_LI.CompanyId,Ser_LI.JobOrderId,Ser_LI.JobOrderNo,Ser_LI.TaskId,Ser_LI.Quantity,Ser_LI.SupplierId,M_Ser.SupplierName,Ser_LI.ChargeId,M_Chr.ChargeName,Ser_LI.StatusId,M_Or.OrderTypeName As StatusName,Ser_LI.UomId,M_Uo.UomName,Ser_LI.LandingDate,Ser_LI.GLId,Ser_LI.DebitNoteId,Ser_LI.DebitNoteNo,Ser_LI.Remarks,Ser_LI.CreateById,Ser_LI.CreateDate,Ser_LI.EditById,Ser_LI.EditDate,Ser_LI.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_LandingItems Ser_LI INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_LI.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_LI.ChargeId AND M_Chr.TaskId = Ser_LI.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_LI.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_LI.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_LI.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_LI.EditById WHERE Ser_LI.JobOrderId={JobOrderId} AND Ser_LI.LandingItemId={LandingItemId} AND Ser_LI.CompanyId={CompanyId}");

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
                    TblName = "Ser_LandingItems",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveLandingItemsAsync(short CompanyId, short UserId, Ser_LandingItems ser_LandingItems)
        {
            bool IsEdit = ser_LandingItems.LandingItemId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_LandingItems WHERE LandingItemId=@LandingItemId",
                     new { ser_LandingItems.LandingItemId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_LandingItems);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "LandingItems Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (LandingItemId + 1) FROM dbo.Ser_LandingItems WHERE (LandingItemId + 1) NOT IN (SELECT LandingItemId FROM dbo.Ser_LandingItems)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_LandingItems.LandingItemId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_LandingItems.EditById = null;
                            ser_LandingItems.EditDate = null;
                            _context.Add(ser_LandingItems);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_LandingItems set EditVersion=EditVersion+1 WHERE LandingItemId={ser_LandingItems.LandingItemId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1) FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LandingItems.JobOrderId} AND (ItemNo + 1) NOT IN (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LandingItems.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1) FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LandingItems.JobOrderId} AND TaskId = {ser_LandingItems.TaskId} AND (TaskItemNo + 1) NOT IN (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LandingItems.JobOrderId} AND TaskId = {ser_LandingItems.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId) VALUES ({CompanyId},{ser_LandingItems.JobOrderId},'{ser_LandingItems.JobOrderNo}',@ItemNo,{ser_LandingItems.TaskId},@TaskItemNo,{ser_LandingItems.LandingItemId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_LandingItems.LandingItemId,
                            DocumentNo = "",
                            TblName = "Ser_LandingItems",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "LandingItems Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_LandingItems",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_LandingItems.LandingItemId,
                    DocumentNo = "",
                    TblName = "Ser_LandingItems",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteLandingItemsAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 landingItemsId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (landingItemsId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={landingItemsId}");

                        var deleteResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_LandingItems WHERE CompanyId={CompanyId} AND LandingItemId={landingItemsId}");

                        if (deletejobOrderResult > 0 && deleteResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = landingItemsId,
                                DocumentNo = "",
                                TblName = "Ser_LandingItems",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "LandingItems Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "LandingItemId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = landingItemsId,
                    DocumentNo = "",
                    TblName = "Ser_LandingItems",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = landingItemsId,
                    DocumentNo = "",
                    TblName = "Ser_LandingItems",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion LandingItems

        #region OtherService

        public async Task<OtherServiceViewModelCount> GetOtherServiceListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            OtherServiceViewModelCount countViewModel = new OtherServiceViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_OtherService Ser_OS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_OS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_OS.ChargeId AND M_Chr.TaskId = Ser_OS.TaskId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_OS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_OS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_OS.EditById WHERE Ser_OS.JobOrderId={JobOrderId} AND Ser_OS.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<OtherServiceViewModel>($"SELECT Ser_OS.OtherServiceId,Ser_OS.CompanyId,Ser_OS.JobOrderId,Ser_OS.JobOrderNo,Ser_OS.TaskId,Ser_OS.SupplierId,M_Ser.SupplierName,Ser_OS.ChargeId,M_Chr.ChargeName,Ser_OS.StatusId,M_Or.OrderTypeName As StatusName,Ser_OS.ServiceDate,Ser_OS.GLId,Ser_OS.DebitNoteId,Ser_OS.DebitNoteNo,Ser_OS.Remarks,Ser_OS.CreateById,Ser_OS.CreateDate,Ser_OS.EditById,Ser_OS.EditDate,Ser_OS.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_OtherService Ser_OS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_OS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_OS.ChargeId AND M_Chr.TaskId = Ser_OS.TaskId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_OS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_OS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_OS.EditById WHERE Ser_OS.JobOrderId={JobOrderId} AND Ser_OS.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<OtherServiceViewModel>();

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
                    TblName = "Ser_OtherService",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<OtherServiceViewModel> GetOtherServiceByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 OtherServiceId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<OtherServiceViewModel>($"SELECT Ser_OS.OtherServiceId,Ser_OS.CompanyId,Ser_OS.JobOrderId,Ser_OS.JobOrderNo,Ser_OS.TaskId,Ser_OS.SupplierId,M_Ser.SupplierName,Ser_OS.ChargeId,M_Chr.ChargeName,Ser_OS.StatusId,M_Or.OrderTypeName As StatusName,Ser_OS.ServiceDate,Ser_OS.GLId,Ser_OS.DebitNoteId,Ser_OS.DebitNoteNo,Ser_OS.Remarks,Ser_OS.CreateById,Ser_OS.CreateDate,Ser_OS.EditById,Ser_OS.EditDate,Ser_OS.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_OtherService Ser_OS INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_OS.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_OS.ChargeId AND M_Chr.TaskId = Ser_OS.TaskId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_OS.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_OS.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_OS.EditById WHERE Ser_OS.JobOrderId={JobOrderId} AND Ser_OS.OtherServiceId={OtherServiceId} AND Ser_OS.CompanyId={CompanyId}");

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
                    TblName = "Ser_OtherService",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveOtherServiceAsync(short CompanyId, short UserId, Ser_OtherService ser_OtherService)
        {
            bool IsEdit = ser_OtherService.OtherServiceId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_OtherService WHERE OtherServiceId=@OtherServiceId",
                     new { ser_OtherService.OtherServiceId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_OtherService);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "OtherService Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (OtherServiceId + 1) FROM dbo.Ser_OtherService WHERE (OtherServiceId + 1) NOT IN (SELECT OtherServiceId FROM dbo.Ser_OtherService)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_OtherService.OtherServiceId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_OtherService.EditById = null;
                            ser_OtherService.EditDate = null;
                            _context.Add(ser_OtherService);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_OtherService set EditVersion=EditVersion+1 WHERE OtherServiceId={ser_OtherService.OtherServiceId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1) FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_OtherService.JobOrderId} AND (ItemNo + 1) NOT IN (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_OtherService.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1) FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_OtherService.JobOrderId} AND TaskId = {ser_OtherService.TaskId} AND (TaskItemNo + 1) NOT IN (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_OtherService.JobOrderId} AND TaskId = {ser_OtherService.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId) VALUES ({CompanyId},{ser_OtherService.JobOrderId},'{ser_OtherService.JobOrderNo}',@ItemNo,{ser_OtherService.TaskId},@TaskItemNo,{ser_OtherService.OtherServiceId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_OtherService.OtherServiceId,
                            DocumentNo = "",
                            TblName = "Ser_OtherService",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "OtherService Save Successfully",
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

                    return new SqlResponce();
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
                    TblName = "Ser_OtherService",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_OtherService.OtherServiceId,
                    DocumentNo = "",
                    TblName = "Ser_OtherService",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteOtherServiceAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 otherServiceId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (otherServiceId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={otherServiceId}");

                        var deleteResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_OtherService WHERE CompanyId={CompanyId} AND OtherServiceId={otherServiceId}");

                        if (deletejobOrderResult > 0 && deleteResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = otherServiceId,
                                DocumentNo = "",
                                TblName = "Ser_OtherService",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "OtherService Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "OtherServiceId Should be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = otherServiceId,
                    DocumentNo = "",
                    TblName = "Ser_OtherService",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = otherServiceId,
                    DocumentNo = "",
                    TblName = "Ser_OtherService",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion OtherService

        #region Agency Remuneration

        public async Task<AgencyRemunerationViewModelCount> GetAgencyRemunerationListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            AgencyRemunerationViewModelCount countViewModel = new AgencyRemunerationViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_AgencyRemuneration Ser_Agency INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Agency.ChargeId AND M_Chr.TaskId = Ser_Agency.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Agency.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Agency.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Agency.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Agency.EditById WHERE Ser_Agency.JobOrderId={JobOrderId} AND Ser_Agency.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<AgencyRemunerationViewModel>($"SELECT Ser_Agency.AgencyRemunerationId,Ser_Agency.CompanyId,Ser_Agency.JobOrderId,Ser_Agency.JobOrderNo,Ser_Agency.TaskId,Ser_Agency.Quantity,Ser_Agency.ChargeId,M_Chr.ChargeName,Ser_Agency.StatusId,M_Or.OrderTypeName As StatusName,Ser_Agency.UomId,M_Uo.UomName,Ser_Agency.DeliverDate,Ser_Agency.GLId,Ser_Agency.DebitNoteId,Ser_Agency.DebitNoteNo,Ser_Agency.Remarks,Ser_Agency.CreateById,Ser_Agency.CreateDate,Ser_Agency.EditById,Ser_Agency.EditDate,Ser_Agency.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_AgencyRemuneration Ser_Agency INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Agency.ChargeId AND M_Chr.TaskId = Ser_Agency.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Agency.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Agency.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Agency.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Agency.EditById WHERE Ser_Agency.JobOrderId={JobOrderId} AND Ser_Agency.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<AgencyRemunerationViewModel>();

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
                    TblName = "Ser_AgencyRemuneration",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<AgencyRemunerationViewModel> GetAgencyRemunerationByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 AgencyRemunerationId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<AgencyRemunerationViewModel>($"SELECT Ser_Agency.AgencyRemunerationId,Ser_Agency.CompanyId,Ser_Agency.JobOrderId,Ser_Agency.JobOrderNo,Ser_Agency.TaskId,Ser_Agency.Quantity,Ser_Agency.ChargeId,M_Chr.ChargeName,Ser_Agency.StatusId,M_Or.OrderTypeName As StatusName,Ser_Agency.UomId,M_Uo.UomName,Ser_Agency.DeliverDate,Ser_Agency.GLId,Ser_Agency.DebitNoteId,Ser_Agency.DebitNoteNo,Ser_Agency.Remarks,Ser_Agency.CreateById,Ser_Agency.CreateDate,Ser_Agency.EditById,Ser_Agency.EditDate,Ser_Agency.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_AgencyRemuneration Ser_Agency INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Agency.ChargeId AND M_Chr.TaskId = Ser_Agency.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Agency.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Agency.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Agency.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Agency.EditById WHERE Ser_Agency.JobOrderId={JobOrderId} AND Ser_Agency.AgencyRemunerationId={AgencyRemunerationId} AND Ser_Agency.CompanyId={CompanyId} ");

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
                    TblName = "Ser_AgencyRemuneration",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveAgencyRemunerationAsync(short CompanyId, short UserId, Ser_AgencyRemuneration ser_AgencyRemuneration)
        {
            // Determine if this is an update vs. a new insert
            bool IsEdit = ser_AgencyRemuneration.AgencyRemunerationId != 0;

            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        // Check if the record exists
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT 1 AS IsExist FROM dbo.Ser_AgencyRemuneration WHERE AgencyRemunerationId=@AgencyRemunerationId",
                            new { ser_AgencyRemuneration.AgencyRemunerationId }
                        );

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            // Update fields (exclude creation info)
                            var entityEntry = _context.Update(ser_AgencyRemuneration);
                            entityEntry.Property(b => b.CreateById).IsModified = false;
                            entityEntry.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Agency Remuneration Not Found" };
                        }
                    }
                    else
                    {
                        // Get the next available AgencyRemunerationId from the table
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "SELECT ISNULL((SELECT TOP 1 (AgencyRemunerationId + 1) FROM dbo.Ser_AgencyRemuneration WHERE (AgencyRemunerationId + 1) NOT IN (SELECT AgencyRemunerationId FROM dbo.Ser_AgencyRemuneration)), 1) AS NextId"
                        );

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_AgencyRemuneration.AgencyRemunerationId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_AgencyRemuneration.EditById = null;
                            ser_AgencyRemuneration.EditDate = null;
                            _context.Add(ser_AgencyRemuneration);
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    // Persist changes for the Agency Remuneration record
                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        // Create Audit Log for tracking the operation
                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_AgencyRemuneration.AgencyRemunerationId,
                            DocumentNo = "",
                            TblName = "Ser_AgencyRemuneration",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "Agency Remuneration Saved Successfully",
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
                        return new SqlResponce { Result = -1, Message = "Save Failed" };
                    }

                    return new SqlResponce();
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
                    DocumentId = ser_AgencyRemuneration.AgencyRemunerationId,
                    DocumentNo = "",
                    TblName = "Ser_AgencyRemuneration",
                    ModeId = (short)E_Mode.Create, // or Update based on context
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = ser_AgencyRemuneration.AgencyRemunerationId,
                    DocumentNo = "",
                    TblName = "Ser_AgencyRemuneration",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteAgencyRemunerationAsync(short CompanyId, short UserId, long jobOrderId, long agencyRemunerationId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (agencyRemunerationId > 0)
                    {
                        // Delete from the Agency Remuneration table
                        var deleteResult = await _repository.GetRowExecuteAsync(
                            $"DELETE FROM dbo.Ser_AgencyRemuneration WHERE CompanyId = {CompanyId} AND AgencyRemunerationId = {agencyRemunerationId}"
                        );

                        if (deleteResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = agencyRemunerationId,
                                DocumentNo = "",
                                TblName = "Ser_AgencyRemuneration",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "Agency Remuneration Deleted Successfully",
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
                        return new SqlResponce { Result = -1, Message = "Agency Remuneration Id must be greater than zero" };
                    }
                    return new SqlResponce();
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
                    DocumentId = agencyRemunerationId,
                    DocumentNo = "",
                    TblName = "Ser_AgencyRemuneration",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = sqlEx.Number.ToString() + " " + sqlEx.Message + sqlEx.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                string errorMessage = SqlErrorHelper.GetErrorMessage(sqlEx.Number);

                return new SqlResponce
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
                    DocumentId = agencyRemunerationId,
                    DocumentNo = "",
                    TblName = "Ser_AgencyRemuneration",
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

#endregion Agency Remuneration