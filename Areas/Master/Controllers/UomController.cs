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
    public class UomController : Controller
    {
        private readonly ILogger<UomController> _logger;
        private readonly IUomService _uomService;

        public UomController(ILogger<UomController> logger, IUomService uomService)
        {
            _logger = logger;
            _uomService = uomService;
        }

        // GET: /master/Uom/Index
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

                var data = await _uomService.GetUomListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching UOMs.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/Uom/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(short uomId, string companyId)
        {
            if (uomId <= 0)
            {
                return Json(new { success = false, message = "Invalid UOM ID." });
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
                var data = await _uomService.GetUomByIdAsync(companyIdShort, parsedUserId, uomId);

                if (data == null)
                {
                    return Json(new { success = false, message = "UOM not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching UOM by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/Uom/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveUomViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var uom = model.Uom;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var uomToSave = new M_Uom
            {
                UomId = uom.UomId,
                CompanyId = companyIdShort,
                UomCode = uom.UomCode ?? string.Empty,
                UomName = uom.UomName ?? string.Empty,
                Remarks = uom.Remarks?.Trim() ?? string.Empty,
                IsActive = uom.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = uom.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _uomService.SaveUomAsync(companyIdShort, parsedUserId, uomToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save UOM." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the UOM.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/Uom/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short uomId, string companyId)
        {
            if (uomId <= 0)
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
                var uomGet = await _uomService.GetUomByIdAsync(companyIdShort, parsedUserId, uomId);

                var data = await _uomService.DeleteUomAsync(companyIdShort, 1, uomGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save UOM." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the UOM.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}