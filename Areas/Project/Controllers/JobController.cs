using AEMSWEB.Areas.Project.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Project.Controllers
{
    [Area("project")]
    [Authorize]
    public class JobController : BaseController
    {
        private readonly ILogger<JobController> _logger;
        private readonly IJobOrderService _customerService;

        public JobController(ILogger<JobController> logger,
            IBaseService baseService,
            IJobOrderService customerService)
            : base(logger, baseService)
        {
            _logger = logger;
            _customerService = customerService;
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

        [HttpGet]
        public async Task<JsonResult> JobOrderList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, string fromDate, string toDate, string status)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
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
                var data = await _customerService.GetJobOrderListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, fromDateParsed, toDateParsed, status);
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

        //    var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
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
        //        var data = await _customerService.GetJobOrderListAsync(
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

        [HttpPost]
        public async Task<JsonResult> GetJobStatusCounts(string searchString, string companyId, int customerId, string fromDate, string toDate)
        {
            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
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
                var counts = await _customerService.GetJobStatusCountsAsync(
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
    }
}