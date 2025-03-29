using AEMSWEB.Areas.Project.Data.IServices;
using AEMSWEB.Areas.Project.Models;
using AEMSWEB.Data;
using AEMSWEB.Entities.Admin;
using AEMSWEB.Entities.Project;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models;
using AEMSWEB.Repository;

namespace AEMSWEB.Areas.Project.Data.Services
{
    public sealed class TariffService : ITariffService
    {
        private readonly IRepository<Ser_Tariff> _repository;
        private ApplicationDbContext _context; private readonly ILogService _logService;

        public TariffService(IRepository<Ser_Tariff> repository, ApplicationDbContext context, ILogService logService)
        {
            _repository = repository;
            _context = context; _logService = logService;
        }

        public async Task<TariffViewModelCount> GetTariffFreshWaterListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {

                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_OrderType M_Or1 ON M_Or1.OrderTypeId = Ser_Tar.FromPlace INNER JOIN dbo.M_OrderType M_Or2 ON M_Or2.OrderTypeId = Ser_Tar.ToPlace INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.FreshWaterSupply} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.FromPlace,M_Or1.OrderTypeName AS FromOrderTypeName,Ser_Tar.ToPlace,M_Or2.OrderTypeName AS ToOrderTypeName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_OrderType M_Or1 ON M_Or1.OrderTypeId = Ser_Tar.FromPlace INNER JOIN dbo.M_OrderType M_Or2 ON M_Or2.OrderTypeId = Ser_Tar.ToPlace INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.FreshWaterSupply} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Build the result
                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TariffViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Tariff,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "Ser_Tariff",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<TariffViewModelCount> GetTariffLaunchServiceListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, DateTime? fromDate, DateTime? toDate, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {

                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar  INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tar.SlabUomId INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.LaunchServices} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.SlabUomId,M_Or.OrderTypeName AS SlabUomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar  INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tar.SlabUomId INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={{(short)E_Task.LaunchServices}} AND Ser_Tar.CompanyId={{CompanyId}} AND Ser_Tar.CustomerId={{customerId}} AND Ser_Tar.PortId={{portId ORDER BY Hd.TariffNo OFFSET {pageSize} * ({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                // Build the result
                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TariffViewModel>();

                return countViewModel;
            }
            catch (Exception ex)
            {
                var errorLog = new AdmErrorLog
                {
                    CompanyId = CompanyId,
                    ModuleId = (short)E_Modules.Project,
                    TransactionId = (short)E_Project.Tariff,
                    DocumentId = 0,
                    DocumentNo = "",
                    TblName = "Ser_Tariff",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<TariffViewModelCount> GetTariffListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, DateTime? fromDate, DateTime? toDate, string statusName)
        {
            int statusId = 0;
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                if (statusName.ToLower() != "all")
                {
                    var statusExist = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        "SELECT TOP 1 OrderTypeId AS IsExist FROM dbo.M_OrderType " +
                        "WHERE OrderTypeName LIKE '%' + @OrderTypeName + '%' AND OrderTypeCategoryId = 4",
                        new { OrderTypeName = statusName }); // Using LIKE to support partial matches

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
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Hd " +
                        $"LEFT JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                        $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                        $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                        $"OR Hd.TariffNo LIKE '%{searchString}%' " +
                        $"OR Hd.Remark1 LIKE '%{searchString}%') " +
                        $"AND Hd.TariffId <> 0 " +
                        $"AND Hd.CompanyId = {CompanyId} " +
                        $"AND Hd.TariffDate BETWEEN '{fromDate:yyyy-MM-dd}' AND '{toDate:yyyy-MM-dd}' " +
                        $"AND (Hd.CustomerId = {customerId} OR {customerId} = 0) " +
                        $"AND (Hd.StatusId = {statusId}  OR {statusId} = 0) ");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Hd.TariffId, Hd.CompanyId, Hd.TariffNo, Hd.TariffDate, Hd.CustomerId, Cur.CurrencyId, Cut.CustomerCode, Cut.CustomerName, Hd.IMONo, " +
                    $"Cur.CurrencyName, Cur.CurrencyCode, Hd.TotalAmt, Hd.TotalLocalAmt, Hd.Remark1, Hd.IsActive, Hd.IsClose, " +
                    $"Usr.UserName AS CreateBy, Usr1.UserName AS EditBy, " +
                    $"Ord.OrderTypeName AS Status " + // Added OrderTypeName as Status
                    $"FROM dbo.Ser_Tariff Hd " +
                    $"INNER JOIN dbo.M_Customer Cut ON Cut.CustomerId = Hd.CustomerId " +
                    $"INNER JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                    $"LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Hd.CreateById " +
                    $"LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Hd.EditById " +
                    $"LEFT JOIN dbo.M_OrderType Ord ON Ord.OrderTypeId = Hd.StatusId " + // Join for OrderType
                    $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                    $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                    $"OR Hd.TariffNo LIKE '%{searchString}%' " +
                    $"OR Hd.Remark1 LIKE '%{searchString}%') " +
                    $"AND Hd.TariffId <> 0 " +
                    $"AND Hd.CompanyId = {CompanyId} " +
                    $"AND Hd.TariffDate BETWEEN '{fromDate:yyyy-MM-dd}' AND '{toDate:yyyy-MM-dd}' " +
                    $"AND (Hd.CustomerId = {customerId} OR {customerId} = 0) " +
                    $"AND (Hd.StatusId = {statusId}  OR {statusId} = 0) " +
                    $"ORDER BY Hd.TariffNo " +
                    $"OFFSET {pageSize} * ({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                // Build the result
                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TariffViewModel>();

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
                    TblName = "Ser_Tariff",
                    ModeId = (short)E_Mode.View,
                    Remarks = ex.Message + ex.InnerException?.Message,
                    CreateById = UserId
                };

                _context.Add(errorLog);
                _context.SaveChanges();

                throw new Exception(ex.ToString());
            }
        }

        public async Task<TariffViewModelCount> GetTariffListAsyncV1(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, string customerId, DateTime? fromDate, DateTime? toDate, string status)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Build additional filter conditions based on provided parameters
                string additionalFilters = string.Empty;

                if (!string.IsNullOrEmpty(customerId))
                {
                    additionalFilters += $" AND (Hd.CustomerId = {customerId} OR {customerId} = 0 )";
                }
                if (fromDate.HasValue)
                {
                    additionalFilters += $" AND Hd.TariffDate >= '{fromDate.Value:yyyy-MM-dd}'";
                }
                if (toDate.HasValue)
                {
                    additionalFilters += $" AND Hd.TariffDate <= '{toDate.Value:yyyy-MM-dd}'";
                }
                // Only add a status filter if the status parameter is not empty and not "all"
                if (!string.IsNullOrEmpty(status) && !status.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    int statusId = 0;
                    switch (status.ToLower())
                    {
                        case "completed":
                            statusId = 400;
                            break;

                        case "pending":
                            statusId = 401;
                            break;

                        case "confirm":
                            statusId = 402;
                            break;

                        case "cancel":
                            statusId = 403;
                            break;

                        case "post":
                            statusId = 405;
                            break;

                        default:
                            if (!int.TryParse(status, out statusId))
                            {
                                statusId = 0;
                            }
                            break;
                    }
                    if (statusId > 0)
                    {
                        additionalFilters += $" AND (Hd.StatusId = {statusId} OR {statusId} = 0)";
                    }
                }

                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                    $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Hd " +
                    $"LEFT JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                    $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                    $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                    $"OR Hd.TariffNo LIKE '%{searchString}%' " +
                    $"OR Hd.Remark1 LIKE '%{searchString}%') " +
                    $"AND Hd.TariffId <> 0 AND Hd.CompanyId = {CompanyId} " + additionalFilters);

                // Query to fetch paginated data with additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Hd.TariffId, Hd.CompanyId, Hd.TariffNo, Hd.TariffDate, Hd.CustomerId, " +
                    $"Cur.CurrencyId, Cut.CustomerCode, Cut.CustomerName, Hd.IMONo, " +
                    $"Cur.CurrencyName, Cur.CurrencyCode, Hd.TotalAmt, Hd.TotalLocalAmt, Hd.Remark1, Hd.IsActive, Hd.IsClose, " +
                    $"Usr.UserName AS CreateBy, Usr1.UserName AS EditBy " +
                    $"FROM dbo.Ser_Tariff Hd " +
                    $"INNER JOIN dbo.M_Customer Cut ON Cut.CustomerId = Hd.CustomerId " +
                    $"INNER JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                    $"LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = Hd.CreateById " +
                    $"LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = Hd.EditById " +
                    $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                    $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                    $"OR Hd.TariffNo LIKE '%{searchString}%' " +
                    $"OR Hd.Remark1 LIKE '%{searchString}%') " +
                    $"AND Hd.TariffId <> 0 AND Hd.CompanyId = {CompanyId} " + additionalFilters + " " +
                    $"ORDER BY Hd.TariffNo " +
                    $"OFFSET {pageSize} * ({pageNumber - 1}) ROWS FETCH NEXT {pageSize} ROWS ONLY");

                countViewModel.responseCode = 200;
                countViewModel.responseMessage = "Success";
                countViewModel.totalRecords = totalcount == null ? 0 : totalcount.CountId;
                countViewModel.data = result?.ToList() ?? new List<TariffViewModel>();

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
                    TblName = "Ser_Tariff",
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
                    $"SELECT Hd.StatusId,COUNT(*) AS CountId FROM  dbo.Ser_Tariff Hd " +
                    $"INNER JOIN dbo.M_Customer Cut ON Cut.CustomerId = Hd.CustomerId " +
                    $"INNER JOIN dbo.M_Currency Cur ON Cur.CurrencyId = Hd.CurrencyId " +
                    $"WHERE (Cur.CurrencyName LIKE '%{searchString}%' " +
                    $"OR Cur.CurrencyCode LIKE '%{searchString}%' " +
                    $"OR Hd.TariffNo LIKE '%{searchString}%' " +
                    $"OR Hd.Remark1 LIKE '%{searchString}%') " +
                    $"AND Hd.TariffId <> 0 " +
                    $"AND Hd.CompanyId = {companyId} " +
                    $"AND Hd.TariffDate BETWEEN '{fromDate:yyyy-MM-dd}' AND '{toDate:yyyy-MM-dd}' " +
                    $"AND (Hd.CustomerId = {customerId} OR {customerId} = 0) " +
                    $"GROUP BY Hd.StatusId");

                // Aggregate counts for each status
                int countAll = countsResult.Sum(c => c.CountId);
                int countPending = countsResult.FirstOrDefault(c => c.StatusId == 401)?.CountId ?? 0;
                int countConfirm = countsResult.FirstOrDefault(c => c.StatusId == 402)?.CountId ?? 0;
                int countCompleted = countsResult.FirstOrDefault(c => c.StatusId == 400)?.CountId ?? 0;
                int countCancel = countsResult.FirstOrDefault(c => c.StatusId == 403)?.CountId ?? 0;
                int countPost = countsResult.FirstOrDefault(c => c.StatusId == 405)?.CountId ?? 0;

                // Build and return result
                return new StatusCountsViewModel
                {
                    All = countAll,
                    Pending = countPending,
                    Confirm = countConfirm,
                    Completed = countCompleted,
                    Cancel = countCancel,
                    Post = countPost
                };
            }
            catch (Exception ex)
            {
                // Log exception (if applicable) before re-throwing
                throw new Exception($"Error in GetJobStatusCountsAsync: {ex.Message}", ex);
            }
        }

        public async Task<TariffViewModel> GetTariffByIdAsync(short CompanyId, short UserId, int TariffId, string TariffCode, string TariffName)
        {
            try
            {
                var result = await _repository.GetQuerySingleOrDefaultAsync<TariffViewModel>($"SELECT   M_Ban.TariffId,M_Ban.CompanyId,M_Ban.TariffCode,M_Ban.TariffName,M_Ban.CurrencyId,M_Cur.CurrencyCode,M_Cur.CurrencyName,M_Ban.AccountNo,M_Ban.SwiftCode,M_Ban.Remark11,M_Ban.Remark12,M_Ban.IsActive,M_Ban.IsOwnTariff,M_Ban.GLId,M_Chr.GLName,M_Chr.GLCode,M_Ban.CreateById,M_Ban.CreateDate,M_Ban.EditById,M_Ban.EditDate,Usr.UserName AS CreateBy,Usr1.UserName AS EditBy FROM M_Tariff M_Ban INNER JOIN dbo.M_ChartOfAccount M_Chr ON M_Chr.GLId = M_Ban.GLId INNER JOIN M_Currency M_Cur ON M_Cur.CurrencyId = M_Ban.CurrencyId LEFT JOIN dbo.AdmUser Usr ON Usr.UserId = M_Ban.CreateById LEFT JOIN dbo.AdmUser Usr1 ON Usr1.UserId = M_Ban.EditById WHERE (M_Ban.TariffId={TariffId} OR {TariffId}=0) AND (M_Ban.TariffCode='{TariffCode}' OR '{TariffCode}'='{string.Empty}') AND (M_Ban.TariffName='{TariffName}' OR '{TariffName}'='{string.Empty}') AND M_Ban.CompanyId={CompanyId}");

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
                    TblName = "M_Tariff",
                    ModeId = (short)E_Mode.View,
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