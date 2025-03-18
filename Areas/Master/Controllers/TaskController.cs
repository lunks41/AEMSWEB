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
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly ITaskService _vesselService;

        public TaskController(ILogger<TaskController> logger, ITaskService vesselService)
        {
            _logger = logger;
            _vesselService = vesselService;
        }

        // GET: /master/Task/Index
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

                var data = await _vesselService.GetTaskListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

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

        // GET: /master/Task/GetById
        [HttpGet]
        public async Task<JsonResult> GetById(int vesselId, string companyId)
        {
            if (vesselId <= 0)
            {
                return Json(new { success = false, message = "Invalid Task ID." });
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
                var data = await _vesselService.GetTaskByIdAsync(companyIdShort, parsedUserId, vesselId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Task not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching vessel by ID.");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // POST: /master/Task/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveTaskViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var vessel = model.Task;

            if (string.IsNullOrEmpty(model.CompanyId) || !short.TryParse(model.CompanyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var vesselToSave = new M_Task
            {
                TaskId = vessel.TaskId,
                TaskCode = vessel.TaskCode ?? string.Empty,
                TaskName = vessel.TaskName ?? string.Empty,
                TaskOrder = vessel.TaskOrder,
                Remarks = vessel.Remarks?.Trim() ?? string.Empty,
                IsActive = vessel.IsActive,
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = vessel.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _vesselService.SaveTaskAsync(companyIdShort, parsedUserId, vesselToSave);

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

        // DELETE: /master/Task/Delete
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
                var vesselGet = await _vesselService.GetTaskByIdAsync(companyIdShort, parsedUserId, vesselId);

                var data = await _vesselService.DeleteTaskAsync(companyIdShort, 1, vesselGet);

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