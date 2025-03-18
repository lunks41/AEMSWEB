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
    public class AccountGroupController : BaseController
    {
        private readonly ILogger<AccountGroupController> _logger;
        private readonly IAccountGroupService _AccountGroupService;

        public AccountGroupController(ILogger<AccountGroupController> logger, IBaseService baseService, IAccountGroupService AccountGroupService) : base(logger, baseService)
        {
            _logger = logger;
            _AccountGroupService = AccountGroupService;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? companyId)
        {
            if (!companyId.HasValue || companyId == 0 || companyId < short.MinValue || companyId > short.MaxValue)
            {
                _logger.LogWarning("Invalid company ID: {CompanyId}", companyId);
                return Json(new { success = false, message = "Invalid company ID.", data = "" });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var permissions = await HasPermission((short)companyId, parsedUserId.Value, (short)E_Modules.Master, (short)E_Master.AccountGroup);

            ViewBag.IsRead = permissions?.IsRead ?? false;
            ViewBag.IsCreate = permissions?.IsCreate ?? false;
            ViewBag.IsEdit = permissions?.IsEdit ?? false;
            ViewBag.IsDelete = permissions?.IsDelete ?? false;
            ViewBag.IsExport = permissions?.IsExport ?? false;
            ViewBag.IsPrint = permissions?.IsPrint ?? false;
            ViewBag.CompanyId = companyId;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
                }

                var parsedUserId = GetParsedUserId();
                if (!parsedUserId.HasValue)
                {
                    _logger.LogWarning("User not logged in or invalid user ID.");
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _AccountGroupService.GetAccountGroupListAsync(companyIdShort, parsedUserId.Value, pageSize, pageNumber, searchString ?? string.Empty);

                var total = data.totalRecords;
                var paginatedData = data.data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Json(new { data = paginatedData, total = total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account groups.");
                return Json(new { Result = -1, Message = "An error occurred", Data = "" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(short accGroupId, string companyId)
        {
            if (accGroupId <= 0)
            {
                return Json(new { success = false, message = "Invalid Account Group ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                var data = await _AccountGroupService.GetAccountGroupByIdAsync(companyIdShort, parsedUserId.Value, accGroupId);

                if (data == null)
                {
                    return Json(new { success = false, message = "Account Group not found." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching account group by ID.");
                return Json(new { success = false, message = "An error occurred", data = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveAccountGroupViewModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("Save failed: Null model provided.");
                return Json(new { success = false, message = "Invalid request data." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Save failed: Invalid model state.");
                return Json(new { success = false, message = "Model validation failed." });
            }

            var accountGroup = model.AccountGroup;
            var companyId = model.CompanyId;

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                _logger.LogWarning("Save failed: Invalid company ID {CompanyId}.", companyId);
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("Save failed: User not logged in or invalid user ID.");
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            var permissions = await HasPermission(companyIdShort, parsedUserId.Value, (short)E_Modules.Master, (short)E_Master.AccountGroup);

            if (permissions == null || (!permissions.IsEdit && !permissions.IsCreate))
            {
                _logger.LogWarning("Save failed: User ID {UserId} does not have edit/create permissions.", parsedUserId.Value);
                return Forbid();
            }

            var accountGroupToSave = new M_AccountGroup
            {
                AccGroupId = accountGroup.AccGroupId,
                CompanyId = companyIdShort,
                AccGroupCode = accountGroup.AccGroupCode ?? string.Empty,
                AccGroupName = accountGroup.AccGroupName ?? string.Empty,
                SeqNo = accountGroup.SeqNo,
                Remarks = accountGroup.Remarks?.Trim() ?? string.Empty,
                IsActive = accountGroup.IsActive,
                CreateById = parsedUserId.Value,
                CreateDate = DateTime.UtcNow,
                EditById = accountGroup.EditById ?? 0,
                EditDate = DateTime.UtcNow
            };

            try
            {
                var data = await _AccountGroupService.SaveAccountGroupAsync(companyIdShort, parsedUserId.Value, accountGroupToSave);

                if (data == null)
                {
                    _logger.LogError("Save failed: Unable to save account group for Company ID {CompanyId}.", companyIdShort);
                    return Json(new { success = false, message = "Failed to save account group." });
                }

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the account group.");
                return Json(new { success = false, message = "An error occurred.", data = "" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(short accGroupId, string companyId)
        {
            if (accGroupId <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid Account Group ID {AccGroupId}.", accGroupId);
                return BadRequest(new { success = false, message = "Invalid Account Group ID." });
            }

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                _logger.LogWarning("Delete failed: Invalid Company ID {CompanyId}.", companyId);
                return BadRequest(new { success = false, message = "Invalid Company ID." });
            }

            var parsedUserId = GetParsedUserId();
            if (!parsedUserId.HasValue)
            {
                _logger.LogWarning("Delete failed: User not logged in or invalid User ID.");
                return Unauthorized(new { success = false, message = "User not logged in or invalid User ID." });
            }

            try
            {
                var permissions = await HasPermission(companyIdShort, parsedUserId.Value, (short)E_Modules.Master, (short)E_Master.AccountGroup);

                if (permissions == null || !permissions.IsDelete)
                {
                    _logger.LogWarning("Delete failed: User ID {UserId} does not have delete permissions.", parsedUserId.Value);
                    return Forbid();
                }

                var accountGroupget = await _AccountGroupService.GetAccountGroupByIdAsync(companyIdShort, parsedUserId.Value, accGroupId);

                if (accountGroupget == null)
                {
                    _logger.LogWarning("Delete failed: Account Group with ID {AccGroupId} not found for Company ID {CompanyId}.", accGroupId, companyIdShort);
                    return NotFound(new { success = false, message = "Account Group not found." });
                }

                var data = await _AccountGroupService.DeleteAccountGroupAsync(companyIdShort, parsedUserId.Value, accountGroupget);

                if (data == null)
                {
                    _logger.LogError("Delete failed: Unable to delete Account Group with ID {AccGroupId}.", accGroupId);
                    return StatusCode(500, new { success = false, message = "Failed to delete Account Group." });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Account Group. Account Group ID: {AccGroupId}, Company ID: {CompanyId}.", accGroupId, companyId);
                return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
            }
        }
    }
}