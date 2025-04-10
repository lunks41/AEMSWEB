using AMESWEB.Areas.Project.Data.IServices;
using AMESWEB.Areas.Project.Models;
using AMESWEB.Controllers;
using AMESWEB.Entities.Project;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Project.Controllers
{
    [Area("project")]
    [Authorize]
    public class JobController : BaseController
    {
        private readonly ILogger<JobController> _logger;
        private readonly IJobOrderService _jobOrderService;
        private readonly IJobTaskService _jobTaskService;

        public JobController(ILogger<JobController> logger,
            IBaseService baseService,
            IJobOrderService jobOrderService,
            IJobTaskService jobTaskService)
            : base(logger, baseService)
        {
            _logger = logger;
            _jobOrderService = jobOrderService;
            _jobTaskService = jobTaskService;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? companyId)
        {
            ViewBag.IsRead = true;
            ViewBag.IsCreate = true;
            ViewBag.IsEdit = true;
            ViewBag.IsDelete = true;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region Job Order

        [HttpGet]
        public async Task<JsonResult> JobOrderList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, string fromDate, string toDate, string status)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            DateTime? fromDateParsed = null, toDateParsed = null;
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime dtFrom))
            {
                fromDateParsed = dtFrom;
            }
            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime dtTo))
            {
                toDateParsed = dtTo;
            }

            try
            {
                var data = await _jobOrderService.GetJobOrderListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, fromDateParsed, toDateParsed, status);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetJobOrderById(Int64 jobOrderId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid JobOrder ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobOrderService.GetJobOrderByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return data == null
                    ? Json(new { success = false, message = "JobOrder not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching jobOrder by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetJobStatusCounts(string searchString, string companyId, int customerId, string fromDate, string toDate)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
                return validationResult;

            DateTime? fromDateParsed = null;
            DateTime? toDateParsed = null;
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime dtFrom))
                fromDateParsed = dtFrom;
            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime dtTo))
                toDateParsed = dtTo;

            try
            {
                var counts = await _jobOrderService.GetJobStatusCountsAsync(
                    companyIdShort, parsedUserId.Value, searchString, customerId, fromDateParsed, toDateParsed
                );
                return Json(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job status counts");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Job Order

        #region Task

        #region Port Expenses

        [HttpGet]
        public async Task<JsonResult> PortExpensesList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetPortExpensesListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account type list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetPortExpensesById(Int64 jobOrderId, Int64 portExpenseId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Account Type ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetPortExpensesByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, portExpenseId);
                return data == null
                    ? Json(new { success = false, message = "Account Type not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account type by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePortExpenses([FromBody] SavePortExpensesViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var portExpenseToSave = new Ser_PortExpenses
                {
                    PortExpenseId = model.portExpense.PortExpenseId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.portExpense.JobOrderId,
                    JobOrderNo = model.portExpense.JobOrderNo ?? string.Empty,
                    TaskId = model.portExpense.TaskId,
                    Quantity = model.portExpense.Quantity,
                    SupplierId = model.portExpense.SupplierId,
                    ChargeId = model.portExpense.ChargeId,
                    StatusId = model.portExpense.StatusId,
                    UomId = model.portExpense.UomId,
                    DeliverDate = model.portExpense.DeliverDate == "" ? null : Convert.ToDateTime(model.portExpense.DeliverDate),
                    GLId = model.portExpense.GLId,
                    DebitNoteId = model.portExpense.DebitNoteId,
                    DebitNoteNo = model.portExpense.DebitNoteNo?.Trim(),
                    TotAmt = model.portExpense.TotAmt,
                    GstAmt = model.portExpense.GstAmt,
                    TotAmtAftGst = model.portExpense.TotAmtAftGst,
                    Remarks = model.portExpense.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var result = await _jobTaskService.SavePortExpensesAsync(companyIdShort, parsedUserId.Value, portExpenseToSave);
                return Json(new { success = true, message = "Account Type saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePortExpenses(Int64 jobOrderId, Int64 portExpenseId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Account Type ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Account Type ID." });
            }

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value, (short)E_Modules.Project, (short)E_Project.Job);
            if (permissions == null || !permissions.IsDelete)
            {
                _logger.LogWarning("Delete failed: User ID {UserId} does not have delete permissions.", parsedUserId.Value);
                return Json(new { success = false, message = "You do not have permission to delete this account group." });
            }

            try
            {
                await _jobTaskService.DeletePortExpensesAsync(companyIdShort, parsedUserId.Value, jobOrderId, portExpenseId);
                return Json(new { success = true, message = "Account Type deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Account Type. Account Type ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Port Expenses

        #region Launch Services

        [HttpGet]
        public async Task<JsonResult> LaunchServicesList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetLaunchServicesListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account type list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetLaunchServicesById(Int64 jobOrderId, Int64 launchServiceId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Account Type ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetLaunchServicesByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, launchServiceId);
                return data == null
                    ? Json(new { success = false, message = "Account Type not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account type by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveLaunchServices([FromBody] SaveLaunchServicesViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var launchServiceToSave = new Ser_LaunchServices
                {
                    LaunchServiceId = model.launchService.LaunchServiceId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.launchService.JobOrderId,
                    JobOrderNo = model.launchService.JobOrderNo?.Trim() ?? string.Empty,

                    TaskId = model.launchService.TaskId,
                    GLId = model.launchService.GLId,
                    ChargeId = model.launchService.ChargeId,
                    StatusId = model.launchService.StatusId,
                    UomId = model.launchService.UomId,

                    LaunchServiceDate = Convert.ToDateTime(model.launchService.LaunchServiceDate),

                    LoadingTime = string.IsNullOrWhiteSpace(model.launchService.LoadingTime)
                        ? (DateTime?)null
                        : Convert.ToDateTime(model.launchService.LoadingTime),
                    LeftJetty = string.IsNullOrWhiteSpace(model.launchService.LeftJetty)
                        ? (DateTime?)null
                        : Convert.ToDateTime(model.launchService.LeftJetty),

                    DebitNoteId = model.launchService.DebitNoteId,
                    DebitNoteNo = model.launchService.DebitNoteNo?.Trim(),

                    TotAmt = model.launchService.TotAmt,
                    GstAmt = model.launchService.GstAmt,
                    TotAmtAftGst = model.launchService.TotAmtAftGst,

                    Remarks = model.launchService.Remarks?.Trim() ?? string.Empty,
                    BoatOperator = model.launchService.BoatOperator?.Trim(),
                    InvoiceNo = model.launchService.InvoiceNo?.Trim(),
                    Annexure = model.launchService.Annexure?.Trim(),

                    DistanceFromJetty = model.launchService.DistanceFromJetty,
                    DistanceFromJettyToVessel = model.launchService.DistanceFromJettyToVessel,
                    WeightOfCargoDelivered = model.launchService.WeightOfCargoDelivered,
                    WeightOfCargoLanded = model.launchService.WeightOfCargoLanded,

                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                };

                var result = await _jobTaskService.SaveLaunchServicesAsync(companyIdShort, parsedUserId.Value, launchServiceToSave);
                return Json(new { success = true, message = "Account Type saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteLaunchServices(Int64 jobOrderId, Int64 launchServiceId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Account Type ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Account Type ID." });
            }

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
            {
                return validationResult;
            }

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value, (short)E_Modules.Project, (short)E_Project.Job);
            if (permissions == null || !permissions.IsDelete)
            {
                _logger.LogWarning("Delete failed: User ID {UserId} does not have delete permissions.", parsedUserId.Value);
                return Json(new { success = false, message = "You do not have permission to delete this account group." });
            }

            try
            {
                await _jobTaskService.DeleteLaunchServicesAsync(companyIdShort, parsedUserId.Value, jobOrderId, launchServiceId);
                return Json(new { success = true, message = "Account Type deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Account Type. Account Type ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Launch Services

        #region Equipment Used

        [HttpGet]
        public async Task<JsonResult> EquipmentsUsedList(string companyId, Int64 jobOrderId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (parsedUserId.HasValue)
                {
                    var result = await _jobTaskService.GetEquipmentsUsedListAsync(parsedCompanyId, parsedUserId.Value, jobOrderId);
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EquipmentsUsedList");
                return Json(new { success = false, message = "An error occurred while fetching equipment used list." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetEquipmentsUsedById(Int64 jobOrderId, Int64 equipmentsUsedId, string companyId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (parsedUserId.HasValue)
                {
                    var result = await _jobTaskService.GetEquipmentsUsedByIdAsync(parsedCompanyId, parsedUserId.Value, jobOrderId, equipmentsUsedId);
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetEquipmentsUsedById");
                return Json(new { success = false, message = "An error occurred while fetching equipment used details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveEquipmentsUsed([FromBody] SaveEquipmentsUsedViewModel model)
        {
            try
            {
                if (model == null || model.equipmentsUsed == null || string.IsNullOrEmpty(model.companyId))
                {
                    return BadRequest(new { success = false, message = "Invalid input data." });
                }

                var parsedCompanyId = short.Parse(model.companyId);
                var parsedUserId = GetParsedUserId();

                if (!parsedUserId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                // Create the entity from view model
                var equipmentsUsed = new Ser_EquipmentsUsed
                {
                    EquipmentsUsedId = model.equipmentsUsed.EquipmentsUsedId,
                    CompanyId = parsedCompanyId,
                    JobOrderId = model.equipmentsUsed.JobOrderId,
                    JobOrderNo = model.equipmentsUsed.JobOrderNo,
                    TaskId = model.equipmentsUsed.TaskId,

                    // Equipment details
                    EquipmentId = model.equipmentsUsed.EquipmentId,
                    EquipmentUseDate = model.equipmentsUsed.EquipmentUseDate,
                    Duration = model.equipmentsUsed.Duration,
                    GLId = model.equipmentsUsed.GLId,
                    StatusId = model.equipmentsUsed.StatusId,
                    ChargeId = model.equipmentsUsed.ChargeId,

                    // Financial information
                    DebitNoteId = model.equipmentsUsed.DebitNoteId,
                    DebitNoteNo = model.equipmentsUsed.DebitNoteNo,
                    TotAmt = model.equipmentsUsed.TotAmt,
                    GstAmt = model.equipmentsUsed.GstAmt,
                    TotAmtAftGst = model.equipmentsUsed.TotAmtAftGst,

                    // Additional information
                    Remarks = model.equipmentsUsed.Remarks ?? string.Empty,

                    // Audit fields
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = model.equipmentsUsed.EditById,
                    EditDate = model.equipmentsUsed.EditDate,
                    EditVersion = model.equipmentsUsed.EditVersion
                };

                var result = await _jobTaskService.SaveEquipmentsUsedAsync(parsedCompanyId, parsedUserId.Value, equipmentsUsed);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveEquipmentsUsed");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while saving equipment used data." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEquipmentsUsed(Int64 jobOrderId, Int64 equipmentsUsedId, string companyId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (!parsedUserId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var result = await _jobTaskService.DeleteEquipmentsUsedAsync(parsedCompanyId, parsedUserId.Value, jobOrderId, equipmentsUsedId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteEquipmentsUsed");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while deleting equipment used data." });
            }
        }

        #endregion Equipment Used

        #region Crew Sign On

        [HttpGet]
        public async Task<JsonResult> CrewSignOnList(string companyId, Int64 jobOrderId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (parsedUserId.HasValue)
                {
                    var result = await _jobTaskService.GetCrewSignOnListAsync(parsedCompanyId, parsedUserId.Value, jobOrderId);
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CrewSignOnList");
                return Json(new { success = false, message = "An error occurred while fetching crew sign on list." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCrewSignOnById(Int64 jobOrderId, Int64 crewSignOnId, string companyId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (parsedUserId.HasValue)
                {
                    var result = await _jobTaskService.GetCrewSignOnByIdAsync(parsedCompanyId, parsedUserId.Value, jobOrderId, crewSignOnId);
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCrewSignOnById");
                return Json(new { success = false, message = "An error occurred while fetching crew sign on details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCrewSignOn([FromBody] SaveCrewSignOnViewModel model)
        {
            try
            {
                if (model == null || model.crewSignOn == null || string.IsNullOrEmpty(model.companyId))
                {
                    return BadRequest(new { success = false, message = "Invalid input data." });
                }

                var parsedCompanyId = short.Parse(model.companyId);
                var parsedUserId = GetParsedUserId();

                if (!parsedUserId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var crewSignOn = new Ser_CrewSignOn
                {
                    CrewSignOnId = model.crewSignOn.CrewSignOnId,
                    CompanyId = parsedCompanyId,
                    JobOrderId = model.crewSignOn.JobOrderId,
                    JobOrderNo = model.crewSignOn.JobOrderNo,
                    TaskId = model.crewSignOn.TaskId,
                    CrewName = model.crewSignOn.CrewName,
                    SignOnDate = model.crewSignOn.SignOnDate,
                    Position = model.crewSignOn.Position,
                    GLId = model.crewSignOn.GLId,
                    GenderId = model.crewSignOn.GenderId,
                    Nationality = model.crewSignOn.Nationality,
                    VisaTypeId = model.crewSignOn.VisaTypeId,
                    TicketNo = model.crewSignOn.TicketNo,
                    Clearing = model.crewSignOn.Clearing,
                    TransportName = model.crewSignOn.TransportName,
                    HotelName = model.crewSignOn.HotelName,
                    StatusId = model.crewSignOn.StatusId,
                    FlightDetails = model.crewSignOn.FlightDetails,
                    RankId = model.crewSignOn.RankId,
                    ChargeId = model.crewSignOn.ChargeId,
                    DebitNoteId = model.crewSignOn.DebitNoteId,
                    DebitNoteNo = model.crewSignOn.DebitNoteNo,
                    TotAmt = model.crewSignOn.TotAmt,
                    GstAmt = model.crewSignOn.GstAmt,
                    TotAmtAftGst = model.crewSignOn.TotAmtAftGst,
                    Remarks = model.crewSignOn.Remarks ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = model.crewSignOn.EditById,
                    EditDate = model.crewSignOn.EditDate,
                    EditVersion = model.crewSignOn.EditVersion
                };

                var result = await _jobTaskService.SaveCrewSignOnAsync(parsedCompanyId, parsedUserId.Value, crewSignOn);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveCrewSignOn");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while saving crew sign on data." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCrewSignOn(Int64 jobOrderId, Int64 crewSignOnId, string companyId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (!parsedUserId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var result = await _jobTaskService.DeleteCrewSignOnAsync(parsedCompanyId, parsedUserId.Value, jobOrderId, crewSignOnId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCrewSignOn");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while deleting crew sign on data." });
            }
        }

        #endregion Crew Sign On

        #region Crew Sign Off

        [HttpGet]
        public async Task<JsonResult> CrewSignOffList(string companyId, Int64 jobOrderId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (parsedUserId.HasValue)
                {
                    var result = await _jobTaskService.GetCrewSignOffListAsync(parsedCompanyId, parsedUserId.Value, jobOrderId);
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CrewSignOffList");
                return Json(new { success = false, message = "An error occurred while fetching crew sign off list." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCrewSignOffById(Int64 jobOrderId, Int64 crewSignOffId, string companyId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (parsedUserId.HasValue)
                {
                    var result = await _jobTaskService.GetCrewSignOffByIdAsync(parsedCompanyId, parsedUserId.Value, jobOrderId, crewSignOffId);
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCrewSignOffById");
                return Json(new { success = false, message = "An error occurred while fetching crew sign off details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCrewSignOff([FromBody] SaveCrewSignOffViewModel model)
        {
            try
            {
                if (model == null || model.crewSignOff == null || string.IsNullOrEmpty(model.companyId))
                {
                    return BadRequest(new { success = false, message = "Invalid input data." });
                }

                var parsedCompanyId = short.Parse(model.companyId);
                var parsedUserId = GetParsedUserId();

                if (!parsedUserId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                // Create the entity from view model
                var crewSignOff = new Ser_CrewSignOff
                {
                    // Basic identification
                    CrewSignOffId = model.crewSignOff.CrewSignOffId,
                    CompanyId = parsedCompanyId,
                    JobOrderId = model.crewSignOff.JobOrderId,
                    JobOrderNo = model.crewSignOff.JobOrderNo,
                    TaskId = model.crewSignOff.TaskId,

                    // Crew details
                    CrewName = model.crewSignOff.CrewName,
                    SignOffDate = model.crewSignOff.SignOffDate,
                    Position = model.crewSignOff.Position,
                    GLId = model.crewSignOff.GLId,
                    GenderId = model.crewSignOff.GenderId,
                    Nationality = model.crewSignOff.Nationality,
                    VisaTypeId = model.crewSignOff.VisaTypeId,
                    TicketNo = model.crewSignOff.TicketNo,
                    Clearing = model.crewSignOff.Clearing,
                    TransportName = model.crewSignOff.TransportName,
                    HotelName = model.crewSignOff.HotelName,
                    StatusId = model.crewSignOff.StatusId,
                    FlightDetails = model.crewSignOff.FlightDetails,
                    RankId = model.crewSignOff.RankId,
                    ChargeId = model.crewSignOff.ChargeId,

                    // Financial information
                    DebitNoteId = model.crewSignOff.DebitNoteId,
                    DebitNoteNo = model.crewSignOff.DebitNoteNo,
                    TotAmt = model.crewSignOff.TotAmt,
                    GstAmt = model.crewSignOff.GstAmt,
                    TotAmtAftGst = model.crewSignOff.TotAmtAftGst,

                    // Additional information
                    Remarks = model.crewSignOff.Remarks ?? string.Empty,

                    // Audit fields
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = model.crewSignOff.EditById,
                    EditDate = model.crewSignOff.EditDate,
                    EditVersion = model.crewSignOff.EditVersion
                };

                var result = await _jobTaskService.SaveCrewSignOffAsync(parsedCompanyId, parsedUserId.Value, crewSignOff);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveCrewSignOff");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while saving crew sign off data." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCrewSignOff(Int64 jobOrderId, Int64 crewSignOffId, string companyId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (!parsedUserId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var result = await _jobTaskService.DeleteCrewSignOffAsync(parsedCompanyId, parsedUserId.Value, jobOrderId, crewSignOffId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCrewSignOff");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while deleting crew sign off data." });
            }
        }

        #endregion Crew Sign Off

        #region Crew Miscellaneous

        #endregion Crew Miscellaneous

        #region Medical Assistance

        [HttpGet]
        public async Task<IActionResult> GetMedicalAssistanceList(Int64 JobOrderId)
        {
            try
            {
                var result = await _jobTaskService.GetMedicalAssistanceListAsync(GetCompanyId(), GetUserId(), JobOrderId);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicalAssistanceById(Int64 JobOrderId, Int64 MedicalAssistanceId)
        {
            try
            {
                var result = await _jobTaskService.GetMedicalAssistanceByIdAsync(GetCompanyId(), GetUserId(), JobOrderId, MedicalAssistanceId);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveMedicalAssistance(Ser_MedicalAssistance ser_MedicalAssistance)
        {
            try
            {
                ser_MedicalAssistance.CompanyId = GetCompanyId();
                ser_MedicalAssistance.CreateById = GetUserId();

                var result = await _jobTaskService.SaveMedicalAssistanceAsync(GetCompanyId(), GetUserId(), ser_MedicalAssistance);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMedicalAssistance(Int64 JobOrderId, Int64 MedicalAssistanceId)
        {
            try
            {
                var result = await _jobTaskService.DeleteMedicalAssistanceAsync(GetCompanyId(), GetUserId(), JobOrderId, MedicalAssistanceId);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Medical Assistance

        #region Consignment Import

        #endregion Consignment Import

        #region Consignment Export

        #endregion Consignment Export

        #region Third Party Supply

        #endregion Third Party Supply

        #region Fresh Water Supply

        [HttpGet]
        public async Task<JsonResult> FreshWaterList(string companyId, Int64 jobOrderId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (parsedUserId.HasValue)
                {
                    var result = await _jobTaskService.GetFreshWaterListAsync(parsedCompanyId, parsedUserId.Value, jobOrderId);
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FreshWaterList");
                return Json(new { success = false, message = "An error occurred while fetching fresh water list." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetFreshWaterById(Int64 jobOrderId, Int64 freshWaterId, string companyId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (parsedUserId.HasValue)
                {
                    var result = await _jobTaskService.GetFreshWaterByIdAsync(parsedCompanyId, parsedUserId.Value, jobOrderId, freshWaterId);
                    return Json(result);
                }
                else
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFreshWaterById");
                return Json(new { success = false, message = "An error occurred while fetching fresh water details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveFreshWater([FromBody] SaveFreshWaterViewModel model)
        {
            try
            {
                if (model == null || model.freshWater == null || string.IsNullOrEmpty(model.companyId))
                {
                    return BadRequest(new { success = false, message = "Invalid input data." });
                }

                var parsedCompanyId = short.Parse(model.companyId);
                var parsedUserId = GetParsedUserId();

                if (!parsedUserId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                // Create the entity from view model
                var freshWater = new Ser_FreshWater
                {
                    FreshWaterId = model.freshWater.FreshWaterId,
                    CompanyId = parsedCompanyId,
                    JobOrderId = model.freshWater.JobOrderId,
                    JobOrderNo = model.freshWater.JobOrderNo,
                    TaskId = model.freshWater.TaskId,

                    // Supply details
                    SupplyDate = model.freshWater.SupplyDate,
                    Quantity = model.freshWater.Quantity,
                    UomId = model.freshWater.UomId,
                    SupplierId = model.freshWater.SupplierId,
                    GLId = model.freshWater.GLId,
                    StatusId = model.freshWater.StatusId,
                    ChargeId = model.freshWater.ChargeId,

                    // Financial information
                    DebitNoteId = model.freshWater.DebitNoteId,
                    DebitNoteNo = model.freshWater.DebitNoteNo,
                    TotAmt = model.freshWater.TotAmt,
                    GstAmt = model.freshWater.GstAmt,
                    TotAmtAftGst = model.freshWater.TotAmtAftGst,

                    // Additional information
                    Remarks = model.freshWater.Remarks ?? string.Empty,

                    // Audit fields
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = model.freshWater.EditById,
                    EditDate = model.freshWater.EditDate,
                    EditVersion = model.freshWater.EditVersion
                };

                var result = await _jobTaskService.SaveFreshWaterAsync(parsedCompanyId, parsedUserId.Value, freshWater);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveFreshWater");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while saving fresh water data." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFreshWater(Int64 jobOrderId, Int64 freshWaterId, string companyId)
        {
            try
            {
                var parsedCompanyId = short.Parse(companyId);
                var parsedUserId = GetParsedUserId();

                if (!parsedUserId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var result = await _jobTaskService.DeleteFreshWaterAsync(parsedCompanyId, parsedUserId.Value, jobOrderId, freshWaterId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteFreshWater");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while deleting fresh water data." });
            }
        }

        #endregion Fresh Water Supply

        #region FreshWater

        [HttpGet]
        public async Task<IActionResult> GetFreshWater(Int64 JobOrderId)
        {
            try
            {
                var result = await _service.GetFreshWaterListAsync(CompanyId, UserId, JobOrderId);

                return Json(new
                {
                    responseCode = 200,
                    responseMessage = "success",
                    totalRecords = result.totalRecords,
                    data = result.data
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    responseCode = 500,
                    responseMessage = ex.Message + " " + ex.InnerException?.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFreshWaterById(Int64 JobOrderId, Int64 FreshWaterId)
        {
            try
            {
                var result = await _service.GetFreshWaterByIdAsync(CompanyId, UserId, JobOrderId, FreshWaterId);

                return Json(new
                {
                    responseCode = 200,
                    responseMessage = "success",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    responseCode = 500,
                    responseMessage = ex.Message + " " + ex.InnerException?.Message
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveFreshWater(Ser_FreshWater ser_FreshWater)
        {
            try
            {
                ser_FreshWater.CompanyId = CompanyId;
                ser_FreshWater.CreateById = UserId;

                if (ser_FreshWater.FreshWaterId > 0)
                {
                    ser_FreshWater.EditById = UserId;
                    ser_FreshWater.EditDate = DateTime.Now;
                }

                var result = await _service.SaveFreshWaterAsync(CompanyId, UserId, ser_FreshWater);

                return Json(new
                {
                    responseCode = result.Result > 0 ? 200 : 400,
                    responseMessage = result.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    responseCode = 500,
                    responseMessage = ex.Message + " " + ex.InnerException?.Message
                });
            }
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteFreshWater(Int64 JobOrderId, Int64 FreshWaterId)
        {
            try
            {
                var result = await _service.DeleteFreshWaterAsync(CompanyId, UserId, JobOrderId, FreshWaterId);

                return Json(new
                {
                    responseCode = result.Result > 0 ? 200 : 400,
                    responseMessage = result.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    responseCode = 500,
                    responseMessage = ex.Message + " " + ex.InnerException?.Message
                });
            }
        }

        #endregion FreshWater

        #region Technicians Surveyors

        #endregion Technicians Surveyors

        #region Landing Items

        #endregion Landing Items

        #region Other Service

        #endregion Other Service

        #region Agency Remuneration

        #endregion Agency Remuneration

        #endregion Task

        #region DebitNote

        #endregion DebitNote

        #region Task Forward

        [HttpPost]
        public async Task<IActionResult> SaveTaskForward(Int64 jobOrderId, string jobOrderNo, Int64 prevJobOrderId, int taskId, string multipleId, string companyId)
        {
            if (multipleId == null || multipleId == "")
            {
                _logger.LogWarning($"Save failed: Invalid multiple ID {multipleId}.");
                return Json(new { success = false, message = "Invalid selected ID." });
            }

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var result = await _jobOrderService.SaveTaskForwardAsync(companyIdShort, parsedUserId.Value, jobOrderId, jobOrderNo, prevJobOrderId, taskId, multipleId);
                return Json(new { success = true, message = "Account Type saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Task Forward

        #region Bulk Update

        [HttpPost]
        public async Task<IActionResult> SaveBulkUpdate(Int64 jobOrderId, string jobOrderNo, Int64 prevJobOrderId, int taskId, string multipleId, string companyId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var result = await _jobOrderService.SaveTaskForwardAsync(companyIdShort, parsedUserId.Value, jobOrderId, jobOrderNo, prevJobOrderId, taskId, multipleId);
                return Json(new { success = true, message = "Account Type saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving account type");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Bulk Update

        [HttpGet]
        public async Task<JsonResult> GetTaskJobOrderCounts(string searchString, string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
                return validationResult;

            try
            {
                var counts = await _jobOrderService.GetTaskJobOrderCountsAsync(
                    companyIdShort, parsedUserId.Value, searchString, jobOrderId);
                return Json(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job status counts");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #region Purchase

        [HttpPost]
        public async Task<JsonResult> GetPurchaseJobOrder(string searchString, string companyId, Int64 jobOrderId, int taskId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
                return validationResult;

            try
            {
                var data = await _jobOrderService.GetPurchaseJobOrderAsync(companyIdShort, parsedUserId.Value, jobOrderId, taskId);
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job status counts");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SavePurchaseJobOrder(string searchString, string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null)
                return validationResult;

            try
            {
                var counts = await _jobOrderService.GetTaskJobOrderCountsAsync(
                    companyIdShort, parsedUserId.Value, searchString, jobOrderId);
                return Json(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job status counts");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Purchase
    }
}