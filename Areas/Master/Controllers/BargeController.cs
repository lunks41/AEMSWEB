using AMESWEB.Areas.Master.Data.IServices;
using AMESWEB.Controllers;
using AMESWEB.Entities.Masters;
using AMESWEB.Enums;
using AMESWEB.IServices;
using AMESWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMESWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class BargeController : BaseController
    {
        private readonly ILogger<BargeController> _logger;
        private readonly IBargeService _bargeService;

        public BargeController(ILogger<BargeController> logger,
            IBaseService baseService,
            IBargeService bargeService)
            : base(logger, baseService)
        {
            _logger = logger;
            _bargeService = bargeService;
        }

        #region Barge CRUD

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
                (short)E_Modules.Master, (short)E_Master.Barge);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> BargeList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _bargeService.GetBargeListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching barge list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetBargeById(short bargeId, string companyId)
        {
            if (bargeId <= 0)
                return Json(new { success = false, message = "Invalid Barge ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _bargeService.GetBargeByIdAsync(companyIdShort, parsedUserId.Value, bargeId);
                return data == null
                    ? Json(new { success = false, message = "Barge not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching barge by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveBarge([FromBody] SaveBargeViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var bargeToSave = new M_Barge
                {
                    BargeId = model.barge.BargeId,
                    CompanyId = companyIdShort,
                    BargeCode = model.barge.BargeCode ?? string.Empty,
                    BargeName = model.barge.BargeName ?? string.Empty,
                    CallSign = model.barge.CallSign ?? string.Empty,
                    IMOCode = model.barge.IMOCode ?? string.Empty,
                    GRT = model.barge.GRT ?? string.Empty,
                    LicenseNo = model.barge.LicenseNo ?? string.Empty,
                    BargeType = model.barge.BargeType ?? string.Empty,
                    Flag = model.barge.Flag ?? string.Empty,
                    Remarks = model.barge.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.barge.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.Now,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.Now
                };

                var result = await _bargeService.SaveBargeAsync(companyIdShort, parsedUserId.Value, bargeToSave);
                return Json(new { success = true, message = "Barge saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving barge");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBarge(short bargeId, string companyId)
        {
            if (bargeId <= 0)
                return Json(new { success = false, message = "Invalid Barge ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out byte companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Barge);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _bargeService.DeleteBargeAsync(companyIdShort, parsedUserId.Value, bargeId);
                return Json(new { success = true, message = "Barge deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting barge");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Barge CRUD
    }
}