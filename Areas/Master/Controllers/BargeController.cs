using AEMSWEB.Areas.Master.Data.IServices;
using AEMSWEB.Entities.Masters;
using AEMSWEB.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AEMSWEB.Areas.Master.Controllers
{
    [Area("master")]
    [Authorize]
    public class BargeController : Controller
    {
        private readonly ILogger<BargeController> _logger;
        private readonly IBargeService _bargeService;

        public BargeController(ILogger<BargeController> logger, IBargeService bargeService)
        {
            _logger = logger;
            _bargeService = bargeService;
        }

        // GET: /master/Barge/Index
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

                var data = await _bargeService.GetBargeListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching barges.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/Barge/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(short bargeId, string companyId)
        {
            if (bargeId <= 0)
            {
                return Json(new { success = false, message = "Invalid Barge ID." });
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
                var data = await _bargeService.GetBargeByIdAsync(companyIdShort, parsedUserId, bargeId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Barge not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching barge by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/Barge/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveBargeViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var barge = model.Barge;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var bargeToSave = new M_Barge
            {
                BargeId = barge.BargeId,
                CompanyId = companyIdShort,
                BargeCode = barge.BargeCode ?? string.Empty,
                BargeName = barge.BargeName ?? string.Empty,
                CallSign = barge.CallSign ?? string.Empty,
                IMOCode = barge.IMOCode ?? string.Empty,
                GRT = barge.GRT ?? string.Empty,
                LicenseNo = barge.LicenseNo ?? string.Empty,
                BargeType = barge.BargeType ?? string.Empty,
                Flag = barge.Flag ?? string.Empty,
                Remarks = barge.Remarks?.Trim() ?? string.Empty,
                IsActive = barge.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = barge.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _bargeService.SaveBargeAsync(companyIdShort, parsedUserId, bargeToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save barge." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the barge.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/Barge/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short bargeId, string companyId)
        {
            if (bargeId <= 0)
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
                var bargeGet = await _bargeService.GetBargeByIdAsync(companyIdShort, parsedUserId, bargeId);

                var data = await _bargeService.DeleteBargeAsync(companyIdShort, 1, bargeGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save barge." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the barge.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}