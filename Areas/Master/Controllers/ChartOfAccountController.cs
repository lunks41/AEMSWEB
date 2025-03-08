using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    public class ChartOfAccountController : BaseController
    {
        private readonly ILogger<ChartOfAccountController> _logger;
        private readonly IChartOfAccountService _ChartOfAccountService;

        public ChartOfAccountController(ILogger<ChartOfAccountController> logger, IBaseService baseService, IChartOfAccountService ChartOfAccountService)
            : base(logger, baseService)
        {
            _logger = logger;
            _ChartOfAccountService = ChartOfAccountService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [Route("ChartOfAccount/List")]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
                }

                //var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                //if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                //{
                //    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                //}

                var data = await _ChartOfAccountService.GetChartOfAccountListAsync(companyIdShort, 1, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total = total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account groups.");
                return Json(new { Result = -1, Message = "An error occurred", Data = "" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(Int16 GLId, string companyId)
        {
            // Validate GLId
            if (GLId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Group ID." });
            }

            // Validate input parameters
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            //var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            //{
            //    return Json(new { success = false, message = "User not logged in or invalid user ID." });
            //}

            try
            {
                // Assuming you would have some logic here to use the headers in your service call
                var data = await _ChartOfAccountService.GetChartOfAccountByIdAsync(companyIdShort, 1, GLId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Account Group not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account group by ID.");
                return Json(new { success = false, message = "An error occurred", data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveChartOfAccountViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var chartofaccount = model.ChartOfAccount;
            var companyId = model.CompanyId;

            // Validate input parameters
            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            //var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            //{
            //    return Json(new { success = false, message = "User not logged in or invalid user ID." });
            //}

            var chartOfAccountToSave = new M_ChartOfAccount
            {
                GLId = chartofaccount.GLId,
                CompanyId = Convert.ToInt16(HttpContext.Session.GetInt32("selectedCompany")),
                GLCode = chartofaccount.GLCode ?? string.Empty,
                GLName = chartofaccount.GLName ?? string.Empty,
                AccTypeId = chartofaccount.AccTypeId,
                AccGroupId = chartofaccount.AccGroupId,
                COACategoryId1 = chartofaccount.COACategoryId1,
                COACategoryId2 = chartofaccount.COACategoryId2,
                COACategoryId3 = chartofaccount.COACategoryId3,
                IsSysControl = chartofaccount.IsSysControl,
                SeqNo = chartofaccount.SeqNo,
                Remarks = chartofaccount.Remarks?.Trim() ?? string.Empty,
                IsActive = chartofaccount.IsActive,
                CreateById = chartofaccount.CreateById ?? 0,
                CreateDate = DateTime.Now,
                EditById = chartofaccount.EditById ?? 0,
                EditDate = DateTime.Now,
            };

            try
            {
                var data = await _ChartOfAccountService.SaveChartOfAccountAsync(companyIdShort, 1, chartOfAccountToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        public async Task<IActionResult> Remove(short GLId, string companyId)
        {
            if (GLId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            //var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            //{
            //    return Json(new { success = false, message = "User not logged in or invalid user ID." });
            //}

            try
            {
                var chartOfAccountget = await _ChartOfAccountService.GetChartOfAccountByIdAsync(companyIdShort, 1, GLId);

                var data = await _ChartOfAccountService.DeleteChartOfAccountAsync(companyIdShort, 1, GLId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }
    }
}