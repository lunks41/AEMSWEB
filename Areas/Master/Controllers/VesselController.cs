using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Controllers;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models.Masters;
using AEMSWEB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class VesselController : Controller
    {
        private readonly ILogger<VesselController> _logger;
        private readonly IVesselService _vesselService;

        public VesselController(ILogger<VesselController> logger, IVesselService vesselService)
        {
            _logger = logger;
            _vesselService = vesselService;
        }

        // GET: /master/Vessel/Index
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("List")]
        public async Task<JsonResult> List(short pageNumber, short pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _vesselService.GetVesselListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching vessels.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/Vessel/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(int vesselId, string companyId)
        {
            if (vesselId <= 0)
            {
                return Json(new { success = false, message = "Invalid Vessel ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID" });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var data = await _vesselService.GetVesselByIdAsync(companyIdShort, parsedUserId, vesselId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Vessel not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching vessel by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/Vessel/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveVesselViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var vessel = model.Vessel;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var vesselToSave = new M_Vessel
            {
                VesselId = vessel.VesselId,
                CompanyId = companyIdShort,
                VesselCode = vessel.VesselCode ?? string.Empty,
                VesselName = vessel.VesselName ?? string.Empty,
                CallSign = vessel.CallSign ?? string.Empty,
                IMOCode = vessel.IMOCode ?? string.Empty,
                GRT = vessel.GRT ?? string.Empty,
                LicenseNo = vessel.LicenseNo ?? string.Empty,
                VesselType = vessel.VesselType ?? string.Empty,
                Flag = vessel.Flag ?? string.Empty,
                Remarks = vessel.Remarks?.Trim() ?? string.Empty,
                IsActive = vessel.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = vessel.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _vesselService.SaveVesselAsync(companyIdShort, parsedUserId, vesselToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save vessel." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the vessel.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/Vessel/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(int vesselId, string companyId)
        {
            if (vesselId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var vesselGet = await _vesselService.GetVesselByIdAsync(companyIdShort, parsedUserId, vesselId);

                var data = await _vesselService.DeleteVesselAsync(companyIdShort, 1, vesselGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save vessel." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the vessel.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}