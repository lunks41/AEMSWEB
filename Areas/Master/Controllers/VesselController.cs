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
    public class VesselController : BaseController
    {
        private readonly ILogger<VesselController> _logger;
        private readonly IVesselService _vesselService;

        public VesselController(ILogger<VesselController> logger, IBaseService baseService, IVesselService vesselService)
            : base(logger, baseService)
        {
            _logger = logger;
            _vesselService = vesselService;
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
                (short)E_Modules.Master, (short)E_Master.Vessel);

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
                var data = await _vesselService.GetVesselListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vessel list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int vesselId, string companyId)
        {
            if (vesselId <= 0)
                return Json(new { success = false, message = "Invalid Vessel ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _vesselService.GetVesselByIdAsync(companyIdShort, parsedUserId.Value, vesselId);
                return data == null
                    ? Json(new { success = false, message = "Vessel not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vessel by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveVesselViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var vesselToSave = new M_Vessel
                {
                    VesselId = model.vessel.VesselId,
                    CompanyId = companyIdShort,
                    VesselCode = model.vessel.VesselCode ?? string.Empty,
                    VesselName = model.vessel.VesselName ?? string.Empty,
                    CallSign = model.vessel.CallSign ?? string.Empty,
                    IMOCode = model.vessel.IMOCode ?? string.Empty,
                    GRT = model.vessel.GRT ?? string.Empty,
                    LicenseNo = model.vessel.LicenseNo ?? string.Empty,
                    VesselType = model.vessel.VesselType ?? string.Empty,
                    Flag = model.vessel.Flag ?? string.Empty,
                    Remarks = model.vessel.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.vessel.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = parsedUserId.Value,
                    EditDate = DateTime.UtcNow
                };

                var result = await _vesselService.SaveVesselAsync(companyIdShort, parsedUserId.Value, vesselToSave);
                return Json(new { success = true, message = "Vessel saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving vessel");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int vesselId, string companyId)
        {
            if (vesselId <= 0)
                return Json(new { success = false, message = "Invalid Vessel ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Vessel);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var vessel = await _vesselService.GetVesselByIdAsync(companyIdShort, parsedUserId.Value, vesselId);
                if (vessel == null)
                    return Json(new { success = false, message = "Vessel not found" });

                await _vesselService.DeleteVesselAsync(companyIdShort, parsedUserId.Value, vessel);
                return Json(new { success = true, message = "Vessel deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vessel");
                return Json(new { success = false, message = "An error occurred" });
            }
        }
    }
}