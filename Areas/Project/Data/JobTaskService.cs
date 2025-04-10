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

        public async Task<LaunchServicesViewModelCount> GetLaunchServicesListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            LaunchServicesViewModelCount countViewModel = new LaunchServicesViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId  FROM dbo.Ser_LaunchServices Ser_Launch INNER JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Launch.ChargeId AND M_Chr.TaskId = Ser_Launch.TaskId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Launch.UomId LEFT JOIN dbo.M_Barge M_Bar ON M_Bar.BargeId = Ser_Launch.BargeId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Launch.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Launch.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Launch.EditById WHERE Ser_Launch.JobOrderId={JobOrderId} AND Ser_Launch.CompanyId={CompanyId}");

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

        public async Task<SqlResponce> SaveLaunchServicesAsync(short CompanyId, short UserId, Ser_LaunchServices ser_LaunchServices)
        {
            bool IsEdit = ser_LaunchServices.LaunchServiceId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
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
                            return new SqlResponce { Result = -1, Message = "LaunchServices Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
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
                            return new SqlResponce { Result = -1, Message = "Internal Server Error" };
                        }
                    }

                    var saveChangeRecord = _context.SaveChanges();

                    if (saveChangeRecord > 0)
                    {
                        if (IsEdit)
                        {
                            await _repository.UpsertExecuteScalarAsync($"update Ser_LaunchServices  set EditVersion=EditVersion+1 WHERE   LaunchServiceId={ser_LaunchServices.LaunchServiceId} AND CompanyId={CompanyId} ");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
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
                    TblName = "Ser_LaunchServices",
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

        public async Task<SqlResponce> DeleteLaunchServicesAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 portExpenseId)
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
                    TblName = "Ser_LaunchServices",
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

        #region Equipments Used

        public async Task<EquipmentsUsedViewModelCount> GetEquipmentsUsedListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            EquipmentsUsedViewModelCount countViewModel = new EquipmentsUsedViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_EquipmentsUsed Ser_Eq LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Eq.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Eq.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Eq.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Eq.ChargeId AND M_Chr.TaskId = Ser_Eq.TaskId LEFT JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Eq.UomId LEFT JOIN dbo.M_Equipment M_Eq ON M_Eq.EquipmentId = Ser_Eq.EquipmentId WHERE Ser_Eq.JobOrderId={JobOrderId} AND Ser_Eq.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<EquipmentsUsedViewModel>($"SELECT Ser_Eq.EquipmentsUsedId,Ser_Eq.CompanyId,Ser_Eq.JobOrderId,Ser_Eq.JobOrderNo,Ser_Eq.TaskId,Ser_Eq.EquipmentId,M_Eq.EquipmentName,Ser_Eq.DateUsed,Ser_Eq.Quantity,Ser_Eq.UomId,M_Uo.UomName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Eq.GLId,Ser_Eq.StatusId,M_Or.OrderTypeName As StatusName,Ser_Eq.DebitNoteId,Ser_Eq.DebitNoteNo,Ser_Eq.TotAmt,Ser_Eq.GstAmt,Ser_Eq.TotAmtAftGst,Ser_Eq.Remarks,Ser_Eq.CreateById,Ser_Eq.CreateDate,Ser_Eq.EditById,Ser_Eq.EditDate,Ser_Eq.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_EquipmentsUsed Ser_Eq LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Eq.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Eq.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Eq.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Eq.ChargeId AND M_Chr.TaskId = Ser_Eq.TaskId LEFT JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Eq.UomId LEFT JOIN dbo.M_Equipment M_Eq ON M_Eq.EquipmentId = Ser_Eq.EquipmentId WHERE Ser_Eq.JobOrderId={JobOrderId} AND Ser_Eq.CompanyId={CompanyId}");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<EquipmentsUsedViewModel>($"SELECT Ser_Eq.EquipmentsUsedId,Ser_Eq.CompanyId,Ser_Eq.JobOrderId,Ser_Eq.JobOrderNo,Ser_Eq.TaskId,Ser_Eq.EquipmentId,M_Eq.EquipmentName,Ser_Eq.DateUsed,Ser_Eq.Quantity,Ser_Eq.UomId,M_Uo.UomName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Eq.GLId,Ser_Eq.StatusId,M_Or.OrderTypeName As StatusName,Ser_Eq.DebitNoteId,Ser_Eq.DebitNoteNo,Ser_Eq.TotAmt,Ser_Eq.GstAmt,Ser_Eq.TotAmtAftGst,Ser_Eq.Remarks,Ser_Eq.CreateById,Ser_Eq.CreateDate,Ser_Eq.EditById,Ser_Eq.EditDate,Ser_Eq.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_EquipmentsUsed Ser_Eq LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Eq.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Eq.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Eq.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Eq.ChargeId AND M_Chr.TaskId = Ser_Eq.TaskId LEFT JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Eq.UomId LEFT JOIN dbo.M_Equipment M_Eq ON M_Eq.EquipmentId = Ser_Eq.EquipmentId WHERE Ser_Eq.JobOrderId={JobOrderId} AND Ser_Eq.EquipmentsUsedId={EquipmentsUsedId} AND Ser_Eq.CompanyId={CompanyId}");

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
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_CrewSignOn Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Sign.CrewId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<CrewSignOnViewModel>($"SELECT Ser_Sign.CrewSignOnId,Ser_Sign.CompanyId,Ser_Sign.JobOrderId,Ser_Sign.JobOrderNo,Ser_Sign.TaskId,Ser_Sign.SignOnDate,Ser_Sign.CrewId,M_Cr.CrewName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Sign.GLId,Ser_Sign.StatusId,M_Or.OrderTypeName As StatusName,Ser_Sign.DebitNoteId,Ser_Sign.DebitNoteNo,Ser_Sign.TotAmt,Ser_Sign.GstAmt,Ser_Sign.TotAmtAftGst,Ser_Sign.Remarks,Ser_Sign.CreateById,Ser_Sign.CreateDate,Ser_Sign.EditById,Ser_Sign.EditDate,Ser_Sign.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewSignOn Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Sign.CrewId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CompanyId={CompanyId}");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<CrewSignOnViewModel>($"SELECT Ser_Sign.CrewSignOnId,Ser_Sign.CompanyId,Ser_Sign.JobOrderId,Ser_Sign.JobOrderNo,Ser_Sign.TaskId,Ser_Sign.SignOnDate,Ser_Sign.CrewId,M_Cr.CrewName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Sign.GLId,Ser_Sign.StatusId,M_Or.OrderTypeName As StatusName,Ser_Sign.DebitNoteId,Ser_Sign.DebitNoteNo,Ser_Sign.TotAmt,Ser_Sign.GstAmt,Ser_Sign.TotAmtAftGst,Ser_Sign.Remarks,Ser_Sign.CreateById,Ser_Sign.CreateDate,Ser_Sign.EditById,Ser_Sign.EditDate,Ser_Sign.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewSignOn Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Sign.CrewId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CrewSignOnId={CrewSignOnId} AND Ser_Sign.CompanyId={CompanyId}");

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
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_CrewSignOff Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Sign.CrewId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<CrewSignOffViewModel>($"SELECT Ser_Sign.CrewSignOffId,Ser_Sign.CompanyId,Ser_Sign.JobOrderId,Ser_Sign.JobOrderNo,Ser_Sign.TaskId,Ser_Sign.SignOffDate,Ser_Sign.CrewId,M_Cr.CrewName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Sign.GLId,Ser_Sign.StatusId,M_Or.OrderTypeName As StatusName,Ser_Sign.DebitNoteId,Ser_Sign.DebitNoteNo,Ser_Sign.TotAmt,Ser_Sign.GstAmt,Ser_Sign.TotAmtAftGst,Ser_Sign.Remarks,Ser_Sign.CreateById,Ser_Sign.CreateDate,Ser_Sign.EditById,Ser_Sign.EditDate,Ser_Sign.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewSignOff Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Sign.CrewId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CompanyId={CompanyId}");

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
                var result = await _repository.GetQuerySingleOrDefaultAsync<CrewSignOffViewModel>($"SELECT Ser_Sign.CrewSignOffId,Ser_Sign.CompanyId,Ser_Sign.JobOrderId,Ser_Sign.JobOrderNo,Ser_Sign.TaskId,Ser_Sign.SignOffDate,Ser_Sign.CrewId,M_Cr.CrewName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Sign.GLId,Ser_Sign.StatusId,M_Or.OrderTypeName As StatusName,Ser_Sign.DebitNoteId,Ser_Sign.DebitNoteNo,Ser_Sign.TotAmt,Ser_Sign.GstAmt,Ser_Sign.TotAmtAftGst,Ser_Sign.Remarks,Ser_Sign.CreateById,Ser_Sign.CreateDate,Ser_Sign.EditById,Ser_Sign.EditDate,Ser_Sign.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewSignOff Ser_Sign LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Sign.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Sign.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Sign.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Sign.ChargeId AND M_Chr.TaskId = Ser_Sign.TaskId LEFT JOIN dbo.M_Crew M_Cr ON M_Cr.CrewId = Ser_Sign.CrewId WHERE Ser_Sign.JobOrderId={JobOrderId} AND Ser_Sign.CrewSignOffId={CrewSignOffId} AND Ser_Sign.CompanyId={CompanyId}");

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

                var result = await _repository.GetQueryAsync<CrewMiscellaneousViewModel>($"SELECT Ser_Misc.CrewMiscId,Ser_Misc.CompanyId,Ser_Misc.JobOrderId,Ser_Misc.JobOrderNo,Ser_Misc.TaskId,Ser_Misc.MiscDate,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Misc.GLId,Ser_Misc.StatusId,M_Or.OrderTypeName As StatusName,Ser_Misc.DebitNoteId,Ser_Misc.DebitNoteNo,Ser_Misc.TotAmt,Ser_Misc.GstAmt,Ser_Misc.TotAmtAftGst,Ser_Misc.Remarks,Ser_Misc.CreateById,Ser_Misc.CreateDate,Ser_Misc.EditById,Ser_Misc.EditDate,Ser_Misc.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewMiscellaneous Ser_Misc LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Misc.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Misc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Misc.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Misc.ChargeId AND M_Chr.TaskId = Ser_Misc.TaskId WHERE Ser_Misc.JobOrderId={JobOrderId} AND Ser_Misc.CompanyId={CompanyId}");

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

        public async Task<CrewMiscellaneousViewModel> GetCrewMiscellaneousByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 CrewMiscId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<CrewMiscellaneousViewModel>($"SELECT Ser_Misc.CrewMiscId,Ser_Misc.CompanyId,Ser_Misc.JobOrderId,Ser_Misc.JobOrderNo,Ser_Misc.TaskId,Ser_Misc.MiscDate,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Misc.GLId,Ser_Misc.StatusId,M_Or.OrderTypeName As StatusName,Ser_Misc.DebitNoteId,Ser_Misc.DebitNoteNo,Ser_Misc.TotAmt,Ser_Misc.GstAmt,Ser_Misc.TotAmtAftGst,Ser_Misc.Remarks,Ser_Misc.CreateById,Ser_Misc.CreateDate,Ser_Misc.EditById,Ser_Misc.EditDate,Ser_Misc.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_CrewMiscellaneous Ser_Misc LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Misc.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Misc.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Misc.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Misc.ChargeId AND M_Chr.TaskId = Ser_Misc.TaskId WHERE Ser_Misc.JobOrderId={JobOrderId} AND Ser_Misc.CrewMiscId={CrewMiscId} AND Ser_Misc.CompanyId={CompanyId}");

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
            bool IsEdit = ser_CrewMiscellaneous.CrewMiscId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_CrewMiscellaneous WHERE CrewMiscId=@CrewMiscId",
                     new { ser_CrewMiscellaneous.CrewMiscId });

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
                     "SELECT ISNULL((SELECT TOP 1 (CrewMiscId + 1) FROM dbo.Ser_CrewMiscellaneous WHERE (CrewMiscId + 1) NOT IN (SELECT CrewMiscId FROM dbo.Ser_CrewMiscellaneous)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_CrewMiscellaneous.CrewMiscId = Convert.ToInt16(sqlMissingResponse.NextId);
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
                            await _repository.UpsertExecuteScalarAsync($"update Ser_CrewMiscellaneous set EditVersion=EditVersion+1 WHERE CrewMiscId={ser_CrewMiscellaneous.CrewMiscId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewMiscellaneous.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewMiscellaneous.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewMiscellaneous.JobOrderId} AND TaskId = {ser_CrewMiscellaneous.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_CrewMiscellaneous.JobOrderId} AND TaskId = {ser_CrewMiscellaneous.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_CrewMiscellaneous.JobOrderId},'{ser_CrewMiscellaneous.JobOrderNo}',@ItemNo,{ser_CrewMiscellaneous.TaskId},@TaskItemNo,{ser_CrewMiscellaneous.CrewMiscId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_CrewMiscellaneous.CrewMiscId,
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
                    DocumentId = ser_CrewMiscellaneous.CrewMiscId,
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

                        var deleteCrewMiscellaneousResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_CrewMiscellaneous WHERE CompanyId={CompanyId} AND CrewMiscId={crewMiscId}");

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
                        return new SqlResponce { Result = -1, Message = "CrewMiscId Should be greater than zero" };
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

        #region Third Party Supply

        public async Task<ThirdPartySupplyViewModelCount> GetThirdPartySupplyListAsync(short CompanyId, short UserId, Int64 JobOrderId)
        {
            ThirdPartySupplyViewModelCount countViewModel = new ThirdPartySupplyViewModelCount();
            try
            {
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>($"SELECT COUNT(*) AS CountId FROM dbo.Ser_ThirdPartySupply Ser_Tps LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tps.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Tps.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Tps.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Tps.ChargeId AND M_Chr.TaskId = Ser_Tps.TaskId LEFT JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Ser_Tps.SupplierId WHERE Ser_Tps.JobOrderId={JobOrderId} AND Ser_Tps.CompanyId={CompanyId}");

                var result = await _repository.GetQueryAsync<ThirdPartySupplyViewModel>($"SELECT Ser_Tps.ThirdPartySupplyId,Ser_Tps.CompanyId,Ser_Tps.JobOrderId,Ser_Tps.JobOrderNo,Ser_Tps.TaskId,Ser_Tps.SupplyDate,Ser_Tps.SupplierId,M_Sup.SupplierName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Tps.GLId,Ser_Tps.StatusId,M_Or.OrderTypeName As StatusName,Ser_Tps.DebitNoteId,Ser_Tps.DebitNoteNo,Ser_Tps.TotAmt,Ser_Tps.GstAmt,Ser_Tps.TotAmtAftGst,Ser_Tps.Remarks,Ser_Tps.CreateById,Ser_Tps.CreateDate,Ser_Tps.EditById,Ser_Tps.EditDate,Ser_Tps.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_ThirdPartySupply Ser_Tps LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tps.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Tps.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Tps.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Tps.ChargeId AND M_Chr.TaskId = Ser_Tps.TaskId LEFT JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Ser_Tps.SupplierId WHERE Ser_Tps.JobOrderId={JobOrderId} AND Ser_Tps.CompanyId={CompanyId}");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<ThirdPartySupplyViewModel>();

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
                    TblName = "Ser_ThirdPartySupply",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<ThirdPartySupplyViewModel> GetThirdPartySupplyByIdAsync(short CompanyId, short UserId, Int64 JobOrderId, Int64 ThirdPartySupplyId)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<ThirdPartySupplyViewModel>($"SELECT Ser_Tps.ThirdPartySupplyId,Ser_Tps.CompanyId,Ser_Tps.JobOrderId,Ser_Tps.JobOrderNo,Ser_Tps.TaskId,Ser_Tps.SupplyDate,Ser_Tps.SupplierId,M_Sup.SupplierName,M_Chr.ChargeId,M_Chr.ChargeName,Ser_Tps.GLId,Ser_Tps.StatusId,M_Or.OrderTypeName As StatusName,Ser_Tps.DebitNoteId,Ser_Tps.DebitNoteNo,Ser_Tps.TotAmt,Ser_Tps.GstAmt,Ser_Tps.TotAmtAftGst,Ser_Tps.Remarks,Ser_Tps.CreateById,Ser_Tps.CreateDate,Ser_Tps.EditById,Ser_Tps.EditDate,Ser_Tps.EditVersion,Usr.UserName AS CreateBy, Usr1.UserName AS EditBy FROM dbo.Ser_ThirdPartySupply Ser_Tps LEFT JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tps.StatusId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Ser_Tps.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Ser_Tps.EditById LEFT JOIN dbo.M_Charge M_Chr ON M_Chr.ChargeId = Ser_Tps.ChargeId AND M_Chr.TaskId = Ser_Tps.TaskId LEFT JOIN dbo.M_Supplier M_Sup ON M_Sup.SupplierId = Ser_Tps.SupplierId WHERE Ser_Tps.JobOrderId={JobOrderId} AND Ser_Tps.ThirdPartySupplyId={ThirdPartySupplyId} AND Ser_Tps.CompanyId={CompanyId}");

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
                    TblName = "Ser_ThirdPartySupply",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<SqlResponce> SaveThirdPartySupplyAsync(short CompanyId, short UserId, Ser_ThirdPartySupply ser_ThirdPartySupply)
        {
            bool IsEdit = ser_ThirdPartySupply.ThirdPartySupplyId != 0;
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (IsEdit)
                    {
                        var dataExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     $"SELECT 1 AS IsExist FROM dbo.Ser_ThirdPartySupply WHERE ThirdPartySupplyId=@ThirdPartySupplyId",
                     new { ser_ThirdPartySupply.ThirdPartySupplyId });

                        if ((dataExist?.IsExist ?? 0) > 0)
                        {
                            var entityHead = _context.Update(ser_ThirdPartySupply);
                            entityHead.Property(b => b.CreateById).IsModified = false;
                            entityHead.Property(b => b.CompanyId).IsModified = false;
                        }
                        else
                        {
                            return new SqlResponce { Result = -1, Message = "ThirdPartySupply Not Found" };
                        }
                    }
                    else
                    {
                        // Take the Next Id From SQL
                        var sqlMissingResponse = await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                     "SELECT ISNULL((SELECT TOP 1 (ThirdPartySupplyId + 1) FROM dbo.Ser_ThirdPartySupply WHERE (ThirdPartySupplyId + 1) NOT IN (SELECT ThirdPartySupplyId FROM dbo.Ser_ThirdPartySupply)),1) AS NextId");

                        if (sqlMissingResponse != null && sqlMissingResponse.NextId > 0)
                        {
                            ser_ThirdPartySupply.ThirdPartySupplyId = Convert.ToInt16(sqlMissingResponse.NextId);
                            ser_ThirdPartySupply.EditById = null;
                            ser_ThirdPartySupply.EditDate = null;
                            _context.Add(ser_ThirdPartySupply);
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
                            await _repository.UpsertExecuteScalarAsync($"update Ser_ThirdPartySupply set EditVersion=EditVersion+1 WHERE ThirdPartySupplyId={ser_ThirdPartySupply.ThirdPartySupplyId} AND CompanyId={CompanyId}");
                        }
                        else
                        {
                            //Insert into Ser_JobOrderDt Table
                            await _repository.GetQuerySingleOrDefaultAsync<SqlResponceIds>(
                            "DECLARE @ItemNo SMALLINT;   " +
                            "DECLARE @TaskItemNo SMALLINT; " +
                            $"SELECT @ItemNo = ISNULL((SELECT TOP 1 (ItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ThirdPartySupply.JobOrderId} AND (ItemNo + 1) NOT IN     (SELECT ItemNo FROM dbo.Ser_JobOrderDt WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ThirdPartySupply.JobOrderId})), 1);  " +
                            $"SELECT @TaskItemNo = ISNULL((SELECT TOP 1 (TaskItemNo + 1)  FROM dbo.Ser_JobOrderDt  WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ThirdPartySupply.JobOrderId} AND TaskId = {ser_ThirdPartySupply.TaskId} AND (TaskItemNo + 1) NOT IN     (SELECT TaskItemNo FROM dbo.Ser_JobOrderDt      WHERE CompanyId = {CompanyId} AND JobOrderId = {ser_ThirdPartySupply.JobOrderId} AND TaskId = {ser_ThirdPartySupply.TaskId})), 1);   " +
                            $"INSERT INTO dbo.Ser_JobOrderDt (CompanyId,JobOrderId,JobOrderNo,ItemNo,TaskId,TaskItemNo,ServiceId )   VALUES ({CompanyId},{ser_ThirdPartySupply.JobOrderId},'{ser_ThirdPartySupply.JobOrderNo}',@ItemNo,{ser_ThirdPartySupply.TaskId},@TaskItemNo,{ser_ThirdPartySupply.ThirdPartySupplyId});");
                        }

                        var auditLog = new AdmAuditLog
                        {
                            CompanyId = CompanyId,
                            ModuleId = (short)E_Modules.Project,
                            TransactionId = (short)E_Project.Job,
                            DocumentId = ser_ThirdPartySupply.ThirdPartySupplyId,
                            DocumentNo = "",
                            TblName = "Ser_ThirdPartySupply",
                            ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                            Remarks = "ThirdPartySupply Save Successfully",
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
                    TblName = "Ser_ThirdPartySupply",
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
                    DocumentId = ser_ThirdPartySupply.ThirdPartySupplyId,
                    DocumentNo = "",
                    TblName = "Ser_ThirdPartySupply",
                    ModeId = IsEdit ? (short)E_Mode.Update : (short)E_Mode.Create,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };
                _context.Add(errorLog);
                _context.SaveChanges();

                throw;
            }
        }

        public async Task<SqlResponce> DeleteThirdPartySupplyAsync(short CompanyId, short UserId, Int64 jobOrderId, Int64 thirdPartySupplyId)
        {
            try
            {
                using (var TScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (thirdPartySupplyId > 0)
                    {
                        var deletejobOrderResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_JobOrderDt WHERE CompanyId={CompanyId} AND JobOrderId={jobOrderId} AND ServiceId={thirdPartySupplyId}");

                        var deleteThirdPartySupplyResult = await _repository.GetRowExecuteAsync($"DELETE FROM dbo.Ser_ThirdPartySupply WHERE CompanyId={CompanyId} AND ThirdPartySupplyId={thirdPartySupplyId}");

                        if (deletejobOrderResult > 0 && deleteThirdPartySupplyResult > 0)
                        {
                            var auditLog = new AdmAuditLog
                            {
                                CompanyId = CompanyId,
                                ModuleId = (short)E_Modules.Project,
                                TransactionId = (short)E_Project.Job,
                                DocumentId = thirdPartySupplyId,
                                DocumentNo = "",
                                TblName = "Ser_ThirdPartySupply",
                                ModeId = (short)E_Mode.Delete,
                                Remarks = "ThirdPartySupply Delete Successfully",
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
                        return new SqlResponce { Result = -1, Message = "ThirdPartySupplyId Should be greater than zero" };
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
                    DocumentId = thirdPartySupplyId,
                    DocumentNo = "",
                    TblName = "Ser_ThirdPartySupply",
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
                    DocumentId = thirdPartySupplyId,
                    DocumentNo = "",
                    TblName = "Ser_ThirdPartySupply",
                    ModeId = (short)E_Mode.Delete,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId,
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        #endregion Third Party Supply
    }
}