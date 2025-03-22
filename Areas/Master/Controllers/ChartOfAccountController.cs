using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Areas.Master.Models;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class ChartOfAccountController : BaseController
    {
        private readonly ILogger<ChartOfAccountController> _logger;
        private readonly IChartOfAccountService _chartOfAccountService;

        public ChartOfAccountController(ILogger<ChartOfAccountController> logger,
            IBaseService baseService,
            IChartOfAccountService chartOfAccountService)
            : base(logger, baseService)
        {
            _logger = logger;
            _chartOfAccountService = chartOfAccountService;
        }

        #region ChartOfAccount CRUD

        [Authorize]
        public async Task<IActionResult> Index(int? companyId)
        {
            if (!companyId.HasValue || companyId <= 0)
            {
                _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var permissions = await HasPermission((short)companyId, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.ChartOfAccount);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _chartOfAccountService.GetChartOfAccountListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching chart of account list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short glId, string companyId)
        {
            if (glId <= 0)
                return Json(new { success = false, message = "Invalid GL ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _chartOfAccountService.GetChartOfAccountByIdAsync(companyIdShort, parsedUserId.Value, glId);
                return data == null
                    ? Json(new { success = false, message = "Chart of Account not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching chart of account by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveChartOfAccountViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var chartToSave = new M_ChartOfAccount
                {
                    GLId = model.chartOfAccount.GLId,
                    CompanyId = companyIdShort,
                    GLCode = model.chartOfAccount.GLCode ?? string.Empty,
                    GLName = model.chartOfAccount.GLName ?? string.Empty,
                    AccTypeId = model.chartOfAccount.AccTypeId,
                    AccGroupId = model.chartOfAccount.AccGroupId,
                    COACategoryId1 = model.chartOfAccount.COACategoryId1,
                    COACategoryId2 = model.chartOfAccount.COACategoryId2,
                    COACategoryId3 = model.chartOfAccount.COACategoryId3,
                    IsSysControl = model.chartOfAccount.IsSysControl,
                    SeqNo = model.chartOfAccount.SeqNo,
                    Remarks = model.chartOfAccount.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.chartOfAccount.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _chartOfAccountService.SaveChartOfAccountAsync(companyIdShort, parsedUserId.Value, chartToSave);
                return Json(new { success = true, message = "Chart of Account saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving chart of account");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short glId, string companyId)
        {
            if (glId <= 0)
                return Json(new { success = false, message = "Invalid GL ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.ChartOfAccount);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var chart = await _chartOfAccountService.GetChartOfAccountByIdAsync(companyIdShort, parsedUserId.Value, glId);
                if (chart == null)
                    return Json(new { success = false, message = "Chart of Account not found" });

                await _chartOfAccountService.DeleteChartOfAccountAsync(companyIdShort, parsedUserId.Value, glId);
                return Json(new { success = true, message = "Chart of Account deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting chart of account");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion ChartOfAccount CRUD
    }
}