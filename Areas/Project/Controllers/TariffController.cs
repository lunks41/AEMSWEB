using AEMSWEB.Areas.Project.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Project.Controllers
{
    //Rates of customer task wise
    [Area("project")]
    public class TariffController : BaseController
    {
        private readonly ILogger<TariffController> _logger;
        private readonly ITariffService _customerService;

        public TariffController(ILogger<TariffController> logger,
            IBaseService baseService,
            ITariffService customerService)
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
            //    (short)E_Modules.Project, (short)E_Project.Tariff);

            ViewBag.IsRead = true;
            ViewBag.IsCreate = true;
            ViewBag.IsEdit = true;
            ViewBag.IsDelete = true;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> TariffList(int pageNumber, int pageSize, string searchString, string companyId, int customerId, string fromDate, string toDate, string status)
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
                var data = await _customerService.GetTariffListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty, customerId, fromDateParsed, toDateParsed, status);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }
    }
}