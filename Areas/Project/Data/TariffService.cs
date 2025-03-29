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

        #region TASKS

        public async Task<TariffViewModelCount> GetTariffPortExpensesListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.PortExpenses} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.PortExpenses} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffLaunchServicesListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar  INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tar.SlabUomId INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.LaunchServices} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.SlabUomId,M_Or.OrderTypeName AS SlabUomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar  INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_OrderType M_Or ON M_Or.OrderTypeId = Ser_Tar.SlabUomId INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.LaunchServices} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffEquipmentsUsedListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.EquipmentsUsed} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.EquipmentsUsed} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffCrewSignOnListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.CrewSignOn} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.CrewSignOn} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffCrewSignOffListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.CrewSignOff} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.CrewSignOff} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffCrewMiscellaneousListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.CrewMiscellaneous} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.CrewMiscellaneous} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffMedicalAssistanceListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.MedicalAssistance} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.MedicalAssistance} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffConsignmentImportListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.ConsignmentImport} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.ConsignmentImport} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffConsignmentExportListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.ConsignmentExport} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.ConsignmentExport} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffThirdPartySupplyListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.ThirdPartySupply} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.ThirdPartySupply} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffFreshWaterSupplyListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
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

        public async Task<TariffViewModelCount> GetTariffTechniciansSurveyorsListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.TechniciansSurveyors} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.TechniciansSurveyors} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffLandingItemsListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.LandingItems} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.LandingItems} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffOtherServiceListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.OtherService} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.OtherService} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        public async Task<TariffViewModelCount> GetTariffAgencyRemunerationListAsync(short CompanyId, short UserId, int pageSize, int pageNumber, string searchString, int customerId, int portId)
        {
            TariffViewModelCount countViewModel = new TariffViewModelCount();
            try
            {
                // Count query for total records with additional filters
                var totalcount = await _repository.GetQuerySingleOrDefaultAsync<SqlResponseIds>(
                        $"SELECT COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.AgencyRemuneration} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

                // Query to fetch paginated data with the additional filters
                var result = await _repository.GetQueryAsync<TariffViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TariffId,Ser_Tar.TaskId,M_Tsk.TaskName,Ser_Tar.ChargeId,M_Ch.ChargeName,Ser_Tar.PortId,M_Po.PortName,Ser_Tar.CustomerId,M_Cu.CustomerName,Ser_Tar.CurrencyId,M_Cur.CurrencyName,Ser_Tar.UomId,M_Uo.UomName,Ser_Tar.BasicRate,Ser_Tar.MinUnit,Ser_Tar.MaxUnit,Ser_Tar.IsAdditional,Ser_Tar.AdditionalUnit,Ser_Tar.AdditionalRate FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId  INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.TaskId={(short)E_Task.AgencyRemuneration} AND Ser_Tar.CompanyId={CompanyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId}");

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

        #endregion TASKS

        public async Task<TaskCountsViewModel> GetTariffStatusCountsAsync(short companyId, short userId, string searchString, int customerId, int portId)
        {
            try
            {
                var countsResult = await _repository.GetQueryAsync<TaskCountViewModel>(
                    $"SELECT Ser_Tar.CompanyId,Ser_Tar.TaskId,COUNT(*) AS CountId FROM dbo.Ser_Tariff Ser_Tar INNER JOIN dbo.M_Customer M_Cu ON M_Cu.CustomerId = Ser_Tar.CustomerId INNER JOIN dbo.M_Currency M_Cur ON M_Cur.CurrencyId = Ser_Tar.CurrencyId INNER JOIN dbo.M_Port M_Po ON M_Po.PortId = Ser_Tar.PortId INNER JOIN dbo.M_Uom M_Uo ON M_Uo.UomId = Ser_Tar.UomId INNER JOIN dbo.M_Task M_Tsk ON M_Tsk.TaskId = Ser_Tar.TaskId INNER JOIN dbo.M_Charge M_Ch ON M_Ch.ChargeId = Ser_Tar.ChargeId AND M_Ch.TaskId = M_Tsk.TaskId WHERE Ser_Tar.CompanyId={companyId} AND Ser_Tar.CustomerId={customerId} AND Ser_Tar.PortId={portId} GROUP BY Ser_Tar.CompanyId,Ser_Tar.TaskId");

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
                throw new Exception($"Error in GetTariffStatusCountsAsync: {ex.Message}", ex);
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