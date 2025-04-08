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
    [Area("project")]
    [Authorize]
    public class JobController : BaseController
    {
        private readonly ILogger<JobController> _logger;
        private readonly IJobOrderService _jobOrderService;

        public JobController(ILogger<JobController> logger,
            IBaseService baseService,
            IJobOrderService jobOrderService)
            : base(logger, baseService)
        {
            _logger = logger;
            _jobOrderService = jobOrderService;
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
            //    (short)E_Modules.Project, (short)E_Project.Job);

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

            // Parse the date filters if provided
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

        //[HttpPost]
        //public async Task<JsonResult> JobOrderList(int pageNumber, int pageSize, string searchString, string companyId, string customerId, string fromDate, string toDate, string status)
        //{
        //    if (pageNumber < 1 || pageSize < 1)
        //        return Json(new { success = false, message = "Invalid page parameters" });

        //    var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
        //    if (validationResult != null)
        //        return validationResult;

        //    // Parse the date filters if provided
        //    DateTime? fromDateParsed = null, toDateParsed = null;
        //    if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime dtFrom))
        //    {
        //        fromDateParsed = dtFrom;
        //    }
        //    if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime dtTo))
        //    {
        //        toDateParsed = dtTo;
        //    }

        //    try
        //    {
        //        // Updated the service call to include the new parameters.
        //        var data = await _jobOrderService.GetJobOrderListAsyncV1(
        //            companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, fromDateParsed, toDateParsed, status
        //        );
        //        return Json(new { data = data.data, total = data.totalRecords });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching job list");
        //        return Json(new { success = false, message = "An error occurred" });
        //    }
        //}

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

        //List of Job Order
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
                var data = await _jobOrderService.GetPortExpensesListAsync(companyIdShort, parsedUserId.Value, jobOrderId);
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
                var data = await _jobOrderService.GetPortExpensesByIdAsync(companyIdShort, parsedUserId.Value, jobOrderId, portExpenseId);
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
                    PortExpenseId = model.portExpense.PortExpenseId, // Assuming this is being assigned elsewhere
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
                    Description = model.portExpense.Description?.Trim() ?? string.Empty,
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

                var result = await _jobOrderService.SavePortExpensesAsync(companyIdShort, parsedUserId.Value, portExpenseToSave);
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
                await _jobOrderService.DeletePortExpensesAsync(companyIdShort, parsedUserId.Value, jobOrderId, portExpenseId);
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

        #endregion Launch Services

        #region Equipment Used

        #endregion Equipment Used

        #region Crew Sign On

        #endregion Crew Sign On

        #region Crew Sign Off

        #endregion Crew Sign Off

        #region Crew Miscellaneous

        #endregion Crew Miscellaneous

        #region Medical Assistance

        #endregion Medical Assistance

        #region Consignment Import

        #endregion Consignment Import

        #region Consignment Export

        #endregion Consignment Export

        #region Third Party Supply

        #endregion Third Party Supply

        #region Fresh Water Supply

        #endregion Fresh Water Supply

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

        [HttpPost]
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
    }
}