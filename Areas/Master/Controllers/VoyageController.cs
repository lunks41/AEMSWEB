using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Enums;
using AEMSWEB.IServices;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class VoyageController : BaseController
    {
        private readonly ILogger<VoyageController> _logger;
        private readonly IVoyageService _voyageService;

        public VoyageController(ILogger<VoyageController> logger, IBaseService baseService, IVoyageService voyageService)
            : base(logger, baseService)
        {
            _logger = logger;
            _voyageService = voyageService;
        }

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
                (short)E_Modules.Master, (short)E_Master.Voyage);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        #region Voyage

        [HttpGet]
        public async Task<JsonResult> VoyageList(int pageNumber, int pageSize, string searchString, string companyId)
        {
            if (pageNumber < 1 || pageSize < 1)
                return Json(new { success = false, message = "Invalid page parameters" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _voyageService.GetVoyageListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching voyage list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetVoyageById(short voyageId, string companyId)
        {
            if (voyageId <= 0)
                return Json(new { success = false, message = "Invalid Voyage ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _voyageService.GetVoyageByIdAsync(companyIdShort, parsedUserId.Value, voyageId);
                return data == null
                    ? Json(new { success = false, message = "Voyage not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching voyage by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveVoyage([FromBody] SaveVoyageViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var voyageToSave = new M_Voyage
                {
                    VoyageId = model.voyage.VoyageId,
                    CompanyId = companyIdShort,
                    VoyageNo = model.voyage.VoyageNo ?? string.Empty,
                    ReferenceNo = model.voyage.ReferenceNo ?? string.Empty,
                    VesselId = model.voyage.VesselId,
                    BargeId = model.voyage.BargeId,
                    Remarks = model.voyage.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.voyage.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.UtcNow
                };

                var result = await _voyageService.SaveVoyageAsync(companyIdShort, parsedUserId.Value, voyageToSave);
                return Json(new { success = true, message = "Voyage saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving voyage");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteVoyage(short voyageId, string companyId)
        {
            if (voyageId <= 0)
                return Json(new { success = false, message = "Invalid Voyage ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Voyage);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                await _voyageService.DeleteVoyageAsync(companyIdShort, parsedUserId.Value, voyageId);
                return Json(new { success = true, message = "Voyage deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting voyage");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion
    }
}