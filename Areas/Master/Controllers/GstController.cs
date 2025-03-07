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
    public class GstController : Controller
    {
        private readonly ILogger<GstController> _logger;
        private readonly IGstService _gstService;

        public GstController(ILogger<GstController> logger, IGstService gstService)
        {
            _logger = logger;
            _gstService = gstService;
        }

        // GET: /master/Gst/Index
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

                var data = await _gstService.GetGstListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching GSTs.");
                return Json(new { Result = -1, Message = "An error occurred" });
            }
        }

        // GET: /master/Gst/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(short gstId, string companyId)
        {
            if (gstId <= 0)
            {
                return Json(new { success = false, message = "Invalid GST ID." });
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
                var data = await _gstService.GetGstByIdAsync(companyIdShort, parsedUserId, gstId);

                if (data == null)
                {
                    return Json(new { success = false, message = "GST not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching GST by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/Gst/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveGstViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var gst = model.Gst;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var gstToSave = new M_Gst
            {
                GstId = gst.GstId,
                CompanyId = companyIdShort,
                GstCategoryId = gst.GstCategoryId,
                GstCode = gst.GstCode ?? string.Empty,
                GstName = gst.GstName ?? string.Empty,
                Remarks = gst.Remarks?.Trim() ?? string.Empty,
                IsActive = gst.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = gst.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _gstService.SaveGstAsync(companyIdShort, parsedUserId, gstToSave);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save GST." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the GST.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        // DELETE: /master/Gst/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short gstId, string companyId)
        {
            if (gstId <= 0)
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
                var gstGet = await _gstService.GetGstByIdAsync(companyIdShort, parsedUserId, gstId);

                var data = await _gstService.DeleteGstAsync(companyIdShort, 1, gstGet);

                if (data == null)
                {
                    return Json(new { success = false, message = "Failed to save GST." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the GST.");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}