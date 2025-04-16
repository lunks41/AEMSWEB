using AMESWEB.Areas.Project.Data.IServices;
using AMESWEB.Areas.Project.Models;
using AMESWEB.Controllers;
using AMESWEB.Entities.Project;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                var data = await _jobTaskService.SavePortExpensesAsync(companyIdShort, parsedUserId.Value, portExpenseToSave);
                return Json(new { success = true, message = "Account Type saved successfully", data = data });
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

                var data = await _jobTaskService.SaveLaunchServicesAsync(companyIdShort, parsedUserId.Value, launchServiceToSave);
                return Json(new { success = true, message = "Account Type saved successfully", data = data });
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
        public async Task<JsonResult> EquipmentUsedList(string companyId, Int64 jobOrderId)
        {
            try
            {
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                if (parsedUserId.HasValue)
                {
                    var data = await _jobTaskService.GetEquipmentsUsedListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                    return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                if (parsedUserId.HasValue)
                {
                    var data = await _jobTaskService.GetEquipmentsUsedByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, equipmentsUsedId);
                    return data == null
                    ? Json(new { success = false, message = "Account Type not found" })
                    : Json(new { success = true, data });
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

                var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                // Create the entity from view model
                var equipmentsUsed = new Ser_EquipmentsUsed
                {
                    EquipmentsUsedId = model.equipmentsUsed.EquipmentsUsedId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.equipmentsUsed.JobOrderId,
                    JobOrderNo = model.equipmentsUsed.JobOrderNo,
                    TaskId = model.equipmentsUsed.TaskId,

                    // Equipment details
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

                var data = await _jobTaskService.SaveEquipmentsUsedAsync(companyIdShort, parsedUserId.Value, equipmentsUsed);

                return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                var data = await _jobTaskService.DeleteEquipmentsUsedAsync(companyIdShort, parsedUserId.Value, jobOrderId, equipmentsUsedId);
                return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                if (parsedUserId.HasValue)
                {
                    var data = await _jobTaskService.GetCrewSignOnListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                    return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                if (parsedUserId.HasValue)
                {
                    var data = await _jobTaskService.GetCrewSignOnByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, crewSignOnId);
                    return Json(data);
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

                var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                var crewSignOn = new Ser_CrewSignOn
                {
                    CrewSignOnId = model.crewSignOn.CrewSignOnId,
                    CompanyId = companyIdShort,
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

                var data = await _jobTaskService.SaveCrewSignOnAsync(companyIdShort, parsedUserId.Value, crewSignOn);

                return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                var data = await _jobTaskService.DeleteCrewSignOnAsync(companyIdShort, parsedUserId.Value, jobOrderId, crewSignOnId);
                return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                if (parsedUserId.HasValue)
                {
                    var data = await _jobTaskService.GetCrewSignOffListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                    return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                if (parsedUserId.HasValue)
                {
                    var data = await _jobTaskService.GetCrewSignOffByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, crewSignOffId);
                    return Json(data);
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

                var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                // Create the entity from view model
                var crewSignOff = new Ser_CrewSignOff
                {
                    // Basic identification
                    CrewSignOffId = model.crewSignOff.CrewSignOffId,
                    CompanyId = companyIdShort,
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

                var data = await _jobTaskService.SaveCrewSignOffAsync(companyIdShort, parsedUserId.Value, crewSignOff);

                return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                var data = await _jobTaskService.DeleteCrewSignOffAsync(companyIdShort, parsedUserId.Value, jobOrderId, crewSignOffId);
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCrewSignOff");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while deleting crew sign off data." });
            }
        }

        #endregion Crew Sign Off

        #region Crew Miscellaneous

        [HttpGet]
        public async Task<JsonResult> CrewMiscellaneousList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetCrewMiscellaneousListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Crew Miscellaneous list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCrewMiscellaneousById(Int64 jobOrderId, Int64 crewMiscellaneousId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Crew Miscellaneous ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetCrewMiscellaneousByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, crewMiscellaneousId);
                return data == null
                    ? Json(new { success = false, message = "Crew Miscellaneous not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Crew Miscellaneous by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveCrewMiscellaneous([FromBody] SaveCrewMiscellaneousViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var crewMiscellaneousToSave = new Ser_CrewMiscellaneous
                {
                    CrewMiscellaneousId = model.crewMiscellaneous.CrewMiscellaneousId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.crewMiscellaneous.JobOrderId,
                    JobOrderNo = model.crewMiscellaneous.JobOrderNo ?? string.Empty,
                    TaskId = model.crewMiscellaneous.TaskId,
                    Remarks = model.crewMiscellaneous.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                };

                var data = await _jobTaskService.SaveCrewMiscellaneousAsync(companyIdShort, parsedUserId.Value, crewMiscellaneousToSave);
                return Json(new { success = true, message = "Crew Miscellaneous saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Crew Miscellaneous");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCrewMiscellaneous(Int64 jobOrderId, Int64 crewMiscellaneousId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Crew Miscellaneous ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Crew Miscellaneous ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this Crew Miscellaneous." });
            }

            try
            {
                await _jobTaskService.DeleteCrewMiscellaneousAsync(companyIdShort, parsedUserId.Value, jobOrderId, crewMiscellaneousId);
                return Json(new { success = true, message = "Crew Miscellaneous deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Crew Miscellaneous. Crew Miscellaneous ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Crew Miscellaneous

        #region Medical Assistance

        [HttpGet]
        public async Task<JsonResult> MedicalAssistanceList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetMedicalAssistanceListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Medical Assistance list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetMedicalAssistanceById(Int64 jobOrderId, Int64 medicalAssistanceId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Medical Assistance ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetMedicalAssistanceByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, medicalAssistanceId);
                return data == null
                    ? Json(new { success = false, message = "Medical Assistance not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Medical Assistance by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveMedicalAssistance([FromBody] SaveMedicalAssistanceViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var medicalAssistanceToSave = new Ser_MedicalAssistance
                {
                    MedicalAssistanceId = model.medicalAssistance.MedicalAssistanceId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.medicalAssistance.JobOrderId,
                    JobOrderNo = model.medicalAssistance.JobOrderNo ?? string.Empty,
                    TaskId = model.medicalAssistance.TaskId,
                    Remarks = model.medicalAssistance.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var data = await _jobTaskService.SaveMedicalAssistanceAsync(companyIdShort, parsedUserId.Value, medicalAssistanceToSave);
                return Json(new { success = true, message = "Medical Assistance saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Medical Assistance");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMedicalAssistance(Int64 jobOrderId, Int64 medicalAssistanceId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Medical Assistance ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Medical Assistance ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this Medical Assistance." });
            }

            try
            {
                await _jobTaskService.DeleteMedicalAssistanceAsync(companyIdShort, parsedUserId.Value, jobOrderId, medicalAssistanceId);
                return Json(new { success = true, message = "Medical Assistance deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Medical Assistance. Medical Assistance ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Medical Assistance

        #region Agency Remuneration

        [HttpGet]
        public async Task<JsonResult> AgencyRemunerationList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetAgencyRemunerationListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching agency remuneration list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAgencyRemunerationById(Int64 jobOrderId, Int64 agencyRemunerationId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Agency Remuneration ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetAgencyRemunerationByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, agencyRemunerationId);
                return data == null
                    ? Json(new { success = false, message = "Agency Remuneration not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching agency remuneration by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAgencyRemuneration([FromBody] SaveAgencyRemunerationViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var agencyRemunerationToSave = new Ser_AgencyRemuneration
                {
                    AgencyRemunerationId = model.agencyRemuneration.AgencyRemunerationId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.agencyRemuneration.JobOrderId,
                    JobOrderNo = model.agencyRemuneration.JobOrderNo ?? string.Empty,
                    TaskId = model.agencyRemuneration.TaskId,
                    ChargeId = model.agencyRemuneration.ChargeId,
                    StatusId = model.agencyRemuneration.StatusId,
                    GLId = model.agencyRemuneration.GLId,
                    DebitNoteId = model.agencyRemuneration.DebitNoteId,
                    DebitNoteNo = model.agencyRemuneration.DebitNoteNo?.Trim(),
                    TotAmt = model.agencyRemuneration.TotAmt,
                    GstAmt = model.agencyRemuneration.GstAmt,
                    TotAmtAftGst = model.agencyRemuneration.TotAmtAftGst,
                    Remarks = model.agencyRemuneration.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var data = await _jobTaskService.SaveAgencyRemunerationAsync(companyIdShort, parsedUserId.Value, agencyRemunerationToSave);
                return Json(new { success = true, message = "Agency Remuneration saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving agency remuneration");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAgencyRemuneration(Int64 jobOrderId, Int64 agencyRemunerationId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Agency Remuneration ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Agency Remuneration ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this agency remuneration." });
            }

            try
            {
                await _jobTaskService.DeleteAgencyRemunerationAsync(companyIdShort, parsedUserId.Value, jobOrderId, agencyRemunerationId);
                return Json(new { success = true, message = "Agency Remuneration deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Agency Remuneration. Agency Remuneration ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Agency Remuneration

        #region Consignment Export

        [HttpGet]
        public async Task<JsonResult> ConsignmentExportList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetConsignmentExportListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching consignment export list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetConsignmentExportById(Int64 jobOrderId, Int64 consignmentExportId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Consignment Export ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetConsignmentExportByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, consignmentExportId);
                return data == null
                    ? Json(new { success = false, message = "Consignment Export not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching consignment export by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveConsignmentExport([FromBody] SaveConsignmentExportViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var consignmentExportToSave = new Ser_ConsignmentExport
                {
                    ConsignmentExportId = model.consignmentExport.ConsignmentExportId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.consignmentExport.JobOrderId,
                    JobOrderNo = model.consignmentExport.JobOrderNo ?? string.Empty,
                    TaskId = model.consignmentExport.TaskId,
                    ConsignmentNo = model.consignmentExport.ConsignmentNo?.Trim(),
                    CargoTypeId = model.consignmentExport.CargoTypeId,
                    Quantity = model.consignmentExport.Quantity,
                    UomId = model.consignmentExport.UomId,
                    ChargeId = model.consignmentExport.ChargeId,
                    StatusId = model.consignmentExport.StatusId,
                    GLId = model.consignmentExport.GLId,
                    DebitNoteId = model.consignmentExport.DebitNoteId,
                    DebitNoteNo = model.consignmentExport.DebitNoteNo?.Trim(),
                    TotAmt = model.consignmentExport.TotAmt,
                    GstAmt = model.consignmentExport.GstAmt,
                    TotAmtAftGst = model.consignmentExport.TotAmtAftGst,
                    Remarks = model.consignmentExport.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var data = await _jobTaskService.SaveConsignmentExportAsync(companyIdShort, parsedUserId.Value, consignmentExportToSave);
                return Json(new { success = true, message = "Consignment Export saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving consignment export");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteConsignmentExport(Int64 jobOrderId, Int64 consignmentExportId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Consignment Export ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Consignment Export ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this consignment export." });
            }

            try
            {
                await _jobTaskService.DeleteConsignmentExportAsync(companyIdShort, parsedUserId.Value, jobOrderId, consignmentExportId);
                return Json(new { success = true, message = "Consignment Export deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Consignment Export. Consignment Export ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Consignment Export

        #region Consignment Import

        [HttpGet]
        public async Task<JsonResult> ConsignmentImportList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetConsignmentImportListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching consignment import list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetConsignmentImportById(Int64 jobOrderId, Int64 consignmentImportId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Consignment Import ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetConsignmentImportByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, consignmentImportId);
                return data == null
                    ? Json(new { success = false, message = "Consignment Import not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching consignment import by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveConsignmentImport([FromBody] SaveConsignmentImportViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var consignmentImportToSave = new Ser_ConsignmentImport
                {
                    ConsignmentImportId = model.consignmentImport.ConsignmentImportId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.consignmentImport.JobOrderId,
                    JobOrderNo = model.consignmentImport.JobOrderNo ?? string.Empty,
                    TaskId = model.consignmentImport.TaskId,
                    ConsignmentNo = model.consignmentImport.ConsignmentNo?.Trim() ?? string.Empty,
                    CargoTypeId = model.consignmentImport.CargoTypeId,
                    Quantity = model.consignmentImport.Quantity,
                    UomId = model.consignmentImport.UomId,
                    ChargeId = model.consignmentImport.ChargeId,
                    StatusId = model.consignmentImport.StatusId,
                    GLId = model.consignmentImport.GLId,
                    DebitNoteId = model.consignmentImport.DebitNoteId,
                    DebitNoteNo = model.consignmentImport.DebitNoteNo?.Trim(),
                    TotAmt = model.consignmentImport.TotAmt,
                    GstAmt = model.consignmentImport.GstAmt,
                    TotAmtAftGst = model.consignmentImport.TotAmtAftGst,
                    Remarks = model.consignmentImport.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var data = await _jobTaskService.SaveConsignmentImportAsync(companyIdShort, parsedUserId.Value, consignmentImportToSave);
                return Json(new { success = true, message = "Consignment Import saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving consignment import");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteConsignmentImport(Int64 jobOrderId, Int64 consignmentImportId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Consignment Import ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Consignment Import ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this consignment import." });
            }

            try
            {
                await _jobTaskService.DeleteConsignmentImportAsync(companyIdShort, parsedUserId.Value, jobOrderId, consignmentImportId);
                return Json(new { success = true, message = "Consignment Import deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Consignment Import. Consignment Import ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Consignment Import

        #region Landing Items

        [HttpGet]
        public async Task<JsonResult> LandingItemsList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetLandingItemsListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching landing items list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetLandingItemsById(Int64 jobOrderId, Int64 landingItemId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Landing Item ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetLandingItemsByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, landingItemId);
                return data == null
                    ? Json(new { success = false, message = "Landing Item not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching landing item by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveLandingItems([FromBody] SaveLandingItemsViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var landingItemToSave = new Ser_LandingItems
                {
                    LandingItemId = model.landingItem.LandingItemId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.landingItem.JobOrderId,
                    JobOrderNo = model.landingItem.JobOrderNo ?? string.Empty,
                    TaskId = model.landingItem.TaskId,
                    ItemName = model.landingItem.ItemName?.Trim(),
                    Quantity = model.landingItem.Quantity,
                    UomId = model.landingItem.UomId,
                    ChargeId = model.landingItem.ChargeId,
                    StatusId = model.landingItem.StatusId,
                    GLId = model.landingItem.GLId,
                    DebitNoteId = model.landingItem.DebitNoteId,
                    DebitNoteNo = model.landingItem.DebitNoteNo?.Trim(),
                    TotAmt = model.landingItem.TotAmt,
                    GstAmt = model.landingItem.GstAmt,
                    TotAmtAftGst = model.landingItem.TotAmtAftGst,
                    Remarks = model.landingItem.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var data = await _jobTaskService.SaveLandingItemsAsync(companyIdShort, parsedUserId.Value, landingItemToSave);
                return Json(new { success = true, message = "Landing Item saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving landing item");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteLandingItems(Int64 jobOrderId, Int64 landingItemId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Landing Item ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Landing Item ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this landing item." });
            }

            try
            {
                await _jobTaskService.DeleteLandingItemsAsync(companyIdShort, parsedUserId.Value, jobOrderId, landingItemId);
                return Json(new { success = true, message = "Landing Item deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Landing Item. Landing Item ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Landing Items

        #region Other Service

        [HttpGet]
        public async Task<JsonResult> OtherServiceList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetOtherServiceListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching other service list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOtherServiceById(Int64 jobOrderId, Int64 otherServiceId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Other Service ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetOtherServiceByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, otherServiceId);
                return data == null
                    ? Json(new { success = false, message = "Other Service not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching other service by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOtherService([FromBody] SaveOtherServiceViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var otherServiceToSave = new Ser_OtherService
                {
                    OtherServiceId = model.otherService.OtherServiceId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.otherService.JobOrderId,
                    JobOrderNo = model.otherService.JobOrderNo ?? string.Empty,
                    TaskId = model.otherService.TaskId,
                    ChargeId = model.otherService.ChargeId,
                    StatusId = model.otherService.StatusId,
                    GLId = model.otherService.GLId,
                    DebitNoteId = model.otherService.DebitNoteId,
                    DebitNoteNo = model.otherService.DebitNoteNo?.Trim(),
                    TotAmt = model.otherService.TotAmt,
                    GstAmt = model.otherService.GstAmt,
                    TotAmtAftGst = model.otherService.TotAmtAftGst,
                    Remarks = model.otherService.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var data = await _jobTaskService.SaveOtherServiceAsync(companyIdShort, parsedUserId.Value, otherServiceToSave);
                return Json(new { success = true, message = "Other Service saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving other service");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOtherService(Int64 jobOrderId, Int64 otherServiceId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Other Service ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Other Service ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this other service." });
            }

            try
            {
                await _jobTaskService.DeleteOtherServiceAsync(companyIdShort, parsedUserId.Value, jobOrderId, otherServiceId);
                return Json(new { success = true, message = "Other Service deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Other Service. Other Service ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Other Service

        #region Technicians Surveyors

        [HttpGet]
        public async Task<JsonResult> TechniciansSurveyorsList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetTechniciansSurveyorsListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching technicians surveyors list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTechniciansSurveyorsById(Int64 jobOrderId, Int64 technicianSurveyorId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Technician/Surveyor ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetTechniciansSurveyorsByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, technicianSurveyorId);
                return data == null
                    ? Json(new { success = false, message = "Technician/Surveyor not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching technician/surveyor by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTechniciansSurveyors([FromBody] SaveTechniciansSurveyorsViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var technicianSurveyorToSave = new Ser_TechniciansSurveyors
                {
                    TechnicianSurveyorId = model.technicianSurveyor.TechnicianSurveyorId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.technicianSurveyor.JobOrderId,
                    JobOrderNo = model.technicianSurveyor.JobOrderNo ?? string.Empty,
                    TaskId = model.technicianSurveyor.TaskId,
                    ChargeId = model.technicianSurveyor.ChargeId,
                    StatusId = model.technicianSurveyor.StatusId,
                    GLId = model.technicianSurveyor.GLId,
                    DebitNoteId = model.technicianSurveyor.DebitNoteId,
                    DebitNoteNo = model.technicianSurveyor.DebitNoteNo?.Trim(),
                    TotAmt = model.technicianSurveyor.TotAmt,
                    GstAmt = model.technicianSurveyor.GstAmt,
                    TotAmtAftGst = model.technicianSurveyor.TotAmtAftGst,
                    Remarks = model.technicianSurveyor.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var data = await _jobTaskService.SaveTechniciansSurveyorsAsync(companyIdShort, parsedUserId.Value, technicianSurveyorToSave);
                return Json(new { success = true, message = "Technician/Surveyor saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving technician/surveyor");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTechniciansSurveyors(Int64 jobOrderId, Int64 technicianSurveyorId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Technician/Surveyor ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Technician/Surveyor ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this technician/surveyor." });
            }

            try
            {
                await _jobTaskService.DeleteTechniciansSurveyorsAsync(companyIdShort, parsedUserId.Value, jobOrderId, technicianSurveyorId);
                return Json(new { success = true, message = "Technician/Surveyor deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Technician/Surveyor. Technician/Surveyor ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Technicians Surveyors

        #region Third Party Supply

        [HttpGet]
        public async Task<JsonResult> ThirdPartySupplyList(string companyId, Int64 jobOrderId)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetThirdPartySupplyListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching third party supply list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetThirdPartySupplyById(Int64 jobOrderId, Int64 thirdPartySupplyId, string companyId)
        {
            if (jobOrderId <= 0)
                return Json(new { success = false, message = "Invalid Third Party Supply ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _jobTaskService.GetThirdPartySupplyByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, thirdPartySupplyId);
                return data == null
                    ? Json(new { success = false, message = "Third Party Supply not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching third party supply by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveThirdPartySupply([FromBody] SaveThirdPartySupplyViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var thirdPartySupplyToSave = new Ser_ThirdPartySupply
                {
                    ThirdPartySupplyId = model.thirdPartySupply.ThirdPartySupplyId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.thirdPartySupply.JobOrderId,
                    JobOrderNo = model.thirdPartySupply.JobOrderNo ?? string.Empty,
                    TaskId = model.thirdPartySupply.TaskId,
                    SupplierName = model.thirdPartySupply.SupplierName?.Trim(),
                    Quantity = model.thirdPartySupply.Quantity,
                    UomId = model.thirdPartySupply.UomId,
                    ChargeId = model.thirdPartySupply.ChargeId,
                    StatusId = model.thirdPartySupply.StatusId,
                    GLId = model.thirdPartySupply.GLId,
                    DebitNoteId = model.thirdPartySupply.DebitNoteId,
                    DebitNoteNo = model.thirdPartySupply.DebitNoteNo?.Trim(),
                    TotAmt = model.thirdPartySupply.TotAmt,
                    GstAmt = model.thirdPartySupply.GstAmt,
                    TotAmtAftGst = model.thirdPartySupply.TotAmtAftGst,
                    Remarks = model.thirdPartySupply.Remarks?.Trim() ?? string.Empty,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now,
                    EditVersion = 0
                };

                var data = await _jobTaskService.SaveThirdPartySupplyAsync(companyIdShort, parsedUserId.Value, thirdPartySupplyToSave);
                return Json(new { success = true, message = "Third Party Supply saved successfully", data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving third party supply");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteThirdPartySupply(Int64 jobOrderId, Int64 thirdPartySupplyId, string companyId)
        {
            if (jobOrderId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Third Party Supply ID {JobOrderId}.", jobOrderId);
                return Json(new { success = false, message = "Invalid Third Party Supply ID." });
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
                return Json(new { success = false, message = "You do not have permission to delete this third party supply." });
            }

            try
            {
                await _jobTaskService.DeleteThirdPartySupplyAsync(companyIdShort, parsedUserId.Value, jobOrderId, thirdPartySupplyId);
                return Json(new { success = true, message = "Third Party Supply deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Third Party Supply. Third Party Supply ID: {JobOrderId}, Company ID: {CompanyId}.", jobOrderId, companyId);
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        #endregion Third Party Supply

        #region Fresh Water Supply

        [HttpGet]
        public async Task<JsonResult> FreshWaterList(string companyId, Int64 jobOrderId)
        {
            try
            {
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                if (parsedUserId.HasValue)
                {
                    var data = await _jobTaskService.GetFreshWaterListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
                    return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                if (parsedUserId.HasValue)
                {
                    var data = await _jobTaskService.GetFreshWaterByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, freshWaterId);
                    return Json(data);
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

                var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                // Create the entity from view model
                var freshWater = new Ser_FreshWater
                {
                    FreshWaterId = model.freshWater.FreshWaterId,
                    CompanyId = companyIdShort,
                    JobOrderId = model.freshWater.JobOrderId,
                    JobOrderNo = model.freshWater.JobOrderNo,
                    TaskId = model.freshWater.TaskId,

                    // Supply details
                    Quantity = model.freshWater.Quantity,
                    UomId = model.freshWater.UomId,
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

                var data = await _jobTaskService.SaveFreshWaterAsync(companyIdShort, parsedUserId.Value, freshWater);

                return Json(data);
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
                var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
                if (validationResult != null) return validationResult;

                var data = await _jobTaskService.DeleteFreshWaterAsync(companyIdShort, parsedUserId.Value, jobOrderId, freshWaterId);
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteFreshWater");
                return Json(new SqlResponce { Result = -1, Message = "An error occurred while deleting fresh water data." });
            }
        }

        #endregion Fresh Water Supply

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
                var data = await _jobOrderService.SaveTaskForwardAsync(companyIdShort, parsedUserId.Value, jobOrderId, jobOrderNo, prevJobOrderId, taskId, multipleId);
                return Json(new { success = true, message = "Account Type saved successfully", data = data });
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
                var data = await _jobOrderService.SaveTaskForwardAsync(companyIdShort, parsedUserId.Value, jobOrderId, jobOrderNo, prevJobOrderId, taskId, multipleId);
                return Json(new { success = true, message = "Account Type saved successfully", data = data });
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