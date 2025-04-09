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
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId  FROM dbo.Ser_PortExpenses Ser_Port INNER JOIN dbo.M_Supplier M_Ser ON M_Ser.SupplierId = Ser_Port.SupplierId INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Port.ChargeId AND M_Chr.TaskId = Ser_Port.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Port.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Port.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Port.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Port.EditById WHERE Ser_Port.JobOrderId={JobOrderId} AND Ser_Port.CompanyId={CompanyId}");

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

        #region Launch Services

        public async Task<LaunchServicesViewModelCount> GetLaunchServicesListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            LaunchServicesViewModelCount countViewModel = new LaunchServicesViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>($"SELECT COUNT(*) AS CountId  FROM dbo.Ser_LaunchServices Ser_Launch INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Launch.ChargeId AND M_Chr.TaskId = Ser_Launch.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Launch.UomId LEFT JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = Ser_Launch.BargeId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Launch.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Launch.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Launch.EditById WHERE Ser_Launch.JobOrderId={JobOrderId} AND Ser_Launch.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<LaunchServicesViewModel>($"SELECT Ser_Launch.LaunchServiceId,Ser_Launch.LaunchServiceDate,Ser_Launch.CompanyId,Ser_Launch.JobOrderId,Ser_Launch.JobOrderNo,Ser_Launch.TaskId,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Launch.LoadingTime,Ser_Launch.LeftJetty,Ser_Launch.AlongsideVessel,Ser_Launch.DepartedFromVessel,Ser_Launch.ArrivedAtJetty,Ser_Launch.DistanceFromJetty,Ser_Launch.UomId,M_Uo.UomName,Ser_Launch.AmeTally,Ser_Launch.BoatopTally,Ser_Launch.DistanceFromJettyToVessel,Ser_Launch.WeightOfCargoDelivered,Ser_Launch.WeightOfCargoLanded,Ser_Launch.BoatOperator,Ser_Launch.InvoiceNo,Ser_Launch.Annexure,Ser_Launch.TimeDiff,Ser_Launch.LaunchWaitingTime,Ser_Launch.BargeId,Ser_Launch.BargeName,Ser_Launch.StatusId,M_Or.OrderTypeName As StatusName,Ser_Launch.GLId,Ser_Launch.DebitNoteId,Ser_Launch.DebitNoteNo,Ser_Launch.Remarks,Ser_Launch.CreateById,Ser_Launch.CreateDate,Ser_Launch.EditById,Ser_Launch.EditDate,Ser_Launch.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_LaunchServices Ser_Launch INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Launch.ChargeId AND M_Chr.TaskId = Ser_Launch.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Launch.UomId LEFT JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = Ser_Launch.BargeId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Launch.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Launch.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Launch.EditById WHERE Ser_Launch.JobOrderId={JobOrderId} AND Ser_Launch.CompanyId={CompanyId} ");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<LaunchServicesViewModel>();

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
                    TblName = "Ser_LaunchServices",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<LaunchServicesViewModel> GetLaunchServicesByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 LaunchServiceId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<LaunchServicesViewModel>($"SELECT Ser_Launch.LaunchServiceId,Ser_Launch.LaunchServiceDate,Ser_Launch.CompanyId,Ser_Launch.JobOrderId,Ser_Launch.JobOrderNo,Ser_Launch.TaskId,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Launch.LoadingTime,Ser_Launch.LeftJetty,Ser_Launch.AlongsideVessel,Ser_Launch.DepartedFromVessel,Ser_Launch.ArrivedAtJetty,Ser_Launch.DistanceFromJetty,Ser_Launch.UomId,M_Uo.UomName,Ser_Launch.AmeTally,Ser_Launch.BoatopTally,Ser_Launch.DistanceFromJettyToVessel,Ser_Launch.WeightOfCargoDelivered,Ser_Launch.WeightOfCargoLanded,Ser_Launch.BoatOperator,Ser_Launch.InvoiceNo,Ser_Launch.Annexure,Ser_Launch.TimeDiff,Ser_Launch.LaunchWaitingTime,Ser_Launch.BargeId,Ser_Launch.BargeName,Ser_Launch.StatusId,M_Or.OrderTypeName As StatusName,Ser_Launch.GLId,Ser_Launch.DebitNoteId,Ser_Launch.DebitNoteNo,Ser_Launch.Remarks,Ser_Launch.CreateById,Ser_Launch.CreateDate,Ser_Launch.EditById,Ser_Launch.EditDate,Ser_Launch.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_LaunchServices Ser_Launch INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Launch.ChargeId AND M_Chr.TaskId = Ser_Launch.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Launch.UomId LEFT JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = Ser_Launch.BargeId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Launch.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Launch.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Launch.EditById WHERE Ser_Launch.JobOrderId={JobOrderId} AND Ser_Launch.CompanyId={CompanyId} AND AND Ser_Launch.LaunchServiceId={LaunchServiceId} ");

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
                    TblName = "Ser_LaunchServices",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponse> SaveLaunchServicesAsync(short CompanyId, short UserId, Ser_LaunchServices ser_LaunchServices)
        {
            bool IsEdit = ser_LaunchServices.LaunchServiceId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_LaunchServices WHERE LaunchServiceId=@LaunchServiceId",
                     new { ser_LaunchServices.LaunchServiceId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_LaunchServices);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponse { Result = -1, Message = "LaunchServices Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                     "SELECT ISNULL((SELECT TOP 1 (LaunchServiceId + 1) FROM dbo.Ser_LaunchServices WHERE (LaunchServiceId + 1) NOT IN (SELECT LaunchServiceId FROM dbo.Ser_LaunchServices)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_LaunchServices.LaunchServiceId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_LaunchServices.EditById = null;
                            ser_LaunchServices.EditDate = null;
                            _context.Add(ser_LaunchServices);
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
                            await _repository.UpsertExecuteScalarAsync($"update Ser_LaunchServices  set EditVersion=EditVersion+1 WHERE   LaunchServiceId={ser_LaunchServices.LaunchServiceId} AND CompanyId={CompanyId} ");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LaunchServices.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LaunchServices.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LaunchServices.JobOrderId} AND TaskId = {ser_LaunchServices.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_LaunchServices.JobOrderId} AND TaskId = {ser_LaunchServices.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_LaunchServices.JobOrderId},'{ser_LaunchServices.JobOrderNo}',@ItemNo,{ser_LaunchServices.TaskId},@TaskItemNo,{ser_LaunchServices.LaunchServiceId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_LaunchServices.LaunchServiceId,
                            DocumentNo = "",
                            TblName = "Ser_LaunchServices",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "LaunchServices Save Successfully",
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
                    TblName = "Ser_LaunchServices",
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
                    DocumentId = ser_LaunchServices.LaunchServiceId,
                    DocumentNo = "",
                    TblName = "Ser_LaunchServices",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponse> DeleteLaunchServicesAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 portExpenseId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (portExpenseId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND  ServiceId={portExpenseId} ");

                        var deleteportExpensesResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_LaunchServices WHERE CompanyId={CompanyId} AND LaunchServiceId={portExpenseId}");

                        if (deletejobOrderResult > 0 && deleteportExpensesResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = portExpenseId,
                                DocumentNo = "",
                                TblName = "Ser_LaunchServices",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "LaunchServices Delete Successfully",
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
                        return new SqlResponse { Result = -1, Message = "LaunchServiceId Should be zero" };
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
                    TblName = "Ser_LaunchServices",
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
                    TblName = "Ser_LaunchServices",
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
    }
}