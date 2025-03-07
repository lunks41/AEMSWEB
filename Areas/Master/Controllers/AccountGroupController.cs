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
    public class AccountGroupController : Controller
    {
        private readonly ILogger<AccountGroupController> _logger;
        private readonly IAccountGroupService _AccountGroupService;

        public AccountGroupController(ILogger<AccountGroupController> logger, IAccountGroupService AccountGroupService)

        {
            _logger = logger;
            _AccountGroupService = AccountGroupService;
        }

        // GET: /master/AccountGroup/Index
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet("List")]
        public async Task<JsonResult> List(int pageNumber, int pageSize, string searchString, string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
                {
                    return Json(new { Result = -1, Message = "Invalid company ID", Data = "" });
                }

                var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
                {
                    return Json(new { success = false, message = "User not logged in or invalid user ID." });
                }

                var data = await _AccountGroupService.GetAccountGroupListAsync(companyIdShort, parsedUserId, pageSize, pageNumber, searchString ?? string.Empty);

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

        // GET: /master/AccountGroup/GetById
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

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
            }

            try
            {
                // Assuming you would have some logic here to use the headers in your service call
                var data = await _AccountGroupService.GetAccountGroupByIdAsync(companyIdShort, parsedUserId, accGroupId);

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

        // POST: /master/AccountGroup/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveAccountGroupViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Data operation failed due to null model." });
            }

            var accountGroup = model.AccountGroup;
            var companyId = model.CompanyId;

            if (string.IsNullOrEmpty(companyId) || !short.TryParse(companyId, out short companyIdShort))
            {
                return Json(new { success = false, message = "Invalid company ID." });
            }

            var userId = HttpContext.Session.GetString("UserId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short parsedUserId))
            {
                return Json(new { success = false, message = "User not logged in or invalid user ID." });
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
                CreateById = parsedUserId,
                CreateDate = DateTime.Now,
                EditById = accountGroup.EditById ?? 0,
                EditDate = DateTime.Now
            };

            try
            {
                var data = await _AccountGroupService.SaveAccountGroupAsync(companyIdShort, parsedUserId, accountGroupToSave);

                if (data == null)
                {
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

        // DELETE: /master/AccountGroup/Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(short accGroupId, string companyId)
        {
            if (accGroupId <= 0)
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
                var accountGroupget = await _AccountGroupService.GetAccountGroupByIdAsync(companyIdShort, parsedUserId, accGroupId);

                var data = await _AccountGroupService.DeleteAccountGroupAsync(companyIdShort, 1, accountGroupget);

                if (data == null)
                {
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
    }
}