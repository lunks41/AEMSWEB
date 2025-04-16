using AMESWEB.Areas.Project.Data.IServices;
using AMESWEB.Areas.Project.Models;
using AMESWEB.Controllers;
using AMESWEB.Entities.Project;
using AMESWEB.Enums;
using AMESWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Project.Controllers
{
    //Rates of tariff task wise
    [Area("project")]
    public class TariffController : BaseController
    {
        private readonly ILogger<TariffController> _logger;
        private readonly ITariffService _tariffService;

        public TariffController(ILogger<TariffController> logger,
            IBaseService baseService,
            ITariffService tariffService)
            : base(logger, baseService)
        {
            _logger = logger;
            _tariffService = tariffService;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? companyId)
        {
            //if (!companyId.HasValue || companyId <= 0)
            //{
            //    _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
            //    return Json(new { success = false, message = "Invalid company ID." });
            //}

            //var parsedUserId = GetParsedUserId();
            //if (!parsedUserId.HasValue)
            //{
            //    _logger.LogWarning("User not logged in or invalid user ID.");
            //    return Json(new { success = false, message = "User not logged in or invalid user ID." });
            //}

            //var permissions = await HasPermission((short)companyId, parsedUserId.Value,
            //    (short)E_Modules.Project, (short)E_Project.Tariff);

            ViewBag.IsRead = true;
            ViewBag.IsCreate = true;
            ViewBag.IsEdit = true;
            ViewBag.IsDelete = true;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region TASKS LIST

        [HttpGet]
        public async Task<JsonResult> GetTariffPortExpensesList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffPortExpensesListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffLaunchServicesList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffLaunchServicesListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffEquipmentsUsedList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffEquipmentsUsedListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffCrewSignOnList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffCrewSignOnListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffCrewSignOffList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffCrewSignOffListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffCrewMiscellaneousList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffCrewMiscellaneousListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffMedicalAssistanceList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffMedicalAssistanceListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffConsignmentImportList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffConsignmentImportListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffConsignmentExportList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffConsignmentExportListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffThirdPartyList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffThirdPartyListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffFreshWaterList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffFreshWaterListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffTechniciansSurveyorsList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffTechniciansSurveyorsListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffLandingItemsList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffLandingItemsListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffOtherServiceList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffOtherServiceListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffAgencyRemunerationList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffAgencyRemunerationListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTariffVisaList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, int portId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _tariffService.GetTariffFreshWaterListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, portId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion TASKS LIST

        [HttpPost]
        public async Task<JsonResult> GetTariffStatusCounts(string searchString, string companyId, int customerId, int portId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
                return validationResult;

            try
            {
                var counts = await _tariffService.GetTariffStatusCountsAsync(
                    companyIdShort, parsedUserId.Value, searchString, customerId, portId);
                return Json(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job status counts");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #region CRUD

        [HttpGet]
        public async Task<JsonResult> GetTariffById(short tariffId, string companyId, int taskId, int customerId, int portId, int chargeId)
        {
            // Validate input parameters
            if (tariffId <= 0)
            {
                return Json(new { success = false, message = "Invalid Tariff ID" });
            }

            // Validate companyId and parsed userId
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return validationResult; // Return validation error result
            }

            try
            {
                // Fetch tariff data asynchronously
                var data = await _tariffService.GetTariffByIdAsync(companyIdShort, parsedUserId.Value, tariffId, taskId, customerId, portId, chargeId);

                // Check if data exists
                return data == null
                    ? Json(new { success = false, message = "Tariff not found" }) // Use "Tariff not found" for clarity
                    : Json(new { success = true, data }); // Return success and data
            }
            catch (Exception ex)
            {
                // Log error and return user-friendly error response
                _logger.LogError(ex, $"Error fetching tariff by ID: {tariffId}");
                return Json(new { success = false, message = "An error occurred while fetching tariff data" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTariff([FromBody] SaveTariffViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var tariffToSave = new Ser_Tariff
                {
                    TariffId = model.tariff.TariffId,
                    CompanyId = companyIdShort,
                    RateType = model.tariff.RateType?.Trim() ?? string.Empty, // Trim unnecessary spaces
                    TaskId = model.tariff.TaskId,
                    ChargeId = model.tariff.ChargeId,
                    PortId = model.tariff.PortId,
                    CustomerId = model.tariff.CustomerId,
                    CurrencyId = model.tariff.CurrencyId,
                    UomId = model.tariff.UomId,
                    SlabUomId = model.tariff.SlabUomId,
                    VisaTypeId = model.tariff.VisaTypeId,
                    FromPlace = model.tariff.FromPlace,
                    ToPlace = model.tariff.ToPlace,
                    DisplayRate = model.tariff.DisplayRate,
                    BasicRate = model.tariff.BasicRate,
                    MinUnit = model.tariff.MinUnit,
                    MaxUnit = model.tariff.MaxUnit,
                    IsAdditional = model.tariff.IsAdditional,
                    AdditionalUnit = model.tariff.AdditionalUnit,
                    AdditionalRate = model.tariff.AdditionalRate,
                    PrepaymentPercentage = model.tariff.PrepaymentPercentage,
                    IsPrepayment = model.tariff.IsPrepayment,
                    ItemNo = model.tariff.ItemNo,
                    Remarks = model.tariff.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.tariff.IsActive,
                    CreateById = parsedUserId ?? 0, // Safe handling if parsedUserId is null
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId ?? 0,
                    EditDate = DateTime.Now
                };

                var result = await _tariffService.SaveTariffAsync(companyIdShort, parsedUserId.Value, tariffToSave);
                return Json(new { success = true, message = "Account Type saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTariff(short tariffId, string companyId, int taskId, int customerId, int portId, int chargeId)
        {
            if (tariffId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Account Type ID {tariffId}.", tariffId);
                return Json(new { success = false, message = "Invalid Account Type ID." });
            }

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value, (short)E_Modules.Project, (short)E_Project.Tariff);
            if (permissions == null || !permissions.IsDelete)
            {
                _logger.LogWarning("Delete failed: User ID {UserId} does not have delete permissions.", parsedUserId.Value);
                return Json(new { success = false, message = "You do not have permission to delete this account group." });
            }

            try
            {
                await _tariffService.DeleteTariffAsync(companyIdShort, parsedUserId.Value, tariffId, taskId, customerId, portId, chargeId);
                return Json(new { success = true, message = "Account Type deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Account Type. Account Type ID: {AccTypeId}, Company ID: {CompanyId}.", tariffId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion CRUD

        [HttpPost]
        public IActionResult CopyRates(string companyId, int fromCustomerId, int fromPortId, int fromTaskId, int toCustomerId, int toPortId, int toTaskId, bool overwriteExisting, bool deleteExisting)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
                return validationResult;

            try
            {
                var sourceRates = _tariffService.CopyCustomerTariffAsync(companyIdShort, parsedUserId.Value, fromCustomerId, fromPortId, fromTaskId, toCustomerId, toPortId, toTaskId,
                                  overwriteExisting, deleteExisting);

                return Json(new { success = true, message = "Rates copied successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CopyCompanyRates(string companyId, int fromCompanyId, int fromCustomerId, int fromPortId, int fromTaskId, int toCompanyId, int toCustomerId, int toPortId, int toTaskId, bool overwriteExisting, bool deleteExisting)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
                return validationResult;

            try
            {
                var sourceRates = _tariffService.CopyCompanyToCustomerTariffAsync(companyIdShort, parsedUserId.Value, fromCompanyId, fromCustomerId, fromPortId, fromTaskId, toCompanyId, toCustomerId, toPortId, toTaskId, overwriteExisting, deleteExisting);

                return Json(new { success = true, message = "Rates copied successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}