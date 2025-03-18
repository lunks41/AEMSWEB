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
    public class TaskController : BaseController
    {
        private readonly ILogger<TaskController> _logger;
        private readonly ITaskService _taskService;

        public TaskController(ILogger<TaskController> logger, IBaseService baseService, ITaskService taskService)
            : base(logger, baseService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        #region Task CRUD

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
                (short)E_Modules.Master, (short)E_Master.Task);

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
                var data = await _taskService.GetTaskListAsync(companyIdShort, parsedUserId.Value,
                    pageSize, pageNumber, searchString ?? string.Empty);
                return Json(new { data = data.data, total = data.totalRecords });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task list");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int taskId, string companyId)
        {
            if (taskId <= 0)
                return Json(new { success = false, message = "Invalid Task ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var data = await _taskService.GetTaskByIdAsync(companyIdShort, parsedUserId.Value, taskId);
                return data == null
                    ? Json(new { success = false, message = "Task not found" })
                    : Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task by ID");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveTaskViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request data" });

            var validationResult = ValidateCompanyAndUserId(model.companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            try
            {
                var taskToSave = new M_Task
                {
                    TaskId = model.task.TaskId,
                    TaskCode = model.task.TaskCode ?? string.Empty,
                    TaskName = model.task.TaskName ?? string.Empty,
                    TaskOrder = model.task.TaskOrder,
                    Remarks = model.task.Remarks?.Trim() ?? string.Empty,
                    IsActive = model.task.IsActive,
                    CreateById = parsedUserId.Value,
                    CreateDate = DateTime.UtcNow,
                    EditById = model.task.EditById ?? 0,
                    EditDate = DateTime.UtcNow
                };

                var result = await _taskService.SaveTaskAsync(companyIdShort, parsedUserId.Value, taskToSave);
                return Json(new { success = true, message = "Task saved successfully", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving task");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int taskId, string companyId)
        {
            if (taskId <= 0)
                return Json(new { success = false, message = "Invalid Task ID" });

            var validationResult = ValidateCompanyAndUserId(companyId, out short companyIdShort, out short? parsedUserId);
            if (validationResult != null) return validationResult;

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value,
                (short)E_Modules.Master, (short)E_Master.Task);

            if (permissions == null || !permissions.IsDelete)
                return Json(new { success = false, message = "No delete permission" });

            try
            {
                var task = await _taskService.GetTaskByIdAsync(companyIdShort, parsedUserId.Value, taskId);
                if (task == null)
                    return Json(new { success = false, message = "Task not found" });

                await _taskService.DeleteTaskAsync(companyIdShort, parsedUserId.Value, task);
                return Json(new { success = true, message = "Task deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        #endregion Task CRUD
    }
}